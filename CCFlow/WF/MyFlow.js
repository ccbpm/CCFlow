//原有的
function OpenOfiice(fk_ath, pkVal, delPKVal, FK_MapData, NoOfObj, FK_Node) {
    var date = new Date();
    var t = date.getFullYear() + "" + date.getMonth() + "" + date.getDay() + "" + date.getHours() + "" + date.getMinutes() + "" + date.getSeconds();

    var url = 'WebOffice/AttachOffice.aspx?DoType=EditOffice&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal + "&FK_MapData=" + FK_MapData + "&NoOfObj=" + NoOfObj + "&FK_Node=" + FK_Node + "&T=" + t;
    //var url = 'WebOffice.aspx?DoType=EditOffice&DelPKVal=' + delPKVal + '&FK_FrmAttachment=' + fk_ath + '&PKVal=' + pkVal;
    // var str = window.showModalDialog(url, '', 'dialogHeight: 1250px; dialogWidth:900px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no;resizable:yes');
    //var str = window.open(url, '', 'dialogHeight: 1200px; dialogWidth:1110px; dialogTop: 100px; dialogLeft: 100px; center: no; help: no;resizable:yes');
    window.open(url, '_blank', 'height=600,width=850,top=50,left=50,toolbar=no,menubar=no,scrollbars=yes, resizable=yes,location=no, status=no');
}

//按钮.
function FocusBtn(btn, workid) {
    if (btn.value == '关注') {
        btn.value = '取消关注';
    }
    else {
        btn.value = '关注';
    }
    $.ajax({ url: "Do.aspx?ActionType=Focus&WorkID=" + workid, async: false });
}

function ReturnVal(ctrl, url, winName, width, height, title) {
    if (url == "")
        return;
    //update by dgq 2013-4-12 判断有没有？
    if (ctrl && ctrl.value != "") {
        if (url.indexOf('?') > 0)
            url = url + '&CtrlVal=' + ctrl.value;
        else
            url = url + '?CtrlVal=' + ctrl.value;
    }
    //修改标题控制不进行保存
    if (typeof self.parent.TabFormExists != 'undefined') {
        var bExists = self.parent.TabFormExists();
        if (bExists) {
            self.parent.ChangTabFormTitleRemove();
        }
    }

    //OpenJbox();
    try {
        $.jBox("iframe:" + url, {
            title: title,
            width: width || 760,
            height: height || 450,
            buttons: { '确定': 'ok' },
            submit: function (v, h, f) {
                var iframeWin = h[0].firstChild.contentWindow;
                if (iframeWin) {
                    if (typeof iframeWin.getReturnText != 'undefined') {
                        var txtId = ctrl.id;
                        ctrl.value = iframeWin.getReturnText();
                        if (typeof iframeWin.getReturnValue != 'undefined') {
                            $('#' + txtId + "_ReValue").val(iframeWin.getReturnValue());
                        }
                    } else {
                        ctrl.value = iframeWin.returnValue;
                    }
                }
                //移除
                $(".jbox-body").remove();
            }
        });
    } catch (e) {
        alert(e);
    }

    //修改标题，失去焦点时进行保存
    if (typeof self.parent.TabFormExists != 'undefined') {
        var bExists = self.parent.TabFormExists();
        if (bExists) {
            self.parent.ChangTabFormTitle();
        }
    }
    return;
}
//然浏览器最大化.
function ResizeWindow() {
    if (window.screen) {  //判断浏览器是否支持window.screen判断浏览器是否支持screen
        var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽
        var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高
        window.moveTo(0, 0);           //把window放在左上角
        window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh
    }
}
window.onload = ResizeWindow;




//以下是软通写的
//初始化网页URL参数
function initPageParam() {
    //新建独有
    pageData.UserNo = GetQueryString("UserNo");
    pageData.DoWhat = GetQueryString("DoWhat");
    pageData.IsMobile = GetQueryString("IsMobile");

    pageData.FK_Flow = GetQueryString("FK_Flow");
    pageData.FK_Node = GetQueryString("FK_Node");
    //FK_Flow=004&FK_Node=402&FID=0&WorkID=232&IsRead=0&T=20160920223812&Paras=
    pageData.FID = GetQueryString("FID");
    pageData.WorkID = GetQueryString("WorkID");
    pageData.IsRead = GetQueryString("IsRead");
    pageData.T = GetQueryString("T");
    pageData.Paras = GetQueryString("Paras");
}
//初始化按钮
function initBar() {
    $.ajax({
        type: 'post',
        async: true,
        data: pageData,
        url: "MyFlow.ashx?Method=InitToolBar&m=" + Math.random(),
        dataType: 'html',
        success: function (data) {
            var barHtml = data;
            $('.Bar').html(barHtml);

            $('.Bar').attr('onclick', $('.Bar').attr('onclick') + ";Send();");
            console.log('ok');
        }
    });
}





