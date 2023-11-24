
/**
* �����С������󳤶ȡ�
* ����:str  �ַ���
*  ����:true �� flase; true��ʾ��ʽ��ȷ
*/
function checkLength(tb, minLen, maxLen) {
    if (tb.value.length < minLen || tb.value.length > maxLen) {
        alert('��������ĳ��ȱ�����' + minLen + ' �� ' + maxLen + '֮��.');
    }
}

/* �Ƿ������� */
function isEmail(tb) {
    if (tb.search(/^\w+((-\w+)|(\.\w+))*\@[A-Za-z0-9]+((\.|-)[A-Za-z0-9]+)*\.[A-Za-z0-9]+$/) != -1)
        return true;
    alert('������ʼ���ʽ.');
}

/**
* ���������ֻ������ʽ�Ƿ���ȷ
* ����:str  �ַ���
* ����:true �� flase; true��ʾ��ʽ��ȷ
*/
function checkMobilePhone(str) {
    if (str.match(/^(?:13\d|15[89])-?\d{5}(\d{3}|\*{3})$/) == null) {
        return false;
    }
    else {
        return true;
    }
}

/**
* �������Ĺ̶��绰�����Ƿ���ȷ
* ����:str  �ַ���
* ����:true �� flase; true��ʾ��ʽ��ȷ
*/
function checkTelephone(str) {
    if (str.match(/^(([0\+]\d{2,3}-)?(0\d{2,3})-)(\d{7,8})(-(\d{3,}))?$/) == null) {
        return false;
    }
    else {
        return true;
    }
}


/**
* �����������֤���Ƿ���ȷ
* ����:str  �ַ���
*  ����:true �� flase; true��ʾ��ʽ��ȷ
*/
function checkCard(str) {
    //15λ�����֤������ʽ
    var arg1 = /^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$/;
    //18λ�����֤������ʽ
    var arg2 = /^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])((\d{4})|\d{3}[A-Z])$/;
    if (str.match(arg1) == null && str.match(arg2) == null) {
        return false;
    }
    else {
        return true;
    }
}




/**************************** δ���� **************************************/

/**
* 2009-10-01
* ��  ��
* ��  Ե
* js���ֱ�������֤
*/
/**************************************************************************************/
/*************************************���ֵ���֤*****************************************/
/**************************************************************************************/
/**
* ��������һ���ַ��Ƿ�ȫ��������
* ����:str  �ַ���
* ����:true �� flase; true��ʾΪ����
*/
function checkNum(str) {
    return str.match(/\D/) == null;
}


/**
* ��������һ���ַ��Ƿ�ΪС��
* ����:str  �ַ���
* ����:true �� flase; true��ʾΪС��
*/
function checkDecimal(str) {
    if (str.match(/^-?\d+(\.\d+)?$/g) == null) {
        return false;
    }
    else {
        return true;
    }
}

/**************************************************************************************/
/*************************************�ַ�����֤*****************************************/
/**************************************************************************************/


/**
* ��������һ���ַ��Ƿ����ַ�
* ����:str  �ַ���
* ����:true �� flase; true��ʾΪȫ��Ϊ�ַ� ����������
*/
function checkStr(str) {
    if (/[^\x00-\xff]/g.test(str)) {
        return false;
    }
    else {
        return true;
    }
}


/**
* ��������һ���ַ��Ƿ��������
* ����:str  �ַ���
* ����:true �� flase; true��ʾ��������
*/
function checkChinese(str) {
    if (escape(str).indexOf("%u") != -1) {
        return true;
    }
    else {
        return false;
    }
}


/**
* �������������ʽ�Ƿ���ȷ
* ����:str  �ַ���
* ����:true �� flase; true��ʾ��ʽ��ȷ
*/
function checkEmail(str) {
    if (str.match(/[A-Za-z0-9_-]+[@](\S*)(net|com|cn|org|cc|tv|[0-9]{1,3})(\S*)/g) == null) {
        return false;
    }
    else {
        return true;
    }
}





/**
* ���QQ�ĸ�ʽ�Ƿ���ȷ
* ����:str  �ַ���
*  ����:true �� flase; true��ʾ��ʽ��ȷ
*/
function checkQQ(str) {
    if (str.match(/^\d{5,10}$/) == null) {
        return false;
    }
    else {
        return true;
    }
}


/**
* ��������IP��ַ�Ƿ���ȷ
* ����:str  �ַ���
*  ����:true �� flase; true��ʾ��ʽ��ȷ
*/
function checkIP(str) {
    var arg = /^(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])\.(\d{1,2}|1\d\d|2[0-4]\d|25[0-5])$/;
    if (str.match(arg) == null) {
        return false;
    }
    else {
        return true;
    }
}

