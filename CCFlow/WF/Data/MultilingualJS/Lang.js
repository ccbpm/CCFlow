
//获得语言.
function Tran(dVal, key, p1, p2) {
    if (CurrentLang == "CN" || CurrentLang == undefined)
        return dVal;

}

function Tran_Public(dVal, key, p1, p2) {

    if (CurrentLang == "CN" || CurrentLang == undefined)
        return dVal;

    var json = GetPublicJSON();
    for (var i = 0; i < json.length; i++) {

    }
    return dVal;
}