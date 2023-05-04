using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.TA
{
    /// <summary>
    /// 轨迹属性
    /// </summary>
    public class TrackAttr : EntityMyPKAttr
    {
        #region 基本属性
        /// <summary>
        /// 标题
        /// </summary>
        public const string TaskID = "TaskID";
        /// <summary>
        /// 轨迹内容
        /// </summary>
        public const string PrjNo = "PrjNo";
        /// <summary>
        /// 按表单字段轨迹
        /// </summary>
        public const string EmpNo = "EmpNo";
        /// <summary>
        /// 表单字段
        /// </summary>
        public const string EmpName = "EmpName";
        /// <summary>
        /// 是否启用轨迹到角色
        /// </summary>
        public const string ActionType = "ActionType";
        /// <summary>
        /// 按照角色计算方式
        /// </summary>
        public const string ActionName = "ActionName";
        /// <summary>
        /// 是否轨迹到部门
        /// </summary>
        public const string Docs = "Docs";
        /// <summary>
        /// 是否轨迹到人员
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 是否启用按照SQL轨迹.
        /// </summary>
        public const string RecNo = "RecNo";
        /// <summary>
        /// 要轨迹的SQL
        /// </summary>
        public const string RecName = "RecName";
        #endregion

        public const string PrjSta = "PrjSta";
        public const string PRI = "PRI";
        public const string WCL = "WCL";

        public const string UseHH = "UseHH";
        public const string UseMM = "UseMM";
        
    }
    /// <summary>
    /// 轨迹
    /// </summary>
    public class Track : EntityMyPK
    {
        #region 属性
        public int UseHH
        {
            get
            {
                return this.GetValIntByKey(TrackAttr.UseHH);
            }
            set
            {
                this.SetValByKey(TrackAttr.UseHH, value);
            }
        }
        public int UseMM
        {
            get
            {
                return this.GetValIntByKey(TrackAttr.UseMM);
            }
            set
            {
                this.SetValByKey(TrackAttr.UseMM, value);
            }
        }
        public int WCL
        {
            get
            {
                return this.GetValIntByKey(TrackAttr.WCL);
            }
            set
            {
                this.SetValByKey(TrackAttr.WCL, value);
            }
        }
        public int TaskID
        {
            get
            {
                return this.GetValIntByKey(TrackAttr.TaskID);
            }
            set
            {
                this.SetValByKey(TrackAttr.TaskID, value);
            }
        }
        public string PrjNo
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.PrjNo);
            }
            set
            {
                this.SetValByKey(TrackAttr.PrjNo, value);
            }
        }
        public string EmpNo
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.EmpNo);
            }
            set
            {
                this.SetValByKey(TrackAttr.EmpNo, value);
            }
        }
        public string EmpName
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.EmpName);
            }
            set
            {
                this.SetValByKey(TrackAttr.EmpName, value);
            }
        }
        public string ActionType
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.ActionType);
            }
            set
            {
                this.SetValByKey(TrackAttr.ActionType, value);
            }
        }
        public string ActionName
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.ActionName);
            }
            set
            {
                this.SetValByKey(TrackAttr.ActionName, value);
            }
        }
        public string Docs
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.Docs);
            }
            set
            {
                this.SetValByKey(TrackAttr.Docs, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.RDT);
            }
            set
            {
                this.SetValByKey(TrackAttr.RDT, value);
            }
        }
        public string RecNo
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.RecNo);
            }
            set
            {
                this.SetValByKey(TrackAttr.RecNo, value);
            }
        }
        public string RecName
        {
            get
            {
                return this.GetValStringByKey(TrackAttr.RecName);
            }
            set
            {
                this.SetValByKey(TrackAttr.RecName, value);
            }
        }
        #endregion 属性

        #region 构造函数
        /// <summary>
        /// 轨迹
        /// </summary>
        public Track()
        {
        }
        /// <summary>
        /// 轨迹
        /// </summary>
        /// <param name="mypk"></param>
        public Track(string mypk)
        {
            this.setMyPK(mypk);
            this.Retrieve();
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

                Map map = new Map("TA_Track", "轨迹");

                map.AddMyPK();
                map.AddTBInt(TrackAttr.TaskID, 0, "TaskID", true, true);
                map.AddTBString(TrackAttr.PrjNo, null, "项目编号", true, false, 0, 50, 300);
                map.AddTBString(TrackAttr.EmpNo, null, "人员编号", true, false, 0, 50, 300);
                map.AddTBString(TrackAttr.EmpName, null, "人员名称", true, false, 0, 50, 300);

                map.AddTBString(TrackAttr.ActionType, null, "活动类型", true, false, 0, 50, 300);
                map.AddTBString(TrackAttr.ActionName, null, "活动名称", true, false, 0, 50, 300);
                map.AddTBString(TrackAttr.Docs, null, "内容", true, false, 0, 4000, 300);

                map.AddTBString(TrackAttr.RDT, null, "记录日期", true, false, 0, 50, 300);
                map.AddTBString(TrackAttr.RecNo, null, "记录人编号", true, false, 0, 50, 300);
                map.AddTBString(TrackAttr.RecName, null, "记录人名称", true, false, 0, 50, 300);


                map.AddTBInt(TrackAttr.WCL, 0, "完成率", true, true);
                map.AddTBInt(TrackAttr.UseHH, 0, "UseHH", true, true);
                map.AddTBInt(TrackAttr.UseMM, 0, "UseMM", true, true);


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 轨迹s
    /// </summary>
    public class Tracks : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Track();
            }
        }
        /// <summary>
        /// 轨迹
        /// </summary>
        public Tracks() { }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Track> ToJavaList()
        {
            return (System.Collections.Generic.IList<Track>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Track> Tolist()
        {
            System.Collections.Generic.List<Track> list = new System.Collections.Generic.List<Track>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Track)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
