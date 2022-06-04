/// <reference path="jquery.js" />
/*
* bitmapcutter
* version: 1.0.0 (05/24/2009)
* @ jQuery v1.2.*
*
* Licensed under the GPL:
*   http://gplv3.fsf.org
*
* Copyright 2008, 2009 Jericho [ thisnamemeansnothing[at]gmail.com ] 
*  
*/
//using global variables to store the data
window.$bcglobal = {
    $originalSize: { width: 0, top: 0 },
    $zoomValue: 1.0,
    $thumbimg: null,
    $img: null,
    $cutter: null
};
$.extend(
    $.fn, {
        ///<summary>
        /// fetch css value
        ///</summary>
        ///<param name="c">style name</param>
        f: function (c) {
            return parseInt($(this).css(c));
        },
        ///<summary>
        /// preload bitmap
        ///</summary>
        ///<param name="callback">callback function, it'll be fired after the 'preload' has completed</param>
        loadBitmap: function (callback) {
            var me = this,
                  bitmapCutterHolder = new Image(),
                  src = me.attr('rel');
            bitmapCutterHolder.src = src;
            this.onCompleted = function () {
                $(me).attr('src', src);
                $bcglobal.$originalSize = { width: bitmapCutterHolder.width, height: bitmapCutterHolder.height };
                callback(src);
            };
            if ($.browser.msie) {
                if (bitmapCutterHolder.readyState == "complete") {
                    me.onCompleted();
                }
                else bitmapCutterHolder.onreadystatechange = function () {
                    if (this.readyState == "complete") {
                        me.onCompleted();
                    }
                }
            }
            else {
                bitmapCutterHolder.onload = me.onCompleted;
            }
        },
        ///<summary>
        /// zoom in or zoom out the bitmap
        ///<remarks>
        /// use the global variable 'zoomValue' as zoom rate
        ///</remarks>
        ///</summary>
        scaleBitmap: function () {
            return this.each(function () {
                var me = $(this),
                      os = $bcglobal.$originalSize,
                      zoomValue = $bcglobal.$zoomValue;

                me.hide();
                if (os.width > 0 && os.height > 0) {
                    me.height(os.height * zoomValue)
                          .width(os.width * zoomValue);
                }
                var p = me.parent(),
                      w = me.width(),
                      h = me.height(),
                      t = (p.height() - h) / 2,
                      l = (p.width() - w) / 2;
                me.css({ 'top': t, 'left': l }).show();
                $('.jquery-bitmapcutter-info').html(w + ' : ' + h);
            });
        },
        ///<summary>
        /// resize, drag and drop
        ///</summary>
        ///<param name="setting">parameters, include limited value(left and top),condition and callback function</param>
        dragndrop: function (setting) {
            var ps = $.fn.extend({
                limited: {
                    lw: { min: 0, max: 100 },
                    th: { min: 0, max: 100 }
                },
                handler: null,
                callback: function (e) { }
            }, setting);
            var dragndrop = {
                drag: function (e) {
                    var d = e.data.d;
                    var p = {
                        left: Math.min(Math.max(e.pageX + d.left, ps.limited.lw.min), ps.limited.lw.max),
                        top: Math.min(Math.max(e.pageY + d.top, ps.limited.th.min), ps.limited.th.max),
                        target: d.target
                    };
                    ps.callback(p);
                },
                drop: function (e) {
                    $("#cutterDiv").unbind('mousemove', dragndrop.drag).unbind('mouseup', dragndrop.drop);
                }
            };
            return this.each(function () {
                if (ps.handler == null) { ps.handler = $(this) };
                var handler = (typeof ps.handler == 'string' ? $(ps.handler) : ps.handler);
                handler.bind('mousedown', function (e) {
                    var data = {
                        target: $(this),
                        left: $(this).f('left') - e.pageX,
                        top: $(this).f('top') - e.pageY
                    };
                    $("#cutterDiv").bind('mousemove', { d: data }, dragndrop.drag).bind('mouseup', dragndrop.drop);
                });
            });
        },
        ///<summary>
        /// bitmap cutter, main function
        ///</summary>
        ///<param name="setting">parameters</param>
        bitmapCutter: function (settings) {
            var lang = {
                zoomout: 'Zoom out',
                zoomin: 'Zoom in',
                original: 'Original size',
                clockwise: 'Clockwise rotation({0} degrees)',
                counterclockwise: 'Counterclockwise rotation({0} degrees)',
                generate: '开始裁切!',
                process: 'Please wait, transaction is processing......',
                left: 'Left',
                right: 'Right',
                up: 'Up',
                down: 'Down'
            };
            var ps = $.fn.extend({
                src: '',
                renderTo: $(document.body),
                holderSize: { width: 300, height: 400 },
                cutterSize: { width: 70, height: 70 },
                zoomStep: 0.2,
                zoomIn: 2.0,
                zoomOut: 0.1,
                rotateAngle: 90,
                //pixel
                moveStep: 100,
                onGenerated: function (src) { },
                lang: lang
            }, settings);

            //fill parameters
            ps.lang = $.extend(lang, ps.lang);
            ps.lang.clockwise = ps.lang.clockwise.format(ps.rotateAngle);
            ps.lang.counterclockwise = ps.lang.counterclockwise.format(ps.rotateAngle);
            ///<sammary>
            /// zoom interface
            ///</sammary>
            ///<param name="zv">current zoom value</param>
            function izoom(zv) {
                $bcglobal.$zoomValue = zv;
                $bcglobal.$img.scaleBitmap($bcglobal.$zoomValue);
                $bcglobal.$thumbimg.scaleBitmap($bcglobal.$zoomValue);
                scissors.createThumbnail();
            }
            ///<sammary>
            /// image rotation interface
            ///</sammary>
            ///<param name="angle">rotate angle(degree)</param>
            function irotate(angle) {
                var img = $bcglobal.$img,
                  thumbimg = $bcglobal.$thumbimg,
                  zoomValue = $bcglobal.$zoomValue,
                //Use 'rel' but not 'src',  this helps to make sure that the 'src' attribute was not start with 'http://localhost:8888/...'(msie)
                  src = img.attr('rel');
                $.ajax({
                    url: 'scissors.axd',
                    dataType: 'json',
                    data: { action: 'RotateBitmap', src: src, angle: angle, t: Math.random() },
                    error: function (msg) {
                        location = location;
                    },
                    success: function (json) {
                        if (json.msg == 'success') {
                            $bcglobal.$originalSize = json.size;
                            //clear cache of img
                            src += '?t=' + Math.random();
                            img.attr('src', src).scaleBitmap();
                            thumbimg.attr('src', src).scaleBitmap();
                            scissors.createThumbnail();
                        }
                        else {
                            alert(json.msg);
                        }
                    }
                });
            }
            ///<sammary>
            /// image movement
            ///</sammary>
            ///<param name="direction">move direction(left, up, right, down)</param>
            function imove(direction) {
                var thumbimg = $bcglobal.$thumbimg,
                      img = $bcglobal.$img,
                      cutter = $bcglobal.$cutter,
                      w = img.width(),
                      h = img.height(),
                      l = img.f('left'),
                      t = img.f('top');

                if (w <= ps.holderSize.width && h <= ps.holderSize.height) {
                    return;
                }
                var limited = {
                    left: { min: Math.min(ps.holderSize.width - w, 0), max: Math.max(ps.holderSize.width - w, 0) },
                    top: { min: Math.min(ps.holderSize.height - h, 0), max: Math.max(ps.holderSize.height - h, 0) }
                };
                /*
                * it's really a weird thing that i cant use '
                *  img.animate({
                *       d: v
                *    }, function() {
                *       thumbimg.fadeIn();
                *        scissors.createThumbnail();
                *   });
                * '
                * here (d was the direction-'left' and 'top', v was the position data to be calculated!)
                * maybe it's the json to haunt me;-)
                */
                if (!img.is(':animated')) {
                    thumbimg.fadeOut();
                    var v = 0, d = {};
                    switch (direction) {
                        case 'left':
                            v = Math.min(limited.left.max, l + ps.moveStep);
                            d = { left: v };
                            break;
                        case 'right':
                            v = Math.max(limited.left.min, l - ps.moveStep);
                            d = { left: v };
                            break;
                        case 'up':
                            v = Math.min(limited.top.max, t + ps.moveStep);
                            d = { top: v };
                            break;
                        case 'down':
                            v = Math.max(limited.top.min, t - ps.moveStep);
                            d = { top: v };
                            break;
                    }

                    img.animate(d, function () {
                        thumbimg.fadeIn();
                        scissors.createThumbnail();
                    });
                }
            }
            var scissors = {
                createThumbnail: function () {
                    var thumbimg = $bcglobal.$thumbimg,
                          img = $bcglobal.$img,
                          cutter = $bcglobal.$cutter;

                    thumbimg.css({
                        'left': -cutter.f('left') + img.f('left'),
                        'top': -cutter.f('top') + img.f('top')
                    });
                },
                zoomin: function () {
                    //window.console && console.log('zoom value: %s', zoomValue);
                    izoom.call(this, Math.min($bcglobal.$zoomValue + ps.zoomStep, ps.zoomIn));
                },
                zoomout: function () {
                    //window.console && console.log('zoom value: %s', zoomValue);
                    izoom.call(this, Math.max($bcglobal.$zoomValue - ps.zoomStep, ps.zoomOut));
                },
                original: function () {
                    izoom.call(this, 1, 1);
                },
                clockwise: function () {
                    irotate.call(this, ps.rotateAngle);
                },
                counterclockwise: function (e) {
                    irotate.call(this, -ps.rotateAngle);
                },
                left: function () {
                    imove.call(this, 'left');
                },
                up: function () {
                    imove.call(this, 'up');
                },
                right: function () {
                    imove.call(this, 'right');
                },
                down: function () {
                    imove.call(this, 'down');
                }
            };
            ps.renderTo = (typeof ps.renderTo == 'string' ? $(ps.renderTo) : ps.renderTo);

            var $cl = $('<div class="jquery-bitmapcutter-cl" onselectstart="return false;"></div>').appendTo(ps.renderTo);
            var $cr = $('<div class="jquery-bitmapcutter-cr"></div>').appendTo(ps.renderTo);

            //bitmap holder
            var $holder = $('<div class="jquery-bitmapcutter-holder jquery-loader" />')
                                    .css(ps.holderSize)
                                        .appendTo($cl);

            //options
            var $opts = $('<div class="jquery-bitmapcutter-opts" >' +
                                    '<div class="r1c1"><a href="javascript:void(0)" onfocus="this.blur()" class="up">&nbsp</a></div>' +
                                    '<div class="r2c1">' +
                                        '<a href="javascript:void(0)" onfocus="this.blur()" class="zoomout">&nbsp</a>' +
                                        '<a href="javascript:void(0)" onfocus="this.blur()" class="zoomin">&nbsp</a>' +
                                        '<a href="javascript:void(0)" onfocus="this.blur()" class="left">&nbsp</a>' +
                                        '<a href="javascript:void(0)" onfocus="this.blur()" class="original">&nbsp</a>' +
                                        '<a href="javascript:void(0)" onfocus="this.blur()" class="right">&nbsp</a>' +
                                        '<a href="javascript:void(0)" onfocus="this.blur()" class="counterclockwise">&nbsp</a>' +
                                        '<a href="javascript:void(0)" onfocus="this.blur()" class="clockwise">&nbsp</a>' +
                                    '</div>' +
                                     '<div class="r3c1"><a href="javascript:void(0)" onfocus="this.blur()" class="down">&nbsp</a></div>' +
                                '</div>').insertAfter($holder);

            $opts.css('width', ps.holderSize.width)
                        .find('div.r2c1>a:eq(0)')
                            .css('margin-left', (ps.holderSize.width - (16 + 6) * 7 + 3) / 2);

            //informations of bitmap
            var $info = $('<div title="Size Of Bitmap" class="jquery-bitmapcutter-info" />')
                                .insertAfter($opts)
                                    .css('width', ps.holderSize.width);

            //cutter
            var cutterLeft = (ps.holderSize.width - ps.cutterSize.width) / 2;
            var cutterTop = (ps.holderSize.height - ps.cutterSize.height) / 2;
            var $cutter = $('<div id="cutterDiv" class="jquery-bitmapcutter-cutter" >&nbsp</div>')
                                    .css(ps.cutterSize)
                                        .css({
                                            'opacity': 0.4,
                                            'left': cutterLeft, 'top': cutterTop
                                        }).appendTo($holder);
            //initialize zoom value
            $bcglobal.$zoomValue = 1;

            //image
            var $img = $('<img alt="" rel="' + ps.src + '" />')
                                        .appendTo($holder);

            //thumbnail
            var $thumbimg = $('<img alt="" rel="' + ps.src + '" />')
                                        .appendTo(
                                            $('<div class="jquery-bitmapcutter-thumbnail" />')
                                                .css(ps.cutterSize)
                                                    .appendTo($cr)
                                                );

            var $generate = $('<a href="javascript:void(0)" class="generate" onfocus="this.blur()" >' + ps.lang.generate + '</a>')
                                        .appendTo($cr);

            var $newimg = $('<img class="jquery-bitmapcutter-newimg" alt="" src="" />')
                                        .appendTo($cr).hide();

            var $processed = $('<div class="process">' + ps.lang.process + '</div>')
                                            .hide()
                                                .appendTo($cr);

            $img.loadBitmap(function (e) {
                $img.scaleBitmap();
                $holder.removeClass('jquery-loader');

                var ks = {
                    k37: 'left',
                    k38: 'up',
                    k39: 'right',
                    k40: 'down',
                    k45: 'zoomout',
                    k61: 'zoomin'
                };

                $().keypress(function (e) {
                    var k = (e.keyCode || e.which);
                    //window.console && console.log('key code: %s', k);
                    if ((k >= 37 && k <= 40) || k == 45 || k == 61) {
                        var func = cceval('scissors.' + cceval('(ks.k' + k + ')') + '');
                        func();
                    }
                });

                $thumbimg.attr('src', $img.attr('src')).scaleBitmap();

                $opts.find('a').each(function () {
                    var me = $(this), c = me.attr('class');
                    me.attr('title', cceval('(ps.lang.' + c + ')'))
                        .bind('click', cceval('(scissors.' + c + ')'));
                });

                $cutter.dragndrop({
                    limited: {
                        lw: { min: 0, max: ps.holderSize.width - ps.cutterSize.width },
                        th: { min: 0, max: ps.holderSize.height - ps.cutterSize.height }
                    },
                    callback: function (e) {
                        $cutter.css({
                            left: e.left,
                            top: e.top
                        });
                        scissors.createThumbnail();
                    }
                });

                $generate.click(function () {
                    var me = $(this);

                    me.fadeOut();
                    $opts.fadeOut();
                    $cutter.fadeOut();
                    $info.hide();
                    $processed.fadeIn();
                    $.get('scissors.axd', {
                        action: 'GenerateBitmap',
                        src: ps.src,
                        zoom: $bcglobal.$zoomValue,
                        x: $thumbimg.f('left'),
                        y: $thumbimg.f('top'),
                        width: ps.cutterSize.width,
                        height: ps.cutterSize.height,
                        t: Math.random()
                    }, function (json) {
                        json = cceval("(" + json + ")");
                        if (json.msg == 'success') {
                            me.fadeIn();
                            $opts.fadeIn();
                            $cutter.fadeIn();
                            $info.show();
                            $processed.fadeOut();
                            $newimg.attr('src', json.src).show();
                            ps.onGenerated(json.src);
                        }
                        else {
                            alert(json.msg);
                        }
                    }
                    );
                });
                $bcglobal.$cutter = $cutter;
                $bcglobal.$img = $img;
                $bcglobal.$thumbimg = $thumbimg;
            });
        }
    });
///<summary>
/// text format(same as c-sharp,
/// e.g.: 'example {0}: {2}'.format('A','none'))
/// output: example A: none
///</summary>
String.prototype.format = function () {
    var args = arguments;
    return this.replace(/{(\d{1})}/g, function () {
        return args[arguments[1]];
    });
};