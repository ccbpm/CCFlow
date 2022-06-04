//状态 1=备注状态 ， 0=无状态. 1=批阅状态.
var repremarkSta = 0;
var frmDBRemarks = null; //填过意见
var marks = null;
var remarkData = {};
var frmID = "";
var rFrmID = "";
var tip_index = 0;
var curDocument = window.document;
var isTree = false;
var isReadonly = GetQueryString("IsReadonly");

//数据批阅
function FrmDBRemark(remarkEnable) {
    $(".frmDBVer", curDocument).remove();
    var pkVal = GetQueryString("WorkID");
    pkVal = pkVal == null || pkVal == undefined || pkVal == "" ? GetQueryString("OID") : pkVal;

    if (typeof dbVerSta != "undefined")
        dbVerSta = 0;

    //根据节点属性获取当前节点性质
    if (wf_node.FormType == 5) {
        if (repremarkSta == 1 && isTree == true && frmID != vm.frmGenerNo) {
            repremarkSta = 0;
            FrmDBRemark_UnDo();
        }
        debugger
        //获取当前打开的页面FrmID
        frmID = vm.frmGenerNo;
        rFrmID = frmID;
        var iframe = vm.$refs['iframe-' + vm.activeItem];
        if (iframe != null && iframe != undefined) {
            iframe = iframe[0];
            curDocument = iframe.contentWindow.document;
        }
        isTree = true;
    }

    //傻瓜表单、开发者表单、绑定表单库的表单
    if (wf_node.FormType == 0 || wf_node.FormType == 12 || wf_node.FormType == 11) {
        //直接比对当前的数据和历史数据
        frmID = wf_node.NodeFrmID;
        rFrmID = frmID;
        if (frmID == null || frmID == undefined || frmID == "") {
            frmID = "ND" + wf_node.NodeID;
            rFrmID = "ND" + parseInt(wf_node.FK_Flow) + "Rpt";
        }
        if ($("#toIframe").length != 0) {
            var iframe = $("#toIframe");
            curDocument = iframe[0].contentWindow.document;
        }
    }
    var mapData = new Entity("BP.Sys.MapData", frmID);
    //如果是章节表单
    if (mapData.FrmType == 10) {
        layer.alert(mapData.Name + "是章节表单,数据审阅请点击每个章节的【审阅&版本】功能");
        return;
    }
    FrmDBRemark_Init(rFrmID, frmID, pkVal, remarkEnable);

}



/**
 * 初始化有批注的字段.
 * 根据是否有批注，就在控件上加批注标识.
 * */
