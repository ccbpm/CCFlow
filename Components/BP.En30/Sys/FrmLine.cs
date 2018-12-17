using System;
using System.Collections;
using BP.DA;
using BP.En;
namespace BP.Sys
{
      
    /// <summary>
    /// 线
    /// </summary>
    public class FrmLineAttr : EntityOIDNameAttr
    {
        /// <summary>
        /// 主表
        /// </summary>
        public const string FK_MapData = "FK_MapData";
        /// <summary>
        /// X1
        /// </summary>
        public const string X1 = "X1";
        /// <summary>
        /// Y1
        /// </summary>
        public const string Y1 = "Y1";
        /// <summary>
        /// X2
        /// </summary>
        public const string X2 = "X2";
        /// <summary>
        /// Y2
        /// </summary>
        public const string Y2 = "Y2";
        /// <summary>
        /// 宽度
        /// </summary>
        public const string BorderWidth = "BorderWidth";
        /// <summary>
        /// 颜色
        /// </summary>
        public const string BorderColor = "BorderColor";
        /// <summary>
        /// GUID
        /// </summary>
        public const string GUID = "GUID";
    }
    /// <summary>
    /// 线
    /// </summary>
    public class FrmLine : EntityMyPK
    {
        #region 属性
        public string BorderColorHtml
        {
            get
            {
                return PubClass.ToHtmlColor(this.BorderColor);
            }
        }
        public string BorderColor
        {
            get
            {
                return this.GetValStringByKey("BorderColor");
            }
            set
            {
                this.SetValByKey(FrmLineAttr.BorderColor, value);
            }
        }
        public float BorderWidth
        {
            get
            {
                return this.GetValFloatByKey(FrmLineAttr.BorderWidth);
            }
            set
            {
                this.SetValByKey(FrmLineAttr.BorderWidth, value);
            }
        }
        /// <summary>
        /// GUID
        /// </summary>
        public string GUID
        {
            get
            {
                return this.GetValStrByKey(FrmLineAttr.GUID);
            }
            set
            {
                this.SetValByKey(FrmLineAttr.GUID, value);
            }
        }
        /// <summary>
        /// Y1
        /// </summary>
        public float Y1
        {
            get
            {
                return this.GetValFloatByKey(FrmLineAttr.Y1);
            }
            set
            {
                this.SetValByKey(FrmLineAttr.Y1, value);
            }
        }
        /// <summary>
        /// X1
        /// </summary>
        public float X1
        {
            get
            {
                return this.GetValFloatByKey(FrmLineAttr.X1);
            }
            set
            {
                this.SetValByKey(FrmLineAttr.X1, value);
            }
        }
        public string FK_MapData
        {
            get
            {
                return this.GetValStrByKey(FrmLineAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(FrmLineAttr.FK_MapData, value);
            }
        }
        public float Y2
        {
            get
            {
                return this.GetValFloatByKey(FrmLineAttr.Y2);
            }
            set
            {
                this.SetValByKey(FrmLineAttr.Y2, value);
            }
        }
        public float X2
        {
            get
            {
                return this.GetValFloatByKey(FrmLineAttr.X2);
            }
            set
            {
                this.SetValByKey(FrmLineAttr.X2, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 线
        /// </summary>
        public FrmLine()
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
                Map map = new Map("Sys_FrmLine", "线");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);
                map.Java_SetEnType(EnType.Sys);

                map.AddMyPK();
                map.AddTBString(FrmLineAttr.FK_MapData, null, "主表", true, false, 0, 100, 20);

                map.AddTBFloat(FrmLineAttr.X1, 5, "X1", true, false);
                map.AddTBFloat(FrmLineAttr.Y1, 5, "Y1", false, false);

                map.AddTBFloat(FrmLineAttr.X2, 9, "X2", false, false);
                map.AddTBFloat(FrmLineAttr.Y2, 9, "Y2", false, false);

                //不再用的两个字段,但是还不能删除.
                map.AddTBFloat("X", 9, "X", false, false);
                map.AddTBFloat("Y", 9, "Y", false, false);

                map.AddTBFloat(FrmLineAttr.BorderWidth, 1, "宽度", false, false);
                map.AddTBString(FrmLineAttr.BorderColor, "black", "颜色", true, false, 0, 30, 20);

                map.AddTBString(FrmLineAttr.GUID, null, "初始的GUID", true, false, 0, 128, 20);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 是否存在相同的数据?
        /// </summary>
        /// <returns></returns>
        public bool IsExitGenerPK()
        {
            string sql = "SELECT COUNT(*) FROM " + this.EnMap.PhysicsTable + " WHERE FK_MapData='" + this.FK_MapData + "' AND x1=" + this.X1 + " and x2=" + this.X2 + " and y1=" + this.Y1 + " and y2=" + this.Y2;
            if (DBAccess.RunSQLReturnValInt(sql, 0) == 0)
                return false;
            return true;
        }
    }
    /// <summary>
    /// 线s
    /// </summary>
    public class FrmLines : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 线s
        /// </summary>
        public FrmLines()
        {
        }
        /// <summary>
        /// 线s
        /// </summary>
        /// <param name="fk_mapdata">s</param>
        public FrmLines(string fk_mapdata)
        {
            if (SystemConfig.IsDebug)
                this.Retrieve(FrmLineAttr.FK_MapData, fk_mapdata);
            else
                this.RetrieveFromCash(FrmLineAttr.FK_MapData, (object)fk_mapdata);
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmLine();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmLine> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmLine>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmLine> Tolist()
        {
            System.Collections.Generic.List<FrmLine> list = new System.Collections.Generic.List<FrmLine>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmLine)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
