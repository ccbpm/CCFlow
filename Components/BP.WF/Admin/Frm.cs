using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Port;
using BP.En;
using BP.Web;
using BP.Sys;
using BP.WF.Data;
using BP.WF.Template.Frm;


namespace BP.WF.Admin
{
    public class FrmAttr : BP.Sys.MapDataAttr
    {

    }
    /// <summary>
    /// 流程
    /// </summary>
    public class Frm : EntityNoName
    {
        #region 属性.
        /// <summary>
        /// 存储表
        /// </summary>
        public string PTable
        {
            get
            {
                return this.GetValStringByKey(FrmAttr.PTable);
            }
            set
            {
                this.SetValByKey(FrmAttr.PTable, value);
            }
        }
        #endregion 属性.

        #region 构造方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (WebUser.IsAdmin == false)
                    throw new Exception("err@管理员登录用户信息丢失,当前会话[" + WebUser.No + "," + WebUser.Name + "]");
                uac.IsUpdate = true;
                uac.IsDelete = false;
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 流程
        /// </summary>
        public Frm()
        {
        }
        /// <summary>
        /// 重写基类方法
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;


                Map map = new Map("Sys_MapData", "傻瓜表单属性");

                #region 基本属性.
                map.AddTBStringPK(MapDataAttr.No, null, "表单编号", true, true, 1, 190, 20);
                if (BP.WF.Glo.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                    map.AddTBString(MapDataAttr.PTable, null, "存储表", false, false, 0, 100, 20);
                }
                else
                {
                    map.AddTBString(MapDataAttr.PTable, null, "存储表", true, false, 0, 100, 20);
                    string msg = "提示:";
                    msg += "\t\n1. 该表单把数据存储到那个表里.";
                    msg += "\t\n2. 该表必须有一个int64未的OID列作为主键..";
                    msg += "\t\n3. 如果指定了一个不存在的表,系统就会自动创建上.";
                    map.SetHelperAlert(MapDataAttr.PTable, msg);
                }

                map.AddTBString(MapDataAttr.Name, null, "名称", true, false, 0, 500, 20, true);
                map.AddTBInt(MapDataAttr.TableCol, 0, "显示列数", false, false);
                map.AddTBInt(MapDataAttr.FrmW, 900, "表单宽度", true, false);

                if (BP.WF.Glo.CCBPMRunModel == CCBPMRunModel.SAAS)
                {
                }
                else
                {
                    map.AddTBString(MapDataAttr.DBSrc, null, "数据源", false, false, 0, 500, 20);
                    map.AddDDLEntities(MapDataAttr.FK_FormTree, "01", "目录", new FrmSorts(), true);
                }

                //表单的运行类型.
                map.AddDDLSysEnum(MapDataAttr.FrmType, (int)BP.Sys.FrmType.FoolForm, "表单类型",
                    true, true, MapDataAttr.FrmType);

                //表单解析 0 普通 1 页签展示
                map.AddDDLSysEnum(MapDataAttr.FrmShowType, 0, "表单展示方式", true, true, "表单展示方式",
                    "@0=普通方式@1=页签方式");


                map.AddTBString(MapDataAttr.Icon, "icon-doc", "图标", true, false, 0, 100, 100);

                map.AddBoolean("IsEnableJs", false, "是否启用自定义js函数？", true, true, true);
                #endregion 基本属性.

                #region 设计者信息.
                map.AddTBString(MapDataAttr.Designer, null, "设计者", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerContact, null, "联系方式", true, false, 0, 500, 20);
                map.AddTBString(MapDataAttr.DesignerUnit, null, "单位", true, false, 0, 500, 20, true);
                map.AddTBString(MapDataAttr.GUID, null, "GUID", true, true, 0, 128, 20, false);
                map.AddTBString(MapDataAttr.Ver, null, "版本号", true, true, 0, 30, 20);
                map.AddTBString(MapDataAttr.Note, null, "备注", true, false, 0, 400, 100, true);
                //增加参数字段.
                map.AddTBAtParas(4000);
                map.AddTBInt(MapDataAttr.Idx, 100, "顺序号", false, false);
                #endregion 设计者信息.

                map.AddSearchAttr(MapDataAttr.FK_FormTree);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 流程集合
    /// </summary>
    public class Frms : EntitiesNoName
    {

        #region 构造方法
        /// <summary>
        /// 工作流程
        /// </summary>
        public Frms() { }
        #endregion

        #region 得到实体
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Frm();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Frm> ToJavaList()
        {
            return (System.Collections.Generic.IList<Frm>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Frm> Tolist()
        {
            System.Collections.Generic.List<Frm> list = new System.Collections.Generic.List<Frm>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Frm)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}

