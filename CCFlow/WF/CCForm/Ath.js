

//1.初始化附件全局使用的参数
var AthParams = {};
var athRefPKVal = 0;
AthParams.AthInfo = {};

/**
* 附件初始化
* @param athchment 附件属性
* @param athDivID 生成的附件信息追加的位置
*/
function AthTable_Init(athchment, athDivID, refPKVal) {
    if (typeof athchment != "object" && typeof athchment != "String")
        athchment = new Entity("BP.Sys.FrmAttachment", athchment);
    if (refPKVal == null || refPKVal == undefined || refPKVal == 0)
        athRefPKVal = pageData.WorkID == 0 ? pageData.OID : pageData.WorkID;
    else
        athRefPKVal = refPKVal;

    AthParams.FK_MapData = athchment.FK_MapData;

    //2.上传的URL的设置
    var uploadUrl = "";
    if (plant == 'CCFlow')
        uploadUrl = basePath + '/WF/CCForm/Handler.ashx?AttachPK=' + athchment.MyPK + '&DoType=MoreAttach&FK_Flow=' + pageData.FK_Flow + '&PKVal=' + athRefPKVal;
    else {
        uploadUrl = basePath + "/WF/Ath/AttachmentUploadS.do?FK_FrmAttachment=" + athchment.MyPK + '&FK_Flow=' + pageData.FK_Flow + "&PKVal=" + athRefPKVal;
    }
    uploadUrl += "&WorkID=" + pageData.WorkID;
    uploadUrl += "&FID=" + pageData.FID;
    uploadUrl += "&FK_Node=" + pageData.FK_Node;
    uploadUrl += "&PWorkID=" + GetQueryString("PWorkID");
    uploadUrl += "&FK_MapData=" + AthParams.FK_MapData;

    //3.初始化附件列表
    InitAthPage(athDivID, uploadUrl);

    //4.调用附件上传的功能

    $("#fileUpload_" + athchment.MyPK).initUpload({
        "uploadUrl": uploadUrl,//上传文件信息地址
        "progressUrl": "#",//获取进度信息地址，可选，注意需要返回的data格式如下（{bytesRead: 102516060, contentLength: 102516060, items: 1, percent: 100, startTime: 1489223136317, useTime: 2767}）
        "showSummerProgress": false,//总进度条，默认限制
        "size": athDesc.FileMaxSize,//文件大小限制，单位kb,默认不限制
        "ismultiple": true,
        "beforeUpload": beforeUploadFun,//在上传前执行的函数
        "onUpload": function (opt, data) {
            uploadTools.uploadError(opt);//显示上传错误
            InitAthPage(athDivID);
        },
        autoCommit: true,//文件是否自动上传
        "fileType": AthParams.realFileExts,//文件类型限制，默认不限制，注意写的是文件后缀
        "FK_FrmAttachment": athchment.MyPK,
        "IsExpCol": athchment.IsExpCol == 0 ? false : true,
        "TopNumOfUpload": athDesc.TopNumOfUpload//附件上传的最大数量
    });
}

/**
* 附件上传前需要序列化
* @param opt
*/
function beforeUploadFun(opt) {
    if (parseInt(athDesc.IsExpCol) == 0) {
        var sort = $("#Sort_" + opt.FK_FrmAttachment).val();
        if (sort != null && sort != "" && sort != undefined)
            opt.otherData = [{ "name": "Sort", "value": sort }];
    }
    if (parseInt(athDesc.IsExpCol) == 1) {
        var parasData = $("form").serialize();
        parasData = decodeURIComponent(parasData, true);
        parasData = parasData.replace(/&/g, '@');
        parasData = parasData.replace(/TB_/g, '');
        parasData = parasData.replace(/RB_/g, '');
        parasData = parasData.replace(/CB_/g, '');
        parasData = parasData.replace(/DDL_/g, '');
        //获取分组
        var sort = $("#Sort").val();
        if (sort != null && sort != "" && sort != undefined)
            opt.otherData = [{ "name": "Sort", "value": sort }];

        opt.otherData = [{ "name": "parasData", "value": parasData }];
    }
}


/**
* 初始化附件列表信息
* @param athDivID 生成的附件信息追加的位置
*/
function InitAthPage(athDivID, uploadUrl) {
    AthParams.PKVal = athRefPKVal;
    //1.请求后台数据
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddUrlData();
    if (athDivID.indexOf("_AthMDtl") != -1)
        handler.AddPara("RefOID", AthParams.PKVal == undefined ? pageData.WorkID : AthParams.PKVal);
    //alert("RefOID=" + AthParams.PKVal);
    handler.AddPara("FK_FrmAttachment", athDivID.replace("Div_", ""));
    handler.AddPara("FK_MapData", AthParams.FK_MapData);
    var data = handler.DoMethodReturnString("Ath_Init");

    if (data.indexOf('err@') == 0) {
        //执行方法报错
        alert(data);
        console.log(data);
        return;
    }

    if (data.indexOf('url@') == 0) {
        var url = data.replace('url@', '');
        window.location.href = url;
        return;
    }

    data = JSON.parse(data);
    athDesc = data["AthDesc"][0]; // 附件属性
    var dbs = data["DBAths"];  // 附件列表数据
    console.log(dbs);

    //2.自定义表单模式.
    if (athDesc.AthRunModel == 2) {
        src = "../../DataUser/OverrideFiles/AthSelf.htm?PKVal=" + AthParams.PKVal + "&Ath=" + athDesc.NoOfObj + "&FK_MapData=" + athDesc.FK_MapData + "&FK_FrmAttachment=" + athDesc.MyPK;
        window.location.href = src;
        return;
    }

    //3.附件校验属性
    if (!$.isArray(AthParams.AthInfo[athDesc.MyPK])) {
        AthParams.AthInfo[athDesc.MyPK] = [];
    }
    AthParams.AthInfo[athDesc.MyPK].push([athDesc.NumOfUpload, athDesc.TopNumOfUpload, athDesc.FileMaxSize, athDesc.Exts]);


    //AthParams.NumOfUpload = athDesc.NumOfUpload; //最低上传的数量.
    //AthParams.TopNumOfUpload = athDesc.TopNumOfUpload; //最大上传的数量.
    //AthParams.FileMaxSize = athDesc.FileMaxSize; //最大上传的附件大小.

    //附件上传的格式限制
    AthParams.realFileExts = athDesc.Exts;
    if (athDesc.Exts == null || athDesc.Exts == "" || athDesc.Exts == "*.*")
        AthParams.realFileExts = "*";
    else {
        AthParams.realFileExts = AthParams.realFileExts.replace(/\s*/g, "").replace(/[*.]/g, "").split(",");
    }

    //4.附件列表展示

    //4.1.图片的展示方式
    if (athDesc.FileType == 1) {
        $("#" + athDivID).html(FileShowPic(athDesc, dbs, uploadUrl));
        $(".athImg").on("click", function () {
            var _this = $(this); //将当前的pimg元素作为_this传入函数  
            var src = _this.parent().css("background-image").replace("url(\"", "").replace("\")", "")
            imgShow($("#outerdiv"), $("#innerdiv"), $("#bigimg"), src);
        });
    }
    //4.2 普通附件的展示方式（包含图片，word文档，pdf等）
    else {
        if ($("#fileUpload_" + athDesc.MyPK).length != 0)
            uploadEvent.cleanFileEvent(uploadTools.getInitOption("fileUpload_" + athDesc.MyPK));
        if ($("#tbody_" + athDesc.MyPK).length == 0)
            $("#" + athDivID).html(FileShowWayTable(athDesc, dbs, uploadUrl));
        else
            $("#tbody_" + athDesc.MyPK).html(FileShowWayTable(athDesc, dbs, uploadUrl));
    }

}



