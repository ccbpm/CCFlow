
// 发送前执行数据安全检查.
function CheckBlank() {

    alert('sss');
  if (ReqTB('DianHua') == "") {
        alert( '电话不能为空:不能为空 \t\n');
     return false;
 }

 alert('ok');
    return true;  
}
