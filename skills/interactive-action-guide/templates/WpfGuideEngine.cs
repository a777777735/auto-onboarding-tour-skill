using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using InteractiveActionGuide.Common;

namespace InteractiveActionGuide.Wpf
{
    public enum GuidePlacement
    {
        Top,
        Bottom,
        Left,
        Right
    }

    public enum GuideMode
    {
        Concise,  // 4~6 步核心流程
        Detailed  // 全功能地毯式引導
    }

    public class GuideStep
    {
        public object Target { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }

        /// <summary>
        /// 獨立的 SOP / 說明文檔深度業務說明（若未設定則自動 Fallback 到 Content）
        /// </summary>
        public string DetailedDescription { get; set; }

        /// <summary>
        /// 注意事項 / 警告鐵律（選填）
        /// </summary>
        public string Caution { get; set; }

        /// <summary>
        /// 預期操作結果與驗證狀態（選填）
        /// </summary>
        public string ExpectedResult { get; set; }

        public GuidePlacement Placement { get; set; } = GuidePlacement.Bottom;
        public bool AdvanceOnClick { get; set; } = true;
        public Predicate<string> AdvanceOnTextValidate { get; set; }
        public bool AdvanceOnSelectionChange { get; set; }
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
                        return Task.FromResult<object>(null);
                    };
                }
            }
        }
    }

    public class WpfInteractiveGuideManager
    {
        private readonly Window _window;
        private readonly List<GuideStep> _conciseSteps = new List<GuideStep>();
        private readonly List<GuideStep> _detailedSteps = new List<GuideStep>();
        private readonly List<SopStepInfo> _sopSteps = new List<SopStepInfo>();
        private GuideAdorner _currentAdorner;
        private int _currentIndex = -1;
        private GuideMode _currentMode = GuideMode.Concise;

        public string StorageKey { get; set; } = "iag_wpf_guide_completed";
        public double SpotlightPadding { get; set; } = 8.0;
        public bool AutoCaptureSopScreenshot { get; set; } = true;
        public string SopExportDirectory { get; set; } = "docs/SOP";
        public string SopAppTitle { get; set; } = "系統操作指引";

        public GuideMode CurrentMode => _currentMode;
        public IReadOnlyList<GuideStep> Steps => _currentMode == GuideMode.Concise ? (IReadOnlyList<GuideStep>)_conciseSteps : _detailedSteps;
        public bool IsActive => _currentAdorner != null;

        public event EventHandler GuideCompleted;
        public event EventHandler GuideSkipped;
        public event EventHandler<int> StepChanged;

        public WpfInteractiveGuideManager(Window window)
        {
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _window.SizeChanged += (s, e) => _currentAdorner?.InvalidateLayoutAndRender();
            _window.LocationChanged += (s, e) => _currentAdorner?.InvalidateLayoutAndRender();
            _window.PreviewKeyDown += OnWindowKeyDown;
        }

        public WpfInteractiveGuideManager AddStep(GuideStep step, GuideMode mode = GuideMode.Concise)
        {
            if (step != null)
            {
                if (mode == GuideMode.Concise) _conciseSteps.Add(step);
                else _detailedSteps.Add(step);
            }
            return this;
        }

        public WpfInteractiveGuideManager AddStep(string targetName, string title, string content, GuidePlacement placement = GuidePlacement.Bottom, bool advanceOnClick = true, Action beforeShow = null, GuideMode mode = GuideMode.Concise)
        {
            var step = new GuideStep
            {
                Target = targetName,
                Title = title,
                Content = content,
                Placement = placement,
                AdvanceOnClick = advanceOnClick,
                BeforeShow = beforeShow
            };
            return AddStep(step, mode);
        }

        public void StartWithSelectionDialog()
        {
            var dlg = new Window
            {
                Title = "選擇導引模式",
                Width = 380,
                Height = 260,
                WindowStartupLocation = WindowStartupLocation.CenterOwner,
                Owner = _window,
                ResizeMode = ResizeMode.NoResize,
                WindowStyle = WindowStyle.ToolWindow,
                Background = new SolidColorBrush(Color.FromRgb(30, 41, 59)),
                Foreground = new SolidColorBrush(Color.FromRgb(248, 250, 252))
            };

            var sp = new StackPanel { Margin = new Thickness(18) };
            sp.Children.Add(new TextBlock
            {
                Text = "🎯 請勾選導引深度模式：",
                FontWeight = FontWeights.Bold,
                FontSize = 14,
                Margin = new Thickness(0, 0, 0, 14)
            });

            var rbConcise = new RadioButton
            {
                Content = "⚡ 收斂精簡模式 (4~6 步核心流程，起點 -> 核心 -> 終點)",
                IsChecked = _currentMode == GuideMode.Concise,
                Foreground = new SolidColorBrush(Color.FromRgb(248, 250, 252)),
                Margin = new Thickness(0, 0, 0, 10),
                FontWeight = FontWeights.SemiBold
            };
            var rbDetailed = new RadioButton
            {
                Content = "📖 詳細全覆蓋模式 (100% 地毯式涵蓋所有 Tab/按鈕/面板)",
                IsChecked = _currentMode == GuideMode.Detailed,
                Foreground = new SolidColorBrush(Color.FromRgb(248, 250, 252)),
                Margin = new Thickness(0, 0, 0, 18),
                FontWeight = FontWeights.SemiBold
            };
            sp.Children.Add(rbConcise);
            sp.Children.Add(rbDetailed);

            var btnStart = new Button
            {
                Content = "🚀 開始引導",
                Height = 34,
                Background = new SolidColorBrush(Color.FromRgb(2, 132, 199)),
                Foreground = Brushes.White,
                FontWeight = FontWeights.Bold,
                Cursor = Cursors.Hand
            };
            btnStart.Click += (s, e) =>
            {
                GuideMode mode = (rbDetailed.IsChecked == true) ? GuideMode.Detailed : GuideMode.Concise;
                dlg.Close();
                Start(mode);
            };
            sp.Children.Add(btnStart);

            dlg.Content = sp;
            dlg.ShowDialog();
        }

        public void StartIfFirstTime(GuideMode mode = GuideMode.Concise)
        {
            string flagFile = GetStorageFilePath();
            if (!File.Exists(flagFile))
            {
                StartWithSelectionDialog();
            }
        }

        private string GetStorageFilePath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            string dir = Path.Combine(appData, "InteractiveActionGuide");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return Path.Combine(dir, $"{StorageKey}.flag");
        }

        public void Start(GuideMode mode = GuideMode.Concise, int startIndex = 0)
        {
            _currentMode = mode;
            _sopSteps.Clear();
            var activeSteps = Steps;
            if (activeSteps.Count == 0) return;

            var contentVisual = _window.Content as UIElement;
            if (contentVisual == null) return;

            var layer = AdornerLayer.GetAdornerLayer(contentVisual);
            if (layer == null) return;

            if (_currentAdorner != null)
            {
                layer.Remove(_currentAdorner);
            }

            _currentAdorner = new GuideAdorner(contentVisual, this);
            layer.Add(_currentAdorner);

            GoToStep(startIndex);
        }

        public async void GoToStep(int index)
        {
            if (index < 0 || index >= Steps.Count) return;

            _currentIndex = index;
            var step = Steps[index];

            if (step.BeforeShowAsync != null)
            {
                try
                {
                    await step.BeforeShowAsync();
                    await Task.Delay(80);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"[InteractiveGuide] BeforeShow error: {ex.Message}");
                }
            }

            _currentAdorner?.UpdateStep(step, _currentIndex, Steps.Count);
            StepChanged?.Invoke(this, _currentIndex);

            // 自動觸發 SOP 截圖
            if (AutoCaptureSopScreenshot)
            {
                Task.Delay(350).ContinueWith(t =>
                {
                    try
                    {
                        _window.Dispatcher.Invoke(new Action(() =>
                        {
                            CaptureStep(step, _currentIndex + 1);
                        }));
                    }
                    catch { }
                });
            }
        }

        private void CaptureStep(GuideStep step, int stepNumber)
        {
            try
            {
                string shotsDir = Path.Combine(SopExportDirectory, "screenshots");
                if (!Directory.Exists(shotsDir)) Directory.CreateDirectory(shotsDir);

                string shotFileName = string.Format("step_{0}.png", stepNumber);
                string shotFullPath = Path.Combine(shotsDir, shotFileName);

                int w = (int)_window.ActualWidth;
                int h = (int)_window.ActualHeight;
                if (w <= 0 || h <= 0) return;

                var rtb = new RenderTargetBitmap(w, h, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(_window);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));
                using (var fs = new FileStream(shotFullPath, FileMode.Create))
                {
                    encoder.Save(fs);
                }

                _sopSteps.Add(new SopStepInfo
                {
                    StepIndex = stepNumber,
                    Title = step.Title,
                    Content = step.Content,
                    DetailedDescription = step.DetailedDescription,
                    Caution = step.Caution,
                    ExpectedResult = step.ExpectedResult,
                    TargetName = step.Target != null ? step.Target.ToString() : string.Empty,
                    ActionType = step.AdvanceOnClick ? "點擊控制項" : (step.AdvanceOnSelectionChange ? "切換選項" : "輸入內容"),
                    ScreenshotRelativePath = Path.Combine("screenshots", shotFileName)
                });
            }
            catch { }
        }

        public void Next()
        {
            if (_currentIndex < Steps.Count - 1)
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

            if (AutoCaptureSopScreenshot && _sopSteps.Count > 0)
            {
                try
                {
                    SopDocumentGenerator.GenerateMarkdown(SopAppTitle, _currentMode.ToString(), _sopSteps, SopExportDirectory);
                }
                catch { }
            }

            Stop();
            GuideCompleted?.Invoke(this, EventArgs.Empty);
        }

        public void Skip()
        {
            SaveCompletionFlag();
            Stop();
            GuideSkipped?.Invoke(this, EventArgs.Empty);
        }

        public void Stop()
        {
            if (_currentAdorner != null)
            {
                var contentVisual = _window.Content as UIElement;
                var layer = AdornerLayer.GetAdornerLayer(contentVisual);
                layer?.Remove(_currentAdorner);
                _currentAdorner.Dispose();
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
            if (e.Key == Key.Escape)
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

        private class GuideAdorner : Adorner, IDisposable
        {
            private readonly WpfInteractiveGuideManager _manager;
            private readonly Border _cardContainer;
            private readonly VisualCollection _visualChildren;
            private Rect _targetHoleRect;
            private FrameworkElement _currentTarget;
            private GuideStep _currentStep;
            private Action _cleanupListener;

            public GuideAdorner(UIElement adornedElement, WpfInteractiveGuideManager manager) : base(adornedElement)
            {
                _manager = manager;
                _visualChildren = new VisualCollection(this);

                _cardContainer = BuildCardUI();
                _visualChildren.Add(_cardContainer);

                IsHitTestVisible = true;
                Focusable = false;
            }

            public void Dispose()
            {
                ClearListener();
            }

            private void ClearListener()
            {
                if (_cleanupListener != null)
                {
                    _cleanupListener();
                    _cleanupListener = null;
                }
            }

            protected override int VisualChildrenCount => _visualChildren.Count;
            protected override Visual GetVisualChild(int index) => _visualChildren[index];

            protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
            {
                Point pt = hitTestParameters.HitPoint;

                if (_cardContainer != null && _cardContainer.IsVisible)
                {
                    GeneralTransform transform = _cardContainer.TransformToAncestor(this);
                    if (transform != null)
                    {
                        Rect cardRect = transform.TransformBounds(new Rect(0, 0, _cardContainer.ActualWidth, _cardContainer.ActualHeight));
                        if (cardRect.Contains(pt))
                        {
                            return new PointHitTestResult(this, pt);
                        }
                    }
                }

                if (!_targetHoleRect.IsEmpty && _targetHoleRect.Contains(pt))
                {
                    return null;
                }

                return new PointHitTestResult(this, pt);
            }

            private Border BuildCardUI()
            {
                return new Border
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
            }

            public void UpdateStep(GuideStep step, int index, int total)
            {
                ClearListener();
                _currentStep = step;
                _currentTarget = _manager.FindTargetElement(step.Target);

                BindTargetEvents(step);

                var stack = new StackPanel();

                var headerGrid = new Grid();
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                headerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });

                var txtTitle = new TextBlock
                {
                    Text = "👉 " + step.Title,
                    FontWeight = FontWeights.Bold,
                    FontSize = 14.5,
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

                var progressWrap = new Border
                {
                    Height = 4,
                    Background = new SolidColorBrush(Color.FromRgb(226, 232, 240)),
                    CornerRadius = new CornerRadius(2),
                    Margin = new Thickness(0, 8, 0, 10)
                };
                var progressFill = new Border
                {
                    Height = 4,
                    Background = new SolidColorBrush(Color.FromRgb(2, 132, 199)),
                    CornerRadius = new CornerRadius(2),
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Width = 320 * ((double)(index + 1) / total)
                };
                progressWrap.Child = progressFill;
                stack.Children.Add(progressWrap);

                var txtContent = new TextBlock
                {
                    Text = step.Content,
                    TextWrapping = TextWrapping.Wrap,
                    FontSize = 13,
                    LineHeight = 19,
                    Foreground = new SolidColorBrush(Color.FromRgb(71, 85, 105)),
                    Margin = new Thickness(0, 0, 0, 10)
                };
                stack.Children.Add(txtContent);

                var badgeBorder = new Border
                {
                    Background = new SolidColorBrush(Color.FromRgb(240, 249, 255)),
                    BorderBrush = new SolidColorBrush(Color.FromRgb(186, 230, 253)),
                    BorderThickness = new Thickness(1),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(6, 4, 6, 4),
                    Margin = new Thickness(0, 0, 0, 12)
                };
                var badgeTxt = new TextBlock
                {
                    Text = "🎯 請直接在上方高亮控制項中操作以繼續",
                    FontSize = 12,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Color.FromRgb(3, 105, 161))
                };
                badgeBorder.Child = badgeTxt;
                stack.Children.Add(badgeBorder);

                var footerGrid = new Grid();
                footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                footerGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

                var txtCounter = new TextBlock
                {
                    Text = string.Format("{0} / {1}", index + 1, total),
                    FontSize = 12,
                    FontWeight = FontWeights.SemiBold,
                    Foreground = new SolidColorBrush(Color.FromRgb(100, 116, 139)),
                    VerticalAlignment = VerticalAlignment.Center
                };
                Grid.SetColumn(txtCounter, 0);

                var btnStack = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Right };

                var btnSkip = new Button
                {
                    Content = "跳過",
                    Margin = new Thickness(0, 0, 6, 0),
                    Padding = new Thickness(8, 4, 8, 4),
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
                        Padding = new Thickness(10, 4, 10, 4),
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
                    Padding = new Thickness(12, 4, 12, 4),
                    Background = new SolidColorBrush(Color.FromRgb(2, 132, 199)),
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

            private void BindTargetEvents(GuideStep step)
            {
                if (_currentTarget == null) return;

                if (step.AdvanceOnClick)
                {
                    if (_currentTarget is ButtonBase btn)
                    {
                        RoutedEventHandler clickHandler = null;
                        clickHandler = (s, e) =>
                        {
                            btn.Click -= clickHandler;
                            AdvanceWithDelay();
                        };
                        btn.Click += clickHandler;
                        _cleanupListener = () => btn.Click -= clickHandler;
                    }
                    else
                    {
                        MouseButtonEventHandler downHandler = null;
                        downHandler = (s, e) =>
                        {
                            _currentTarget.PreviewMouseDown -= downHandler;
                            AdvanceWithDelay();
                        };
                        _currentTarget.PreviewMouseDown += downHandler;
                        _cleanupListener = () => _currentTarget.PreviewMouseDown -= downHandler;
                    }
                }
                else if (step.AdvanceOnTextValidate != null && _currentTarget is TextBox tb)
                {
                    TextChangedEventHandler textHandler = null;
                    textHandler = (s, e) =>
                    {
                        if (step.AdvanceOnTextValidate(tb.Text))
                        {
                            tb.TextChanged -= textHandler;
                            AdvanceWithDelay();
                        }
                    };
                    tb.TextChanged += textHandler;
                    _cleanupListener = () => tb.TextChanged -= textHandler;
                }
                else if (step.AdvanceOnSelectionChange && _currentTarget is Selector sel)
                {
                    SelectionChangedEventHandler selHandler = null;
                    selHandler = (s, e) =>
                    {
                        sel.SelectionChanged -= selHandler;
                        AdvanceWithDelay();
                    };
                    sel.SelectionChanged += selHandler;
                    _cleanupListener = () => sel.SelectionChanged -= selHandler;
                }
            }

            private void AdvanceWithDelay()
            {
                Dispatcher.BeginInvoke(new Action(async () =>
                {
                    await Task.Delay(250);
                    if (_manager.IsActive) _manager.Next();
                }));
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
                    var placement = _currentStep?.Placement ?? GuidePlacement.Bottom;

                    if (placement == GuidePlacement.Bottom && _targetHoleRect.Bottom + margin + cardHeight > finalSize.Height)
                        placement = GuidePlacement.Top;
                    else if (placement == GuidePlacement.Top && _targetHoleRect.Top - margin - cardHeight < 0)
                        placement = GuidePlacement.Bottom;

                    switch (placement)
                    {
                        case GuidePlacement.Top:
                            y = _targetHoleRect.Top - cardHeight - margin;
                            x = _targetHoleRect.Left + (_targetHoleRect.Width - cardWidth) / 2;
                            break;
                        case GuidePlacement.Bottom:
                            y = _targetHoleRect.Bottom + margin;
                            x = _targetHoleRect.Left + (_targetHoleRect.Width - cardWidth) / 2;
                            break;
                        case GuidePlacement.Left:
                            x = _targetHoleRect.Left - cardWidth - margin;
                            y = _targetHoleRect.Top + (_targetHoleRect.Height - cardHeight) / 2;
                            break;
                        case GuidePlacement.Right:
                            x = _targetHoleRect.Right + margin;
                            y = _targetHoleRect.Top + (_targetHoleRect.Height - cardHeight) / 2;
                            break;
                    }

                    x = Math.Max(12, Math.Min(x, finalSize.Width - cardWidth - 12));
                    y = Math.Max(12, Math.Min(y, finalSize.Height - cardHeight - 12));
                }
                else
                {
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

                    drawingContext.DrawGeometry(new SolidColorBrush(Color.FromArgb(198, 15, 23, 42)), null, maskGeometry);

                    var borderPen = new Pen(new SolidColorBrush(Color.FromArgb(220, 2, 132, 199)), 2.5);
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