/**
* 生成附件列表的Html代码(Table模式)
* @param athDesc 附件属性
* @param dbs 附件列表
*/
var columnNum = 6;
function FileShowWayTable(athDesc, dbs, uploadUrl) {
    var _html = "<table class='table annex-table'>";
    //1.是否启用扩展列
    var mapAttrs = null;
    if (athDesc.IsExpCol != 0) {
        mapAttrs = new Entities("BP.Sys.MapAttrs");
        var extMapData = athDesc.MyPK;
        mapAttrs.Retrieve("FK_MapData", extMapData);
    }

    //2.是否增加有类别
    var isHaveSort = athDesc.Sort != null && athDesc.Sort != undefined && athDesc.Sort != "" ? true : false;
    var sortColoum = isHaveSort == true && athDesc.Sort.indexOf("@") != -1 ? athDesc.Sort.substring(0, athDesc.Sort.indexOf("@")) : "类别";

    var currImgPath = './Img';
    if (window.location.href.indexOf("CCForm") != -1 || window.location.href.indexOf("CCBill") != -1)
        currImgPath = '../Img';
    //3.是否显示标题列
    if (athDesc.IsShowTitle == 1 && $("#thead_" + athDesc.MyPK).length == 0) {
        _html += "<thead id='thead_" + athDesc.MyPK + "'>";
        _html += "<tr style='border:0px;'>";

        var colstyle = "line-height:30px;border: 1px solid #ddd;background-color:white;";

        _html += "<th  style='" + colstyle + "width:50px;'>序号</th>";
        if (isHaveSort == true)
            _html += "<th style='" + colstyle + "width:120px' nowrap=true >" + sortColoum + "</th>";
        if ((athDesc.IsUpload == 0 || pageData.IsReadonly == "1") || athDesc.IsExpCol == 1)
            _html += "<th  style='" + colstyle + "width:200px'>文件名</th>";
        else
            _html += "<th  style='" + colstyle + "width:200px'>文件名<div style='float:right' id='fileUpload_" + athDesc.MyPK + "' class='fileUploadContent'></div> </th>";
        //_html += "<th  style='" + colstyle + "width:50px;'>大小KB</th>";
        _html += "<th  style='" + colstyle + "width:120px;'>上传时间</th>";
        _html += "<th  style='" + colstyle + "width:80px;'>上传人</th>";
        //增加了扩展列
        if (athDesc.IsExpCol == 1) {
            $.each(mapAttrs, function (i, mapAttr) {
                if (mapAttr.UIIsInput == 1 && mapAttr.UIIsEnable == 1)
                    _html += "<th style='" + colstyle + "'><span style='color:red' class='mustInput' data-keyofen='" + mapAttr.KeyOfEn + "' >*</span>";
                else
                    _html += "<th style='" + colstyle + "'>";
                if (mapAttr.UIContralType == 0)
                    _html += "<label for='TB_" + mapAttr.KeyOfEn + "' class='" + (mapAttr.UIIsInput == 1 ? "mustInput" : "") + "' >" + mapAttr.Name + "</label></th>";

                if (mapAttr.UIContralType == 1)
                    _html += "<label for='DDL_" + mapAttr.KeyOfEn + "' class='" + (mapAttr.UIIsInput == 1 ? "mustInput" : "") + "' >" + mapAttr.Name + "</label></th>";
                if (mapAttr.UIContralType == 2)
                    _html += "<label for='CB_" + mapAttr.KeyOfEn + "' class='" + (mapAttr.UIIsInput == 1 ? "mustInput" : "") + "'  >" + mapAttr.Name + "</label></th>";
                if (mapAttr.UIContralType == 3)
                    _html += "<label for='RB_" + mapAttr.KeyOfEn + "' class='" + (mapAttr.UIIsInput == 1 ? "mustInput" : "") + "'  >" + mapAttr.Name + "</label></th>";
            })
        }
        //排序列的增加
        if (athDesc.IsIdx == 1 && athDesc.IsReadonly != 1) {
            _html += "<th  nowrap=true  style='" + colstyle + "width:50px' >排序</th>";
        }
        //增加操作列
        _html += "<th  nowrap=true  style='" + colstyle + "width:100px' >";
        if (athDesc.IsDownload == 1 && dbs.length > 0)
            _html += "操作" + "<a href=\"javascript:DownZip('" + athDesc.MyPK + "','" + AthParams.PKVal + "')\" ><img src='" + currImgPath + "/FileType/zip.png' style='width:16px;height:16px;margin-left:5px;' alt='打包下载' /></a>";
        else
            _html += "操作";
        _html += "</th>";
        _html += "</thead>";
    }

    //4.增加附件列表的显示
    //4.1.存在分组，增加一个空分组主要是为合并解析没有分组的情况
    if (isHaveSort == true && athDesc.Sort.lastIndexOf(",") + 1 != athDesc.Sort.length)
        athDesc.Sort = athDesc.Sort + ",";

    var fileSorts = athDesc.Sort.indexOf("@") != -1 ? athDesc.Sort.substring(athDesc.Sort.indexOf('@') + 1).split(',') : athDesc.Sort.split(',');

    var athIdx = 0;
    if ($("#tbody_" + athDesc.MyPK).length == 0)
        _html += "<tbody id='tbody_" + athDesc.MyPK + "'>";
    //循环分组
    for (var j = 0; j < fileSorts.length; j++) {
        var sort = fileSorts[j]
        //存在分组只显示分组下的文件
        if (fileSorts.length > 1 && sort == "")
            continue;


        var IsExistFile = false; //该分组是否有文件，不存在文件增加一行空白数据

        var isAddSortTD = false; //是否增加类别所在的列

        for (var k = 0; k < dbs.length; k++) {
            var db = dbs[k];
            if (isHaveSort == true && db.Sort != sort)
                continue;
            IsExistFile = true;
            athIdx++;
            _html += "<tr class='athInfo'>";

            //①序号
            _html += "<td class='Idx' nowrap>" + athIdx + "</td>";

            //增加类别列，有可能跨多行
            if (isHaveSort == true && isAddSortTD == false) {
                isAddSortTD = true;
                var rowSpan = GetSortLenth_FromDB(sort, dbs);
                _html += "<td rowspan=" + rowSpan + " style='text-align:center;vertical-align: middle;'>" + db.Sort + "</td>";
            }

            //②附件名称 ，扩展了预览功能，先阶段需要用户自己在DataUser/OverrideFiles/Ath.js重写AthViewOverWrite_Del方法
            _html += "<td style='text-align:left'><a href=\"javascript:AthView('" + db.MyPK + "');\" ><img src='" + currImgPath + "/FileType/" + db.FileExts + ".gif' border=0 onerror=\"src='" + currImgPath + "/FileType/Undefined.gif'\" style='margin-right:5px;float:left;' />" + db.FileName + "</td>";

            //③附件大小
            //_html += "<td>" + db.FileSize + "</td>";
            //④上传时间
            _html += "<td>" + db.RDT + "</td>";
            //⑤上传人
            _html += "<td>" + db.RecName + "</td>";

            //⑥扩展列数据的增加
            if (athDesc.IsExpCol == 1) {
                $.each(mapAttrs, function (index, mapAttr) {
                    var defVal = GetPara(db.AtPara, mapAttr.KeyOfEn);
                    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) {
                        var senum = new Entity("BP.Sys.SysEnum", mapAttr.UIBindKey + "_CH" + "_" + defVal);
                        defVal = senum.Lab;
                    }
                    if (mapAttr.MyDataType == 4) {
                        if (defVal == "on") defVal = "是";
                        else defVal = "否";
                    }

                    _html += "<td>" + defVal + "</td>";
                });
            }

            //排序列的增加
            if (athDesc.IsIdx == 1 && athDesc.IsReadonly != 1) {
                _html += "<td class='operate'>";
                _html += "<a href=\"javascript:GFDoUp('" + db.MyPK + "' )\"><img src=\"../WF/\Img/\Btn/\Up.GIF\"/></a>";
                _html += "<a href=\"javascript:GFDoDown('" + db.MyPK + "');\"><img src=\"../WF/\Img/\Btn/\Down.GIF\"/></a>";
                _html += "</td>";
            }

            //⑦操作列的增加.
            _html += "<td class='operate'>";
            if (athDesc.IsDownload == 1)
                _html += "<a href=\"javascript:Down2018('" + db.MyPK + "')\"><img src=\"../WF/\Img/\Btn/\Down.gif\"/></a>&nbsp;&nbsp;&nbsp;&nbsp;";
            if (pageData.IsReadonly != 1) {
                if (athDesc.DeleteWay == 1)//删除所有
                    _html += "<a href=\"javascript:Del('" + db.MyPK + "','" + athDesc.MyPK + "','" + db.FileName + "')\"><img src=\"../WF/\Img/\Btn/\Delete.gif\"/></a>";
                var webuser = new WebUser();
                if (athDesc.DeleteWay == 2 && db.Rec == webuser.No)//删除自己上传的
                    _html += "<a href=\"javascript:Del('" + db.MyPK + "','" + athDesc.MyPK + "','" + db.FileName + "')\"><img src=\"../WF/\Img/\Btn/\Delete.gif\"/></a>";
            }
            _html += "</td>";

            _html += "</tr>";
        } //结束数据输出.

        //输出上传功能》
        if (IsExistFile == false || (athDesc.IsExpCol == 1 && athDesc.IsUpload == true && pageData.IsReadonly != "1")) {
            athIdx++;

            //①序号
            _html += "<td class='Idx'>" + athIdx + "</td>";

            //增加类别列
            if (isHaveSort == true)
                _html += "<td>" + sort + "</td>";

            //②附件名称
            if (athDesc.IsUpload == true && pageData.IsReadonly != "1")
                _html += "<td style='width:100px;color:red'>请上传附件..</td>";
            else
                _html += "<td style='width:100px;color:red'></td>";

            //③附件大小
            // _html += "<td>&nbsp&nbsp</td>";
            //④上传时间
            _html += "<td>&nbsp&nbsp</td>";
            //⑤上传人
            _html += "<td>&nbsp&nbsp</td>";

            //⑥扩展列数据的增加
            if (athDesc.IsExpCol == 1) {
                $.each(mapAttrs, function (index, mapAttr) {
                    _html += "<td>" + InitAthMapAttrOfCtrlFool("", mapAttr) + "</td>"
                });
            }
            //⑦操作列的增加.
            if (athDesc.IsExpCol == 1)
                _html += "<td><a href='javaScript:void(0)' onclick='changeSort(\"" + sort + "\",\"" + athDesc.MyPK + "\")'>上传</a>&nbsp;&nbsp;&nbsp;<a href=\"\" onclick='return SaveUpload(\"" + athDesc.MyPK + "\",\"" + uploadUrl + "\")'>保存</a></td>";
            else
                _html += "<td></td>";

            _html += "</tr>";
        }

    }
    //附件可上传并且存在分组，增加分组的选择下拉框
    if (athDesc.IsUpload == true && pageData.IsReadonly != "1" && (isHaveSort == true || athDesc.IsNote)) {
        columnNum += mapAttrs != null ? mapAttrs.length + isHaveSort == true ? 1 : 0 : 0 + isHaveSort == true ? 1 : 0;
        _html += "<tr>";
        _html += "<td colspan=" + columnNum + ">"
        _html += "<div id='file_upload-queue' class='uploadify-queue'></div>";
        var operations = "";
        for (var idx = 0; idx < fileSorts.length; idx++) {
            operations += "<option  value='" + fileSorts[idx] + "'>" + fileSorts[idx] + "</option>";
        }
        _html += "<div id='s' style='text-align:left;float:left;display:inline;width:100%'  >";
        if (isHaveSort == true) {
            _html += "<div style='float:left;padding-right:2px'>";
            _html += "请选择" + sortColoum + "：";
            _html += "<select id='Sort_" + athDesc.MyPK + "' class='form-control' style='margin:0px 0px !important;width:auto !important'>" + operations + "</select>";
            _html += "</div>";
        }
        if (athDesc.IsNote)
            _html += "<input type='text' id='TB_Note' style='width:90%;display:none;' size='30'/>";
        _html += "</div>";
        _html += "</td>";
        _html += "</tr>";
    }

    if ($("#tbody_" + athDesc.MyPK).length == 0) {
        _html += "</tbody>";
        _html += "</table>";
    }

    return _html;

}
// 向上移动.
function GFDoUp(mypk) {

    var en = new Entity("BP.Sys.FrmAttachmentDB", mypk);
    var data = en.DoMethodReturnString("DoUpTabIdx");
    if (data.indexOf('err@') != -1)
        alert(data);
    window.location.href = window.location.href;
}

