

//1.初始化附件全局使用的参数
if (typeof AthParams == "undefined") {
    AthParams = {};
    AthParams.AthInfo = {};

}
if (typeof athRefPKVal == "undefined")
    athRefPKVal = 0;
var IsOnlinePreviewOfAth = getConfigByKey("IsOnlinePreviewOfAth", false);
var PreviewPathOfAth = getConfigByKey("PreviewPathOfAth", "");
/**
* 附件初始化
* @param athchment 附件属性
* @param athDivID 生成的附件信息追加的位置
*/

function AthTable_Init(athchment, athDivID, refPKVal) {
    if (typeof athchment != "object" && typeof athchment == "string")
        athchment = new Entity("BP.Sys.FrmAttachment", athchment);
    if (refPKVal == null || refPKVal == undefined || refPKVal == 0)
        athRefPKVal = pageData.WorkID == 0 ? pageData.OID : pageData.WorkID;
    else
        athRefPKVal = refPKVal;

    AthParams.FK_MapData = athchment.FK_MapData;
    var uploadUrl = "";
    //2.上传的URL的设置
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
            AthTable_Init(athchment, athDivID, pageData.WorkID);
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
        SetHref(url);
        return;
    }

    data = JSON.parse(data);
    athDesc = data["AthDesc"][0]; // 附件属性
    var dbs = data["DBAths"];  // 附件列表数据
    //不显示附件分组
    if (athDesc.IsVisable == "0") {
        $("#" + athDivID).hide();
        //傻瓜表单隐藏分组Lab
        $("#Group_" + athDesc.MyPK).hide();

        //如果是开发者表单
        var parent = $("#" + athDivID).parent()[0];
        if (parent && parent.tagName.toLowerCase() == "td") {

            //当前节点的兄弟节点，如果没有input，select,就隐藏
            var prev = $(parent).prev();
            if (prev[0].tagName.toLowerCase() == "td" && prev[0].innerText == athDesc.Name)
                prev.hide();
        }
    }
    console.log(dbs);

    //2.自定义表单模式.
    if (athDesc.AthRunModel == 2) {
        src = "../../DataUser/OverrideFiles/AthSelf.htm?PKVal=" + AthParams.PKVal + "&Ath=" + athDesc.NoOfObj + "&FK_MapData=" + athDesc.FK_MapData + "&FK_FrmAttachment=" + athDesc.MyPK;
        SetHref(src);
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
    // debugger
    //4.1.图片的展示方式
    if (athDesc.FileType == 1) {
        $("#" + athDivID).html(FileShowPic(athDesc, dbs, uploadUrl));
        $(".athImg").on("click", function () {
            var _this = $(this); //将当前的pimg元素作为_this传入函数  
            var src = _this.parent().css("background-image").replace("url(\"", "").replace("\")", "")
            imgShow(this, src);
        });
        $(".athImg").on("mousemove", function () {
            debugger
            var _this = $(this);
            $(_this.children()[0]).show();
        })
        $(".athImg").on("mouseout", function () {
            var _this = $(this);
            $(_this.children()[0]).hide();
        })
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
    $("#" + athDivID).show();
    layui.form.render("select");
}



/**
* 生成附件列表的Html代码(Table模式)
* @param athDesc 附件属性
* @param dbs 附件列表
*/
var columnNum = 6;
function FileShowWayTable(athDesc, dbs, uploadUrl) {
    var _html = "<table class='layui-table'>";
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
    if (GetHrefUrl().indexOf("CCForm") != -1 || GetHrefUrl().indexOf("CCBill") != -1)
        currImgPath = '../Img';
    //3.是否显示标题列
    if (athDesc.IsShowTitle == 1 && $("#thead_" + athDesc.MyPK).length == 0) {
        _html += "<thead id='thead_" + athDesc.MyPK + "'>";
        _html += "<tr style='border:0px;'>";

        var colstyle = "line-height:30px;border: 1px solid #ddd;background-color:white;";
        colstyle = "background-color:#FAFAFA !important; ";
        _html += "<th  style='" + colstyle + "width:50px;'>序号</th>";
        if (isHaveSort == true)
            _html += "<th style='" + colstyle + "width:120px' nowrap=true >" + sortColoum + "</th>";
        //if ((athDesc.IsUpload == 0 || pageData.IsReadonly == "1") || athDesc.IsExpCol == 1)
        //    _html += "<th  style='" + colstyle + "width:200px'>文件名</th>";
        //else
        _html += "<th  style='" + colstyle + "width:200px'>文件名</th>";
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
        _html += "<th  nowrap=true  style='" + colstyle + "width:23%;text-align: center;' >";
        if (athDesc.IsDownload == 1 && dbs.length > 0)
            _html += "操作" + "<a href=\"javascript:DownZip('" + athDesc.MyPK + "','" + AthParams.PKVal + "')\" ><img src='" + currImgPath + "/FileType/zip.png' style='width:16px;height:16px;margin-left:5px;' alt='打包下载' /></a>";
        else
           _html += "操作";

        //if (((athDesc.IsUpload != 0 || pageData.IsReadonly != "1") || athDesc.IsExpCol != 1) && _html.indexOf("操作") != -1) {
        //    _html += "<div style='float:right' id='fileUpload_" + athDesc.MyPK + "' class='fileUploadContent'></div> ";
        //} else if ((((athDesc.IsUpload != 0 || pageData.IsReadonly != "1") || athDesc.IsExpCol != 1) && _html.indexOf("操作") == -1)) {
        //    _html += "操作" + "<div style='float:right' id='fileUpload_" + athDesc.MyPK + "' class='fileUploadContent'></div> ";
        //}
        //if(_html.indexOf("操作") == -1)
        _html += "</th>";
        _html += "</thead>";
    }

    //4.增加附件列表的显示
    //4.1.存在分组，增加一个空分组主要是为合并解析没有分组的情况
    if (isHaveSort == true && athDesc.Sort.lastIndexOf(",") + 1 != athDesc.Sort.length)
        athDesc.Sort = athDesc.Sort + ",";
    athDesc.Sort = athDesc.Sort == null ? "" : athDesc.Sort;
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

        var imgUrl = "../";
        var localPath = GetHrefUrl();
        if (localPath.indexOf("CCBill") != -1 || localPath.indexOf("CCForm") != -1)
            imgUrl = "../../";


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
            //  debugger
            var filePath = db.FileFullName;
            var i = filePath.indexOf('\DataUser');
            var str = '/' + filePath.substring(i);
            filePath = str.replace(new RegExp("\\\\", "gm"), "/");
            //②附件名称 ，扩展了预览功能，先阶段需要用户自己在DataUser/OverrideFiles/Ath.js重写AthViewOverWrite_Del方法
            _html += "<td style='text-align:left'><a href=\"javascript:AthView('" + db.MyPK + "','" + filePath + "');\" ><img src='" + currImgPath + "/FileType/" + db.FileExts + ".gif' border=0 onerror=\"src='" + currImgPath + "/FileType/Undefined.gif'\" style='margin-right:5px;float:left;' />" + db.FileName + "</td>";

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
                _html += "<a href=\"javascript:GFDoUp('" + db.MyPK + "' )\"><img src=\"" + imgUrl + "WF/\Img/\Btn/\Up.GIF\"/></a>";
                _html += "<a href=\"javascript:GFDoDown('" + db.MyPK + "');\"><img src=\"" + imgUrl + "WF/\Img/\Btn/\Down.GIF\"/></a>";
                _html += "</td>";
            }

            //⑦操作列的增加.
            _html += "<td  style='text-align: center;'class='operate'>";
            if (isHaveSort == true)
                _html += "<a href='javaScript:void(0)' onclick='changeSort(\"" + sort + "\",\"" + athDesc.MyPK + "\")'>上传</a>";
            if (athDesc.IsDownload == 1)
                _html += "<a href=\"javascript:Down2018('" + db.MyPK + "')\">下载</a>&nbsp;&nbsp;&nbsp;&nbsp;";
            if (pageData.IsReadonly != 1) {
                if (athDesc.DeleteWay == 1)//删除所有
                    _html += "<a style='color:red;' href=\"javascript:Del('" + db.MyPK + "','" + athDesc.MyPK + "','" + db.FileName + "')\">删除</a>";
                var webuser = new WebUser();
                if (athDesc.DeleteWay == 2 && db.Rec == webuser.No)//删除自己上传的
                    _html += "<a style='color:red;' href=\"javascript:Del('" + db.MyPK + "','" + athDesc.MyPK + "','" + db.FileName + "')\">删除</a>";
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
            if (isHaveSort == true) {
                _html += "<td><a href='javaScript:void(0)' onclick='changeSort(\"" + sort + "\",\"" + athDesc.MyPK + "\")'>上传</a>";
                if (athDesc.IsExpCol == 1) {
                    _html += "&nbsp;&nbsp;&nbsp;<a href=\"\" onclick='return SaveUpload(\"" + athDesc.MyPK + "\",\"" + uploadUrl + "\")'>保存</a>";
                }
                _html += "</td>";
            }
            else {
                if (athDesc.IsExpCol == 1) {
                    _html += "<td><a href=\"\" onclick='return SaveUpload(\"" + athDesc.MyPK + "\",\"" + uploadUrl + "\")'>保存</a></td>";
                } else
                    _html += "<td></td>";
            }
               

            _html += "</tr>";
        }

    }
    //附件可上传并且存在分组，增加分组的选择下拉框   && (isHaveSort == true || athDesc.IsNote)
    //  debugger;
    if ((athDesc.IsUpload == true && pageData.IsReadonly != "1") && athDesc.IsExpCol != 1) {
        columnNum += mapAttrs != null ? mapAttrs.length + isHaveSort == true ? 1 : 0 : 0 + isHaveSort == true ? 1 : 0;
        _html += "<tr>";
        _html += "<td colspan=" + columnNum + ">"
        _html += "<div id='file_upload-queue' class='uploadify-queue'></div>";

        _html += "<div id='s' style='text-align:left;float:left;display:inline;width:100%'  >";
        if (isHaveSort == false)
            _html += "<div style='float:right' id='fileUpload_" + athDesc.MyPK + "' class='fileUploadContent'></div> ";
        else {
            _html += "<div style='float:right;display:none' id='fileUpload_" + athDesc.MyPK + "' class='fileUploadContent'></div> ";
            _html += "<input id='Sort_" + athDesc.MyPK + "'style='display:none'>";
        }
        //if (isHaveSort == true || athDesc.IsNote) {
        //    var operations = "";
        //    for (var idx = 0; idx < fileSorts.length; idx++) {
        //        operations += "<option  value='" + fileSorts[idx] + "'>" + fileSorts[idx] + "</option>";
        //    }

        //    if (isHaveSort == true) {
        //        _html += "<div style='float:left;padding-right:2px'>";
        //        _html += "请选择" + sortColoum + "：";
        //        _html += "<select id='Sort_" + athDesc.MyPK + "' class='form-control' style='margin:0px 0px !important;width:auto !important'>" + operations + "</select>";
        //        _html += "</div>";
        //    }
        //    if (athDesc.IsNote)
        //        _html += "<input type='text' id='TB_Note' style='width:90%;display:none;' size='30'/>";
        //}
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
    Reload();
}

