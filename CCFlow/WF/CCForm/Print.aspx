<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true"
    Inherits="CCFlow.WF.CCForm.WF_CCForm_Print" CodeBehind="Print.aspx.cs" %>

<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/CreateControl.js" type="text/javascript"></script>
    <script type="text/javascript">

        function btnPreview_onclick(name) {
            try {
                var ReportViewer = document.getElementById("ReportViewer");

                var Report = ReportViewer.Report;
                Report.LoadFromURL("../../DataUser/CyclostyleFile/FlowFrm/<%=this.FK_Flow %>/<%=this.FK_Node %>/" + name);
                // Report.LoadFromURL("../../DataUser/grf/4a.grf");
                var jsonCount;
                //                var json_data = { "WorkID": "<%=this.WorkID %>", "FK_Flow": "<%=this.FK_Flow %>", "FK_Node": "<%=this.FK_Node %>", "DoType": "2" };
                var json_data = { "WorkID": "<%=this.WorkID %>", "FK_Flow": "<%=this.FK_Flow %>", "FK_Node": "<%=this.FK_Node %>", "DoType": "3" };
                $.ajax({
                    type: "get",
                    url: "../WorkOpt/GridData.ashx",
                    data: json_data,
                    async: false,
                    beforeSend: function (XMLHttpRequest, fk_mapExt) {
                        //ShowLoading();
                    },
                    success: function (data, textStatus) {
                        if (data != null) {
                            var resultData = eval("(" + data + ")");
                            for (var idx in resultData) {

                                //                                json_data = {
                                //                                    "WorkID": "<%=this.WorkID %>",
                                //                                    "FK_Flow": "<%=this.FK_Flow %>",
                                //                                    "FK_Node": "<%=this.FK_Node %>",
                                //                                    "DoType": "4",
                                //                                    "ChildName": resultData[idx].Name
                                //                                };
                                //                                $.ajax({
                                //                                    type: "get",
                                //                                    url: "../WorkOpt/GridData.ashx",
                                //                                    data: json_data,
                                //                                    async: false,
                                //                                    success: function (child, dataStatus) {
                                //                                        var childData = eval("(" + child + ")");
                                //                                        if (childData.length > 0) {
                                //                                            for (var childIdx in childData) {
                                try {

                                    var childReport = Report.ControlByName(resultData[idx].Name);
                                    if (childReport == "" || childReport == null) {
                                        alert("没有在模版中找到相应的子报表");
                                    } else {
                                        //                                                        childReport.AsSubReport.Report.LoadDataFromURL("../WorkOpt/GridData.ashx?WorkID=<%=this.WorkID %>&FK_Flow=<%=this.FK_Flow %>&FK_Node=<%=this.FK_Node %>&DoType=4&Name=" + resultData[idx].Name + "&GetType=" + childData[childIdx].Name);
                                        childReport.AsSubReport.Report.LoadDataFromURL("../WorkOpt/GridData.ashx?WorkID=<%=this.WorkID %>&FK_Flow=<%=this.FK_Flow %>&FK_Node=<%=this.FK_Node %>&DoType=4&GetType=" + resultData[idx].Name);
                                    }
                                } catch (e) {
                                    alert(e.message);
                                }
                                //                                            }
                                //                                        }

                                //                                    },
                                //                                    error: function () {
                                //                                        alert('error when load data.');
                                //                                    }
                                //                                });


                                //                                ReportViewer.Report.ControlByName(resultData[idx].Name).AsSubReport.Report.LoadDataFromURL("../WorkOpt/GridData.ashx?WorkID=<%=this.WorkID %>&FK_Flow=<%=this.FK_Flow %>&FK_Node=<%=this.FK_Node %>&DoType=4&Name=" + resultData[idx].Name + "&GetType=MainPage");

                            }

                            json_data = { "WorkID": "<%=this.WorkID %>", "FK_Flow": "<%=this.FK_Flow %>", "FK_Node": "<%=this.FK_Node %>", "DoType": "6" };

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
                                        try {
                                            Report.ControlByName(dataPhoto[index].Name).AsPictureBox.LoadFromFile(dataPhoto[index].Value);
                                        }
                                        catch (e) {

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


                            ReportViewer.Report.LoadDataFromURL("../WorkOpt/GridData.ashx?WorkID=<%=this.WorkID %>&FK_Flow=<%=this.FK_Flow %>&FK_Node=<%=this.FK_Node %>&DoType=4&GetType=MainPage");
                            Report.PrintPreview(true);
                        } else {
                            alert('加载数据出现异常!');
                        }
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
                alert(ex.message);
            }
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript">
        CreatePrintViewerEx("0", "0", "", "", false, "<param name=BorderStyle value=1>");
    </script>
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>
