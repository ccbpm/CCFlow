function CreatHelpDiv(_parent, _element, _id, _css) {//创建层
    var newObj = document.createElement(_element);
    if (_id && _id != "") newObj.id = _id;
    if (_css && _css != "") {
        newObj.setAttribute("style", _css);
        newObj.style.cssText = _css;
    }

    if (_parent && _parent != "") {
        var theObj = getobj(_parent);
        var parent = theObj[0].parentNode;
        if (parent.lastChild == theObj[0]) {
            theObj[0].appendChild(newObj);
        }
        else {
            theObj[0].insertBefore(newObj, theObj[0].nextSibling);
        }
    }
    else getobj("divCCForm")[0].appendChild(newObj);
}

function getobj(o) {//获取对象
    return $("*[id$=" + o + "]");
}

var swtemp = 0, objtemp;
function ShowHelpDiv(inputid, appPath, attrKey, enName, doType) {//显示层
    if (appPath == "")
        return;
    //弹出页面Pop返回值
    if (doType == "returnval") {
        HelpTBOpenPopWin(inputid, appPath, "rev");
        return;
    }
    if (swtemp == 1) { if (getobj(inputid + "mydiv").length > 0) getobj(objtemp + "mydiv")[0].style.display = "none"; }
    if (getobj(inputid + "mydiv").length <= 0) {//若尚未创建就建之
        var souceObj = getobj(inputid)[0];
        var parentEle = souceObj.parentElement.parentElement;
        var left = parentEle.offsetLeft;
        var top = parentEle.offsetTop + souceObj.offsetHeight;
        var divcss = "word-break: keep-all;z-index:9999;font-size:12px;color:blue;position:absolute;left:" + left + "px;top:" + top + "px;border:1px solid blue"

        CreatHelpDiv("", "div", inputid + "mydiv", divcss); //创建层"mydiv"
        CreatHelpDiv(inputid + "mydiv", "ul", inputid + "myul"); //创建ul 

        CreatHelpDiv(inputid + "myul", "li", inputid + "li0", "background:#fff");
        //常用词汇
        if (doType == "wordhelp") {
            getobj(inputid + "li0")[0].innerHTML = "<a href='javascript:TBHelp(\"" + inputid + "\",\"" + appPath + "\",\"" + attrKey + "\",\"" + enName + "\")'>常用词汇</a>";
        }
        //弹出页面Pop返回值
        if (doType == "returnval") {
            getobj(inputid + "li0")[0].innerHTML = "<a href='javascript:HelpTBOpenPopWin(\"" + inputid + "\",\"" + appPath + "\",\"rev\")'>打开选择页面</a>";
        }
        //内置POP返回值
        if (doType == "returnvalccformpopval") {
            getobj(inputid + "li0")[0].innerHTML = "<a href='javascript:HelpTBOpenPopWin(\"" + inputid + "\",\"" + attrKey + "\",\"" + enName + "\",\"ccform\")'>打开选择页面</a>";
        }

        CreatHelpDiv(inputid + "myul", "li", inputid + "li1", "color:#f00;background:#fff"); //创建"clear"li
        getobj(inputid + "li1")[0].innerHTML = "<a style=\"color:Red;\" href='javascript:ClearHelpTBText(\"" + inputid + "\")'>清空</a>";

        getobj(inputid + "mydiv")[0].innerHTML += "<style type='text/css'>#" + inputid + "mydiv ul {padding:0px;margin:0;}#" + inputid + "mydiv ul li{list-style-type:none;padding:5px;margin:0;float:left;CURSOR: pointer;}</style>";
        for (var i = 0; i <= 1; i++) {
            getobj(inputid + "li" + i)[0].onmouseover = function () { this.style.background = "#eee"; clearTimeout(HelpTBTimer) }
            getobj(inputid + "li" + i)[0].onmouseout = function () { this.style.background = "#fff" }
        }
    }

    var newdiv = getobj(inputid + "mydiv")[0];
    newdiv.onclick = function () { HelpTBHiddiv(event, inputid); }
    newdiv.onmouseout = function () { HelpTBMout(inputid + "mydiv") }
    newdiv.onmouseover = function () { clearTimeout(HelpTBTimer) }
    getobj(inputid)[0].onmouseout = function () { HelpTBMout(inputid + "mydiv") }
    newdiv.style.display = "block";
    swtemp = 1;
    objtemp = inputid;
}

var HelpTBTimer;
function HelpTBMout(newDiv) {
    HelpTBTimer = setTimeout(function () { $("#" + newDiv).remove(); }, 300);
    swtemp = 0;
}

function ClearHelpTBText(inputid) {
    getobj(inputid).val('');
}

function HelpTBHiddiv(e, inputid) {
    e = e || window.event;
    ev = e.target || e.srcElement;
    v = ev.innerText || ev.textContent;
    if (v == "清空") {
        getobj(inputid)[0].value = "";
    }
    getobj(inputid + "mydiv")[0].style.display = "none";
}

//内置PoP返回值方法
function HelpTBOpenPopWin(inputid, fk_mapExt, refEnPK, model) {
    var ctrl = getobj(inputid)[0];
    if (model == "ccform") {
        ReturnValCCFormPopVal(ctrl, fk_mapExt, refEnPK);
    } else {
        ReturnVal(ctrl, fk_mapExt, refEnPK);
    }
}