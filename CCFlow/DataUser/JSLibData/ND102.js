
function isPasswd(s) {
    var patrn = /^(\w){6,20}$/;
    if (!patrn.exec(s.value)) 
    {
       alert('非法的密码格式.');
       return false;
    }
    return true
}