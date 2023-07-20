
function SS() {

    var en = new Entity("BP.Sys.GroupField");
    en.FrmID = "dict_cc";
    en.Lab = "xxxx";
    en.Insert();


  //  http://localhost:2296/WF/Comm/Handler.ashx?DoType=Entity_Delete&EnName=&PKVal=Frm_GuanYongCeShi_RiQiZiDuan

    //var en = new Entity("BP.Sys.GroupField");
    //en.Lab = "新建分组zzz";
    //en.FrmID = "xxx";
    //en.Idx = 1000;
    //en.Insert();
    //var oid = en.OID;

    //var en = new Entity("BP.Sys.FrmUI.MapAttrDT", 'Frm_GuanYongCeShi_RiQiZiDuan');
    //en.Delete();

    //var en = new Entity("BP.Sys.MapAttr", 'Frm_GuanYongCeShi_RiQiZiDuan');
    //var idx = en.Idx;

    //en.Delete();


    //var en = new Entity("BP.Sys.FrmUI.MapAttrDT", 'Frm_GuanYongCeShi_RiQiZiDuan');
    //en.Delete();

    //var en = new Entity("BP.Sys.FrmUI.MapAttrString", 'Frm_GuanYongCeShi_RiQiZiDuan');
    //en.Delete();



    // 创建当前表单的实例.
    var workID = GetQueryString('WorkID'); //获得传入的实体ID.
    var frmID = GetQueryString('FrmID'); //获得传入的frmID

    // 获得主表信息.
    var en = new Entity(frmID, workID); //创建实例, 当前的en就是一个该实体的json.
    //获得从表配件信息.
    var enDtls = new Entities(frmID + "Dtl1");
    enDtls.Retrieve("RefPK", workID);

    var JiaoFuRiQi = '2021-01-01'; //交付日期.
    var KaiGongShuLiang = 2; //开工数量.

    //产品实例.
    var chanPinEntity = new Entity("Dict_CPSL");
    var chanPinEntityDtl = new Entity("Dict_CPSLDtl1");

    //遍历数据生成订单信息.
    for (var i = 0; i < KaiGongShuLiang; i++) {

        //生成主表实例. 
        chanPinEntity.CopyJSON(en);
        chanPinEntity.OID = 0;
        chanPinEntity.JiaoFuRiQi = JiaoFuRiQi;
        chanPinEntity.Insert(); //插入产品实力主表.

        //生成从表实例.
        for (var idx = 0; idx < enDtls.length; idx++) {

            var enDtl = enDtls[idx];

            //生成步骤从表..
            chanPinEntityDtl.CopyJSON(enDtl);
            chanPinEntityDtl.OID = 0;
            chanPinEntityDtl.RefPK = chanPinEntity.OID;
            chanPinEntityDtl.Insert(); //插入产品实力 dtl.
        }
    }

    alert("执行完成.");
}


$(document).ready(function () {

    var html = "<table style='width:100%;'>";
    html += "<tr>";
    html += "<td> <img src='Title.png' style='width:400px;' /></td>";
    html += "<td>";

    html += "<a href ='http://ccflow.org' target = '_blank' >驰骋官网</a >";
    html += "| <a href='Default.htm' >常见问题-主页-登录</a>";
    html += "| <a href='/Helper/Default.htm' >帮助中心</a>";

    //  html += "| <a href='Login.html'  target='_blank' >登录-注册-企业</a>";
    // html += "| <a href='/App/Portal/Login.htm' target='_blank'>终端客户登录</a>";
    html += "</td>";
    html += "</tr>";
    html += "</table>";

    var toobar = $("#toolbar");
    if (toobar != null)
        $("#toolbar").html(html);


    var frmEndHtml = "<table style='width:400px;'>";
    frmEndHtml += "<tr>";
    frmEndHtml += "<td><a href='http://ccflow.org' target=_blank>驰骋官网</a></td>";
    frmEndHtml += "<td><a href='http://edu.ccflow.org' target=_blank>流程学院</a></td>";
    frmEndHtml += "<td><a href='http://ccflow.ke.qq.com' target=_blank>视频教程 </a></td>";
    frmEndHtml += "<td><a href='http://doc.ccbpm.cn' target=_blank>操作手册 </a></td>";
    frmEndHtml += "<td><a href='http://ccflow.org/down.htm' target=_blank >源代码下载 </a></td>";
    frmEndHtml += "</tr>";
    //frmEndHtml += "<tr>";
    //frmEndHtml += "</tr>";

    frmEndHtml += "</table>";

    var frmEnd = $("#FrmEnd");
    if (frmEnd != null)
        $("#FrmEnd").html(frmEndHtml);

    html = "<center><ul>";
    //html += "<li>商务<br><img src='http://app.ccflow.org/WeiXinZhuMengJuan.jpg' /></li>";
    html += "<li><img src='http://ccflow.org/WeiXin/WeiXinBiger.jpg' style='width:300px;' /><br/>驰骋BPM公众号 </li>";

    html += "</ul></center>";
    $("#WeiXin").html(html);

})
