<%@ Control Language="C#" AutoEventWireup="true" Inherits="CCFlow.WF.UC.ToolWap"
    CodeBehind="ToolsWap.ascx.cs" %>
<script src="../Comm/JS/Calendar/WdatePicker.js" type="text/javascript"></script>
<script type="text/javascript" src="/WF/Comm/JS/Calendar/jquery-1.3.1.min.js"></script>
<script type="text/javascript" src="/WF/Comm/JS/Calendar/jquery.bitmapcutter.js"></script>
<link rel="Stylesheet" type="text/css" href="/WF/Comm/CSS/jquery.bitmapcutter.css" />
<script type="text/javascript">
    function SetSelected(cb, ids) {
        //alert(ids);
        var arrmp = ids.split(',');
        var arrObj = document.all;
        var isCheck = false;
        if (cb.checked)
            isCheck = true;
        else
            isCheck = false;
        for (var i = 0; i < arrObj.length; i++) {
            if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                for (var idx = 0; idx <= arrmp.length; idx++) {
                    if (arrmp[idx] == '')
                        continue;
                    var cid = arrObj[i].name + ',';
                    var ctmp = arrmp[idx] + ',';
                    if (cid.indexOf(ctmp) > 1) {
                        arrObj[i].checked = isCheck;
                        //                    alert(arrObj[i].name + ' is checked ');
                        //                    alert(cid + ctmp);
                    }
                }
            }
        }
    }
    $(function () {
        try {
            var url = '';
            var width = document.getElementById("ContentPlaceHolder1_Tools1_ToolsWap1_WSize").value;
            var height = document.getElementById("ContentPlaceHolder1_Tools1_ToolsWap1_HSize").value;
            var cwidth = document.getElementById("ContentPlaceHolder1_Tools1_ToolsWap1_Chg").value;
            var name = document.getElementById("ContentPlaceHolder1_Tools1_ToolsWap1_ImageName").value;
            if (name) {
                url = '/DataUser/UserIcon/' + name;
                var cw=width / 2;
                var ch=width / 2;
                $.fn.bitmapCutter({
                    src: url,
                    renderTo: '#Container',
                    holderSize: { width: 500, height: 400 },
                    cutterSize: { width: 200, height: 200 },
                    onGenerated: function (src) {
                        alert(src);
                    },
                    rotateAngle: 90,
                    lang: { clockwise: '顺时针旋转{0}度.' }
                });
            }
        }
        catch (e) {

        }
    });
</script>
<style type="text/css">
   
    #Container
    {
        width: 1100px;
        margin: 10px auto;
    }
    #Player
    {
        width: 500px;
        height: 20px;
        padding: 1px;
    }
   #Container{
    width:800px;
    }
    #Content{
        height:600px;   
        margin-top:20px;
    }
    #Content-Left
    {
        height:450px;
        width:600px;
        margin:20px;
        float:left;
    }
    #Content-Main{
        height:60px;
        width:60px;
        margin:20px;
        float:left;
    }
    #Content-Main1{
        height:100px;
        width:100px;
        margin:20px;
        float:left;
    }
    #Content-Main2{
        height:40px;
        width:40px;
        margin:20px;
        float:left;
    }
    #Content-Main3{
        height:10px;
        width:50px;
        margin:20px;
        float:left;
    }
    
    fieldset
{
    border-left: 0px;
    border-right: 0px;
    border-bottom: 0px;
    border-top: 1px outset;
    border-top-color: #CCCCCC;
    border-right-color: inherit;
    border-left-color: inherit;
    border-width: 1px 0px 0px 0px;
    border-style: outset none outset none;
}

legend
{
    font-size:14px;
    color:Black;
}

.LabText
{
    color:Gray;
}

hr
{
    margin-bottom:10px;
    margin-top:10px;
    border-color:white;
    noshade:true;
}
</style>


