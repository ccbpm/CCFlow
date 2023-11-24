window.$ = HTMLElement.prototype.$ = function (selector) {
    return (this == window ? document : this).querySelectorAll(selector);
}

window.onload = function () {
    nav.init();
}
var nav = {
    LIHEIGHT: 110,
    moved: 0,
    count: 0,
    ul: null,
    btnT: null,
    btnB: null,
    init: function () {
        this.ul = $("#nav")[0];
        this.btnT = this.ul.parentNode.$("i")[0];
        this.btnB = this.ul.parentNode.$("i")[1];
        this.btnT.onclick = this.btnB.onclick = function () {
            nav.move(this);
        };
        this.count = this.ul.$("li").length;
    },
    move: function (btn) {
        if (typeof String.prototype.endsWith != 'function') {
            String.prototype.endsWith = function (suffix) {
                return this.indexOf(suffix, this.length - suffix.length) !== -1;
            };
        }
        if (!btn.className.endsWith("_disabled")) {
            this.count = this.ul.$("li").length;
            if (this.count < 5) {
                this.btnT.className += "_disabled";
                this.btnB.className += "_disabled";
            } else {
                if (btn == this.btnB) {
                    this.ul.style.top =
                        -(this.LIHEIGHT * (++this.moved) - 30) + "px";
                } else {
                    this.ul.style.top =
                        -(this.LIHEIGHT * (--this.moved) - 30) + "px";
                }
                this.btnEnable();
            }
        }
    },
    btnEnable: function () {
        this.count = this.ul.$("li").length;
        if (this.moved == 0) {
            this.btnT.className += "_disabled";
            if (this.count == 5) {
                this.btnB.className =
                    this.btnB.className.replace("_disabled", "");
            }
        } else if (this.count - this.moved == 4) {
            this.btnB.className += "_disabled";
            if (this.count == 5) {
                this.btnT.className =
                    this.btnT.className.replace("_disabled", "");
            }
        } else {
            this.btnT.className =
                this.btnT.className.replace("_disabled", "");
            this.btnB.className =
                this.btnB.className.replace("_disabled", "");
        }
    }
}