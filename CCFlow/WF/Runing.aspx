<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WF_Runing" Codebehind="Runing.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script language="JavaScript" src="./Comm/JScript.js"></script>
    <script language="javascript" >
        function Do(warning, url) {
            if (window.confirm(warning) == false)
                return;

            window.location.href = url;
            // WinOpen(url);
        }
              // 撤销。
              function UnSend(appPath, pageID, fid, workid, fk_flow) {
                  if (window.confirm('您确定要撤销本次发送吗？') == false)
                      return;
                  var url = appPath + 'WF/Do.aspx?DoType=UnSend&FID=' + fid + '&WorkID=' + workid + '&FK_Flow=' + fk_flow + '&PageID=' + pageID;
                  window.location.href = url;
                  return;
              }
              function Press(appPath, fid, workid, fk_flow) {
                  var url = appPath + 'WF/WorkOpt/Press.htm?FID=' + fid + '&WorkID=' + workid + '&FK_Flow=' + fk_flow;
                  var v = window.showModalDialog(url, 'sd', 'dialogHeight: 200px; dialogWidth: 350px;center: yes; help: no');
              }

              function GroupBarClick(appPath, rowIdx) {
                  var alt = document.getElementById('Img' + rowIdx).alert;
                  var sta = 'block';
                  if (alt == 'Max') {
                      sta = 'block';
                      alt = 'Min';
                  } else {
                      sta = 'none';
                      alt = 'Max';
                  }

                  document.getElementById('Img' + rowIdx).src = appPath + 'WF/Img/' + alt + '.gif';
                  document.getElementById('Img' + rowIdx).alert = alt;
                  var i = 0;
                  for (i = 0; i <= 5000; i++) {
                      if (document.getElementById(rowIdx + '_' + i) == null)
                          continue;
                      if (sta == 'block') {
                          document.getElementById(rowIdx + '_' + i).style.display = '';
                      } else {
                          document.getElementById(rowIdx + '_' + i).style.display = sta;
                      }
                  }
              }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
<uc1:Pub ID="Pub1" runat="server" />
</asp:Content>



 
  
