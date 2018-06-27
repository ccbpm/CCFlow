<%@ Control Language="c#" Inherits="CCFlow.WF.UC.UCEn" CodeBehind="UCEn.ascx.cs" %>
<script language="javascript" type="text/javascript">
    var kvs = null;
    function GenerPageKVs() {
        var ddls = null;
        ddls = parent.document.getElementsByTagName("select");
        kvs = "";
        for (var i = 0; i < ddls.length; i++) {
            var id = ddls[i].name;

            if (id.indexOf('DDL_') == -1) {
                continue;
            }
            var myid = id.substring(id.indexOf('DDL_') + 4);
            kvs += '~' + myid + '=' + ddls[i].value;
        }

        ddls = document.getElementsByTagName("select");
        for (var i = 0; i < ddls.length; i++) {
            var id = ddls[i].name;

            if (id.indexOf('DDL_') == -1) {
                continue;
            }
            var myid = id.substring(id.indexOf('DDL_') + 4);
            kvs += '~' + myid + '=' + ddls[i].value;
        }
        return kvs;
    }
    function UploadChange(btn) {
        $('[id$=' + btn + ']').click(); //qin 15-5-26
        //document.getElementById(btn).click();
    }
    function HidShowSta() {
        if (document.getElementById('RptTable').style.display == "none") {
            document.getElementById('RptTable').style.display = "block";
            document.getElementById('ImgUpDown').src = "../Img/arrow_down.gif";
        }
        else {
            document.getElementById('ImgUpDown').src = "../Img/arrow_up.gif";
            document.getElementById('RptTable').style.display = "none";
        }
    }
    function GroupBarClick(rowIdx) {
        var alt = document.getElementById('Img' + rowIdx).alert;
        var sta = 'block';
        if (alt == 'Max') {
            sta = 'block';
            alt = 'Min';
        } else {
            sta = 'none';
            alt = 'Max';
        }
        document.getElementById('Img' + rowIdx).src = '../Img/' + alt + '.gif';
        document.getElementById('Img' + rowIdx).alert = alt;
        var i = 0
        for (i = 0; i <= 40; i++) {
            if (document.getElementById(rowIdx + '_' + i) == null)
                continue;
            if (sta == 'block') {
                document.getElementById(rowIdx + '_' + i).style.display = '';
            } else {
                document.getElementById(rowIdx + '_' + i).style.display = sta;
            }
        }
    }
    function NoSubmit(ev) {
        if (window.event.srcElement.tagName == "TEXTAREA")
            return true;

        if (ev.keyCode == 13) {
            window.event.keyCode = 9;
            // alert(' code=: ' + ev.keyCode + ' tagName:' + window.event.srcElement.tagName);
            ev.keyCode = 9;
            // alert('ok');
            //alert(ev.keyCode);
            return true;
            //                event.keyCode = 9;
            //                ev.keyCode = 9;
            //                window.event.keyCode = 9;
        }
        return true;
    }
 
</script>