function FrmDBRemark_Init(frmID,mapDataNo, pkval, remarkEnable) {

    remarkData.FrmID = frmID;
    remarkData.PKVal = pkval;
    remarkData.mapDataNo = mapDataNo;
    remarkData.RemarkEnable = remarkEnable;
    
    //如果是.
    if (repremarkSta == 1) {
        FrmDBRemark_UnDo();
        return;
    }
    var style = "position: absolute;bottom: -1px;";
    //获得有批注的字段.
    frmDBRemarks = new Entities("BP.Sys.FrmDBRemarks");
    frmDBRemarks.Retrieve("FrmID", frmID, "RefPKVal", pkval, "RDT");
    frmDBRemarks = frmDBRemarks.TurnToArry();
    //  frmDBRemarks.reverse();
    //根据字段进行分组
    marks = GetFrmDBRemarkByFieldGroup(frmDBRemarks);

    //给字段增加审阅标签.
    for (key in marks) {
        var element = $("#TB_" + key, curDocument);
        if (element.length == 0)
            element = $("#DDL_" + key, curDocument);
        if (element.length == 0)
            element = $("#CB_" + key, curDocument);
        if (element.length == 0) {
            element = $("input[name='CB_" + key + "']", curDocument);
            if (element.length != 0) {
                element = $("#DIV_" + key, curDocument);
                if (element.length == 0)
                    element = $("#SC_" + key, curDocument);
            }
        }
           
        if (element.length == 0) {
            element = $("input[name='RB_" + key + "']", curDocument);
            if (element.length != 0) {
                element = $("#DIV_" + key, curDocument);
                if (element.length == 0)
                    element = $("#SR_" + key, curDocument);
            }
        }
           
        if (element.length == 0)  // 元素不存在
            continue;

        //增加标签内容
        var w = element[0].offsetWidth - 15;
        var name = "";
        var style = "";
        if ($("#Lab_" + key, curDocument).length != 0) {
            name = $("#Lab_" + key, curDocument)[0].innerText;
            style = "top:-13px;";
        } else {
            name = element.attr("data-name");
            var h = element.height();
            h = h + 13;
            style = "margin-top:-" + h+"px;"
        }
           
       
            
        var remarkElemnt = $("<div class='remark'data-info='" + key + "' data-name='" + name + "'style='position: absolute;margin-left:" + w + "px;"+style+"'><i class='layui-icon' style='color:red;font-weight:bold'>&#x1005;</i>   </div>");
        element.after(remarkElemnt);
        remarkElemnt.bind('click', function () {
            var field = $(this).attr("data-info");
            var name = $(this).attr("data-name");
            FrmDBRemark_Show(field, name);
        })
    }
    //如果不是只读，需要把可编辑的字段增加审阅标签
    if (isReadonly != "1" && remarkData.RemarkEnable == 1) {
        var mapAttrs = new Entities("BP.Sys.MapAttrs");
        mapAttrs.Retrieve("FK_MapData", mapDataNo);
        $.each(mapAttrs, function (i, attr) {
            if (attr.UIVisible == 0)
                return true;
            if (marks[attr.KeyOfEn] != undefined && marks[attr.KeyOfEn].length != 0)
                return true;

            //增加标签内容
            var key = attr.KeyOfEn;
            var NameValue = attr.Name;
            
            element = $("#DIV_" + attr.KeyOfEn, curDocument);
            if (element.length == 0) {//开发表单
                var element = $("#TB_" + attr.KeyOfEn, curDocument);
                if (element.length == 0)
                    element = $("#DDL_" + attr.KeyOfEn, curDocument);
                if (element.length == 0)
                    element = $("#CB_" + attr.KeyOfEn, curDocument);
                if (element.length == 0) {
                    element = $("input[name='CB_" + attr.KeyOfEn + "']", curDocument);
                    if (element.length != 0) {
                         element = $("#SC_" + attr.KeyOfEn, curDocument);
                    }
                }

                if (element.length == 0) {
                    element = $("input[name='RB_" + attr.KeyOfEn + "']", curDocument);
                    if (element.length != 0) {
                         element = $("#SR_" + attr.KeyOfEn, curDocument);
                    }
                }
                
                element.parent().css('position', 'relative');

            } else {//傻瓜
                element.css('position', 'relative');
            }

            
            if (true) {
                //if (attr.UIContralType == 0 && attr.AtPara && attr.AtPara.indexOf("@IsRichText=1") >= 0) {
                // element = $("#TD_" + attr.KeyOfEn, curDocument);
                // var tips = $(".tips", curDocument);

                var tipsonclick = $("#tips_" + key, curDocument);//已经包含
                if (tipsonclick.length > 0) {
                    var tips = $(".tips", curDocument);//隐藏批注功能
                    if (element[0] && element[0].disabled) {//不可编辑状态
                        element.parent().on('mouseover', function (event) {
                            var tips = $(".tips", curDocument);
                            tips.hide();
                            var node = $(this);
                            node.siblings('.tips').show();
                        })
                    } else {
                        element.on('mouseover', function (event) {
                            var tips = $(".tips", curDocument);
                            tips.hide();
                            var node = $(this);
                            node.siblings('.tips').show();

                        })
                    }
                    
                   
                } else {
                    var content = `<div  id='tips_${key}' class="tips" style='padding: 0px;
                                    border: 1px solid #c2b483;
                                    background - color: #c2b483;
                                    overflow: hidden;
                                    z-index: 99999;
                                    position: absolute;
                                    width: 100px;
                                    height:30px;
                                    line-height:30px;
                                    cursor: pointer;
                                    text-align: center;
                                    left: 0px;
                                    top: 39px;
                                    background: #ddcd;
                                    color: #333;display:none'><a >审阅</a></div>`;

                    element.parent().append(content);

                    if (element[0] && element[0].disabled) {//不可编辑状态
                        element.wrap("<div></div>");
                        element.parent().on('mouseenter', function (event) {
                            var tips = $(".tips", curDocument);
                            tips.hide();
                            var node = $(this);
                            node.siblings('.tips').show();
                        })
                    } else {//可编辑
                        element.on('mouseenter', function (event) {
                            var tips = $(".tips", curDocument);
                            tips.hide();
                            var node = $(this);
                            // var content = "<div style='padding:5px 0px;width:50px;text-align:center;font-size:15px;'><a onclick=FrmDBRemark_Show('" + key + "','" + NameValue + "');>审阅</a></div>";
                            // var id = this.id;
                            //tip_index = layui.layer.tips(content, element , { tips: [4, '#FFF6D9'], time: 10000, area: ['auto', 'auto'] });
                            node.siblings('.tips').show();
                            // node.siblings('.tips').onclick = FrmDBRemark_Show(key, NameValue);

                        })
                    }

                    var tipsonclick = $("#tips_" + key, curDocument);
                    if (tipsonclick.length > 0) {
                        tipsonclick[0].onclick = function () {
                            FrmDBRemark_Show(key, NameValue);
                        }
                    }
                }

                return true;
            }

        })
    }

    //设置为批注状态.
    repremarkSta = 1;
}


/**
 * 撤销批注状态
 * */
