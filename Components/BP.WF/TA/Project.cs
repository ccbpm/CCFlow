using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.TA
{
    /// <summary>
    /// 項目属性
    /// </summary>
    public class ProjectAttr : EntityNoNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 标题
        /// </summary>
        public const string StarterNo = "StarterNo";
        /// <summary>
        /// 項目内容
        /// </summary>
        public const string StarterName = "StarterName";
        /// <summary>
        /// 按表单字段項目
        /// </summary>
        public const string PrjSta = "PrjSta";
        /// <summary>
        /// 内容描述
        /// </summary>
        public const string PrjDesc = "PrjDesc";
        /// <summary>
        /// 表单字段
        /// </summary>
        public const string PRI = "PRI";
        /// <summary>
        /// 是否启用項目到角色
        /// </summary>
        public const string WCL = "WCL";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        #endregion

        public const string TemplateNo = "TemplateNo";
        public const string TemplateName = "TemplateName";

        public const string Msg = "Msg";

        public const string NumTasks = "NumTasks";
        public const string NumComplete = "NumComplete";


    }
    /// <summary>
    /// 項目
    /// </summary>
    public class Project : EntityNoName
    {
        #region 属性.
        public string Msg
        {
            get
            {
                return this.GetValStringByKey(ProjectAttr.Msg);
            }
            set
            {
                this.SetValByKey(ProjectAttr.Msg, value);
            }
        }
        public string PrjDesc
        {
            get
            {
                return this.GetValStringByKey(ProjectAttr.PrjDesc);
            }
            set
            {
                this.SetValByKey(ProjectAttr.PrjDesc, value);
            }
        }
        public int PRI
        {
            get
            {
                return this.GetValIntByKey(ProjectAttr.PRI);
            }
            set
            {
                this.SetValByKey(ProjectAttr.PRI, value);
            }
        }
        public int PrjSta
        {
            get
            {
                return this.GetValIntByKey(ProjectAttr.PrjSta);
            }
            set
            {
                this.SetValByKey(ProjectAttr.PrjSta, value);
            }
        }
        public int WCL
        {
            get
            {
                return this.GetValIntByKey(ProjectAttr.WCL);
            }
            set
            {
                this.SetValByKey(ProjectAttr.WCL, value);
            }
        }
        public string StarterNo
        {
            get
            {
                return this.GetValStringByKey(ProjectAttr.StarterNo);
            }
            set
            {
                this.SetValByKey(ProjectAttr.StarterNo, value);
            }
        }
        public string StarterName
        {
            get
            {
                return this.GetValStringByKey(ProjectAttr.StarterName);
            }
            set
            {
                this.SetValByKey(ProjectAttr.StarterName, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(ProjectAttr.RDT);
            }
            set
            {
                this.SetValByKey(ProjectAttr.RDT, value);
            }
        }
        public string TemplateNo
        {
            get
            {
                return this.GetValStringByKey(ProjectAttr.TemplateNo);
            }
            set
            {
                this.SetValByKey(ProjectAttr.TemplateNo, value);
            }
        }
        public string TemplateName
        {
            get
            {
                return this.GetValStringByKey(ProjectAttr.TemplateName);
            }
            set
            {
                this.SetValByKey(ProjectAttr.TemplateName, value);
            }
        }
        #endregion 属性.

        #region 统计
        public int NumTasks
        {
            get
            {
                return this.GetValIntByKey(ProjectAttr.NumTasks);
            }
            set
            {
                this.SetValByKey(ProjectAttr.NumTasks, value);
            }
        }
        public int NumComplete
        {
            get
            {
                return this.GetValIntByKey(ProjectAttr.NumComplete);
            }
            set
            {
                this.SetValByKey(ProjectAttr.NumComplete, value);
            }
        }
        #endregion 统计

        #region 构造函数
        /// <summary>
        /// 項目设置
        /// </summary>
        public Project()
        {
        }
        /// <summary>
        /// 項目设置
        /// </summary>
        /// <param name="no"></param>
        public Project(string no)
        {
            this.No = no;
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

                Map map = new Map("TA_Project", "項目");

                map.AddTBStringPK(ProjectAttr.No, null, "编号", true, true, 5, 5, 5);
                map.AddTBString(ProjectAttr.Name, null, "名称", true, true, 0, 100, 10);
                map.AddTBString(ProjectAttr.PrjDesc, null, "内容描述", true, true, 0, 100, 10);


                map.AddTBInt(ProjectAttr.PrjSta, 0, "状态", true, true);
                map.AddTBInt(ProjectAttr.PRI, 0, "优先级", true, true);
                map.AddTBInt(ProjectAttr.WCL, 0, "完成率", true, true);

                map.AddTBString(ProjectAttr.Msg, null, "消息", true, true, 0, 500, 10);


                map.AddTBString(ProjectAttr.StarterNo, null, "发起人", true, false, 0, 100, 10, true);
                map.AddTBString(ProjectAttr.StarterName, null, "名称", true, false, 0, 100, 10, true);
                map.AddTBDateTime(ProjectAttr.RDT, null, "发起日期", true, false);

                map.AddTBString(ProjectAttr.TemplateNo, null, "模板编号", true, true, 0, 100, 10);
                map.AddTBString(ProjectAttr.TemplateName, null, "模板名称", true, true, 0, 100, 10);

                map.AddTBInt(ProjectAttr.NumTasks, 0, "任务数", true, true);
             //   map.AddTBInt(ProjectAttr.NumChecking, 0, "审核中", true, true);
                map.AddTBInt(ProjectAttr.NumComplete, 0, "已完成", true, true);

      //          { Key: 'NumTasks', Name: '任务数', IsShow: true, IsShowMobile: true, DataType: 2, width: 70 },
      //{ Key: 'NumChecking', Name: '待确认', IsShow: true, IsShowMobile: true, DataType: 2, width: 70 },
      //{ Key: 'NumComplete', Name: '完成数', IsShow: true, IsShowMobile: true, DataType: 2, width: 70 },

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 項目s
    /// </summary>
    public class Projects : EntitiesNoName
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Project();
            }
        }
        /// <summary>
        /// 項目
        /// </summary>
        public Projects() { }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Project> ToJavaList()
        {
            return (System.Collections.Generic.IList<Project>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Project> Tolist()
        {
            System.Collections.Generic.List<Project> list = new System.Collections.Generic.List<Project>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Project)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
