using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using System.Text.RegularExpressions;
namespace BP.GPM
{
    /// <summary>
    /// 信息块
    /// </summary>
    public class BarAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 信息块类型
        /// </summary>
        public const string BarType = "BarType";
        /// <summary>
        /// 顺序
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// tag1
        /// </summary>
        public const string Tag1 = "Tag1";
        /// <summary>
        /// Tag2
        /// </summary>
        public const string Tag2 = "Tag2";
        /// <summary>
        /// Tag3
        /// </summary>
        public const string Tag3 = "Tag3";
        /// <summary>
        /// 是否可删除
        /// </summary>
        public const string IsDel = "IsDel";
        /// <summary>
        /// 控制方式
        /// </summary>
        public const string CtrlWay = "CtrlWay";
        /// <summary>
        /// 打开方式
        /// </summary>
        public const string OpenWay = "OpenWay";
        /// <summary>
        /// 更多标签
        /// </summary>
        public const string MoreLab = "MoreLab";
        /// <summary>
        /// MoreUrl
        /// </summary>
        public const string MoreUrl = "MoreUrl";
        /// <summary>
        /// Doc
        /// </summary>
        public const string Doc = "Doc";
        /// <summary>
        /// 产生时间
        /// </summary>
        public const string DocGenerRDT = "DocGenerRDT";
        /// <summary>
        /// 显示宽度
        /// </summary>
        public const string Width = "Width";
        /// <summary>
        /// 显示高度
        /// </summary>
        public const string Height = "Height";
    }
    /// <summary>
    /// 信息块
    /// </summary>
    public class Bar : EntityNoName
    {
        /// <summary>
        /// 控制权限
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.IsInsert = false;
               
                return base.HisUAC;
            }
        }
        #region 属性
        /// <summary>
        /// 更多的URL
        /// </summary>
        public string MoreUrl
        {
            get
            {
                return this.GetValStrByKey(BarAttr.MoreUrl);
            }
            set
            {
                this.SetValByKey(BarAttr.MoreUrl, value);
            }
        }
        /// <summary>
        /// 更多标签
        /// </summary>
        public string MoreLab
        {
            get
            {
                return this.GetValStrByKey(BarAttr.MoreLab);
            }
            set
            {
                this.SetValByKey(BarAttr.MoreLab, value);
            }
        }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title
        {
            get
            {
                return this.GetValStrByKey(BarAttr.Title);
            }
            set
            {
                this.SetValByKey(BarAttr.Title, value);
            }
        }
        /// <summary>
        /// 用户是否可以删除
        /// </summary>
        public bool IsDel
        {
            get
            {
                return this.GetValBooleanByKey(BarAttr.IsDel);
            }
            set
            {
                this.SetValByKey(BarAttr.IsDel, value);
            }
        }
        /// <summary>
        /// 类型
        /// </summary>
        public int BarType
        {
            get
            {
                return this.GetValIntByKey(BarAttr.BarType);
            }
            set
            {
                this.SetValByKey(BarAttr.BarType, value);
            }
        }
        /// <summary>
        /// 打开方式
        /// </summary>
        public int OpenWay
        {
            get
            {
                return this.GetValIntByKey(BarAttr.OpenWay);
            }
            set
            {
                this.SetValByKey(BarAttr.OpenWay, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(BarAttr.Idx);
            }
            set
            {
                this.SetValByKey(BarAttr.Idx, value);
            }
        }
        /// <summary>
        /// Tag1
        /// </summary>
        public string Tag1
        {
            get
            {
                return this.GetValStrByKey(BarAttr.Tag1);
            }
            set
            {
                this.SetValByKey(BarAttr.Tag1, value);
            }
        }
        /// <summary>
        /// Tag
        /// </summary>
        public string Tag2
        {
            get
            {
                return this.GetValStrByKey(BarAttr.Tag2);
            }
            set
            {
                this.SetValByKey(BarAttr.Tag2, value);
            }
        }
        public string Tag3
        {
            get
            {
                return this.GetValStrByKey(BarAttr.Tag3);
            }
            set
            {
                this.SetValByKey(BarAttr.Tag3, value);
            }
        }
        public string DocGenerRDT
        {
            get
            {
                return this.GetValStrByKey(BarAttr.DocGenerRDT);
            }
            set
            {
                this.SetValByKey(BarAttr.DocGenerRDT, value);
            }
        }
        public string Width
        {
            get
            {
                return this.GetValStrByKey(BarAttr.Width);
            }
            set
            {
                this.SetValByKey(BarAttr.Width, value);
            }
        }
        public string Height
        {
            get
            {
                return this.GetValStrByKey(BarAttr.Height);
            }
            set
            {
                this.SetValByKey(BarAttr.Height, value);
            }
        }
        public string Doc
        {
            get
            {
                string html = "";
                switch (this.BarType)
                {
                    case 0:
                        html += "\t\n<ul style='padding-left:13px;width:200px;overflow:hidden;'>";

                        string sql = this.Tag1;
                        sql = sql.Replace("~", "'");
                        sql = sql.Replace("@WebUser.No", BP.Web.WebUser.No);
                        sql = sql.Replace("@WebUser.Name", BP.Web.WebUser.Name);
                        DataTable dt = DBAccess.RunSQLReturnTable(sql);
                        foreach (DataRow dr in dt.Rows)
                        {
                            string no = dr["No"].ToString();
                            string name = dr["Name"].ToString();
                            string url = this.Tag2.Clone().ToString();
                            //url = url.Replace("@No", no);
                            url = url.Replace("~", "'");
                            //if (url.Contains("@"))
                            //{
                            //   foreach (DataColumn dc in dt.Columns)
                            //        url = url.Replace("@" + dc.ColumnName, dr[dc.ColumnName].ToString());
                            //}
                            url = this.GetParameteredString(url,dr);

                            if (url.ToLower().StartsWith("javascript:"))
                            {
                                html += "\t\n<li><a href=\"" + url + "\">" + name + "</a></li>";
                            }
                            else
                            {
                                switch (this.OpenWay)
                                {
                                    case 0: //新窗口
                                        html += "\t\n<li><a href=\"" + url + "\"  target='_blank' >" + name + "</a></li>";
                                        break;
                                    case 1: // 本窗口
                                        html += "\t\n<li><a href=\"" + url + "\" target='_self' >" + name + "</a></li>";
                                        break;
                                    case 2: //覆盖新窗口 
                                        html += "\t\n<li><a href=\"" + url + "\" target='" + this.No + "' >" + name + "</a></li>";
                                        break;
                                    default:
                                        break;
                                }
                            }
                        }
                        html += "\t\n</ul>";
                        return html;
                    case 1:
                        return this.Tag1;
                    default:
                        break;
                }
                return this.GetValStrByKey(BarAttr.Doc);
            }
        }
        /// <summary>
        /// 获取参数化的字符串
        /// </summary>
        /// <param name="stringInput"></param>
        /// <param name="dr"></param>
        /// <returns></returns>
        private string GetParameteredString(string stringInput, DataRow dr)
        {
            String regE = "@[a-zA-Z]([\\w-]*[a-zA-Z0-9])?"; //字母开始，字母+数字结尾，字母+数字+下划线+中划线中间
            //String regE = "@[\\w-]+";                               //字母+数字+下划线+中划线
            MatchCollection mc = Regex.Matches(stringInput, regE, RegexOptions.IgnoreCase);
            foreach (Match m in mc)
            {
                string v = m.Value;
                string f = m.Value.Substring(1);
                stringInput = stringInput.Replace(v, String.Format("{0}", dr[f]));
            }
            return stringInput;
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 信息块
        /// </summary>
        public Bar()
        {
        }
        /// <summary>
        /// 信息块
        /// </summary>
        /// <param name="mypk"></param>
        public Bar(string no)
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
                Map map = new Map("GPM_Bar");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "信息块";
                map.EnType = EnType.Sys;

                map.AddTBStringPK(BarAttr.No, null, "编号", true, false, 2, 2, 2);
                map.AddTBString(BarAttr.Name, null, "名称", true, false, 0, 3900, 20);
                map.AddTBString(BarAttr.Title, null, "标题", true, false, 0, 3900, 20);
                map.AddDDLSysEnum(BarAttr.BarType, 0, "信息块类型", true, true, 
                    BarAttr.BarType, "@0=标题消息列表(Tag1=SQL语句)@1=其它");

                map.AddTBString(BarAttr.Tag1, null, "Tag1", true, false, 0, 3900, 300,true);
                map.AddTBString(BarAttr.Tag2, null, "Tag2", true, false, 0, 3900, 300, true);
                map.AddTBString(BarAttr.Tag3, null, "Tag3", true, false, 0, 3900, 300, true);

                map.AddBoolean(BarAttr.IsDel, true, "用户是否可删除",true,true);

                map.AddDDLSysEnum(BarAttr.OpenWay, 0, "打开方式", true, true,
                  BarAttr.OpenWay, "@0=新窗口@1=本窗口@2=覆盖新窗口");

                //map.AddDDLSysEnum(AppAttr.CtrlWay, 0, "控制方式", true, true,
                //   AppAttr.CtrlWay, "@0=游客@1=所有人员@2=按岗位@3=按部门@4=按人员@5=按SQL");

                map.AddTBInt(BarAttr.Idx, 0, "显示顺序", false, true);

                map.AddTBString(BarAttr.MoreLab, "更多...", "更多标签", true, false, 0, 900, 20);
                map.AddTBString(BarAttr.MoreUrl, null, "更多标签Url", true, false, 0, 3900, 20,true);

                map.AddTBString(BarAttr.Doc, null, "Doc", false, false, 0, 3900, 20, false);
                map.AddTBDateTime(BarAttr.DocGenerRDT, null, "Doc生成日期", false, false);

                map.AddTBInt(BarAttr.Width, 200, "显示宽度", false, true);
                map.AddTBInt(BarAttr.Height, 100, "显示高度", false, true);

                map.AttrsOfOneVSM.Add(new ByStations(), new Stations(), ByStationAttr.RefObj, ByStationAttr.FK_Station, StationAttr.Name, StationAttr.No, "可访问的岗位");
                map.AttrsOfOneVSM.Add(new ByDepts(), new Depts(), ByStationAttr.RefObj, ByDeptAttr.FK_Dept, DeptAttr.Name, DeptAttr.No, "可访问的部门");
                map.AttrsOfOneVSM.Add(new ByEmps(), new Emps(), ByStationAttr.RefObj, ByEmpAttr.FK_Emp, EmpAttr.Name, EmpAttr.No, "可访问的人员");

                map.AddSearchAttr(BarAttr.OpenWay);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 信息块s
    /// </summary>
    public class Bars : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 信息块s
        /// </summary>
        public Bars()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Bar();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Bar> ToJavaList()
        {
            return (System.Collections.Generic.IList<Bar>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Bar> Tolist()
        {
            System.Collections.Generic.List<Bar> list = new System.Collections.Generic.List<Bar>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Bar)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