//向下移动.
function GFDoDown(mypk) {

    var en = new Entity("BP.Sys.FrmAttachmentDB", mypk);
    var data = en.DoMethodReturnString("DoDownTabIdx");
    if (data.indexOf('err@') != -1)
        alert(data);
    window.location.href = window.location.href;
}

/**
* 附件图片显示的方式
* @param athDesc 附件属性
* @param dbs 附件列表
*/
function FileShowPic(athDesc, dbs, uploadUrl) {
    var exts = athDesc.Exts;
    if (exts != null && exts != undefined && (exts.indexOf("*.*") != -1 || exts == ""))
        exts = "image/gif,image/jpg,image/jepg,image/jpeg,image/bmp,image/png,image/tif,image/gsp";

    var _Html = "<form id='Form_" + athDesc.MyPK + "' enctype='multipart/form-data' method='post'>";
    for (var i = 0; i < dbs.length; i++) {
        var db = dbs[i];
        var url = GetFileStream(db.MyPK, db.FK_FrmAttachment);
        _Html += "<div id='" + db.MyPK + "' class='image-item athInfo' style='background-image: url(&quot;" + url + "&quot;);'>";
        if ((athDesc.DeleteWay == 1) || ((athDesc.DeleteWay == 2) && (db.Rec == WebUser.No)))
            _Html += "<div class='image-close' onclick='Del(\"" + db.MyPK + "\",\"" + db.FK_FrmAttachment + "\")'>X</div>";
        _Html += "<div style ='width: 100%; height: 100%;' class='athImg' ></div>";
        _Html += "<div class='image-name' id = 'name-0-0' > ";
        if (athDesc.IsDownload == 0)
            _Html += "<p style = 'text-align:center;width:63.4px;margin:0;padding:0' >" + db.FileName + "</p>";
        else
            _Html += "<p style = 'text-align:center;width:63.4px;margin:0;padding:0' ><a href=\"javascript:Down2018('" + db.MyPK + "');\" >" + db.FileName.split(".")[0] + "</a></p>";
        _Html += "</div>";
        _Html += "</div>";
    }
    //可以上传附件，增加上传附件按钮
    if (athDesc.IsUpload == true && pageData.IsReadonly != "1") {

        _Html += "<div class='image-item space'><input type='file' id='file_" + athDesc.MyPK + "'name='file_" + athDesc.MyPK + "' accept='" + exts + "' onchange='UploadChange(\"" + uploadUrl + "\",\"" + athDesc.MyPK + "\");'></div>";
    }
    _Html += "</form>";

    return _Html;

}