/**
* ��������URL��ַ�Ƿ���ȷ
* ����:str  �ַ���
*  ����:true �� flase; true��ʾ��ʽ��ȷ
*/
function checkURL(str) {
    if (str.match(/(http[s]?|ftp):\/\/[^\/\.]+?\..+\w$/i) == null) {
        return false
    }
    else {
        return true;
    }
}


/**************************************************************************************/
/*************************************ʱ�����֤*****************************************/
/**************************************************************************************/

/**
* ������ڸ�ʽ�Ƿ���ȷ
* ����:str  �ַ���
* ����:true �� flase; true��ʾ��ʽ��ȷ
* ע�⣺�˴�������֤�������ڸ�ʽ
* ��֤�����ڣ�2007-06-05��
*/
function checkDate(str) {
    //var value=str.match(/((^((1[8-9]\d{2})|([2-9]\d{3}))(-)(10|12|0?[13578])(-)(3[01]|[12][0-9]|0?[1-9])$)|(^((1[8-9]\d{2})|([2-9]\d{3}))(-)(11|0?[469])(-)(30|[12][0-9]|0?[1-9])$)|(^((1[8-9]\d{2})|([2-9]\d{3}))(-)(0?2)(-)(2[0-8]|1[0-9]|0?[1-9])$)|(^([2468][048]00)(-)(0?2)(-)(29)$)|(^([3579][26]00)(-)(0?2)(-)(29)$)|(^([1][89][0][48])(-)(0?2)(-)(29)$)|(^([2-9][0-9][0][48])(-)(0?2)(-)(29)$)|(^([1][89][2468][048])(-)(0?2)(-)(29)$)|(^([2-9][0-9][2468][048])(-)(0?2)(-)(29)$)|(^([1][89][13579][26])(-)(0?2)(-)(29)$)|(^([2-9][0-9][13579][26])(-)(0?2)(-)(29)$))/);
    var value = str.match(/^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2})$/);
    if (value == null) {
        return false;
    }
    else {
        var date = new Date(value[1], value[3] - 1, value[4]);
        return (date.getFullYear() == value[1] && (date.getMonth() + 1) == value[3] && date.getDate() == value[4]);
    }
}

/**
* ���ʱ���ʽ�Ƿ���ȷ
* ����:str  �ַ���
* ����:true �� flase; true��ʾ��ʽ��ȷ
* ��֤ʱ��(10:57:10)
*/
function checkTime(str) {
    var value = str.match(/^(\d{1,2})(:)?(\d{1,2})\2(\d{1,2})$/)
    if (value == null) {
        return false;
    }
    else {
        if (value[1] > 24 || value[3] > 60 || value[4] > 60) {
            return false
        }
        else {
            return true;
        }
    }
}

/**
* ���ȫ����ʱ���ʽ�Ƿ���ȷ
* ����:str  �ַ���
* ����:true �� flase; true��ʾ��ʽ��ȷ
* (2007-06-05 10:57:10)
*/
function checkFullTime(str) {
    //var value = str.match(/^(\d{1,4})(-|\/)(\d{1,2})\2(\d{1,2}) (\d{1,2}):(\d{1,2}):(\d{1,2})$/);
    var value = str.match(/^(?:19|20)[0-9][0-9]-(?:(?:0[1-9])|(?:1[0-2]))-(?:(?:[0-2][1-9])|(?:[1-3][0-1])) (?:(?:[0-2][0-3])|(?:[0-1][0-9])):[0-5][0-9]:[0-5][0-9]$/);
    if (value == null) {
        return false;
    }
    else {
        //var date = new Date(checkFullTime[1], checkFullTime[3] - 1, checkFullTime[4], checkFullTime[5], checkFullTime[6], checkFullTime[7]);
        //return (date.getFullYear() == value[1] && (date.getMonth() + 1) == value[3] && date.getDate() == value[4] && date.getHours() == value[5] && date.getMinutes() == value[6] && date.getSeconds() == value[7]);
        return true;
    }

}

/**************************************************************************************/
/************************************���֤�������֤*************************************/
/**************************************************************************************/

