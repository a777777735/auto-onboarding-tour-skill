using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace AutoOnboardingTour.Wpf
{
    public enum TourPlacement
    {
        Top,
        Bottom,
        Left,
        Right
    }

    /// <summary>
    /// 定義單一步驟配置
    /// </summary>
    public class WpfTourStep
    {
        /// <summary>
        /// 目標控制項：支援控制項名稱 (string) 或 FrameworkElement 實體
        /// </summary>
        public object Target { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public TourPlacement Placement { get; set; } = TourPlacement.Bottom;

        /// <summary>
        /// 在顯示此步驟前執行的委派（例如切換 TabControl.SelectedIndex、展開 Expander 等）
        /// </summary>
        public Func<Task> BeforeShowAsync { get; set; }

        public Action BeforeShow
        {
            set
            {
                if (value != null)
                {
                    BeforeShowAsync = () =>
                    {
                        value();
                        return Task.CompletedTask;
                    };
                }
            }
        }
    }

    /// <summary>
    /// WPF 零依賴純原生聚光燈導覽引擎
    /// 使用 AdornerLayer 與 CombinedGeometry 實作動態挖洞遮罩與智慧卡片定位
    /// </summary>
    public class WpfTourManager
    {
        private readonly Window _window;
        private readonly List<WpfTourStep> _steps = new List<WpfTourStep>();
        private TourAdorner _currentAdorner;
        private int _currentIndex = -1;

        public string StorageKey { get; set; } = "wpf_tour_completed";
        public double SpotlightPadding { get; set; } = 8.0;
        public IReadOnlyList<WpfTourStep> Steps => _steps;
        public bool IsActive => _currentAdorner != null;

        public event EventHandler TourCompleted;
        public event EventHandler TourSkipped;
        public event EventHandler<int> StepChanged;

        public WpfTourManager(Window window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _window.SizeChanged += (s, e) => _currentAdorner?.InvalidateLayoutAndRender();
            _window.LocationChanged += (s, e) => _currentAdorner?.InvalidateLayoutAndRender();
            _window.PreviewKeyDown += OnWindowKeyDown;
        }

        public WpfTourManager AddStep(WpfTourStep step)
        {
            if (step != null) _steps.Add(step);
            return this;
        }

        public WpfTourManager AddStep(string targetName, string title, string content, TourPlacement placement = TourPlacement.Bottom, Action beforeShow = null)
        {
            var step = new WpfTourStep
            {
                Target = targetName,
                Title = title,
                Content = content,
                Placement = placement,
                BeforeShow = beforeShow
            };
            _steps.Add(step);
            return this;
        }

        /// <summary>
        /// 首次打開時自動執行（以本機應用設定檔目錄儲存旗標）
        /// </summary>
        public void StartIfFirstTime()
        {
            string flagFile = GetStorageFilePath();
            if (!File.Exists(flagFile))
            {
                Start(0);
            }
        }

        private string GetStorageFilePath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dir = Path.Combine(appData, "AutoOnboardingTour");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return Path.Combine(dir, $"{StorageKey}.flag");
        }

        public void Start(int startIndex = 0)
        {
            if (_steps.Count == 0) return;

            var contentVisual = _window.Content as UIElement;
            if (contentVisual == null) return;

            var layer = AdornerLayer.GetAdornerLayer(contentVisual);
            if (layer == null) return;

            if (_currentAdorner != null)
            {
                layer.Remove(_currentAdorner);
            }

            _currentAdorner = new TourAdorner(contentVisual, this);
            layer.Add(_currentAdorner);

            GoToStep(startIndex);
        }

        public async void GoToStep(int index)
        {
            if (index < 0 || index >= _steps.Count) return;

            _currentIndex = index;
            var step = _steps[index];

            if (step.BeforeShowAsync != null)
            {
                try
                {
                    await step.BeforeShowAsync();
                    await Task.Delay(80); // 等待視覺樹刷新
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[WpfTour] BeforeShow error: {ex.Message}");
                }
            }

            _currentAdorner?.UpdateStep(step, _currentIndex, _steps.Count);
            StepChanged?.Invoke(this, _currentIndex);
        }

        public void Next()
        {
            if (_currentIndex < _steps.Count - 1)
            {
                GoToStep(_currentIndex + 1);
            }
            else
            {
                Complete();
            }
        }

        public void Prev()
        {
            if (_currentIndex > 0)
            {
                GoToStep(_currentIndex - 1);
            }
        }

        public void Complete()
        {
            SaveCompletionFlag();
            Stop();
            TourCompleted?.Invoke(this, EventArgs.Empty);
        }

        public void Skip()
        {
            SaveCompletionFlag();
            Stop();
            TourSkipped?.Invoke(this, EventArgs.Empty);
        }

        public void Stop()
        {
            if (_currentAdorner != null)
            {
                var contentVisual = _window.Content as UIElement;
                var layer = AdornerLayer.GetAdornerLayer(contentVisual);
                layer?.Remove(_currentAdorner);
                _currentAdorner = null;
            }
            _currentIndex = -1;
        }

        private void SaveCompletionFlag()
        {
            try
            {
                File.WriteAllText(GetStorageFilePath(), DateTime.UtcNow.ToString("O"));
            }
            catch { }
        }

        private void OnWindowKeyDown(object sender, KeyEventArgs e)
        {
            if (!IsActive) return;

            if (e.Key == Key.Right)
            {
                Next();
                e.Handled = true;
            }
            else if (e.Key == Key.Left)
            {
                Prev();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                Skip();
                e.Handled = true;
            }
        }

        internal FrameworkElement FindTargetElement(object target)
        {
            if (target is FrameworkElement fe) return fe;
            if (target is string name && !string.IsNullOrWhiteSpace(name))
            {
                return _window.FindName(name) as FrameworkElement;
            }
            return null;
        }

        /// <summary>
        /// 裝飾層：負責繪製 CombinedGeometry 挖洞遮罩與承載教學卡片
        /// </summary>
        private class TourAdorner : Adorner
        {
            private readonly WpfTourManager _manager;
            private readonly Border _cardContainer;
            private readonly VisualCollection _visualChildren;
            private Rect _targetHoleRect;
            private FrameworkElement _currentTarget;
            private WpfTourStep _currentStep;

            public TourAdorner(UIElement adornedElement, WpfTourManager manager) : base(adornedElement)
            {
                _manager = manager;
                _visualChildren = new VisualCollection(this);

                _cardContainer = BuildCardUI();
                _visualChildren.Add(_cardContainer);

                IsHitTestVisible = true;
                Focusable = false;
            }

            protected override int VisualChildrenCount => _visualChildren.Count;
            protected override Visual GetVisualChild(int index) => _visualChildren[index];

            private Border BuildCardUI()
            {
                var border = new Border
                {
                    Background = Brushes.White,
                    BorderBrush = new SolidColorBrush(Color.FromArgb(230, 226, 232, 240)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(10),
                    Width = 320,
                    Padding = new Thickness(16),
                    Effect = new System.Windows.Media.Effects.DropShadowEffect
                    {
                        BlurRadius = 24,
                        ShadowDepth = 6,
                        Direction = 270,
                        Opacity = 0.22,
                        Color = Colors.Black
                    }
                };

                return border;
            }

            public void UpdateStep(WpfTourStep step, int index, int total)
            {
                _currentStep = step;
                _currentTarget = _manager.FindTargetElement(step.Target);

                // 更新教學卡片內容
                var stack = new StackPanel();

                // 標題列 + 關閉按鈕
                var headerGrid = new Grid();
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var txtTitle = new TextBlock
                {
                    Text = step.Title,
                    FontWeight = FontWeights.Bold,
                    FontSize = 15,
                    Foreground = new SolidColorBrush(Color.FromRgb(15, 23, 42)),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(txtTitle, 0);

                var btnClose = new Button
                {
                    Content = "✕",
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Foreground = new SolidColorBrush(Color.FromRgb(148, 163, 184)),
                    FontWeight = FontWeights.Bold,
                    Cursor = Cursors.Hand,
                    Padding = new Thickness(4, 0, 4, 0)
                };
                btnClose.Click += (s, e) => _manager.Skip();
                Grid.SetColumn(btnClose, 1);

                headerGrid.Children.Add(txtTitle);
                headerGrid.Children.Add(btnClose);
                stack.Children.Add(headerGrid);

                // 進度條
                var progressWrap = new Border
                {
                    Height = 4,
                    Background = new SolidColorBrush(Color.FromRgb(226, 232, 240)),
                    CornerRadius = new CornerRadius(2),
                    Margin = new Thickness(0, 10, 0, 10)
                };
                var progressFill = new Border
                {
                    Height = 4,
                    Background = new SolidColorBrush(Color.FromRgb(37, 99, 235)),
                    CornerRadius = new CornerRadius(2),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 320 * ((double)(index + 1) / total)
                };
                progressWrap.Child = progressFill;
                stack.Children.Add(progressWrap);

                // 說明文字
                var txtContent = new TextBlock
                {
                    Text = step.Content,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 13.5,
                    LineHeight = 20,
                    Foreground = new SolidColorBrush(Color.FromRgb(71, 85, 105)),
                    Margin = new Thickness(0, 0, 0, 16)
                };
                stack.Children.Add(txtContent);

                // 底部按鈕區
                var footerGrid = new Grid();
                footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var txtCounter = new TextBlock
                {
                    Text = $"{index + 1} / {total}",
                    FontSize = 12,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Color.FromRgb(100, 116, 139)),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(txtCounter, 0);

                var btnStack = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Right
                };

                var btnSkip = new Button
                {
                    Content = "跳過",
                    Margin = new Thickness(0, 0, 6, 0),
                    Padding = new Thickness(10, 5, 10, 5),
                    Background = Brushes.Transparent,
                    BorderThickness = new Thickness(0),
                    Foreground = new SolidColorBrush(Color.FromRgb(100, 116, 139)),
                    Cursor = Cursors.Hand
                };
                btnSkip.Click += (s, e) => _manager.Skip();
                btnStack.Children.Add(btnSkip);

                if (index > 0)
                {
                    var btnPrev = new Button
                    {
                        Content = "上一步",
                        Margin = new Thickness(0, 0, 6, 0),
                        Padding = new Thickness(10, 5, 10, 5),
                        Background = new SolidColorBrush(Color.FromRgb(241, 245, 249)),
                        BorderBrush = new SolidColorBrush(Color.FromRgb(226, 232, 240)),
                        Foreground = new SolidColorBrush(Color.FromRgb(51, 65, 85)),
                        Cursor = Cursors.Hand
                    };
                    btnPrev.Click += (s, e) => _manager.Prev();
                    btnStack.Children.Add(btnPrev);
                }

                bool isLast = index == total - 1;
                var btnNext = new Button
                {
                    Content = isLast ? "完成" : "下一步",
                    Padding = new Thickness(12, 5, 12, 5),
                    Background = new SolidColorBrush(Color.FromRgb(37, 99, 235)),
                    Foreground = Brushes.White,
                    BorderThickness = new Thickness(0),
                    FontWeight = FontWeights.SemiBold,
                    Cursor = Cursors.Hand
                };
                btnNext.Click += (s, e) => _manager.Next();
                btnStack.Children.Add(btnNext);

                Grid.SetColumn(btnStack, 1);
                footerGrid.Children.Add(txtCounter);
                footerGrid.Children.Add(btnStack);
                stack.Children.Add(footerGrid);

                _cardContainer.Child = stack;

                InvalidateLayoutAndRender();
            }

            public void InvalidateLayoutAndRender()
            {
                CalculateHoleRect();
                InvalidateArrange();
                InvalidateVisual();
            }

            private void CalculateHoleRect()
            {
                if (_currentTarget == null || !_currentTarget.IsVisible)
                {
                    _targetHoleRect = Rect.Empty;
                    return;
                }

                try
                {
                    var transform = _currentTarget.TransformToAncestor(AdornedElement);
                    var bounds = new Rect(0, 0, _currentTarget.ActualWidth, _currentTarget.ActualHeight);
                    var rectInAdorned = transform.TransformBounds(bounds);

                    double pad = _manager.SpotlightPadding;
                    _targetHoleRect = new Rect(
                        Math.Max(0, rectInAdorned.Left - pad),
                        Math.Max(0, rectInAdorned.Top - pad),
                        rectInAdorned.Width + pad * 2,
                        rectInAdorned.Height + pad * 2
                    );
                }
                catch
                {
                    _targetHoleRect = Rect.Empty;
                }
            }

            protected override Size MeasureOverride(Size constraint)
            {
                _cardContainer.Measure(constraint);
                return base.MeasureOverride(constraint);
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                CalculateHoleRect();

                double cardWidth = _cardContainer.DesiredSize.Width > 0 ? _cardContainer.DesiredSize.Width : 320;
                double cardHeight = _cardContainer.DesiredSize.Height > 0 ? _cardContainer.DesiredSize.Height : 200;

                double x = 20;
                double y = 20;
                double margin = 14;

                if (!_targetHoleRect.IsEmpty)
                {
                    var placement = _currentStep?.Placement ?? TourPlacement.Bottom;

                    // 自動防溢出
                    if (placement == TourPlacement.Bottom && _targetHoleRect.Bottom + margin + cardHeight > finalSize.Height)
                        placement = TourPlacement.Top;
                    else if (placement == TourPlacement.Top && _targetHoleRect.Top - margin - cardHeight < 0)
                        placement = TourPlacement.Bottom;

                    switch (placement)
                    {
                        case TourPlacement.Top:
                            y = _targetHoleRect.Top - cardHeight - margin;
                            x = _targetHoleRect.Left + (_targetHoleRect.Width - cardWidth) / 2;
                            break;
                        case TourPlacement.Bottom:
                            y = _targetHoleRect.Bottom + margin;
                            x = _targetHoleRect.Left + (_targetHoleRect.Width - cardWidth) / 2;
                            break;
                        case TourPlacement.Left:
                            x = _targetHoleRect.Left - cardWidth - margin;
                            y = _targetHoleRect.Top + (_targetHoleRect.Height - cardHeight) / 2;
                            break;
                        case TourPlacement.Right:
                            x = _targetHoleRect.Right + margin;
                            y = _targetHoleRect.Top + (_targetHoleRect.Height - cardHeight) / 2;
                            break;
                    }

                    // Clamping
                    x = Math.Max(12, Math.Min(x, finalSize.Width - cardWidth - 12));
                    y = Math.Max(12, Math.Min(y, finalSize.Height - cardHeight - 12));
                }
                else
                {
                    // 置中顯示
                    x = (finalSize.Width - cardWidth) / 2;
                    y = (finalSize.Height - cardHeight) / 2;
                }

                _cardContainer.Arrange(new Rect(x, y, cardWidth, cardHeight));
                return finalSize;
            }

            protected override void OnRender(DrawingContext drawingContext)
            {
                base.OnRender(drawingContext);

                var totalRect = new Rect(0, 0, ActualWidth, ActualHeight);
                var fullGeometry = new RectangleGeometry(totalRect);

                Geometry maskGeometry = fullGeometry;

                if (!_targetHoleRect.IsEmpty)
                {
                    var holeGeometry = new RectangleGeometry(_targetHoleRect, 8, 8);
                    maskGeometry = new CombinedGeometry(GeometryCombineMode.Exclude, fullGeometry, holeGeometry);

                    // 繪製半透明挖洞遮罩 (Slate-900 / 78% opacity)
                    drawingContext.DrawGeometry(new SolidColorBrush(Color.FromArgb(198, 15, 23, 42)), null, maskGeometry);

                    // 繪製目標高亮外框
                    var borderPen = new Pen(new SolidColorBrush(Color.FromArgb(200, 59, 130, 246)), 2.0);
                    drawingContext.DrawRoundedRectangle(null, borderPen, _targetHoleRect, 8, 8);
                }
                else
                {
                    drawingContext.DrawGeometry(new SolidColorBrush(Color.FromArgb(198, 15, 23, 42)), null, maskGeometry);
                }
            }
        }
    }
}
