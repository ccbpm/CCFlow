<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.WorkOpt.WF_WorkOpt_PrintDoc" Codebehind="PrintDoc.aspx.cs" %>
<%@ Register src="Pub.ascx" tagname="Pub" tagprefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" Runat="Server">
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/CreateControl.js" type="text/javascript"></script>
    <script type="text/javascript">
        function btnPreview_onclick(name) {
            try { 
                var ReportViewer = document.getElementById("ReportViewer");

                var Report = ReportViewer.Report;
                Report.LoadFromURL("../../DataUser/CyclostyleFile/" + name + ".grf");

                               
                var json_data = { "WorkID": "<%=this.WorkID %>", "FK_Flow": "<%=this.FK_Flow %>", "FK_Node": "<%=this.FK_Node %>", "DoType": "0" };
                $.ajax({
                    type: "get",
                    url: "../WorkOpt/GridData.ashx",
                    data: json_data,
                    async:false,
                    beforeSend: function (XMLHttpRequest, fk_mapExt) {
                        //ShowLoading();
                    },
                    success: function (data, textStatus) {
                        var jsonData = eval("(" + data + ")");
                        if (jsonData.length > 0) {
                            for (var idx in jsonData) {
                                var childReport = ReportViewer.Report.ControlByName(jsonData[idx].Name);
                                if (childReport == undefined || childReport == null) {
                                    //alert("加载子报表为空");
                                } else {
                                    childReport.AsSubReport.Report.LoadDataFromURL("/WF/WorkOpt/GridData.ashx?WorkID=<%=this.WorkID %>&FK_Flow=<%=this.FK_Flow %>&FK_Node=<%=this.FK_Node %>&DoType=1&Name="+jsonData[idx].Name);
                                }
                            }
                        }


                        json_data = { "WorkID": "<%=this.WorkID %>", "FK_Flow": "<%=this.FK_Flow %>", "FK_Node": "<%=this.FK_Node %>", "DoType": "5" };

                        $.ajax({
                            type: "get",
                            url: "../WorkOpt/GridData.ashx",
                            data: json_data,
                            async: false,
                            beforeSend: function (XMLHttpRequest, fk_mapExt) {


                                //ShowLoading();
                            },
                            success: function (PhotoData, textStatus) {

                                var dataPhoto = eval("(" + PhotoData + ")");

                                for (var index in dataPhoto) {

                                    //alert(dataPhoto[index].Name + "--" + dataPhoto[index].Value);
                                    try {
                                        Report.ControlByName(dataPhoto[index].Name).AsPictureBox.LoadFromFile(dataPhoto[index].Value);
                                    } catch (e) {
                                        
                                    }
                                }

                            },
                            complete: function (XMLHttpRequest, textStatus) {
                                //    alert('HideLoading');
                                //HideLoading();
                            },
                            error: function () {
                                alert('加载图片出现异常！！.');
                                //请求出错处理
                            }
                        });

                        Report.LoadDataFromURL("../WorkOpt/GridData.ashx?WorkID=<%=this.WorkID %>&FK_Flow=<%=this.FK_Flow %>&FK_Node=<%=this.FK_Node %>&DoType=1&Name=MainPage");
                        Report.PrintPreview(true);
                    },
                    complete: function (XMLHttpRequest, textStatus) {
                        //    alert('HideLoading');
                        //HideLoading();
                    },
                    error: function () {
                        alert('error when load data.');
                        //请求出错处理
                    }
                });

            } catch (ex) {
                alert("error when open the view");
            }
        }

    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">
      <script type="text/javascript">
          if ("<%=this.IsRuiLang %>" == "True") {
                  CreatePrintViewerEx("0", "0", "", "", false, "<param name=BorderStyle value=1>");
          }
      </script>
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>

