/*
说明:
1. 该文件被嵌入到 /WF/WorkOpt/WorkCheck.htm 里面去，与WorkCheck.js 一起工作.
2. 为了适合不同的电子签名的需要,集成不同的电子签名厂家.
3. 如果不需要电子签名该文件保留为空.
4, WorkCheck.htm文件引用了jquery 在这里可以使用jQuery 的函数.
*/


function InitSignature() {

    iSP.init(
                  {
                      //fixed: false,
                      isGet: true,
                      //currentKeysn: '0020000001', //正式环境应为this.EmpFrom0101000005
                      crossDomain: 'http://192.168.100.82:8080/iSignature-Phone/', //如果不是跨域,iSignature-Phone请设置项目路径http://127.0.0.1:8080/iSM_V3/
                      documentId: workid, //必须指定文档ID
                      moveable: true //是否可以移动签章
                  }
             );
}

//作用：进行签章
function DoSignaturePhone(empNo, nodeID, EleID) {

    //alert(empNo + ',' + nodeID +','+EleID);

    if (CheckHaveSing(empNo) == true) {
        alert('您已经盖过章，请撤销重盖.');
        return;
    }

    var txt = $("#WorkCheck_Doc").val();
    if (txt == "" || txt == null || txt == undefined) {
        alert('请输入审核意见.');
        return;
    }

    var tbID = nodeID + '_' + empNo;
    document.getElementById(tbID).value = txt;


    var divID = "div_" + tbID;
    var user = EleID;


    //签章必须的参数
    var runSignatureParams = {
        keySN: user, //签章服务的keysn,获取key文件名称,不包括后缀名,所有key文件存放在WEB-INF/key文件
        documentId: workid, //文档ID
        elemId: divID, //指定定位的页面元素的id,不仅限div元素,所有html元素都可以.
        enableMove: true
        //,DivList:"yfdiv"//添加显示改签章在其他位置，多个位置用“;”分割
    };

    var params = {
        callback: function (data) {
            if (data.error) {
                alert('签章失败');
            }
            else {
                //执行数据库保存.
                SaveWorkCheck();
                // alert('签章成功');
            }
        },
        backGetPwd: false, //是否后台获取印章密码,跨域不支持

        protectedData: [//定义保护数据,fieldName用于指定元素的id或者name获取该元素的值,元素为input或textarea时,获取该元素的value属性,其他元素调用innerHTML属性.
         { fieldDesc: "电子签章", fieldName: tbID }    //已经设置了保护项描述信息,不必添加desc属性.

		               ],
        runSignatureParams: runSignatureParams//运行签章的参数

    };
    //显示选择印章列表div窗口
    iSP.showGetSignatureByKey(params);


}


function CheckHaveSing(empNo) {
    //  var sg = DocForm.SignatureControl.GetSignatureInfo(empNo);
    var objs = document.getElementsByName("iHtmlSignature");  //获得页面内签章个数
    for (var i = 0; i < objs.length; i++) {
        var item = objs[i];

        if (item.UserName == empNo || item.UserName == '0020000001') {                         //将已存在的签章KEYNAME与当前业务系统用户进行比对
            return true;
        }
    }
    return false;
}

/**
*验证签章
*/
function checkSignature(isAlert) {
    var checkSignatureParams = {
        callback: function (data) {
            console.log(data);
        },
        "signatures": iSP.signatures//获取签章数据,指定验证那些印章
    };
    iSP.checkSignature(checkSignatureParams); //执行验证签章操作
}

/**
*获取印章信息
*/
function getSignInfo() {

    var _signature = [];
    _signature.push(iSP.signatures[iSP.signatures.length - 1]); //获取最后一个签章,放在数组里面
    // alert(_signature.documentId); //json转化成字符串
    var signInfoParam = {

        "signatures": _signature //获取签章数据,指定获取那些印章,
        // , "callback": function (data) {   }



        //获取签章回调方法,该参数不是必须,客户可以通过设置该属性和方法,接受验证结果,做自己的业务流程.
    };
    iSP.getSignInfo(signInfoParam); //获取指定的印章的信息.

}


/**
* 删除签章
*/
function removeSignature(documentId, userNo) {


    // getSignInfo();
    //return;

    var _signature = [];
    _signature.push(iSP.signatures[iSP.signatures.length - 1]); //获取最后一个签章,放在数组里面
    //alert(jsonToString(_signature));//json转化成字符串
    var removeSignatureParam = {
        "signatures": _signature //获取签章数据,指定获取那些印章,
        //,"callback":testFunction//删除签章回调方法,该参数不是必须,客户可以通过设置该属性和方法,接受验证结果,做自己的业务流程.
    };
    iSP.removeSignature(removeSignatureParam);
}