/**  
* ���֤15λ�������dddddd yymmdd xx p
* dddddd��������
* yymmdd: ����������
* xx: ˳������룬�޷�ȷ��
* p: �Ա�����Ϊ�У�ż��ΪŮ
* <p />
* ���֤18λ�������dddddd yyyymmdd xxx y
* dddddd��������
* yyyymmdd: ����������
* xxx:˳������룬�޷�ȷ��������Ϊ�У�ż��ΪŮ
* y: У���룬��λ��ֵ��ͨ��ǰ17λ������
* <p />
* 18λ�����Ȩ����Ϊ(���ҵ���) Wi = [ 7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2,1 ]
* ��֤λ Y = [ 1, 0, 10, 9, 8, 7, 6, 5, 4, 3, 2 ]
* У��λ���㹫ʽ��Y_P = mod( ��(Ai��Wi),11 )
* iΪ���֤��������������� 2...18 λ; Y_PΪ��ѾУ��������У��������λ��
*
*/
var Wi = [7, 9, 10, 5, 8, 4, 2, 1, 6, 3, 7, 9, 10, 5, 8, 4, 2, 1]; // ��Ȩ����   
var ValideCode = [1, 0, 10, 9, 8, 7, 6, 5, 4, 3, 2]; // ���֤��֤λֵ.10����X   
function IdCardValidate(idCard) {
    idCard = trim(idCard.replace(/ /g, ""));
    if (idCard.length == 15) {
        return isValidityBrithBy15IdCard(idCard);
    }
    else
        if (idCard.length == 18) {
            var a_idCard = idCard.split(""); // �õ����֤����   
            if (isValidityBrithBy18IdCard(idCard) && isTrueValidateCodeBy18IdCard(a_idCard)) {
                return true;
            }
            else {
                return false;
            }
        }
        else {
            return false;
        }
}

/**  
* �ж����֤����Ϊ18λʱ������֤λ�Ƿ���ȷ
* @param a_idCard ���֤��������
* @return
*/
function isTrueValidateCodeBy18IdCard(a_idCard) {
    var sum = 0; // ������Ȩ��ͱ���   
    if (a_idCard[17].toLowerCase() == 'x') {
        a_idCard[17] = 10; // �����λΪx����֤���滻Ϊ10�����������   
    }
    for (var i = 0; i < 17; i++) {
        sum += Wi[i] * a_idCard[i]; // ��Ȩ���   
    }
    valCodePosition = sum % 11; // �õ���֤����λ��   
    if (a_idCard[17] == ValideCode[valCodePosition]) {
        return true;
    }
    else {
        return false;
    }
}

/**  
* ͨ�����֤�ж�������Ů
* @param idCard 15/18λ���֤����
* @return 'female'-Ů��'male'-��
*/
function maleOrFemalByIdCard(idCard) {
    idCard = trim(idCard.replace(/ /g, "")); // �����֤���������������ַ����пո�   
    if (idCard.length == 15) {
        if (idCard.substring(14, 15) % 2 == 0) {
            return 'female';
        }
        else {
            return 'male';
        }
    }
    else
        if (idCard.length == 18) {
            if (idCard.substring(14, 17) % 2 == 0) {
                return 'female';
            }
            else {
                return 'male';
            }
        }
        else {
            return null;
        }
}

/**  
* ��֤18λ�����֤�����е������Ƿ�����Ч����
* @param idCard 18λ�����֤�ַ���
* @return
*/
function isValidityBrithBy18IdCard(idCard18) {
    var year = idCard18.substring(6, 10);
    var month = idCard18.substring(10, 12);
    var day = idCard18.substring(12, 14);
    var temp_date = new Date(year, parseFloat(month) - 1, parseFloat(day));
    // ������getFullYear()��ȡ��ݣ�����ǧ�������   
    if (temp_date.getFullYear() != parseFloat(year) ||
    temp_date.getMonth() != parseFloat(month) - 1 ||
    temp_date.getDate() != parseFloat(day)) {
        return false;
    }
    else {
        return true;
    }
}

/**  
* ��֤15λ�����֤�����е������Ƿ�����Ч����
* @param idCard15 15λ�����֤�ַ���
* @return
*/
function isValidityBrithBy15IdCard(idCard15) {
    var year = idCard15.substring(6, 8);
    var month = idCard15.substring(8, 10);
    var day = idCard15.substring(10, 12);
    var temp_date = new Date(year, parseFloat(month) - 1, parseFloat(day));
    // ���������֤�е����������迼��ǧ��������ʹ��getYear()����   
    if (temp_date.getYear() != parseFloat(year) ||
    temp_date.getMonth() != parseFloat(month) - 1 ||
    temp_date.getDate() != parseFloat(day)) {
        return false;
    }
    else {
        return true;
    }
}

//ȥ���ַ���ͷβ�ո�   
function trim(str) {
    return str.replace(/(^\s*)|(\s*$)/g, "");
}
