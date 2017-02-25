using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 多对多的类型
    /// </summary>
    public enum M2MType
    {
        /// <summary>
        /// 一对多
        /// </summary>
        M2M,
        /// <summary>
        /// 一对多对多
        /// </summary>
        M2MM
    }
    /// <summary>
    /// 点对点
    /// </summary>
    public class MapM2MAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// 插入表单的位置
        /// </summary>
        public const string RowIdx = "RowIdx";
        /// <summary>
        /// GroupID
        /// </summary>
        public const string GroupID = "GroupID";
        /// <summary>
        /// 是否可以自适应大小
        /// </summary>
        public const string ShowWay = "ShowWay";
        /// <summary>
        /// 类型
        /// </summary>
        public const string M2MType = "M2MType";
        /// <summary>
        /// DBOfLists (对一对多对多模式有效）
        /// </summary>
        public const string DBOfLists = "DBOfLists";
        /// <summary>
        /// DBOfObjs
        /// </summary>
        public const string DBOfObjs = "DBOfObjs";
        public const string DBOfGroups = "DBOfGroups";
        public const string IsDelete = "IsDelete";
        public const string IsInsert = "IsInsert";
        /// <summary>
        /// 是否显示选择全部?
        /// </summary>
        public const string IsCheckAll = "IsCheckAll";
        public const string W = "W";
        public const string H = "H";
        public const string X = "X";
        public const string Y = "Y";
        public const string Cols = "Cols";
        /// <summary>
        /// 对象编号
        /// </summary>
        public const string NoOfObj = "NoOfObj";
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
    }
    /// <summary>
    /// 呈现方式
    /// </summary>
    public enum FrmShowWay
    {
        /// <summary>
        /// 隐藏
        /// </summary>
        Hidden,
        /// <summary>
        /// 自动大小
        /// </summary>
        FrmAutoSize,
        /// <summary>
        /// 指定大小
        /// </summary>
        FrmSpecSize,
        /// <summary>
        /// 新连接
        /// </summary>
        WinOpen
    }
    /// <summary>
    /// 点对点
    /// </summary>
    public class MapM2M : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 显示方式
        /// </summary>
        public FrmShowWay ShowWay
        {
            get
            {
                return (FrmShowWay)this.GetValIntByKey(MapM2MAttr.ShowWay);
            }
            set
            {
                this.SetValByKey(MapM2MAttr.ShowWay, (int)value);
            }
        }
        /// <summary>
        /// 是否显示选择全部？
        /// </summary>
        public bool IsCheckAll
        {
            get
            {
                return this.GetValBooleanByKey(MapM2MAttr.IsCheckAll);
            }
            set
            {
                this.SetValByKey(MapM2MAttr.IsCheckAll, value);
            }
        }
        
        public bool IsDelete
        {
            get
            {
                return this.GetValBooleanByKey(MapM2MAttr.IsDelete);
            }
            set
            {
                this.SetValByKey(MapM2MAttr.IsDelete, value);
            }
        }
        public bool IsInsert
        {
            get
            {
                return this.GetValBooleanByKey(MapM2MAttr.IsInsert);
            }
            set
            {
                this.SetValByKey(MapM2MAttr.IsInsert, value);
            }
        }
        public bool IsEdit
        {
            get
            {
                if (this.IsInsert || this.IsDelete)
                    return true;
                return false;
            }
        }
        /// <summary>
        /// 列表(对一对多对多模式有效）
        /// </summary>
        public string DBOfLists
        {
            get
            {
                string sql = this.GetValStrByKey(MapM2MAttr.DBOfLists);
                sql = sql.Replace("~", "'");
                return sql;
            }
            set
            {
                this.SetValByKey(MapM2MAttr.DBOfLists, value);
            }
        }
        /// <summary>
        /// 列表(对一对多对多模式有效）
        /// </summary>
        public string DBOfListsRun
        {
            get
            {
                string sql = this.DBOfLists;
                sql = sql.Replace("~", "'");
                sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                sql = sql.Replace("@WebUser.FK_DeptNameOfFull", BP.Web.WebUser.FK_DeptNameOfFull);
                sql = sql.Replace("@WebUser.FK_DeptName", BP.Web.WebUser.FK_DeptName);
                sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                return sql;
            }
        }
        public string DBOfObjs
        {
            get
            {
                string sql = this.GetValStrByKey(MapM2MAttr.DBOfObjs);
                sql = sql.Replace("~", "'");
                return sql;
            }
            set
            {
                this.SetValByKey(MapM2MAttr.DBOfObjs, value);
            }
        }
        public string DBOfGroups
        {
            get
            {
                string sql = this.GetValStrByKey(MapM2MAttr.DBOfGroups);
                sql = sql.Replace("~", "'");
                return sql;
            }
            set
            {
                this.SetValByKey(MapM2MAttr.DBOfGroups, value);
            }
        }
        public string DBOfObjsRun
        {
            get
            {
                string sql = this.GetValStrByKey(MapM2MAttr.DBOfObjs);
                sql = sql.Replace("~", "'");
                sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                sql = sql.Replace("@WebUser.FK_DeptNameOfFull", BP.Web.WebUser.FK_DeptNameOfFull);
                sql = sql.Replace("@WebUser.FK_DeptName", BP.Web.WebUser.FK_DeptName);
                sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                return sql;
            }
            set
            {
                this.SetValByKey(MapM2MAttr.DBOfObjs, value);
            }
        }
        public string DBOfGroupsRun
        {
            get
            {
                string sql = this.GetValStrByKey(MapM2MAttr.DBOfGroups);
                sql = sql.Replace("~", "'");
                sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                sql = sql.Replace("@WebUser.FK_DeptNameOfFull", BP.Web.WebUser.FK_DeptNameOfFull);
                sql = sql.Replace("@WebUser.FK_DeptName", BP.Web.WebUser.FK_DeptName);
                sql = sql.Replace("@WebUser.FK_Dept", BP.Web.WebUser.FK_Dept);
                return sql;
            }
            set
            {
                this.SetValByKey(MapM2MAttr.DBOfGroups, value);
            }
        }
        /// <summary>
        /// 内部编号
        /// </summary>
        public string NoOfObj
        {
            get
            {
                return this.GetValStrByKey(MapM2MAttr.NoOfObj);
            }
            set
            {
                this.SetValByKey(MapM2MAttr.NoOfObj, value);
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStrByKey(MapM2MAttr.Name);
            }
            set
            {
                this.SetValByKey(MapM2MAttr.Name, value);
            }
        }
        public bool IsUse = false;
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(MapM2MAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(MapM2MAttr.FK_MapData, value);
            }
        }
        public int RowIdx
        {
            get
            {
                return this.GetValIntByKey(MapM2MAttr.RowIdx);
            }
            set
            {
                this.SetValByKey(MapM2MAttr.RowIdx, value);
            }
        }
        public int Cols
        {
            get
            {
                return this.GetValIntByKey(MapM2MAttr.Cols);
            }
            set
            {
                this.SetValByKey(MapM2MAttr.Cols, value);
            }
        }
        public M2MType HisM2MType
        {
            get
            {
                return (M2MType)this.GetValIntByKey(MapM2MAttr.M2MType);
            }
            set
            {
                this.SetValByKey(MapM2MAttr.M2MType, (int)value);
            }
        }
        public int GroupID
        {
            get
            {
                return this.GetValIntByKey(MapM2MAttr.GroupID);
            }
            set
            {
                this.SetValByKey(MapM2MAttr.GroupID, value);
            }
        }
        public float H
        {
            get
            {
                return this.GetValFloatByKey(MapM2MAttr.H);
            }
            set
            {
                this.SetValByKey(MapM2MAttr.H, value);
            }
        }
        public float W
        {
            get
            {
                return this.GetValFloatByKey(MapM2MAttr.W);
            }
            set
            {
                this.SetValByKey(MapM2MAttr.W, value);
            }
        }
        public float X
        {
            get
            {
                return this.GetValFloatByKey(MapM2MAttr.X);
            }
            set
            {
                this.SetValByKey(MapM2MAttr.X, value);
            }
        }
        public float Y
        {
            get
            {
                return this.GetValFloatByKey(MapM2MAttr.Y);
            }
            set
            {
                this.SetValByKey(MapM2MAttr.Y, value);
            }
        }
        /// <summary>
        /// 扩展属性
        /// </summary>
        public int FK_Node
        {
            get
            {
                return int.Parse(this.FK_MapData.Replace("ND", ""));
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 点对点
        /// </summary>
        public MapM2M()
        {
        }
        public MapM2M(string myPK)
        {
            this.MyPK = myPK;
            this.Retrieve();
        }
        /// <summary>
        /// 点对点
        /// </summary>
        /// <param name="fk_mapdata"></param>
        /// <param name="noOfObj"></param>
        public MapM2M(string fk_mapdata, string noOfObj)
        {
            this.FK_MapData=fk_mapdata;
            this.NoOfObj=noOfObj;
            this.MyPK = this.FK_MapData + "_" + this.NoOfObj;
            this.RetrieveFromDBSources();
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
                Map map = new Map("Sys_MapM2M");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "多选";
                map.EnType = EnType.Sys;

                map.AddMyPK();
                map.AddTBString(MapM2MAttr.FK_MapData, null, "主表", true, false, 1, 100, 20);
                map.AddTBString(MapM2MAttr.NoOfObj, null, "编号", true, false, 1, 20, 20);

                map.AddTBString(MapM2MAttr.Name, null, "名称", true, false, 1, 200, 20);

                map.AddTBString(MapM2MAttr.DBOfLists, null, "列表数据源(对一对多对多模式有效）", true, false, 0, 4000, 20);

                map.AddTBString(MapM2MAttr.DBOfObjs, null, "DBOfObjs", true, false, 0, 4000, 20);
                map.AddTBString(MapM2MAttr.DBOfGroups, null, "DBOfGroups", true, false, 0, 4000, 20);

                map.AddTBFloat(MapM2MAttr.H, 100, "H", false, false);
                map.AddTBFloat(MapM2MAttr.W, 160, "W", false, false);
                map.AddTBFloat(FrmImgAttr.X, 5, "X", true, false);
                map.AddTBFloat(FrmImgAttr.Y, 5, "Y", false, false);

                map.AddTBInt(MapM2MAttr.ShowWay, (int)FrmShowWay.FrmAutoSize, "显示方式", false, false);

                map.AddTBInt(MapM2MAttr.M2MType, (int)M2MType.M2M, "类型", false, false);

                map.AddTBInt(MapM2MAttr.RowIdx, 99, "位置", false, false);
                map.AddTBInt(MapM2MAttr.GroupID, 0, "分组ID", false, false);

                map.AddTBInt(MapM2MAttr.Cols, 4, "记录呈现列数", false, false);

                map.AddBoolean(MapM2MAttr.IsDelete, true, "可删除否", false, false);
                map.AddBoolean(MapM2MAttr.IsInsert, true, "可插入否", false, false);

                map.AddBoolean(MapM2MAttr.IsCheckAll, true, "是否显示选择全部", false, false);


                //map.AddTBFloat(FrmImgAttr.H, 200, "H", true, false);
                //map.AddTBFloat(FrmImgAttr.W, 500, "W", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        protected override bool beforeInsert()
        {
            if (this.DBOfObjs.Trim().Length <= 5)
            {
                this.DBOfGroups = "SELECT No,Name FROM Port_Dept";
                this.DBOfObjs = "SELECT No,Name,FK_Dept FROM Port_Emp";
            }

            return base.beforeInsert();
        }
        protected override bool beforeUpdateInsertAction()
        {
            this.MyPK = this.FK_MapData + "_" + this.NoOfObj;
            return base.beforeUpdateInsertAction();
        }
        #endregion
    }
    /// <summary>
    /// 点对点s
    /// </summary>
    public class MapM2Ms : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 点对点s
        /// </summary>
        public MapM2Ms()
        {
        }
        /// <summary>
        /// 点对点s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public MapM2Ms(string fk_mapdata)
        {
            this.Retrieve(MapM2MAttr.FK_MapData, fk_mapdata, MapM2MAttr.GroupID);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new MapM2M();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<MapM2M> ToJavaList()
        {
            return (System.Collections.Generic.IList<MapM2M>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<MapM2M> Tolist()
        {
            System.Collections.Generic.List<MapM2M> list = new System.Collections.Generic.List<MapM2M>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((MapM2M)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
