(function (window, document, $) {
	'use strict';

	// Get a regular interval for drawing to the screen
	window.requestAnimFrame = (function (callback) {
		return window.requestAnimationFrame ||
			window.webkitRequestAnimationFrame ||
			window.mozRequestAnimationFrame ||
			window.oRequestAnimationFrame ||
			window.msRequestAnimaitonFrame ||
			function (callback) {
				window.setTimeout(callback, 1000 / 60);
			};
	})();

	/*
	 * Plugin Constructor
	 */

	var pluginName = 'jqSignature',
		defaults = {
			lineColor: '#222222',
			lineWidth: 1,
			border: '1px dashed #AAAAAA',
			background: '#FFFFFF',
			width: 300,
			height: 100,
			autoFit: false
		},
		canvasFixture = '<canvas></canvas>';

	function Signature(element, options) {
		// DOM elements/objects
		this.element = element;
		this.$element = $(this.element);
		this.canvas = false;
		this.$canvas = false;
		this.ctx = false;
		// Drawing state
		this.drawing = false;

		this.currentPos = {
			x: 0,
			y: 0
		};
		this.lastPos = this.currentPos;
		// Determine plugin settings
		this._data = this.$element.data();
		this.settings = $.extend({}, defaults, options, this._data);
		// Initialize the plugin
		this.init();
	}

	Signature.prototype = {
		// Initialize the signature canvas
		init: function () {
			// Set up the canvas
			this.$canvas = $(canvasFixture).appendTo(this.$element);
			this.$canvas.attr({
				width: this.settings.width,
				height: this.settings.height
			});
			this.$canvas.css({
				boxSizing: 'border-box',
				width: this.settings.width + 'px',
				height: this.settings.height + 'px',
				border: this.settings.border,
				background: this.settings.background,
				cursor: 'crosshair'
			});
			// Fit canvas to width of parent
			if (this.settings.autoFit === true) {
				this._resizeCanvas();
				// TO-DO - allow for dynamic canvas resizing 
				// (need to save canvas state before changing width to avoid getting cleared)
				// var timeout = false;
				// $(window).on('resize', $.proxy(function(e) {
				//   clearTimeout(timeout);
				//   timeout = setTimeout($.proxy(this._resizeCanvas, this), 250);
				// }, this));
			}
			this.canvas = this.$canvas[0];
			this.cxt = this.canvas.getContext('2d');
			this.x = []; //��¼����ƶ��ǵ�X����
			this.y = []; //��¼����ƶ��ǵ�Y����
			this.clickDrag = [];
			this.lock = false; //����ƶ�ǰ���ж�����Ƿ���
			this.storageColor = "#000000";
			this.cxt.lineJoin = "round"; //context.lineJoin - ָ�������߶ε����ӷ�ʽ
			this.cxt.lineWidth1 = 5; //�����Ŀ��
			var user = new WebUser();
			console.log(user.Name);
			if (user.Name == "����") {
				this.cxt.lineWidth1 = 2; //�����Ŀ��
				$("#select-font-size").val("2");
			}
			this.w = this.canvas.width; //ȡ�����Ŀ�
			this.h = this.canvas.height; //ȡ�����ĸ�
			this.touch = ("createTouch" in document); //�ж��Ƿ�Ϊ�ֳ��豸
			this.StartEvent = this.touch ? "touchstart" : "mousedown"; //֧�ִ���ʽʹ����Ӧ���¼����
			this.MoveEvent = this.touch ? "touchmove" : "mousemove";
			this.EndEvent = this.touch ? "touchend" : "mouseup";
			this._resetCanvas();
			// Set up mouse events
			this.$canvas.on('mousedown touchstart', $.proxy(function (e) {
				//var touch = this.touch ? e.touches[0] : e;
				//������Ƥ��
				var skno1 = $('#clr').attr('val');
				var xPos, yPos, rect;
				rect = this.canvas.getBoundingClientRect();
				var event = e;
				if (event.type.indexOf('touch') !== -1) { // event.constructor === TouchEvent
					xPos = event.touches[0].clientX - rect.left;
					yPos = event.touches[0].clientY - rect.top;
				}
				// Mouse event
				else {
					xPos = event.clientX - rect.left;
					yPos = event.clientY - rect.top;
				}


				var _x = xPos; //����ڻ����ϵ�x���꣬�Ի������Ͻ�Ϊ���
				var _y = yPos; //����ڻ����ϵ�y���꣬�Ի������Ͻ�Ϊ���
				if (skno1 == 1) {

					this.resetEraser(_x, _y);

				} else {
					this.cxt.strokeStyle = "#000000";
					this.cxt.lineWidth = this.cxt.lineWidth1;
					/*        
					this.drawing = true;
							this.lastPos = this.currentPos = this._getPosition(e);
					*/
					this.movePoint(_x, _y); //��¼���λ��
					this.drawPoint(); //����·��
				}
				this.lock = true;
			}, this));
			this.$canvas.on('mousemove touchmove', $.proxy(function (e) {
				//var touch = this.touch ? e.touches[0] : e;
				var skno1 = $('#clr').attr('val');
				if (this.lock) { //t.lockΪtrue��ִ��
					var xPos, yPos, rect;
					rect = this.canvas.getBoundingClientRect();
					var event = e;
					if (event.type.indexOf('touch') !== -1) { // event.constructor === TouchEvent
						xPos = event.touches[0].clientX - rect.left;
						yPos = event.touches[0].clientY - rect.top;
					}
					// Mouse event
					else {
						xPos = event.clientX - rect.left;
						yPos = event.clientY - rect.top;
					}


					var _x = xPos; //����ڻ����ϵ�x���꣬�Ի������Ͻ�Ϊ���
					var _y = yPos; //����ڻ����ϵ�y���꣬�Ի������Ͻ�Ϊ���							{
					if (skno1 == 1) {

						this.resetEraser(_x, _y);

					} else {
						this.cxt.strokeStyle = "#000000";
						this.cxt.lineWidth = this.cxt.lineWidth1;
						/*
								this.currentPos = this._getPosition(e);
						*/
						this.movePoint(_x, _y, true); //��¼���λ��
						this.drawPoint(); //����·��
					}
				}
			}, this));
			this.$canvas.on('mouseup touchend', $.proxy(function (e) {
				this.lock = false;
				this.x = [];
				this.y = [];
				this.clickDrag = [];
				this.drawing = false;
				// Trigger a change event
				var changedEvent = $.Event('jq.signature.changed');
				this.$element.trigger(changedEvent);
			}, this));
			// Prevent document scrolling when touching canvas
			$(document).on('mouseup touchend', $.proxy(function (e) {
				this.lock = false;
				this.x = [];
				this.y = [];
				this.clickDrag = [];
				this.drawing = false;
				// Trigger a change event
				var changedEvent = $.Event('jq.signature.changed');
				this.$element.trigger(changedEvent);
			}, this));

			$(document).on('touchstart touchmove touchend', $.proxy(function (e) {
				if (e.target === this.canvas) {
					e.preventDefault();
				}
			}, this));
			// Start drawing
			var that = this;
			(function drawLoop() {
				window.requestAnimFrame(drawLoop);
				that._renderCanvas();
			})();
		},
		// Clear the canvas
		clearCanvas: function () {
			this.canvas.width = this.canvas.width;
			this._resetCanvas();
		},
		// Get the content of the canvas as a base64 data URL
		getDataURL: function () {
			return this.canvas.toDataURL();
		},
		setfont: function () { //���������С
			var fontsize = $("#select-font-size").val();
			console.log(fontsize);
			this.cxt.lineWidth1 = parseInt(fontsize);
		},
		// Get the position of the mouse/touch
		_getPosition: function (event) {
			var xPos, yPos, rect;
			rect = this.canvas.getBoundingClientRect();
			event = event.originalEvent;
			// Touch event
			if (event.type.indexOf('touch') !== -1) { // event.constructor === TouchEvent
				xPos = event.touches[0].clientX - rect.left;
				yPos = event.touches[0].clientY - rect.top;
			}
			// Mouse event
			else {
				xPos = event.clientX - rect.left;
				yPos = event.clientY - rect.top;
			}
			return {
				x: xPos,
				y: yPos
			};
		},
		movePoint: function (x, y, dragging) {
			/*�����������ӵ����Զ�Ӧ��������*/
			this.x.push(x);
			this.y.push(y);
			this.clickDrag.push(y);
		},
		drawPoint: function (x, y, radius) {
			for (var i = 0; i < this.x.length; i++) //ѭ������
			{
				this.cxt.beginPath(); //context.beginPath() , ׼������һ��·��
				if (this.clickDrag[i] && i) { //�����϶�����i!=0ʱ������һ���㿪ʼ���ߡ�
					this.cxt.moveTo(this.x[i - 1], this.y[i - 1]); //context.moveTo(x, y) , �¿�һ��·������ָ��·�������
				} else {
					this.cxt.moveTo(this.x[i] - 1, this.y[i]);
				}
				this.cxt.lineTo(this.x[i], this.y[i]); //context.lineTo(x, y) , ����ǰ����ָ���ĵ���һ����ֱ��·����������
				this.cxt.closePath(); //context.closePath() , �����ǰ·���Ǵ򿪵���ر���
				this.cxt.stroke(); //context.stroke() , ���Ƶ�ǰ·��
			}
		},
		resetEraser: function (_x, _y) {
			/*ʹ����Ƥ��-����*/
			var t = this;

			//this.cxt.lineWidth = 30;
			/*source-over Ĭ��,�ཻ�����ɺ����ͼ�ε����(��ɫ,����,����)����,ȫ�������ͨ��*/
			t.cxt.globalCompositeOperation = "destination-out";
			t.cxt.beginPath();
			t.cxt.arc(_x, _y, this.cxt.lineWidth1 + 6, 0, Math.PI * 2);
			t.cxt.strokeStyle = "rgba(250,250,250,0)";
			t.cxt.fill();
			t.cxt.globalCompositeOperation = "source-over"
		},
		// Render the signature to the canvas
		_renderCanvas: function () {
			if (this.drawing) {
				this.ctx.moveTo(this.lastPos.x, this.lastPos.y);
				this.ctx.lineTo(this.currentPos.x, this.currentPos.y);
				this.ctx.stroke();
				this.lastPos = this.currentPos;
			}
		},
		// Reset the canvas context
		_resetCanvas: function () {
			//this.ctx = this.canvas.getContext("2d");
			// this.ctx.strokeStyle = "#000000";
			//this.ctx.lineWidth = 5;
		},
		// Resize the canvas element
		_resizeCanvas: function () {
			var width = this.$element.outerWidth();
			this.$canvas.attr('width', width);
			this.$canvas.css('width', width + 'px');
		}
	};

	/*
	 * Plugin wrapper and initialization
	 */

	$.fn[pluginName] = function (options) {
		var args = arguments;
		if (options === undefined || typeof options === 'object') {
			return this.each(function () {
				if (!$.data(this, 'plugin_' + pluginName)) {
					$.data(this, 'plugin_' + pluginName, new Signature(this, options));
				}
			});
		} else if (typeof options === 'string' && options[0] !== '_' && options !== 'init') {
			var returns;
			this.each(function () {
				var instance = $.data(this, 'plugin_' + pluginName);
				if (instance instanceof Signature && typeof instance[options] === 'function') {
					returns = instance[options].apply(instance, Array.prototype.slice.call(args, 1));
				}
				if (options === 'destroy') {
					$.data(this, 'plugin_' + pluginName, null);
				}
			});
			return returns !== undefined ? returns : this;
		}
	};

})(window, document, jQuery);