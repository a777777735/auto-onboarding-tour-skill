using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace AutoOnboardingTour.WinForms
{
    public enum TourPlacement
    {
        Top,
        Bottom,
        Left,
        Right
    }

    /// <summary>
    /// WinForms 單一步驟配置
    /// </summary>
    public class WinFormsTourStep
    {
        /// <summary>
        /// 目標控制項：支援控制項名稱 (string) 或 Control 實體
        /// </summary>
        public object Target { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public TourPlacement Placement { get; set; } = TourPlacement.Bottom;

        /// <summary>
        /// 顯示此步驟前執行的委派（例如切換 tabControl1.SelectedIndex = 1）
        /// </summary>
        public Action BeforeShow { get; set; }
    }

    /// <summary>
    /// WinForms 零依賴純原生聚光燈導覽引擎
    /// 使用貼合主視窗的無邊框 Overlay Form 搭配 Region 挖洞鏤空與浮動說明卡片
    /// </summary>
    public class WinFormsTourManager
    {
        private readonly Form _mainForm;
        private readonly List<WinFormsTourStep> _steps = new List<WinFormsTourStep>();
        private TourOverlayForm _overlayForm;
        private int _currentIndex = -1;

        public string StorageKey { get; set; } = "winforms_tour_completed";
        public int SpotlightPadding { get; set; } = 8;
        public IReadOnlyList<WinFormsTourStep> Steps => _steps;
        public bool IsActive => _overlayForm != null && !_overlayForm.IsDisposed && _overlayForm.Visible;

        public event EventHandler TourCompleted;
        public event EventHandler TourSkipped;
        public event EventHandler<int> StepChanged;

        public WinFormsTourManager(Form mainForm)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
        }

        public WinFormsTourManager AddStep(WinFormsTourStep step)
        {
            if (step != null) _steps.Add(step);
            return this;
        }

        public WinFormsTourManager AddStep(string controlName, string title, string content, TourPlacement placement = TourPlacement.Bottom, Action beforeShow = null)
        {
            _steps.Add(new WinFormsTourStep
            {
                Target = controlName,
                Title = title,
                Content = content,
                Placement = placement,
                BeforeShow = beforeShow
            });
            return this;
        }

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
            if (_steps.Count == 0 || _mainForm.IsDisposed) return;

            if (_overlayForm == null || _overlayForm.IsDisposed)
            {
                _overlayForm = new TourOverlayForm(_mainForm, this);
            }

            _overlayForm.Show(_mainForm);
            _overlayForm.SyncBoundsWithMainForm();

            GoToStep(startIndex);
        }

        public void GoToStep(int index)
        {
            if (index < 0 || index >= _steps.Count) return;

            _currentIndex = index;
            var step = _steps[index];

            try
            {
                step.BeforeShow?.Invoke();
                Application.DoEvents(); // 讓介面即時刷新（如 Tab 切換）
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[WinFormsTour] BeforeShow error: {ex.Message}");
            }

            _overlayForm?.RenderStep(step, _currentIndex, _steps.Count);
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
            if (_overlayForm != null && !_overlayForm.IsDisposed)
            {
                _overlayForm.Hide();
                _overlayForm.Dispose();
                _overlayForm = null;
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

        internal Control FindControl(object target)
        {
            if (target is Control c) return c;
            if (target is string name && !string.IsNullOrWhiteSpace(name))
            {
                var matches = _mainForm.Controls.Find(name, true);
                if (matches.Length > 0) return matches[0];
            }
            return null;
        }

        /// <summary>
        /// 貼合於主 Form 之上的無邊框半透明遮罩視窗
        /// </summary>
        private class TourOverlayForm : Form
        {
            private readonly Form _ownerForm;
            private readonly WinFormsTourManager _manager;
            private readonly Panel _cardPanel;
            private Rectangle _targetHoleRect = Rectangle.Empty;
            private Control _currentTargetControl;
            private WinFormsTourStep _currentStep;

            public TourOverlayForm(Form ownerForm, WinFormsTourManager manager)
            {
                _ownerForm = ownerForm;
                _manager = manager;

                FormBorderStyle = FormBorderStyle.None;
                ShowInTaskbar = false;
                StartPosition = FormStartPosition.Manual;
                DoubleBuffered = true;
                BackColor = Color.FromArgb(15, 23, 42);
                Opacity = 0.82;

                _cardPanel = new Panel
                {
                    BackColor = Color.White,
                    Width = 320,
                    Padding = new Padding(16),
                    BorderStyle = BorderStyle.None
                };
                Controls.Add(_cardPanel);

                // 綁定主視窗跟隨事件
                _ownerForm.LocationChanged += (s, e) => SyncBoundsWithMainForm();
                _ownerForm.SizeChanged += (s, e) => SyncBoundsWithMainForm();
                _ownerForm.FormClosing += (s, e) => Close();
            }

            public void SyncBoundsWithMainForm()
            {
                if (_ownerForm == null || _ownerForm.IsDisposed) return;

                var clientRect = _ownerForm.RectangleToScreen(_ownerForm.ClientRectangle);
                Bounds = clientRect;
                RecomputeHoleAndCardPosition();
            }

            public void RenderStep(WinFormsTourStep step, int index, int total)
            {
                _currentStep = step;
                _currentTargetControl = _manager.FindControl(step.Target);

                _cardPanel.SuspendLayout();
                _cardPanel.Controls.Clear();

                // 標題 + 關閉按鈕
                var lblTitle = new Label
                {
                    Text = step.Title,
                    Font = new Font("Microsoft JhengHei UI", 11.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(15, 23, 42),
                    Location = new Point(14, 14),
                    AutoSize = true
                };

                var btnClose = new Button
                {
                    Text = "✕",
                    FlatStyle = FlatStyle.Flat,
                    ForeColor = Color.FromArgb(148, 163, 184),
                    Font = new Font("Arial", 9f, FontStyle.Bold),
                    Size = new Size(24, 24),
                    Location = new Point(_cardPanel.Width - 36, 12),
                    Cursor = Cursors.Hand
                };
                btnClose.FlatAppearance.BorderSize = 0;
                btnClose.Click += (s, e) => _manager.Skip();

                // 進度條裝飾
                var pnlProgress = new Panel
                {
                    Location = new Point(14, 42),
                    Size = new Size(_cardPanel.Width - 28, 4),
                    BackColor = Color.FromArgb(226, 232, 240)
                };
                var pnlFill = new Panel
                {
                    Dock = DockStyle.Left,
                    Width = (int)((_cardPanel.Width - 28) * ((double)(index + 1) / total)),
                    BackColor = Color.FromArgb(37, 99, 235)
                };
                pnlProgress.Controls.Add(pnlFill);

                // 內文
                var lblContent = new Label
                {
                    Text = step.Content,
                    Font = new Font("Microsoft JhengHei UI", 9.5f),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    Location = new Point(14, 56),
                    Size = new Size(_cardPanel.Width - 28, 56),
                    AutoEllipsis = true
                };

                // 計數
                var lblCounter = new Label
                {
                    Text = $"{index + 1} / {total}",
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    Location = new Point(14, 122),
                    AutoSize = true
                };

                // 按鈕
                var btnNext = new Button
                {
                    Text = index == total - 1 ? "完成" : "下一步",
                    Size = new Size(68, 28),
                    Location = new Point(_cardPanel.Width - 82, 116),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(37, 99, 235),
                    ForeColor = Color.White,
                    Font = new Font("Microsoft JhengHei UI", 9f, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnNext.FlatAppearance.BorderSize = 0;
                btnNext.Click += (s, e) => _manager.Next();

                var btnPrev = new Button
                {
                    Text = "上一步",
                    Size = new Size(62, 28),
                    Location = new Point(btnNext.Left - 68, 116),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(241, 245, 249),
                    ForeColor = Color.FromArgb(51, 65, 85),
                    Font = new Font("Microsoft JhengHei UI", 9f),
                    Cursor = Cursors.Hand,
                    Visible = index > 0
                };
                btnPrev.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
                btnPrev.Click += (s, e) => _manager.Prev();

                var btnSkip = new Button
                {
                    Text = "跳過",
                    Size = new Size(50, 28),
                    Location = new Point((btnPrev.Visible ? btnPrev.Left : btnNext.Left) - 54, 116),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.Transparent,
                    ForeColor = Color.FromArgb(100, 116, 139),
                    Font = new Font("Microsoft JhengHei UI", 9f),
                    Cursor = Cursors.Hand
                };
                btnSkip.FlatAppearance.BorderSize = 0;
                btnSkip.Click += (s, e) => _manager.Skip();

                _cardPanel.Height = 158;
                _cardPanel.Controls.Add(lblTitle);
                _cardPanel.Controls.Add(btnClose);
                _cardPanel.Controls.Add(pnlProgress);
                _cardPanel.Controls.Add(lblContent);
                _cardPanel.Controls.Add(lblCounter);
                _cardPanel.Controls.Add(btnSkip);
                _cardPanel.Controls.Add(btnPrev);
                _cardPanel.Controls.Add(btnNext);
                _cardPanel.ResumeLayout(true);

                RecomputeHoleAndCardPosition();
            }

            private void RecomputeHoleAndCardPosition()
            {
                if (_currentTargetControl != null && _currentTargetControl.Visible)
                {
                    var screenRect = _currentTargetControl.RectangleToScreen(_currentTargetControl.ClientRectangle);
                    var localPt = PointToClient(screenRect.Location);

                    int p = _manager.SpotlightPadding;
                    _targetHoleRect = new Rectangle(
                        localPt.X - p,
                        localPt.Y - p,
                        screenRect.Width + p * 2,
                        screenRect.Height + p * 2
                    );

                    // 使用 Region 挖洞鏤空，使目標區域完全通透並可穿透檢視
                    var fullRegion = new Region(ClientRectangle);
                    fullRegion.Exclude(_targetHoleRect);
                    Region = fullRegion;
                }
                else
                {
                    _targetHoleRect = Rectangle.Empty;
                    Region = new Region(ClientRectangle);
                }

                PositionCard();
                Invalidate();
            }

            private void PositionCard()
            {
                int cardW = _cardPanel.Width;
                int cardH = _cardPanel.Height;
                int margin = 14;

                int x = 20;
                int y = 20;

                if (!_targetHoleRect.IsEmpty)
                {
                    var placement = _currentStep?.Placement ?? TourPlacement.Bottom;

                    // 智慧翻轉防溢出
                    if (placement == TourPlacement.Bottom && _targetHoleRect.Bottom + margin + cardH > ClientSize.Height)
                        placement = TourPlacement.Top;
                    else if (placement == TourPlacement.Top && _targetHoleRect.Top - margin - cardH < 0)
                        placement = TourPlacement.Bottom;

                    switch (placement)
                    {
                        case TourPlacement.Top:
                            y = _targetHoleRect.Top - cardH - margin;
                            x = _targetHoleRect.Left + (_targetHoleRect.Width - cardW) / 2;
                            break;
                        case TourPlacement.Bottom:
                            y = _targetHoleRect.Bottom + margin;
                            x = _targetHoleRect.Left + (_targetHoleRect.Width - cardW) / 2;
                            break;
                        case TourPlacement.Left:
                            x = _targetHoleRect.Left - cardW - margin;
                            y = _targetHoleRect.Top + (_targetHoleRect.Height - cardH) / 2;
                            break;
                        case TourPlacement.Right:
                            x = _targetHoleRect.Right + margin;
                            y = _targetHoleRect.Top + (_targetHoleRect.Height - cardH) / 2;
                            break;
                    }

                    // 防溢出邊界 Clamping
                    x = Math.Max(12, Math.Min(x, ClientSize.Width - cardW - 12));
                    y = Math.Max(12, Math.Min(y, ClientSize.Height - cardH - 12));
                }
                else
                {
                    x = (ClientSize.Width - cardW) / 2;
                    y = (ClientSize.Height - cardH) / 2;
                }

                _cardPanel.Location = new Point(x, y);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                base.OnPaint(e);

                if (!_targetHoleRect.IsEmpty)
                {
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    using (var pen = new Pen(Color.FromArgb(59, 130, 246), 2.5f))
                    {
                        e.Graphics.DrawRectangle(pen, _targetHoleRect);
                    }
                }
            }

            protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            {
                if (keyData == Keys.Right)
                {
                    _manager.Next();
                    return true;
                }
                if (keyData == Keys.Left)
                {
                    _manager.Prev();
                    return true;
                }
                if (keyData == Keys.Escape)
                {
                    _manager.Skip();
                    return true;
                }
                return base.ProcessCmdKey(ref msg, keyData);
            }
        }
    }
}
