
// 发送前执行数据安全检查.
function CheckBlank() {
     
    var msg = "";
    if (ReqAthFileName('GaoJian') == null) {
        msg += '您没有上传文档附件 \t\n';
    }
    if (ReqTB('BianXiaoRen') == "") {
        msg += '编校人:不能为空 \t\n';
    }

    if (ReqTB('BianXiaoRenDianHua') == "") {
        msg += '编校人电话:不能为空 \t\n';
    }

    if (ReqTB('QianFaRen') == "") {
        msg += '签发人:不能为空 \t\n';
    }

    if (ReqTB('QianFaRenDianHua') == "") {
        msg += '签发人电话:不能为空 \t\n';
    }
     
    if (ReqTB('WenZhangBiaoTi') == "") {
        msg += '文章标题:不能为空 \t\n';
    }
    if (msg == "")
        return true; /*可以提交.*/
    alert(msg);
    return false; /*不能提交.*/
}
