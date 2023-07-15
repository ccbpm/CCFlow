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
    public class TemplateAttr : EntityNoNameAttr
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
        public const string PrjDesc = "PrjDesc";
        public const string TaskModel = "TaskModel";
        #endregion
    }
    /// <summary>
    /// 項目
    /// </summary>
    public class Template : EntityNoName
    {
        #region 属性.
        public string TaskModel
        {
            get
            {
                return this.GetValStringByKey(TemplateAttr.TaskModel);
            }
            set
            {
                this.SetValByKey(TemplateAttr.TaskModel, value);
            }
        }
        public string PrjDesc
        {
            get
            {
                return this.GetValStringByKey(TemplateAttr.PrjDesc);
            }
            set
            {
                this.SetValByKey(TemplateAttr.PrjDesc, value);
            }
        }
        public int PRI
        {
            get
            {
                return this.GetValIntByKey(TemplateAttr.PRI);
            }
            set
            {
                this.SetValByKey(TemplateAttr.PRI, value);
            }
        }
        public int PrjSta
        {
            get
            {
                return this.GetValIntByKey(TemplateAttr.PrjSta);
            }
            set
            {
                this.SetValByKey(TemplateAttr.PrjSta, value);
            }
        }
        public int WCL
        {
            get
            {
                return this.GetValIntByKey(TemplateAttr.WCL);
            }
            set
            {
                this.SetValByKey(TemplateAttr.WCL, value);
            }
        }
        public string StarterNo
        {
            get
            {
                return this.GetValStringByKey(TemplateAttr.StarterNo);
            }
            set
            {
                this.SetValByKey(TemplateAttr.StarterNo, value);
            }
        }
        public string StarterName
        {
            get
            {
                return this.GetValStringByKey(TemplateAttr.StarterName);
            }
            set
            {
                this.SetValByKey(TemplateAttr.StarterName, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(TemplateAttr.RDT);
            }
            set
            {
                this.SetValByKey(TemplateAttr.RDT, value);
            }
        }
        #endregion 属性.

        #region 构造函数
        /// <summary>
        /// 項目设置
        /// </summary>
        public Template()
        {
        }
        /// <summary>
        /// 項目设置
        /// </summary>
        /// <param name="nodeid"></param>
        public Template(string no)
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

                Map map = new Map("TA_Template", "項目");

                map.AddTBStringPK(TemplateAttr.No, null, "编号", true, true, 5, 5, 5);
                map.AddTBString(TemplateAttr.Name, null, "名称", true, true, 0, 100, 10);
                map.AddTBString(TemplateAttr.TaskModel, null, "模式", true, true, 0, 100, 10);
                
                map.AddTBStringDoc(TemplateAttr.PrjDesc, null, "项目描述", true, false, true);

                map.AddTBInt(TemplateAttr.PrjSta, 0, "状态", true, true);
                map.AddTBInt(TemplateAttr.PRI, 0, "优先级", true, true);
                map.AddTBInt(TemplateAttr.WCL, 0, "完成率", true, true);

                map.AddTBString(TemplateAttr.StarterNo, null, "发起人", true, false, 0, 100, 10, true);
                map.AddTBString(TemplateAttr.StarterName, null, "名称", true, false, 0, 100, 10, true);
                map.AddTBDateTime(TemplateAttr.RDT, null, "发起日期", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 項目s
    /// </summary>
    public class Templates : EntitiesNoName
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Template();
            }
        }
        /// <summary>
        /// 項目
        /// </summary>
        public Templates() { }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Template> ToJavaList()
        {
            return (System.Collections.Generic.IList<Template>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Template> Tolist()
        {
            System.Collections.Generic.List<Template> list = new System.Collections.Generic.List<Template>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Template)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