//初始化表单。。按钮
function initForm() {
    //bottomToolBar  flowInfo  Message childThread CCForm pub2Class
    //{"IsSuccess":true,"Msg":null,"ErrMsg":null,"List":null,"Data":2}
}



//保存
function save() {
}

//退回
function returnBack() {

}

//移交

//子线程

//子流程

//{"IsSuccess":true,"Msg":null,"ErrMsg":null,"List":null,"Data":2}
function getData(data, url, dataParam) {
    var jsonStr = '{"IsSuccess":true,"Msg":null,"ErrMsg":null,"List":null,"Data":2}';
    var data = JSON.parse(jsonStr);
    if (data.IsSuccess != true) {
        console.log('返回参数失败，ErrMsg:' + data.ErrMsg + ";Msg:" + data.Msg + ";url:" + url);
        console.log(dataParam);
    }
    return data;
}



var jsonStr = '{"Sys_GroupField":[{"OID":1643,"Lab":"开始节点","EnName":"ND17901","Idx":1,"GUID":"","CtrlType":"","CtrlID":"","AtPara":""},{"OID":1653,"Lab":"傻瓜表单测试","EnName":"ND17901","Idx":2,"GUID":"","CtrlType":"","CtrlID":"","AtPara":""}],"Sys_Enum":[{"MyPK":"FindLeader_CH_0","Lab":"直接领导","EnumKey":"FindLeader","IntKey":0,"Lang":"CH"},{"MyPK":"FindLeader_CH_1","Lab":"指定职务级别的领导","EnumKey":"FindLeader","IntKey":1,"Lang":"CH"},{"MyPK":"FindLeader_CH_2","Lab":"指定职务的领导","EnumKey":"FindLeader","IntKey":2,"Lang":"CH"},{"MyPK":"FindLeader_CH_3","Lab":"指定岗位的领导","EnumKey":"FindLeader","IntKey":3,"Lang":"CH"},{"MyPK":"PRI_CH_0","Lab":"低","EnumKey":"PRI","IntKey":0,"Lang":"CH"},{"MyPK":"PRI_CH_1","Lab":"中","EnumKey":"PRI","IntKey":1,"Lang":"CH"},{"MyPK":"PRI_CH_2","Lab":"高","EnumKey":"PRI","IntKey":2,"Lang":"CH"},{"MyPK":"QingJiaLeiXing_CH_0","Lab":"事假","EnumKey":"QingJiaLeiXing","IntKey":0,"Lang":"CH"},{"MyPK":"QingJiaLeiXing_CH_1","Lab":"病假","EnumKey":"QingJiaLeiXing","IntKey":1,"Lang":"CH"},{"MyPK":"QingJiaLeiXing_CH_2","Lab":"婚假","EnumKey":"QingJiaLeiXing","IntKey":2,"Lang":"CH"},{"MyPK":"WJLB_CH_0","Lab":"上行文","EnumKey":"WJLB","IntKey":0,"Lang":"CH"},{"MyPK":"WJLB_CH_1","Lab":"平行文","EnumKey":"WJLB","IntKey":1,"Lang":"CH"},{"MyPK":"WJLB_CH_2","Lab":"下行文","EnumKey":"WJLB","IntKey":2,"Lang":"CH"},{"MyPK":"WJLB_CH_3","Lab":"简讯","EnumKey":"WJLB","IntKey":3,"Lang":"CH"}],"WF_Node":[],"Sys_MapData":[{"No":"ND17901","Name":"填写请假申请单","FrmW":900,"FrmH":1200}],"Sys_MapAttr":[{"MyPK":"ND17901_Title","FK_MapData":"ND17901","KeyOfEn":"Title","Name":"标题","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":251,"UIHeight":23,"MinLen":0,"MaxLen":200,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":1,"UIIsInput":1,"IsSigan":0,"X":174.83,"Y":54.4,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1643,"Idx":-1,"AtPara":""},{"MyPK":"ND17901_FK_DQ","FK_MapData":"ND17901","KeyOfEn":"FK_DQ","Name":"地区","DefVal":"","UIContralType":1,"MyDataType":1,"LGType":2,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"CN_PQ","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":1,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1653,"Idx":1,"AtPara":""},{"MyPK":"ND17901_FK_DQT","FK_MapData":"ND17901","KeyOfEn":"FK_DQT","Name":"地区","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":60,"UIBindKey":"CN_PQ","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1653,"Idx":1,"AtPara":""},{"MyPK":"ND17901_QingJiaRen","FK_MapData":"ND17901","KeyOfEn":"QingJiaRen","Name":"请假人","DefVal":"@WebUser.Name","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1643,"Idx":1,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_QingJiaRenBuMen","FK_MapData":"ND17901","KeyOfEn":"QingJiaRenBuMen","Name":"请假人部门","DefVal":"@WebUser.FK_DeptName","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1643,"Idx":2,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_FK_SFT","FK_MapData":"ND17901","KeyOfEn":"FK_SFT","Name":"省份","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":60,"UIBindKey":"CN_SF","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1653,"Idx":2,"AtPara":""},{"MyPK":"ND17901_PRI","FK_MapData":"ND17901","KeyOfEn":"PRI","Name":"优先级","DefVal":"2","UIContralType":1,"MyDataType":2,"LGType":1,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":200,"UIBindKey":"PRI","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":174.76,"Y":56.19,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":3,"AtPara":""},{"MyPK":"ND17901_QingJiaLeiXing","FK_MapData":"ND17901","KeyOfEn":"QingJiaLeiXing","Name":"请假类型","DefVal":"0","UIContralType":1,"MyDataType":2,"LGType":1,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"QingJiaLeiXing","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1643,"Idx":4,"AtPara":"@RBShowModel=0"},{"MyPK":"ND17901_FK_SF","FK_MapData":"ND17901","KeyOfEn":"FK_SF","Name":"省份","DefVal":"","UIContralType":1,"MyDataType":1,"LGType":2,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"CN_SF","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1653,"Idx":4,"AtPara":""},{"MyPK":"ND17901_QingJiaRiQiCong","FK_MapData":"ND17901","KeyOfEn":"QingJiaRiQiCong","Name":"请假日期从","DefVal":"@RDT","UIContralType":0,"MyDataType":7,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":20,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":0,"GroupID":1643,"Idx":5,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_SanLieWenBenKuang","FK_MapData":"ND17901","KeyOfEn":"SanLieWenBenKuang","Name":"三列文本框","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":3,"GroupID":1653,"Idx":5,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_RiQiDao","FK_MapData":"ND17901","KeyOfEn":"RiQiDao","Name":"日期到","DefVal":"","UIContralType":0,"MyDataType":7,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":20,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":0,"GroupID":1643,"Idx":6,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_YouYanJing","FK_MapData":"ND17901","KeyOfEn":"YouYanJing","Name":"右眼睛","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":69,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1653,"Idx":7,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_QingJiaTianShu","FK_MapData":"ND17901","KeyOfEn":"QingJiaTianShu","Name":"请假天数","DefVal":"0","UIContralType":0,"MyDataType":3,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":0,"GroupID":1643,"Idx":7,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_QingJiaYuanYin","FK_MapData":"ND17901","KeyOfEn":"QingJiaYuanYin","Name":"请假原因","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":69,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":4,"GroupID":1643,"Idx":8,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_ZuoYanJing","FK_MapData":"ND17901","KeyOfEn":"ZuoYanJing","Name":"左眼睛","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":69,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1653,"Idx":10,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_PopDCCT","FK_MapData":"ND17901","KeyOfEn":"PopDCCT","Name":"Pop弹出窗体(表格模式)","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":3,"GroupID":1653,"Idx":12,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_PopFZMS","FK_MapData":"ND17901","KeyOfEn":"PopFZMS","Name":"Pop分组模式","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":3,"GroupID":1653,"Idx":13,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_PopSMS","FK_MapData":"ND17901","KeyOfEn":"PopSMS","Name":"Pop树模式","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":3,"GroupID":1653,"Idx":14,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_PopBGFY","FK_MapData":"ND17901","KeyOfEn":"PopBGFY","Name":"Pop表格分页","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":3,"GroupID":1653,"Idx":15,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_WJLB","FK_MapData":"ND17901","KeyOfEn":"WJLB","Name":"竖向展示枚举","DefVal":"0","UIContralType":3,"MyDataType":2,"LGType":1,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"WJLB","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1653,"Idx":16,"AtPara":"@RBShowModel=1"},{"MyPK":"ND17901_FindLeader","FK_MapData":"ND17901","KeyOfEn":"FindLeader","Name":"竖向展示枚举左边","DefVal":"0","UIContralType":3,"MyDataType":2,"LGType":1,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"FindLeader","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1653,"Idx":17,"AtPara":"@RBShowModel=1"},{"MyPK":"ND17901_DuYanLong","FK_MapData":"ND17901","KeyOfEn":"DuYanLong","Name":"独眼龙","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":69,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":1,"UIIsEnable":1,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":3,"GroupID":1653,"Idx":18,"AtPara":"@FontSize=0"},{"MyPK":"ND17901_Emps","FK_MapData":"ND17901","KeyOfEn":"Emps","Name":"Emps","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":400,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_FID","FK_MapData":"ND17901","KeyOfEn":"FID","Name":"FID","DefVal":"0","UIContralType":0,"MyDataType":2,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_FK_CityT","FK_MapData":"ND17901","KeyOfEn":"FK_CityT","Name":"城市","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":60,"UIBindKey":"CN_City","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":0,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_FK_Dept","FK_MapData":"ND17901","KeyOfEn":"FK_Dept","Name":"操作员部门","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":50,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_CDT","FK_MapData":"ND17901","KeyOfEn":"CDT","Name":"发起时间","DefVal":"@RDT","UIContralType":0,"MyDataType":7,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"1","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_FK_NY","FK_MapData":"ND17901","KeyOfEn":"FK_NY","Name":"年月","DefVal":"","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":7,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_MyNum","FK_MapData":"ND17901","KeyOfEn":"MyNum","Name":"个数","DefVal":"1","UIContralType":0,"MyDataType":2,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_OID","FK_MapData":"ND17901","KeyOfEn":"OID","Name":"WorkID","DefVal":"0","UIContralType":0,"MyDataType":2,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":2,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_RDT","FK_MapData":"ND17901","KeyOfEn":"RDT","Name":"接受时间","DefVal":"","UIContralType":0,"MyDataType":7,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":300,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"1","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""},{"MyPK":"ND17901_Rec","FK_MapData":"ND17901","KeyOfEn":"Rec","Name":"发起人","DefVal":"@WebUser.No","UIContralType":0,"MyDataType":1,"LGType":0,"UIWidth":100,"UIHeight":23,"MinLen":0,"MaxLen":20,"UIBindKey":"","UIRefKey":"","UIRefKeyText":"","UIVisible":0,"UIIsEnable":0,"UIIsLine":0,"UIIsInput":0,"IsSigan":0,"X":5,"Y":5,"GUID":"","Tag":"","EditType":1,"ColSpan":1,"GroupID":1643,"Idx":999,"AtPara":""}],"Sys_MapExt":[],"Sys_FrmLine":[{"MyPK":"29d9936a-fb29-4fd9-b6d3-611f523490d2","FK_MapData":"ND17901","X1":719.09,"X2":719.09,"Y1":40,"Y2":482.73,"BorderColor":"Black","BorderWidth":2},{"MyPK":"45638417-30af-4f05-a82b-b09cd501ad3a","FK_MapData":"ND17901","X1":81.55,"X2":718.82,"Y1":80,"Y2":80,"BorderColor":"Black","BorderWidth":2},{"MyPK":"4bfd8e91-99bb-4d96-aa8f-ed567d6c5684","FK_MapData":"ND17901","X1":83.36,"X2":717.91,"Y1":120.91,"Y2":120.91,"BorderColor":"Black","BorderWidth":2},{"MyPK":"6d8005f2-3018-44a4-9b16-5a1fc5aa4446","FK_MapData":"ND17901","X1":83.36,"X2":717.91,"Y1":40.91,"Y2":40.91,"BorderColor":"Black","BorderWidth":2},{"MyPK":"7c558b34-d002-4fcf-abb0-80e3b7b3b7b8","FK_MapData":"ND17901","X1":81.82,"X2":81.82,"Y1":40,"Y2":480.91,"BorderColor":"Black","BorderWidth":2},{"MyPK":"d6053f98-1b9e-42dc-bf93-0dff21ca9dff","FK_MapData":"ND17901","X1":81.82,"X2":720,"Y1":481.82,"Y2":481.82,"BorderColor":"Black","BorderWidth":2},{"MyPK":"d9fd4ff6-3142-4774-b3a9-a9c47f9faa52","FK_MapData":"ND17901","X1":281.82,"X2":281.82,"Y1":81.82,"Y2":121.82,"BorderColor":"Black","BorderWidth":2},{"MyPK":"ebe7c5ce-8947-4595-90be-25f406a639cb","FK_MapData":"ND17901","X1":360,"X2":360,"Y1":80.91,"Y2":120.91,"BorderColor":"Black","BorderWidth":2},{"MyPK":"f978aa2f-57e9-4f31-8d92-83289130ae22","FK_MapData":"ND17901","X1":158.82,"X2":158.82,"Y1":41.82,"Y2":482.73,"BorderColor":"Black","BorderWidth":2}],"Sys_FrmLink":[],"Sys_FrmBtn":[],"Sys_FrmImg":[{"MyPK":"I20160922161940_1","FK_MapData":"ND17901","ImgAppType":0,"X":577.26,"Y":3.45,"H":40,"W":137,"ImgURL":"/ccform；component/Img/LogoBig.png","ImgPath":"","LinkURL":"http：//ccflow.org","LinkTarget":"_blank","GUID":"","Tag0":"","SrcType":0,"IsEdit":0,"Name":"","EnPK":""}],"Sys_FrmLab":[{"MyPK":"Lab20160922161940_1","FK_MapData":"ND17901","Text":"优先级","X":109.05,"Y":58.1,"FontColor":"black","FontName":"Portable User Interface","FontSize":11,"FontStyle":"Normal","FontWeight":"normal","IsBold":0,"IsItalic":0},{"MyPK":"Lab20160922161940_10","FK_MapData":"ND17901","Text":"新建节点(请修改标题)","X":294.67,"Y":8.27,"FontColor":"Blue","FontName":"Portable User Interface","FontSize":23,"FontStyle":"Normal","FontWeight":"normal","IsBold":0,"IsItalic":0},{"MyPK":"Lab20160922161940_13","FK_MapData":"ND17901","Text":"说明：以上内容是ccflow自动产生的，您可以修改/删除它。@为了更方便您的设计您可以到http：//ccflow.org官网下载表单模板.@因为当前技术问题与silverlight开发工具使用特别说明如下：@@1，改变控件位置： @  所有的控件都支持 wasd， 做为方向键用来移动控件的位置， 部分控件支持方向键. @2， 增加textbox， 从表， dropdownlistbox， 的宽度 shift+ -> 方向键增加宽度 shift + <- 减小宽度.@3， 保存 windows键 + s.  删除 delete.  复制 ctrl+c   粘帖： ctrl+v.@4， 支持全选，批量移动， 批量放大缩小字体.， 批量改变线的宽度.@5， 改变线的长度： 选择线，点绿色的圆点，拖拉它。.@6， 放大或者缩小　label 的字体 ， 选择一个多个label ， 按 A+ 或者　A－　按钮.@7， 改变线或者标签的颜色， 选择操作对象，点工具栏上的调色板.","X":168.24,"Y":163.7,"FontColor":"Red","FontName":"Portable User Interface","FontSize":11,"FontStyle":"Normal","FontWeight":"normal","IsBold":0,"IsItalic":0},{"MyPK":"Lab20160922161940_4","FK_MapData":"ND17901","Text":"发起人","X":106.48,"Y":96.08,"FontColor":"black","FontName":"Portable User Interface","FontSize":11,"FontStyle":"Normal","FontWeight":"normal","IsBold":0,"IsItalic":0},{"MyPK":"Lab20160922161940_7","FK_MapData":"ND17901","Text":"发起时间","X":307.64,"Y":95.17,"FontColor":"black","FontName":"Portable User Interface","FontSize":11,"FontStyle":"Normal","FontWeight":"normal","IsBold":0,"IsItalic":0}],"Sys_FrmRB":[],"Sys_FrmEle":[],"Sys_FrmAttachment":[],"Sys_FrmImgAth":[],"Sys_MapDtl":[],"WF_NodeBar":[{"NodeID":17901,"Step":1,"FK_Flow":"179","Name":"填写请假申请单","Tip":"","WhoExeIt":0,"TurnToDeal":0,"TurnToDealDoc":"","ReadReceipts":0,"CondModel":0,"CancelRole":0,"IsTask":1,"IsRM":1,"DTFrom":"2016-09-22 16：19","DTTo":"2016-09-22 16：19","IsBUnit":0,"FocusField":"请假原因：@QingJiaYuanYin","SaveModel":0,"IsGuestNode":0,"SelfParas":"","RunModel":0,"SubThreadType":0,"PassRate":100,"SubFlowStartWay":0,"SubFlowStartParas":"","TodolistModel":0,"IsAllowRepeatEmps":0,"AutoRunEnable":0,"AutoRunParas":"","AutoJumpRole0":0,"AutoJumpRole1":0,"AutoJumpRole2":0,"WhenNoWorker":0,"SendLab":"发送","SendJS":"","SaveLab":"保存","SaveEnable":1,"ThreadLab":"子线程","ThreadEnable":0,"ThreadKillRole":0,"SubFlowLab":"子流程","SubFlowCtrlRole":0,"JumpWayLab":"跳转","JumpWay":0,"JumpToNodes":"","ReturnLab":"退回","ReturnRole":0,"ReturnAlert":"","IsBackTracking":0,"ReturnField":"","ReturnReasonsItems":"","CCLab":"抄送","CCRole":0,"CCWriteTo":0,"ShiftLab":"移交","ShiftEnable":0,"DelLab":"删除","DelEnable":0,"EndFlowLab":"结束流程","EndFlowEnable":0,"PrintDocLab":"打印单据","PrintDocEnable":0,"TrackLab":"轨迹","TrackEnable":1,"HungLab":"挂起","HungEnable":0,"SelectAccepterLab":"接受人","SelectAccepterEnable":0,"SearchLab":"查询","SearchEnable":0,"WorkCheckLab":"审核","WorkCheckEnable":0,"BatchLab":"批处理","BatchEnable":0,"AskforLab":"加签","AskforEnable":0,"TCLab":"流转自定义","TCEnable":0,"WebOffice":"公文","WebOfficeEnable":0,"PRILab":"重要性","PRIEnable":0,"CHLab":"节点时限","CHEnable":0,"FocusLab":"关注","FocusEnable":1,"FWCSta":0,"FWCShowModel":1,"FWCType":0,"FWCNodeName":"","FWCAth":0,"FWCTrackEnable":1,"FWCListEnable":1,"FWCIsShowAllStep":0,"SigantureEnabel":0,"FWCIsFullInfo":1,"FWCOpLabel":"审核","FWCDefInfo":"同意","FWC_H":300,"FWC_W":400,"FWCFields":"","MPhone_WorkModel":0,"MPhone_SrcModel":0,"MPad_WorkModel":0,"MPad_SrcModel":0,"FTCLab":"流转自定义","FTCSta":0,"FTCWorkModel":0,"FTC_X":5,"FTC_Y":5,"FTC_H":300,"FTC_W":400,"OfficeOpenLab":"打开本地","OfficeOpenEnable":0,"OfficeOpenTemplateLab":"打开模板","OfficeOpenTemplateEnable":0,"OfficeSaveLab":"保存","OfficeSaveEnable":1,"OfficeAcceptLab":"接受修订","OfficeAcceptEnable":0,"OfficeRefuseLab":"拒绝修订","OfficeRefuseEnable":0,"OfficeOverLab":"套红","OfficeOverEnable":0,"OfficeMarksEnable":1,"OfficePrintLab":"打印","OfficePrintEnable":0,"OfficeSealLab":"签章","OfficeSealEnable":0,"OfficeInsertFlowLab":"插入流程","OfficeInsertFlowEnable":0,"OfficeNodeInfo":0,"OfficeReSavePDF":0,"OfficeDownLab":"下载","OfficeDownEnable":0,"OfficeIsMarks":1,"OfficeTemplate":"","OfficeIsParent":1,"OfficeTHEnable":0,"OfficeTHTemplate":"","SFLab":"子流程","SFSta":0,"SFShowModel":1,"SFCaption":"","SFDefInfo":"","SFActiveFlows":"","SF_X":5,"SF_Y":5,"SF_H":300,"SF_W":400,"SFFields":"","SFShowCtrl":0,"SelectorDBShowWay":0,"SelectorModel":0,"SelectorP1":"","SelectorP2":"","OfficeOpen":"打开本地","OfficeOpenTemplate":"打开模板","OfficeSave":"保存","OfficeAccept":"接受修订","OfficeRefuse":"拒绝修订","OfficeOver":"套红按钮","OfficeMarks":1,"OfficeReadOnly":0,"OfficePrint":"打印按钮","OfficeSeal":"签章按钮","OfficeSealEnabel":0,"OfficeInsertFlow":"插入流程","OfficeInsertFlowEnabel":0,"OfficeIsDown":0,"OfficeIsTrueTH":0,"WebOfficeFrmModel":0,"FrmThreadLab":"子线程","FrmThreadSta":0,"FrmThread_X":5,"FrmThread_Y":5,"FrmThread_H":300,"FrmThread_W":400,"CheckNodes":"","DeliveryWay":0,"FWCLab":"审核信息","FWC_X":5,"FWC_Y":5,"CCIsStations":0,"CCIsDepts":0,"CCIsEmps":0,"CCIsSQLs":0,"CCCtrlWay":0,"CCSQL":"","CCTitle":"","CCDoc":"","IsExpSender":1,"DeliveryParas":"","BatchRole":0,"BatchListCount":12,"BatchParas":"","FormType":0,"NodeFrmID":"","FormUrl":"http：//","BlockModel":0,"BlockExp":"","BlockAlert":"","TSpanDay":0,"TSpanHour":8,"WarningDay":0,"WarningHour":4,"TCent":2,"CHWay":0,"IsEval":0,"OutTimeDeal":0,"DoOutTime":"","DoOutTimeCond":"","FrmTrackLab":"轨迹","FrmTrackSta":0,"FrmTrack_X":5,"FrmTrack_Y":5,"FrmTrack_H":300,"FrmTrack_W":400,"ICON":"前台","NodeWorkType":1,"FlowName":"我的流程(傻瓜表单)","FK_FlowSort":"01","FK_FlowSortT":"","FrmAttr":"","TAlertRole":0,"TAlertWay":0,"WAlertRole":0,"WAlertWay":0,"Doc":"","IsCanRpt":1,"IsCanOver":0,"IsSecret":0,"IsCanDelFlow":0,"IsHandOver":0,"NodePosType":0,"IsCCFlow":0,"HisStas":"@07@08@09@10@11","HisDeptStrs":"@07@08@09@10@11","HisToNDs":"@17902","HisBillIDs":"","HisSubFlows":"","PTable":"","ShowSheets":"","GroupStaNDs":"@17901","X":170,"Y":81,"AtPara":"","DocLeftWord":"","DocRightWord":""}],"WF_Flow":[{"No":"179","Name":"我的流程(傻瓜表单)","FK_FlowSort":"01","FK_FlowSortText":"线性流程","SysType":"","FlowRunWay":0,"RunObj":"","Note":"","RunSQL":"","NumOfBill":0,"NumOfDtl":0,"FlowAppType":0,"ChartType":1,"IsCanStart":"1","AvgDay":0,"IsFullSA":0,"IsMD5":0,"Idx":0,"TimelineRole":0,"Paras":"@StartNodeX=200@StartNodeY=50@EndNodeX=200@EndNodeY=350","PTable":"","Draft":0,"DataStoreModel":1,"TitleRole":"","FlowMark":"","FlowEventEntity":"","HistoryFields":"","IsGuestFlow":0,"BillNoFormat":"","FlowNoteExp":"","DRCtrlType":0,"StartLimitRole":0,"StartLimitPara":"","StartLimitAlert":"","StartLimitWhen":0,"StartGuideWay":0,"StartGuidePara1":"","StartGuidePara2":"","StartGuidePara3":"","IsResetData":0,"IsLoadPriData":0,"CFlowWay":0,"CFlowPara":"","IsBatchStart":0,"BatchStartFields":"","IsAutoSendSubFlowOver":0,"Ver":"2016-09-20 15：11：11","DType":1,"AtPara":"","DTSWay":0,"DTSDBSrc":"","DTSBTable":"","DTSBTablePK":"","DTSTime":0,"DTSSpecNodes":"","DTSField":0,"DTSFields":""}],"MainTable":[{"QingJiaYuanYin":"我是请假原因","Title":"财务部-guobaogeng，郭宝庚在2016-09-22 17：14发起.","QingJiaRen":"郭宝庚","QingJiaRenBuMen":"财务部","PRI":2,"PRIText":"高","QingJiaLeiXing":1,"QingJiaLeiXingText":"事假","QingJiaRiQiCong":"2016-09-22 17：14","RiQiDao":"","QingJiaTianShu":0,"QingJiaYuanYin":"我是请假原因2","RDT":"2016-09-22 17：49","Rec":"guobaogeng","FK_NY":"2016-09","MyNum":1,"OID":102,"CDT":"2016-09-22 17：49","Emps":"guobaogeng","FID":0,"FK_CityT":"","FK_Dept":"5","FK_DQ":"","FK_DQText":"","FK_DQT":"","FK_SFT":"","FK_SF":"","FK_SFText":"","SanLieWenBenKuang":"","YouYanJing":"","ZuoYanJing":"","PopDCCT":"","PopFZMS":"","PopSMS":"","PopBGFY":"","WJLB":0,"WJLBText":"上行文","FindLeader":1,"FindLeaderText":"指定职务级别的领导","DuYanLong":""}],"CN_PQ":[{"No":"AA","Name":"城市"},{"No":"DB","Name":"东北"},{"No":"HB","Name":"华北"},{"No":"HD","Name":"华东"},{"No":"XB","Name":"西北"},{"No":"XN","Name":"西南"},{"No":"ZN","Name":"中南"},{"No":"ZZ","Name":"香澳台"}],"CN_SF":[{"No":"11","Name":"北京","Names":"北京市","JC":"京","FK_PQ":"AA"},{"No":"12","Name":"天津","Names":"天津市","JC":"津","FK_PQ":"AA"},{"No":"13","Name":"河北","Names":"河北省","JC":"冀","FK_PQ":"HB"},{"No":"14","Name":"山西","Names":"山西省","JC":"晋","FK_PQ":"HB"},{"No":"15","Name":"内蒙","Names":"内蒙古自治区","JC":"蒙","FK_PQ":"HB"},{"No":"21","Name":"辽宁","Names":"辽宁省","JC":"辽","FK_PQ":"DB"},{"No":"22","Name":"吉林","Names":"吉林省","JC":"吉","FK_PQ":"DB"},{"No":"23","Name":"黑龙江","Names":"黑龙江省","JC":"黑","FK_PQ":"DB"},{"No":"31","Name":"上海","Names":"上海市","JC":"沪","FK_PQ":"AA"},{"No":"32","Name":"江苏","Names":"江苏省","JC":"苏","FK_PQ":"HD"},{"No":"33","Name":"浙江","Names":"浙江省","JC":"浙","FK_PQ":"HD"},{"No":"34","Name":"安徽","Names":"安徽省","JC":"皖","FK_PQ":"HD"},{"No":"35","Name":"福建","Names":"福建省","JC":"闽","FK_PQ":"HD"},{"No":"36","Name":"江西","Names":"江西省","JC":"赣","FK_PQ":"HD"},{"No":"37","Name":"山东","Names":"山东省","JC":"鲁","FK_PQ":"HD"},{"No":"41","Name":"河南","Names":"河南省","JC":"豫","FK_PQ":"ZN"},{"No":"42","Name":"湖北","Names":"湖北省","JC":"鄂","FK_PQ":"ZN"},{"No":"43","Name":"湖南","Names":"湖南省","JC":"湘","FK_PQ":"ZN"},{"No":"44","Name":"广东","Names":"广东省","JC":"粤","FK_PQ":"ZN"},{"No":"45","Name":"广西","Names":"广西壮族自治区","JC":"桂","FK_PQ":"ZN"},{"No":"46","Name":"海南","Names":"海南省","JC":"琼","FK_PQ":"ZN"},{"No":"50","Name":"重庆","Names":"重庆市","JC":"渝","FK_PQ":"AA"},{"No":"51","Name":"四川","Names":"四川省","JC":"川","FK_PQ":"XN"},{"No":"52","Name":"贵州","Names":"贵州省","JC":"贵","FK_PQ":"XN"},{"No":"53","Name":"云南","Names":"云南省","JC":"云","FK_PQ":"XN"},{"No":"54","Name":"西藏","Names":"西藏自治区","JC":"藏","FK_PQ":"XN"},{"No":"61","Name":"陕西","Names":"陕西省","JC":"陕","FK_PQ":"XB"},{"No":"62","Name":"甘肃","Names":"甘肃省","JC":"甘","FK_PQ":"XB"},{"No":"63","Name":"青海","Names":"青海省","JC":"青","FK_PQ":"XB"},{"No":"64","Name":"宁夏","Names":"宁夏回族自治区","JC":"宁","FK_PQ":"XB"},{"No":"65","Name":"新疆","Names":"新疆维吾尔自治区","JC":"新","FK_PQ":"XB"},{"No":"71","Name":"台湾","Names":"台湾省","JC":"台","FK_PQ":"ZZ"},{"No":"81","Name":"香港","Names":"香港特别行政区","JC":"港","FK_PQ":"ZZ"},{"No":"82","Name":"澳门","Names":"澳门特别行政区","JC":"澳","FK_PQ":"ZZ"}]}'