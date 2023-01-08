//多附件显示
function ShowMultiFile(AttrFiles, FileManagers, isExitMyNum) {
    var html = "";

    //如果多附件没有分类
    if (AttrFiles.length == 0) {
        html += "<div class='layui-row FoolFrmFieldRow'>";
        html += "<div class='layui-col-md2 layui-col-xs2 FoolFrmFieldLabel'><label class='layui-form-label'>附件</label></div>";

        html += "<div class='layui-col-md10 layui-col-xs10 FoolFrmFieldInput'>";
        if (isExitMyNum == true) {
            html += "<div class='FoolFrmFieldLabel'>";

            if (isReadonly != "1" && (mapData.GetPara("IsDelete") == 1 || mapData.GetPara("IsUpdate") == 1 || mapData.GetPara("IsInsert") == 1))
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
            html += "<td width='25%'>" + sf.RDT + "</td>";
            html += "<td width='25%'>";
            html += "<a href='javaScript:void(0)' onclick='downLoadFileM(\"" + sf.OID + "\")'>下载</a>&nbsp;&nbsp;&nbsp;&nbsp;";
            if (isReadonly != "1" && (mapData.GetPara("IsDelete") == 1 || mapData.GetPara("IsUpdate") == 1 || mapData.GetPara("IsInsert") == 1))
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
        html += "<div class='layui-col-md2 layui-col-xs2 FoolFrmFieldLabel'><label class='layui-form-label'>附件</label></div>";

        html += "<div class='layui-col-md10 layui-col-xs10 FoolFrmFieldInput'>";
        if (isExitMyNum == true) {
            html += "<div class='FoolFrmFieldLabel'>";
            if (isReadonly != "1" && (mapData.GetPara("IsDelete") == 1 || mapData.GetPara("IsUpdate") == 1 || mapData.GetPara("IsInsert") == 1))
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
    $("#File_Upload").val("");
    //激活上传文件
    $("#File_Upload").click();
}

function downLoadFileM(OID) {
    if (plant == "CCFlow")
        SetHref('../../CCForm/DownFile.aspx?DoType=EntityMutliFile_Load&OID=' + OID);
    else {
        SetHref(basePath + '/WF/Ath/EntityMutliFile_Load.do?OID=' + OID);
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
    window.location.reload();

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
    html += "<td>" + fileManager.RDT + "</td>";
    html += "<td>";
    html += "<a href='javaScript:void(0)' onclick='downLoadFileM(\"" + fileManager.OID + "\")'>下载</a>&nbsp;&nbsp;&nbsp;&nbsp;";
    if (mapData.GetPara("IsDelete") == 1 || mapData.GetPara("IsUpdate") == 1 || mapData.GetPara("IsInsert") == 1)
        html += "<a href='javaScript:void(0)' onclick='deleteFile(\"" + fileManager.OID + "\",this)'>删除</a>";
    html += "</td>";
    html += "</tr>";
    tb.append(html);
}


//多文件上传
function MultiUploadFile() {

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_CommEntity");
    handler.AddFileData();
    handler.AddPara("FileNo", fileNo);
    handler.AddPara("FileName", fileName);
    handler.AddPara("EnName", GetQueryString("EnName"));
    handler.AddPara("PKVal", pkVal);
    var data = handler.DoMethodReturnString("EntityMultiAth_Upload");
    if (data.indexOf("err@") != -1) {
        alert(data);
        return;
    }
    data = JSON.parse(data);
    tableAddTr(data);

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