function FrmDBRemark_UnDo() {
    $(".remark", curDocument).remove();
    $(".tips", curDocument).remove();
    $(".tips", curDocument).hide();
    offShenYue();
    //批注状态.
    repremarkSta = 0;
}
function offShenYue() {
    var mapAttrs = new Entities("BP.Sys.MapAttrs");
    mapAttrs.Retrieve("FK_MapData", remarkData.mapDataNo );
    $.each(mapAttrs, function (i, attr) {
        if (attr.UIVisible == 0)
            return true;
        if (marks[attr.KeyOfEn] != undefined && marks[attr.KeyOfEn].length != 0)
            return true;

        //增加标签内容
        var key = attr.KeyOfEn;
        var NameValue = attr.Name;

        element = $("#DIV_" + attr.KeyOfEn, curDocument);
        if (element.length == 0) {//开发表单
            var element = $("#TB_" + attr.KeyOfEn, curDocument);
            if (element.length == 0)
                element = $("#DDL_" + attr.KeyOfEn, curDocument);
            if (element.length == 0)
                element = $("#CB_" + attr.KeyOfEn, curDocument);
            if (element.length == 0) {
                element = $("input[name='CB_" + attr.KeyOfEn + "']", curDocument);
                if (element.length != 0) {
                    element = $("#SC_" + attr.KeyOfEn, curDocument);
                }
            }

            if (element.length == 0) {
                element = $("input[name='RB_" + attr.KeyOfEn + "']", curDocument);
                if (element.length != 0) {
                    element = $("#SR_" + attr.KeyOfEn, curDocument);
                }
            }
        } 
        

        if (element[0] && element[0].disabled) {//不可编辑状态
            element.parent().off('mouseenter');

        } else {
            element.off('mouseenter');
        }



    })
}

/**
 * 显示批阅信息（弹窗显示）
 * @param {字段} field
 */
function FrmDBRemark_Show(field, name) {

    //数据版本
    var isEnable = remarkData.RemarkEnable == 1 && (isReadonly == null || isReadonly==undefined ||isReadonly =="0")?1:0;

    remarkData.Field = field;
    remarkData.Name = name;
    var url = "./CCForm/FrmDBVerAndRemark.htm?Field=" + field + "&FieldType=0&FrmID=" + remarkData.mapDataNo + "&RFrmID=" + remarkData.FrmID + "&RefPKVal=" + remarkData.PKVal + "&IsEnable=" + isEnable + "&PageType=1";
    if (GetHrefUrl().indexOf("CCForm/") != -1)
        url = "./FrmDBVerAndRemark.htm?Field=" + field + "&FieldType=0&FrmID=" + remarkData.mapDataNo + "&RFrmID=" + remarkData.FrmID + "&RefPKVal=" + remarkData.PKVal + "&IsEnable=" + isEnable + "&PageType=1";
    layer.open({
        type: 2,
        id: 'frmDBRemark',
        title: remarkData.Name + '数据批阅',
        content: url,
        offset: 'r',
        area: ['500px', '100%']
    });

}



/**
 * 保存
 * @param {表单ID} frmID
 * @param {字段ID} field
 * @param {主键} pkval
 * @param {批阅信息} remark
 */
function FrmDBRemark_Save() {
    var remark = $("#FrmDBRemark_Doc").val();
    if (remark == "") {
        layer.alert("审阅信息不能为空");
        return;

    }
    var en = new Entity("BP.Sys.FrmDBRemark");
    en.FrmID = remarkData.FrmID; //表单ID.
    en.Field = remarkData.Field; //字段。
    en.Remark = remark; //批阅意见.
    en.RefPKVal = remarkData.PKVal; //主键字段.
    en.Insert(); //插入数据.
    if (!marks[remarkData.Field])
        marks[remarkData.Field] = [en];
    else
        marks[remarkData.Field].unshift(en);
 
    var _html = GetHtml(remarkData.Field, remarkData.Name);
    $("#frmDBRemark").html(_html);
}

/**
 * 删除
 * @param {主键} mypk
 */
function FrmDBRemark_Delete(mypk) {
    var en = new Entity("BP.Sys.FrmDBRemark", mypk);
    en.Delete();
    var arry = marks[remarkData.Field];
    $.each(arry, function (i, item) {
        if (item.MyPK == mypk) {
            marks[remarkData.Field].splice(i, 1);
            return false;
        }
    })
    var _html = GetHtml(remarkData.Field, remarkData.Name);
    $("#frmDBRemark").html(_html);
}

/**
 * 根据字段获取字段分组的审阅信息
 * @param {any} frmDBRemarks
 */
function GetFrmDBRemarkByFieldGroup(frmDBRemarks) {
    var map = {};
    var keyOfEn = "";
    //对mapExt进行分组，根据AttrOfOper
    $.each(frmDBRemarks, function (i, frmDBRemark) {
        //不是操作字段不解析
        keyOfEn = frmDBRemark.Field;
        if (!map[keyOfEn])
            map[keyOfEn] = [frmDBRemark];
        else
            map[keyOfEn].push(frmDBRemark);
    });
    var res = [];
    Object.keys(map).forEach(key => {
        res.push({
            attrKey: key,
            data: map[key],
        })
    });
    return map;
}