/**
* 图片预览
* @param outerdiv
* @param innerdiv
* @param bigimg
* @param src
*/
function imgShow(outerdiv, innerdiv, bigimg, src) {
    bigimg.attr("src", src); //设置#bigimg元素的src属性  

    /*获取当前点击图片的真实大小，并显示弹出层及大图*/
    $("<img/>").attr("src", src).load(function () {
        var windowW = $(window).width(); //获取当前窗口宽度  
        var windowH = $(window).height(); //获取当前窗口高度  
        var realWidth = this.width; //获取图片真实宽度  
        var realHeight = this.height; //获取图片真实高度  
        var imgWidth, imgHeight;
        var scale = 0.8; //缩放尺寸，当图片真实宽度和高度大于窗口宽度和高度时进行缩放  

        if (realHeight > windowH * scale) {//判断图片高度  
            imgHeight = windowH * scale; //如大于窗口高度，图片高度进行缩放  
            imgWidth = imgHeight / realHeight * realWidth; //等比例缩放宽度  
            if (imgWidth > windowW * scale) {//如宽度扔大于窗口宽度  
                imgWidth = windowW * scale; //再对宽度进行缩放  
            }
        } else if (realWidth > windowW * scale) {//如图片高度合适，判断图片宽度  
            imgWidth = windowW * scale; //如大于窗口宽度，图片宽度进行缩放  
            imgHeight = imgWidth / realWidth * realHeight; //等比例缩放高度  
        } if (realHeight > windowH * scale) {
            imgWidth = windowH * scale;
            imgHeight = windowH * scale;
        } else {//如果图片真实高度和宽度都符合要求，高宽不变  
            imgWidth = realWidth;
            imgHeight = realHeight;
        }
        bigimg.css("width", imgWidth); //以最终的宽度对图片缩放  

        var w = (windowW - imgWidth) / 2; //计算图片与窗口左边距  
        var h = (windowH - imgHeight) / 2; //计算图片与窗口上边距  
        innerdiv.css({ "top": h, "left": w }); //设置#innerdiv的top和left属性  
        outerdiv.fadeIn("fast"); //淡入显示#outerdiv及.pimg  
    });

    outerdiv.click(function () {//再次点击淡出消失弹出层  
        $(this).fadeOut("fast");
    });
}


