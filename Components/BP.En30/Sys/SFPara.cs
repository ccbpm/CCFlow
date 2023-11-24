using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 用户自定义表
    /// </summary>
    public class SFPara : EntityMyPK
    {
        public string ParaKey
        {
            get
            {
                return this.GetValStrByKey("ParaKey");
            }
        }
        public string ParaName
        {
            get
            {
                return this.GetValStrByKey("ParaName");
            }
        }
        public int ItIsSys
        {
            get
            {
                return this.GetValIntByKey("IsSys");
            }
        }
        public string Exp
        {
            get
            {
                return this.GetValStrByKey("Exp");
            }
        }

        public string ExpVal
        {
            get
            {
                string exp = this.GetValStrByKey("Exp");

                if (exp.Equals("@WebUser.No"))
                    return BP.Web.WebUser.No;
                if (exp.Equals("@WebUser.Name"))
                    return BP.Web.WebUser.Name;

                if (exp.Equals("@WebUser.FK_Dept"))
                    return BP.Web.WebUser.DeptNo;

                if (exp.Equals("@WebUser.FK_DeptName"))
                    return BP.Web.WebUser.DeptName;

                if (exp.Equals("@WebUser.OrgNo"))
                    return BP.Web.WebUser.OrgNo;

                if (exp.Equals("@WebUser.OrgName"))
                    return BP.Web.WebUser.OrgName;

                if (exp.Equals("@Token"))
                    return "xxxxxx";

                return exp;
            }
        }

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsInsert = false;
                return uac;
            }
        }
        /// <summary>
        /// 用户自定义表
        /// </summary>
        public SFPara()
        {
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
                Map map = new Map("Sys_SFPara", "参数");

                map.AddMyPK();
                map.AddTBString("RefPKVal", null, "实体主键", false, false, 1, 200, 20);
                map.AddTBString("ParaKey", null, "参数标记", true, false, 1, 200, 100);
                map.AddTBString("ParaName", null, "参数名称", true, false, 0, 200, 100);
                map.AddTBString("DataType", null, "数据类型", true, false, 0, 200, 100);
                map.AddTBString("IsSys", null, "获取类型", true, false, 0, 200, 100);
                map.AddTBString("Exp", null, "表达式", true, false, 0, 200, 100);

                //map.AddDDLStringEnum("DataType", "String", "数据类型", "@String=String@Int=Int@Float=Float", true, "", false, 100);
                //map.AddDDLStringEnum("IsSys", "String", "获取类型", "@0=内部@1=外部", true, "", false, 100);

                map.AddTBInt("Idx", 0, "序号", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 用户自定义表s
    /// </summary>
    public class SFParas : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 用户自定义表s
        /// </summary>
        public SFParas()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SFPara();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SFPara> ToJavaList()
        {
            return (System.Collections.Generic.IList<SFPara>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SFPara> Tolist()
        {
            System.Collections.Generic.List<SFPara> list = new System.Collections.Generic.List<SFPara>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SFPara)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
