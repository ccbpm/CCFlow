//状态 1=备注状态 ， 0=无状态. 1=批阅状态.
var repremarkSta = 0;
var frmDBRemarks = null; //填过意见
var marks = null;
var remarkData = {};

var tip_index = 0;
/**
 * 初始化有批注的字段.
 * 根据是否有批注，就在控件上加批注标识.
 * */
function FrmDBRemark_Init(frmID, pkval, remarkEnable) {

    remarkData.FrmID = frmID;
    remarkData.PKVal = pkval;
    remarkData.RemarkEnable = remarkEnable;
    //如果是.
    if (repremarkSta == 1) {
        FrmDBRemark_UnDo();
        return;
    }
    var style = "position: absolute;bottom: -1px;";
    //获得有批注的字段.
    frmDBRemarks = new Entities("BP.Sys.FrmDBRemarks");
    frmDBRemarks.Retrieve("FrmID", frmID, "RefPKVal", pkval, "RDT DESC ");
    frmDBRemarks = frmDBRemarks.TurnToArry();
  //  frmDBRemarks.reverse();
    debugger;
    //根据字段进行分组
    marks = GetFrmDBRemarkByFieldGroup(frmDBRemarks);

    //给字段增加审阅标签.
    for (key in marks) {
        var element = $("#TB_" + key);
        if (element.length == 0)
            element = $("#DDL_" + key);
        if (element.length == 0)
            element = $("#CB_" + key);
        if (element.length == 0)
            element = $("input[name='CB_" + key + "']");
        if (element.length == 0)
            element = $("input[name='RB_" + key + "']");
        if (element.length == 0)  // 元素不存在
            continue;

        //增加标签内容
        var w = element[0].offsetWidth - 15;
        var name = $("#Lab_" + key)[0].innerText;
        var remarkElemnt = $("<div class='mark'data-info='" + key + "' data-name='" + name + "'style='position: absolute;top:-13px;margin-left:" + w + "px'><i class='layui-icon' style='color:red;font-weight:bold'>&#xe61f;</i>   </div>");
        element.after(remarkElemnt);
        remarkElemnt.bind('click', function () {
            var field = $(this).attr("data-info");
            var name = $(this).attr("data-name");
            FrmDBRemark_Show(field, name);
        })
    }

    //如果不是只读，需要把可编辑的字段增加审阅标签
    if (pageData.IsReadonly != "1" && remarkData.RemarkEnable==1) {
        var mapAttrs = frmMapAttrs;
        $.each(mapAttrs, function (i, attr) {
            if (attr.UIVisible == 0 )
                return true;
            if (marks[attr.KeyOfEn] != undefined && marks[attr.KeyOfEn].length != 0)
                return true;

             //增加标签内容
            var element = $("#TB_" + attr.KeyOfEn);
            if (attr.UIContralType == 1)
                element = $("#DDL_" + attr.KeyOfEn);
            if (attr.UIContralType == 2)
                element = $("#CB_" + attr.KeyOfEn);
            if (attr.UIContralType == 3)
                element = $("#RB_" + attr.KeyOfEn);

            if ( true ) {
            //if (attr.UIContralType == 0 && attr.AtPara && attr.AtPara.indexOf("@IsRichText=1") >= 0) {
                element = $("#TD_" + attr.KeyOfEn);

                var key = attr.KeyOfEn;
                var NameValue = attr.Name;
                
                element.on('mouseenter', function (event) {
                    var node = $(this);
                    var conn ='<div style="padding:5px 0px;margin:0px;text-align:center;font-size:15px;background-color:#139ff0;border-radius:5px 5px 0px 0px;box-shadow: 1px 1px 3px rgba(0,0,0,.2);font-weight:bold;">温 馨 提 示</div><div style="padding:5px 28px;"><span style="color:red;">* </span><span style="color:#000000">规则1 请输入正整数</span><br/><span style="color:red;">* </span><span style="color:#000000">规则2 请输入正整数</span></div>'
                    var content = "<div style='padding:5px 0px;width:50px;text-align:center;font-size:15px;'><a onclick=FrmDBRemark_Show('" + key + "','" + NameValue +"');>审阅</a></div>";
/*                    layui.layer.tips(content, element, {
                        tips: 1
                    });*/
                    tip_index = layui.layer.tips(content, element, { tips: [4, '#FFF6D9'], time: 10000, area: ['auto', 'auto']});
                   // $(".layui-layer-tips i.layui-layer-TipsB").css("top", "0px");
                    //$(".layui - layer layui - layer - tips").css("top", top);
                   /* layer.tips(content, element, {
                        tips: [1, ' #FFFFFF'],
                        time: 10000,
                        skin: 'custom' //自定义class名称
                    });
                    
*/
                    /*var tip_index = 0;
                    $(document).on('mouseenter', '.layer_hover', function () {
                        var words = $(this).data('words');
                        tip_index = layui.layer.tips(words, '.layer_hover', { time: 0 });
                    }).on('mouseleave', '.layer_hover', function () {
                        layui.layer.close(tip_index);
                    });*/

                   // event.stopPropagation()
                    
                })

               /* $("#TD_" + attr.KeyOfEn).on('blur', function () {
                    if (tip_index != 0) {
                        layui.layer.close(tip_index);
                    }

                })*/
                
               
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
    $(".mark").remove();
    //批注状态.
    repremarkSta = 0;
}


/**
 * 显示批阅信息（弹窗显示）
 * @param {字段} field
 */
function FrmDBRemark_Show(field,name) {
    remarkData.Field = field;
    remarkData.Name = name;

    layui.use('layer', function () {
        var layer = layui.layer;
        /*layer.open({
            type: 1,
            id:'frmDBRemark',
            title: remarkData.Name+'数据批阅',
            content: GetHtml( field, name),
            offset: 'r',
            area: ['500px', '100%']
        });*/
        layer.tab({
            id: 'frmDBRemark',
            area: ['500px', '100%'],
            offset: 'r',
            tab: [{
                title: remarkData.Name + '数据批阅',
                content: GetHtml(field, name),
            }, {
                title: '数据版本',
                content: '数据版本2'
            }]
        });
    });
}


function GetHtml( field, name) {
    var _html = "";
    if (remarkData.RemarkEnable == 1) {
        _html += "<div class='layui-card'style='border-radius:10px' id='FrmDBRemark_Div'>";
        _html += "<div class='layui-card-body'>";
        _html += "<div class='layui-form-item'>";
        _html += "<div class=''>";
        _html += "<textarea id='FrmDBRemark_Doc'style='outline:none;transition:1s;border:1px solid #DDDDDD;box-shadow;2px 2px 4px #f5f5f5 inset;padding:10px' placeholder='请输入内容' class='layui-textarea' ></textarea >";
        _html += "</div >";
        _html += "</div >";
        _html += "<div style='text-align:right'>"
        _html += "<button type='button' class='layui-btn layui-btn-sm' style='width:80px' onclick='FrmDBRemark_Save()'>保存</button>";
        _html += "</div>";
        _html += "</div>";
        _html += "</div>";
    }
   
    var items = marks[field];
    if (items && items.length > 0) {
        /*$.each(items, function (i, item) {
            _html += "<div class='layui-card' style='border-radius:10px'>";
            _html += "<div class='layui-card-body'>";
            _html += "<div class=''>";
            _html += "<span style='font-weight:bold;margin-right: 20px;'>" + item.RecName + "</span>";
            _html += "<span >" + item.RDT + "</span>";
            _html += "</div > ";
            _html += "<div style='margin-left: 20px;line-height: 34px;'>";
            _html += item.Remark;
            _html += "</div>";
            _html += "<div style='text-align:right'>"
            _html += "<button type='button' class='layui-btn  layui-btn-sm' onclick='FrmDBRemark_Delete(\"" + item.MyPK + "\")'><i class='layui-icon'>&#x1006;</i>删除</button>";
            _html += "</div>";
            _html += "</div>";
            _html += "</div>";
        })*/
        _html += '<ul class="layui-timeline">';
        $.each(items, function (i, item) {
            _html += '<li class="layui-timeline-item">';
            _html += '<i class="layui-icon layui-timeline-axis">&#xe63f;</i>';
            _html += '<div class="layui-timeline-content layui-text">';
            _html += '<h3 class="layui-timeline-title">';
            _html += item.RDT+" - "+item.RecName;
            _html += '</h3>';
            _html += "<p>";
            _html += item.Remark;
            _html += "<div style='text-align:right'>"
            _html += "<button type='button' class='layui-btn  layui-btn-sm' onclick='FrmDBRemark_Delete(\"" + item.MyPK + "\")'><i class='layui-icon'>&#x1006;</i>删除</button>";
            _html += "</div>";
            _html += "</p>";
            _html += "</div>";
            
            _html += "</li>";

        })
        _html += '</ul>';

    }
    
    return _html;
}
/**
 * 保存
 * @param {表单ID} frmID
 * @param {字段ID} field
 * @param {主键} pkval
 * @param {批阅信息} remark
 */
function FrmDBRemark_Save() {
    debugger
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
    var _html = "";
/*    _html += "<div class='layui-card' style='border-radius:10px'>";
    _html += "<div class='layui-card-body'>";
    _html += "<div class=''>";
    _html += "<span style='font-weight:bold;margin-right: 20px;'>" + en.RecName + "</span>";
    _html += "<span >" + en.RDT + "</span>";
    _html += "</div > ";
    _html += "<div style='margin-left: 20px;line-height: 34px;'>";
    _html += en.Remark;
    _html += "</div>";
    _html += "<div style='text-align:right'>"
    _html += "<button type='button' class='layui-btn  layui-btn-sm' onclick='FrmDBRemark_Delete(\"" + en.MyPK + "\")'><i class='layui-icon'>&#x1006;</i>删除</button>";
    _html += "</div>";
    _html += "</div>";
    _html += "</div>";*/
    _html += '<li class="layui-timeline-item">';
    _html += '<i class="layui-icon layui-timeline-axis">&#xe63f;</i>';
    _html += '<div class="layui-timeline-content layui-text">';
    _html += '<h3 class="layui-timeline-title">';
    _html += en.RDT + " - " + en.RecName;
    _html += '</h3>';
    _html += "<p>";
    _html += en.Remark;
    _html += "<div style='text-align:right'>"
    _html += "<button type='button' class='layui-btn  layui-btn-sm' onclick='FrmDBRemark_Delete(\"" + en.MyPK + "\")'><i class='layui-icon'>&#x1006;</i>删除</button>";
    _html += "</div>";
    _html += "</p>";
    _html += "</div>";

    _html += "</li>";
    $("#FrmDBRemark_Div").after(_html);
    $("#FrmDBRemark_Doc").val("");
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
    var _html = GetHtml( remarkData.Field, remarkData.Name);
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