//向下移动.
function GFDoDown(mypk) {

    var en = new Entity("BP.Sys.FrmAttachmentDB", mypk);
    var data = en.DoMethodReturnString("DoDownTabIdx");
    if (data.indexOf('err@') != -1)
        alert(data);
    Reload();
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
        if(pageData.IsReadonly != 1 &&((athDesc.DeleteWay == 1) || ((athDesc.DeleteWay == 2) && (db.Rec == webUser.No))))
            _Html += "<div class='image-close' onclick='Del(\"" + db.MyPK + "\",\"" + db.FK_FrmAttachment + "\")'>X</div>";
        _Html += "<div style ='width: 100%; height: 100%;' class='athImg' >";
        _Html += "<div class='Img_ShowText'><span>上传人:"+db.RecName+"</span><br/><span>上传时间:"+db.RDT+"</span></div>";
        _Html +="</div > ";
        _Html += "<div class='image-name' id = 'name-0-0' > ";
        if (athDesc.IsDownload == 0)
            _Html += "<p style = 'text-align:center;width:63.4px;margin:0;padding:0;overflow:hidden;text-overflow: ellipsis;white-space: nowrap' >" + db.FileName + "</p>";
        else
            _Html += "<p style = 'text-align:center;width:63.4px;margin:0;padding:0;overflow:hidden;text-overflow: ellipsis;white-space: nowrap' ><a href=\"javascript:Down2018('" + db.MyPK + "');\" title='" + db.FileName.split(".")[0] + "'>" + db.FileName.split(".")[0] + "</a></p>";
        _Html += "</div>";
        _Html += "</div>";
    }
    //可以上传附件，增加上传附件按钮
    if (athDesc.IsUpload == true && pageData.IsReadonly != "1") {

        _Html += "<div class='image-item space'><input type='file' id='file_" + athDesc.MyPK + "'name='file_" + athDesc.MyPK + "' accept='" + exts + "' onchange='UploadChangeAth(\"" + uploadUrl + "\",\"" + athDesc.MyPK + "\");'></div>";
    }
    _Html += "</form>";

    return _Html;

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
        Url = basePath + "/WF/Comm/Handler.ashx?DoType=HttpHandler&DoMethod=AttachmentUpload_Down&HttpHandlerName=BP.WF.HttpHandler.WF_CCForm&WorkID=" + GetQueryString("WorkID") + "&FK_Node=" + GetQueryString("FK_Node") + "&MyPK=" + mypk;
    } else {
        //按照数据流模式下载。
        Url = basePath + "/WF/Ath/downLoad.do?MyPK=" + mypk + "&FK_FrmAttachment=" + FK_FrmAttachment;
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
    if (plant == "CCFlow") {
        SetHref(basePath + "/WF/Comm/Handler.ashx?DoType=HttpHandler&DoMethod=AttachmentUpload_Down&HttpHandlerName=BP.WF.HttpHandler.WF_CCForm&WorkID=" + workID + "&FK_Node=" + nodeID + "&MyPK=" + mypk);
        return;
    }


    var currentPath = GetHrefUrl();
    var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
    url = path + 'WF/Ath/downLoad.do?MyPK=' + mypk + "&WorkID=" + workID + "&FK_Node=" + nodeID;
    if(typeof filterXSS === 'function'){
        url = filterXSS(url);
    }else {
        url = url.replace(/<\/?[^>]+>/gi, '')
        .replace(/[(]/g, '')
        .replace(/->/g, '_')
    }
    if (IEVersion() < 11) {
        window.open(url);
        return;
    }
    var link = document.createElement('a');
    link.setAttribute("download", "");
    link.href = url;
    link.click();

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
            var currentPath = GetHrefUrl();
            var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
            str = path + str;
        } else {
            str = basePath + str;
        }
        SetHref(str);
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
    if (opt != undefined) {
        var fileListArray = uploadFileList.getFileList(opt);
        var newFileListArray = [];
        for (var i = 0; i < fileListArray.length; i++) {
            if (fileListArray[i].name == name)
                continue;
            newFileListArray.push(fileListArray[i]);
        }

        uploadFileList.setFileList(newFileListArray, opt);
    }

    //获取
    InitAthPage("Div_" + fk_framAttachment);
    AthTable_Init(fk_framAttachment, "Div_" + fk_framAttachment, pageData.WorkID);
}

