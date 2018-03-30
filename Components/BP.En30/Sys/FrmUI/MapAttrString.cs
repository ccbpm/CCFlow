using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using System.Web;

namespace BP.Sys.FrmUI
{
    /// <summary>
    /// 实体属性
    /// </summary>
    public class MapAttrString : EntityMyPK
    {
        #region 文本字段参数属性.
        public bool IsSupperText
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.IsSupperText);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.IsSupperText, value);
            }
        }
        public bool IsRichText
        {
            get
            {
                return this.GetValBooleanByKey(MapAttrAttr.IsRichText);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.IsRichText, value);
            }
        }
        /// <summary>
        /// 表单ID
        /// </summary>
        public string FK_MapData
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.FK_MapData, value);
            }
        }
        /// <summary>
        /// 字段
        /// </summary>
        public string KeyOfEn
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.KeyOfEn);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.KeyOfEn, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 控制权限
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsInsert = false;
                uac.IsUpdate = true;
                uac.IsDelete = true;
                return uac;
            }
        }
        /// <summary>
        /// 实体属性
        /// </summary>
        public MapAttrString()
        {
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

                Map map = new Map("Sys_MapAttr", "文本字段");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                #region 基本字段信息.
                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主键", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "实体标识", false, false, 1, 100, 20);

                map.AddTBString(MapAttrAttr.Name, null, "字段中文名", true, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段名", true, true, 1, 200, 20);

                //默认值.
                string sql = "SELECT No,Name FROM Sys_GloVar WHERE GroupKey='DefVal'";
                //显示的分组.
                map.AddDDLSQL("ExtDefVal", "0", "系统默认值", sql, true);

                map.AddTBString(MapAttrAttr.DefVal, null, "默认值表达式", true, false, 0, 400, 20);

                map.AddTBFloat(MapAttrAttr.MinLen, 0, "最小长度", true, false);
                map.AddTBFloat(MapAttrAttr.MaxLen, 50, "最大长度", true, false);

                map.AddTBFloat(MapAttrAttr.UIWidth, 100, "宽度", true, false);
                map.AddTBFloat(MapAttrAttr.UIHeight, 23, "高度", true, false);
                //map.AddTBFloat("ExtRows", 1, "文本框行数(决定高度)", true, false);

                map.AddBoolean(MapAttrAttr.UIVisible, true, "是否可见？", true, true);
                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "是否可编辑？", true, true);
                map.AddBoolean(MapAttrAttr.UIIsInput, false, "是否必填项？", true, true);
                map.AddBoolean(MapAttrAttr.IsRichText, false, "是否富文本？", true, true);
                map.AddBoolean(MapAttrAttr.IsSupperText, false, "是否大块文本？(是否该字段存放的超长字节字段)", true, true,true);
                map.AddTBString(MapAttrAttr.Tip, null, "激活提示", true, false, 0, 400, 20, true);

                #endregion 基本字段信息.

                #region 傻瓜表单
                //单元格数量 2013-07-24 增加
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "单元格数量", true, true, "ColSpanAttrString", 
                    "@1=跨1个单元格@3=跨3个单元格@4=跨4个单元格");

                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, "0", "显示的分组",
                    "SELECT OID as No, Lab as Name FROM Sys_GroupField WHERE EnName='@FK_MapData'  AND (CtrlType IS NULL OR CtrlType='')  ", true);

                map.AddDDLSysEnum(MapAttrAttr.IsSigan, 0, "签名模式", true, true,
                    MapAttrAttr.IsSigan, "@0=无@1=图片签名@2=山东CA@3=广东CA");
                #endregion 傻瓜表单

                #region 基本功能.
                RefMethod rm = new RefMethod();

                rm = new RefMethod();
                rm.Title = "正则表达式";
                rm.ClassMethodName = this.ToString() + ".DoRegularExpression()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "文本框自动完成";
                rm.ClassMethodName = this.ToString() + ".DoTBFullCtrl()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "脚本验证";
                rm.ClassMethodName = this.ToString() + ".DoInputCheck()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "事件绑函数";
                rm.ClassMethodName = this.ToString() + ".BindFunction()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "超链接";
                rm.ClassMethodName = this.ToString() + ".DoLink()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);
                #endregion 基本功能.

                #region 输入多选.
                rm = new RefMethod();
                rm.GroupName = "输入内容多选";
                rm.Title = "小范围多选(combox)";
                rm.ClassMethodName = this.ToString() + ".DoMultipleChoiceSmall()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "输入内容多选";
                rm.Title = "搜索多选";
                rm.ClassMethodName = this.ToString() + ".DoMultipleChoiceSearch()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);
                #endregion 输入多选

                #region Pop 自动填充.
                rm = new RefMethod();
                rm.GroupName = "Pop自动填充";
                rm.Title = "简单列表模式";
                rm.ClassMethodName = this.ToString() + ".DoPopFullCtrl()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "Pop自动填充";
                rm.Title = "多条件查询列表模式";
                rm.ClassMethodName = this.ToString() + ".DoPopFullCtrlAdv()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);
                #endregion Pop 自动填充.


                #region Pop返回值.
                rm = new RefMethod();
                rm.GroupName = "Pop返回值2018";
                rm.Title = "枝干叶子模式";
                rm.ClassMethodName = this.ToString() + ".DoBranchesAndLeaf()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "Pop返回值2018";
                rm.Title = "枝干叶子模式-懒加载";
                rm.ClassMethodName = this.ToString() + ".DoBranchesAndLeafLazyLoad()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "Pop返回值2018";
                rm.Title = "枝干模式(简单)";
                rm.ClassMethodName = this.ToString() + ".DoBranches()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "Pop返回值2018";
                rm.Title = "枝干模式(简单)-懒加载";
                rm.ClassMethodName = this.ToString() + ".DoBranchesLazyLoad()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "Pop返回值2018";
                rm.Title = "分组列表平铺";
                rm.ClassMethodName = this.ToString() + ".DoGroupList()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "Pop返回值2018";
                rm.Title = "单实体平铺";
                rm.ClassMethodName = this.ToString() + ".DoTableList()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.GroupName = "Pop返回值2018";
                rm.Title = "表格条件查询";
                rm.ClassMethodName = this.ToString() + ".DoTableSearch()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                rm = new RefMethod();
                rm.GroupName = "Pop返回值2018";
                rm.Title = "自定义URL";
                rm.ClassMethodName = this.ToString() + ".DoSelfUrl()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);


                //  设置开窗返回值-正则表达式-文本框自动完成-脚本验证-扩展控件
                rm = new RefMethod();
                rm.GroupName = "Pop返回值2018";
                rm.Title = "设置开窗返回值(旧版本)";
                rm.ClassMethodName = this.ToString() + ".DoPopVal()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);
                #endregion

                #region 高级设置.
                rm = new RefMethod();
                rm.Title = "扩展控件";
                rm.ClassMethodName = this.ToString() + ".DoEditFExtContral()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "高级设置";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "批处理";
                rm.ClassMethodName = this.ToString() + ".DoEleBatch()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                rm.GroupName = "高级设置";
                map.AddRefMethod(rm);
                #endregion 执行的方法.

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 删除
        /// </summary>
        protected override void afterDelete()
        {
            //删除可能存在的关联属性.
            string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + this.FK_MapData + "' AND KeyOfEn='" + this.KeyOfEn + "T'";
            DBAccess.RunSQL(sql);

            base.afterDelete();
        }
        #endregion

        #region 基本功能.
        /// <summary>
        /// 绑定函数
        /// </summary>
        /// <returns></returns>
        public string BindFunction()
        {
            return "../../Admin/FoolFormDesigner/MapExt/BindFunction.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        #endregion

        #region 方法执行 Pop自动完成.
        /// <summary>
        /// 简单列表模式
        /// </summary>
        /// <returns></returns>
        public string DoPopFullCtrl()
        {
            return "../../Admin/FoolFormDesigner/MapExt/PopFullCtrl.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=TBFullCtrl_" + HttpUtility.UrlEncode(this.MyPK);
        }
        /// <summary>
        /// 多条件查询列表模式
        /// </summary>
        /// <returns></returns>
        public string DoPopFullCtrlAdv()
        {
            return "../../Admin/FoolFormDesigner/MapExt/PopFullCtrl.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=TBFullCtrl_" + HttpUtility.UrlEncode(this.MyPK);
        }
        #endregion 方法执行 Pop填充自动完成.
        
        #region 方法执行 pop返回值
        /// <summary>
        /// 自定义Url.
        /// </summary>
        /// <returns></returns>
        public string DoSelfUrl()
        {
            return "../../Admin/FoolFormDesigner/PopSetting/SelfUrl.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 实体列表模式
        /// </summary>
        /// <returns>返回url</returns>
        public string DoTableList()
        {
            return "../../Admin/FoolFormDesigner/PopSetting/TableList.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 枝干叶子模式
        /// </summary>
        /// <returns>返回url</returns>
        public string DoBranchesAndLeaf()
        {
            return "../../Admin/FoolFormDesigner/PopSetting/BranchesAndLeaf.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 枝干叶子懒加载
        /// </summary>
        /// <returns></returns>
        public string DoBranchesAndLeafLazyLoad()
        {
            return "../../Admin/FoolFormDesigner/PopSetting/BranchesAndLeafLazyLoad.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&IsLazyLoad=1";
        }
        /// <summary>
        /// 枝干模式
        /// </summary>
        /// <returns>返回url</returns>
        public string DoBranches()
        {
            return "../../Admin/FoolFormDesigner/PopSetting/Branches.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 枝干模式懒加载
        /// </summary>
        /// <returns>返回url</returns>
        public string DoBranchesLazyLoad()
        {
            return "../../Admin/FoolFormDesigner/PopSetting/Branches.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 分组模式
        /// </summary>
        /// <returns></returns>
        public string DoGroupList()
        {
            return "../../Admin/FoolFormDesigner/PopSetting/GroupList.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 实体查询模式
        /// </summary>
        /// <returns></returns>
        public string DoTableSearch()
        {
            return "../../Admin/FoolFormDesigner/PopSetting/TableSearch.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        #endregion

        #region 方法执行.
        /// <summary>
        /// 批处理
        /// </summary>
        /// <returns></returns>
        public string DoEleBatch()
        {
            return "../../Admin/FoolFormDesigner/EleBatch.aspx?EleType=MapAttr&KeyOfEn=" + this.KeyOfEn + "&FType=1&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData;
        }
        public string DoOldVerAspx()
        {
            return "../../Admin/FoolFormDesigner/EditF.aspx?DoType=Edit&KeyOfEn=" + this.KeyOfEn + "&FType=1&MyPK=" + this.MyPK + "&FK_MapData=" + this.FK_MapData;
        }
        /// <summary>
        /// 小范围多选
        /// </summary>
        /// <returns></returns>
        public string DoMultipleChoiceSmall()
        {
            return "../../Admin/FoolFormDesigner/MapExt/MultipleChoiceSmall.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&m=s";
        }
        /// <summary>
        /// 大范围多选
        /// </summary>
        /// <returns></returns>
        public string DoMultipleChoiceSearch()
        {
            return "../../Admin/FoolFormDesigner/MapExt/MultipleChoiceSearch.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&m=s";
        }
        /// <summary>
        /// 超链接
        /// </summary>
        /// <returns></returns>
        public string DoLink()
        {
            return "../../Admin/FoolFormDesigner/MapExt/Link.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + HttpUtility.UrlEncode(this.MyPK) + "&FK_MapExt=Link_" + this.FK_MapData + "_" + this.KeyOfEn;
        }
        /// <summary>
        /// 设置开窗返回值
        /// </summary>
        /// <returns></returns>
        public string DoPopVal()
        {
            return "../../Admin/FoolFormDesigner/MapExt/PopVal.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + HttpUtility.UrlEncode(this.MyPK) + "&FK_MapExt=PopVal_" + this.FK_MapData + "_" + this.KeyOfEn;
        }
        /// <summary>
        /// 正则表达式
        /// </summary>
        /// <returns></returns>
        public string DoRegularExpression()
        {
            return "../../Admin/FoolFormDesigner/MapExt/RegularExpression.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + HttpUtility.UrlEncode(this.MyPK);
        }
        /// <summary>
        /// 文本框自动完成
        /// </summary>
        /// <returns></returns>
        public string DoTBFullCtrl()
        {
            return "../../Admin/FoolFormDesigner/MapExt/TBFullCtrl.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=TBFullCtrl_" + HttpUtility.UrlEncode(this.MyPK);
        }
        

        /// <summary>
        /// 设置级联
        /// </summary>
        /// <returns></returns>
        public string DoInputCheck()
        {
            return "../../Admin/FoolFormDesigner/MapExt/InputCheck.aspx?FK_MapData=" + this.FK_MapData + "&OperAttrKey=" + this.KeyOfEn + "&RefNo=" + HttpUtility.UrlEncode(this.MyPK) + "&DoType=New&ExtType=InputCheck";
            // return "../../Admin/FoolFormDesigner/MapExt/InputCheck.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn  +"&RefNo="+this.MyPK;
            //  return "../../Admin/FoolFormDesigner/MapExt/InputCheck.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }
        /// <summary>
        /// 扩展控件
        /// </summary>
        /// <returns></returns>
        public string DoEditFExtContral()
        {
            return "../../Admin/FoolFormDesigner/EditFExtContral.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn + "&MyPK=" + HttpUtility.UrlEncode(this.MyPK);
            //  return "../../Admin/FoolFormDesigner/MapExt/InputCheck.aspx?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + this.KeyOfEn + "&RefNo=" + this.MyPK;
        }
        #endregion 方法执行.

        #region 重载.
        protected override bool beforeUpdateInsertAction()
        {
            MapAttr attr = new MapAttr();
            attr.MyPK = this.MyPK;
            attr.RetrieveFromDBSources();

            //高度.
            //  attr.UIHeightInt = this.GetValIntByKey("ExtRows") * 23;

            attr.IsRichText = this.GetValBooleanByKey(MapAttrAttr.IsRichText); //是否是富文本？
            attr.IsSupperText = this.GetValBooleanByKey(MapAttrAttr.IsSupperText); //是否是大块文本？

            if (attr.IsRichText || attr.IsSupperText)
            {
                attr.MaxLen = 4000;
                this.SetValByKey(MapAttrAttr.MaxLen, 4000);
            }


            #region 自动扩展字段长度.  @杜. 需要翻译.
            if (attr.MaxLen < 4000)
            {
                string sql = "";

                if (SystemConfig.AppCenterDBType == DBType.MSSQL)
                    sql = "ALTER TABLE " + this.EnMap.PhysicsTable + " ALTER column " + this.KeyOfEn + " NVARCHAR(" + attr.MaxLen + ")";

                if (SystemConfig.AppCenterDBType == DBType.MySQL)
                    sql = "alter table " + this._enMap.PhysicsTableExt + " modify " + attr.Field + " NVARCHAR(" + attr.MaxLen + ")";

                if (SystemConfig.AppCenterDBType == DBType.MySQL)
                    sql = "alter table " + this.EnMap.PhysicsTableExt + " modify " + attr.Field + " varchar2(" + attr.MaxLen + ")";
                DBAccess.RunSQL(sql);
            }
            #endregion 自动扩展字段长度.


            //默认值.
            string defval = this.GetValStrByKey("ExtDefVal");
            if (defval == "" || defval == "0")
            {
                string defVal = this.GetValStrByKey("DefVal");
                if (defval.Contains("@") == true)
                    this.SetValByKey("DefVal", "");
            }
            else
            {
                this.SetValByKey("DefVal", this.GetValByKey("ExtDefVal"));
            }

            //执行保存.
            attr.Save();

            if (this.GetValStrByKey("GroupID") == "无")
                this.SetValByKey("GroupID", "0");

            return base.beforeUpdateInsertAction();
        }
        #endregion
    }
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrStrings : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrStrings()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrString();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrString> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrString>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrString> Tolist()
        {
            System.Collections.Generic.List<MapAttrString> list = new System.Collections.Generic.List<MapAttrString>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrString)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
