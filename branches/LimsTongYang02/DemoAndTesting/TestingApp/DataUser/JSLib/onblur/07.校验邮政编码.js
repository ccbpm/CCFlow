function isPostalCode(s) {
    var patrn = /^[a-zA-Z0-9 ]{3,12}$/;
    if (!patrn.exec(s.value)) 
    {
       alert('非法的邮政编码格式.');
       return false;
    }
    return true
} 
