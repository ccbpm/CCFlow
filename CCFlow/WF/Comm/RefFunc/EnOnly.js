//多附件显示
function ShowMultiFile(AttrFiles, FileManagers, isExitMyNum) {
    var html = "";

    //如果多附件没有分类
    if (AttrFiles.length == 0) {
        html += "<div class='layui-row FoolFrmFieldRow'>";
        html += "<div class='layui-col-md2 layui-col-xs2 FoolFrmFieldLabel'><label class='layui-form-label'>多附件上传</label></div>";
       
        html += "<div class='layui-col-md10 layui-col-xs10 FoolFrmFieldInput'>";
        if (isExitMyNum == true) {
            html += "<div class='FoolFrmFieldLabel'>";
            
            if (mapData.GetPara("IsDelete") == 1 || mapData.GetPara("IsUpdate") == 1 || mapData.GetPara("IsInsert") == 1)
                html += "<a href='javaScript:void(0)' style='color:#fff;' class='layui-btn layui-btn-xs' onclick='ActiveUploadFile(\"\",\"\")'>上传附件</a>";

            html += "</div>"
        }
        html += "<table class='layui-table' width='99%' id='TableFile'>";
        for (var k = 0; k < FileManagers.length; k++) {
            var sf = FileManagers[k];
            //显示附件
            html += "<tr>";
            html += "<td width='*'>";
            html += "<img src='../../Img/FileType/" + sf.MyFileExt.substr(1) + ".gif' border=0 />" + sf.MyFileName + sf.MyFileExt;
            html += "</td>";
            html += "<td width='25%'>";
            html += "<a href='javaScript:void(0)' onclick='downLoadFileM(\"" + sf.OID + "\")'>下载</a>&nbsp;&nbsp;&nbsp;&nbsp;";
            if (mapData.GetPara("IsDelete") == 1 || mapData.GetPara("IsUpdate") == 1 || mapData.GetPara("IsInsert") == 1 )
                html += "<a href='javaScript:void(0)' onclick='deleteFile(\"" + sf.OID + "\",this)'>删除</a>";
            html += "</td>";
            html += "</tr>";
            html += "";
        }
        html += "</table>";
        if (count == 0)
            html += "没有上传附件";
        html += "</div>";
        
        html += "</div>";
        return html;
    }
    //多附件分类
    for (var i = 0; i < AttrFiles.length; i++) {
        var attrFile = AttrFiles[i];
        html += "<div class='layui-row FoolFrmFieldRow'>";
        html += "<div class='layui-col-md2 layui-col-xs2 FoolFrmFieldLabel'><label class='layui-form-label'>多附件上传</label></div>";

        html += "<div class='layui-col-md10 layui-col-xs10 FoolFrmFieldInput'>";
        if (isExitMyNum == true) {
            html += "<div class='FoolFrmFieldLabel'>";
            if (mapData.GetPara("IsDelete") == 1 || mapData.GetPara("IsUpdate") == 1 || mapData.GetPara("IsInsert") == 1)
                html += "<a href='javaScript:void(0)' style='color:#fff;' class='layui-btn layui-btn-xs' onclick='ActiveUploadFile(\"" + attrFile.FileNo + "\",\"" + attrFile.FileName + "\")'>上传附件</a>";
            html += "</div>"
        }

        var count = 0;
        html += "<table class='layui-table' width='99%' id='Table_" + attrFile.FileNo + "'>";
        for (var k = 0; k < FileManagers.length; k++) {
            var sf = FileManagers[k];
            if (sf.AttrFileNo != attrFile.FileNo)
                continue;
            //显示附件
            html += "<tr>";
            html += "<td width='*'>";
            html += "<img src='../../Img/FileType/" + sf.MyFileExt.substr(1) + ".gif' border=0 />" + sf.MyFileName + sf.MyFileExt;
            html += "</td>";
            html += "<td width='25%'>";
            html += "<a href='javaScript:void(0)' onclick='downLoadFile(\"" + sf.OID + "\")'>下载</a>&nbsp;&nbsp;&nbsp;&nbsp;";
            if (mapData.GetPara("IsDelete") == 1 || mapData.GetPara("IsUpdate") == 1 || mapData.GetPara("IsInsert") == 1)
                html += "<a href='javaScript:void(0)' onclick='deleteFile(\"" + sf.OID + "\",this)'>删除</a>";
            html += "</td>";
            html += "</tr>";
            html += "";
            count++;

        }
        html += "</table>";
        if (count == 0)
            html += "没有上传附件";
        html += "</div>";
        html += "</div>";
    }
    return html;
}
var fileNo = "";
var fileName = "";
function ActiveUploadFile(upfileNo, upfileName) {
    if (pkVal == null) {
        var flag = Update(false);
        if (flag == false)
            return;
    }
       
    fileNo = upfileNo;
    fileName = upfileName;
    //激活上传文件
    $("#File_Upload").click();
}

