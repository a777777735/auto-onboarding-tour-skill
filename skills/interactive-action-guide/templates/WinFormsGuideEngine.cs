using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;
using InteractiveActionGuide.Common;

namespace InteractiveActionGuide.WinForms
{
    public enum GuidePlacement
    {
        Top,
        Bottom,
        Left,
        Right
    }

    /// <summary>
    /// 導引模式：收斂精簡 (4~6步) 或 詳細全覆蓋 (無限制)
    /// </summary>
    public enum GuideMode
    {
        Concise,  // 收斂精簡模式：保留 4 至 6 個核心步驟
        Detailed  // 詳細全覆蓋模式：地毯式涵蓋所有 Tab/Expander/輸入框
    }

    /// <summary>
    /// 定義 WinForms 實戰互動引導步驟
    /// </summary>
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

        /// <summary>
        /// 是否在目標控制項被真實點擊 (Click) 時自動跳下一步
        /// </summary>
        public bool AdvanceOnClick { get; set; } = true;

        /// <summary>
        /// 針對 TextBox：文字變更時的檢驗委派，返回 true 則自動跳下一步
        /// </summary>
        public Predicate<string> AdvanceOnTextValidate { get; set; }

        /// <summary>
        /// 針對 ComboBox：選單切換時是否自動跳下一步
        /// </summary>
        public bool AdvanceOnSelectionChange { get; set; }

