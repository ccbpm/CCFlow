var code;
function createCode() {
    code = "";
    var codeLength = 5; //验证码的长度
    var checkCode = document.getElementById("checkCode");

    var codeChars = new Array(2, 3, 4, 5, 6, 7, 8, 9,
        'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j', 'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z',
        'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'); //所有候选组成验证码的字符，当然也可以用中文的
    for (var i = 0; i < codeLength; i++) {
        var charNum = Math.floor(Math.random() * 58);
        code += codeChars[charNum];
    }
    if (checkCode) {
        checkCode.className = "code";
        checkCode.innerHTML = code;
    }


    var canvas = document.getElementById('canvas');
    context = canvas.getContext('2d');
    context.clearRect(0, 0, canvas.width, canvas.height);
    context.font = '16px sans-serif';
    context.fillStyle = '#4B4B4B';
    context.fillText(code, 12, 18);
}

function validateCode() {

    var inputCode = $("#inputCode").val();
    if (inputCode.length <= 0) {
        alert('请输入验证码');
        return false;
    }
    else if (inputCode.toUpperCase() != code.toUpperCase()) {
        alert('验证码错误，请重新输入');
        $("#inputCode").val("");
        $("#TB_PW").val("");
        createCode();
        return false;
    }
    else {
        return true;
    }
}
