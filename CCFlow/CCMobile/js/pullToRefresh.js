/*!
* pull to refresh v2.0
*author:wallace
*2015-7-22
*/
var refresher = {
    info: {
        "pullDownLable": "下滑刷新...",
        "pullingDownLable": "Release to refresh...",
        "pullUpLable": "上滑加载更多...",
        "pullingUpLable": "Release to load more...",
        "loadingLable": "Loading..."
    },
    init: function (parameter) {
        var wrapper = document.getElementById(parameter.id);
        var div = document.createElement("div");
        div.className = "scroller";
        wrapper.appendChild(div);
        var scroller = wrapper.querySelector(".scroller");
        var list = wrapper.querySelector("#" + parameter.id + " ul");
        scroller.insertBefore(list, scroller.childNodes[0]);
        var pullDown = document.createElement("div");
        pullDown.className = "pullDown";
        var loader = document.createElement("div");
        loader.className = "loader";
        for (var i = 0; i < 4; i++) {
            var span = document.createElement("span");
            loader.appendChild(span);
        }
        pullDown.appendChild(loader);
        var pullDownLabel = document.createElement("div");
        pullDownLabel.className = "pullDownLabel";
        pullDown.appendChild(pullDownLabel);
        scroller.insertBefore(pullDown, scroller.childNodes[0]);
        var pullUp = document.createElement("div");
        pullUp.className = "pullUp";
        var loader = document.createElement("div");
        loader.className = "loader";
        for (var i = 0; i < 4; i++) {
            var span = document.createElement("span");
            loader.appendChild(span);
        }
        pullUp.appendChild(loader);
        var pullUpLabel = document.createElement("div");
        pullUpLabel.className = "pullUpLabel";
        if (parameter.ShowUpLable == true) {
            var content = document.createTextNode(refresher.info.pullUpLable);
            pullUpLabel.appendChild(content);
        }
        pullUp.appendChild(pullUpLabel);
        scroller.appendChild(pullUp);
        var pullDownEl = wrapper.querySelector(".pullDown");
        var pullDownOffset = pullDownEl.offsetHeight;
        var pullUpEl = wrapper.querySelector(".pullUp");
        var pullUpOffset = pullUpEl.offsetHeight;
        this.scrollIt(parameter, pullDownEl, pullDownOffset, pullUpEl, pullUpOffset);
    },
    scrollIt: function (parameter, pullDownEl, pullDownOffset, pullUpEl, pullUpOffset) {
        cceval(parameter.id + "= new iScroll(parameter.id, {useTransition: true,vScrollbar: false,topOffset: pullDownOffset,onRefresh: function () {refresher.onRelease(pullDownEl,pullUpEl,parameter);},onScrollMove: function () {refresher.onScrolling(this,pullDownEl,pullUpEl,pullUpOffset);},onScrollEnd: function () {refresher.onPulling(pullDownEl,parameter.pullDownAction,pullUpEl,parameter.pullUpAction);},})");
        if (parameter.ShowDownLabel == true) {
            pullDownEl.querySelector('.pullDownLabel').innerHTML = refresher.info.pullDownLable;
        }
        document.addEventListener('touchmove', function (e) {
            e.preventDefault();
        }, false);
    },
    onScrolling: function (e, pullDownEl, pullUpEl, pullUpOffset) {
        if (e.y > -(pullUpOffset)) {
            pullDownEl.id = '';
            pullDownEl.querySelector('.pullDownLabel').innerHTML = refresher.info.pullDownLable;
            e.minScrollY = -pullUpOffset;
        }
        if (e.y > 0) {
            pullDownEl.classList.add("flip");
            pullDownEl.querySelector('.pullDownLabel').innerHTML = refresher.info.pullingDownLable;
            e.minScrollY = 0;
        }
        if (e.scrollerH < e.wrapperH && e.y < (e.minScrollY - pullUpOffset) || e.scrollerH > e.wrapperH && e.y < (e.maxScrollY - pullUpOffset)) {
            pullUpEl.style.display = "block";
            pullUpEl.classList.add("flip");
            pullUpEl.querySelector('.pullUpLabel').innerHTML = refresher.info.pullingUpLable;
        }
        if (e.scrollerH < e.wrapperH && e.y > (e.minScrollY - pullUpOffset) && pullUpEl.id.match('flip') || e.scrollerH > e.wrapperH && e.y > (e.maxScrollY - pullUpOffset) && pullUpEl.id.match('flip')) {
            pullDownEl.classList.remove("flip");
            pullUpEl.querySelector('.pullUpLabel').innerHTML = refresher.info.pullUpLable;
        }
    },
    onRelease: function (pullDownEl, pullUpEl, parameter) {
        if (pullDownEl.className.match('loading')) {
            pullDownEl.classList.toggle("loading");
            pullDownEl.querySelector('.loader').style.display = "none"
            pullDownEl.style.lineHeight = pullDownEl.offsetHeight + "px";
            if (parameter.ShowDownLabel == true) {
                pullDownEl.querySelector('.pullDownLabel').innerHTML = refresher.info.pullDownLable;
            } else {
                pullDownEl.style.lineHeight = "0px";
            }
        }
        if (pullUpEl.className.match('loading')) {
            pullUpEl.classList.toggle("loading");
            pullUpEl.querySelector('.loader').style.display = "none"
            pullUpEl.style.lineHeight = pullUpEl.offsetHeight + "px";
            if (parameter.ShowUpLable == true) {
                pullUpEl.querySelector('.pullUpLabel').innerHTML = refresher.info.pullUpLable;
            } else {
                pullUpEl.querySelector('.pullUpLabel').style.display = "none"    
            }
        }
    },
    onPulling: function (pullDownEl, pullDownAction, pullUpEl, pullUpAction) {
        if (pullDownEl.className.match('flip') /*&&!pullUpEl.className.match('loading')*/) {
            pullDownEl.classList.add("loading");
            pullDownEl.classList.remove("flip");
            pullDownEl.querySelector('.pullDownLabel').innerHTML = refresher.info.loadingLable;
            pullDownEl.querySelector('.loader').style.display = "block"
            pullDownEl.style.lineHeight = "20px";
            if (pullDownAction) pullDownAction();
        }
        if (pullUpEl.className.match('flip') /*&&!pullDownEl.className.match('loading')*/) {
            pullUpEl.classList.add("loading");
            pullUpEl.classList.remove("flip");
            pullUpEl.querySelector('.pullUpLabel').innerHTML = refresher.info.loadingLable;
            pullUpEl.querySelector('.loader').style.display = "block"
            pullUpEl.style.lineHeight = "20px";
            if (pullUpAction) pullUpAction();
        }
    }
}