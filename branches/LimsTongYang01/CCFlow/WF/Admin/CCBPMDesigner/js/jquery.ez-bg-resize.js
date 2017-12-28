/******************************************************
    * jQuery plug-in
    * Easy Background Image Resizer
    * Developed by J.P. Given (http://johnpatrickgiven.com)
    * Useage: anyone so long as credit is left alone
******************************************************/

(function($) {
	// Global Namespace
    var jqez = {};

    // Define the plugin
    $.fn.ezBgResize = function(options) {
		
		// Set global to obj passed
		jqez = options;
		
		// If img option is string convert to array.
		// This is in preparation for accepting an slideshow of images.
		if (!$.isArray(jqez.img)) {
			var tmp_img = jqez.img;
			jqez.img = [tmp_img]
		}
		
		$("<img/>").attr("src", jqez.img).load(function() {
			jqez.width = this.width;
			jqez.height = this.height;
			
			// Create a unique div container
			$("body").append('<div id="jq_ez_bg"></div>');

			// Add the image to it.
			$("#jq_ez_bg").html('<img src="' + jqez.img[0] + '" width="' + jqez.width + '" height="' + jqez.height + '" border="0">');

			// First position object
	        $("#jq_ez_bg").css("visibility","hidden");

			// Overflow set to hidden so scroll bars don't mess up image size.
	        $("body").css({
	            "overflow":"hidden"
	        });

			resizeImage();
		});
    };

	$(window).bind("resize", function() {
		resizeImage();
	});
	
	// Actual resize function
    function resizeImage() {
	
        $("#jq_ez_bg").css({
            "position":"fixed",
            "top":"0px",
            "left":"0px",
            "z-index":"-1",
            "overflow":"hidden",
            "width":window.screen.availWidth + "px",
            "height":window.screen.availHeight + "px",
			"opacity" : jqez.opacity
        });
		
		// Image relative to its container
		$("#jq_ez_bg").children('img').css("position", "relative");

        // Resize the img object to the proper ratio of the window.
        var iw = $("#jq_ez_bg").children('img').width();
        var ih = $("#jq_ez_bg").children('img').height();
        
        if (window.screen.availWidth > window.screen.availHeight) {
            //console.log(iw, ih);
            if (iw > ih) {
                var fRatio = iw/ih;
                $("#jq_ez_bg").children('img').css("width",window.screen.availWidth + "px");
                $("#jq_ez_bg").children('img').css("height",Math.round(window.screen.availWidth * (1/fRatio)));

                var newIh = Math.round(window.screen.availWidth * (1/fRatio));

                if(newIh < window.screen.availHeight) {
                    var fRatio = ih/iw;
                    $("#jq_ez_bg").children('img').css("height",window.screen.availHeight);
                    $("#jq_ez_bg").children('img').css("width",Math.round(window.screen.availHeight * (1/fRatio)));
                }
            } else {
                var fRatio = ih/iw;
                $("#jq_ez_bg").children('img').css("height",window.screen.availHeight);
                $("#jq_ez_bg").children('img').css("width",Math.round(window.screen.availHeight * (1/fRatio)));
            }
        } else {
            var fRatio = ih/iw;
            $("#jq_ez_bg").children('img').css("height",window.screen.availHeight);
            $("#jq_ez_bg").children('img').css("width",Math.round(window.screen.availHeight * (1/fRatio)));
        }
		
		// Center the image
		if (typeof(jqez.center) == 'undefined' || jqez.center) {
			if ($("#jq_ez_bg").children('img').width() > window.screen.availWidth) {
				var this_left = ($("#jq_ez_bg").children('img').width() - window.screen.availWidth) / 2;
				$("#jq_ez_bg").children('img').css({
					"top"  : 0,
					"left" : -this_left
				});
			}
			if ($("#jq_ez_bg").children('img').height() > window.screen.availHeight) {
				var this_height = ($("#jq_ez_bg").children('img').height() - window.screen.availHeight) / 2;
				$("#jq_ez_bg").children('img').css({
					"left" : 0,
					"top" : -this_height
				});
			}
		}

        $("#jq_ez_bg").css({
			"visibility" : "visible"
		});

		// Allow scrolling again
		$("body").css({
            "overflow":"auto"
        });
		
        
    }
})(jQuery);