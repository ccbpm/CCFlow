/*****************����******************************
˵����
WebSign_AddSeal ���ӡ�½ӿ�
1.���ø����� (���Բ�����)
2.���ø���ʱ�� (���Դӷ������ϻ�ȡ,Ҳ���Բ�����)
3.���������ǵ�λ�� (������������ڱ����ƫ��λ��,��Ч��ֹӡ���ڲ�ͬ�ֱ����´�λ)
    
WebSign ֧������ӡ�»�ȡ��ʽ
1.����ӡ���ļ�
2.USBKey���ܿ�Կ����
3.Զ�̷�����
����ʾ���õ�һ�ַ�ʽ.
   	
���һ��������Ҫ�Ƕ��ӡ��,��ÿ���°󶨲�ͬ������,���Բ��տ����ĵ�
WebSign��AddSeal�ӿڿ������û��߻�ȡ��ǰ�µ�Name, 
�����µ�Name,��������ÿ���°󶨵�����,����֤����.
��ǰ����.����ϵͳĬ�Ϸ����Name,���е�ӡ�¶�Ĭ�ϰ����б�����.
***********************************************/
function WebSign_AddSeal(sealName, sealPostion, signData) {
    try {
        //�Ƿ��Ѿ�����
        var strObjectName;
        strObjectName = DWebSignSeal.FindSeal("", 0);


        while (strObjectName != "") {
            if (sealName == strObjectName) {
                alert("��ǰҳ���Ѿ��Ӹǹ�ӡ�£���" + sealName + "�����ʵ");
                return false;
            }
            strObjectName = DWebSignSeal.FindSeal(strObjectName, 0);

        }

        //���õ�ǰӡ�°󶨵ı���
        Enc_onclick(signData);

        //���ø����ˣ�������OA���û���
        document.all.DWebSignSeal.SetCurrUser("������");
        //����������·��
        //	document.all.DWebSignSeal.HttpAddress = "http://127.0.0.1:8089/inc/seal_interface/";
        //������Ψһҳ��ID ��SessionID
        //	document.all.DWebSignSeal.RemoteID = "0100018";
        //�����Ϳ��ԺܺõĹ̶�ӡ�µ�λ��
        document.all.DWebSignSeal.SetPosition(-10, 0, sealPostion);
        //���ø��µĽӿ�
        document.all.DWebSignSeal.AddSeal("", "test");
    } catch (e) {
        alert("�ؼ�û�а�װ����ˢ�±�ҳ�棬�ؼ����Զ����ء�\r\n�������ذ�װ����װ��" + e);
    }
}
function WebSign_AddSeal(sealName, sealPostion, signData, posX, posY) {
    try {
        //�Ƿ��Ѿ�����
        var strObjectName;
        strObjectName = DWebSignSeal.FindSeal("", 0);


        while (strObjectName != "") {
            if (sealName == strObjectName) {
                alert("��ǰҳ���Ѿ��Ӹǹ�ӡ�£���" + sealName + "�����ʵ");
                return false;
            }
            strObjectName = DWebSignSeal.FindSeal(strObjectName, 0);

        }

        //���õ�ǰӡ�°󶨵ı���
        Enc_onclick(signData);

        //���ø����ˣ�������OA���û���
        document.all.DWebSignSeal.SetCurrUser("������");
        //����������·��
        //	document.all.DWebSignSeal.HttpAddress = "http://127.0.0.1:8089/inc/seal_interface/";
        //������Ψһҳ��ID ��SessionID
        //	document.all.DWebSignSeal.RemoteID = "0100018";
        //�����Ϳ��ԺܺõĹ̶�ӡ�µ�λ��
        document.all.DWebSignSeal.SetPosition(posX, posY, sealPostion);
        //���ø��µĽӿ�
        document.all.DWebSignSeal.AddSeal("", "test");
    } catch (e) {
        alert("�ؼ�û�а�װ����ˢ�±�ҳ�棬�ؼ����Զ����ء�\r\n�������ذ�װ����װ��" + e);
    }
}
/*******************ȫ��Ļ��д****************************
˵����
WebSign_HandWrite���ӡ�½ӿ�
1.����ǩ���� (���Բ�����)
2.����ǩ��ʱ�� (���Դӷ������ϻ�ȡ,Ҳ���Բ�����) 
 
   	
���һ��������Ҫ�Ƕ��ǩ��,��ÿ��ǩ���󶨲�ͬ������,���Բ��տ����ĵ�
WebSign��HandWrite�ӿڿ������û��߻�ȡ��ǰǩ����Name, 
����ǩ����Name,��������ÿ��ǩ���󶨵�����,����֤����.
��ǰ����.����ϵͳĬ�Ϸ����Name,���е�ǩ����Ĭ�ϰ����б�����.
***********************************************/
function WebSign_HandWrite(sealName, sealPostion, signData) {
    try {
        //���õ�ǰӡ�°󶨵ı���
        Enc_onclick(signData);
        //����ǩ���ˣ�������OA���û���
        document.all.DWebSignSeal.SetCurrUser("ȫ����д��111");
        //����ǩ��ʱ�䣬�����з�����������
        //document.all.SetCurrTime("2006-02-07 11:11:11");
        //����ǩ���Ľӿ�
        document.all.DWebSignSeal.SetPosition(100, 10, sealPostion);
        if ("" == document.all.DWebSignSeal.HandWrite(0, 255, sealName)) {
            alert("ȫ��Ļǩ��ʧ��");
            return false;
        }
    } catch (e) {
        alert("�ؼ�û�а�װ����ˢ�±�ҳ�棬�ؼ����Զ����ء�\r\n�������ذ�װ����װ��" + e);
    }
}

