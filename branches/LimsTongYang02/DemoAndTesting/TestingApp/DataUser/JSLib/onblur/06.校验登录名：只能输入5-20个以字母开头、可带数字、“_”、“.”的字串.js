function isRegisterUserName(s) {
    var patrn = /^[a-zA-Z]{1}([a-zA-Z0-9]|[._]){4,19}$/;
    if (!patrn.exec(s.value))
   {
       alert('非法的用户名格式.');
       return false;
    }
    return true
}