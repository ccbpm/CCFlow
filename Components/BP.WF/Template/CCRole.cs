using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 抄送属性
    /// </summary>
    public class CCRoleAttr
    {
        #region 基本属性
        /// <summary>
        /// 标题
        /// </summary>
        public const string NodeID = "NodeID";
        /// <summary>
        /// 抄送内容
        /// </summary>
        public const string FlowNo = "FlowNo";

        public const string CCRoleExcType = "CCRoleExcType";
        public const string EnIDs = "EnIDs";
        public const string Tag2 = "Tag2";
        public const string CCStaWay = "CCStaWay";
        public const string Idx = "Idx";
        #endregion
    }
    /// <summary>
    /// 抄送
    /// </summary>
    public class CCRole : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 获得抄送人
        /// </summary>
        /// <param name="rpt"></param>
        /// <returns></returns>
       
        /// <summary>
        ///节点ID
        /// </summary>
        public int NodeID
        {
            get
            {
                return this.GetValIntByKey(NodeAttr.NodeID);
            }
            set
            {
                this.SetValByKey(NodeAttr.NodeID, value);
            }
        }
        /// <summary>
        /// 执行类型
        /// </summary>
        public CCRoleExcType CCRoleExcType
        {
            get
            {
                return (CCRoleExcType)this.GetValIntByKey(CCRoleAttr.CCRoleExcType);
            }
            set
            {
                this.SetValByKey(CCRoleAttr.CCRoleExcType, value);
            }
        }
        public CCStaWay CCStaWay
        {
            get
            {
                return (CCStaWay)this.GetValIntByKey(CCRoleAttr.CCStaWay);
            }
        }

        /// <summary>
        /// 多个元素的分割.
        /// </summary>
        public string EnIDs
        {
            get
            {
                string str= this.GetValStringByKey(CCRoleAttr.EnIDs);
                str = str.Replace(",","','");
                str = "'" + str+"'";
                str = str.Replace("''","'");
                str = str.Replace("''", "'");
                return str;
            }
        }
        public string EnDeptIDs
        {
            get
            {
                string str= this.GetValStringByKey(CCRoleAttr.Tag2);
                str = str.Replace(",","','");
                str = "'" + str+"'";
                str = str.Replace("''","'");
                str = str.Replace("''", "'");
                return str;
            }
        }
        
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();

                if (BP.Web.WebUser.IsAdmin == false)
                {
                    uac.IsView = false;
                    return uac;
                }
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = true;
                return uac;
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 抄送设置
        /// </summary>
        public CCRole()
        {
        }
        /// <summary>
        /// 抄送设置
        /// </summary>
        /// <param name="nodeid"></param>
        public CCRole(int nodeid)
        {
            this.NodeID = nodeid;
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

                Map map = new Map("WF_CCRole", "抄送规则");

                map.AddMyPK();
                map.AddTBInt(CCRoleAttr.NodeID, 0, "节点", false, true);
                map.AddTBString(CCRoleAttr.FlowNo, null, "流程编号", false, false, 0, 10, 50, true);
                // 执行类型.
                string val = "@0=按表单字段计算@1=按人员计算@2=按角色计算@3=按部门计算@4=按SQL计算@5=按接受人规则计算";
                map.AddDDLSysEnum(CCRoleAttr.CCRoleExcType, 0, "执行类型", true, true, CCRoleAttr.CCRoleExcType, val);
                map.AddTBInt(CCRoleAttr.CCStaWay, 0, "CCStaWay", false, true);

                map.AddTBStringDoc(CCRoleAttr.EnIDs, null, "执行内容1", true, false, true);
                map.AddTBStringDoc(CCRoleAttr.Tag2, null, "执行内容2", true, false, true);

                map.AddTBInt(CCRoleAttr.Idx, 0, "Idx", false, true);

                map.AddTBAtParas(300);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 执行类型
    /// </summary>
    public enum CCRoleExcType
    {
        /// <summary>
        /// 按表单字段计算
        /// </summary>
        ByFrmField=0,
        /// <summary>
        /// 按人员
        /// </summary>
        ByEmps = 1,
        /// <summary>
        /// 按照岗位
        /// </summary>
        ByStations = 2,
        /// <summary>
        /// 按部门
        /// </summary>
        ByDepts = 3,
        /// <summary>
        /// 按SQL
        /// </summary>
        BySQLs = 4,
        /// <summary>
        /// 按照节点绑定的接受人计算.
        /// </summary>
        ByDeliveryWay = 5
    }
    /// <summary>
    /// 抄送s
    /// </summary>
    public class CCRoles : EntitiesMyPK
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new CCRole();
            }
        }
        /// <summary>
        /// 抄送
        /// </summary>
        public CCRoles() { }
        public CCRoles(int nodeID)
        {
            this.Retrieve(NodeAttr.NodeID, nodeID, NodeAttr.Step);
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<CCRole> ToJavaList()
        {
            return (System.Collections.Generic.IList<CCRole>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<CCRole> Tolist()
        {
            System.Collections.Generic.List<CCRole> list = new System.Collections.Generic.List<CCRole>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((CCRole)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