function downLoadFileM(OID) {
    if (plant == "CCFlow")
        SetHref('../../CCForm/DownFile.aspx?DoType=EntityMutliFile_Load&OID=' + OID);
    else {
        var currentPath = GetHrefUrl();
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        Url = path + '/WF/CCForm/EntityMutliFile_Load.do?OID=' + OID;
        SetHref(url);
    }
}

function deleteFile(OID, td) {
    if (window.confirm('您确定要删除吗?') == false)
        return;

    //需要删除文件
    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CommEntity");
    handler.AddPara("OID", OID);
    var data = handler.DoMethodReturnString("EntityMultiFile_Delete");
    if (data.indexOf("err@") != -1) {
        alert(data);
        return;
    }
    //处理显示问题，删除一行
    $(td).parent().parent().remove();

}

//新增一行
function tableAddTr(fileManager) {
    var tb;
    if (fileManager.AttrFileNo == "")
        tb = $("#TableFile");
    else
        tb = $("#Table_" + fileManager.AttrFileNo);
    var html = "";
    html += "<tr>";
    html += "<td>";
    html += "<img src='../../Img/FileType/" + fileManager.MyFileExt.substr(1) + ".gif' border=0 />" + fileManager.MyFileName + fileManager.MyFileExt;
    html += "</td>";
    html += "<td>";
    html += "<a href='javaScript:void(0)' onclick='downLoadFileM(\"" + fileManager.OID + "\")'>下载</a>&nbsp;&nbsp;&nbsp;&nbsp;";
    if (mapData.GetPara("IsDelete") == 1)
        html += "<a href='javaScript:void(0)' onclick='deleteFile(\"" + fileManager.OID + "\",this)'>删除</a>";
    html += "</td>";
    html += "</tr>";
    tb.append(html);
}


//多文件上传
function MultiUploadFile() {

    var fileObj = document.getElementById("File_Upload").files[0]; // js 获取文件对象
    if (fileObj == null || (fileObj) == "undefined" || fileObj.size <= 0) {
        return;
    }

    //获取文件的后缀名
    if (fileObj.name.indexOf(".") == -1) {
        alert("上传的文件名没有扩展名，请检查后上传");
        return;
    }


    //form表单序列话
    var formData = new FormData();
    var name = $("input").val();
    formData.append("file", fileObj);
    formData.append("name", name);
    //获取发送请求时的参数
    var queryString = document.location.search.substr(1);

    queryString = replaceParamVal(queryString, "PKVal", pkVal);

    queryString += "&FileNo=" + fileNo + "&FileName=" + encodeURI(fileName);

    //URL 路径
    var URL = basePath + "/WF/Comm/Handler.ashx?DoType=EntityMultiAth_Upload";
    if (plant != "CCFlow") {
        var currentPath = GetHrefUrl();
        var path = currentPath.substring(0, currentPath.indexOf('/WF') + 1);
        URL = path + "/WF/Comm/ProcessRequest.do?DoType=EntityMultiAth_Upload";
    }

    if (queryString != null && queryString.length > 0)
        URL += "&" + queryString;


    $.ajax({
        url: URL,
        type: 'POST',
        data: formData,
        async: false,
        xhrFields: {
            withCredentials: true
        },
        crossDomain: true,
        // 告诉jQuery不要去处理发送的数据
        processData: false,
        // 告诉jQuery不要去设置Content-Type请求头
        contentType: false,
        beforeSend: function () {
            console.log("正在进行，请稍候");
        },
        success: function (responseStr) {
            if (responseStr.indexOf('err@') == 0)
                return;
            responseStr = JSON.parse(responseStr);
            tableAddTr(responseStr);
        },
        error: function (responseStr) {
            console.log("error");
        }
    });

}

//树形结构
function findChildren(jsonArray, parentNo) {
    var appendToTree = function (treeToAppend, o) {
        $.each(treeToAppend, function (i, child) {
            if (o.id == child.ParentNo)
                o.children.push({
                    "id": child.No,
                    "text": child.Name,
                    "children": []
                });
        });

        $.each(o.children, function (i, o) {
            appendToTree(jsonArray, o);
        });

    };

    var jsonTree = [];
    var jsonchildTree = [];
    if (jsonArray.length > 0 && typeof parentNo !== "undefined") {
        $.each(jsonArray, function (i, o) {
            if (o.ParentNo == parentNo) {
                jsonchildTree.push(o);
                jsonTree.push({
                    "id": o.No,
                    "text": o.Name,
                    "children": []
                });
            }
        });

        $.each(jsonTree, function (i, o) {
            appendToTree(jsonArray, o);
        });

    }

    function _(treeArray) {
        $.each(treeArray, function (i, o) {
            if ($.isArray(o.children)) {
                if (o.children.length == 0) {
                    o.children = undefined;
                } else {
                    _(o.children);
                }
            }
        });
    }
    _(jsonTree);
    return jsonTree;
}