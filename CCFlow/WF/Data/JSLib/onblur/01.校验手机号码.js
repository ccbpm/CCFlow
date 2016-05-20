function isMobil(c) {
    var patrn = /^[+]{0,1}(\d){1,3}[ ]?([-]?((\d)|[ ]){1,12})+$/;
    if (c.value == '')
        return;
    if (!patrn.exec(c.value)) {
        c.focus();
        c.select();
        alert('非法的手机号码.');
        return false;
    }
    return true
}