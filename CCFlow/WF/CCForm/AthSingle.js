var webUser = new WebUser();
$(function () {
    
    var items = $("[name=AthSingle]");
    if (items.length == 0)
        return;
    //获取引入JS的路径
    var ccPath = "../DataUser/";
    var currentURL = GetHrefUrl();
    if (currentURL.indexOf("FoolFormDesigner/Designer") != -1)
        ccPath = "../../../DataUser/";
    if (currentURL.indexOf("CCForm") != -1)
        ccPath = "../../DataUser/";
    //引入wps的js
    //Skip.addJs(ccPath + "JSLibData/GovFile110/wps1/wps_js/wps_sdk.js");
    //Skip.addJs(ccPath + "JSLibData/GovFile110/wps1/wps_js/wps.js");
    Skip.addJs(ccPath + "JSLibData/GovFile110/wps/oaassist/server/wwwroot/resource/js/wps_sdk.js");
    Skip.addJs(ccPath + "JSLibData/GovFile110/wps/oaassist/server/wwwroot/resource/js/wps.js");
    //点击事件
    $("[name=AthSingle]").bind('click', function () {
        var myPK = this.id;
        var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
        handler.AddUrlData();
        handler.AddPara("AthMyPK", myPK);
        handler.AddPara("PKVal", pageData.WorkID);
        var data = handler.DoMethodReturnString("AthSingle_Init");
        if (data.indexOf("err@") != -1) {
            alert(data.replace("err@", ""));
            return;
        }
        var ath = JSON.parse(data);
        var isHaveFile = true;
        if (ath.AtPara.indexOf("IsHaveFile=0") != -1)
            isHaveFile = false;

        //文档打开方式
        var openType = {};
        //只读
        if (isReadonly == true || ath.AthEditModel == 0) {
            if (isHaveFile == false) {
                alert("文件不存在，不能打开");
                return;
            }
            // 文档保护类型，-1：不启用保护模式，0：只允许对现有内容进行修订，
            // 1：只允许添加批注，2：只允许修改窗体域(禁止拷贝功能)，3：只读
            openType.protectType =3;
        }
        //可编辑全部区域
        else if (ath.AthEditModel == 1)
            openType.protectType = -1
        //可编辑非数据标签区域
        else if (ath.AthEditModel == 2)
            openType.protectType = 0
        openAthSingleDoc(myPK, myPK, ccPath, ath, openType, isHaveFile);

    });
});

function GetTempPath(fk_mapData,mypk,fileType) {

    var url = document.location.host;
    if (fileType==0)
        return document.location.protocol + "//" + url + "/DataUser/Temp/" + mypk + ".docx";
    return document.location.protocol + "//" + url + "/DataUser/UploadFile/" + fk_mapData + "/" + pageData.WorkID + "/" + mypk + ".docx";
}

/**
 * 打开、编辑文档
 * @param {any} docID
 * @param {any} mypk
 * @param {any} ccPath
 */
function openAthSingleDoc(docID, mypk, ccPath, ath, openType, isHaveFile) {
    //文件路径
    var filePath = "";
    if (pageData.WorkID == 0)
        filePath = GetTempPath(ath.FK_MapData, mypk, 0);
    else
        filePath = GetTempPath(ath.FK_MapData, mypk, 1);

    if (openType.protectType == 3) {
        _WpsInvoke([{
            "OpenDoc": {
                "docId": docID, // 文档ID
                "fileName": filePath,
                "userName": webUser.Name,
                "openType": openType
            }
        }])
        return;
    }
    //保存文档上传路径
    var uploadPath = basePath + '/WF/CCForm/Handler.ashx?AthMyPK=' + mypk + '&DoType=AthSingle_Upload&FK_Flow=' + pageData.FK_Flow + '&PKVal=' + pageData.WorkID;
    //需要填充的数据
    var tempDataPath = "";
    //使用模板规则
    switch (ath.AthSingleRole) {
        case 0: //不使用模板
            if (isHaveFile == false) {
                _WpsInvoke([{
                    "NewDoc": {
                        "docId": docID, // 文档ID
                        //"fileName": filePath,
                        "uploadPath": uploadPath, // 保存文档上传路径
                        "userName": webUser.Name,
                        "openType": openType
                    }
                }]) 
                break;
            }
        case 1://使用上传模板
            _WpsInvoke([{
                "OpenDoc": {
                    "docId": docID, // 文档ID
                    "uploadPath": uploadPath, // 保存文档上传路径
                    "fileName": filePath,
                    "userName": webUser.Name,
                    "openType": openType
                }
            }])
            break;
        case 2: //使用上传模板自动加载数据标签
           // debugger
            var templatePath = basePath + '/WF/CCForm/Handler.ashx?AthMyPK=' + mypk + '&DoType=AthSingle_TemplateData&FK_Flow=' + pageData.FK_Flow + '&FK_Node='+pageData.FK_Node+'&PKVal=' + pageData.WorkID;
            _WpsInvoke([{
                "OpenDoc": {
                    "docId": docID, // 文档ID
                    "fileName": filePath,
                    "uploadPath": uploadPath, // 保存文档上传路径
                    "templateDataUrl": templatePath,
                    "userName": webUser.Name,
                    "openType": openType
                }
            }])
            break;
    }
}


/**
 * 通用的打开wps客户端插件
 * @param {any} funcs 配置的参数
 */
function _WpsInvoke(funcs) {
    var info = {};
    info.funcs = funcs;
    var func = bUseHttps ? WpsInvoke.InvokeAsHttps : WpsInvoke.InvokeAsHttp
    func(WpsInvoke.ClientType.wps, // 组件类型
        "WpsOAAssist", // 插件名，与wps客户端加载的加载的插件名对应
        "dispatcher", // 插件方法入口，与wps客户端加载的加载的插件代码对应，详细见插件代码
        info, // 传递给插件的数据
        function (result) { // 调用回调，status为0为成功，其他是错误
            if (result.status) {
                if (bUseHttps && result.status == 100) {
                    WpsInvoke.AuthHttpesCert('请在稍后打开的网页中，点击"高级" => "继续前往"，完成授权。')
                    return;
                }
                alert(result.message)

            } else {
                console.log(result.response)
                showresult(result.response)
            }
        })
}
