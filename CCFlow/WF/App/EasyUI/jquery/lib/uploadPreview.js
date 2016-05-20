(function($) {
    jQuery.fn.extend({
        uploadPreview: function(opts) {
            opts = jQuery.extend({
                imgDiv: "#imgDiv",
                imgType: ["gif", "jpeg", "jpg", "bmp", "png"],
                maxwidth: 0,
                maxheight: 0,
                imgurl: null,
                callback: function() { return false; }
            }, opts || {});
            var _self = this;
            var _this = $(this);
            var imgDiv = $(opts.imgDiv);

            imgDiv.width(opts.maxwidth);
            imgDiv.height(opts.maxheight);
            autoScaling = function() {
                if ($.browser.version == "7.0" || $.browser.version == "8.0") imgDiv.get(0).filters.item("DXImageTransform.Microsoft.AlphaImageLoader").sizingMethod = "image";
                var img_width = imgDiv.width();
                var img_height = imgDiv.height();
                if (img_width > opts.maxwidth || img_height > opts.maxheight) {
                    alert("图片大小不符合要求");
                    clearvalue(_this[0]);
                    _this.trigger("blur"); //失去控件焦点

                    imgDiv.hide();
                    return false;
                }

                if (img_width > 0 && img_height > 0) {
                    var rate = (opts.maxwidth / img_width < opts.maxheight / img_height) ? opts.maxwidth / img_width : opts.maxheight / img_height;
                    if (rate <= 1) {
                        if ($.browser.version == "7.0" || $.browser.version == "8.0") imgDiv.get(0).filters.item("DXImageTransform.Microsoft.AlphaImageLoader").sizingMethod = "scale";
                        imgDiv.width(img_width * rate);
                        imgDiv.height(img_height * rate);
                    } else {
                        imgDiv.width(img_width);
                        imgDiv.height(img_height);
                    }
                    var left = (opts.maxwidth - imgDiv.width()) * 0.5;
                    var top = (opts.maxheight - imgDiv.height()) * 0.5;
                    imgDiv.css({ "margin-left": left, "margin-top": top });
                    imgDiv.show();
                }
            }
            _this.change(function() {
                if (this.value) {
                    if (!RegExp("\.(" + opts.imgType.join("|") + ")$", "i").test(this.value.toLowerCase())) {
                        alert("图片类型必须是" + opts.imgType.join("，") + "中的一种");
                        this.value = "";
                        return false;
                    }
                    imgDiv.hide();
                    if ($.browser.msie) {
                        if ($.browser.version == "6.0") {
                            var img = $("<img />");
                            imgDiv.replaceWith(img);
                            imgDiv = img;

                            var image = new Image();
                            image.src = 'file:///' + this.value;

                            imgDiv.attr('src', image.src);
                            autoScaling();
                        }
                        else {
                            imgDiv.css({ filter: "progid:DXImageTransform.Microsoft.AlphaImageLoader(sizingMethod=image)" });
                            imgDiv.get(0).filters.item("DXImageTransform.Microsoft.AlphaImageLoader").sizingMethod = "image";
                            try {

                                imgDiv.get(0).filters.item('DXImageTransform.Microsoft.AlphaImageLoader').src = getPath(_this[0]);
                            } catch (e) {
                                alert("无效的图片文件！");
                                return;
                            }
                            setTimeout("autoScaling()", 100);

                        }
                    }
                    else {
                        var img = $("<img />");
                        imgDiv.replaceWith(img);
                        imgDiv = img;
                        imgDiv.attr('src', this.files.item(0).getAsDataURL());
                        imgDiv.css({ "vertical-align": "middle" });
                        setTimeout("autoScaling()", 100);
                    }
                }
            });
        }
    });
})(jQuery);
//获得上传控件的值，obj为上传控件对象
function getPath(obj) {
    if (obj) {
        if (window.navigator.userAgent.indexOf("MSIE") >= 1) {
            obj.select();
            return document.selection.createRange().text;
        }
        else if (window.navigator.userAgent.indexOf("Firefox") >= 1) {
            if (obj.files) {
                return obj.files.item(0).getAsDataURL();
            }
            return obj.value;
        }
        return obj.value;
    }
}
//清空上传控件的值，obj为上传控件对象
function clearvalue(obj) {
    obj.select();
    document.execCommand("delete");
}  