/**
* 获取分组中的附件列表数据个数
* @param sort 类别
* @param dbs 附件列表
*/
function GetSortLenth_FromDB(sort, dbs) {
    var sortLength = 0;
    for (var p = 0; p < dbs.length; p++) {
        if (dbs[p].Sort == sort) sortLength++;
    }
    return sortLength;
}


//文件数据流
function GetFileStream(mypk, FK_FrmAttachment) {
    var Url = "";
    if (plant == "CCFlow") {
        if (window.location.href.indexOf("/CCForm") != -1)
            Url = './DownFile.aspx?DoType=Down&MyPK=' + mypk + '&FK_FrmAttachment=' + FK_FrmAttachment;
        else if (window.location.href.indexOf("/CCBill") != -1)
            Url = '../CCForm/DownFile.aspx?DoType=Down&MyPK=' + mypk + '&FK_FrmAttachment=' + FK_FrmAttachment;
        else
            Url = './CCForm/DownFile.aspx?DoType=Down&MyPK=' + mypk + '&FK_FrmAttachment=' + FK_FrmAttachment;

    } else {
        //按照数据流模式下载。
        var currentPath = window.document.location.href;
        var path = currentPath.substring(0, currentPath.indexOf('/CCMobile') + 1);
        Url = path + "WF/Ath/downLoad.do?MyPK=" + mypk + "&FK_FrmAttachment=" + FK_FrmAttachment;
    }

    return Url;
}

/**
* 附件下载
* @param fk_ath  附件的属性
* @param MyPK 上传附件数据的信息主键
*/
function Down2018(mypk) {

    var nodeID = GetQueryString("FK_Node");
    var workID = GetQueryString("WorkID");

    var url = "";
    if (plant == "CCFlow")
        url = basePath + '/WF/CCForm/DownFile.aspx?DoType=Down&MyPK=' + mypk + "&WorkID=" + workID + "&FK_Node=" + nodeID;
    else {
        var currentPath = window.document.location.href;
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        url = path + 'WF/Ath/downLoad.do?MyPK=' + mypk + "&WorkID=" + workID + "&FK_Node=" + nodeID;
    }
    if (IEVersion() < 11) {
        window.open(url);
        return;
    }
    var link = document.createElement('a');
    link.setAttribute("download", "");
    link.href = url;
    link.click();

    var x = new XMLHttpRequest();
    x.open("GET", url, true);
    x.responseType = 'blob';
    x.onload = function (e) { download(x.response, fileName, "image/gif"); }
    x.send();
}

/**
* 打包下载
* @param fk_frmattachment 附件属性MyPK
* @param PKVal 附件控制权限的ID
*/
function DownZip(fk_frmattachment, PKVal) {

    var httpHandler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    httpHandler.AddUrlData();
    httpHandler.AddPara("FK_FrmAttachment", fk_frmattachment);
    httpHandler.AddPara("PKVal", PKVal)
    var data = httpHandler.DoMethodReturnString("AttachmentUpload_DownZip");

    if (data.indexOf('err@') == 0) {
        alert(data); //如果是异常，就提提示.
        return;
    }

    if (data.indexOf('url@') == 0) {

        data = data.replace('url@', ''); //如果返回url，就直接转向.

        var i = data.indexOf('\DataUser');
        var str = '/' + data.substring(i);
        str = str.replace('\\\\', '\\');
        if (plant != 'CCFlow') {
            var currentPath = window.document.location.href;
            var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
            str = path + str;
        } else {
            str = basePath + str;
        }
        window.location.href = str;



    }

}

/**
* 删除附件
* @param delPKVal
*/
function Del(delPKVal, fk_framAttachment, name) {
    if (window.confirm('您确定要删除吗？ ') == false)
        return;
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CCForm");
    handler.AddPara("DelPKVal", delPKVal);
    var data = handler.DoMethodReturnString("AttachmentUpload_Del");
    if (data.indexOf("err@") != -1) {
        alert(data);
        console.log(data);
        return;
    }
    var opt = AthParams.Opt;
    var fileListArray = uploadFileList.getFileList(opt);
    var newFileListArray = [];
    for (var i = 0; i < fileListArray.length; i++) {
        if (fileListArray[i].name == name)
            continue;
        newFileListArray.push(fileListArray[i]);
    }

    uploadFileList.setFileList(newFileListArray, opt);
    //获取
    InitAthPage("Div_" + fk_framAttachment);
}

//在线预览，如果需要连接其他的文件预览查看器，就需要在这里重写该方法.
function AthView(mypk) {

    if (typeof AthViewOverWrite === 'function') {
        AthViewOverWrite(mypk);
        return;
    }

    var nodeID = GetQueryString("FK_Node");
    var workID = GetQueryString("WorkID");

    if (plant == "CCFlow") {
        window.location.href = basePath + '/WF/CCForm/DownFile.aspx?DoType=Down&MyPK=' + mypk + '&PKVal=' + mypk + '&FK_Node=' + nodeID + "&WorkID=" + workID;
        return;
    }

    var currentPath = window.document.location.href;
    var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
    Url = path + 'WF/Ath/downLoad.do?DelPKVal=' + mypk + '&PKVal=' + mypk + '&FK_Node=' + nodeID + "&WorkID=" + workID;
    window.location.href = Url;
}

