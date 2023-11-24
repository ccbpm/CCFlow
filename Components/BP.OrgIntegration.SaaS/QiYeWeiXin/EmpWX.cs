using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;
using BP.Difference;

namespace BP.OrgIntegration
{
    /// <summary>
    /// 操作员 属性
    /// </summary>
    public class EmpWXAttr : EmpAttr
    {
        #region 基本属性           
        /// <summary>
        /// 微信用户ID
        /// </summary>
        public const string OpenID = "OpenID";
        public const string Dutys = "Dutys";
        #endregion
    }
    /// <summary>
    /// 操作员 的摘要说明。
    /// </summary>
    public class EmpWX : EntityNoName
    {
        #region 扩展属性
        /// <summary>
        /// 职务s
        /// </summary>
        public string Dutys
        {
            get
            {
                return this.GetValStrByKey(EmpWXAttr.Dutys);
            }
            set
            {
                this.SetValByKey(EmpWXAttr.Dutys, value);
            }
        }
        public string DeptNo
        {
            get
            {
                return this.GetValStrByKey(EmpWXAttr.FK_Dept);
            }
            set
            {
                this.SetValByKey(EmpWXAttr.FK_Dept, value);
            }
        }
        /// <summary>
        /// 用户ID.
        /// </summary>
        public string UserID
        {
            get
            {
                return this.GetValStrByKey(EmpWXAttr.UserID);
            }
            set
            {
                this.SetValByKey(EmpWXAttr.UserID, value);
            }
        }
        /// <summary>
        /// 拼音
        /// </summary>
        public string PinYin
        {
            get
            {
                return this.GetValStrByKey(EmpWXAttr.PinYin);
            }
            set
            {
                this.SetValByKey(EmpWXAttr.PinYin, value);
            }
        }
       
        public string Tel
        {
            get
            {
                return this.GetValStrByKey(EmpWXAttr.Tel);
            }
            set
            {
                this.SetValByKey(EmpWXAttr.Tel, value);
            }
        }
        public string Email
        {
            get
            {
                return this.GetValStrByKey(EmpWXAttr.Email);
            }
            set
            {
                this.SetValByKey(EmpWXAttr.Email, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(EmpWXAttr.Idx);
            }
            set
            {
                this.SetValByKey(EmpWXAttr.Idx, value);
            }
        }
        /// <summary>
        /// 组织结构编码
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(EmpWXAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(EmpWXAttr.OrgNo, value);
            }
        }
        /// <summary>
        /// 微信用户ID
        /// </summary>
        public string OpenID
        {
            get
            {
                return this.GetValStrByKey(EmpWXAttr.OpenID);
            }
            set
            {
                this.SetValByKey(EmpWXAttr.OpenID, value);
            }
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// 操作员
        /// </summary>
        public EmpWX()
        {
        }
        /// <summary>
        /// 操作员
        /// </summary>
        /// <param name="no">编号</param>
        public EmpWX(string no)
        {
            try
            {
                //如果小于11位估计不是一个手机号.
                if (BP.DA.DataType.IsMobile(no) == false)
                    this.No = BP.Web.WebUser.OrgNo + "_" + no;
                else
                    this.No = no;

                this.Retrieve();
              //  this.No = no;
            }
            catch (Exception ex)
            {
                int i = this.RetrieveFromDBSources();
                if (i == 0)
                    throw new Exception("err@用户账号[" + this.No + "]错误:" + ex.Message);
            }
        }

        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (BP.Web.WebUser.IsAdmin == true)
                    uac.OpenAll();
                else
                    uac.Readonly();
                return uac;
            }
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

                Map map = new Map("Port_Emp", "用户");
                map.EnType = EnType.App;
                map.IndexField = EmpWXAttr.FK_Dept;

                #region 字段

                /*关于字段属性的增加 */
                map.AddTBStringPK(EmpWXAttr.No, null, "手机号/ID", true, false, 1, 500, 90);
                map.AddTBString(EmpWXAttr.UserID, null, "昵称", false, false, 0, 100, 10);
                map.AddTBString(EmpWXAttr.Name, null, "姓名", true, false, 0, 500, 130);

                map.AddDDLEntities(EmpWXAttr.FK_Dept, null, "当前登录的部门",
                    new BP.Port.Depts(), false);

                map.AddTBString(EmpWXAttr.Tel, null, "电话", true, false, 0, 20, 130);
                map.AddTBString(EmpWXAttr.Email, null, "邮箱", true, false, 0, 100, 132, true);
                map.AddTBString(EmpWXAttr.PinYin, null, "拼音", true, false, 0, 1000, 132, true);

                //map.AddTBString(EmpWXAttr.QQ, null, "QQ", true, false, 0, 500, 132, true);
                //map.AddTBString(EmpWXAttr.QQAppID, null, "QQAppID", true, false, 0, 500, 132, true);

                map.AddTBString(EmpWXAttr.OrgNo, null, "OrgNo", false, false, 0, 36, 36);
                map.AddTBString(EmpWXAttr.OpenID, null, "微信OpenID", false, false, 0, 200, 36);

                map.AddTBString(EmpWXAttr.Dutys, null, "职务s", false, false, 0, 500, 36);


                map.AddTBInt(EmpWXAttr.Idx, 0, "序号", true, false);
                #endregion 字段

