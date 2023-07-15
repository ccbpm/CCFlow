using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 附件
    /// </summary>
    public class FrmAttachmentExt : EntityMyPK
    {
        /// <summary>
        /// 访问权限.
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsView = true;
                uac.IsInsert = false;
                if (BP.Web.WebUser.No.Equals("admin")
                    || BP.Web.WebUser.IsAdmin == true)
                {
                    uac.IsUpdate = true;
                    uac.IsDelete = true;
                    return uac;
                }
                return uac;
            }
        }

        #region 参数属性.
        /// <summary>
        /// 是否可见？
        /// </summary>
        public bool IsVisable
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.IsVisable, true);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.IsVisable, value);
            }
        }
        /// <summary>
        /// 附件类型
        /// </summary>
        public int FileType
        {
            get
            {
                return this.GetParaInt(FrmAttachmentAttr.FileType);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.FileType, value);
            }
        }
        /// <summary>
        /// 使用上传附件的 - 控件类型
        /// 0=批量.
        /// 1=单个。
        /// </summary>
        public int UploadCtrl
        {
            get
            {
                return this.GetParaInt(FrmAttachmentAttr.UploadCtrl);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.UploadCtrl, value);
            }
        }
        /// <summary>
        /// 上传校验
        /// 0=不校验.
        /// 1=不能为空.
        /// 2=每个类别下不能为空.
        /// </summary>
        public UploadFileNumCheck UploadFileNumCheck
        {
            get
            {
                return (UploadFileNumCheck)this.GetValIntByKey(FrmAttachmentAttr.UploadFileNumCheck);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.UploadFileNumCheck, (int)value);
            }
        }

        #endregion 参数属性.

        #region 属性
        /// <summary>
        /// 节点编号
        /// </summary>
        public int FK_Node
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.FK_Node, value);
            }
        }
        /// <summary>
        /// 上传类型（单个的，多个，指定的）
        /// </summary>
        public AttachmentUploadType UploadType
        {
            get
            {
                return (AttachmentUploadType)this.GetValIntByKey(FrmAttachmentAttr.UploadType);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.UploadType, (int)value);
            }
        }
        /// <summary>
        /// 类型名称
        /// </summary>
        public string UploadTypeT
        {
            get
            {
                if (this.UploadType == AttachmentUploadType.Multi)
                    return "多附件";
                if (this.UploadType == AttachmentUploadType.Single)
                    return "单附件";
                if (this.UploadType == AttachmentUploadType.Specifically)
                    return "指定的";
                return "XXXXX";
            }
        }
        /// <summary>
        /// 是否可以上传
        /// </summary>
        public bool IsUpload
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsUpload);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsUpload, value);
            }
        }
        /// <summary>
        /// 是否可以下载
        /// </summary>
        public bool IsDownload
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsDownload);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsDownload, value);
            }
        }
        /// <summary>
        /// 附件删除方式
        /// </summary>
        public AthDeleteWay HisDeleteWay
        {
            get
            {
                return (AthDeleteWay)this.GetValIntByKey(FrmAttachmentAttr.DeleteWay);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.DeleteWay, (int)value);
            }
        }


        /// <summary>
        /// 自动控制大小
        /// </summary>
        public bool IsAutoSize
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsAutoSize);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsAutoSize, value);
            }
        }
        /// <summary>
        /// IsShowTitle
        /// </summary>
        public bool IsShowTitle
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsShowTitle);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsShowTitle, value);
            }
        }
        /// <summary>
        /// 是否是节点表单.
        /// </summary>
        public bool IsNodeSheet
        {
            get
            {
                if (this.FK_MapData.StartsWith("ND") == true)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 备注列
        /// </summary>
        public bool IsNote
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsNote);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsNote, value);
            }
        }
        /// <summary>
        /// 附件名称
        /// </summary>
        public string Name
        {
            get
            {
                string str = this.GetValStringByKey(FrmAttachmentAttr.Name);
                if (DataType.IsNullOrEmpty(str) == true)
                    str = "未命名";
                return str;
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.Name, value);
            }
        }
        /// <summary>
        /// 类别
        /// </summary>
        public string Sort
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentAttr.Sort);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.Sort, value);
            }
        }
        /// <summary>
        /// 要求的格式
        /// </summary>
        public string Exts
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentAttr.Exts);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.Exts, value);
            }
        }

        /// <summary>
        /// 附件标识
        /// </summary>
        public string NoOfObj
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentAttr.NoOfObj);
            }

        }

        /// <summary>
        /// H
        /// </summary>
        public float H
        {
            get
            {
                return this.GetValFloatByKey(FrmAttachmentAttr.H);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.H, value);
            }
        }
        /// <summary>
        /// 数据控制方式
        /// </summary>
        public AthCtrlWay HisCtrlWay
        {
            get
            {
                return (AthCtrlWay)this.GetValIntByKey(FrmAttachmentAttr.CtrlWay);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.CtrlWay, (int)value);
            }
        }
        /// <summary>
        /// 是否是合流汇总多附件？
        /// </summary>
        public bool IsHeLiuHuiZong
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsHeLiuHuiZong);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsHeLiuHuiZong, value);
            }
        }
        /// <summary>
        /// 该附件是否汇总到合流节点上去？
        /// </summary>
        public bool IsToHeLiuHZ
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsToHeLiuHZ);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.IsToHeLiuHZ, value);
            }
        }
        /// <summary>
        /// 文件展现方式
        /// </summary>
        public FileShowWay FileShowWay
        {
            get
            {
                return (FileShowWay)this.GetParaInt(FrmAttachmentAttr.FileShowWay);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.FileShowWay, (int)value);
            }
        }
        /// <summary>
        /// 上传方式（对于父子流程有效）
        /// </summary>
        public AthUploadWay AthUploadWay
        {
            get
            {
                return (AthUploadWay)this.GetValIntByKey(FrmAttachmentAttr.AthUploadWay);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.AthUploadWay, (int)value);
            }
        }
        /// <summary>
        /// FK_MapData
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(FrmAttachmentAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.FK_MapData, value);
            }
        }

        /// <summary>
        /// 是否要转换成html
        /// </summary>
        public bool IsTurn2Html
        {
            get
            {
                return this.GetValBooleanByKey(FrmAttachmentAttr.IsTurn2Html);
            }
        }
        #endregion

        #region 快捷键
        /// <summary>
        /// 是否启用快捷键
        /// </summary>
        public bool FastKeyIsEnable
        {
            get
            {
                return this.GetParaBoolen(FrmAttachmentAttr.FastKeyIsEnable);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.FastKeyIsEnable, value);
            }
        }
        /// <summary>
        /// 启用规则
        /// </summary>
        public string FastKeyGenerRole
        {
            get
            {
                return this.GetParaString(FrmAttachmentAttr.FastKeyGenerRole);
            }
            set
            {
                this.SetPara(FrmAttachmentAttr.FastKeyGenerRole, value);
            }
        }
        #endregion 快捷键

        #region 构造方法
        /// <summary>
        /// 附件
        /// </summary>
        public FrmAttachmentExt()
        {
        }
        /// <summary>
        /// 附件
        /// </summary>
        /// <param name="mypk">主键</param>
        public FrmAttachmentExt(string mypk)
        {
            this.setMyPK(mypk);
            this.Retrieve();
        }
        /// <summary>
        /// EnMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_FrmAttachment", "附件");
                map.IndexField = MapAttrAttr.FK_MapData;
                map.AddGroupAttr("基础信息");
                map.AddMyPK();

                #region 基本属性。
                map.AddTBString(FrmAttachmentAttr.FK_MapData, null, "表单ID", true, true, 1, 100, 20);
                map.AddTBString(FrmAttachmentAttr.NoOfObj, null, "附件标识", true, true, 0, 50, 20);
                map.AddTBInt(FrmAttachmentAttr.FK_Node, 0, "节点控制(对sln有效)", false, false);

                //for渔业厅增加.
                map.AddDDLSysEnum(FrmAttachmentAttr.AthRunModel, 0, "运行模式", true, true, FrmAttachmentAttr.AthRunModel,
                  "@0=流水模式@1=固定模式@2=自定义页面");

                map.AddTBString(FrmAttachmentAttr.Name, null, "附件名称", true, false, 0, 50, 20, true);

                map.AddTBString(FrmAttachmentAttr.Exts, null, "文件格式", true, false, 0, 50, 20, true, null);
                map.SetHelperAlert(FrmAttachmentAttr.Exts, "上传要求,设置模式为: *.*, *.doc, *.docx, *.png,多个中间用逗号分开.\t\n表示仅仅允许上传指定的后缀的文件.");

                map.AddTBInt(FrmAttachmentAttr.NumOfUpload, 0, "最小上传数量", true, false);
                map.SetHelperAlert("NumOfUpload", "如果为0则标识必须上传. \t\n用户上传的附件数量低于指定的数量就不让保存.");

                map.AddTBInt(FrmAttachmentAttr.TopNumOfUpload, 99, "最大上传数量", true, false);
                map.AddTBInt(FrmAttachmentAttr.FileMaxSize, 10240, "附件最大限制(KB)", true, false);
                map.AddDDLSysEnum(FrmAttachmentAttr.UploadFileNumCheck, 0, "上传校验方式", true, true, FrmAttachmentAttr.UploadFileNumCheck,
                  "@0=不用校验@1=不能为空@2=每个类别下不能为空");

                map.AddDDLSysEnum(FrmAttachmentAttr.AthSaveWay, 0, "保存方式", true, true, FrmAttachmentAttr.AthSaveWay,
                  "@0=保存到web服务器@1=保存到数据库@2=ftp服务器");


                map.AddBoolean(FrmAttachmentAttr.IsIdx, false, "是否排序?", true, true);


                map.AddTBString(FrmAttachmentAttr.Sort, null, "类别", true, false, 0, 500, 20, true, null);
                map.SetHelperAlert(FrmAttachmentAttr.Sort, "设置格式:生产类,文件类,其他，也可以设置一个SQL，比如select Name FROM Port_Dept  \t\n目前已经支持了扩展列,可以使用扩展列定义更多的字段，该设置将要被取消.");

                map.AddBoolean(FrmAttachmentAttr.IsTurn2Html, false, "是否转换成html(方便手机浏览)", true, true, true);

                //附件是否显示
                map.AddBoolean(FrmAttachmentAttr.IsVisable, true, "是否显示附件分组", true, true, true);

                map.AddDDLSysEnum(FrmAttachmentAttr.FileType, 0, "附件类型", true, true, FrmAttachmentAttr.FileType, "@0=普通附件@1=图片文件");

                map.AddDDLSysEnum(FrmAttachmentAttr.PicUploadType, 0, "图片附件上传方式", true, true, FrmAttachmentAttr.PicUploadType, "@0=拍照上传或者相册上传@1=只能拍照上传");
                map.SetHelperAlert(FrmAttachmentAttr.PicUploadType, "该功能只使用于移动端图片文件上传的方式.");

                map.AddBoolean(FrmAttachmentAttr.IsEnableTemplate, true, "是否启用模板下载？", true, true, true);
                map.AddTBFloat(FrmAttachmentAttr.H, 150, "高度", true, false);
                // @wwh.
                map.AddMyFileS("附件模板");

                #endregion 基本属性。

                #region 权限控制。
                //hzm新增列
                // map.AddTBInt(FrmAttachmentAttr.DeleteWay, 0, "附件删除规则(0=不能删除1=删除所有2=只能删除自己上传的", false, false);
                map.AddGroupAttr("权限控制");
                map.AddDDLSysEnum(FrmAttachmentAttr.DeleteWay, 1, "附件删除规则", true, true, FrmAttachmentAttr.DeleteWay,
                    "@0=不能删除@1=删除所有@2=只能删除自己上传的");

                map.AddBoolean(FrmAttachmentAttr.IsUpload, true, "是否可以上传", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsDownload, true, "是否可以下载", true, true);

                map.AddBoolean(FrmAttachmentAttr.IsAutoSize, true, "自动控制大小", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsNote, true, "是否增加备注", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsExpCol, true, "是否启用扩展列", true, true);

                map.AddBoolean(FrmAttachmentAttr.IsShowTitle, true, "是否显示标题列", true, true);

                map.AddDDLSysEnum(FrmAttachmentAttr.UploadType, 0, "上传类型", true, false, FrmAttachmentAttr.CtrlWay, "@0=单个@1=多个@2=指定");
                map.SetHelperAlert(FrmAttachmentAttr.UploadType, "单附件：请使用字段单附件组件。");


                map.AddDDLSysEnum(FrmAttachmentAttr.AthUploadWay, 0, "控制上传控制方式", true, true, FrmAttachmentAttr.AthUploadWay, "@0=继承模式@1=协作模式");

                map.AddDDLSysEnum(FrmAttachmentAttr.CtrlWay, 4, "控制呈现控制方式", true, true, "Ath" + FrmAttachmentAttr.CtrlWay,
                    "@0=PK-主键@1=FID-干流程ID@2=PWorkID-父流程ID@3=仅能查看自己上传的附件@4=WorkID-按照WorkID计算(对流程节点表单有效)@5=P2WorkID@6=P3WorkID");


                //map.AddDDLSysEnum(FrmAttachmentAttr.DataRef, 0, "数据引用", true, true, FrmAttachmentAttr.DataRef,
                //    "@0=当前组件ID@1=指定的组件ID");
                #endregion 权限控制。

                #region 流程相关
                map.AddGroupAttr("流程相关");
                //map.AddDDLSysEnum(FrmAttachmentAttr.DtlOpenType, 0, "附件删除规则", true, true, FrmAttachmentAttr.DeleteWay, 
                //    "@0=不能删除@1=删除所有@2=只能删除自己上传的");
                map.AddBoolean(FrmAttachmentAttr.IsToHeLiuHZ, true, "该附件是否要汇总到合流节点上去？(对子线程节点有效)", true, true, true);
                map.AddBoolean(FrmAttachmentAttr.IsHeLiuHuiZong, true, "是否是合流节点的汇总附件组件？(对合流节点有效)", true, true, true);
                map.AddTBString(FrmAttachmentAttr.DataRefNoOfObj, "AttachM1", "对应附件标识", true, false, 0, 150, 20);
                map.SetHelperAlert("DataRefNoOfObj", "对WorkID权限模式有效,用于查询贯穿整个流程的附件标识,与从表的标识一样.");

                map.AddDDLSysEnum(FrmAttachmentAttr.ReadRole, 0, "阅读规则", true, true, FrmAttachmentAttr.ReadRole,
               "@0=不控制@1=未阅读阻止发送@2=未阅读做记录");
                #endregion 流程相关

                #region 其他属性。
                //参数属性.
                map.AddTBAtParas(3000);
                #endregion 其他属性。

                #region 基本功能.
                map.AddGroupMethod("基本功能");
                RefMethod rm = new RefMethod();
                //  rm.Icon = "/WF/Admin/CCFormDesigner/Img/Menu/CC.png";
                //rm.ClassMethodName = this.ToString() + ".DoAdv";
                // rm.RefMethodType = RefMethodType.RightFrameOpen;
                //  map.AddRefMethod(rm);
                rm = new RefMethod();
                rm.Title = "测试FTP服务器";
                rm.ClassMethodName = this.ToString() + ".DoTestFTPHost";
                rm.RefMethodType = RefMethodType.Func;
                rm.Icon = "icon-fire";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "重命名标记";
                rm.ClassMethodName = this.ToString() + ".ResetAthName";
                rm.HisAttrs.AddTBString("F", null, "命名后的标记", true, false, 0, 100, 50);
                rm.RefMethodType = RefMethodType.Func;
                rm.Icon = "icon-note";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Icon = "icon-credit-card";
                rm.Title = "分组属性"; // "设计表单";
                rm.ClassMethodName = this.ToString() + ".DoGroup";
                // rm.Icon = "../Img/AttachmentM.png";
                rm.Visable = true;
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Target = "_blank";
                map.AddRefMethod(rm);

                string msg = "说明：";
                msg += "\t\n 1. 每个附件都有一个标记比如，Ath1,Ath2, FJ. ";
                msg += "\t\n 2. 这个标记在一个表单中不能重复，这个标记也叫附件的小名。";
                msg += "\t\n 3. 在父子流程，或者多表单流程中，这个标记可以用于继承附件的展示。";
                msg += "\t\n 4. 比如：一个父流程的附件组件的标记为Ath1, 一个子流程的表单的附件表单要看到这个附件信息，就需要把两个小名保持一致。";
                rm.Warning = msg;
                map.AddRefMethod(rm);

                #endregion 基本功能.

                #region 高级设置.
                map.AddGroupMethod("实验中功能");
                rm = new RefMethod();

                rm.Title = "类别设置";
                rm.ClassMethodName = this.ToString() + ".DoSettingSort";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();

                rm.Title = "设置扩展列";
                rm.ClassMethodName = this.ToString() + ".DtlOfAth";
                rm.RefMethodType = RefMethodType.LinkeWinOpen;
                // map.AddRefMethod(rm);
                #endregion 高级设置.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 重命名表单标记.
        /// </summary>
        /// <param name="fname"></param>
        /// <returns></returns>
        public string ResetAthName(string fname)
        {
            //检查一下是否有重名的？
            FrmAttachments ens = new FrmAttachments();
            ens.Retrieve(FrmAttachmentAttr.FK_MapData, this.FK_MapData);
            foreach (FrmAttachment item in ens)
            {
                if (item.NoOfObj.Equals(fname) == true)
                    return "err@修改失败，该表单中已经存在该标记了.";
            }

            //修改模版.
            string myPKNew = this.FK_MapData + "_" + fname;
            string sql = "UPDATE Sys_FrmAttachment SET MyPK='" + myPKNew + "', NoOfObj='" + fname + "' WHERE MyPK='" + this.MyPK + "' ";
            DBAccess.RunSQL(sql);

            //修改分组信息，不然就丢失了.
            sql = "UPDATE Sys_GroupField SET CtrlID='" + myPKNew + "'  WHERE CtrlID='" + this.MyPK + "' ";
            DBAccess.RunSQL(sql);

            //修改：数据库.
            sql = "UPDATE Sys_FrmAttachmentDB SET NoOfObj='" + fname + "',FK_FrmAttachment='" + myPKNew + "'  WHERE FK_FrmAttachment='" + this.MyPK + "'";
            DBAccess.RunSQL(sql);

            return "执行成功: 您需要关闭表单设计器，然后重新进入。";
        }

        public string DtlOfAth()
        {
            string url = "../../Admin/FoolFormDesigner/MapDefDtlFreeFrm.htm?FK_MapDtl=" + this.MyPK + "&For=" + this.MyPK;
            return url;
        }


        /// <summary>
        /// 编辑分组属性
        /// </summary>
        /// <returns></returns>
        public string DoGroup()
        {
            GroupField gf = new GroupField();
            int i = gf.Retrieve(GroupFieldAttr.FrmID, this.FK_MapData, GroupFieldAttr.CtrlID, this.MyPK);
            if (i == 0)
            {
                gf.Lab = this.Name;
                gf.FrmID = this.FK_MapData;
                gf.CtrlType = "Dtl";
                gf.CtrlID = this.MyPK;
                gf.Idx = 10;
                gf.Insert();
            }
            string url = "../../Comm/EnOnly.htm?EnName=BP.Sys.GroupField&PKVal=" + gf.OID + "&Token=" + Web.WebUser.Token + "&AppCenterDBType=" + DBAccess.AppCenterDBType + "&CustomerNo=" + BP.Difference.SystemConfig.CustomerNo;
            return url;
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <returns></returns>
        public string DoTestFTPHost()
        {
            try
            {
                FtpConnection conn = new FtpConnection();
                conn.Connect(BP.Difference.SystemConfig.FTPServerIP, BP.Difference.SystemConfig.FTPServerPort, BP.Difference.SystemConfig.FTPUserNo, BP.Difference.SystemConfig.FTPUserPassword);
                return "连接成功.";
            }
            catch (Exception ex)
            {
                return "err@连接失败:" + ex.Message + " ,配置信息请参考,系统配置文件.";
            }
        }
        /// <summary>
        /// 固定模式类别设置
        /// </summary>
        /// <returns></returns>
        public string DoSettingSort()
        {
            return "../../Admin/FoolFormDesigner/AttachmentSortSetting.htm?FK_MapData=" + this.FK_MapData + "&MyPK=" + this.MyPK + "&Ath=" + this.NoOfObj;
        }
        /// <summary>
        /// 执行高级设置.
        /// </summary>
        /// <returns></returns>
        public string DoAdv()
        {
            return "/WF/Admin/FoolFormDesigner/Attachment.aspx?FK_MapData=" + this.FK_MapData + "&MyPK=" + this.MyPK + "&Ath=" + this.NoOfObj;
        }

        public bool IsUse = false;
        protected override bool beforeUpdateInsertAction()
        {
            if (this.FK_Node == 0)
            {
                //适应设计器新的规则 by dgq 
                if (!DataType.IsNullOrEmpty(this.NoOfObj) && this.NoOfObj.Contains(this.FK_MapData))
                    this.setMyPK(this.NoOfObj);
                else
                    this.setMyPK(this.FK_MapData + "_" + this.NoOfObj);
            }

            if (this.FK_Node != 0)
            {
                /*工作流程模式.*/
                if (this.HisCtrlWay == AthCtrlWay.PK)
                    this.HisCtrlWay = AthCtrlWay.WorkID;
                this.setMyPK(this.FK_MapData + "_" + this.NoOfObj + "_" + this.FK_Node);
            }

            if (this.HisCtrlWay != AthCtrlWay.WorkID)
                this.AthUploadWay = AthUploadWay.Interwork;


            //如果是pworkid. 就不让其删除或者上传. 
            if (this.HisCtrlWay == AthCtrlWay.PWorkID
                || this.HisCtrlWay == AthCtrlWay.PWorkID
                || this.HisCtrlWay == AthCtrlWay.P2WorkID
                || this.HisCtrlWay == AthCtrlWay.P3WorkID
                || this.HisCtrlWay == AthCtrlWay.RootFlowWorkID
                )
            {
                this.SetValByKey(FrmAttachmentAttr.DeleteWay, 0);
                this.SetValByKey(FrmAttachmentAttr.IsUpload, 0);
            }


            #region 处理分组.
            //更新相关的分组信息.
            if (this.IsVisable == true && this.FK_Node == 0)
            {
                GroupField gf = new GroupField();
                int i = gf.Retrieve(GroupFieldAttr.FrmID, this.FK_MapData, GroupFieldAttr.CtrlID, this.MyPK);
                if (i == 0)
                {
                    gf.Lab = this.Name;
                    gf.FrmID = this.FK_MapData;
                    gf.CtrlType = "Ath";
                    //gf.CtrlID = this.MyPK;
                    gf.Insert();
                }
                else
                {
                    gf.Lab = this.Name;
                    gf.FrmID = this.FK_MapData;
                    gf.CtrlType = "Ath";
                    //gf.CtrlID = this.MyPK;
                    gf.Update();
                }
            }

            //如果不显示.
            if (this.IsVisable == false)
            {
                GroupField gf = new GroupField();
                gf.Delete(GroupFieldAttr.FrmID, this.FK_MapData, GroupFieldAttr.CtrlID, this.MyPK);
            }
            #endregion 处理分组.


            return base.beforeUpdateInsertAction();
        }
        protected override bool beforeInsert()
        {
            //在属性实体集合插入前，clear父实体的缓存.
            BP.Sys.Base.Glo.ClearMapDataAutoNum(this.FK_MapData);


            if (this.FK_Node == 0)
                this.setMyPK(this.FK_MapData + "_" + this.NoOfObj);
            else
                this.setMyPK(this.FK_MapData + "_" + this.NoOfObj + "_" + this.FK_Node);

            return base.beforeInsert();
        }
        /// <summary>
        /// 插入之后
        /// </summary>
        protected override void afterInsert()
        {
            GroupField gf = new GroupField();
            if (this.FK_Node == 0 && gf.IsExit(GroupFieldAttr.CtrlID, this.MyPK) == false)
            {
                gf.FrmID = this.FK_MapData;
                gf.CtrlID = this.MyPK;
                gf.CtrlType = "Ath";
                gf.Lab = this.Name;
                gf.Idx = 0;
                gf.Insert(); //插入.

            }
            base.afterInsert();
        }

        protected override void afterInsertUpdateAction()
        {
            FrmAttachment ath = new FrmAttachment();
            ath.setMyPK(this.MyPK);
            ath.RetrieveFromDBSources();
            ath.IsToHeLiuHZ = this.IsToHeLiuHZ;
            ath.IsHeLiuHuiZong = this.IsHeLiuHuiZong;

            //强制设置,保存到ftp服务器上.
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                ath.AthSaveWay = AthSaveWay.FTPServer;

            ath.Update();


            //判断是否是字段附件
            MapAttr mapAttr = new MapAttr();
            mapAttr.setMyPK(this.MyPK);
            if (mapAttr.RetrieveFromDBSources() != 0)
            {
                mapAttr.UIHeight = this.H;
                mapAttr.Name = this.Name;
                mapAttr.Update();
            }

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            base.afterInsertUpdateAction();
        }

        /// <summary>
        /// 删除之后.
        /// </summary>
        protected override void afterDelete()
        {
            GroupFields gfs = new GroupFields();
            //   gfs.RetrieveByLike(GroupFieldAttr.CtrlID, this.MyPK + "%");
            //  gfs.Delete();
            gfs.Delete(GroupFieldAttr.CtrlID, this.MyPK);

            //把相关的字段也要删除.
            MapAttrString attr = new MapAttrString();
            attr.setMyPK(this.MyPK);
            if (attr.RetrieveFromDBSources() != 0)
                attr.Delete();

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            base.afterDelete();
        }
    }
    /// <summary>
    /// 附件s
    /// </summary>
    public class FrmAttachmentExts : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 附件s
        /// </summary>
        public FrmAttachmentExts()
        {
        }
        /// <summary>
        /// 附件s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmAttachmentExts(string fk_mapdata)
        {
            this.Retrieve(FrmAttachmentAttr.FK_MapData, fk_mapdata, FrmAttachmentAttr.FK_Node, 0);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmAttachmentExt();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmAttachmentExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmAttachmentExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmAttachmentExt> Tolist()
        {
            System.Collections.Generic.List<FrmAttachmentExt> list = new System.Collections.Generic.List<FrmAttachmentExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmAttachmentExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