/**
 * 更改附件上传的分类类别
 * @param {any} sort
 * @param {any} FK_FrmAttachment
 */
function changeSort(sort, FK_FrmAttachment) {
    $("#Sort_" + FK_FrmAttachment).val(sort);

    /*if ($("#file_" + FK_FrmAttachment).length == 0) {
        var inputObj = document.createElement('input');
        inputObj.setAttribute('id', "file_" + FK_FrmAttachment);
        inputObj.setAttribute('name', "file_" + FK_FrmAttachment);
        inputObj.setAttribute('type', 'file');
        document.forms[0].appendChild(inputObj);
    }
    document.getElementById("file_" + FK_FrmAttachment).click();*/
    //激活上传文件
    $("#fileUpload_" + FK_FrmAttachment + " .uploadBts .selectFileBt").click()

}


function SaveUpload(fk_frmAttachment, uploadUrl) {

    //必填项和正则表达式检查
    var formCheckResult = true;

    if (checkBlanks() == false) {
        formCheckResult = false;
    }

    if (checkReg() == false) {
        formCheckResult = false;
    }

    if (formCheckResult == false) {
        alert("请检查表单必填项和正则表达式");
        return false;
    }


    UploadChange(uploadUrl, fk_frmAttachment);
}

/**
 * 图片附件上传
 */
function UploadChange(uploadUrl, fk_frmAttachment) {
    if ($("#file_" + fk_frmAttachment).length == 0)
        return;
    var fileObj = $("#file_" + fk_frmAttachment).val();
    if (typeof (fileObj) == "undefined" || fileObj.size <= 0) {
        alert("请选择上传的文件.");
        return;
    }

    var file = document.getElementById("file_" + fk_frmAttachment).files[0];
    var fileSize = AthParams.AthInfo[fk_frmAttachment][0][2];
    if (file.size * 1000 > fileSize) {
        alert("上传附件大小的最大限制是" + fileSize + "KB");
        return;
    }
    var fileExt = fileObj.substring(fileObj.lastIndexOf(".")).toLowerCase();

    var exts = AthParams.AthInfo[fk_frmAttachment][0][3];
    if (exts == null || exts == "" || exts == undefined)
        exts = "*.*";
    //附件的后缀
    if (exts != "*.*" && exts.indexOf(fileExt) == -1) {
        alert("附件上传的格式是" + exts);
        return;
    }




    //form表单序列话
    var parasData = $("form").serialize();
    //form表单序列化时调用了encodeURLComponent方法将数据编码了
    parasData = decodeURIComponent(parasData, true);
    parasData = decodeURIComponent(parasData, true);
    parasData = parasData.replace(/&/g, '@');
    parasData = parasData.replace(/TB_/g, '');
    parasData = parasData.replace(/RB_/g, '');
    parasData = parasData.replace(/CB_/g, '');
    parasData = parasData.replace(/DDL_/g, '');
    uploadUrl += "&parasData=" + parasData;

    //提交数据
    var option = {
        url: uploadUrl,
        type: 'POST',
        dataType: 'json',
        headers: { "ClientCallMode": "ajax" },
        success: function (data) {
            if (data != null && data.indexOf("err@") != -1) {
                alert(data.replace("err@", ""));
                return;
            }
            if (typeof AfterAthUploadOver === 'function')
                AfterAthUploadOver(fk_frmAttachment, AthParams.PKVal, xhr.responseText.substring(0, xhr.responseText.length - 1));
            InitAthPage("Div_" + fk_frmAttachment);
        },
        error: function (xhr, status, err) {
            if (xhr.responseText != null && xhr.responseText.indexOf("err@") != -1) {
                alert(xhr.responseText);
                return;
            }
            if (typeof AfterAthUploadOver === 'function')
                AfterAthUploadOver(fk_frmAttachment, AthParams.PKVal, xhr.responseText.substring(0, xhr.responseText.length - 1));
            InitAthPage("Div_" + fk_frmAttachment);
        }

    };

    $("form").ajaxSubmit(option);
    return false;


}




