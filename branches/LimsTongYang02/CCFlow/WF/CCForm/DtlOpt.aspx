<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.CCForm.WF_DtlOpt" Codebehind="DtlOpt.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <link href="../Comm/Style/Tabs.css" rel="stylesheet" type="text/css" />
    <script language=javascript>
        function selectAll() {
            var arrObj = document.all;
            if (document.forms[0].checkedAll.checked) {
                for (var i = 0; i < arrObj.length; i++) {
                    if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox') {
                        arrObj[i].checked = true;
                    }
                }
            } else {
                for (var i = 0; i < arrObj.length; i++) {
                    if (typeof arrObj[i].type != "undefined" && arrObj[i].type == 'checkbox')
                        arrObj[i].checked = false;
                }
            }
        }

        function checkType() 
        {
            //得到上传文件的值   
            var fileName = document.getElementById("fup").value;

            //返回String对象中子字符串最后出现的位置.   
            var seat = fileName.lastIndexOf(".");

            //返回位于String对象中指定位置的子字符串并转换为小写.   
            var extension = fileName.substring(seat).toLowerCase();

            //判断允许上传的文件格式   
            //if(extension!=".jpg"&&extension!=".jpeg"&&extension!=".gif"&&extension!=".png"&&extension!=".bmp"){   
            //alert("不支持"+extension+"文件的上传!");   
            //return false;   
            //}else{   
            //return true;   
            //}   

            var allowed = [".jpg", ".gif", ".png", ".bmp", ".jpeg"];
            for (var i = 0; i < allowed.length; i++) {
                if (!(allowed[i] != extension)) {
                    return true;
                }
            }
            alert("不支持" + extension + "格式");
            return false;
        }  
    </script>

    <base target=_self />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
    <uc1:Pub ID="Top" runat="server" />
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>