        /// <summary>
        /// 顯示此步驟前執行的委派（例如切換 tabControl1.SelectedIndex = 1）
        /// </summary>
        public Action BeforeShow { get; set; }
    }

    /// <summary>
    /// WinForms 實戰互動導引引擎 (C# 7.3 嚴格相容)
    /// </summary>
    public class WinFormsInteractiveGuideManager
    {
        private readonly Form _mainForm;
        private readonly List<GuideStep> _conciseSteps = new List<GuideStep>();
        private readonly List<GuideStep> _detailedSteps = new List<GuideStep>();
        private readonly List<SopStepInfo> _sopSteps = new List<SopStepInfo>();
        private GuideOverlayForm _overlayForm;
        private int _currentIndex = -1;
        private GuideMode _currentMode = GuideMode.Concise;

        public string StorageKey { get; set; } = "iag_winforms_guide_completed";
        public int SpotlightPadding { get; set; } = 8;
        public bool AutoCaptureSopScreenshot { get; set; } = true;
        public string SopExportDirectory { get; set; } = "docs/SOP";
        public string SopAppTitle { get; set; } = "系統操作指引";

        public GuideMode CurrentMode => _currentMode;
        public IReadOnlyList<GuideStep> Steps => _currentMode == GuideMode.Concise ? (IReadOnlyList<GuideStep>)_conciseSteps : _detailedSteps;
        public bool IsActive => _overlayForm != null && !_overlayForm.IsDisposed && _overlayForm.Visible;

        public event EventHandler GuideCompleted;
        public event EventHandler GuideSkipped;
        public event EventHandler<int> StepChanged;

        public WinFormsInteractiveGuideManager(Form mainForm)
        {
            _mainForm = mainForm ?? throw new ArgumentNullException(nameof(mainForm));
        }

        public WinFormsInteractiveGuideManager AddStep(GuideStep step, GuideMode mode = GuideMode.Concise)
        {
            if (step != null)
            {
                if (mode == GuideMode.Concise) _conciseSteps.Add(step);
                else _detailedSteps.Add(step);
            }
            return this;
        }

        public WinFormsInteractiveGuideManager AddStep(string controlName, string title, string content, GuidePlacement placement = GuidePlacement.Bottom, bool advanceOnClick = true, Action beforeShow = null, GuideMode mode = GuideMode.Concise)
        {
            var step = new GuideStep
            {
                Target = controlName,
                Title = title,
                Content = content,
                Placement = placement,
                AdvanceOnClick = advanceOnClick,
                BeforeShow = beforeShow
            };
            return AddStep(step, mode);
        }

        /// <summary>
        /// 彈出模式勾選選擇器視窗，供使用者自由勾選後啟動
        /// </summary>
        public void StartWithSelectionDialog()
        {
            using (var dlg = new Form
            {
                Text = "選擇導引模式",
                Size = new Size(390, 240),
                StartPosition = FormStartPosition.CenterParent,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false,
                MinimizeBox = false,
                BackColor = Color.FromArgb(30, 41, 59),
                ForeColor = Color.FromArgb(248, 250, 252)
            })
            {
                var lbl = new Label
                {
                    Text = "🎯 請勾選導引深度模式：",
                    Font = new Font("Microsoft JhengHei UI", 10.5f, FontStyle.Bold),
                    Location = new Point(20, 16),
                    AutoSize = true
                };

                var rbConcise = new RadioButton
                {
                    Text = "⚡ 收斂精簡模式 (4~6 步核心流程，快速上手)",
                    Checked = _currentMode == GuideMode.Concise,
                    Location = new Point(24, 48),
                    Size = new Size(330, 28),
                    Font = new Font("Microsoft JhengHei UI", 9.5f, FontStyle.Bold)
                };

                var rbDetailed = new RadioButton
                {
                    Text = "📖 詳細全覆蓋模式 (100% 地毯式涵蓋所有 Tab/按鈕)",
                    Checked = _currentMode == GuideMode.Detailed,
                    Location = new Point(24, 82),
                    Size = new Size(330, 28),
                    Font = new Font("Microsoft JhengHei UI", 9.5f, FontStyle.Bold)
                };

                var btnStart = new Button
                {
                    Text = "🚀 開始引導",
                    Location = new Point(24, 126),
                    Size = new Size(326, 36),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(2, 132, 199),
                    ForeColor = Color.White,
                    Font = new Font("Microsoft JhengHei UI", 9.5f, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnStart.FlatAppearance.BorderSize = 0;
                btnStart.Click += (s, e) =>
                {
                    GuideMode mode = rbDetailed.Checked ? GuideMode.Detailed : GuideMode.Concise;
                    dlg.DialogResult = DialogResult.OK;
                    dlg.Close();
                    Start(mode);
                };

                dlg.Controls.Add(lbl);
                dlg.Controls.Add(rbConcise);
                dlg.Controls.Add(rbDetailed);
                dlg.Controls.Add(btnStart);
                dlg.ShowDialog(_mainForm);
            }
        }

        public void StartIfFirstTime(GuideMode mode = GuideMode.Concise)
        {
            string flagFile = GetStorageFilePath();
            if (!File.Exists(flagFile))
            {
                StartWithSelectionDialog();
            }
        }

        public void SwitchMode(GuideMode mode)
        {
            _currentMode = mode;
            if (IsActive)
            {
                GoToStep(0);
            }
        }

        public void Start(GuideMode mode = GuideMode.Concise, int startIndex = 0)
        {
            _currentMode = mode;
            var activeSteps = Steps;
            if (activeSteps.Count == 0 || _mainForm.IsDisposed) return;

            if (_overlayForm == null || _overlayForm.IsDisposed)
            {
                _overlayForm = new GuideOverlayForm(_mainForm, this);
            }

            _overlayForm.Show(_mainForm);
            _overlayForm.SyncBoundsWithMainForm();

            GoToStep(startIndex);
        }

        public void GoToStep(int index)
        {
            if (index < 0 || index >= Steps.Count) return;

            _currentIndex = index;
            var step = Steps[index];

            try
            {
                step.BeforeShow?.Invoke();
                Application.DoEvents();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[InteractiveGuide] BeforeShow error: {ex.Message}");
            }

            _overlayForm?.RenderStep(step, _currentIndex, Steps.Count);
            StepChanged?.Invoke(this, _currentIndex);

            if (AutoCaptureSopScreenshot)
            {
                var timer = new Timer { Interval = 350 };
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    CaptureStep(step, _currentIndex + 1);
                };
                timer.Start();
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

                var bounds = _mainForm.Bounds;
                if (bounds.Width <= 0 || bounds.Height <= 0) return;

                using (var bmp = new Bitmap(bounds.Width, bounds.Height))
                {
                    using (var g = Graphics.FromImage(bmp))
                    {
                        g.CopyFromScreen(bounds.Location, Point.Empty, bounds.Size);
                    }
                    bmp.Save(shotFullPath, System.Drawing.Imaging.ImageFormat.Png);
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

        private string GetStorageFilePath()
        {
            string appData = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string dir = Path.Combine(appData, "InteractiveActionGuide");
            if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
            return Path.Combine(dir, StorageKey + ".flag");
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

        private class GuideOverlayForm : Form
        {
            private readonly Form _ownerForm;
            private readonly WinFormsInteractiveGuideManager _manager;
            private readonly Panel _cardPanel;
            private Rectangle _targetHoleRect = Rectangle.Empty;
            private Control _currentTargetControl;
            private GuideStep _currentStep;
            private Action _cleanupListener;

            public GuideOverlayForm(Form ownerForm, WinFormsInteractiveGuideManager manager)
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

            public void RenderStep(GuideStep step, int index, int total)
            {
                ClearListener();
                _currentStep = step;
                _currentTargetControl = _manager.FindControl(step.Target);

                BindTargetEvents(step);

                _cardPanel.SuspendLayout();
                _cardPanel.Controls.Clear();

                var lblTitle = new Label
                {
                    Text = "👉 " + step.Title,
                    Font = new Font("Microsoft JhengHei UI", 11f, FontStyle.Bold),
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

                var pnlProgress = new Panel
                {
                    Location = new Point(14, 40),
                    Size = new Size(_cardPanel.Width - 28, 4),
                    BackColor = Color.FromArgb(226, 232, 240)
                };
                var pnlFill = new Panel
                {
                    Dock = DockStyle.Left,
                    Width = (int)((_cardPanel.Width - 28) * ((double)(index + 1) / total)),
                    BackColor = Color.FromArgb(2, 132, 199)
                };
                pnlProgress.Controls.Add(pnlFill);

                var lblContent = new Label
                {
                    Text = step.Content,
                    Font = new Font("Microsoft JhengHei UI", 9f),
                    ForeColor = Color.FromArgb(71, 85, 105),
                    Location = new Point(14, 52),
                    Size = new Size(_cardPanel.Width - 28, 40),
                    AutoEllipsis = true
                };

                var lblBadge = new Label
                {
                    Text = "🎯 請在上方高亮控制項中操作以繼續",
                    Font = new Font("Microsoft JhengHei UI", 8.5f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(3, 105, 161),
                    BackColor = Color.FromArgb(240, 249, 255),
                    Location = new Point(14, 96),
                    Size = new Size(_cardPanel.Width - 28, 22),
                    TextAlign = ContentAlignment.MiddleLeft,
                    Padding = new Padding(4, 0, 0, 0)
                };

                var lblCounter = new Label
                {
                    Text = $"{index + 1} / {total}",
                    Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                    ForeColor = Color.FromArgb(100, 116, 139),
                    Location = new Point(14, 128),
                    AutoSize = true
                };

                var btnNext = new Button
                {
                    Text = index == total - 1 ? "完成" : "下一步",
                    Size = new Size(64, 26),
                    Location = new Point(_cardPanel.Width - 78, 124),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(2, 132, 199),
                    ForeColor = Color.White,
                    Font = new Font("Microsoft JhengHei UI", 8.5f, FontStyle.Bold),
                    Cursor = Cursors.Hand
                };
                btnNext.FlatAppearance.BorderSize = 0;
                btnNext.Click += (s, e) => _manager.Next();

                var btnPrev = new Button
                {
                    Text = "上一步",
                    Size = new Size(58, 26),
                    Location = new Point(btnNext.Left - 64, 124),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.FromArgb(241, 245, 249),
                    ForeColor = Color.FromArgb(51, 65, 85),
                    Font = new Font("Microsoft JhengHei UI", 8.5f),
                    Cursor = Cursors.Hand,
                    Visible = index > 0
                };
                btnPrev.FlatAppearance.BorderColor = Color.FromArgb(226, 232, 240);
                btnPrev.Click += (s, e) => _manager.Prev();

                var btnSkip = new Button
                {
                    Text = "跳過",
                    Size = new Size(46, 26),
                    Location = new Point((btnPrev.Visible ? btnPrev.Left : btnNext.Left) - 50, 124),
                    FlatStyle = FlatStyle.Flat,
                    BackColor = Color.Transparent,
                    ForeColor = Color.FromArgb(100, 116, 139),
                    Font = new Font("Microsoft JhengHei UI", 8.5f),
                    Cursor = Cursors.Hand
                };
                btnSkip.FlatAppearance.BorderSize = 0;
                btnSkip.Click += (s, e) => _manager.Skip();

                _cardPanel.Height = 160;
                _cardPanel.Controls.Add(lblTitle);
                _cardPanel.Controls.Add(btnClose);
                _cardPanel.Controls.Add(pnlProgress);
                _cardPanel.Controls.Add(lblContent);
                _cardPanel.Controls.Add(lblBadge);
                _cardPanel.Controls.Add(lblCounter);
                _cardPanel.Controls.Add(btnSkip);
                _cardPanel.Controls.Add(btnPrev);
                _cardPanel.Controls.Add(btnNext);
                _cardPanel.ResumeLayout(true);

                RecomputeHoleAndCardPosition();
            }

            private void BindTargetEvents(GuideStep step)
            {
                if (_currentTargetControl == null) return;

                if (step.AdvanceOnClick)
                {
                    EventHandler clickHandler = null;
                    clickHandler = (s, e) =>
                    {
                        _currentTargetControl.Click -= clickHandler;
                        AdvanceWithDelay();
                    };
                    _currentTargetControl.Click += clickHandler;
                    _cleanupListener = () => _currentTargetControl.Click -= clickHandler;
                }
                else if (step.AdvanceOnTextValidate != null && _currentTargetControl is TextBox tb)
                {
                    EventHandler textHandler = null;
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
                else if (step.AdvanceOnSelectionChange && _currentTargetControl is ComboBox cb)
                {
                    EventHandler selHandler = null;
                    selHandler = (s, e) =>
                    {
                        cb.SelectedIndexChanged -= selHandler;
                        AdvanceWithDelay();
                    };
                    cb.SelectedIndexChanged += selHandler;
                    _cleanupListener = () => cb.SelectedIndexChanged -= selHandler;
                }
            }

            private void AdvanceWithDelay()
            {
                var timer = new Timer { Interval = 250 };
                timer.Tick += (s, e) =>
                {
                    timer.Stop();
                    timer.Dispose();
                    if (_manager.IsActive) _manager.Next();
                };
                timer.Start();
            }

            private void ClearListener()
            {
                if (_cleanupListener != null)
                {
                    _cleanupListener();
                    _cleanupListener = null;
                }
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
                    var placement = _currentStep?.Placement ?? GuidePlacement.Bottom;

                    if (placement == GuidePlacement.Bottom && _targetHoleRect.Bottom + margin + cardH > ClientSize.Height)
                        placement = GuidePlacement.Top;
                    else if (placement == GuidePlacement.Top && _targetHoleRect.Top - margin - cardH < 0)
                        placement = GuidePlacement.Bottom;

                    switch (placement)
                    {
                        case GuidePlacement.Top:
                            y = _targetHoleRect.Top - cardH - margin;
                            x = _targetHoleRect.Left + (_targetHoleRect.Width - cardW) / 2;
                            break;
                        case GuidePlacement.Bottom:
                            y = _targetHoleRect.Bottom + margin;
                            x = _targetHoleRect.Left + (_targetHoleRect.Width - cardW) / 2;
                            break;
                        case GuidePlacement.Left:
                            x = _targetHoleRect.Left - cardW - margin;
                            y = _targetHoleRect.Top + (_targetHoleRect.Height - cardH) / 2;
                            break;
                        case GuidePlacement.Right:
                            x = _targetHoleRect.Right + margin;
                            y = _targetHoleRect.Top + (_targetHoleRect.Height - cardH) / 2;
                            break;
                    }

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
                    using (var pen = new Pen(Color.FromArgb(2, 132, 199), 2.5f))
                    {
                        e.Graphics.DrawRectangle(pen, _targetHoleRect);
                    }
                }
            }

            protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
            {
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