//在线预览，如果需要连接其他的文件预览查看器，就需要在这里重写该方法.
function AthView(mypk, filePath) {
    debugger;
    if (typeof AthViewOverWrite === 'function') {
        AthViewOverWrite(mypk);
        return;
    }
    if (typeof IsOnlinePreviewOfAth == "undefined")
        IsOnlinePreviewOfAth = true;

    if (IsOnlinePreviewOfAth == true) {

        //配置的在线预览的方式，待处理.
        var host = window.location.protocol + "//" + window.location.host;

        var url = host + filePath;
        //url = 'http://localhost:2296/DataUser/UploadFile/ND18201/838/6456dd46-04ec-4843-a057-31351053cd42.混合 - 副本.docx';
        url = encodeURIComponent(base64Encode(url));
        //debugger;
        //预览文件服务器.
        var fileServerHost = PreviewPathOfAth;

        //  window.open("/home/OA/jflow-web/DataUser/UploadFile" + lujin[1], "_blank");
        //对它进行编码 .

        window.open(fileServerHost + '/onlinePreview?url=' + url);
        return;
    }

    Down2018(mypk);
}
function base64Encode(input) {
    let _keyStr = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
    let output = "";
    let chr1, chr2, chr3, enc1, enc2, enc3, enc4;
    let i = 0;
    input = this.utf8_encode(input);
    while (i < input.length) {
        chr1 = input.charCodeAt(i++);
        chr2 = input.charCodeAt(i++);
        chr3 = input.charCodeAt(i++);
        enc1 = chr1 >> 2;
        enc2 = ((chr1 & 3) << 4) | (chr2 >> 4);
        enc3 = ((chr2 & 15) << 2) | (chr3 >> 6);
        enc4 = chr3 & 63;
        if (isNaN(chr2)) {
            enc3 = enc4 = 64;
        } else if (isNaN(chr3)) {
            enc4 = 64;
        }
        output = output +
            _keyStr.charAt(enc1) + _keyStr.charAt(enc2) +
            _keyStr.charAt(enc3) + _keyStr.charAt(enc4);
    }
    return output;
}
function utf8_encode(input) {
    input = input.replace(/\r\n/g, "\n");
    let utftext = "";
    for (let n = 0; n < input.length; n++) {
        let c = input.charCodeAt(n);
        if (c < 128) {
            utftext += String.fromCharCode(c);
        } else if ((c > 127) && (c < 2048)) {
            utftext += String.fromCharCode((c >> 6) | 192);
            utftext += String.fromCharCode((c & 63) | 128);
        } else {
            utftext += String.fromCharCode((c >> 12) | 224);
            utftext += String.fromCharCode(((c >> 6) & 63) | 128);
            utftext += String.fromCharCode((c & 63) | 128);
        }

    }
    return utftext;
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


    UploadChangeAth(uploadUrl, fk_frmAttachment);
}

