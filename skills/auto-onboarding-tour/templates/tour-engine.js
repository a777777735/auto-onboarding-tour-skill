/**
 * OnboardingTour.js - 零依賴原生互動式導覽引擎
 * 支援單檔 HTML、離線環境 (Zero-CDN)
 * 特色：
 *  - 平滑聚光燈挖洞效果 (box-shadow spotlight)
 *  - Tooltip 自動防溢出視窗邊界判定 (Auto-collision detection & placement flip)
 *  - resize / scroll 自動重算定位
 *  - 支援鍵盤快速鍵 (ArrowRight, ArrowLeft, Escape)
 *  - 支援 beforeShow 非同步鉤子 (切換 Tab / 開啟 Modal 等)
 *  - 支援 localStorage 首次自動啟動管理
 */

(function (global) {
  'use strict';

  // 預設樣式注入
  const DEFAULT_STYLES = `
    .aot-backdrop {
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

    .aot-spotlight {
      position: fixed;
      z-index: 999991;
      pointer-events: none;
      border-radius: 8px;
      box-shadow: 0 0 0 9999px rgba(15, 23, 42, 0.78);
      transition: top 0.3s cubic-bezier(0.16, 1, 0.3, 1),
                  left 0.3s cubic-bezier(0.16, 1, 0.3, 1),
                  width 0.3s cubic-bezier(0.16, 1, 0.3, 1),
                  height 0.3s cubic-bezier(0.16, 1, 0.3, 1),
                  border-radius 0.3s ease;
      outline: 2px solid rgba(59, 130, 246, 0.7);
      outline-offset: 2px;
    }

    .aot-tooltip {
      position: fixed;
      z-index: 999995;
      width: 320px;
      max-width: calc(100vw - 32px);
      background: #ffffff;
      color: #1e293b;
      border-radius: 12px;
      box-shadow: 0 20px 25px -5px rgba(0, 0, 0, 0.15), 0 8px 10px -6px rgba(0, 0, 0, 0.1);
      border: 1px solid rgba(226, 232, 240, 0.9);
      padding: 18px 20px;
      font-family: -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, "Helvetica Neue", Arial, sans-serif;
      font-size: 14px;
      line-height: 1.5;
      box-sizing: border-box;
      opacity: 0;
      transform: scale(0.95);
      transition: opacity 0.25s ease, transform 0.25s cubic-bezier(0.16, 1, 0.3, 1), top 0.3s cubic-bezier(0.16, 1, 0.3, 1), left 0.3s cubic-bezier(0.16, 1, 0.3, 1);
      pointer-events: auto;
    }

    .aot-tooltip.aot-visible {
      opacity: 1;
      transform: scale(1);
    }

    .aot-header {
      display: flex;
      align-items: center;
      justify-content: space-between;
      margin-bottom: 8px;
    }

    .aot-title {
      font-size: 16px;
      font-weight: 700;
      color: #0f172a;
      margin: 0;
    }

    .aot-close-btn {
      background: transparent;
      border: none;
      color: #94a3b8;
      cursor: pointer;
      font-size: 18px;
      line-height: 1;
      padding: 2px 6px;
      border-radius: 4px;
      transition: color 0.15s ease, background-color 0.15s ease;
    }
    .aot-close-btn:hover {
      color: #334155;
      background-color: #f1f5f9;
    }

    .aot-body {
      color: #475569;
      font-size: 13.5px;
      margin-bottom: 16px;
      white-space: normal;
      word-break: break-word;
    }

    .aot-progress-bar-wrap {
      width: 100%;
      height: 4px;
      background: #e2e8f0;
      border-radius: 999px;
      overflow: hidden;
      margin-bottom: 14px;
    }

    .aot-progress-bar-fill {
      height: 100%;
      background: #2563eb;
      border-radius: 999px;
      transition: width 0.3s ease;
    }

    .aot-footer {
      display: flex;
      align-items: center;
      justify-content: space-between;
      gap: 8px;
    }

    .aot-step-counter {
      font-size: 12px;
      font-weight: 600;
      color: #64748b;
    }

    .aot-buttons {
      display: flex;
      align-items: center;
      gap: 6px;
    }

    .aot-btn {
      border: none;
      border-radius: 6px;
      padding: 6px 12px;
      font-size: 12.5px;
      font-weight: 600;
      cursor: pointer;
      transition: all 0.15s ease;
      outline: none;
    }

    .aot-btn-skip {
      background: transparent;
      color: #64748b;
    }
    .aot-btn-skip:hover {
      color: #0f172a;
      background: #f1f5f9;
    }

    .aot-btn-prev {
      background: #f1f5f9;
      color: #334155;
    }
    .aot-btn-prev:hover {
      background: #e2e8f0;
    }

    .aot-btn-next {
      background: #2563eb;
      color: #ffffff;
      box-shadow: 0 1px 2px 0 rgba(0, 0, 0, 0.05);
    }
    .aot-btn-next:hover {
      background: #1d4ed8;
    }

    .aot-launcher-btn {
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
      color: #1e293b;
      background: #ffffff;
      border: 1px solid #cbd5e1;
      border-radius: 9999px;
      box-shadow: 0 4px 6px -1px rgba(0, 0, 0, 0.07), 0 2px 4px -2px rgba(0, 0, 0, 0.05);
      cursor: pointer;
      transition: all 0.2s ease;
      font-family: inherit;
    }
    .aot-launcher-btn:hover {
      background: #f8fafc;
      border-color: #94a3b8;
      box-shadow: 0 10px 15px -3px rgba(0, 0, 0, 0.1);
      transform: translateY(-1px);
    }
  `;

  class OnboardingTour {
    /**
     * @param {Object} options
     * @param {Array<Object>} options.steps
     * @param {string} [options.storageKey] - localStorage key
     * @param {number} [options.spotlightPadding=8] - 聚光燈外擴像素
     * @param {Function} [options.onComplete]
     * @param {Function} [options.onSkip]
     * @param {Function} [options.onStep]
     */
    constructor(options = {}) {
      this.steps = options.steps || [];
      this.storageKey = options.storageKey || 'aot_tour_completed';
      this.spotlightPadding = options.spotlightPadding ?? 8;
      this.onComplete = options.onComplete || null;
      this.onSkip = options.onSkip || null;
      this.onStep = options.onStep || null;

      this.currentIndex = -1;
      this.isActive = false;

      // DOM 節點
      this.backdropEl = null;
      this.spotlightEl = null;
      this.tooltipEl = null;

      // 事件綁定參照
      this._boundOnResize = this._onWindowResize.bind(this);
      this._boundOnKeyDown = this._onKeyDown.bind(this);

      this._injectStyles();
    }

    _injectStyles() {
      if (document.getElementById('aot-engine-styles')) return;
      const styleEl = document.createElement('style');
      styleEl.id = 'aot-engine-styles';
      styleEl.textContent = DEFAULT_STYLES;
      document.head.appendChild(styleEl);
    }

    _createDOM() {
      if (this.backdropEl) return;

      // 背景遮罩
      this.backdropEl = document.createElement('div');
      this.backdropEl.className = 'aot-backdrop';
      this.backdropEl.addEventListener('click', () => {
        // 點擊暗部不自動跳出，避免使用者誤觸
      });

      // 聚光燈挖洞
      this.spotlightEl = document.createElement('div');
      this.spotlightEl.className = 'aot-spotlight';

      // Tooltip 氣泡
      this.tooltipEl = document.createElement('div');
      this.tooltipEl.className = 'aot-tooltip';

      document.body.appendChild(this.backdropEl);
      document.body.appendChild(this.spotlightEl);
      document.body.appendChild(this.tooltipEl);
    }

    _removeDOM() {
      if (this.backdropEl && this.backdropEl.parentNode) {
        this.backdropEl.parentNode.removeChild(this.backdropEl);
      }
      if (this.spotlightEl && this.spotlightEl.parentNode) {
        this.spotlightEl.parentNode.removeChild(this.spotlightEl);
      }
      if (this.tooltipEl && this.tooltipEl.parentNode) {
        this.tooltipEl.parentNode.removeChild(this.tooltipEl);
      }
      this.backdropEl = null;
      this.spotlightEl = null;
      this.tooltipEl = null;
    }

    /**
     * 啟動導覽
     * @param {number} startIndex
     */
    start(startIndex = 0) {
      if (this.steps.length === 0) {
        console.warn('[OnboardingTour] 導覽步驟列表為空，無法啟動。');
        return;
      }
      this._createDOM();
      this.isActive = true;
      window.addEventListener('resize', this._boundOnResize);
      window.addEventListener('scroll', this._boundOnResize, true);
      window.addEventListener('keydown', this._boundOnKeyDown);

      this.goToStep(startIndex);
    }

    /**
     * 若為首次打開，則自動啟動導覽
     */
    startIfFirstTime() {
      if (!localStorage.getItem(this.storageKey)) {
        this.start(0);
      }
    }

    /**
     * 切換至指定步驟
     * @param {number} index
     */
    async goToStep(index) {
      if (index < 0 || index >= this.steps.length) return;
      this.currentIndex = index;
      const step = this.steps[index];

      // 執行 beforeShow 鉤子 (支援 async/await)
      if (typeof step.beforeShow === 'function') {
        try {
          await step.beforeShow(step);
        } catch (err) {
          console.error('[OnboardingTour] beforeShow 執行錯誤:', err);
        }
      }

      // 等待 DOM 更新渲染
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
      if (typeof this.onComplete === 'function') {
        this.onComplete();
      }
    }

    skip() {
      localStorage.setItem(this.storageKey, 'true');
      this.stop();
      if (typeof this.onSkip === 'function') {
        this.onSkip();
      }
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
      if (typeof target === 'string') {
        return document.querySelector(target);
      }
      if (target instanceof HTMLElement) {
        return target;
      }
      return null;
    }

    _renderStep(step) {
      const targetEl = this._getTargetElement(step.target);
      if (!targetEl) {
        console.warn(`[OnboardingTour] 找不到目標 DOM 元素: ${step.target}`);
        // 預設置中 Modal 式顯示
        this._renderCenterModal(step);
        return;
      }

      // 捲動目標元素至視野可見處
      targetEl.scrollIntoView({
        behavior: 'smooth',
        block: 'nearest',
        inline: 'nearest'
      });

      // 延遲更新幾毫秒，確保 smooth scroll 定位穩定
      setTimeout(() => {
        this._updatePositions(targetEl, step);
      }, 50);
    }

    _renderCenterModal(step) {
      const p = this.spotlightPadding;
      // 置中但隱藏 spotlight
      this.spotlightEl.style.top = '50%';
      this.spotlightEl.style.left = '50%';
      this.spotlightEl.style.width = '0px';
      this.spotlightEl.style.height = '0px';
      this.spotlightEl.style.borderRadius = '8px';

      this._updateTooltipContent(step);
      this.tooltipEl.style.top = '50%';
      this.tooltipEl.style.left = '50%';
      this.tooltipEl.style.transform = 'translate(-50%, -50%)';
      this.tooltipEl.classList.add('aot-visible');
    }

    _updatePositions(targetEl, step) {
      if (!this.isActive || !this.spotlightEl || !this.tooltipEl) return;

      const rect = targetEl.getBoundingClientRect();
      const p = this.spotlightPadding;

      // 更新聚光燈位置
      const spotTop = Math.max(0, rect.top - p);
      const spotLeft = Math.max(0, rect.left - p);
      const spotWidth = rect.width + (p * 2);
      const spotHeight = rect.height + (p * 2);

      this.spotlightEl.style.top = `${spotTop}px`;
      this.spotlightEl.style.left = `${spotLeft}px`;
      this.spotlightEl.style.width = `${spotWidth}px`;
      this.spotlightEl.style.height = `${spotHeight}px`;

      // 檢查圓角
      const computedRadius = window.getComputedStyle(targetEl).borderRadius;
      this.spotlightEl.style.borderRadius = computedRadius !== '0px' ? computedRadius : '8px';

      // 更新 Tooltip 內容
      this._updateTooltipContent(step);

      // 自動防溢出 Tooltip 座標計算
      this._positionTooltip(rect, step.placement || 'bottom');
    }

    _updateTooltipContent(step) {
      const isFirst = this.currentIndex === 0;
      const isLast = this.currentIndex === this.steps.length - 1;
      const total = this.steps.length;
      const current = this.currentIndex + 1;
      const progressPercent = Math.round((current / total) * 100);

      this.tooltipEl.innerHTML = `
        <div class="aot-header">
          <h4 class="aot-title">${step.title || ''}</h4>
          <button class="aot-close-btn" aria-label="Close" data-action="skip">&times;</button>
        </div>
        <div class="aot-progress-bar-wrap">
          <div class="aot-progress-bar-fill" style="width: ${progressPercent}%;"></div>
        </div>
        <div class="aot-body">${step.content || ''}</div>
        <div class="aot-footer">
          <span class="aot-step-counter">${current} / ${total}</span>
          <div class="aot-buttons">
            <button class="aot-btn aot-btn-skip" data-action="skip">跳過</button>
            ${!isFirst ? `<button class="aot-btn aot-btn-prev" data-action="prev">上一步</button>` : ''}
            <button class="aot-btn aot-btn-next" data-action="next">${isLast ? '完成' : '下一步'}</button>
          </div>
        </div>
      `;

      // 事件委派
      this.tooltipEl.querySelectorAll('[data-action]').forEach(btn => {
        btn.addEventListener('click', (e) => {
          const action = e.currentTarget.getAttribute('data-action');
          if (action === 'next') this.next();
          else if (action === 'prev') this.prev();
          else if (action === 'skip') this.skip();
        });
      });
    }

    /**
     * 智慧防溢出定位演算法
     */
    _positionTooltip(targetRect, preferredPlacement = 'bottom') {
      const tooltip = this.tooltipEl;
      tooltip.style.transform = 'scale(1)';
      tooltip.classList.add('aot-visible');

      const tooltipRect = tooltip.getBoundingClientRect();
      const margin = 14; // 與目標元素間隔
      const edgePadding = 12; // 視窗安全邊界
      const vw = window.innerWidth;
      const vh = window.innerHeight;

      // 候選方向
      let placement = preferredPlacement;

      // 計算各方向可行性
      const spaceAbove = targetRect.top;
      const spaceBelow = vh - targetRect.bottom;
      const spaceLeft = targetRect.left;
      const spaceRight = vw - targetRect.right;

      // 自動翻轉 (Flip)
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

      // 防止視窗邊界溢出 (Clamping)
      left = Math.max(edgePadding, Math.min(left, vw - tooltipRect.width - edgePadding));
      top = Math.max(edgePadding, Math.min(top, vh - tooltipRect.height - edgePadding));

      tooltip.style.top = `${top}px`;
      tooltip.style.left = `${left}px`;
    }

    _onWindowResize() {
      if (!this.isActive || this.currentIndex === -1) return;
      const step = this.steps[this.currentIndex];
      const targetEl = this._getTargetElement(step.target);
      if (targetEl) {
        this._updatePositions(targetEl, step);
      }
    }

    _onKeyDown(e) {
      if (!this.isActive) return;
      if (e.key === 'ArrowRight') {
        e.preventDefault();
        this.next();
      } else if (e.key === 'ArrowLeft') {
        e.preventDefault();
        this.prev();
      } else if (e.key === 'Escape') {
        e.preventDefault();
        this.skip();
      }
    }

    /**
     * 靜態方法：輔助建立「❓ 操作導覽」右上角觸發按鈕
     * @param {Object} [options]
     * @param {string} [options.text='❓ 操作導覽']
     * @param {Function} [options.onClick]
     * @returns {HTMLElement}
     */
    static createLauncherButton(options = {}) {
      const btn = document.createElement('button');
      btn.className = 'aot-launcher-btn';
      btn.innerHTML = `<span>❓</span> <span>${options.text || '操作導覽'}</span>`;
      btn.addEventListener('click', () => {
        if (typeof options.onClick === 'function') {
          options.onClick();
        }
      });
      document.body.appendChild(btn);
      return btn;
    }
  }

  // 匯出至全域
  global.OnboardingTour = OnboardingTour;

})(typeof window !== 'undefined' ? window : this);
