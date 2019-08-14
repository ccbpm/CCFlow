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
    /// 日期字段
    /// </summary>
    public class MapAttrDT : EntityMyPK
    {
        #region 文本字段参数属性.
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
        /// <summary>
        /// 绑定的枚举ID
        /// </summary>
        public string UIBindKey
        {
            get
            {
                return this.GetValStringByKey(MapAttrAttr.UIBindKey);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.UIBindKey, value);
            }
        }
        /// <summary>
        /// 数据类型
        /// </summary>
        public int MyDataType
        {
            get
            {
                return this.GetValIntByKey(MapAttrAttr.MyDataType);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.MyDataType, value);
            }
        }
        public int Format
        {
            get
            {
                return this.GetValIntByKey(MapAttrAttr.IsSupperText);
            }
            set
            {
                this.SetValByKey(MapAttrAttr.IsSupperText, value);
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
        /// 日期字段
        /// </summary>
        public MapAttrDT()
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

                Map map = new Map("Sys_MapAttr", "日期字段");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap(Depositary.Application);
                map.Java_SetEnType(EnType.Sys);
                map.IndexField = MapAttrAttr.FK_MapData;


                #region 基本信息.
                map.AddTBStringPK(MapAttrAttr.MyPK, null, "主键", false, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.FK_MapData, null, "实体标识", false, false, 1, 100, 20);

                map.AddTBString(MapAttrAttr.Name, null, "字段中文名", true, false, 0, 200, 20);
                map.AddTBString(MapAttrAttr.KeyOfEn, null, "字段名", true, true, 1, 200, 20);

                map.AddDDLSysEnum(MapAttrAttr.MyDataType, 6, "数据类型", true, false);

                map.AddTBString(MapAttrAttr.DefVal, null, "默认值(@RDT为当前日期)", true, false, 0, 100, 20);

                map.AddBoolean(MapAttrAttr.UIVisible, true, "是否可见？", true, true);
                map.AddBoolean(MapAttrAttr.UIIsEnable, true, "是否可编辑？", true, true);
                map.AddBoolean(MapAttrAttr.UIIsInput, false, "是否必填项？", true, true);

                map.AddDDLSysEnum(MapAttrAttr.IsSupperText, 2, "格式", true, true, MapAttrAttr.IsSupperText,
                    "@0=yyyy-MM-dd@1=yyyy-MM-dd HH:mm@2=yyyy-MM-dd HH:mm:ss@3=yyyy-MM@4=HH:mm@5=HH:mm:ss@6=MM-dd");

                map.AddTBString(MapAttrAttr.Tip, null, "激活提示", true, false, 0, 400, 20, true);

                #endregion 基本信息.

                #region 傻瓜表单
                map.AddDDLSysEnum(MapAttrAttr.ColSpan, 1, "单元格数量", true, true, "ColSpanAttrDT",
                  "@0=跨0个单元格@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");

                //文本占单元格数量
                map.AddDDLSysEnum(MapAttrAttr.TextColSpan, 1, "文本单元格数量", true, true, "ColSpanAttrString",
                    "@1=跨1个单元格@2=跨2个单元格@3=跨3个单元格@4=跨4个单元格");

                //文本跨行
                map.AddTBInt(MapAttrAttr.RowSpan, 1, "行数", true, false);
                //显示的分组.
                map.AddDDLSQL(MapAttrAttr.GroupID, "0", "显示的分组", MapAttrString.SQLOfGroupAttr, true);
                map.AddTBInt(MapAttrAttr.Idx, 0, "顺序号", true, false); //@李国文

                #endregion 傻瓜表单。

                #region 执行的方法.
                RefMethod rm = new RefMethod();

                //rm = new RefMethod();
                //rm.Title = "自动计算";
                //rm.ClassMethodName = this.ToString() + ".DoAutoFull()";
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "正则表达式";
                rm.ClassMethodName = this.ToString() + ".DoRegularExpression()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "事件绑函数";
                rm.ClassMethodName = this.ToString() + ".BindFunction()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "日期输入限制";
                rm.ClassMethodName = this.ToString() + ".DataFieldInputRole()";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                #endregion 执行的方法.

                this._enMap = map;
                return this._enMap;
            }
        }

        protected override bool beforeInsert()
        {
            if (this.Format == 0 && this.MyDataType == 7)
                this.Format = 1;

            return base.beforeInsert();
        }

        protected override bool beforeUpdateInsertAction()
        {
            //if (this.Format == 0 && this.MyDataType == 7)
            //    this.Format = 1;

            //设置时间类型.
            int format =  this.Format;
            if (format == 0 || format == 3 || format == 6)
                this.MyDataType = 6;
            else
                this.MyDataType = 7;

            return base.beforeUpdateInsertAction();
        }

        protected override void afterInsertUpdateAction()
        {
            MapAttr mapAttr = new MapAttr();
            mapAttr.MyPK = this.MyPK;
            mapAttr.RetrieveFromDBSources();
            mapAttr.Update();

            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);

            base.afterInsertUpdateAction();
        }

        /// <summary>
        /// 删除后清缓存
        /// </summary>
        protected override void afterDelete()
        {
            //删除相对应的rpt表中的字段
            if (this.FK_MapData.Contains("ND") == true)
            {
                string fk_mapData = this.FK_MapData.Substring(0, this.FK_MapData.Length - 2) + "Rpt";
                string sql = "DELETE FROM Sys_MapAttr WHERE FK_MapData='" + fk_mapData + "' AND KeyOfEn='" + this.KeyOfEn + "'";
                DBAccess.RunSQL(sql);
            }
            //调用frmEditAction, 完成其他的操作.
            BP.Sys.CCFormAPI.AfterFrmEditAction(this.FK_MapData);
            base.afterDelete();
        }

        #endregion

        #region 方法执行.
        /// <summary>
        /// 绑定函数
        /// </summary>
        /// <returns></returns>
        public string BindFunction()
        {
            return "../../Admin/FoolFormDesigner/MapExt/BindFunction.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 日期输入限制
        /// </summary>
        /// <returns></returns>
        public string DataFieldInputRole()
        {
            return "../../Admin/FoolFormDesigner/MapExt/DataFieldInputRole.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + this.KeyOfEn;
        }
        /// <summary>
        /// 自动计算
        /// </summary>
        /// <returns></returns>
        public string DoAutoFull()
        {
            return "../../Admin/FoolFormDesigner/MapExt/AutoFull.htm?FK_MapData=" + this.FK_MapData + "&ExtType=AutoFull&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn);
        }
        /// <summary>
        /// 正则表达式
        /// </summary>
        /// <returns></returns>
        public string DoRegularExpression()
        {
            return "../../Admin/FoolFormDesigner/MapExt/RegularExpression.htm?FK_MapData=" + this.FK_MapData + "&KeyOfEn=" + HttpUtility.UrlEncode(this.KeyOfEn) + "&MyPK=" + HttpUtility.UrlEncode(this.MyPK);
        }
        #endregion 方法执行.
    }
    /// <summary>
    /// 实体属性s
    /// </summary>
    public class MapAttrDTs : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 实体属性s
        /// </summary>
        public MapAttrDTs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapAttrDT();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapAttrDT> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapAttrDT>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapAttrDT> Tolist()
        {
            System.Collections.Generic.List<MapAttrDT> list = new System.Collections.Generic.List<MapAttrDT>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapAttrDT)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
