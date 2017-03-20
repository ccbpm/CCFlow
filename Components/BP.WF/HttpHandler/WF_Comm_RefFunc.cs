using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;

namespace BP.WF.HttpHandler
{
    public class WF_Comm_RefFunc : WebContralBase
    {
        /// <summary>
        /// 初始化数据
        /// </summary>
        /// <param name="mycontext"></param>
        public WF_Comm_RefFunc(HttpContext mycontext)
        {
            this.context = mycontext;
        }

        #region Dot2DotTreeDeptEmpModel.htm（部门人员选择）

        /// <summary>
        /// 保存节点绑定人员信息
        /// </summary>
        /// <returns></returns>
        public string Dot2DotTreeDeptEmpModel_SaveNodeEmps()
        {
            JsonResultInnerData jr = new JsonResultInnerData();
            string nodeid = this.GetRequestVal("nodeid");
            string data = this.GetRequestVal("data");
            string partno = this.GetRequestVal("partno");
            bool lastpart = false;
            int partidx = 0;
            int partcount = 0;
            int nid = 0;

            if (string.IsNullOrWhiteSpace(nodeid) || int.TryParse(nodeid, out nid) == false)
                throw new Exception("参数nodeid不正确");

            if (string.IsNullOrWhiteSpace(data))
                data = "";

            BP.WF.Template.NodeEmps nemps = new BP.WF.Template.NodeEmps();
            string[] empNos = data.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            //提交内容过长时，采用分段式提交
            if (string.IsNullOrWhiteSpace(partno))
            {
                nemps.Delete(BP.WF.Template.NodeEmpAttr.FK_Node, nid);
            }
            else
            {
                string[] parts = partno.Split("/".ToCharArray());

                if (parts.Length != 2)
                    throw new Exception("参数partno不正确");

                partidx = int.Parse(parts[0]);
                partcount = int.Parse(parts[1]);

                empNos = data.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (partidx == 1)
                    nemps.Delete(BP.WF.Template.NodeEmpAttr.FK_Node, nid);

                lastpart = partidx == partcount;
            }

            DataTable dtEmps = DBAccess.RunSQLReturnTable("SELECT No FROM Port_Emp");
            BP.WF.Template.NodeEmp nemp = null;

            foreach (string empNo in empNos)
            {
                if (dtEmps.Select(string.Format("No='{0}'", empNo)).Length == 0)
                    continue;

                nemp = new BP.WF.Template.NodeEmp();
                nemp.FK_Node = nid;
                nemp.FK_Emp = empNo;
                nemp.Insert();
            }

            if (string.IsNullOrWhiteSpace(partno))
            {
                jr.Msg = "保存成功";
            }
            else
            {
                jr.InnerData = new { lastpart, partidx, partcount };

                if (lastpart)
                    jr.Msg = "保存成功";
                else
                    jr.Msg = string.Format("第{0}/{1}段保存成功", partidx, partcount);
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(jr);
        }

        /// <summary>
        /// 获取部门树根结点
        /// </summary>
        /// <returns></returns>
        public string Dot2DotTreeDeptEmpModel_GetStructureTreeRoot()
        {
            JsonResultInnerData jr = new JsonResultInnerData();

            EasyuiTreeNode node = null;
            List<EasyuiTreeNode> d = new List<EasyuiTreeNode>();
            string parentrootid = this.GetRequestVal("parentrootid");

            if (string.IsNullOrWhiteSpace(parentrootid))
                throw new Exception("参数parentrootid不能为空");

            if (BP.WF.Glo.OSModel == OSModel.OneOne)
            {
                BP.WF.Port.Dept dept = new BP.WF.Port.Dept();

                if (dept.Retrieve(BP.WF.Port.DeptAttr.ParentNo, parentrootid) == 0)
                {
                    dept.No = "-1";
                    dept.Name = "无部门";
                    dept.ParentNo = "";
                }

                node = new EasyuiTreeNode();
                node.id = "DEPT_" + dept.No;
                node.text = dept.Name;
                node.iconCls = "icon-department";
                node.attributes = new EasyuiTreeNodeAttributes();
                node.attributes.No = dept.No;
                node.attributes.Name = dept.Name;
                node.attributes.ParentNo = dept.ParentNo;
                node.attributes.TType = "DEPT";
                node.state = "closed";
                node.children = new List<EasyuiTreeNode>();
                node.children.Add(new EasyuiTreeNode());
                node.children[0].text = "loading...";

                d.Add(node);
            }
            else
            {
                BP.GPM.Dept dept = new BP.GPM.Dept();

                if (dept.Retrieve(BP.GPM.DeptAttr.ParentNo, parentrootid) == 0)
                {
                    dept.No = "-1";
                    dept.Name = "无部门";
                    dept.ParentNo = "";
                }

                node = new EasyuiTreeNode();
                node.id = "DEPT_" + dept.No;
                node.text = dept.Name;
                node.iconCls = "icon-department";
                node.attributes = new EasyuiTreeNodeAttributes();
                node.attributes.No = dept.No;
                node.attributes.Name = dept.Name;
                node.attributes.ParentNo = dept.ParentNo;
                node.attributes.TType = "DEPT";
                node.state = "closed";
                node.children = new List<EasyuiTreeNode>();
                node.children.Add(new EasyuiTreeNode());
                node.children[0].text = "loading...";

                d.Add(node);
            }

            jr.InnerData = d;

            return Newtonsoft.Json.JsonConvert.SerializeObject(jr);
        }

        /// <summary>
        /// 获取指定部门下一级子部门及人员列表
        /// </summary>
        /// <returns></returns>
        public string Dot2DotTreeDeptEmpModel_GetSubDepts()
        {
            JsonResultInnerData jr = new JsonResultInnerData();

            EasyuiTreeNode node = null;
            JsonResultEmp jremp = null;
            List<JsonResultEmp> jremps = new List<JsonResultEmp>();
            List<EasyuiTreeNode> d = new List<EasyuiTreeNode>();
            List<EasyuiTreeNode> parentNodes = new List<EasyuiTreeNode>();
            EasyuiTreeNode parent = null;
            string rootid = this.GetRequestVal("rootid");
            string parentid = this.GetRequestVal("parentid");
            string nid = this.GetRequestVal("nodeid");
            string currparentid = parentid;
            BP.WF.Template.NodeEmps semps = new BP.WF.Template.NodeEmps();

            if (string.IsNullOrWhiteSpace(rootid))
                throw new Exception("参数rootid不能为空");
            if (string.IsNullOrWhiteSpace(parentid))
                throw new Exception("参数parentid不能为空");
            if (string.IsNullOrWhiteSpace(nid))
                throw new Exception("参数nodeid不能为空");

            semps.Retrieve(BP.WF.Template.NodeEmpAttr.FK_Node, int.Parse(nid));

            if (BP.WF.Glo.OSModel == OSModel.OneOne)
            {
                //逐级计算当前部门下的所有父级部门
                do
                {
                    BP.Port.Dept parentdept = new BP.Port.Dept(parentid);

                    parent = new EasyuiTreeNode();
                    parent.id = "DEPT_" + parentdept.No;
                    parent.text = parentdept.Name;
                    parent.iconCls = "icon-department";
                    parent.attributes = new EasyuiTreeNodeAttributes();
                    parent.attributes.No = parentdept.No;
                    parent.attributes.Name = parentdept.Name;
                    parent.attributes.ParentNo = parentdept.ParentNo;
                    parent.attributes.TType = parentid == currparentid ? "CDEPT" : "PDEPT";
                    parent.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(parentdept.Name);
                    parent.state = "open";
                    parent.children = new List<EasyuiTreeNode>();

                    parentNodes.Add(parent);
                    parentid = parentdept.ParentNo;
                } while (parent.attributes.No != rootid);

                //生成父级tree结构
                for (int i = parentNodes.Count - 1; i >= 0; i--)
                {
                    parent = parentNodes[i];

                    if (i == parentNodes.Count - 1)
                        d.Add(parent);

                    if (i == 0)
                        break;

                    parent.children.Add(parentNodes[i - 1]);
                }

                BP.Port.Depts depts = new BP.Port.Depts();
                depts.Retrieve(BP.Port.DeptAttr.ParentNo, parent.attributes.No, BP.Port.DeptAttr.Name);
                BP.Port.Emps emps = new BP.Port.Emps();
                emps.Retrieve(BP.Port.EmpAttr.FK_Dept, parent.attributes.No, BP.Port.EmpAttr.Name);

                //增加部门
                foreach (BP.Port.Dept dept in depts)
                {
                    node = new EasyuiTreeNode();
                    node.id = "DEPT_" + dept.No;
                    node.text = dept.Name;
                    node.iconCls = "icon-department";
                    node.attributes = new EasyuiTreeNodeAttributes();
                    node.attributes.No = dept.No;
                    node.attributes.Name = dept.Name;
                    node.attributes.ParentNo = dept.ParentNo;
                    node.attributes.TType = "DEPT";
                    node.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(dept.Name);
                    node.state = "closed";
                    node.children = new List<EasyuiTreeNode>();
                    node.children.Add(new EasyuiTreeNode());
                    node.children[0].text = "loading...";

                    parent.children.Add(node);
                }

                //增加人员
                foreach (BP.Port.Emp emp in emps)
                {
                    node = new EasyuiTreeNode();
                    node.id = "EMP_" + parent.attributes.No + "_" + emp.No;
                    node.text = emp.Name;
                    node.iconCls = "icon-user";
                    node.@checked = semps.GetEntityByKey(BP.WF.Template.NodeEmpAttr.FK_Emp, emp.No) != null;
                    node.attributes = new EasyuiTreeNodeAttributes();
                    node.attributes.No = emp.No;
                    node.attributes.Name = emp.Name;
                    node.attributes.ParentNo = parent.attributes.No;
                    node.attributes.TType = "EMP";
                    node.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(emp.Name);

                    parent.children.Add(node);

                    jremp = new JsonResultEmp();
                    jremp.No = emp.No;
                    jremp.Name = emp.Name;
                    jremp.DeptNo = parent.attributes.No;
                    jremp.DeptName = parent.attributes.Name;
                    jremp.Checked = node.@checked;
                    jremp.Code = BP.Tools.chs2py.ConvertStr2Code(emp.Name);

                    jremps.Add(jremp);
                }
            }
            else
            {
                //逐级计算当前部门下的所有父级部门
                do
                {
                    BP.GPM.Dept parentdept = new BP.GPM.Dept(parentid);

                    parent = new EasyuiTreeNode();
                    parent.id = "DEPT_" + parentdept.No;
                    parent.text = parentdept.Name;
                    parent.iconCls = "icon-department";
                    parent.attributes = new EasyuiTreeNodeAttributes();
                    parent.attributes.No = parentdept.No;
                    parent.attributes.Name = parentdept.Name;
                    parent.attributes.ParentNo = parentdept.ParentNo;
                    parent.attributes.TType = parentid == currparentid ? "CDEPT" : "PDEPT";
                    parent.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(parentdept.Name);
                    parent.state = "open";
                    parent.children = new List<EasyuiTreeNode>();

                    parentNodes.Add(parent);
                    parentid = parentdept.ParentNo;
                } while (parent.attributes.No != rootid);

                //生成父级tree结构
                for (int i = parentNodes.Count - 1; i >= 0; i--)
                {
                    parent = parentNodes[i];

                    if (i == parentNodes.Count - 1)
                        d.Add(parent);

                    if (i == 0)
                        break;

                    parent.children.Add(parentNodes[i - 1]);
                }

                BP.GPM.Depts depts = new BP.GPM.Depts();
                depts.Retrieve(BP.GPM.DeptAttr.ParentNo, parent.attributes.No);
                BP.GPM.DeptEmps des = new BP.GPM.DeptEmps();
                des.Retrieve(BP.GPM.DeptEmpAttr.FK_Dept, parent.attributes.No);
                BP.GPM.Emps emps = new BP.GPM.Emps();
                emps.RetrieveAll(BP.GPM.EmpAttr.Name);
                BP.GPM.Emp emp = null;

                //增加部门
                foreach (BP.GPM.Dept dept in depts)
                {
                    node = new EasyuiTreeNode();
                    node.id = "DEPT_" + dept.No;
                    node.text = dept.Name;
                    node.iconCls = "icon-department";
                    node.attributes = new EasyuiTreeNodeAttributes();
                    node.attributes.No = dept.No;
                    node.attributes.Name = dept.Name;
                    node.attributes.ParentNo = dept.ParentNo;
                    node.attributes.TType = "DEPT";
                    node.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(dept.Name);
                    node.state = "closed";
                    node.children = new List<EasyuiTreeNode>();
                    node.children.Add(new EasyuiTreeNode());
                    node.children[0].text = "loading...";

                    parent.children.Add(node);
                }

                //增加人员
                foreach (BP.GPM.DeptEmp de in des)
                {
                    emp = emps.GetEntityByKey(BP.GPM.EmpAttr.No, de.FK_Emp) as BP.GPM.Emp;

                    if (emp == null)
                        continue;

                    node = new EasyuiTreeNode();
                    node.id = "EMP_" + parent.attributes.No + "_" + emp.No;
                    node.text = emp.Name;
                    node.iconCls = "icon-user";
                    node.@checked = semps.GetEntityByKey(BP.WF.Template.NodeEmpAttr.FK_Emp, emp.No) != null;
                    node.attributes = new EasyuiTreeNodeAttributes();
                    node.attributes.No = emp.No;
                    node.attributes.Name = emp.Name;
                    node.attributes.ParentNo = parent.attributes.No;
                    node.attributes.TType = "EMP";
                    node.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(emp.Name);

                    parent.children.Add(node);

                    jremp = new JsonResultEmp();
                    jremp.No = emp.No;
                    jremp.Name = emp.Name;
                    jremp.DeptNo = parent.attributes.No;
                    jremp.DeptName = parent.attributes.Name;
                    jremp.Checked = node.@checked;
                    jremp.Code = BP.Tools.chs2py.ConvertStr2Code(emp.Name);

                    jremps.Add(jremp);
                }
            }

            jr.InnerData = new { TreeData = d, DeptEmps = jremps };

            return Newtonsoft.Json.JsonConvert.SerializeObject(jr);
        }

        /// <summary>
        /// 获取节点绑定人员信息列表
        /// </summary>
        /// <returns></returns>
        public string Dot2DotTreeDeptEmpModel_GetNodeEmps()
        {
            JsonResultInnerData jr = new JsonResultInnerData();

            DataTable dt = null;
            string nid = this.GetRequestVal("nodeid");
            int pagesize = int.Parse(this.GetRequestVal("pagesize"));
            int pageidx = int.Parse(this.GetRequestVal("pageidx"));
            string sql = "SELECT pe.No,pe.Name,pd.No DeptNo,pd.Name DeptName FROM WF_NodeEmp wne "
                         + "  INNER JOIN Port_Emp pe ON pe.No = wne.FK_Emp "
                         + "  INNER JOIN Port_Dept pd ON pd.No = pe.FK_Dept "
                         + "WHERE wne.FK_Node = " + nid + " ORDER BY pe.Name";

            dt = DBAccess.RunSQLReturnTable(sql);   //, pagesize, pageidx, "No", "Name", "ASC"
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Checked", typeof(bool));

            foreach (DataRow row in dt.Rows)
            {
                row["Code"] = BP.Tools.chs2py.ConvertStr2Code(row["Name"] as string);
                row["Checked"] = true;
            }

            //对Oracle数据库做兼容性处理
            if (DBAccess.AppCenterDBType == DBType.Oracle)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    switch (col.ColumnName)
                    {
                        case "NO":
                            col.ColumnName = "No";
                            break;
                        case "NAME":
                            col.ColumnName = "Name";
                            break;
                        case "DEPTNO":
                            col.ColumnName = "DeptNo";
                            break;
                        case "DEPTNAME":
                            col.ColumnName = "DeptName";
                            break;
                    }
                }
            }

