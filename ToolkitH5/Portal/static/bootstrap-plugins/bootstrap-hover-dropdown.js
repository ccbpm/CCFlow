(function(b,a,c){var d=b();b.fn.dropdownHover=function(e){if("ontouchstart" in document){return this}d=d.add(this.parent());return this.each(function(){var l=b(this),j=l.parent(),i={delay:500,hoverDelay:0,instantlyCloseOthers:true},k={delay:b(this).data("delay"),hoverDelay:b(this).data("hover-delay"),instantlyCloseOthers:b(this).data("close-others")},o="show.bs.dropdown",g="hide.bs.dropdown",h=b.extend(true,{},i,e,k),n,f;j.hover(function(p){if(!j.hasClass("open")&&!l.is(p.target)){return true}m(p)},function(){a.clearTimeout(f);n=a.setTimeout(function(){l.attr("aria-expanded","false");j.removeClass("open");l.trigger(g)},h.delay)});l.hover(function(p){if(!j.hasClass("open")&&!j.is(p.target)){return true}m(p)});j.find(".dropdown-submenu").each(function(){var q=b(this);var p;q.hover(function(){a.clearTimeout(p);q.children(".dropdown-menu").show();q.siblings().children(".dropdown-menu").hide()},function(){var r=q.children(".dropdown-menu");p=a.setTimeout(function(){r.hide()},h.delay)})});function m(p){if(l.parents(".navbar").find(".navbar-toggle").is(":visible")){return}a.clearTimeout(n);a.clearTimeout(f);f=a.setTimeout(function(){d.find(":focus").blur();if(h.instantlyCloseOthers===true){d.removeClass("open")}a.clearTimeout(f);l.attr("aria-expanded","true");j.addClass("open");l.trigger(o)},h.hoverDelay)}})};b(document).ready(function(){b('[data-hover="dropdown"]').dropdownHover()})})(jQuery,window);