////关闭窗口  适用于扩展属性
//function close() {
//    if (parent != undefined && parent.SetAth != undefined && typeof (parent.SetAth) == "function") {
//        var nameTds = $('.Idx').next();
//        var nameStrs = [];
//        $.each(nameTds, function (i, nameTd) {
//            nameStrs.push($(nameTd).children('a').text());
//        })
//        parent.SetAth(nameStrs);
//    }
//}
//解析附件扩张字段
function InitAthMapAttrOfCtrlFool(db, mapAttr) {
    var defValue = "";
    if (db == !"")
        defValue = GetPara(mapAttr.Name, db.AtPara)
    var eleHtml = '';

    //外部数据源类型.
    if (mapAttr.LGType == "0" && mapAttr.MyDataType == "1" && mapAttr.UIContralType == 1) {

        if (mapAttr.UIIsEnable == 0) {
            var ctrl = "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' type=hidden  class='form-control' type='text'/>";
            defValue = ConvertDefVal(flowData, mapAttr.DefVal, mapAttr.KeyOfEn + "T");

            if (defValue == '' || defValue == null)
                defValue = '无';

            ctrl += "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "Text'  value='" + defValue + "' disabled='disabled'   type='text'/>";
            return ctrl;
        }


        return "<select id='DDL_" + mapAttr.KeyOfEn + "' name='DDL_" + mapAttr.KeyOfEn + "'  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitDDLOperation(mapAttr, defValue) + "</select>";
    }

    //外键类型.
    if (mapAttr.LGType == "2" && mapAttr.MyDataType == "1") {

        var data = flowData[mapAttr.UIBindKey];
        //枚举类型.
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";


        return "<select id='DDL_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitDDLOperation(mapAttr, defValue) + "</select>";
    }

    //添加文本框 ，日期控件等.
    //AppString
    if (mapAttr.MyDataType == "1") {  //不是外键

        if (mapAttr.UIHeight <= 40) //普通的文本框.
            return "<input maxlength=" + mapAttr.MaxLen + "  id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'   type='text'/>";

        if (mapAttr.AtPara && mapAttr.AtPara.indexOf("@IsRichText=1") >= 0) {

            var eleHtml = "";
            //如果是富文本就使用百度 UEditor
            if (mapAttr.UIIsEnable == "0") {
                //只读状态直接 div 展示富文本内容                
                eleHtml += "<div class='richText' style='width:99%;margin-right:2px'>" + defValue + "</div>";

            } else {
                document.BindEditorMapAttr = mapAttr; //存到全局备用

                //设置编辑器的默认样式
                var styleText = "text-align:left;font-size:12px;";
                styleText += "width:100%;";
                styleText += "height:" + mapAttr.UIHeight + "px;";
                //注意这里 name 属性是可以用来绑定表单提交时的字段名字的 id 是特殊约定的.
                eleHtml += "<script id='editor'  name='TB_" + mapAttr.KeyOfEn + "' type='text/plain' style='" + styleText + "'>" + defValue + "</script>";

            }

            eleHtml = "<div style='white-space:normal;'>" + eleHtml + "</div>";
            return eleHtml
        }

        //普通的大块文本.
        return "<textarea maxlength=" + mapAttr.MaxLen + "  style='height:" + mapAttr.UIHeight + "px;width:100%;' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' type='text'  " + (mapAttr.UIIsEnable == 1 ? '' : ' disabled="disabled"') + " />"
    }

    //日期类型.
    if (mapAttr.MyDataType == 6) {
        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return "<input type='text' " + enableAttr + " value='" + defValue + "' style='width:120px;' class='form-control' id='TB_" + mapAttr.KeyOfEn + "'  name='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    //时期时间类型.
    if (mapAttr.MyDataType == 7) {

        var enableAttr = '';
        if (mapAttr.UIIsEnable == 1)
            enableAttr = 'onfocus="WdatePicker({dateFmt:' + "'yyyy-MM-dd HH:mm'})" + '";';
        else
            enableAttr = "disabled='disabled'";

        return "<input  type='text'  value='" + defValue + "' style='width:140px;' class='form-control' " + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "' />";
    }

    // boolen 类型.
    if (mapAttr.MyDataType == 4) {  // AppBoolean = 7

        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";

        //CHECKBOX 默认值
        var checkedStr = '';
        if (checkedStr != "true" && checkedStr != '1') {
            checkedStr = ' checked="checked" ';
        }

        return "<label ><input " + enableAttr + " " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' id='CB_" + mapAttr.KeyOfEn + "'  name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + " /> </label>";
    }

    //枚举类型.
    if (mapAttr.MyDataType == 2 && mapAttr.LGType == 1) { //AppInt Enum
        if (mapAttr.UIIsEnable == 1)
            enableAttr = "";
        else
            enableAttr = "disabled='disabled'";
        if (mapAttr.UIContralType == 1)
            //return "<select " + enableAttr + "  id='DDL_" + mapAttr.KeyOfEn + "' class='form-control' >" + InitDDLOperation(flowData, mapAttr, defValue) + "</select>";
            return "<select id='DDL_" + mapAttr.KeyOfEn + "' name='DDL_" + mapAttr.KeyOfEn + "' class='form-control'  onchange='changeEnable(this,\"" + mapAttr.FK_MapData + "\",\"" + mapAttr.KeyOfEn + "\",\"" + mapAttr.AtPara + "\")'>" + InitAthDDLOperation(mapAttr, defValue) + "</select>";
        if (mapAttr.UIContralType == 3) {
            //横向排列
            var RBShowModel = 3;
            if (mapAttr.AtPara.indexOf("@RBShowModel=3") == -1)
                RBShowModel = 0;
            return InitRBShowContentAth(mapAttr, defValue, RBShowModel, enableAttr);

        }
    }

    // AppDouble  AppFloat
    if (mapAttr.MyDataType == 5 || mapAttr.MyDataType == 3) {
        return "<input  value='" + defValue + "' style='text-align:right;width:80px;'class='form-control'  onkeyup=" + '"' + "if(isNaN(value)) execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    if ((mapAttr.MyDataType == 2)) { //AppInt
        var enableAttr = '';
        if (mapAttr.UIIsEnable != 1) {
            enableAttr = "disabled='disabled'";
        }

        //alert(defValue);

        return "<input  value='0' style='text-align:right;width:80px;' class='form-control' onkeyup=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value) || (value%1 !== 0))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text'" + enableAttr + " id='TB_" + mapAttr.KeyOfEn + "' name='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    //AppMoney  AppRate
    if (mapAttr.MyDataType == 8) {
        return "<input  value='" + defValue + "' style='text-align:right;width:80px;' class='form-control' onkeyup=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " onafterpaste=" + '"' + "if(isNaN(value))execCommand('undo')" + '"' + " maxlength=" + mapAttr.MaxLen / 2 + "   type='text' name='TB_" + mapAttr.KeyOfEn + "' id='TB_" + mapAttr.KeyOfEn + "'/>";
    }

    alert(mapAttr.Name + "的类型没有判断.");
    return;
}

//初始化下拉列表框的OPERATION
function InitAthDDLOperation(mapAttr, defVal) {

    var operations = '';

    //外键类型的.
    if (mapAttr.LGType == 2) {

        if (flowData[mapAttr.KeyOfEn] != undefined) {

            $.each(flowData[mapAttr.KeyOfEn], function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });

        }

        if (flowData[mapAttr.UIBindKey] != undefined) {

            $.each(flowData[mapAttr.UIBindKey], function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
        }
        return operations;
    }

    //枚举类型的.
    if (mapAttr.LGType == 1) {
        var enums = new Entities("BP.Sys.SysEnums");
        enums.Retrieve("EnumKey", mapAttr.UIBindKey);


        $.each(enums, function (i, obj) {
            operations += "<option " + (obj.IntKey == defVal ? " selected='selected' " : "") + " value='" + obj.IntKey + "'>" + obj.Lab + "</option>";
        });
        return operations;
    }


    //外部数据源类型 FrmGener.js.InitDDLOperation
    if (mapAttr.LGType == 0) {

        //如果是一个函数.
        var fn;
        try {
            if (mapAttr.UIBindKey) {
                fn = eval(mapAttr.UIBindKey);
            }
        } catch (e) {
            // alert(e);
        }

        if (typeof fn == "function") {
            $.each(fn.call(), function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
            return operations;
        }

        if (typeof CommonHandler == "function") {
            CommonHandler.call("", mapAttr.UIBindKey, function (data) {
                GenerBindDDL("DDL_" + mapAttr.KeyOfEn, data, "No", "Name");
            })
            return "";
        }

        if (mapAttr.UIIsEnable == 0) {

            alert('不可编辑');
            operations = "<option  value='" + defVal + "'>" + defVal + "</option>";
            return operations;
        }

        if (flowData[mapAttr.KeyOfEn] != undefined) {
            $.each(flowData[mapAttr.KeyOfEn], function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
            return operations;
        }

        if (flowData[mapAttr.UIBindKey] != undefined) {

            $.each(flowData[mapAttr.UIBindKey], function (i, obj) {
                operations += "<option " + (obj.No == defVal ? " selected='selected' " : "") + " value='" + obj.No + "'>" + obj.Name + "</option>";
            });
            return operations;
        }
        return "";
        //   alert('没有获得约定的数据源.');
        alert('没有获得约定的数据源..' + mapAttr.KeyOfEn + " " + mapAttr.UIBindKey);
    }

    alert(mapAttr.LGType + "没有判断.");
}

function InitRBShowContentAth(mapAttr, defValue, RBShowModel, enableAttr) {
    var rbHtml = "";
    var enums = new Entities("BP.Sys.SysEnums");
    enums.Retrieve("EnumKey", mapAttr.UIBindKey);
    enums = $.grep(enums, function (value) {
        return value.EnumKey == mapAttr.UIBindKey;
    });
    $.each(enums, function (i, obj) {
        if (RBShowModel == 3)
            //<input  " + (defValue == 1 ? "checked='checked'" : "") + " type='checkbox' id='CB_" + mapAttr.KeyOfEn + "'  name='CB_" + mapAttr.KeyOfEn + "' " + checkedStr + " /> &nbsp;" + mapAttr.Name + "</label</div>";
            rbHtml += "<label><input " + enableAttr + " " + (obj.IntKey == defValue ? "checked='checked' " : "") + " type='radio' name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "' />&nbsp;" + obj.Lab + "</label>";
        else
            rbHtml += "<label><input " + enableAttr + " " + (obj.IntKey == defValue ? "checked='checked' " : "") + " type='radio' name='RB_" + mapAttr.KeyOfEn + "' id='RB_" + mapAttr.KeyOfEn + "_" + obj.IntKey + "' value='" + obj.IntKey + "'  />&nbsp;" + obj.Lab + "</label><br/>";
    });
    return rbHtml;
}


////必填项检查   名称最后是*号的必填  如果是选择框，值为'' 或者 显示值为 【*请选择】都算为未填 返回FALSE 检查必填项失败
//function checkBlanks() {
//    var checkBlankResult = true;
//    //获取所有的列名 找到带* 的LABEL mustInput
//    //var lbs = $('[class*=col-md-1] label:contains(*)');
//    var lbs = $('.mustInput'); //获得所有的class=mustInput的元素.
//    $.each(lbs, function (i, obj) {
//        if ($(obj).parent().css('display') != 'none' && $(obj).parent().next().css('display')) {
//            var keyofen = $(obj).data().keyofen;

//            var ele = $('[id$=_' + keyofen + ']');
//            if (ele.length == 1) {
//                switch (ele[0].tagName.toUpperCase()) {
//                    case "INPUT":
//                        if (ele.attr('type') == "text") {
//                            if (ele.val() == "") {
//                                checkBlankResult = false;
//                                ele.addClass('errorInput');
//                            } else {
//                                ele.removeClass('errorInput');
//                            }
//                        }
//                        break;
//                    case "SELECT":
//                        if (ele.val() == "" || ele.children('option:checked').text() == "*请选择") {
//                            checkBlankResult = false;
//                            ele.addClass('errorInput');
//                        } else {
//                            ele.removeClass('errorInput');
//                        }
//                        break;
//                    case "TEXTAREA":
//                        if (ele.val() == "") {
//                            checkBlankResult = false;
//                            ele.addClass('errorInput');
//                        } else {
//                            ele.removeClass('errorInput');
//                        }
//                        break;
//                }
//            }
//        }
//    });


//    //2.对 UMEditor 中的必填项检查
//    if (document.activeEditor != null && document.activeEditor.$body != null) {

//    }   

//    return checkBlankResult;
//}

////正则表达式检查
//function checkReg() {
//    var checkRegResult = true;
//    var regInputs = $('.CheckRegInput');
//    $.each(regInputs, function (i, obj) {
//        var name = obj.name;
//        var mapExtData = $(obj).data();
//        if (mapExtData.Doc != undefined) {
//            var regDoc = mapExtData.Doc.replace(/【/g, '[').replace(/】/g, ']').replace(/（/g, '(').replace(/）/g, ')').replace(/｛/g, '{').replace(/｝/g, '}').replace(/，/g, ',');
//            var tag1 = mapExtData.Tag1;
//            if ($(obj).val() != undefined && $(obj).val() != '') {

//                var result = CheckRegInput(name, regDoc, tag1);
//                if (!result) {
//                    $(obj).addClass('errorInput');
//                    checkRegResult = false;
//                } else {
//                    $(obj).removeClass('errorInput');
//                }
//            }
//        }
//    });

//    return checkRegResult;
//}