            jr.InnerData = dt;

            return Newtonsoft.Json.JsonConvert.SerializeObject(jr);
        }
        #endregion Dot2DotTreeDeptEmpModel.htm（部门人员选择）

        #region Dot2DotTreeDeptModel.htm（部门选择）

        /// <summary>
        /// 保存节点绑定部门信息
        /// </summary>
        /// <returns></returns>
        public string Dot2DotTreeDeptModel_SaveNodeDepts()
        {
            JsonResultInnerData jr = new JsonResultInnerData();
            string nodeid = this.GetRequestVal("nodeid");
            string data = this.GetRequestVal("data");
            string partno = this.GetRequestVal("partno");
            bool lastpart = false;
            int partidx = 0;
            int partcount = 0;
            int nid = 0;

            if (string.IsNullOrWhiteSpace(nodeid) || int.TryParse(nodeid, out nid) == false)
                throw new Exception("参数nodeid不正确");

            if (string.IsNullOrWhiteSpace(data))
                data = "";

            BP.WF.Template.NodeDepts ndepts = new BP.WF.Template.NodeDepts();
            string[] deptNos = data.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            //提交内容过长时，采用分段式提交
            if (string.IsNullOrWhiteSpace(partno))
            {
                ndepts.Delete(BP.WF.Template.NodeDeptAttr.FK_Node, nid);
            }
            else
            {
                string[] parts = partno.Split("/".ToCharArray());

                if (parts.Length != 2)
                    throw new Exception("参数partno不正确");

                partidx = int.Parse(parts[0]);
                partcount = int.Parse(parts[1]);

                deptNos = data.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (partidx == 1)
                    ndepts.Delete(BP.WF.Template.NodeDeptAttr.FK_Node, nid);

                lastpart = partidx == partcount;
            }

            DataTable dtDepts = DBAccess.RunSQLReturnTable("SELECT No FROM Port_Dept");
            BP.WF.Template.NodeDept nemp = null;

            foreach (string deptNo in deptNos)
            {
                if (dtDepts.Select(string.Format("No='{0}'", deptNo)).Length == 0)
                    continue;

                nemp = new BP.WF.Template.NodeDept();
                nemp.FK_Node = nid;
                nemp.FK_Dept = deptNo;
                nemp.Insert();
            }

            if (string.IsNullOrWhiteSpace(partno))
            {
                jr.Msg = "保存成功";
            }
            else
            {
                jr.InnerData = new { lastpart, partidx, partcount };

                if (lastpart)
                    jr.Msg = "保存成功";
                else
                    jr.Msg = string.Format("第{0}/{1}段保存成功", partidx, partcount);
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(jr);
        }
        /// <summary>
        /// 获取指定部门下一级子部门列表
        /// </summary>
        /// <returns></returns>
        public string Dot2DotTreeDeptModel_GetSubDepts()
        {
            JsonResultInnerData jr = new JsonResultInnerData();

            EasyuiTreeNode node = null;
            JsonResultEmp jrdept = null;
            List<JsonResultEmp> jrdepts = new List<JsonResultEmp>();
            List<EasyuiTreeNode> d = new List<EasyuiTreeNode>();
            List<EasyuiTreeNode> parentNodes = new List<EasyuiTreeNode>();
            EasyuiTreeNode parent = null;
            string rootid = this.GetRequestVal("rootid");
            string parentid = this.GetRequestVal("parentid");
            string nid = this.GetRequestVal("nodeid");
            string currparentid = parentid;
            BP.WF.Template.NodeDepts sdepts = new BP.WF.Template.NodeDepts();

            if (string.IsNullOrWhiteSpace(rootid))
                throw new Exception("参数rootid不能为空");
            if (string.IsNullOrWhiteSpace(parentid))
                throw new Exception("参数parentid不能为空");
            if (string.IsNullOrWhiteSpace(nid))
                throw new Exception("参数nodeid不能为空");

            sdepts.Retrieve(BP.WF.Template.NodeDeptAttr.FK_Node, int.Parse(nid));

            if (BP.WF.Glo.OSModel == OSModel.OneOne)
            {
                //逐级计算当前部门下的所有父级部门
                do
                {
                    BP.Port.Dept parentdept = new BP.Port.Dept(parentid);

                    parent = new EasyuiTreeNode();
                    parent.id = "DEPT_" + parentdept.No;
                    parent.text = parentdept.Name;
                    parent.iconCls = "icon-department";
                    parent.attributes = new EasyuiTreeNodeAttributes();
                    parent.attributes.No = parentdept.No;
                    parent.attributes.Name = parentdept.Name;
                    parent.attributes.ParentNo = parentdept.ParentNo;
                    parent.attributes.TType = parentid == currparentid ? "CDEPT" : "PDEPT";
                    parent.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(parentdept.Name);
                    parent.state = "open";
                    parent.children = new List<EasyuiTreeNode>();

                    parentNodes.Add(parent);
                    parentid = parentdept.ParentNo;
                } while (parent.attributes.No != rootid);

                //生成父级tree结构
                for (int i = parentNodes.Count - 1; i >= 0; i--)
                {
                    parent = parentNodes[i];

                    if (i == parentNodes.Count - 1)
                        d.Add(parent);

                    if (i == 0)
                        break;

                    parent.children.Add(parentNodes[i - 1]);
                }

                BP.Port.Depts depts = new BP.Port.Depts();
                depts.Retrieve(BP.Port.DeptAttr.ParentNo, parent.attributes.No, BP.Port.DeptAttr.Name);

                //增加部门
                foreach (BP.Port.Dept dept in depts)
                {
                    node = new EasyuiTreeNode();
                    node.id = "DEPT_" + dept.No;
                    node.text = dept.Name;
                    node.iconCls = "icon-department";
                    node.@checked = sdepts.GetEntityByKey(BP.WF.Template.NodeDeptAttr.FK_Dept, dept.No) != null;
                    node.attributes = new EasyuiTreeNodeAttributes();
                    node.attributes.No = dept.No;
                    node.attributes.Name = dept.Name;
                    node.attributes.ParentNo = dept.ParentNo;
                    node.attributes.TType = "DEPT";
                    node.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(dept.Name);
                    node.state = "closed";
                    node.children = new List<EasyuiTreeNode>();
                    node.children.Add(new EasyuiTreeNode());
                    node.children[0].text = "loading...";

                    parent.children.Add(node);

                    jrdept = new JsonResultEmp();
                    jrdept.No = dept.No;
                    jrdept.Name = dept.Name;
                    jrdept.DeptNo = dept.ParentNo;
                    jrdept.DeptName = parent.attributes.Name;
                    jrdept.Checked = node.@checked;
                    jrdept.Code = node.attributes.Code;

                    jrdepts.Add(jrdept);
                }
            }
            else
            {
                //逐级计算当前部门下的所有父级部门
                do
                {
                    BP.GPM.Dept parentdept = new BP.GPM.Dept(parentid);

                    parent = new EasyuiTreeNode();
                    parent.id = "DEPT_" + parentdept.No;
                    parent.text = parentdept.Name;
                    parent.iconCls = "icon-department";
                    parent.attributes = new EasyuiTreeNodeAttributes();
                    parent.attributes.No = parentdept.No;
                    parent.attributes.Name = parentdept.Name;
                    parent.attributes.ParentNo = parentdept.ParentNo;
                    parent.attributes.TType = parentid == currparentid ? "CDEPT" : "PDEPT";
                    parent.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(parentdept.Name);
                    parent.state = "open";
                    parent.children = new List<EasyuiTreeNode>();

                    parentNodes.Add(parent);
                    parentid = parentdept.ParentNo;
                } while (parent.attributes.No != rootid);

                //生成父级tree结构
                for (int i = parentNodes.Count - 1; i >= 0; i--)
                {
                    parent = parentNodes[i];

                    if (i == parentNodes.Count - 1)
                        d.Add(parent);

                    if (i == 0)
                        break;

                    parent.children.Add(parentNodes[i - 1]);
                }

                BP.GPM.Depts depts = new BP.GPM.Depts();
                depts.Retrieve(BP.GPM.DeptAttr.ParentNo, parent.attributes.No);

                //增加部门
                foreach (BP.GPM.Dept dept in depts)
                {
                    node = new EasyuiTreeNode();
                    node.id = "DEPT_" + dept.No;
                    node.text = dept.Name;
                    node.iconCls = "icon-department";
                    node.@checked = sdepts.GetEntityByKey(BP.WF.Template.NodeDeptAttr.FK_Dept, dept.No) != null;
                    node.attributes = new EasyuiTreeNodeAttributes();
                    node.attributes.No = dept.No;
                    node.attributes.Name = dept.Name;
                    node.attributes.ParentNo = dept.ParentNo;
                    node.attributes.TType = "DEPT";
                    node.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(dept.Name);
                    node.state = "closed";
                    node.children = new List<EasyuiTreeNode>();
                    node.children.Add(new EasyuiTreeNode());
                    node.children[0].text = "loading...";

                    parent.children.Add(node);

                    jrdept = new JsonResultEmp();
                    jrdept.No = dept.No;
                    jrdept.Name = dept.Name;
                    jrdept.DeptNo = dept.ParentNo;
                    jrdept.DeptName = parent.attributes.Name;
                    jrdept.Checked = node.@checked;
                    jrdept.Code = node.attributes.Code;

                    jrdepts.Add(jrdept);
                }
            }

            jr.InnerData = new { TreeData = d, Depts = jrdepts };

            return Newtonsoft.Json.JsonConvert.SerializeObject(jr);
        }

        /// <summary>
        /// 获取节点绑定人员信息列表
        /// </summary>
        /// <returns></returns>
        public string Dot2DotTreeDeptModel_GetNodeDepts()
        {
            JsonResultInnerData jr = new JsonResultInnerData();

            DataTable dt = null;
            string nid = this.GetRequestVal("nodeid");
            string sql = "SELECT pd.No,pd.Name,pd1.No DeptNo,pd1.Name DeptName FROM WF_NodeDept wnd "
                         + "  INNER JOIN Port_Dept pd ON pd.No = wnd.FK_Dept "
                         + "  LEFT JOIN Port_Dept pd1 ON pd1.No = pd.ParentNo "
                         + "WHERE wnd.FK_Node = " + nid + " ORDER BY pd.Name";

            dt = DBAccess.RunSQLReturnTable(sql);   //, pagesize, pageidx, "No", "Name", "ASC"
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Checked", typeof(bool));

            foreach (DataRow row in dt.Rows)
            {
                row["Code"] = BP.Tools.chs2py.ConvertStr2Code(row["Name"] as string);
                row["Checked"] = true;
            }

            //对Oracle数据库做兼容性处理
            if (DBAccess.AppCenterDBType == DBType.Oracle)
            {
                foreach (DataColumn col in dt.Columns)
                {
                    switch (col.ColumnName)
                    {
                        case "NO":
                            col.ColumnName = "No";
                            break;
                        case "NAME":
                            col.ColumnName = "Name";
                            break;
                        case "DEPTNO":
                            col.ColumnName = "DeptNo";
                            break;
                        case "DEPTNAME":
                            col.ColumnName = "DeptName";
                            break;
                    }
                }
            }

            jr.InnerData = dt;

            return Newtonsoft.Json.JsonConvert.SerializeObject(jr);
        }
        #endregion Dot2DotTreeDeptModel.htm（部门选择）

        #region 辅助实体定义

        /// <summary>
        /// Eayui tree node对象
        /// <para>主要用于数据的JSON化组织</para>
        /// </summary>
        public class EasyuiTreeNode
        {
            public string id { get; set; }
            public string text { get; set; }
            public string state { get; set; }
            public bool @checked { get; set; }
            public string iconCls { get; set; }
            public EasyuiTreeNodeAttributes attributes { get; set; }
            public List<EasyuiTreeNode> children { get; set; }
        }

        public class EasyuiTreeNodeAttributes
        {
            public string No { get; set; }
            public string Name { get; set; }
            public string ParentNo { get; set; }
            public string TType { get; set; }
            public string Code { get; set; }
        }

        public class JsonResultEmp
        {
            public string No { get; set; }
            public string Name { get; set; }
            public string DeptNo { get; set; }
            public string DeptName { get; set; }
            public bool Checked { get; set; }
            public string Code { get; set; }
        }
        #endregion 辅助实体定义
    }
}
