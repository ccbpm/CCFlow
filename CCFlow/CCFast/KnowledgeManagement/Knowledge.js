
//执行保存.
function SaveKM() {

}

//获得数据源, 
function Default_Init() {

    var no = GetQueryString("No");

    //获得树结构.
    var trees = new Entities("BP.CCOA.KnowledgeManagement.KMTrees");
    trees.Retrieve("KnowledgeNo", no, "Idx");
    // trees = trees.TurnToArry();

    //获得从表结构.
    var dtls = new Entities("BP.CCOA.KnowledgeManagement.KMDtls");
    dtls.Retrieve("KnowledgeNo", no, "Idx");
    // dtls = dtls.TurnToArry();
}

/**
 * 执行关注
 * @param {any} myno
 */
function DoFuncsOn(myno) {
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