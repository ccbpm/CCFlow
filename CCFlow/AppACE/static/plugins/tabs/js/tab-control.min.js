(function() {
    var b = jQuery("div.tab-control");
    var s = jQuery("div.tab-control div.tab");
    var t = jQuery("div.tab-control div.tab ul");
    var k = jQuery("div.tab-control div.tab input.prev");
    var l = jQuery("div.tab-control div.tab input.next");
    var j = jQuery("div.tab-control div.tab input.find");
    var o = jQuery("div.tab-control div.tab-find");
    var f = jQuery("div.tab-control div.tab-find form");
    var r = jQuery("div.tab-control div.tab-find ul");
    var n = jQuery("div.tab-control div.tab-find input.text");
    var a = jQuery("div.tab-control div.main");
    var v = function(A) {
        for (var z = jQuery(t).find("li"), x = jQuery(t).width(), y = 0; z[y]; y++, A += 120) {
            z[y].style.cssText = "";
            if (A < 0) {
                z[y].style.marginLeft = ( - 120 < A ? A: -120) + "px";
            }
            if (A < x && x < A + 120) {
                z[y].style.marginRight = "-120px";
                
            }
        }
    };
    var g = function(A) {
        var z = jQuery(t).find("li");
        if (A === undefined) {
            for (var y = 0, x = 0; z[y].style.marginLeft; y++) {
                x += parseInt(z[y].style.marginLeft);
            }
        } else {
            if (z[A].style.marginLeft) {
                for (var y = A, x = 0; z[y].style.marginLeft; y++) {
                    x += parseInt(z[y].style.marginLeft || 0);
                }
            } else {
                for (var y = 0, x = 120 - jQuery(t).width(); y != A; y++) {
                    x += parseInt(z[y].style.marginLeft || 0) + 120;
                }
            }
        }
        return x;
    };
    var c = function(A) {
        var z = jQuery(t).find("li");
        var y = window.setInterval(function() {
            var C = g();
            var B = g(z.length - 1);
            if (A > 0) {
                v(C - C / 10);
            } else {
                v(C - (B + 4) / 10);
            }
        },
        20);
        var x = function() {
            window.clearInterval(y);
            jQuery(b).unbind("mouseup", x);
            jQuery(k).attr("class", "prev" + (z[0].style.marginLeft ? " scroll": ""));
            jQuery(l).attr("class", "next" + (g(z.length - 1) > 5 ? " scroll": ""));
        };
        jQuery(b).mouseup(x);
    };
    var p = function(A, z, B, C) {
        var x = jQuery(t).find("li");
        var E = jQuery(a).find("iframe");
        for (var D = -1, y = 0; x[y]; y++) {
            if (x[y].getAttribute("index") == A) {
                D = y;
            }
            x[y].className = "visited";
            E[y].className = "visited";
        }
        if (D > -1) {
            x[D].className = "hover";
            E[D].className = "";
            if (E[D].getAttribute("reload") == "1") {
                E[D].contentWindow.location.reload(true);
            }
        } else {
            if (z) {
                jQuery(t).append('<li index="' + A + '" tab="' + z + '" class="hover">' + z + '<a href="javascript:;">\u5173\u95ed</a></li>');
                jQuery(r).append('<li index="' + A + '" tab="' + z + '">' + z + '<a href="javascript:;">\u5173\u95ed</a></li>');
                jQuery(a).append('<iframe src="' + B + '" scrolling="auto" frameborder="0" reload="' + C + '"></iframe>');
            }
        }
        i();
    };
    var w = function(z) {
        var x = jQuery(t).find("li");
        var B = jQuery(r).find("li");
        var A = jQuery(a).find("iframe");
        for (var y = 0; x[y]; y++) {
            if (x[y].getAttribute("index") == z) {
                x[y].parentNode.removeChild(x[y]);
                B[y].parentNode.removeChild(B[y]);
                A[y].parentNode.removeChild(A[y]);
                if (x[y].className == "hover") {
                	if(y==0)return;//修改Bug
                    p(x[y ? y - 1: y + 1].getAttribute("index"));
                }
            }
        }
        i();
    };
    var i = function(B) {
        var A = jQuery(t).find("li");
        if (!A.length) {
            return
        }
        if (A.length * 120 > jQuery(t).width()) {
            jQuery(s).attr("class", "tab");
        } else {
            jQuery(s).attr("class", "tab simple");
        }
        for (var z = 0; A[z]; z++) {
            if (A[z].className == "hover") {
                var y = g(),
                x = g(z),
                C = g(A.length - 1);
                if (A[z].style.marginLeft) {
                    v(x - y);
                } else {
                    if (x > 0) {
                        v(y - x);
                    } else {
                        if (C < 0) {
                            v(y - C);
                        } else {
                            v(y);
                        }
                    }
                }
                break;
            }
        }
        jQuery(k).attr("class", "prev" + (A[0].style.marginLeft ? " scroll": ""));
        jQuery(l).attr("class", "next" + (g(A.length - 1) > 5 ? " scroll": ""));
        jQuery(a).css("height", (jQuery(document).height() - 120) + "px");
    };
    var d = function(y) {
        var x = y.target;
        if (x.tagName == "LI") {
            p(x.getAttribute("index"));
            jQuery(o).attr("class", "tab-find hidden");
        }
        if (x.innerHTML == "\u5173\u95ed") {
            w(x.parentNode.getAttribute("index"));
            jQuery(o).attr("class", "tab-find hidden");
        }
    };
    var q = function(y) {
        var x = y.target;
        if (x.className == "prev scroll") {
            c(1);
        }
        if (x.className == "next scroll") {
            c( - 1);
        }
    };
    var m = function(A) {
        for (var z = jQuery(r).find("li"), x = jQuery(n).val(), y = 0; z[y]; y++) {
            z[y].className = (x && z[y].getAttribute("tab").toLowerCase().indexOf(x.toLowerCase()) > -1);
        }
        jQuery(f).attr("class", x ? "visited": "");
    };
    var e = function(x) {
        jQuery(f).attr("class", "hover");
    };
    var u = function(z) {
        try {
            var y = z.target;
            if (y.className == "find") {
                jQuery(o).attr("class", "tab-find");
                jQuery(f).attr("class", jQuery(n).val() ? "hover": "");
            } else {
                for (y; y; y = y.parentNode) {
                    if (y.className.indexOf("tab-find") > -1) {
                        return;
                    }
                }
            }
        } catch(x) {
            jQuery(o).attr("class", "tab-find hidden");
        }
    };
    var h = function(A) {
        for (var z = jQuery(r).find("li"), x = jQuery(n).val(), y = 0; z[y]; y++) {
            if (x && z[y].getAttribute("tab").toLowerCase().indexOf(x.toLowerCase()) > -1) {
                p(z[y].getAttribute("index"));
                jQuery(o).attr("class", "tab-find hidden");
                return false;
            }
        }
        return false;
    };
    jQuery(window).resize(i);
    jQuery(b).mousedown(d);
    jQuery(b).mousedown(q);
    jQuery(n).keyup(m);
    jQuery(n).keydown(e);
    jQuery(f).submit(h);
    jQuery(document).mouseover(u);
    TabControlAppend = p;
    TabControlRemove = w;
    i();
})();

