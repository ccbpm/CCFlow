using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 信息推送
    /// </summary>
    public class InfoPushAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 顺序
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 图标
        /// </summary>
        public const string ICON = "ICON";
        /// <summary>
        /// Url
        /// </summary>
        public const string Url = "Url";
        /// <summary>
        /// 获取的sql
        /// </summary>
        public const string GetSQL = "GetSQL";
    }
    /// <summary>
    /// 信息推送
    /// </summary>
    public class InfoPush : EntityNoName
    {
        #region 属性
        /// <summary>
        /// Idx
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(InfoPushAttr.Idx);
            }
            set
            {
                this.SetValByKey(InfoPushAttr.Idx, value);
            }
        }
        /// <summary>
        /// GetSQL
        /// </summary>
        public string GetSQL
        {
            get
            {
                string sql = this.GetValStringByKey(InfoPushAttr.GetSQL);
                sql = sql.Replace("@WebUser.No", "'"+Web.WebUser.No+"'");
                return sql;
            }
            set
            {
                this.SetValByKey(InfoPushAttr.GetSQL, value);
            }
        }
        public string WebPath
        {
            get
            {
                return this.GetValStringByKey("WebPath");
            }
        }
        public string ICON
        {
            get
            {
                return this.WebPath;
            }
            set
            {
                this.SetValByKey(InfoPushAttr.ICON, value);
            }
        }
        public string Url
        {
            get
            {
                return this.GetValStringByKey(InfoPushAttr.Url);
            }
            set
            {
                this.SetValByKey(InfoPushAttr.Url, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 信息推送
        /// </summary>
        public InfoPush()
        {
        }
        /// <summary>
        /// 信息推送
        /// </summary>
        /// <param name="mypk"></param>
        public InfoPush(string no)
        {
            this.No = no;
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
                Map map = new Map("GPM_InfoPush");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "信息推送";
                map.EnType = EnType.Sys;

                map.AddTBStringPK(InfoPushAttr.No, null, "编号", true, false, 2, 30, 20);
                map.AddTBString(InfoPushAttr.Name, null, "名称", true, false, 0, 3900, 20);
                map.AddTBString(InfoPushAttr.Url, null, "连接", true, false, 0, 3900, 20, true);
                map.AddDDLSysEnum(BarAttr.OpenWay, 0, "打开方式", true, true,
                BarAttr.OpenWay, "@0=新窗口@1=本窗口@2=覆盖新窗口");
                map.AddTBInt(InfoPushAttr.Idx, 0, "显示顺序", true, false);

                map.AddTBStringDoc(InfoPushAttr.GetSQL, null, "获取待处理数量的SQL", true, false, true);
                map.AddTBString(MenuAttr.CtrlObjs, null, "控制内容", true, false, 0, 4000, 20,true);

                // 一对多的关系.
                map.AttrsOfOneVSM.Add(new ByStations(), new Stations(), ByStationAttr.RefObj, ByStationAttr.FK_Station,
                    StationAttr.Name, StationAttr.No, "可访问的岗位");
                map.AttrsOfOneVSM.Add(new ByDepts(), new Depts(), ByStationAttr.RefObj, ByDeptAttr.FK_Dept,
                    DeptAttr.Name, DeptAttr.No, "可访问的部门");
                map.AttrsOfOneVSM.Add(new ByEmps(), new Emps(), ByStationAttr.RefObj, ByEmpAttr.FK_Emp,
                    EmpAttr.Name, EmpAttr.No, "可访问的人员");

                map.AddMyFile("图标");


                RefMethod rm = new RefMethod();
                rm.Title = "查看可访问该系统的人员";
                rm.ClassMethodName = this.ToString() + ".DoWhoCanUseApp";
                map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 查看可以访问的人员
        /// </summary>
        /// <returns></returns>
        public string DoWhoCanUseApp()
        {
            EmpInfoPushs eips = new EmpInfoPushs();
            eips.ClearTable();

            InfoPushs ens=new InfoPushs();
            ens.RetrieveAllFromDBSource();

            Emps emps = new Emps();
            emps.RetrieveAll();
            foreach (Emp emp in emps)
            {
                foreach (InfoPush info in ens)
                {
                    
                    EmpInfoPush em = new EmpInfoPush();
                    em.Copy(info);
                    em.FK_Emp = emp.No;
                    em.FK_InfoPush = info.No;

                    em.MyPK = info.No + "_" + emp.No;
                    em.Insert();
                }
            }

            PubClass.WinOpen("/GPM/WhoCanViewInfoPush.aspx?FK_App=" + this.No, 500, 700);
            return null;
        }
    }
    /// <summary>
    /// 信息推送s
    /// </summary>
    public class InfoPushs : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 信息推送s
        /// </summary>
        public InfoPushs()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new InfoPush();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<InfoPush> ToJavaList()
        {
            return (System.Collections.Generic.IList<InfoPush>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<InfoPush> Tolist()
        {
            System.Collections.Generic.List<InfoPush> list = new System.Collections.Generic.List<InfoPush>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((InfoPush)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
