using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;

namespace BP.Cloud
{
    /// <summary>
    /// 部门
    /// </summary>
    public class DeptExt : EntityTree
    {
        #region 属性
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(DeptAttr.OrgNo, value);
            }
        }
        /// <summary>
        /// adminer
        /// </summary>
        public string Adminer
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.Adminer);
            }
            set
            {
                this.SetValByKey(DeptAttr.Adminer, value);
            }
        }
        /// <summary>
        /// 全名
        /// </summary>
        public string NameOfPath
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.NameOfPath);
            }
            set
            {
                this.SetValByKey(DeptAttr.NameOfPath, value);
            }
        }
        /// <summary>
        /// 父节点的ID
        /// </summary>
        public new string ParentNo
        {
            get
            {
                return this.GetValStrByKey(DeptAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(DeptAttr.ParentNo, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 部门
        /// </summary>
        public DeptExt() { }
        /// <summary>
        /// 部门
        /// </summary>
        /// <param name="no">编号</param>
        public DeptExt(string no) : base(no) { }
        #endregion

        #region 重写方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Port_Dept", "部门");
               

                map.AddTBStringPK(DeptAttr.No, null, "编号", true, true, 1, 50, 20);
                map.AddTBString(DeptAttr.Name, null, "名称", true, false, 0, 100, 30);
                map.AddTBString(DeptAttr.ParentNo, null, "父节点编号", true, false, 0, 100, 30);
                map.AddTBString(DeptAttr.OrgNo, null, "组织编码", true, false, 0, 100, 30);
                map.AddTBString(DeptAttr.Adminer, null, "管理员帐号", true, false, 0, 100, 30);
                map.AddTBInt(DeptAttr.Idx, 0, "顺序号", true, false); //顺序号.

                //增加隐藏查询条件.
                map.AddHidden(StationTypeAttr.OrgNo, "=", BP.Web.WebUser.OrgNo);

                //RefMethod rm = new RefMethod();
                //rm.Title = "重置该部门一下的部门路径";
                //rm.ClassMethodName = this.ToString() + ".DoResetPathName";
                //rm.RefMethodType = RefMethodType.Func;

                //string msg = "当该部门名称变化后,该部门与该部门的子部门名称路径(Port_DeptExt.NameOfPath)将发生变化.";
                //msg += "\t\n 该部门与该部门的子部门的人员路径也要发生变化Port_Emp列DeptExtDesc.StaDesc.";
                //msg += "\t\n 您确定要执行吗?";
                //rm.Warning = msg;
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "增加同级部门";
                //rm.ClassMethodName = this.ToString() + ".DoSameLevelDeptExt";
                //rm.HisAttrs.AddTBString("No", null, "同级部门编号", true, false, 0, 100, 100);
                //rm.HisAttrs.AddTBString("Name", null, "部门名称", true, false, 0, 100, 100);
                //map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "增加下级部门";
                //rm.ClassMethodName = this.ToString() + ".DoSubDeptExt";
                //rm.HisAttrs.AddTBString("No", null, "同级部门编号", true, false, 0, 100, 100);
                //rm.HisAttrs.AddTBString("Name", null, "部门名称", true, false, 0, 100, 100);
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

      
        /// <summary>
        /// 创建下级节点.
        /// </summary>
        /// <returns></returns>
        public string DoMyCreateSubNode()
        {
            Entity en = this.DoCreateSubNode();
            return en.ToJson();
        }
       

        /// <summary>
        /// 创建同级节点.
        /// </summary>
        /// <returns></returns>
        public string DoMyCreateSameLevelNode()
        {
            Entity en = this.DoCreateSameLevelNode();
            return en.ToJson();
        }

        public string DoSameLevelDeptExt(string no, string name)
        {
            DeptExt en = new DeptExt();
            en.No = no;
            if (en.RetrieveFromDBSources() == 1)
                return "err@编号已经存在";

            en.Name = name;
            en.ParentNo = this.ParentNo;
            en.Insert();

            return "增加成功..";
        }
        public string DoSubDeptExt(string no, string name)
        {
            DeptExt en = new DeptExt();
            en.No = no;
            if (en.RetrieveFromDBSources() == 1)
                return "err@编号已经存在";

            en.Name = name;
            en.ParentNo = this.No;
            en.Insert();

            return "增加成功..";
        }
        /// <summary>
        /// 重置部门
        /// </summary>
        /// <returns></returns>
        public string DoResetPathName()
        {
            this.GenerNameOfPath();
            return "重置成功.";
        }

        /// <summary>
        /// 生成部门全名称.
        /// </summary>
        public void GenerNameOfPath()
        {
            string name = this.Name;

            //根目录不再处理
            if (this.ItIsRoot == true)
            {
                this.NameOfPath = name;
                this.DirectUpdate();
                this.GenerChildNameOfPath(this.No);
                return;
            }

            DeptExt DeptExt = new DeptExt();
            DeptExt.No = this.ParentNo;
            if (DeptExt.RetrieveFromDBSources() == 0)
                return;

            while (true)
            {
                if (DeptExt.ItIsRoot)
                    break;

                name = DeptExt.Name + "\\" + name;
                DeptExt = new DeptExt(DeptExt.ParentNo);
            }
            //根目录
            name = DeptExt.Name + "\\" + name;
            this.NameOfPath = name;
            this.DirectUpdate();

            this.GenerChildNameOfPath(this.No);

            ////更新人员路径信息.
            //BP.GPM.Emps emps = new Emps();
            //emps.Retrieve(EmpAttr.FK_DeptExt, this.No);
            //foreach (BP.GPM.Emp emp in emps)
            //    emp.Update();
        }

        /// <summary>
        /// 处理子部门全名称
        /// </summary>
        /// <param name="FK_DeptExt"></param>
        public void GenerChildNameOfPath(string DeptExtNo)
        {
            //DeptExts DeptExts = new DeptExts(DeptExtNo);
            //if (DeptExts != null && DeptExts.Count > 0)
            //{
            //    foreach (DeptExt DeptExt in DeptExts)
            //    {
            //        DeptExt.GenerNameOfPath();
            //        GenerChildNameOfPath(DeptExt.No);

            //        ////更新人员路径信息.
            //        //BP.GPM.Emps emps = new Emps();
            //        //emps.Retrieve(EmpAttr.FK_DeptExt, this.No);
            //        //foreach (BP.GPM.Emp emp in emps)
            //        //    emp.Update();
            //    }
            //}
        }
    }
    /// <summary>
    ///部门集合
    /// </summary>
    public class DeptExts : EntitiesTree
    {
        /// <summary>
        /// 得到一个新实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new DeptExt();
            }
        }
        /// <summary>
        /// 部门集合
        /// </summary>
        public DeptExts()
        {
        }
        public override int RetrieveAll()
        {
            return this.Retrieve(EmpAttr.OrgNo, BP.Web.WebUser.OrgNo);
        }

        #region 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<DeptExt> ToJavaList()
        {
            return (System.Collections.Generic.IList<DeptExt>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<DeptExt> Tolist()
        {
            System.Collections.Generic.List<DeptExt> list = new System.Collections.Generic.List<DeptExt>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((DeptExt)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成IList, c#代码调用会出错误。
    }
}
