using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 字段单附件
    /// </summary>
    public class FrmAttachmentSingle : EntityMyPK
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
        /// 字段单附件类型
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
        /// 使用上传字段单附件的 - 控件类型
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


        public int AthSingleRole
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.AthSingleRole);
            }
        }

        public int AthEditModel
        {
            get
            {
                return this.GetValIntByKey(FrmAttachmentAttr.AthEditModel);
            }
        }

        public string SaveTo
        {
            get
            {
              return SystemConfig.PathOfDataUser + "UploadFile/" + this.FK_MapData + "/";
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
                    return "多字段单附件";
                if (this.UploadType == AttachmentUploadType.Single)
                    return "单字段单附件";
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
        /// 字段单附件删除方式
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
        /// 字段单附件名称
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
        /// 字段单附件标识
        /// </summary>
        public string NoOfObj
        {
            get
            {
                return this.GetValStringByKey(FrmAttachmentAttr.NoOfObj);
            }

        }
        /// <summary>
        /// Y
        /// </summary>
        public float Y
        {
            get
            {
                return this.GetValFloatByKey(FrmAttachmentAttr.Y);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.Y, value);
            }
        }
        /// <summary>
        /// X
        /// </summary>
        public float X
        {
            get
            {
                return this.GetValFloatByKey(FrmAttachmentAttr.X);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.X, value);
            }
        }
        /// <summary>
        /// W
        /// </summary>
        public float W
        {
            get
            {
                return this.GetValFloatByKey(FrmAttachmentAttr.W);
            }
            set
            {
                this.SetValByKey(FrmAttachmentAttr.W, value);
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
        /// 是否是合流汇总多字段单附件？
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
        /// 该字段单附件是否汇总到合流节点上去？
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
        /// 字段单附件
        /// </summary>
        public FrmAttachmentSingle()
        {
        }
        /// <summary>
        /// 字段单附件
        /// </summary>
        /// <param name="mypk">主键</param>
        public FrmAttachmentSingle(string mypk)
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

                Map map = new Map("Sys_FrmAttachment", "字段单附件");
                map.IndexField = MapAttrAttr.FK_MapData;

                map.AddMyPK();

                #region 基本属性。
                map.AddTBString(FrmAttachmentAttr.FK_MapData, null, "表单ID", true, true, 1, 100, 20);
                map.AddTBString(FrmAttachmentAttr.NoOfObj, null, "附件标识", true, true, 0, 50, 20);
                map.AddTBInt(FrmAttachmentAttr.FK_Node, 0, "节点控制(对sln有效)", false, false);

                map.AddTBString(FrmAttachmentAttr.Name, null, "附件名称", true, false, 0, 50, 20, false);

                map.AddTBString(FrmAttachmentAttr.Exts, null, "文件格式", true, false, 0, 50, 20, false, null);
                map.SetHelperAlert(FrmAttachmentAttr.Exts, "上传要求,设置模式为: *.*, *.doc, *.docx, *.png,多个中间用逗号分开.\t\n表示仅仅允许上传指定的后缀的文件.");

                map.AddBoolean(FrmAttachmentAttr.NumOfUpload, false, "是否必填？", true, true);
                // map.SetHelperAlert("NumOfUpload", "如果为0则标识必须上传. \t\n用户上传的字段单附件数量低于指定的数量就不让保存.");

                map.AddTBInt(FrmAttachmentAttr.FileMaxSize, 10240, "附件最大限制(KB)", true, false);

                map.AddDDLSysEnum(FrmAttachmentAttr.AthSaveWay, 0, "保存方式", true, true, FrmAttachmentAttr.AthSaveWay,
                  "@0=保存到web服务器@1=保存到数据库@2=ftp服务器");


                map.AddDDLSysEnum(FrmAttachmentAttr.AthSingleRole, 0, "模板规则", true, true, FrmAttachmentAttr.AthSingleRole,
                 "@0=不使用模板@1=使用上传模板@2=使用上传模板自动加载数据标签");
                map.SetHelperAlert(FrmAttachmentAttr.AthSingleRole, "单附件模板使用规则，如果启用，您需要上传wps/word模板。");

                map.AddDDLSysEnum(FrmAttachmentAttr.AthEditModel, 0, "在线编辑模式", true, true, FrmAttachmentAttr.AthEditModel,
                 "@0=只读@1=可编辑全部区域@2=可编辑非数据标签区域");
                map.SetHelperAlert(FrmAttachmentAttr.AthEditModel, "用于控制附件的在线编辑模式");

             
                #endregion 基本属性。

                #region 权限控制。
                map.AddBoolean(FrmAttachmentAttr.DeleteWay, true, "是否可以删除？", true, true);

                //map.AddDDLSysEnum(FrmAttachmentAttr.DeleteWay, 1, "删除规则", true, true, FrmAttachmentAttr.DeleteWay,
                //    "@0=不能删除@1=删除所有@2=只能删除自己上传的");

                map.AddBoolean(FrmAttachmentAttr.IsUpload, true, "是否可以上传", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsDownload, true, "是否可以下载", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsNote, true, "是否增加备注", true, true);

                map.AddDDLSysEnum(FrmAttachmentAttr.AthUploadWay, 0, "控制上传控制方式", true, true, FrmAttachmentAttr.AthUploadWay, "@0=继承模式@1=协作模式");

                map.AddDDLSysEnum(FrmAttachmentAttr.CtrlWay, 0, "控制呈现控制方式", true, true, "Ath" + FrmAttachmentAttr.CtrlWay,
                    "@0=PK-主键@1=FID-干流程ID@2=PWorkID-父流程ID@3=仅能查看自己上传的字段单附件@4=WorkID-按照WorkID计算(对流程节点表单有效)@5=P2WorkID@6=P3WorkID");
                #endregion 权限控制。

                #region 节点相关
                map.AddBoolean(FrmAttachmentAttr.IsToHeLiuHZ, true, "该字段单附件是否要汇总到合流节点上去？(对子线程节点有效)", true, true, true);
                map.AddBoolean(FrmAttachmentAttr.IsHeLiuHuiZong, true, "是否是合流节点的汇总字段单附件组件？(对合流节点有效)", true, true, true);
                map.AddTBString(FrmAttachmentAttr.DataRefNoOfObj, "AttachM1", "对应字段单附件标识", true, false, 0, 150, 20);
                map.SetHelperAlert("DataRefNoOfObj", "对WorkID权限模式有效,用于查询贯穿整个流程的字段单附件标识,与从表的标识一样.");
                map.AddDDLSysEnum(FrmAttachmentAttr.ReadRole, 0, "阅读规则", true, true, FrmAttachmentAttr.ReadRole,
               "@0=不控制@1=未阅读阻止发送@2=未阅读做记录");
                #endregion 节点相关

                #region 其他属性。
                //  map.AddBoolean(FrmAttachmentAttr.IsIdx, false, "是否排序?", true, true);
                map.AddBoolean(FrmAttachmentAttr.IsTurn2Html, false, "是否转换成html(方便手机浏览)", true, true, true);

                //参数属性.
                map.AddTBAtParas(3000);
                //隐藏字段.
                map.AddTBInt(FrmAttachmentAttr.UploadType, 0, "0单附件1多附件", false, false);
                map.AddTBInt(FrmAttachmentAttr.IsVisable, 0, "是否可见?", false, false);

                #endregion 其他属性。

                #region 基本配置.
                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "上传wps模板";
                rm.ClassMethodName = this.ToString() + ".DoUploadTemplateWPS";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-fire";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "上传word模板";
                rm.ClassMethodName = this.ToString() + ".DoUploadTemplateWord";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.Icon = "icon-fire";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "测试FTP服务器";
                rm.ClassMethodName = this.ToString() + ".DoTestFTPHost";
                rm.RefMethodType = RefMethodType.Func;
                rm.Icon = "icon-fire";
                map.AddRefMethod(rm);
                #endregion 基本配置.

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 基本方法.
        public string DoUploadTemplateWPS()
        {
            return "../../Admin/FoolFormDesigner/Template/FrmAttachmentSingle/UploadAthTemplateWPS.htm?FrmID=" + this.FK_MapData + "&MyPK=" + this.MyPK;
        }
        public string DoUploadTemplateWord()
        {
            return "../../Admin/FoolFormDesigner/Template/FrmAttachmentSingle/UploadAthTemplateWord.htm?FrmID=" + this.FK_MapData + "&MyPK=" + this.MyPK;
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
                conn.Connect(SystemConfig.FTPServerIP, SystemConfig.FTPServerPort, SystemConfig.FTPUserNo, SystemConfig.FTPUserPassword);
                return "连接成功.";
            }
            catch (Exception ex)
            {
                return "err@连接失败:" + ex.Message + " ,配置信息请参考,系统配置文件.";
            }
        }

        public bool IsUse = false;
        #endregion 基本方法.

        #region 重写的方法.

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

            GroupField gf = new GroupField();
            gf.Delete(GroupFieldAttr.FrmID, this.FK_MapData, GroupFieldAttr.CtrlID, this.MyPK);

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
            if (SystemConfig.CCBPMRunModel == CCBPMRunModel.SAAS)
                ath.AthSaveWay = AthSaveWay.FTPServer;

            ath.Update();

            //判断是否是字段字段单附件
            MapAttr mapAttr = new MapAttr();
            mapAttr.setMyPK(this.MyPK);
            if (mapAttr.RetrieveFromDBSources() != 0 && mapAttr.Name.Equals(this.Name) == false)
            {
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
            //删除.
            MapAttr attr = new MapAttr();
            attr.setMyPK(this.MyPK);
            attr.Delete();

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);
            base.afterDelete();
        }
        #endregion 重写的方法.

    }
    /// <summary>
    /// 字段单附件s
    /// </summary>
    public class FrmAttachmentSingles : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 字段单附件s
        /// </summary>
        public FrmAttachmentSingles()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmAttachmentSingle();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmAttachmentSingle> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmAttachmentSingle>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmAttachmentSingle> Tolist()
        {
            System.Collections.Generic.List<FrmAttachmentSingle> list = new System.Collections.Generic.List<FrmAttachmentSingle>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmAttachmentSingle)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