/**
 * 图片附件上传
 */
function UploadChangeAth(uploadUrl, fk_frmAttachment) {
    if ($("#file_" + fk_frmAttachment).length == 0)
        return;
    var fileObj = $("#file_" + fk_frmAttachment).val();
    if (typeof (fileObj) == "undefined" || fileObj.size <= 0) {
        alert("请选择上传的文件.");
        return;
    }

    var file = document.getElementById("file_" + fk_frmAttachment).files[0];
    var fileSize = AthParams.AthInfo[fk_frmAttachment][0][2];
    if (file.size > fileSize * 1000) {
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
    var uploadUrl = "";
    if (plant == 'CCFlow')
        uploadUrl = basePath + '/WF/CCForm/Handler.ashx?AttachPK=' + fk_frmAttachment + '&DoType=MoreAttach&FK_Flow=' + pageData.FK_Flow + '&PKVal=' + athRefPKVal;
    else {
        uploadUrl = basePath + "/WF/Ath/AttachmentUploadS.do?FK_FrmAttachment=" + fk_frmAttachment + '&FK_Flow=' + pageData.FK_Flow + "&PKVal=" + athRefPKVal;
    }
    uploadUrl += "&WorkID=" + pageData.WorkID;
    uploadUrl += "&FID=" + pageData.FID;
    uploadUrl += "&FK_Node=" + pageData.FK_Node;
    uploadUrl += "&PWorkID=" + GetQueryString("PWorkID");
    uploadUrl += "&FK_MapData=" + AthParams.FK_MapData;



    //form表单序列话
    var parasData = $("#Form_" + fk_frmAttachment).serialize();
    //form表单序列化时调用了encodeURLComponent方法将数据编码了
    parasData = decodeURIComponent(parasData, true);
    parasData = decodeURIComponent(parasData, true);
    parasData = parasData.replace(/&/g, '@');
    parasData = parasData.replace(/TB_/g, '');
    parasData = parasData.replace(/RB_/g, '');
    parasData = parasData.replace(/CB_/g, '');
    parasData = parasData.replace(/DDL_/g, '');


    //提交数据
    var option = {
        url: uploadUrl + "&parasData=" + parasData,
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
                fn = cceval(mapAttr.UIBindKey);
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