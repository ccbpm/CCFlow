using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP;
namespace BP.Sys
{
    /// <summary>
    /// 数据版本
    /// </summary>
    public class FrmDBVerAttr
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 用户名
        /// </summary>
        public const string Ver = "Ver";
        /// <summary>
        /// 主键值
        /// </summary>
        public const string PKVal = "PKVal";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string RecNo = "RecNo";
        /// <summary>
        /// 记录人姓名
        /// </summary>
        public const string RecName = "RecName";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 轨迹ID
        /// </summary>
        public const string TrackID = "TrackID";
    }
    /// <summary>
    /// 数据版本
    /// </summary>
    public class FrmDBVer : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                var uac = new UAC();
                uac.Readonly();
                uac.IsView = false;
                return uac;
            }
        }
        public string TrackID
        {
            get
            {
                return this.GetValStringByKey(FrmDBVerAttr.TrackID);
            }
            set
            {
                this.SetValByKey(FrmDBVerAttr.TrackID, value);
            }
        }
        public string FrmID
        {
            get
            {
                return this.GetValStringByKey(FrmDBVerAttr.FrmID);
            }
            set
            {
                this.SetValByKey(FrmDBVerAttr.FrmID, value);
            }
        }
        /// <summary>
        /// 主键值键
        /// </summary>
        public string EnPKVal
        {
            get
            {
                return this.GetValStringByKey(FrmDBVerAttr.PKVal);
            }
            set
            {
                this.SetValByKey(FrmDBVerAttr.PKVal, value);
            }
        }
        /// <summary>
        /// FK_Emp
        /// </summary>
        public string RecNo
        {
            get
            {
                return this.GetValStringByKey(FrmDBVerAttr.RecNo);
            }
            set
            {
                this.SetValByKey(FrmDBVerAttr.RecNo, value);
            }
        }
        public string RecName
        {
            get
            {
                return this.GetValStringByKey(FrmDBVerAttr.RecName);
            }
            set
            {
                this.SetValByKey(FrmDBVerAttr.RecName, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(FrmDBVerAttr.RDT);
            }
            set
            {
                this.SetValByKey(FrmDBVerAttr.RDT, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 数据版本
        /// </summary>
        public FrmDBVer()
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
                Map map = new Map("Sys_FrmDBVer", "数据版本");
                map.AddMyPK();


                map.AddTBString(FrmDBVerAttr.FrmID, null, "表单ID", true, true, 0, 100, 20);
                map.AddTBString(FrmDBVerAttr.PKVal, null, "PKVal", true, true, 0, 40, 20);

                map.AddTBString(FrmDBVerAttr.TrackID, null, "TrackID", true, true, 0, 40, 20);

                map.AddTBString(FrmDBVerAttr.RecNo, null, "记录人", true, true, 0, 30, 20);
                map.AddTBString(FrmDBVerAttr.RecName, null, "用户名", true, true, 0, 30, 20);
                map.AddTBDateTime(FrmDBVerAttr.RDT, null, "记录时间", true, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 增加
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="pkval"></param>
        /// <param name="trackID"></param>
        /// <param name="jsonOfFrmDB"></param>
        public static void AddFrmDBTrack(string frmID, string pkval, string trackID, string jsonOfFrmDB,string frmDtlDB)
        {
            if (jsonOfFrmDB == null)
                return;

            FrmDBVer en = new FrmDBVer();
            en.FrmID = frmID;
            en.EnPKVal = pkval;
            en.TrackID = trackID;
            en.Insert();


            //保存主表数据.
            DBAccess.SaveBigTextToDB(jsonOfFrmDB, "Sys_FrmDBVer", "MyPK", en.MyPK, "FrmDB");

            //保存从表数据
            DBAccess.SaveBigTextToDB(frmDtlDB, "Sys_FrmDBVer", "MyPK", en.MyPK, "FrmDtlDB");


        }

        protected override bool beforeInsert()
        {
            this.setMyPK(DBAccess.GenerGUID());
            this.RDT = DataType.CurrentDateTime;

            if (DataType.IsNullOrEmpty(this.RecNo) == true)
            {
                this.RecNo = BP.Web.WebUser.No;
                this.RecName = BP.Web.WebUser.Name;
            }

            return base.beforeInsert();
        }

        #region 重写
        public override Entities GetNewEntities
        {
            get { return new FrmDBVers(); }
        }
        #endregion 重写
    }
    /// <summary>
    /// 数据版本s
    /// </summary>
    public class FrmDBVers : EntitiesMyPK
    {
        #region 构造
        public FrmDBVers()
        {
        }
        #endregion

        #region 重写
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FrmDBVer();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FrmDBVer> Tolist()
        {
            System.Collections.Generic.List<FrmDBVer> list = new System.Collections.Generic.List<FrmDBVer>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FrmDBVer)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.

        #region 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FrmDBVer> ToJavaList()
        {
            return (System.Collections.Generic.IList<FrmDBVer>)this;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。

    }
}
