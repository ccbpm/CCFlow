<%@ Page Language="C#" MasterPageFile="WinOpen.master" AutoEventWireup="true" Inherits="CCFlow.WF.MapDef.WF_MapDef_MapDef"
    Title="ccform傻瓜表单设计器" CodeBehind="Designer.aspx.cs" %>

<%@ Register Src="Pub.ascx" TagName="Pub" TagPrefix="uc1" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="Server">
    <style type="text/css">
        body
        {
            font-size: .80em;
            font-family: "Helvetica Neue" , "Lucida Grande" , "Segoe UI" , Arial, Helvetica, Verdana, sans-serif;
            margin: 0px;
            padding: 0px;
            color: #696969;
        }
    </style>

    <%--<link href="../../Scripts/easyUI/themes/default/easyui.css" rel="stylesheet" type="text/css" />--%>
  <%--  <script src="../../Scripts/easyUI/jquery-1.8.0.min.js" type="text/javascript"></script>
    <script src="../../Scripts/easyUI/jquery.easyui.min.js" type="text/javascript"></script>
    <script src="../../Comm/Gener.js" type="text/javascript"></script>--%>

     <link href="../../Scripts/easyUI/themes/icon.css" rel="stylesheet" type="text/css" />
    <script language="JavaScript" type="text/javascript" src="../../Comm/JScript.js"></script>
    <script src="../../Scripts/EasyUIUtility.js" type="text/javascript"></script>
    <script src="../../Scripts/config.js" type="text/javascript"></script>
    <script src="../../Comm/Gener.js" type="text/javascript"></script>
    <script language="JavaScript" type="text/javascript" src="MapDef.js" ></script>


    <script language="JavaScript" type="text/javascript" src="../../CCForm/MapExt.js" ></script>
    <script language="JavaScript" type="text/javascript" src="../../Style/Verify.js"></script>
    <script language="JavaScript" type="text/javascript" src="../../Comm/JS/Calendar/WdatePicker.js" defer="defer"></script>

    <script src="../../Scripts/CommonUnite.js" type="text/javascript"></script>
    <script src="../../Scripts/EasyUIUtility.js" type="text/javascript"></script>
    <script src="../../Scripts/config.js" type="text/javascript" > </script>

    <script language="javascript" type="text/javascript">


    
        //公共方法
        function AjaxServiceGener(param,extUrl,callbackFunc, scope) {
            $.ajax({
                type: "GET", //使用GET或POST方法访问后台
                dataType: "text", //返回json格式的数据
                contentType: "application/json; charset=utf-8",
                url: Handler+extUrl, //要访问的后台地址
                data: param, //要发送的数据
                async: true,
                cache: false,
                complete: function () { }, //AJAX请求完成时隐藏loading提示
                error: function (XMLHttpRequest, errorThrown) {
                    callback(XMLHttpRequest);
                },
                success: function (msg) { //msg为返回的数据，在这里做数据绑定
                    var data = msg;
                    callbackFunc(data, scope);
                }
            });
        }
        function ReLoad(data) {
            window.location.href = window.location.href;
        }

        function FrmEvent(mypk) {
            var url = 'FrmEvent.htm?FK_MapData=' + mypk;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        }
       
        function Insert(mypk, IDX) {
            var url = 'FieldTypeList.htm?DoType=AddF&MyPK=' + mypk + '&IDX=' + IDX;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
            window.location.href = window.location.href;
        }
        function AddF(fk_mapdata) {
            //var url = 'Do.htm?DoType=AddF&MyPK=' + mypk;
            var url = 'FieldTypeList.htm?DoType=AddF&FK_MapData=' + fk_mapdata;

            var h = 500;
            var w = 600;
            var l = (screen.width - w) / 2;
            var t = (screen.height - h) / 2;
            var s = 'width=' + w + ', height=' + h + ', top=' + t + ', left=' + l;
            s += ', toolbar=no, scrollbars=no, menubar=no, location=no, resizable=no'; 
            var b = window.showModalDialog(url, 'ass', s);
            window.location.href = window.location.href;
        }
        function AddField(fk_mapdata, groupID) {

            var url = 'FieldTypeList.htm?DoType=AddF&FK_MapData=' + fk_mapdata + '&GroupField=' + groupID;

            OpenEasyUiDialog(url, "eudlgframe", '增加字段', 800, 500, "icon-edit", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
             
        }
        function AddTable(mypk) {
            var url = 'EditCells.htm?MyPK=' + mypk;

            OpenEasyUiDialog(url, "eudlgframe", '新建', 800, 500, "icon-edit", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }
       
        function BatchEdit(mypk) {
            var url = 'BatchEdit.htm?FK_MapData=' + mypk;

            OpenEasyUiDialog(url, "eudlgframe", '批处理', 800, 500, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;

            });
        }

        function CopyFieldFromNode(mypk) {
            var url = 'CopyFieldFromNode.htm?DoType=AddF&FK_Node=' + mypk;

            OpenEasyUiDialog(url, "eudlgframe", '复制', 800, 500, "icon-edit", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }
        function GroupFieldNew(mypk) {
            var url = 'GroupField.htm?FK_MapData=' + mypk + "&RefOID=0&DoType=FunList";

            OpenEasyUiDialog(url, "eudlgframe", '新建', 800, 500, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;

            });

        }
        function ExpImp(fk_mapdata, fk_flow) {
            var url = 'ExpImp.htm?FK_MapData=' + fk_mapdata + "&DoType=FunList&FK_Flow=" + fk_flow;

            OpenEasyUiDialog(url, "eudlgframe", '导入导出', 800, 500, "icon-edit", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }

        function GroupField(mypk, OID) {

            // var url = 'GroupFieldEdit.htm?FK_MapData=' + mypk + "&GroupField=" + OID;
            var url = "../../Comm/En.htm?EnsName=BP.Sys.GroupFields&PK=" + OID;

            OpenEasyUiDialog(url, "eudlgframe", '分组', 800, 500, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });

        }

        function GroupFieldDel(mypk, refoid) {
            var url = 'GroupField.htm?RefNo=' + mypk + '&DoType=DelIt&RefOID=' + refoid;
            var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
            window.location.href = window.location.href;
        }

        function Edit(fk_mapdata, mypk, ftype, gf) {

            var url = 'EditF.htm?DoType=Edit&MyPK=' + mypk + '&FType=' + ftype + '&FK_MapData=' + fk_mapdata + '&GroupField=' + gf;
            var title = '';
            if (ftype == 1) {
                title = '字段String属性';
                url = '/WF/Comm/En.htm?EnsName=BP.Sys.FrmUI.MapAttrStrings&PK=' + mypk + '&s=' + Math.random();
            }

            if (ftype == 2 || ftype == 3 || ftype == 5 || ftype == 8) {
                title = '字段Num属性';
                url = '/WF/Comm/En.htm?EnsName=BP.Sys.FrmUI.MapAttrNums&PK=' + mypk + '&s=' + Math.random();
            }

            if (ftype == 6 || ftype == 7) {
                title = '字段 date 属性';

                url = '/WF/Comm/En.htm?EnsName=BP.Sys.FrmUI.MapAttrDTs&PK=' + mypk + '&s=' + Math.random();
            }

            if (ftype == 6 || ftype == 7) {
                title = '字段 datetime 属性';

                url = '/WF/Comm/En.htm?EnsName=BP.Sys.FrmUI.MapAttrDTs&PK=' + mypk + '&s=' + Math.random();
            }

            if (ftype == 4) {
                title = '字段 boolen 属性';
                url = '/WF/Comm/En.htm?EnsName=BP.Sys.FrmUI.MapAttrBoolens&PK=' + mypk + '&s=' + Math.random();
            }

            OpenEasyUiDialog(url, "eudlgframe", title, 800, 500, "icon-edit", true, null, null, null, function () {
                window.location.href = window.location.href;
            });

           // OpenEasyUiDialog(url, "dd", title, 730, 500);
            return;
        }

        function EditEnum(fk_mapdata, keyOfEn, mypk, enumKey, gf) {
            var url = '/WF/Comm/En.htm?EnsName=BP.Sys.FrmUI.MapAttrEnums&PK=' + mypk + '&s=' + Math.random();

            OpenEasyUiDialog(url, "eudlgframe", '枚举' + keyOfEn + '属性', 730, 500, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }

        function EditTable(fk_mapdata, keyOfEn, mypk, sfTable, gf) {
            var url = '/WF/Comm/En.htm?EnsName=BP.Sys.FrmUI.MapAttrSFTables&PK=' + mypk + '&s=' + Math.random();

            OpenEasyUiDialog(url, "eudlgframe", '外键' + keyOfEn + '属性', 730, 500, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }
        // 向上移动.
        function Up(fk_mapdata, mypk, idx, t) {
            var url = '?DoType=Up&FK_MapData=' + fk_mapdata + '&MyPK=' + mypk + '&ToIdx=' + idx + '&T=' + t;
            AjaxServiceGener(null, url, ReLoad, this);
        }
        //向下移动.
        function Down(fk_mapdata, mypk, idx,t) {
            var url = '?DoType=Down&FK_MapData=' + fk_mapdata + '&MyPK=' + mypk + '&ToIdx=' + idx + '&T=' + t;
            AjaxServiceGener(null, url, ReLoad, this);
        }
     
        function GFDoUp(refoid) {
            var url = '?DoType=GFDoUp&RefOID=' + refoid;
            AjaxServiceGener(null, url, ReLoad, this);
        }
        function GFDoDown(refoid) {
            var url = '?DoType=GFDoDown&RefOID=' + refoid;
            AjaxServiceGener(null, url, ReLoad, this);
        }

        function Del(mypk, refoid) {
            if (window.confirm('您确定要删除吗？') == false)
                return;
            var url = '?DoType=Del&MyPK=' + mypk + '&RefOID=' + refoid;
            AjaxServiceGener(null, url, ReLoad, this);
        }
        function Esc() {
            if (event.keyCode == 27)
                window.close();
            return true;
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

            document.getElementById('Img' + rowIdx).src = './Img/' + alt + '.gif';
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

        var isInser = "";

        function CopyFieldFromNode(mypk) {
            var url = 'CopyFieldFromNode.htm?FK_Node=' + mypk;

            OpenEasyUiDialog(url, "eudlgframe", '复制', 730, 900, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }

        // 子线程.
        function EditFrmThread(mypk) {
            var url = '../../Comm/En.htm?EnName=BP.WF.Template.FrmTrack&PK=' + mypk;

            OpenEasyUiDialog(url, "eudlgframe", '子线程', 400, 700, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }

        // 轨迹图.
        function EditTrack(mypk) {
            var url = '../../Comm/En.htm?EnName=BP.WF.Template.FrmTrack&PK=' + mypk;

            OpenEasyUiDialog(url, "eudlgframe", '轨迹图', 400, 700, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });

        }


        /// 审核组件.
        function EditFWC(mypk) {
            //http: //localhost:41466/WF/Comm/RefFunc/UIEn.htm?EnsName=BP.WF.Template.FrmNodeComponents&PK=7901&EnName=BP.WF.Template.FrmNodeComponent&tab=%E7%88%B6%E5%AD%90%E6%B5%81%E7%A8%8B%E7%BB%84%E4%BB%B6
            var url = '../../Comm/En.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + mypk + '&tab=审核组件';

            OpenEasyUiDialog(url, "eudlgframe", '组件', 400, 700, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }
        //子流程.
        function EditSubFlow(mypk) {
            // var url = '../Comm/En.htm?EnName=BP.WF.Template.FrmSubFlow&PK=' + mypk
            var url = '../../Comm/En.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + mypk + '&tab=父子流程组件';

            OpenEasyUiDialog(url, "eudlgframe", '组件', 400, 700, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }

        //子线程.
        function EditThread(mypk) {
            // var url = '../Comm/En.htm?EnName=BP.WF.Template.FrmSubFlow&PK=' + mypk
            var url = '../../Comm/En.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + mypk + '&tab=子线程组件';

            OpenEasyUiDialog(url, "eudlgframe", '组件', 400, 700, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });

        }

        //流转自定义.
        function EditFTC(mypk) {
            // var url = '../Comm/En.htm?EnName=BP.WF.Template.FrmSubFlow&PK=' + mypk
            var url = '../../Comm/En.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + mypk + '&tab=流转自定义';

            OpenEasyUiDialog(url, "eudlgframe", '组件', 400, 700, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });

        }

        //轨迹组件.
        function EditTrack(mypk) {
            // var url = '../Comm/En.htm?EnName=BP.WF.Template.FrmSubFlow&PK=' + mypk
            var url = '../../Comm/En.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + mypk + '&tab=轨迹组件';


            OpenEasyUiDialog(url, "eudlgframe", '组件', 400, 700, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });

        }

        function MapDataEdit(mypk) {
            var url = '../../Comm/En.htm?EnName=BP.WF.Template.MapFrmFool&PK=' + mypk;

            OpenEasyUiDialog(url, "eudlgframe", '属性', 500, 850, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });

        }

        function FrmNodeComponent(mypk) {
            var url = '../../Comm/En.htm?EnName=BP.WF.Template.FrmNodeComponent&PK=' + mypk;

            OpenEasyUiDialog(url, "eudlgframe", '组件', 500, 850, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }

        //新增从表.
        function NewMapDtl(fk_mapdata) {
            var val = prompt('请输入从表ID，要求表单唯一。', fk_mapdata + 'Dtl1');
            if (val == null) {
                return;
            }

            if (val == '') {
                alert('请输入从表ID不能为空，请重新输入！');
                NewMapDtl(fk_mapdata);
                return;
            }

            $.ajax({
                type: 'post',
                async: true,
                url: Handler + "?DoType=Designer_NewMapDtl&FK_MapData=" + fk_mapdata + "&DtlNo=" + val + "&m=" + Math.random(),
                dataType: 'html',
                success: function (data) {
                    if (data.indexOf('err@') == 0) {
                        alert(data);
                        return;
                    }

                    var url = '../../Comm/En.htm?EnsName=BP.WF.Template.MapDtlExts&FK_MapData=' + fk_mapdata + '&No=' + data;
                    OpenEasyUiDialog(url, "eudlgframe", '从表属性', 800, 500, "icon-edit", true, null, null, null, function () {
                        window.location.href = window.location.href;
                    });

                    return;
                }
            });
        }

        
        ///编辑从表.
        function EditDtl(fk_mapdata, mypk) {
            var url = '../../Comm/En.htm?EnsName=BP.WF.Template.MapDtlExts&FK_MapData=' + fk_mapdata + '&No=' + mypk;

            OpenEasyUiDialog(url, "eudlgframe", '从表', 800, 500, "icon-edit", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }

        function EditM2M(mypk, dtlKey) {
            var url = 'MapM2M.htm?DoType=Edit&FK_MapData=' + mypk + '&NoOfObj=' + dtlKey;
            // OpenEasyUiDialog(url, 'dtl', '属性', 800, 500, "icon-edit");
        }


        /// 多选.
        function MapM2M(mypk) {
            var url = 'MapM2M.htm?DoType=List&FK_MapData=' + mypk;

            OpenEasyUiDialog(url, "eudlgframe", '多选', 800, 500, "icon-edit", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }

        function MapM2MM(mypk) {
            var url = 'MapM2MM.htm?FK_MapData=' + mypk;

            OpenEasyUiDialog(url, "eudlgframe", '多选', 800, 500, "icon-edit", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }

        //修改附件.
        function EditAth(fk_mapdata, ath) {
            var url = '../../Comm/En.htm?EnsName=BP.Sys.FrmUI.FrmAttachmentExts&FK_MapData=' + fk_mapdata + '&MyPK=' + fk_mapdata + "_" + ath;

            OpenEasyUiDialog(url, "eudlgframe", '附件', 800, 500, "icon-edit", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }

        //新增附件.
        function NewAth(fk_mapdata) {
            var val = prompt('请输入附件ID，要求表单唯一。', 'Ath1');
            if (val == null) {
                return;
            }
            if (val == '') {
                alert('附件ID不能为空，请重新输入！');
                NewAth(fk_mapdata);
                return;
            }
            $.ajax({
                type: 'post',
                async: true,
                url: Handler + "?DoType=Designer_AthNew&FK_MapData=" + fk_mapdata + "&AthNo=" + val + "&m=" + Math.random(),
                dataType: 'html',
                success: function (data) {
                    if (data.indexOf('err@') == 0) {
                        alert(data);
                        return;
                    }

                    var url = '../../Comm/En.htm?EnsName=BP.Sys.FrmUI.FrmAttachmentExts&FK_MapData=' + fk_mapdata + '&MyPK=' + data;
                    OpenEasyUiDialog(url, "eudlgframe", '附件', 800, 500, "icon-edit", true, null, null, null, function () {
                        window.location.href = window.location.href;
                    });

                    return;
                }
            });
        }

        function NewFrame(fk_mapdata) {

            var val = prompt('请输入框架ID，要求表单唯一。', 'Frame1');
            if (val == null) {
                return;
            }
            if (val == '') {
                alert('框架ID不能为空，请重新输入！');
                NewFrame(fk_mapdata);
                return;
            }

            $.ajax({
                type: 'post',
                async: true,
                url: Handler + "?DoType=Designer_NewFrame&FK_MapData=" + fk_mapdata + "&FrameNo=" + val + "&m=" + Math.random(),
                dataType: 'html',
                success: function (data) {

                    if (data.indexOf('err@') == 0) {
                        alert(data);
                        return;
                    }

                    var url = '../../Comm/En.htm?EnsName=BP.Sys.MapFrames&FK_MapData=' + fk_mapdata + '&MyPK=' + data;

                    OpenEasyUiDialog(url, "eudlgframe", '框架', 800, 500, "icon-property", true, null, null, null, function () {
                        window.location.href = window.location.href;
                    });
                    return;
                }
            });
        }

        function EditFrame(fk_mapdata, myPK) {
            var url = '../../Comm/En.htm?EnsName=BP.Sys.MapFrames&FK_MapData=' + fk_mapdata + '&MyPK=' + myPK;

            OpenEasyUiDialog(url, "eudlgframe", '框架', 800, 500, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }

        function MapFrame(fk_mapdata) {
            var url = 'MapFrame.htm?FK_MapData=' + fk_mapdata;

            OpenEasyUiDialog(url, "eudlgframe", '框架', 800, 500, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });
        }

        function HidAttr(fk_mapData) {
            var url = 'HidAttr.htm?FK_MapData=' + fk_mapData;

            OpenEasyUiDialog(url, "eudlgframe", '隐藏字段', 800, 500, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });

           // var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
           // window.location.href = window.location.href;
        }
        function EnableAthM(fk_MapDtl) {
            var url = '../CCForm/AttachmentUpload.htm?IsBTitle=1&PKVal=0&Ath=AthMDtl&FK_MapData=' + fk_MapDtl + '&FK_FrmAttachment=' + fk_MapDtl + '_AthMDtl';


            OpenEasyUiDialog(url, "eudlgframe", '多附件', 800, 500, "icon-property", true, null, null, null, function () {
                window.location.href = window.location.href;
            });

        }
        function Sln(fk_mapdata) {
            var url = 'Sln.htm?IsBTitle=1&PKVal=0&Ath=AthM&FK_MapData=' + fk_mapdata + '&FK_FrmAttachment=' + fk_mapdata + '_AthM';
            WinOpen(url);
            //var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
        }

        //然浏览器最大化.
        function ResizeWindow() {
            if (window.screen) {  //判断浏览器是否支持window.screen判断浏览器是否支持screen     
                var myw = screen.availWidth;   //定义一个myw，接受到当前全屏的宽     
                var myh = screen.availHeight;  //定义一个myw，接受到当前全屏的高     
                window.moveTo(0, 0);           //把window放在左上角     
                window.resizeTo(myw, myh);     //把当前窗体的长宽跳转为myw和myh     
            }
        }
        window.onload = ResizeWindow();
    </script>
    <base target="_self" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
    <div class="easyui-layout" data-options="fit:true">
        <div data-options="region:'north'" style="height: auto; padding: 5px; overflow-y: hidden;">
            <%
                string fk_mapdata = this.Request.QueryString["FK_MapData"];
                string fk_flow = this.Request.QueryString["FK_Flow"];
                // int nodeID = 0  this.Request.QueryString["FK_Flow"];
                string nodeID = this.Request.QueryString["NodeID"];
            %>
            <a href="javascript:ExpImp('<%=fk_mapdata %>','<%=fk_flow%>');" class="easyui-linkbutton"
                data-options="plain:true,iconCls:'icon-copy'">导入/导出</a> <a href="javascript:AddF('<%=fk_mapdata %>');"
                    class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-new'">新建字段</a>
            <a href="javascript:HidAttr('<%=fk_mapdata %>');" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-hidden'">
                隐藏字段</a> <a href="javascript:GroupFieldNew('<%=fk_mapdata %>');" class="easyui-linkbutton"
                    data-options="plain:true,iconCls:'icon-groupbar'">新建字段分组</a> <a href="javascript:NewMapDtl('<%=fk_mapdata %>');"
                        class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-dtl'">新建从表</a>
            <a href="javascript:NewAth('<%=fk_mapdata %>');" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-attachment'">
                新建附件组件</a> <a href="javascript:NewFrame('<%=fk_mapdata %>');" class="easyui-linkbutton"
                    data-options="plain:true,iconCls:'icon-frame'">新建框架</a>
            <%--<a href="javascript:MapExt('<%=fk_mapdata %>');" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-dts'">扩展设置</a>
            --%>
            <a href="javascript:MapDataEdit('<%=fk_mapdata %>');" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-property'">
                表单属性</a>
            <% if (string.IsNullOrEmpty(fk_flow) == false)
               { %>
            <a href="javascript:FrmNodeComponent('<%=this.NodeID %>');" class="easyui-linkbutton" data-options="plain:true,iconCls:'icon-Components'">节点表单组件</a>
            <% } %>
            <uc1:Pub ID="UCCaption" runat="server" />
        </div>
        <div data-options="region:'center'" style="background-color: #d0d0d0; padding-top: 10px;
            padding-bottom: 10px;">
            <uc1:Pub ID="Pub1" runat="server" />
        </div>
    </div>
</asp:Content>
