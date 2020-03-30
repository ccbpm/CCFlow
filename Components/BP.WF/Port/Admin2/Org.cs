using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;

namespace BP.WF.Port.Admin2
{
    /// <summary>
    /// 独立组织属性
    /// </summary>
    public class OrgAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 管理员帐号
        /// </summary>
        public const string Adminer = "Adminer";
        /// <summary>
        /// 管理员名称
        /// </summary>
        public const string AdminerName = "AdminerName";
        /// <summary>
        /// 父级组织编号
        /// </summary>
        public const string ParentNo = "ParentNo";
        /// <summary>
        /// 父级组织名称
        /// </summary>
        public const string ParentName = "ParentName";
    }
    /// <summary>
    /// 独立组织
    /// </summary>
    public class Org : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 父级组织编号
        /// </summary>
        public string ParentNo
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(OrgAttr.ParentNo, value);
            }
        }
        /// <summary>
        /// 父级组织名称
        /// </summary>
        public string ParentName
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.ParentName);
            }
            set
            {
                this.SetValByKey(OrgAttr.ParentName, value);
            }
        }
        /// <summary>
        /// 父节点编号
        /// </summary>
        public string Adminer
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.Adminer);
            }
            set
            {
                this.SetValByKey(OrgAttr.Adminer, value);
            }
        }
        /// <summary>
        /// 管理员名称
        /// </summary>
        public string AdminerName
        {
            get
            {
                return this.GetValStrByKey(OrgAttr.AdminerName);
            }
            set
            {
                this.SetValByKey(OrgAttr.AdminerName, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 独立组织
        /// </summary>
        public Org() { }
        /// <summary>
        /// 独立组织
        /// </summary>
        /// <param name="no">编号</param>
        public Org(string no) : base(no) { }
        #endregion

        #region 重写方法
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsDelete = false;
                uac.IsInsert = false;
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

                Map map = new Map("Port_Org", "独立组织");
                map.AdjunctType = AdjunctType.None;
                // map.EnType = EnType.View; //独立组织是一个视图.

                map.AddTBStringPK(OrgAttr.No, null, "编号(与部门编号相同)", true, false, 1, 30, 40);
                map.AddTBString(OrgAttr.Name, null, "组织名称", true, false, 0, 60, 200, true);

                map.AddTBString(OrgAttr.ParentNo, null, "父级组织编号", true, false, 0, 60, 200, true);
                map.AddTBString(OrgAttr.ParentName, null, "父级组织名称", true, false, 0, 60, 200, true);

                map.AddTBString(OrgAttr.Adminer, null, "管理员登录帐号", true, true, 0, 60, 200, true);
                map.AddTBString(OrgAttr.AdminerName, null, "管理员名称", true, true, 0, 60, 200, true);

                RefMethod rm = new RefMethod();
                rm.Title = "检查正确性";
                rm.ClassMethodName = this.ToString() + ".DoCheck";
                //rm.HisAttrs.AddTBString("No", null, "子公司管理员编号", true, false, 0, 100, 100);
                map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "设置二级管理员";
                //rm.Warning = "设置为子公司后，系统就会在流程树上分配一个目录节点.";
                //rm.ClassMethodName = this.ToString() + ".SetSubOrg";
                //rm.HisAttrs.AddTBString("No", null, "子公司管理员编号", true, false, 0, 100, 100);
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        public string DoCheck()
        {
            string err = "";


            #region 组织结构信息检查.
            //检查orgNo的部门是否存在？
            Dept dept = new Dept();
            dept.No = this.No;
            if (dept.RetrieveFromDBSources() == 0)
                return "err@部门组织结构树上缺少[" + this.No + "]的部门.";

            if (this.Name.Equals(dept.Name) == false)
            {
                this.Name = dept.Name;
                err += "info@部门名称与组织名称已经同步.";
            }

            Dept deptParent = new Dept();
            deptParent.No = this.ParentNo;
            if (deptParent.RetrieveFromDBSources() == 0)
                return "err@部门组织结构树上父级缺少[" + this.No + "]的部门.";

            if (this.ParentName.Equals(deptParent.Name) == false)
            {
                this.ParentName = deptParent.Name;
                err += "info@父级部门名称与组织名称已经同步.";

            }
            this.Update(); //执行更新.

            //设置子集部门，的OrgNo.
            if (DBAccess.IsView("Port_Dept") == false)
                SetSubDeptOrgNo(this.No);

            #endregion 组织结构信息检查.

            #region 检查流程树.
            BP.WF.Template.FlowSort fs = new WF.Template.FlowSort();
            fs.No = this.No;
            if (fs.RetrieveFromDBSources() == 1)
            {
                fs.OrgNo = this.No;
                fs.DirectUpdate();
            }
            else
            {
                //获得根目录节点.
                BP.WF.Template.FlowSort root = new Template.FlowSort();
                int i = root.Retrieve(BP.WF.Template.FlowSortAttr.ParentNo, "0");

                //设置流程树权限.
                fs.No = this.No;
                fs.Name = this.Name;
                fs.ParentNo = root.No;
                fs.OrgNo = this.No;
                fs.Idx = 999;
                fs.DirectInsert();

                //创建下一级目录.
                BP.WF.Template.FlowSort en = fs.DoCreateSubNode() as BP.WF.Template.FlowSort;
                en.Name = "公文类";
                en.OrgNo = this.No;
                en.Domain = "GongWen";
                en.DirectUpdate();

                en = fs.DoCreateSubNode() as BP.WF.Template.FlowSort;
                en.Name = "办公类";
                en.OrgNo = this.No;
                en.DirectUpdate();

                en = fs.DoCreateSubNode() as BP.WF.Template.FlowSort;
                en.Name = "财务类";
                en.OrgNo = this.No;
                en.DirectUpdate();

                en = fs.DoCreateSubNode() as BP.WF.Template.FlowSort;
                en.Name = "人力资源类";
                en.OrgNo = this.No;
                en.DirectUpdate();
            }
            #endregion 检查流程树.

            #region 检查表单树.
            //表单根目录.
            BP.Sys.FrmTree ftRoot = new Sys.FrmTree();
            ftRoot.Retrieve(BP.WF.Template.FlowSortAttr.ParentNo, "0");

            //设置表单树权限.
            BP.Sys.FrmTree ft = new Sys.FrmTree();
            ft.No = this.No;
            if (ft.RetrieveFromDBSources() == 0)
            {
                ft.Name = this.Name;
                ft.ParentNo = ftRoot.No;
                ft.OrgNo = this.No;
                ft.Idx = 999;
                ft.DirectInsert();

                //创建两个目录.
                BP.Sys.FrmTree mySubFT = ft.DoCreateSubNode() as BP.Sys.FrmTree;
                mySubFT.Name = "表单目录1";
                mySubFT.OrgNo = this.No;
                mySubFT.DirectUpdate();


                mySubFT = ft.DoCreateSubNode() as BP.Sys.FrmTree;
                mySubFT.Name = "表单目录2";
                mySubFT.OrgNo = this.No;
                mySubFT.DirectUpdate();

            }
            else
            {
                ft.Name = this.Name;
                ft.ParentNo = ftRoot.No;
                ft.OrgNo = this.No;
                ft.Idx = 999;
                ft.DirectUpdate();
            }
            #endregion 检查表单树.

            if (DataType.IsNullOrEmpty(err) == true)
                return "系统正确";

            //检查表单树.
            return "err@" + err;
        }
        /// <summary>
        /// 设置
        /// </summary>
        /// <param name="no"></param>
        public void SetSubDeptOrgNo(string no)
        {
            //同步当前部门与当前部门的子集部门，设置相同的orgNo.
            Depts subDepts = new Depts();
            subDepts.Retrieve(DeptAttr.ParentNo, no);
            foreach (Dept subDept in subDepts)
            {
                //判断当前部门是否是组织？
                Org org = new Org();
                org.No = subDept.No;
                if (org.RetrieveFromDBSources() == 1)
                    continue; //说明当前部门是组织.

                subDept.OrgNo = this.No;
                subDept.Update();

                //递归调用.
                SetSubDeptOrgNo(no);
            }
        }
        /// <summary>
        /// 检查组织结构信息.
        /// </summary>
        /// <returns></returns>
        public string CheckOrgInfo()
        {
            /*
             * 检查内容如下：
             * 1. 与org里面的部门是否存在？
             */



            return "";
        }


        //public string SetSubOrg(string userNo)
        //{
        //    BP.WF.Port.Admin2.Dept dept = new WF.Port.Admin2.Dept(this.No);
        //    return dept.SetSubOrg(userNo);
        //}

    }
    /// <summary>
    ///独立组织集合
    /// </summary>
    public class Orgs : EntitiesNoName
    {
        /// <summary>
        /// 得到一个新实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Org();
            }
        }
        /// <summary>
        /// create ens
        /// </summary>
        public Orgs()
        {
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Org> ToJavaList()
        {
            return (System.Collections.Generic.IList<Org>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Org> Tolist()
        {
            System.Collections.Generic.List<Org> list = new System.Collections.Generic.List<Org>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Org)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
