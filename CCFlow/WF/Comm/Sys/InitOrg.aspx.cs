using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.En;
using BP.Port;

namespace CCFlow.WF.Comm.Sys
{
    public partial class InitOrg : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            modelfile.NavigateUrl = "/DataUser/UploadFile/" + HttpUtility.UrlPathEncode("ccflow组织结构批量导入模板.xls");
        }

        protected void btnImport__DeClick(object sender, EventArgs e)
        {
            #region 获取文件.
            if (!file.HasFile)
            {
                AlertAndBack("请选择组织结构Excel文件(*.xls)！");
                return;
            }

            //检查是否是.xls格式，2007格式，下面的方法有问题，暂时限定为xls格式
            if (Path.GetExtension(file.PostedFile.FileName).ToLower() != ".xls")
            {
                AlertAndBack("组织结构Excel文件必须是*.xls（Microsoft Excel 97-2003格式）！");
                return;
            }

            var updir = Path.Combine(BP.Sys.SystemConfig.PathOfDataUser, "UploadFile");
            if (Directory.Exists(updir) == false)
                Directory.CreateDirectory(updir);

            string xls = Path.Combine(updir, "Org_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            file.PostedFile.SaveAs(xls);
            #endregion 获取文件.

            DataTable deptDT = BP.DA.DBLoad.GetTableByExt(xls, "SELECT * FROM [部门$]");
            DataTable stationDT = BP.DA.DBLoad.GetTableByExt(xls, "SELECT * FROM [岗位$]");
            DataTable empDT = BP.DA.DBLoad.GetTableByExt(xls, "SELECT * FROM [人员$]");
            return;
        }

        protected void btnImport_Click(object sender, EventArgs e)
        {
            if (!file.HasFile)
            {
                AlertAndBack("请选择组织结构Excel文件(*.xls)！");
                return;
            }

            //检查是否是.xls格式，2007格式，下面的方法有问题，暂时限定为xls格式
            if (Path.GetExtension(file.PostedFile.FileName).ToLower() != ".xls")
            {
                AlertAndBack("组织结构Excel文件必须是*.xls（Microsoft Excel 97-2003格式）！");
                return;
            }

            var updir = Path.Combine(BP.Sys.SystemConfig.PathOfDataUser, "UploadFile");
            if (Directory.Exists(updir) == false)
                Directory.CreateDirectory(updir);

            var xls = Path.Combine(updir, "Org_" + DateTime.Now.ToString("yyyyMMddHHmmss") + ".xls");
            file.PostedFile.SaveAs(xls);

            
            ////检查是否包含3个表
            //var sheets = excel.GetSheetNames();// BP.DA.DBLoad.GenerTableNames(xls);
            //if (sheets.Contains("岗位") == false || sheets.Contains("部门") == false || sheets.Contains("人员") == false)
            //{
            //    DeleteFile(xls);
            //    AlertAndBack("@组织结构Excel文件必须由“ccflow组织结构批量导入模板”文件编辑而成，请下载模板文件重新编辑，再进行导入！");
            //    return;
            //}

            var colStations = new[] { "岗位名称", "岗位类型" };
            var colDepts = new[] { "部门名称", "上级部门名称", "部门路径", "部门负责人" };
            var colEmps = new[] { "人员帐号", "人员姓名", "人员编号", "部门", "部门路径", "岗位", "职务", "电话", "邮箱", "直属领导" };

            //var dtStation = excel.ExcelToDataTable("岗位", true);  // BP.DA.DBLoad.GetTableByExt(xls, "SELECT * FROM [岗位$]"); //岗位名称*，岗位类型*
            //var dtDept = excel.ExcelToDataTable("部门", true);   // BP.DA.DBLoad.GetTableByExt(xls, "SELECT * FROM [部门$]");    //部门名称*，上级部门名称*，部门负责人
            //var dtEmp = excel.ExcelToDataTable("人员", true);   // BP.DA.DBLoad.GetTableByExt(xls, "SELECT * FROM [人员$]");     //人员帐号*，人员姓名*，人员编号，部门*，岗位*，职务，电话，邮箱，直属领导


            DataTable dtStation = BP.DA.DBLoad.GetTableByExt(xls, "SELECT * FROM [岗位$]");
            DataTable dtDept =    BP.DA.DBLoad.GetTableByExt(xls, "SELECT * FROM [部门$]");
            DataTable dtEmp =     BP.DA.DBLoad.GetTableByExt(xls, "SELECT * FROM [人员$]");

            //初步检查列数
            if (dtStation.Columns.Count < 2 || dtDept.Columns.Count < 3 || dtEmp.Columns.Count < 9)
            {
                AlertAndBack("@组织结构Excel文件结构错误，请重新下载模板文件编辑后，再进行导入！");
                return;
            }

        


            //检查岗位列
            if (CheckColumns(dtStation, colStations, "岗位") == false)
                return;

            //检查部门列
            if (CheckColumns(dtDept, colDepts, "部门") == false)
                return;

            //检查人员列
            if (CheckColumns(dtEmp, colEmps, "人员") == false)
                return;

            //return;
            //获取数据库中已有数据，便于下面的数据对比
            var isBPM = BP.WF.Glo.OSModel == BP.Sys.OSModel.OneMore;
            var stations = new BP.GPM.Stations();
            var stationTypes = new BP.GPM.StationTypes();
            var depts = new BP.GPM.Depts();
            var emps = new BP.GPM.Emps();
            var deptEmpSts = new BP.GPM.DeptEmpStations();
            stations.RetrieveAll();
            stationTypes.RetrieveAll();
            depts.RetrieveAll();
            emps.RetrieveAll();
            deptEmpSts.RetrieveAll();

            //集成模式，涉及数据
            var empDepts = new EmpDepts();
            var empSts = new EmpStations();
            empDepts.RetrieveAll();
            empSts.RetrieveAll();

            //BPM独立模式，涉及数据
            var duties = new BP.GPM.Dutys();
            var deptDuties = new BP.GPM.DeptDutys();
            var deptSts = new BP.GPM.DeptStations();
            var deptEmps = new BP.GPM.DeptEmps();
            duties.RetrieveAll();
            deptDuties.RetrieveAll();
            deptSts.RetrieveAll();
            deptEmps.RetrieveAll();

            //定义错误记录存储表
            var dtStationError = dtStation.Clone();
            var dtDeptError = dtDept.Clone();
            var dtEmpError = dtEmp.Clone();
            DataRow drError = null;
            BP.GPM.Station st = null;
            BP.GPM.StationType stType = null;
            BP.GPM.Dept dept = null;
            BP.GPM.Emp emp = null;
            BP.GPM.DeptEmpStation des = null;
            BP.Port.EmpDept empDept = null;
            BP.Port.EmpStation empSt = null;
            BP.GPM.Duty duty = null;
            BP.GPM.DeptDuty deptDuty = null;
            BP.GPM.DeptStation deptSt = null;
            BP.GPM.DeptEmp deptEmp = null;

            //检查数据，并将符合规范的数据写入数据库

            #region //岗位信息
            foreach (DataRow dr in dtStation.Rows)
            {
                drError = dtStationError.NewRow();
                dtStationError.Rows.Add(drError);

                //检查岗位，名称、类型必须填写，且名称不能重复
                if (IsCellNull(dr, "岗位名称"))
                {
                    drError["岗位名称"] = "岗位名称必须填写！";
                    continue;
                }

                if (IsCellNull(dr, "岗位类型"))
                {
                    drError["岗位类型"] = "岗位类型必须填写！";
                    continue;
                }

                //判断数据库中是否已经存在
                if (stations.GetEntityByKey(BP.GPM.StationAttr.Name, dr["岗位名称"].ToString()) != null)
                    continue;

                stType = stationTypes.GetEntityByKey(BP.GPM.StationTypeAttr.Name, dr["岗位类型"].ToString()) as BP.GPM.StationType;

                if (stType == null)
                {
                    stType = new BP.GPM.StationType { Name = dr["岗位类型"].ToString() };
                    stType.No = stType.GenerNewNo;
                    stType.Insert();
                    stationTypes.AddEntity(stType);
                }

                st = new BP.GPM.Station
                         {
                             Name = dr["岗位名称"].ToString(),
                             FK_StationType = stType.No
                         };
                //st.No = st.GenerNewNo;
                st.Insert();
                stations.AddEntity(st);
            }
            #endregion

            #region  //部门信息
            BP.GPM.Dept parentDept = null;

            foreach (DataRow dr in dtDept.Rows)
            {
                drError = dtDeptError.NewRow();
                dtDeptError.Rows.Add(drError);

                //检查部门，名称必须填写，且名称不能重复
                if (IsCellNull(dr, "部门名称"))
                {
                    drError["部门名称"] = "部门名称必须填写！";
                    continue;
                }

                if (!IsCellNull(dr, "上级部门名称"))
                {
                    parentDept = depts.GetEntityByKey(EntityTreeAttr.Name, dr["上级部门名称"].ToString()) as BP.GPM.Dept;

                    if (parentDept == null)
                    {
                        drError["上级部门名称"] = "上级部门名称 填写错误，未找到上级部门“" + dr["上级部门名称"] + "”！";
                        continue;
                    }
                }
                else
                {
                    parentDept = null;
                }

                //判断数据库中是否已经存在
                if (depts.GetEntityByKey(EntityTreeAttr.Name, dr["部门名称"].ToString(), EntityTreeAttr.ParentNo, parentDept == null ? "0" : parentDept.No) != null)
                    continue;

                if (parentDept == null)
                {
                    //增加根部门
                    dept = depts.GetEntityByKey(EntityTreeAttr.ParentNo, "0") as BP.GPM.Dept;
                    if (dept == null)
                    {
                        //不存在根部门
                        dept = new BP.GPM.Dept
                                   {
                                       No = "100",
                                       Name = dr["部门名称"].ToString(),
                                       ParentNo = "0",
                                       Leader = dr["部门负责人"].ToString(),
                                       IsDir = false
                                   };

                        dept.Insert();
                    }
                    else
                    {
                        //已经存在根部门，建立一个同级别的根部门
                        dept.Name = dr["部门名称"].ToString();
                        dept.Leader = dr["部门负责人"].ToString();

                        //var currDept = dept.DoCreateSameLevelNode();

                        //dept = new BP.GPM.Dept(currDept.No)
                        //           {
                        //               Name = dr["部门名称"].ToString(),
                        //               Leader = dr["部门负责人"].ToString()
                        //           };

                        dept.Update();
                    }

                    depts.AddEntity(dept);
                }
                else
                {
                    //增加非根部门
                    var currDept = parentDept.DoCreateSubNode();

                    dept = new BP.GPM.Dept(currDept.No)
                               {
                                   Name = dr["部门名称"].ToString(),
                                   Leader = dr["部门负责人"].ToString()
                               };

                    dept.Update();
                    depts.AddEntity(dept);
                }
            }
            #endregion

            #region //人员信息
            foreach (DataRow dr in dtEmp.Rows)
            {
                drError = dtEmpError.NewRow();
                dtEmpError.Rows.Add(drError);

                #region //检查人员，帐号、姓名、部门、岗位必须填写.
                if (IsCellNull(dr, "人员帐号"))
                {
                    drError["人员姓名"] = "人员帐号必须填写！";
                    continue;
                }

                if (IsCellNull(dr, "人员姓名"))
                {
                    drError["人员姓名"] = drError["人员姓名"] + "人员姓名必须填写！";
                    continue;
                }

                if (IsCellNull(dr, "部门"))
                {
                    drError["部门"] = "部门必须填写！";
                    continue;
                }

                if (IsCellNull(dr, "岗位"))
                {
                    drError["岗位"] = "岗位必须填写！";
                    continue;
                }
                #endregion

                //处理变量.

                string empNo = dr["人员帐号"].ToString();
                string empName = dr["人员姓名"].ToString();

                string deptName = dr["部门"].ToString();
                string stationName = dr["岗位"].ToString();


                //判断人员在数据库中是否已经存在?
                emp = emps.GetEntityByKey(EntityNoAttr.No, dr["人员帐号"].ToString(), EntityNoNameAttr.Name,
                                          dr["人员姓名"].ToString()) as BP.GPM.Emp;

                dept = depts.GetEntityByKey(BP.GPM.DeptAttr.Name, dr["部门"].ToString()) as BP.GPM.Dept;
                st = stations.GetEntityByKey(BP.GPM.StationAttr.Name, dr["岗位"].ToString()) as BP.GPM.Station;

                if (dept == null)
                {
                    drError["部门"] = string.Format("部门填写错误，“{0}” 在部门中未查到！", dr["部门"]);
                    continue;
                }

                if (st == null)
                {
                    drError["岗位"] = string.Format("岗位填写错误，“{0}” 在岗位中未查到！", dr["岗位"]);
                    continue;
                }

                if (isBPM)
                {
                    //BPM模式，Duty，DeptDuty
                    if (!IsCellNull(dr, "职务"))
                    {
                        duty = duties.GetEntityByKey(BP.GPM.DutyAttr.Name, dr["职务"].ToString()) as BP.GPM.Duty;
                    }
                    else
                    {
                        if(duties.Count == 0)
                        {
                            duty = new BP.GPM.Duty {Name = "默认职务"};
                            duty.Insert();
                            duties.AddEntity(duty);
                        }
                        else
                        {
                            duty = duties[0] as BP.GPM.Duty;
                        }
                    }

                    if (duty == null)
                    {
                        duty = new BP.GPM.Duty { Name = dr["职务"].ToString() };
                        //duty.No = duty.GenerNewNo;
                        duty.Insert();
                        duties.AddEntity(duty);
                    }

                    deptDuty =
                        deptDuties.GetEntityByKey(BP.GPM.DeptDutyAttr.FK_Dept, dept.No, BP.GPM.DeptDutyAttr.FK_Duty,
                                                  duty.No) as BP.GPM.DeptDuty;

                    if (deptDuty == null)
                    {
                        deptDuty = new BP.GPM.DeptDuty {FK_Dept = dept.No, FK_Duty = duty.No};
                        deptDuty.Insert();
                        deptDuties.AddEntity(deptDuty);
                    }
                }

                if (emp != null)
                {
                    //如果人员存在，则判断此人员的部门与岗位，如果部门、岗位不存在，则增加部门岗位关联等
                    //此处不对已存在人员的其他信息进行比对修改，如人员编号，邮箱，电话等
                    des = deptEmpSts.GetEntityByKey(BP.GPM.DeptEmpStationAttr.FK_Emp, emp.No,
                                                    BP.GPM.DeptEmpStationAttr.FK_Dept, dept.No,
                                                    BP.GPM.DeptEmpStationAttr.FK_Station, st.No) as BP.GPM.DeptEmpStation;

                    if (des != null)
                    {
                        drError["人员姓名"] = drError["人员姓名"] =
                                          drError["部门"] = drError["岗位"] = "此人员已经增加过，“人员+部门+岗位”不能重复！";
                        continue;
                    }
                }
                else
                {
                    //此人员不存在
                    emp = new BP.GPM.Emp
                              {
                                  No = dr["人员帐号"].ToString(),
                                  Name = dr["人员姓名"].ToString(),
                                  Pass = "pub",
                                  FK_Dept = dept.No
                              };

                    if (isBPM)
                    {
                        emp.EmpNo = dr["人员编号"] as string;
                        emp.FK_Duty = duty.No;
                        emp.Email = dr["邮箱"] as string;
                        emp.Tel = dr["电话"] as string;
                        emp.Leader = dr["直属领导"] as string;
                        emp.SetValByKey(BP.GPM.EmpAttr.NumOfDept, 1);
                    }

                    emp.Insert();
                    emps.AddEntity(emp);
                }

                des = new BP.GPM.DeptEmpStation
                          {
                              FK_Emp = emp.No,
                              FK_Dept = dept.No,
                              FK_Station = st.No
                          };

                des.Insert();
                deptEmpSts.AddEntity(des);

                if (isBPM)
                {
                    //BPM模式，DeptStation，DeptEmp
                    deptSt = deptSts.GetEntityByKey(BP.GPM.DeptStationAttr.FK_Dept, dept.No,
                                                    BP.GPM.DeptStationAttr.FK_Station, st.No) as BP.GPM.DeptStation;

                    if (deptSt == null)
                    {
                        deptSt = new BP.GPM.DeptStation {FK_Dept = dept.No, FK_Station = st.No};
                        deptSt.Insert();
                        deptSts.AddEntity(deptSt);
                    }

                    deptEmp =
                        deptEmps.GetEntityByKey(BP.GPM.DeptEmpAttr.FK_Dept, dept.No, BP.GPM.DeptEmpAttr.FK_Emp,
                                                emp.No) as BP.GPM.DeptEmp;

                    if (deptEmp == null)
                    {
                        deptEmp = new BP.GPM.DeptEmp
                                      {
                                          FK_Dept = dept.No, 
                                          FK_Emp = emp.No, 
                                          FK_Duty = duty.No
                                      };
                        deptEmp.Insert();
                        deptEmps.AddEntity(deptEmp);
                    }
                }
                else
                {
                    //集成模式，EmpDept，EmpStation
                    empDept = empDepts.GetEntityByKey("FK_Dept", dept.No, EmpDeptAttr.FK_Emp, emp.No) as EmpDept;

                    if (empDept == null)
                    {
                        empDept = new EmpDept {FK_Emp = emp.No, FK_Dept = dept.No};
                        empDept.Insert();
                        empDepts.AddEntity(empDept);
                    }

                    empSt =
                        empSts.GetEntityByKey(EmpStationAttr.FK_Emp, emp.No, EmpStationAttr.FK_Station, st.No) as
                        EmpStation;

                    if (empSt == null)
                    {
                        empSt = new EmpStation {FK_Emp = emp.No, FK_Station = st.No};
                        empSt.Insert();
                        empSts.AddEntity(empSt);
                    }
                }
            }
            #endregion

            #region //前台输出岗位信息

            pub1.AddTableNormal();
            pub1.AddTR();
            pub1.AddTDGroupTitle("style='text-align:center;width:60px;'", "序");
            pub1.AddTDGroupTitle("style='width:140px;'", "名称");
            pub1.AddTDGroupTitle("类型");
            pub1.AddTREnd();

            for (var i = 0; i < dtStation.Rows.Count; i++)
            {
                drError = dtStationError.Rows[i];

                pub1.AddTR();
                pub1.AddTDIdx(i + 1);

                if (IsCellNull(drError, "岗位名称"))
                    pub1.AddTD(dtStation.Rows[i]["岗位名称"].ToString());
                else
                    pub1.AddTD("style='background-color:yellow;color:red;' title='" + drError["岗位名称"] + "'",
                               dtStation.Rows[i]["岗位名称"].ToString());

                if (IsCellNull(drError, "岗位类型"))
                    pub1.AddTD(dtStation.Rows[i]["岗位类型"].ToString());
                else
                    pub1.AddTD("style='background-color:yellow;color:red;' title='" + drError["岗位类型"] + "'",
                               dtStation.Rows[i]["岗位类型"].ToString());

                pub1.AddTREnd();
            }

            pub1.AddTableEnd();
            #endregion

            #region //前台输出部门信息

            pub2.AddTableNormal();
            pub2.AddTR();
            pub2.AddTDGroupTitle("style='text-align:center;width:60px;'", "序");
            pub2.AddTDGroupTitle("style='width:140px;'", "名称");
            pub2.AddTDGroupTitle("style='width:140px;'", "上级名称");
            pub2.AddTDGroupTitle("部门负责人");
            pub2.AddTREnd();

            for (var i = 0; i < dtDept.Rows.Count; i++)
            {
                drError = dtDeptError.Rows[i];

                pub2.AddTR();
                pub2.AddTDIdx(i + 1);

                if (IsCellNull(drError, "部门名称"))
                    pub2.AddTD(dtDept.Rows[i]["部门名称"].ToString());
                else
                    pub2.AddTD("style='background-color:yellow;color:red;' title='" + drError["部门名称"] + "'",
                               dtDept.Rows[i]["部门名称"].ToString());

                if (IsCellNull(drError, "上级部门名称"))
                    pub2.AddTD(dtDept.Rows[i]["上级部门名称"].ToString());
                else
                    pub2.AddTD("style='background-color:yellow;color:red;' title='" + drError["上级部门名称"] + "'",
                               dtDept.Rows[i]["上级部门名称"].ToString());

                pub2.AddTD(dtDept.Rows[i]["部门负责人"].ToString());
                pub2.AddTREnd();
            }

            pub2.AddTableEnd();
            #endregion

            #region //前台输出人员信息

            pub3.AddTableNormal();
            pub3.AddTR();
            pub3.AddTDGroupTitle("style='text-align:center;width:60px;'", "序");
            pub3.AddTDGroupTitle("style='width:120px;'", "帐号");
            pub3.AddTDGroupTitle("style='width:100px;'", "姓名");

            if (isBPM)
                pub3.AddTDGroupTitle("style='width:100px;'", "编号");

            pub3.AddTDGroupTitle("style='width:180px;'", "部门");

            if (!isBPM)
            {
                pub3.AddTDGroupTitle("岗位");
            }
            else
            {
                pub3.AddTDGroupTitle("style='width:100px;'", "岗位");
                pub3.AddTDGroupTitle("style='width:100px;'", "职务");
                pub3.AddTDGroupTitle("style='width:120px;'", "电话");
                pub3.AddTDGroupTitle("style='width:160px;'", "邮箱");
                pub3.AddTDGroupTitle("直属领导");
            }

            pub3.AddTREnd();

            for (var i = 0; i < dtEmp.Rows.Count; i++)
            {
                drError = dtEmpError.Rows[i];

                pub3.AddTR();
                pub3.AddTDIdx(i + 1);

                if (IsCellNull(drError, "人员帐号"))
                    pub3.AddTD(dtEmp.Rows[i]["人员帐号"].ToString());
                else
                    pub3.AddTD("style='background-color:yellow;color:red;' title='" + drError["人员帐号"] + "'",
                               dtEmp.Rows[i]["人员帐号"].ToString());

                if (IsCellNull(drError, "人员姓名"))
                    pub3.AddTD(dtEmp.Rows[i]["人员姓名"].ToString());
                else
                    pub3.AddTD("style='background-color:yellow;color:red;' title='" + drError["人员姓名"] + "'",
                               dtEmp.Rows[i]["人员姓名"].ToString());

                if (isBPM)
                    pub3.AddTD(dtEmp.Rows[i]["人员编号"].ToString());

                if (IsCellNull(drError, "部门"))
                    pub3.AddTD(dtEmp.Rows[i]["部门"].ToString());
                else
                    pub3.AddTD("style='background-color:yellow;color:red;' title='" + drError["部门"] + "'",
                               dtEmp.Rows[i]["部门"].ToString());

                if (IsCellNull(drError, "岗位"))
                    pub3.AddTD(dtEmp.Rows[i]["岗位"].ToString());
                else
                    pub3.AddTD("style='background-color:yellow;color:red;' title='" + drError["岗位"] + "'",
                               dtEmp.Rows[i]["岗位"].ToString());

                if (isBPM)
                {
                    pub3.AddTD(dtEmp.Rows[i]["职务"].ToString());
                    pub3.AddTD(dtEmp.Rows[i]["电话"].ToString());
                    pub3.AddTD(dtEmp.Rows[i]["邮箱"].ToString());
                    pub3.AddTD(dtEmp.Rows[i]["直属领导"].ToString());
                }

                pub3.AddTREnd();
            }

            pub3.AddTableEnd();
            #endregion
        }

        private bool IsCellNull(DataRow dr, string colName)
        {
            return dr[colName] == DBNull.Value || dr[colName] == null ||
                   string.IsNullOrWhiteSpace(dr[colName].ToString());
        }

        /// <summary>
        /// 检查各列名是否正确
        /// </summary>
        /// <param name="dt">提取的数据DataTable</param>
        /// <param name="rightColumns">正确的列名数组</param>
        /// <param name="dtName">当前数据表名称</param>
        /// <returns></returns>
        private bool CheckColumns(DataTable dt, string[] rightColumns, string dtName)
        {
            var idx = 0;

            foreach (DataColumn col in dt.Columns)
            {
                col.ColumnName = col.ColumnName.Replace(" ", "");
                col.ColumnName = col.ColumnName.Replace(" ", "");
                col.ColumnName = col.ColumnName.Replace(" ", "");
                col.ColumnName = col.ColumnName.Replace("*", "");

                ////强制修改为string类型.
                //col.DataType = typeof(string);

                if (Equals(col.ColumnName, rightColumns[idx]) == false)
                {
                    AlertAndBack("@组织结构Excel文件中“" + dtName + "”表结构错误[" + col.ColumnName + "]与[" + rightColumns[idx] + "]不一致，请重新下载模板文件编辑后，再进行导入！");
                    return false;
                }

                idx++;

                if (idx == rightColumns.Length)
                    return true;
            }

            return true;
        }

        private void DeleteFile(string file)
        {
            try
            {
                File.Delete(file);
            }
            catch { }
        }

        private void AlertAndBack(string msg)
        {
            Response.Write("<script>alert('" + msg + "');history.back();</script>");
            Response.End();
        }

        #region NPOI 操作Excel相关方法，暂时放于此处，以后再考虑增加一些NPOI操作Excel的公用方法

        #endregion
    }

      
     
}