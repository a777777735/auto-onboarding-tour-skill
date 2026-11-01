/**
 * InteractiveActionGuide.js (實戰互動導引引擎)
 * 特色：
 *  - 四周背景遮罩鎖死，中間聚光燈洞口完全開放真實操作 (Click-Through)
 *  - 支援真實事件監聽：點擊按鈕 (click)、輸入驗證 (input)、下拉切換 (change)
 *  - 使用者完成真實操作後自動進入下一步
 *  - 支援 beforeShow 非同步切換 Tab / 開啟 Modal
 *  - 智慧防溢出 Tooltip、鍵盤操作與離線零依賴 (Zero-CDN)
 */

(function (global) {
  'use strict';

  const DEFAULT_STYLES = `
    .iag-backdrop {
      position: fixed;
      top: 0;
      left: 0;
      width: 100vw;
      height: 100vh;
      z-index: 999990;
      pointer-events: auto;
      background: transparent;
      user-select: none;
      -webkit-user-select: none;
    }

    .iag-spotlight {
      position: fixed;
      z-index: 999991;
      pointer-events: none; /* 洞口區域本身不攔截滑鼠事件 */
      border-radius: 8px;
      box-shadow: 0 0 0 9999px rgba(15, 23, 42, 0.78);
      transition: top 0.28s cubic-bezier(0.16, 1, 0.3, 1),
                  left 0.28s cubic-bezier(0.16, 1, 0.3, 1),
                  width 0.28s cubic-bezier(0.16, 1, 0.3, 1),
                  height 0.28s cubic-bezier(0.16, 1, 0.3, 1),
                  border-radius 0.28s ease;
      outline: 2.5px solid rgba(2, 132, 199, 0.85);
      outline-offset: 3px;
    }

    /* 目標控制項穿透高亮樣式 (置於 backdrop 之上以接收所有原生事件) */
    .iag-target-active {
      position: relative !important;
      z-index: 999993 !important;
      pointer-events: auto !important;
      transition: box-shadow 0.2s ease;
    }

    .iag-tooltip {
      position: fixed;
      z-index: 999996;
      width: 320px;
      max-width: calc(100vw - 32px);
      background: #ffffff;
      color: #0f172a;
      border-radius: 12px;
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.25), 0 8px 10px -6px rgba(0, 0, 0, 0.15);
      border: 1px solid rgba(226, 232, 240, 0.95);
      padding: 16px 18px;
      font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
      font-size: 13.5px;
      line-height: 1.5;
      box-sizing: border-box;
      opacity: 0;
      transform: scale(0.96);
      transition: opacity 0.25s ease, transform 0.25s cubic-bezier(0.16, 1, 0.3, 1), top 0.28s cubic-bezier(0.16, 1, 0.3, 1), left 0.28s cubic-bezier(0.16, 1, 0.3, 1);
      pointer-events: auto;
    }

    .iag-tooltip.iag-visible {
      opacity: 1;
      transform: scale(1);
    }

    .iag-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 6px;
    }

    .iag-title {
      font-size: 15px;
      font-weight: 700;
      color: #0f172a;
      margin: 0;
      display: flex;
      align-items: center;
      gap: 6px;
    }

    .iag-close-btn {
      background: transparent;
      border: none;
      color: #94a3b8;
      cursor: pointer;
      font-size: 18px;
      line-height: 1;
      padding: 2px 4px;
      border-radius: 4px;
      transition: all 0.15s ease;
    }
    .iag-close-btn:hover {
      color: #0f172a;
      background: #f1f5f9;
    }

    .iag-progress-bar-wrap {
      width: 100%;
      height: 4px;
      background: #e2e8f0;
      border-radius: 999px;
      overflow: hidden;
      margin-bottom: 12px;
    }

    .iag-progress-bar-fill {
      height: 100%;
      background: #0284c7;
      border-radius: 999px;
      transition: width 0.3s ease;
    }

    .iag-body {
      color: #475569;
      font-size: 13px;
      margin-bottom: 12px;
      word-break: break-word;
    }

    .iag-action-badge {
      display: inline-flex;
      align-items: center;
      gap: 5px;
      background: #f0f9ff;
      color: #0369a1;
      border: 1px solid #bae6fd;
      border-radius: 6px;
      padding: 4px 8px;
      font-size: 12px;
      font-weight: 600;
      margin-bottom: 12px;
    }

    .iag-footer {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 8px;
    }

    .iag-step-counter {
      font-size: 12px;
      font-weight: 600;
      color: #64748b;
    }

    .iag-buttons {
      display: flex;
      align-items: center;
      gap: 6px;
    }

    .iag-btn {
      border: none;
      border-radius: 6px;
      padding: 6px 12px;
      font-size: 12.5px;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.15s ease;
      outline: none;
    }

    .iag-btn-skip {
      background: transparent;
      color: #64748b;
    }
    .iag-btn-skip:hover {
      color: #0f172a;
      background: #f1f5f9;
    }

    .iag-btn-prev {
      background: #f1f5f9;
      color: #334155;
    }
    .iag-btn-prev:hover {
      background: #e2e8f0;
    }

    .iag-btn-next {
      background: #0284c7;
      color: #ffffff;
    }
    .iag-btn-next:hover {
      background: #0369a1;
    }

    .iag-launcher-btn {
      position: fixed;
      top: 16px;
      right: 16px;
      z-index: 999900;
      display: inline-flex;
      align-items: center;
      gap: 6px;
      padding: 8px 14px;
      font-size: 13px;
      font-weight: 600;
      color: #0f172a;
      background: #ffffff;
      border: 1px solid #cbd5e1;
      border-radius: 9999px;
      box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.08);
      cursor: pointer;
      transition: all 0.2s ease;
      font-family: inherit;
    }
    .iag-launcher-btn:hover {
      background: #f8fafc;
      border-color: #94a3b8;
      box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.12);
      transform: translateY(-1px);
    }
  `;

  class InteractiveActionGuide {
    /**
     * @param {Object} options
     * @param {Array<Object>} [options.steps]
     * @param {Object} [options.modes] - 支援雙模式 { concise: [...], detailed: [...] }
     * @param {string} [options.defaultMode='concise']
     * @param {string} [options.storageKey]
     * @param {number} [options.spotlightPadding=8]
     * @param {Function} [options.onComplete]
     * @param {Function} [options.onSkip]
     * @param {Function} [options.onStep]
     */
    constructor(options = {}) {
      this.modes = options.modes || null;
      this.currentMode = options.defaultMode || 'concise';
      this.steps = this.modes ? (this.modes[this.currentMode] || []) : (options.steps || []);
      this.storageKey = options.storageKey || 'iag_guide_completed';
      this.spotlightPadding = options.spotlightPadding ?? 8;
      this.onComplete = options.onComplete || null;
      this.onSkip = options.onSkip || null;
      this.onStep = options.onStep || null;

      this.currentIndex = -1;
      this.isActive = false;

      // DOM 元素
      this.backdropEl = null;
      this.spotlightEl = null;
      this.tooltipEl = null;
      this.activeTargetEl = null;
      this.activeCleanupListener = null;

      // 事件綁定參照
      this._boundOnResize = this._onWindowResize.bind(this);
      this._boundOnKeyDown = this._onKeyDown.bind(this);

      this._injectStyles();
    }

    _injectStyles() {
      if (document.getElementById('iag-engine-styles')) return;
      const styleEl = document.createElement('style');
      styleEl.id = 'iag-engine-styles';
      styleEl.textContent = DEFAULT_STYLES;
      document.head.appendChild(styleEl);
    }

    _createDOM() {
      if (this.backdropEl) return;

      this.backdropEl = document.createElement('div');
      this.backdropEl.className = 'iag-backdrop';

      this.spotlightEl = document.createElement('div');
      this.spotlightEl.className = 'iag-spotlight';

      this.tooltipEl = document.createElement('div');
      this.tooltipEl.className = 'iag-tooltip';

      document.body.appendChild(this.backdropEl);
      document.body.appendChild(this.spotlightEl);
      document.body.appendChild(this.tooltipEl);
    }

    _removeDOM() {
      this._clearActiveTarget();
      if (this.backdropEl && this.backdropEl.parentNode) this.backdropEl.parentNode.removeChild(this.backdropEl);
      if (this.spotlightEl && this.spotlightEl.parentNode) this.spotlightEl.parentNode.removeChild(this.spotlightEl);
      if (this.tooltipEl && this.tooltipEl.parentNode) this.tooltipEl.parentNode.removeChild(this.tooltipEl);
      this.backdropEl = null;
      this.spotlightEl = null;
      this.tooltipEl = null;
    }

    _clearActiveTarget() {
      if (this.activeCleanupListener) {
        this.activeCleanupListener();
        this.activeCleanupListener = null;
      }
      if (this.activeTargetEl) {
        this.activeTargetEl.classList.remove('iag-target-active');
        this.activeTargetEl = null;
      }
    }

    /**
     * 啟動導覽
     * @param {number|string} [modeOrIndex=0] - 支援傳入步驟數字或模式名稱 ('concise' | 'detailed')
     */
    start(modeOrIndex = 0) {
      if (typeof modeOrIndex === 'string' && this.modes && this.modes[modeOrIndex]) {
        this.currentMode = modeOrIndex;
        this.steps = this.modes[this.currentMode] || [];
        modeOrIndex = 0;
      }
      if (this.steps.length === 0) return;
      this._createDOM();
      this.isActive = true;
      window.addEventListener('resize', this._boundOnResize);
      window.addEventListener('scroll', this._boundOnResize, true);
      window.addEventListener('keydown', this._boundOnKeyDown);

      this.goToStep(typeof modeOrIndex === 'number' ? modeOrIndex : 0);
    }

    /**
     * 動態切換模式 (concise | detailed)
     * @param {'concise'|'detailed'} newMode
     */
    switchMode(newMode) {
      if (!this.modes || !this.modes[newMode]) return;
      this.currentMode = newMode;
      this.steps = this.modes[newMode];
      if (this.isActive) {
        this.goToStep(0);
      }
    }

    startIfFirstTime() {
      if (!localStorage.getItem(this.storageKey)) {
        this.start(0);
      }
    }

    async goToStep(index) {
      if (index < 0 || index >= this.steps.length) return;
      this._clearActiveTarget();
      this.currentIndex = index;
      const step = this.steps[index];

      // 執行 beforeShow 鉤子 (例如切換 Tab)
      if (typeof step.beforeShow === 'function') {
        try {
          await step.beforeShow(step);
        } catch (err) {
          console.error('[ActionGuide] beforeShow 執行錯誤:', err);
        }
      }

      requestAnimationFrame(() => {
        this._renderStep(step);
        if (typeof this.onStep === 'function') {
          this.onStep(this.currentIndex, step);
        }
      });
    }

    next() {
      if (this.currentIndex < this.steps.length - 1) {
        this.goToStep(this.currentIndex + 1);
      } else {
        this.complete();
      }
    }

    prev() {
      if (this.currentIndex > 0) {
        this.goToStep(this.currentIndex - 1);
      }
    }

    complete() {
      localStorage.setItem(this.storageKey, 'true');
      this.stop();
      if (typeof this.onComplete === 'function') this.onComplete();
    }

    skip() {
      localStorage.setItem(this.storageKey, 'true');
      this.stop();
      if (typeof this.onSkip === 'function') this.onSkip();
    }

    stop() {
      this.isActive = false;
      window.removeEventListener('resize', this._boundOnResize);
      window.removeEventListener('scroll', this._boundOnResize, true);
      window.removeEventListener('keydown', this._boundOnKeyDown);
      this._removeDOM();
    }

    _getTargetElement(target) {
      if (!target) return null;
      if (typeof target === 'string') return document.querySelector(target);
      if (target instanceof HTMLElement) return target;
      return null;
    }

    _renderStep(step) {
      const targetEl = this._getTargetElement(step.target);
      if (!targetEl) {
        this._renderCenterModal(step);
        return;
      }

      // 穿透操作設定：將目標控制項提升至遮罩最上層
      targetEl.classList.add('iag-target-active');
      this.activeTargetEl = targetEl;

      // 註冊真實互動事件監聽 (advanceOn)
      this._bindAdvanceListener(targetEl, step);

      targetEl.scrollIntoView({ behavior: 'smooth', block: 'nearest', inline: 'nearest' });

      setTimeout(() => {
        this._updatePositions(targetEl, step);
      }, 50);
    }

    _bindAdvanceListener(targetEl, step) {
      if (!step.advanceOn) return;

      const eventType = typeof step.advanceOn === 'string' ? step.advanceOn : (step.advanceOn.type || 'click');
      const validator = typeof step.advanceOn === 'object' && typeof step.advanceOn.validate === 'function' ? step.advanceOn.validate : null;

      const handler = (e) => {
        if (validator) {
          if (!validator(e, targetEl)) return;
        }
        // 操作完成微動畫與自動推進
        this.spotlightEl.style.outlineColor = '#10b981'; // 轉為綠色表示動作完成
        setTimeout(() => {
          if (this.isActive) this.next();
        }, 300);
      };

      targetEl.addEventListener(eventType, handler);
      this.activeCleanupListener = () => {
        targetEl.removeEventListener(eventType, handler);
      };
    }

    _renderCenterModal(step) {
      this.spotlightEl.style.width = '0px';
      this.spotlightEl.style.height = '0px';
      this._updateTooltipContent(step);
      this.tooltipEl.style.top = '50%';
      this.tooltipEl.style.left = '50%';
      this.tooltipEl.style.transform = 'translate(-50%, -50%)';
      this.tooltipEl.classList.add('iag-visible');
    }

    _updatePositions(targetEl, step) {
      if (!this.isActive || !this.spotlightEl || !this.tooltipEl) return;

      const rect = targetEl.getBoundingClientRect();
      const p = this.spotlightPadding;

      this.spotlightEl.style.outlineColor = 'rgba(2, 132, 199, 0.85)';
      this.spotlightEl.style.top = `${Math.max(0, rect.top - p)}px`;
      this.spotlightEl.style.left = `${Math.max(0, rect.left - p)}px`;
      this.spotlightEl.style.width = `${rect.width + (p * 2)}px`;
      this.spotlightEl.style.height = `${rect.height + (p * 2)}px`;

      const computedRadius = window.getComputedStyle(targetEl).borderRadius;
      this.spotlightEl.style.borderRadius = computedRadius !== '0px' ? computedRadius : '8px';

      this._updateTooltipContent(step);
      this._positionTooltip(rect, step.placement || 'bottom');
    }

    _updateTooltipContent(step) {
      const isFirst = this.currentIndex === 0;
      const isLast = this.currentIndex === this.steps.length - 1;
      const total = this.steps.length;
      const current = this.currentIndex + 1;
      const progressPercent = Math.round((current / total) * 100);

      let actionHint = '';
      if (step.advanceOn) {
        const type = typeof step.advanceOn === 'string' ? step.advanceOn : step.advanceOn.type;
        const msg = step.advanceOn.message || (type === 'click' ? '請直接點擊上方高亮按鈕完成動作' : type === 'input' ? '請在上方輸入框完成輸入' : '請切換上方選單以繼續');
        actionHint = `<div class="iag-action-badge">🎯 ${msg}</div>`;
      }

      let modeToggleHtml = '';
      if (this.modes) {
        modeToggleHtml = `
          <div style="display:flex;gap:4px;margin-bottom:8px;">
            <button data-action="mode-concise" style="font-size:11px;padding:2px 6px;border-radius:4px;border:1px solid ${this.currentMode === 'concise' ? '#0284c7' : '#cbd5e1'};background:${this.currentMode === 'concise' ? '#f0f9ff' : '#fff'};color:${this.currentMode === 'concise' ? '#0284c7' : '#64748b'};cursor:pointer;font-weight:600;">⚡ 收斂精簡 (4-6步)</button>
            <button data-action="mode-detailed" style="font-size:11px;padding:2px 6px;border-radius:4px;border:1px solid ${this.currentMode === 'detailed' ? '#0284c7' : '#cbd5e1'};background:${this.currentMode === 'detailed' ? '#f0f9ff' : '#fff'};color:${this.currentMode === 'detailed' ? '#0284c7' : '#64748b'};cursor:pointer;font-weight:600;">📖 詳細全覆蓋</button>
          </div>
        `;
      }

      this.tooltipEl.innerHTML = `
        <div class="iag-header">
          <h4 class="iag-title"><span>👉</span> ${step.title || ''}</h4>
          <button class="iag-close-btn" aria-label="Close" data-action="skip">&times;</button>
        </div>
        ${modeToggleHtml}
        <div class="iag-progress-bar-wrap">
          <div class="iag-progress-bar-fill" style="width: ${progressPercent}%;"></div>
        </div>
        <div class="iag-body">${step.content || ''}</div>
        ${actionHint}
        <div class="iag-footer">
          <span class="iag-step-counter">${current} / ${total}</span>
          <div class="iag-buttons">
            <button class="iag-btn iag-btn-skip" data-action="skip">跳過</button>
            ${!isFirst ? `<button class="iag-btn iag-btn-prev" data-action="prev">上一步</button>` : ''}
            <button class="iag-btn iag-btn-next" data-action="next">${isLast ? '完成' : '下一步'}</button>
          </div>
        </div>
      `;

      this.tooltipEl.querySelectorAll('[data-action]').forEach(btn => {
        btn.addEventListener('click', (e) => {
          const action = e.currentTarget.getAttribute('data-action');
          if (action === 'next') this.next();
          else if (action === 'prev') this.prev();
          else if (action === 'skip') this.skip();
          else if (action === 'mode-concise') this.switchMode('concise');
          else if (action === 'mode-detailed') this.switchMode('detailed');
        });
      });
    }

    _positionTooltip(targetRect, preferredPlacement = 'bottom') {
      const tooltip = this.tooltipEl;
      tooltip.style.transform = 'scale(1)';
      tooltip.classList.add('iag-visible');

      const tooltipRect = tooltip.getBoundingClientRect();
      const margin = 14;
      const edgePadding = 12;
      const vw = window.innerWidth;
      const vh = window.innerHeight;

      let placement = preferredPlacement;
      const spaceAbove = targetRect.top;
      const spaceBelow = vh - targetRect.bottom;
      const spaceLeft = targetRect.left;
      const spaceRight = vw - targetRect.right;

      if (placement === 'bottom' && spaceBelow < tooltipRect.height + margin && spaceAbove > tooltipRect.height + margin) {
        placement = 'top';
      } else if (placement === 'top' && spaceAbove < tooltipRect.height + margin && spaceBelow > tooltipRect.height + margin) {
        placement = 'bottom';
      } else if (placement === 'right' && spaceRight < tooltipRect.width + margin && spaceLeft > tooltipRect.width + margin) {
        placement = 'left';
      } else if (placement === 'left' && spaceLeft < tooltipRect.width + margin && spaceRight > tooltipRect.width + margin) {
        placement = 'right';
      }

      let top = 0;
      let left = 0;

      if (placement === 'top') {
        top = targetRect.top - tooltipRect.height - margin;
        left = targetRect.left + (targetRect.width / 2) - (tooltipRect.width / 2);
      } else if (placement === 'bottom') {
        top = targetRect.bottom + margin;
        left = targetRect.left + (targetRect.width / 2) - (tooltipRect.width / 2);
      } else if (placement === 'left') {
        top = targetRect.top + (targetRect.height / 2) - (tooltipRect.height / 2);
        left = targetRect.left - tooltipRect.width - margin;
      } else if (placement === 'right') {
        top = targetRect.top + (targetRect.height / 2) - (tooltipRect.height / 2);
        left = targetRect.right + margin;
      }

      left = Math.max(edgePadding, Math.min(left, vw - tooltipRect.width - edgePadding));
      top = Math.max(edgePadding, Math.min(top, vh - tooltipRect.height - edgePadding));

      tooltip.style.top = `${top}px`;
      tooltip.style.left = `${left}px`;
    }

    _onWindowResize() {
      if (!this.isActive || this.currentIndex === -1) return;
      const step = this.steps[this.currentIndex];
      const targetEl = this._getTargetElement(step.target);
      if (targetEl) this._updatePositions(targetEl, step);
    }

    _onKeyDown(e) {
      if (!this.isActive) return;
      if (e.key === 'Escape') {
        e.preventDefault();
        this.skip();
      }
    }

    static createLauncherButton(options = {}) {
      const btn = document.createElement('button');
      btn.className = 'iag-launcher-btn';
      btn.innerHTML = `<span>❓</span> <span>${options.text || '操作導覽'}</span>`;
      btn.addEventListener('click', () => {
        if (typeof options.onClick === 'function') options.onClick();
      });
      document.body.appendChild(btn);
      return btn;
    }
  }

  global.InteractiveActionGuide = InteractiveActionGuide;
  // 向下相容別名
  global.OnboardingTour = InteractiveActionGuide;

})(typeof window !== 'undefined' ? window : this);