/*******************����������д****************************
˵����
HandWritePop_onclick ����������д
1.����ǩ���� (���Բ�����)
2.����ǩ��ʱ�� (���Դӷ������ϻ�ȡ,Ҳ���Բ�����)
3.����ǩ�����ǵ�λ�� (������������ڱ����ƫ��λ��,��Ч��ֹǩ���ڲ�ͬ�ֱ����´�λ)
       	
���һ��������Ҫ�Ƕ��ǩ��,��ÿ��ǩ���󶨲�ͬ������,���Բ��տ����ĵ�
WebSign��HandWrite�ӿڿ������û��߻�ȡ��ǰǩ����Name, 
����ǩ����Name,��������ÿ��ǩ���󶨵�����,����֤����.
��ǰ����.����ϵͳĬ�Ϸ����Name,���е�ǩ����Ĭ�ϰ����б�����.
***********************************************/
function HandWritePop_onclick() {
    try {
        //���õ�ǰӡ�°󶨵ı���
        Enc_onclick();
        //����ǩ���ˣ�������OA���û���
        document.all.DWebSignSeal.SetCurrUser("������д��");
        //����ǩ��ʱ�䣬�����з�����������
        //document.all.SetCurrTime("2006-02-07 11:11:11");
        //���õ�ǰӡ�µ�λ��,�����sealPostion1 (<div id="handWritePostion1"> </div>) ��λ������ƫ��0px,����ƫ��0px
        //�����Ϳ��ԺܺõĹ̶�ӡ�µ�λ��
        document.all.DWebSignSeal.SetPosition(0, 0, "rPos");
        //����ǩ���Ľӿ�
        if ("" == document.all.DWebSignSeal.HandWritePop(0, 255, 200, 400, 300, "")) {
            alert("ȫ��Ļǩ��ʧ��");
            return false;
        }
    } catch (e) {
        alert("�ؼ�û�а�װ����ˢ�±�ҳ�棬�ؼ����Զ����ء�\r\n�������ذ�װ����װ��" + e);
    }
}

/*****************���ύ******************************
˵����
submit �ύ��
����WebSign��GetStoreData()�ӿڻ�ȡӡ�µ���������(ӡ������+֤������+ǩ������...)
�����ֵ��ֵ��Hiddle����,���浽���ݿ���.
***********************************************/

function submit_onclick() {
    try {
        var v = document.all.DWebSignSeal.GetStoreData();
        if (v.length < 200) {
            alert("�����ȸ��²ſ����ύ");
            return false;
        }
        document.all.form1.sealdata.value = v;
    } catch (e) {
        alert("�ؼ�û�а�װ����ˢ�±�ҳ�棬�ؼ����Զ����ء�\r\n�������ذ�װ����װ��" + e);
        return false;
    }
}


/***********************************************
˵����
Enc_onclick ��Ҫ���ð󶨵ı���
WebSign��SetSignData�ӿ�֧�����ְ����ݷ�ʽ��
1.�ַ�������
2.����
һ�����ݷ����ı䣬WebSign���Զ�У�飬����ʾ�޸ġ�
***********************************************/

function Enc_onclick(tex_name) {
    try {
        //���ԭ������	
        document.all.DWebSignSeal.SetSignData("-");
        // strΪ���󶨵��ַ�������
        //var str = "";
        //���ð󶨵ı���
        //���ĵ�λ
        document.all.DWebSignSeal.SetSignData("+LIST:laiwendanwei;");
        //��������
        document.all.DWebSignSeal.SetSignData("+LIST:laiwenDate;");
        //����
        document.all.DWebSignSeal.SetSignData("+LIST:shiyou;");
        //ʱ��Ҫ��
        document.all.DWebSignSeal.SetSignData("+LIST:time;");
        //���
        document.all.DWebSignSeal.SetSignData("+LIST:" + tex_name + ";");

        /*���ݱ��������Լ���֯������,��ǰ���ӽ�����������
        ������ַ�������,��Ҫ�����µ���
        document.all.DWebSignSeal.SetSignData("+DATA:"+str);		
        */
    } catch (e) {
        alert("�ؼ�û�а�װ����ˢ�±�ҳ�棬�ؼ����Զ����ء�\r\n�������ذ�װ����װ��" + e);
    }
}