                this._enMap = map;
                return this._enMap;
            }
        }
 
        public string DoEmpWXDepts()
        {
            return SystemConfig.CCFlowWebPath + "GPM/EmpDepts.htm?FK_Emp=" + this.No;
        }

        public string DoSinger()
        {
            //路径
            return SystemConfig.CCFlowWebPath + "GPM/Siganture.htm?EmpNo=" + this.No;
        }
        protected override bool beforeInsert()
        {
            return base.beforeInsert();
        }

        protected override bool beforeUpdateInsertAction()
        {
            //增加拼音，以方便查找.
            if (DataType.IsNullOrEmpty(this.Name) == true)
                throw new Exception("err@名称不能为空.");

            string pinyinQP = BP.DA.DataType.ParseStringToPinyin(this.Name).ToLower();
            string pinyinJX = BP.DA.DataType.ParseStringToPinyinJianXie(this.Name).ToLower();
            this.PinYin = "," + pinyinQP + "," + pinyinJX + ",";

            ////处理岗位信息.
            //DeptEmpWXStations des = new DeptEmpWXStations();
            //des.Retrieve(DeptEmpWXStationAttr.FK_EmpWX, this.No);
            //string depts = "";
            //string stas = "";

            //foreach (DeptEmpWXStation item in des)
            //{
            //    BP.GPM.Dept dept = new BP.GPM.Dept();
            //    dept.No = item.FK_Dept;
            //    if (dept.RetrieveFromDBSources() == 0)
            //    {
            //        item.Delete();
            //        continue;D:\CCFlowCloud\BP.Cloud\Port\EmpWX.cs
            //    }

            //    //给拼音重新定义值,让其加上部门的信息.
            //    this.PinYin = this.PinYin + pinyinJX + "/" + BP.DA.DataType.ParseStringToPinyinJianXie(dept.Name).ToLower()+",";

            //    BP.Port.Station sta = new Port.Station();
            //    sta.No = item.FK_Station;
            //    if (sta.RetrieveFromDBSources() == 0)
            //    {
            //        item.Delete();
            //        continue;
            //    }

            //    stas += "@" + dept.NameOfPath + "|" + sta.Name;
            //    depts += "@" + dept.NameOfPath;
            //}
            return base.beforeUpdateInsertAction();
        }

        /// <summary>
        /// 保存后修改WF_EmpWX中的邮箱
        /// </summary>
        protected override void afterInsertUpdateAction()
        {
            string sql = "Select Count(*) From WF_Emp Where No='" + this.No + "'";
            int count = DBAccess.RunSQLReturnValInt(sql);
            if (count == 0)
                sql = "INSERT INTO WF_Emp (No,Name,Email) VALUES('" + this.No + "','" + this.Name + "','" + this.Email + "')";
            else
                sql = "UPDATE WF_Emp SET Email='" + this.Email + "'";
            DBAccess.RunSQL(sql);

            //修改Port_EmpWX中的缓存                  
            BP.Port.Emp EmpWX = new BP.Port.Emp();
            EmpWX.No = this.No;
            if (EmpWX.RetrieveFromDBSources() != 0)
            {
                EmpWX.DeptNo = this.DeptNo;
                EmpWX.Update();
            }

            base.afterInsertUpdateAction();
        }
        protected override bool beforeDelete()
        {
            if (this.OrgNo != BP.Web.WebUser.OrgNo)
                throw new Exception("err@您不能删除别人的数据.");

            return base.beforeDelete();
        }
        /// <summary>
        /// 删除之后要做的事情
        /// </summary>
        protected override void afterDelete()
        {
            base.afterDelete();
        }

        public static string GenerPinYin(string no, string name)
        {
            //增加拼音，以方便查找.
            string pinyinQP = BP.DA.DataType.ParseStringToPinyin(name).ToLower();
            string pinyinJX = BP.DA.DataType.ParseStringToPinyinJianXie(name).ToLower();
            string py = "," + pinyinQP + "," + pinyinJX + ",";

            ////处理岗位信息.
            //DeptEmpWXStations des = new DeptEmpWXStations();
            //des.Retrieve(DeptEmpWXStationAttr.FK_EmpWX, no);

            //string depts = "";
            //string stas = "";

            //foreach (DeptEmpWXStation item in des)
            //{
            //    BP.GPM.Dept dept = new BP.GPM.Dept();
            //    dept.No = item.FK_Dept;
            //    if (dept.RetrieveFromDBSources() == 0)
            //    {
            //        item.Delete();
            //        continue;
            //    }

            //    //给拼音重新定义值,让其加上部门的信息.
            //    py = py + pinyinJX + "/" + BP.DA.DataType.ParseStringToPinyinJianXie(dept.Name).ToLower() + ",";

            //    BP.Port.Station sta = new Port.Station();
            //    sta.No = item.FK_Station;
            //    if (sta.RetrieveFromDBSources() == 0)
            //    {
            //        item.Delete();
            //        continue;
            //    }

            //    stas += "@" + dept.NameOfPath + "|" + sta.Name;
            //    depts += "@" + dept.NameOfPath;
            //}

            return py;
        }

        /// <summary>
        /// 获取集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new EmpWXs(); }
        }
        #endregion 构造函数
    }
    /// <summary>
    /// 操作员s
    // </summary>
    public class EmpWXs : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EmpWX();
            }
        }
        /// <summary>
        /// 操作员s
        /// </summary>
        public EmpWXs()
        {
        }

        #endregion 构造方法

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EmpWX> ToJavaList()
        {
            return (System.Collections.Generic.IList<EmpWX>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EmpWX> Tolist()
        {
            System.Collections.Generic.List<EmpWX> list = new System.Collections.Generic.List<EmpWX>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EmpWX)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
