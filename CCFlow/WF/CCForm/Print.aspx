<%@ Page Title="" Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true"
    Inherits="CCFlow.WF.CCForm.WF_CCForm_Print" CodeBehind="Print.aspx.cs" %>

<%@ Register Src="../Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <script src="../Scripts/jquery-1.7.2.min.js" type="text/javascript"></script>
    <script src="../Scripts/QueryString.js" type="text/javascript"></script>
    <script src="../Scripts/CreateControl.js" type="text/javascript"></script>
    <script type="text/javascript">
        
        function btnPreview_onclick(name) {
            try {
                var FK_MapData = "";
                var frmParas = name.split('.');
                if (frmParas.length == 3 || frmParas.length == 2) {
                    FK_MapData = frmParas[0];
                }

                var ReportViewer = document.getElementById("ReportViewer");
                var Report = ReportViewer.Report;
                var arges = new RequestArgs();
                Report.LoadFromURL("../../DataUser/CyclostyleFile/FlowFrm/" + arges.FK_Flow + "/" + arges.FK_Node + "/" + encodeURI(name));

                //装载主表
                var pageUrl = "../WorkOpt/GridData.ashx?WorkID=" + arges.WorkID + "&FK_Flow=" + arges.FK_Flow + "&FK_Node=" + arges.FK_Node + "&FK_MapData=" + FK_MapData + "&DoType=4&GetType=MainPage";
                Report.LoadDataFromURL(pageUrl);
                //获取明细表编号
                var json_data = { "WorkID": arges.WorkID, "FK_Flow": arges.FK_Flow, "FK_Node": arges.FK_Node, FK_MapData: FK_MapData, "DoType": "3" };
                $.ajax({
                    type: "get",
                    url: "../WorkOpt/GridData.ashx",
                    data: json_data,
                    async: true,
                    beforeSend: function (XMLHttpRequest, fk_mapExt) {
                        //ShowLoading();
                    },
                    success: function (data, textStatus) {
                        if (data != null) {
                            //装载明细表
                            var resultData = eval("(" + data + ")");
                            var arges = new RequestArgs();
                            for (var idx in resultData) {
                                try {
                                    var childReport = ReportViewer.Report.ControlByName(resultData[idx].Name);
                                    if (childReport == "" || childReport == null) {
                                        alert("没有在模版中找到相应的子报表");
                                    } else {
                                        childReport.AsSubReport.Report.LoadDataFromURL("../WorkOpt/GridData.ashx?WorkID=" + arges.WorkID + "&FK_Flow=" + arges.FK_Flow + "&FK_Node=" + arges.FK_Node + "&DoType=4&GetType=" + resultData[idx].Name);
                                    }
                                } catch (e) {
                                    alert(e.message);
                                }
                            }
                            //装载图片
                            json_data = { "WorkID": arges.WorkID, "FK_Flow": arges.FK_Flow, "FK_Node": arges.FK_Node, "DoType": "6" };
                            $.ajax({
                                type: "get",
                                url: "../WorkOpt/GridData.ashx",
                                data: json_data,
                                async: true,
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
                                    //alert('HideLoading');
                                    //HideLoading();
                                },
                                error: function () {
                                    alert('加载图片出现异常！！.');
                                    //请求出错处理
                                }
                            });
                        } else {
                            alert('加载数据出现异常!');
                        }
                        //最后显示
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
                alert(ex.message);
            }
        }

        //获取参数
        var RequestArgs = function () {
            this.WorkID = GetQueryString("WorkID");
            this.FK_Flow = GetQueryString("FK_Flow");
            this.FK_Node = GetQueryString("FK_Node");
            if (this.FK_Node) {
                while (this.FK_Node.substring(0, 1) == '0') this.FK_Node = this.FK_Node.substring(1);
                this.FK_Node = this.FK_Node.replace('#', '');
            }
            this.FID = GetQueryString("FID");
        }
    </script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <script type="text/javascript">
        CreatePrintViewerEx("0", "0", "", "", false, "<param name=BorderStyle value=1>");
    </script>
    <uc1:Pub ID="Pub1" runat="server" />
</asp:Content>