/***********************************************
˵����
SetUI �����û�������
***********************************************/
function SetUI() {
    try {
        document.all.DWebSignSeal.TipBKLeftColor = 29087;
        document.all.DWebSignSeal.TipBKRightColor = 65443;
        document.all.DWebSignSeal.TipLineColor = 65535;
        document.all.DWebSignSeal.TipTitleColor = 32323;
        document.all.DWebSignSeal.TipTextColor = 323;
    } catch (e) {
        alert("�ؼ�û�а�װ����ˢ�±�ҳ�棬�ؼ����Զ����ء�\r\n�������ذ�װ����װ��" + e);
    }
}



function ShowSeal() {

    document.all.DWebSignSeal.ShowWebSeals();
}
function GetAllSeal() {
    var strSealName = document.all.DWebSignSeal.AddSeal("", "");
    alert(strSealName);
    strSealName = document.all.DWebSignSeal.FindSeal("", 0);
}

function addsealPostion(objectname) {
    /* vSealName:ӡ������
    vSealPostion:ӡ�°󶨵�λ��
    vSealSignData:ӡ�°󶨵�����
    */
    var vSealName = objectname;
    var vSealPostion = objectname + "sealpostion";
    var vSealSignData = objectname;
    WebSign_AddSeal(vSealName, vSealPostion, vSealSignData, 70, -30);
    Change('SealData');
    SetSealType(0);
}

function addseal(objectname) {
    /* vSealName:ӡ������
    vSealPostion:ӡ�°󶨵�λ��
    vSealSignData:ӡ�°󶨵�����
    */
    var vSealName = objectname;
    var vSealPostion = objectname + "sealpostion";
    var vSealSignData = objectname;
    WebSign_AddSeal(vSealName, vSealPostion, vSealSignData, -10, 0);
    Change('SealData');
    SetSealType(0);

}
function handwrite(objectname) {
    /* vSealName:ӡ������
    vSealPostion:ӡ�°󶨵�λ��
    vSealSignData:ӡ�°󶨵�����
    */
    var vSealName = objectname + "handwrite";
    var vSealPostion = objectname + "sealpostion";
    var vSealSignData = objectname;
    WebSign_HandWrite(vSealName, vSealPostion, vSealSignData);
}
function checkData() {
    try {
        var strObjectName;
        strObjectName = document.all.DWebSignSeal.FindSeal("", 0);
        while (strObjectName != "") {
            var v = document.all.DWebSignSeal.VerifyDoc(strObjectName);
            strObjectName = document.all.DWebSignSeal.FindSeal(strObjectName, 0);

            SetSealType(v);
        }
    } catch (e) {
        //alert("�ؼ�û�а�װ����ˢ�±�ҳ�棬�ؼ����Զ����ء�\r\n�������ذ�װ����װ��" +e);
    }
}
function GetValue_onclick() {
    var v = document.all.DWebSignSeal.GetStoreData();

    var valueData = document.getElementById('SealData').value;
    if (v == valueData) {
        alert("�����ȸ��²ſ����ύ");
        return false;
    }

    if (v.length == "") {
        alert("�����ȸ��²ſ����ύ");
        return false;
    }
    document.all.SealData.value = v;
}
function SetValue_onclick() {
    document.all.DWebSignSeal.SetStoreData(document.all.SealData.value);
    document.all.DWebSignSeal.ShowWebSeals();
}
function popshouxie_onClick() {
    //	document.all.DWebSignSeal.SetCurrUser("������");
    //	document.all.DWebSignSeal.HttpAddress = "127.0.0.1:1127";				
    //	alert(document.all.DWebSignSeal.Login());

    HandWritePop_onclick();
}

function GetBmpSeal() {
    var strObjectName;
    strObjectName = document.all.DWebSignSeal.FindSeal("", 0);
    var data;

    if (strObjectName != "") {
        try {
            data = document.all.DWebSignSeal.GetSealBmpString(strObjectName, "bmp");
        } catch (ex) {
            alert('��ȡͼƬ��Ϣʧ��:' + ex);
        }
    }
    return data;
}

function SaveSealToFile() {
    var strObjectName;
    strObjectName = document.all.DWebSignSeal.FindSeal("", 0);

    var vPath = GetFilePath();
    if (strObjectName != "") {
        try {
            document.all.DWebSignSeal.GetSealBmpToFile(strObjectName, "jpg", vPath);
            //     var a = document.all.DWebSignSeal.GetSealBmpString(strObjectName, "bmp");

        } catch (e) {
            alert("ͼƬ����ʧ��:" + e);
        }

    }
}