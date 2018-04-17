using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web;
using BP.DA;
using BP.Sys;
using BP.WF.Template;

namespace BP.WF.HttpHandler
{
    public class WF_Comm_RefFunc : DirectoryPageBase
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
            string[] empNos = data.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

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

                empNos = data.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (partidx == 1)
                    nemps.Delete(BP.WF.Template.NodeEmpAttr.FK_Node, nid);

                lastpart = partidx == partcount;
            }

            DataTable dtEmps = DBAccess.RunSQLReturnTable("SELECT No FROM Port_Emp");
            BP.WF.Template.NodeEmp nemp = null;

            foreach (string empNo in empNos)
            {
                if (dtEmps.Select(string.Format("No='{0}'", empNo)).Length + dtEmps.Select(string.Format("NO='{0}'", empNo)).Length == 0)
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
            string parentid = this.GetRequestVal("parentid");
            string nid = this.GetRequestVal("nodeid");

            if (string.IsNullOrWhiteSpace(parentid))
                throw new Exception("参数parentid不能为空");
            if (string.IsNullOrWhiteSpace(nid))
                throw new Exception("参数nodeid不能为空");

            EasyuiTreeNode node = null;
            List<EasyuiTreeNode> d = new List<EasyuiTreeNode>();
            BP.WF.Template.NodeEmps semps = new BP.WF.Template.NodeEmps();

            semps.Retrieve(BP.WF.Template.NodeEmpAttr.FK_Node, int.Parse(nid));

            if (BP.WF.Glo.OSModel == OSModel.OneOne)
            {
                BP.Port.Dept parentDept = new BP.Port.Dept(parentid);
                BP.Port.Depts depts = new BP.Port.Depts();
                depts.Retrieve(BP.Port.DeptAttr.ParentNo, parentid, BP.Port.DeptAttr.Name);
                BP.Port.Emps emps = new BP.Port.Emps();
                emps.Retrieve(BP.Port.EmpAttr.FK_Dept, parentid, BP.Port.EmpAttr.Name);

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

                    d.Add(node);
                }

                //增加人员
                foreach (BP.Port.Emp emp in emps)
                {
                    node = new EasyuiTreeNode();
                    node.id = "EMP_" + parentid + "_" + emp.No;
                    node.text = emp.Name;
                    node.iconCls = "icon-user";
                    node.@checked = semps.GetEntityByKey(BP.WF.Template.NodeEmpAttr.FK_Emp, emp.No) != null;
                    node.attributes = new EasyuiTreeNodeAttributes();
                    node.attributes.No = emp.No;
                    node.attributes.Name = emp.Name;
                    node.attributes.ParentNo = parentDept.No;
                    node.attributes.ParentName = parentDept.Name;
                    node.attributes.TType = "EMP";
                    node.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(emp.Name);

                    d.Add(node);
                }
            }
            else
            {
                BP.GPM.Dept parentDept = new BP.GPM.Dept(parentid);
                BP.GPM.Depts depts = new BP.GPM.Depts();
                depts.Retrieve(BP.GPM.DeptAttr.ParentNo, parentid, BP.GPM.DeptAttr.Name);
                BP.GPM.DeptEmps des = new BP.GPM.DeptEmps();
                des.Retrieve(BP.GPM.DeptEmpAttr.FK_Dept, parentid);
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
                    node.attributes.ParentNo = parentDept.No;
                    node.attributes.ParentName = parentDept.Name;
                    node.attributes.TType = "DEPT";
                    node.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(dept.Name);
                    node.state = "closed";
                    node.children = new List<EasyuiTreeNode>();
                    node.children.Add(new EasyuiTreeNode());
                    node.children[0].text = "loading...";

                    d.Add(node);
                }

                //增加人员
                foreach (BP.GPM.DeptEmp de in des)
                {
                    emp = emps.GetEntityByKey(BP.GPM.EmpAttr.No, de.FK_Emp) as BP.GPM.Emp;

                    if (emp == null)
                        continue;

                    node = new EasyuiTreeNode();
                    node.id = "EMP_" + parentid + "_" + emp.No;
                    node.text = emp.Name;
                    node.iconCls = "icon-user";
                    node.@checked = semps.GetEntityByKey(BP.WF.Template.NodeEmpAttr.FK_Emp, emp.No) != null;
                    node.attributes = new EasyuiTreeNodeAttributes();
                    node.attributes.No = emp.No;
                    node.attributes.Name = emp.Name;
                    node.attributes.ParentNo = parentDept.No;
                    node.attributes.ParentName = parentDept.Name;
                    node.attributes.TType = "EMP";
                    node.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(emp.Name);

                    d.Add(node);
                }
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(d);
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
                         + "  LEFT JOIN Port_Dept pd ON pd.No = pe.FK_Dept "
                         + "WHERE wne.FK_Node = " + nid + " ORDER BY pd.Idx, pe.Name";

            dt = DBAccess.RunSQLReturnTable(sql);   //, pagesize, pageidx, "No", "Name", "ASC"
            dt.Columns.Add("Code", typeof(string));
            dt.Columns.Add("Checked", typeof(bool));

            foreach (DataRow row in dt.Rows)
            {
                if ((Glo.Plant == BP.WF.Plant.JFlow) && (DBAccess.AppCenterDBType == DBType.Oracle))
                    row["Code"] = BP.Tools.chs2py.ConvertStr2Code(row["NAME"] as string);
                else
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
        /// 获取部门树根结点
        /// </summary>
        /// <returns></returns>
        public string Dot2DotTreeDeptModel_GetStructureTreeRoot()
        {
            JsonResultInnerData jr = new JsonResultInnerData();

            EasyuiTreeNode node = null;
            List<EasyuiTreeNode> d = new List<EasyuiTreeNode>();
            string parentrootid = this.GetRequestVal("parentrootid");
            string nodeid = this.GetRequestVal("nodeid");

            if (string.IsNullOrWhiteSpace(parentrootid))
                throw new Exception("参数parentrootid不能为空");
            if (string.IsNullOrWhiteSpace(nodeid))
                throw new Exception("参数nodeid不能为空");

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
                node.@checked = new NodeDept().IsExit(NodeDeptAttr.FK_Node, int.Parse(nodeid), NodeDeptAttr.FK_Dept,
                                                      dept.No);
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
                node.@checked = new NodeDept().IsExit(NodeDeptAttr.FK_Node, int.Parse(nodeid), NodeDeptAttr.FK_Dept,
                                                      dept.No);
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
                if (dtDepts.Select(string.Format("No='{0}'", deptNo)).Length + dtDepts.Select(string.Format("NO='{0}'", deptNo)).Length == 0)
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
            string parentid = this.GetRequestVal("parentid");
            string nid = this.GetRequestVal("nodeid");

            if (string.IsNullOrWhiteSpace(parentid))
                throw new Exception("参数parentid不能为空");
            if (string.IsNullOrWhiteSpace(nid))
                throw new Exception("参数nodeid不能为空");

            EasyuiTreeNode node = null;
            List<EasyuiTreeNode> d = new List<EasyuiTreeNode>();
            BP.WF.Template.NodeDepts sdepts = new BP.WF.Template.NodeDepts();

            sdepts.Retrieve(BP.WF.Template.NodeDeptAttr.FK_Node, int.Parse(nid));

            if (BP.WF.Glo.OSModel == OSModel.OneOne)
            {
                BP.Port.Dept parentDept = new BP.Port.Dept(parentid);
                BP.Port.Depts depts = new BP.Port.Depts();
                depts.Retrieve(BP.Port.DeptAttr.ParentNo, parentid);

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
                    node.attributes.ParentNo = parentDept.No;
                    node.attributes.ParentName = parentDept.Name;
                    node.attributes.TType = "DEPT";
                    node.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(dept.Name);
                    node.state = "closed";
                    node.children = new List<EasyuiTreeNode>();
                    node.children.Add(new EasyuiTreeNode());
                    node.children[0].text = "loading...";

                    d.Add(node);
                }
            }
            else
            {
                BP.GPM.Dept parentDept = new BP.GPM.Dept(parentid);
                BP.GPM.Depts depts = new BP.GPM.Depts();
                depts.Retrieve(BP.GPM.DeptAttr.ParentNo, parentid, BP.GPM.DeptAttr.Idx);

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
                    node.attributes.ParentNo = parentDept.No;
                    node.attributes.ParentName = parentDept.Name;
                    node.attributes.TType = "DEPT";
                    node.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(dept.Name);
                    node.state = "closed";
                    node.children = new List<EasyuiTreeNode>();
                    node.children.Add(new EasyuiTreeNode());
                    node.children[0].text = "loading...";

                    d.Add(node);
                }
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(d);
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
                         + "WHERE wnd.FK_Node = " + nid + " ORDER BY pd1.Idx, pd.Name";

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
            string re = BP.Tools.Json.ToJson(jr);
            if (Glo.Plant == BP.WF.Plant.JFlow)
            {
                re = re.Replace("\"NO\"", "\"No\"").Replace("\"NAME\"", "\"Name\"").Replace("\"DEPTNO\"", "\"DeptNo\"").Replace("\"DEPTNAME\"", "\"DeptName\"");
            }
            return re;
        }
        #endregion Dot2DotTreeDeptModel.htm（部门选择）

        #region Dot2DotStationModel.htm（岗位选择）

        /// <summary>
        /// 保存节点绑定岗位信息
        /// </summary>
        /// <returns></returns>
        public string Dot2DotStationModel_SaveNodeStations()
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

            BP.WF.Template.NodeStations nsts = new BP.WF.Template.NodeStations();
            string[] stNos = data.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

            //提交内容过长时，采用分段式提交
            if (string.IsNullOrWhiteSpace(partno))
            {
                nsts.Delete(BP.WF.Template.NodeStationAttr.FK_Node, nid);
            }
            else
            {
                string[] parts = partno.Split("/".ToCharArray());

                if (parts.Length != 2)
                    throw new Exception("参数partno不正确");

                partidx = int.Parse(parts[0]);
                partcount = int.Parse(parts[1]);

                stNos = data.Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                if (partidx == 1)
                    nsts.Delete(BP.WF.Template.NodeStationAttr.FK_Node, nid);

                lastpart = partidx == partcount;
            }

            DataTable dtSts = DBAccess.RunSQLReturnTable("SELECT No FROM Port_Station");
            BP.WF.Template.NodeStation nst = null;

            foreach (string stNo in stNos)
            {
                if (dtSts.Select(string.Format("No='{0}'", stNo)).Length + dtSts.Select(string.Format("NO='{0}'", stNo)).Length == 0)
                    continue;

                nst = new BP.WF.Template.NodeStation();
                nst.FK_Node = nid;
                nst.FK_Station = stNo;
                nst.Insert();
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
        public string Dot2DotStationModel_GetStructureTreeRoot()
        {
            JsonResultInnerData jr = new JsonResultInnerData();

            EasyuiTreeNode node, subnode;
            List<EasyuiTreeNode> d = new List<EasyuiTreeNode>();
            string parentrootid = this.GetRequestVal("parentrootid");
            string sql = null;
            DataTable dt = null;

            if (string.IsNullOrWhiteSpace(parentrootid))
                throw new Exception("参数parentrootid不能为空");

            CheckStationTypeIdxExists();
            bool isUnitModel = DBAccess.IsExitsTableCol("Port_Dept", "IsUnit");

            if (isUnitModel)
            {
                bool isValid = DBAccess.IsExitsTableCol("Port_Station", "FK_Unit");

                if (!isValid)
                    isUnitModel = false;
            }

            if (isUnitModel)
            {
                sql = string.Format("SELECT No,Name,ParentNo FROM Port_Dept WHERE IsUnit = 1 AND ParentNo = '{0}'", parentrootid);
                dt = DBAccess.RunSQLReturnTable(sql);

                if (dt.Rows.Count == 0)
                    dt.Rows.Add("-1", "无单位数据", parentrootid);

                node = new EasyuiTreeNode();
                node.id = "UNITROOT_" + dt.Rows[0]["No"];
                node.text = dt.Rows[0]["Name"] as string;
                node.iconCls = "icon-department";
                node.attributes = new EasyuiTreeNodeAttributes();
                node.attributes.No = dt.Rows[0]["No"] as string;
                node.attributes.Name = dt.Rows[0]["Name"] as string;
                node.attributes.ParentNo = parentrootid;
                node.attributes.TType = "UNITROOT";
                node.state = "closed";

                if (node.text != "无单位数据")
                {
                    node.children = new List<EasyuiTreeNode>();
                    node.children.Add(new EasyuiTreeNode());
                    node.children[0].text = "loading...";
                }

                d.Add(node);
            }
            else
            {
                sql = "SELECT No,Name FROM Port_StationType";
                dt = DBAccess.RunSQLReturnTable(sql);

                node = new EasyuiTreeNode();
                node.id = "STROOT_-1";
                node.text = "岗位类型";
                node.iconCls = "icon-department";
                node.attributes = new EasyuiTreeNodeAttributes();
                node.attributes.No = "-1";
                node.attributes.Name = "岗位类型";
                node.attributes.ParentNo = parentrootid;
                node.attributes.TType = "STROOT";
                node.state = "closed";

                if (dt.Rows.Count > 0)
                {
                    node.children = new List<EasyuiTreeNode>();
                    node.children.Add(new EasyuiTreeNode());
                    node.children[0].text = "loading...";
                }

                d.Add(node);
            }

            jr.InnerData = d;
            jr.Msg = isUnitModel.ToString().ToLower();

            return Newtonsoft.Json.JsonConvert.SerializeObject(jr);
        }

        /// <summary>
        /// 获取指定部门下一级子部门及人员列表
        /// </summary>
        /// <returns></returns>
        public string Dot2DotStationModel_GetSubUnits()
        {
            string parentid = this.GetRequestVal("parentid");
            string nid = this.GetRequestVal("nodeid");
            string tp = this.GetRequestVal("stype");    //ST,UNIT
            string ttype = this.GetRequestVal("ttype"); //STROOT,UNITROOT,ST,CST,S

            if (string.IsNullOrWhiteSpace(parentid))
                throw new Exception("参数parentid不能为空");
            if (string.IsNullOrWhiteSpace(nid))
                throw new Exception("参数nodeid不能为空");

            EasyuiTreeNode node = null;
            List<EasyuiTreeNode> d = new List<EasyuiTreeNode>();
            string sql = string.Empty;
            DataTable dt = null;
            BP.WF.Template.NodeStations sts = new BP.WF.Template.NodeStations();
            string sortField = CheckStationTypeIdxExists() ? "Idx" : "No";

            sts.Retrieve(BP.WF.Template.NodeStationAttr.FK_Node, int.Parse(nid));

            if (tp == "ST")
            {
                if (ttype == "STROOT")
                {
                    sql = "SELECT No,Name FROM Port_StationType ORDER BY " + sortField + " ASC";
                    dt = DBAccess.RunSQLReturnTable(sql);

                    foreach (DataRow row in dt.Rows)
                    {
                        node = new EasyuiTreeNode();
                        node.id = "ST_" + row["No"];
                        node.text = row["Name"] as string;
                        node.iconCls = "icon-department";
                        node.attributes = new EasyuiTreeNodeAttributes();
                        node.attributes.No = row["No"] as string;
                        node.attributes.Name = row["Name"] as string;
                        node.attributes.ParentNo = "-1";
                        node.attributes.TType = "ST";
                        node.state = "closed";
                        node.children = new List<EasyuiTreeNode>();
                        node.children.Add(new EasyuiTreeNode());
                        node.children[0].text = "loading...";

                        d.Add(node);
                    }
                }
                else
                {
                    sql =
                        string.Format(
                            "SELECT ps.No,ps.Name,ps.FK_StationType,pst.Name FK_StationTypeName FROM Port_Station ps"
                            + " INNER JOIN Port_StationType pst ON pst.No = ps.FK_StationType"
                            + " WHERE ps.FK_StationType = '{0}' ORDER BY ps.Name ASC", parentid);
                    dt = DBAccess.RunSQLReturnTable(sql);

                    foreach (DataRow row in dt.Rows)
                    {
                        node = new EasyuiTreeNode();
                        node.id = "S_" + parentid + "_" + row["No"];
                        node.text = row["Name"] as string;
                        node.iconCls = "icon-user";
                        node.@checked =
                            sts.GetEntityByKey(BP.WF.Template.NodeStationAttr.FK_Station, row["No"]) != null;
                        node.attributes = new EasyuiTreeNodeAttributes();
                        node.attributes.No = row["No"] as string;
                        node.attributes.Name = row["Name"] as string;
                        node.attributes.ParentNo = row["FK_StationType"] as string;
                        node.attributes.ParentName = row["FK_StationTypeName"] as string;
                        node.attributes.TType = "S";
                        node.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(row["Name"] as string);

                        d.Add(node);
                    }
                }
            }
            else
            {
                //岗位所属单位UNIT
                dt = DBAccess.RunSQLReturnTable(string.Format("SELECT * FROM Port_Dept WHERE IsUnit = 1 AND ParentNo='{0}' ORDER BY Name ASC", parentid));

                foreach (DataRow dept in dt.Rows)
                {
                    node = new EasyuiTreeNode();
                    node.id = "UNIT_" + dept["No"];
                    node.text = dept["Name"] as string;
                    node.iconCls = "icon-department";
                    node.attributes = new EasyuiTreeNodeAttributes();
                    node.attributes.No = dept["No"] as string;
                    node.attributes.Name = dept["Name"] as string;
                    node.attributes.ParentNo = dept["ParentNo"] as string;
                    node.attributes.TType = "UNIT";
                    node.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(dept["Name"] as string);
                    node.state = "closed";
                    node.children = new List<EasyuiTreeNode>();
                    node.children.Add(new EasyuiTreeNode());
                    node.children[0].text = "loading...";

                    d.Add(node);
                }

                dt = DBAccess.RunSQLReturnTable(
                    string.Format(
                        "SELECT ps.No,ps.Name,pst.No FK_StationType, pst.Name FK_StationTypeName,ps.FK_Unit,pd.Name FK_UnitName FROM Port_Station ps"
                        + " INNER JOIN Port_StationType pst ON pst.No = ps.FK_StationType"
                        + " INNER JOIN Port_Dept pd ON pd.No = ps.FK_Unit"
                        + " WHERE ps.FK_Unit = '{0}' ORDER BY pst.{1} ASC,ps.Name ASC", parentid, sortField));

                //增加岗位
                foreach (DataRow st in dt.Rows)
                {
                    node = new EasyuiTreeNode();
                    node.id = "S_" + st["FK_Unit"] + "_" + st["No"];
                    node.text = st["Name"] + "[" + st["FK_StationTypeName"] + "]";
                    node.iconCls = "icon-user";
                    node.@checked = sts.GetEntityByKey(BP.WF.Template.NodeStationAttr.FK_Station, st["No"]) != null;
                    node.attributes = new EasyuiTreeNodeAttributes();
                    node.attributes.No = st["No"] as string;
                    node.attributes.Name = st["Name"] as string;
                    node.attributes.ParentNo = st["FK_Unit"] as string;
                    node.attributes.ParentName = st["FK_UnitName"] as string;
                    node.attributes.TType = "S";
                    node.attributes.Code = BP.Tools.chs2py.ConvertStr2Code(st["Name"] as string);

                    d.Add(node);
                }
            }

            return Newtonsoft.Json.JsonConvert.SerializeObject(d);
        }

        /// <summary>
        /// 获取节点绑定人员信息列表
        /// </summary>
        /// <returns></returns>
        public string Dot2DotStationModel_GetNodeStations()
        {
            JsonResultInnerData jr = new JsonResultInnerData();

            DataTable dt = null;
            string nid = this.GetRequestVal("nodeid");
            int pagesize = int.Parse(this.GetRequestVal("pagesize"));
            int pageidx = int.Parse(this.GetRequestVal("pageidx"));
            string st = this.GetRequestVal("stype");
            string sql = string.Empty;
            string sortField = CheckStationTypeIdxExists() ? "Idx" : "No";

            if (st == "UNIT")
            {
                sql = "SELECT ps.No,ps.Name,pd.No UnitNo,pd.Name UnitName FROM WF_NodeStation wns "
                             + "  INNER JOIN Port_Station ps ON ps.No = wns.FK_Station "
                             + "  INNER JOIN Port_Dept pd ON pd.No = ps.FK_Unit "
                             + "WHERE wns.FK_Node = " + nid + " ORDER BY ps.Name ASC";
            }
            else
            {
                sql = "SELECT ps.No,ps.Name,pst.No UnitNo,pst.Name UnitName FROM WF_NodeStation wns "
                             + "  INNER JOIN Port_Station ps ON ps.No = wns.FK_Station "
                             + "  INNER JOIN Port_StationType pst ON pst.No = ps.FK_StationType "
                             + "WHERE wns.FK_Node = " + nid + " ORDER BY pst." + sortField + " ASC,ps.Name ASC";
            }

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
                        case "UNITNO":
                            col.ColumnName = "DeptNo";
                            break;
                        case "UNITNAME":
                            col.ColumnName = "DeptName";
                            break;
                    }
                }
            }

            jr.InnerData = dt;
            jr.Msg = "";
            string re = Newtonsoft.Json.JsonConvert.SerializeObject(jr);
            if (Glo.Plant == BP.WF.Plant.JFlow)
            {
                re = re.Replace("\"NO\"", "\"No\"").Replace("\"NAME\"", "\"Name\"").Replace("\"UNITNO\"", "\"UnitNo\"").Replace("\"UNITNAME\"", "\"UnitName\"");
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(re);
        }
        #endregion Dot2DotStationModel.htm（岗位选择）

        #region Methods
        /// <summary>
        /// 判断Port_StationType表中是否含有Idx字段
        /// </summary>
        /// <returns></returns>
        public bool CheckStationTypeIdxExists()
        {
            if (DBAccess.IsExitsTableCol("Port_StationType", "Idx") == false)
            {
                if (DBAccess.IsView("Port_StationType") == false)
                {
                    BP.GPM.StationType st = new GPM.StationType();
                    st.CheckPhysicsTable();

                    DBAccess.RunSQL("UPDATE Port_StationType SET Idx = 1");
                    return true;
                }
            }
            else
            {
                return true;
            }

            return false;
        }
        #endregion

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
            public string ParentName { get; set; }
            public string TType { get; set; }
            public string Code { get; set; }
        }
        #endregion 辅助实体定义
    }
}
