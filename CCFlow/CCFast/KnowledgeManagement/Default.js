
//执行保存.
function SaveKM() {

    var en = new Entity("BP.CCOA.KnowledgeManagement.Knowledge");

    en.Name = $("TB_Name").val();
    en.ImgUrl = $("TB_ImgUrl").val();
    en.Docs = $("TB_Docs").val();
    en.Insert();

    SetHref("Knowledge.htm?No=" + en.No);
}

//获得数据源, 
function Default_Init() {

    var ens = new Entities("BP.CCOA.KnowledgeManagement.Knowledges");
    var data = ens.DoMethodReturnJSON("Default_Init");
}

/**
 * 执行关注
 * @param {any} myno
 */
function DoFuncsOn( myno)
{
    var webUser = new WebUser();
    var en = new Entity("BP.CCOA.KnowledgeManagement.Knowledge", myno);
    en.Foucs = en.Foucs + "," + webUser.No + ",";
    en.Update();
}

/**
 * 取消关注
 * @param {any} myno
 */
function DoFuncsOn(myno) {
    var webUser = new WebUser();
    var en = new Entity("BP.CCOA.KnowledgeManagement.Knowledge", myno);

    //替换掉 webUser.No; 
   // en.Foucs = en.Foucs.replace(," + webUser.No + ",");
    en.Update();
}