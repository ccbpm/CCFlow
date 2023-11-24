using System;
using System.Data;
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
        public const string RefPKVal = "RefPKVal";
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
        /// <summary>
        /// 修改的字段
        /// </summary>
        public const string ChangeFields = "ChangeFields";
        /// <summary>
        /// 修改的字段数量
        /// </summary>
        public const string ChangeNum = "ChangeNum";
        /// <summary>
        /// 字段（章节表单有效）
        /// </summary>
        public const string KeyOfEn = "KeyOfEn";
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
                UAC uac = new UAC();
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

        public string KeyOfEn
        {
            get
            {
                return this.GetValStringByKey(FrmDBVerAttr.KeyOfEn);
            }
            set
            {
                this.SetValByKey(FrmDBVerAttr.KeyOfEn, value);
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
        public string RefPKVal
        {
            get
            {
                return this.GetValStringByKey(FrmDBVerAttr.RefPKVal);
            }
            set
            {
                this.SetValByKey(FrmDBVerAttr.RefPKVal, value);
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
        public int Ver
        {
            get
            {
                return this.GetValIntByKey(FrmDBVerAttr.Ver);
            }
            set
            {
                this.SetValByKey(FrmDBVerAttr.Ver, value);
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
        public FrmDBVer(string mypk)
        {
            this.MyPK = mypk;
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
                Map map = new Map("Sys_FrmDBVer", "数据版本");
                map.AddMyPK();

                map.AddTBString(FrmDBVerAttr.FrmID, null, "表单ID", true, true, 0, 100, 20);
                map.AddTBString(FrmDBVerAttr.RefPKVal, null, "主键值", true, true, 0, 40, 20);

                map.AddTBString(FrmDBVerAttr.ChangeFields, null, "修改的字段", true, true, 0, 3900, 20);
                map.AddTBInt(FrmDBVerAttr.ChangeNum, 0, "修改的字段数量", true, true);

                map.AddTBString(FrmDBVerAttr.TrackID, null, "TrackID", true, true, 0, 40, 20);

                map.AddTBString(FrmDBVerAttr.RecNo, null, "记录人", true, true, 0, 30, 20);
                map.AddTBString(FrmDBVerAttr.RecName, null, "用户名", true, true, 0, 30, 20);
                map.AddTBDateTime(FrmDBVerAttr.RDT, null, "记录时间", true, true);
                map.AddTBInt(FrmDBVerAttr.Ver, 0, "版本号", true, true);


                map.AddTBString(FrmDBVerAttr.KeyOfEn, null, "章节字段有效", true, true, 0, 100, 20);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 增加版本.


        /// <summary>
        /// 增加主表、从表的数据
        /// </summary>
        /// <param name="frmID">表单ID</param>
        /// <param name="refPKVal">关联值</param>
        /// <param name="trackID"></param>
        /// <param name="jsonOfFrmDB"></param>
        /// <param name="frmDtlDB"></param>
        /// <param name="frmAthDB"></param>
        /// <param name="isChartFrm">是否章节表单？</param>
        public static void AddFrmDBTrack(int ver,string frmID, string refPKVal, string trackID, string jsonOfFrmDB, string frmDtlDB,string frmAthDB, bool isChartFrm = false)
        {
            if (jsonOfFrmDB == null && isChartFrm == false)
                return;

            FrmDBVer en = new FrmDBVer();
            en.FrmID = frmID;
            en.RefPKVal = refPKVal;
            en.TrackID = trackID;
            en.Ver = ver;
            en.Insert();

            //保存主表数据.
            if (DataType.IsNullOrEmpty(jsonOfFrmDB) == false)
                DBAccess.SaveBigTextToDB(jsonOfFrmDB, "Sys_FrmDBVer", "MyPK", en.MyPK, "FrmDB");

            //保存从表数据
            if(DataType.IsNullOrEmpty(frmDtlDB)==false)
                DBAccess.SaveBigTextToDB(frmDtlDB, "Sys_FrmDBVer", "MyPK", en.MyPK, "FrmDtlDB");

            //保存附件数据
            if (DataType.IsNullOrEmpty(frmAthDB) == false)
                DBAccess.SaveBigTextToDB(frmAthDB, "Sys_FrmDBVer", "MyPK", en.MyPK, "FrmAthDB");
        }
        /// <summary>
        /// 保存章节表单的字段数据
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="refPKVal"></param>
        /// <param name="trackID"></param>
        /// <param name="chartValue"></param>
        /// <param name="keyOfEn"></param>
        public static void AddKeyOfEnDBTrack(int ver,string frmID, string refPKVal, string trackID, string chartValue, string keyOfEn)
        {

            FrmDBVer en = new FrmDBVer();
            en.FrmID = frmID;
            en.RefPKVal = refPKVal;
            en.TrackID = trackID;
            en.KeyOfEn=keyOfEn;
            en.Ver = ver;
            en.Insert();

            //保存章节表单字段的数据.
            DBAccess.SaveBigTextToDB(chartValue, "Sys_FrmDBVer", "MyPK", en.MyPK, "FrmDB");
        }

        protected override bool beforeInsert()
        {
            this.setMyPK(DBAccess.GenerGUID());
            this.SetValByKey(FrmDBVerAttr.RDT, DataType.CurrentDateTime);

            if (DataType.IsNullOrEmpty(this.RecNo) == true)
            {
                this.RecNo = BP.Web.WebUser.No;
                this.RecName = BP.Web.WebUser.Name;
            }

            return base.beforeInsert();
        }
        #endregion 增加版本.

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
        /// <summary>
        /// 获得章节表单的版本.
        /// </summary>
        /// <param name="frmID"></param>
        /// <param name="workID"></param>
        /// <returns></returns>
        public string ChapterFrmDBVers(string frmID, string workID)
        {
            string sql = " SELECT DISTINCT Ver,RDT,RecName FROM Sys_FrmDBVer WHERE FrmID = '" + frmID + "' AND RefPKVal='" + workID + "' Group BY Ver,RDT,RecName ORDER BY RDT";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            // dt.Columns.Add("MaxVer"); //最大版本.
            if (BP.Difference.SystemConfig.AppCenterDBFieldCaseModel != FieldCaseModel.None)
            {
                dt.Columns[0].ColumnName = "Ver"; //版本号
                dt.Columns[1].ColumnName = "RDT";
                dt.Columns[2].ColumnName = "RecName";
            }


            return BP.Tools.Json.ToJson(dt);
        }


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
