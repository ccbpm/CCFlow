function isTrueName(s) {
    var patrn = /^[a-zA-Z]{1,30}$/;
    if (!patrn.exec(s.value))
    {
       alert('非法的用户名格式.');
       return false;
    }
    return true
} 