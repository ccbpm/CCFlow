using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Difference;

namespace BP.Port
{
    /// <summary>
    /// 登录记录
    /// </summary>
    public class TokenAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 人员编号
        /// </summary>
        public const string EmpNo = "EmpNo";
        /// <summary>
        /// 人员名称
        /// </summary>
        public const string EmpName = "EmpName";
        /// <summary>
        /// 部门编号
        /// </summary>
        public const string DeptNo = "DeptNo";
        /// <summary>
        /// 部门名称
        /// </summary>
        public const string DeptName = "DeptName";
        /// <summary>
        /// 组织机构编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 组织名称
        /// </summary>
        public const string OrgName = "OrgName";
        /// <summary>
        /// 日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 登录设备0=PC,1=Mobile
        /// </summary>
        public const string SheBei = "SheBei";
    }
    /// <summary>
    ///  登录记录
    /// </summary>
    public class Token : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 组织编码
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(TokenAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(TokenAttr.OrgNo, value);
            }
        }
        public string OrgName
        {
            get
            {
                return this.GetValStrByKey(TokenAttr.OrgName);
            }
            set
            {
                this.SetValByKey(TokenAttr.OrgName, value);
            }
        }

      
        public string EmpNo
        {
            get
            {
                return this.GetValStrByKey(TokenAttr.EmpNo);
            }
            set
            {
                this.SetValByKey(TokenAttr.EmpNo, value);
            }
        }

        public string EmpName
        {
            get
            {
                return this.GetValStrByKey(TokenAttr.EmpName);
            }
            set
            {
                this.SetValByKey(TokenAttr.EmpName, value);
            }
        }
        public string DeptNo
        {
            get
            {
                return this.GetValStrByKey(TokenAttr.DeptNo);
            }
            set
            {
                this.SetValByKey(TokenAttr.DeptNo, value);
            }
        }
        public string DeptName
        {
            get
            {
                return this.GetValStrByKey(TokenAttr.DeptName);
            }
            set
            {
                this.SetValByKey(TokenAttr.DeptName, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStrByKey(TokenAttr.RDT);
            }
            set
            {
                this.SetValByKey(TokenAttr.RDT, value);
            }
        }
        public int SheBei
        {
            get
            {
                return this.GetValIntByKey(TokenAttr.SheBei);
            }
            set
            {
                this.SetValByKey(TokenAttr.SheBei, value);
            }
        }
        #endregion

        #region 实现基本的方方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 登录记录
        /// </summary>
        public Token()
        {
        }
        /// <summary>
        /// 登录记录
        /// </summary>
        /// <param name="pkval"></param>
        public Token(string pkval) : base(pkval) { }
        #endregion

        /// <summary>
        /// 登录记录Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;
                Map map = new Map("Port_Token", "登录记录");
                map.CodeStruct = "2";

                map.AddMyPK();

                map.AddTBString(TokenAttr.EmpNo, null, "人员编号", true, false, 0, 100, 20);
                map.AddTBString(TokenAttr.EmpName, null, "人员名称", true, false, 0, 100, 20);

                map.AddTBString(TokenAttr.DeptNo, null, "部门编号", true, false, 0, 100, 20);
                map.AddTBString(TokenAttr.DeptName, null, "部门名称", true, false, 0, 100, 20);

                map.AddTBString(TokenAttr.OrgNo, null, "组织编号", true, false, 0, 100, 20);
                map.AddTBString(TokenAttr.OrgName, null, "组织名称", true, false, 0, 100, 20);

                map.AddTBDateTime(TokenAttr.RDT, null, "记录日期", true, false);


                map.AddTBInt(TokenAttr.SheBei, 0, "0=PC,1=移动", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }

    }
    /// <summary>
    /// 登录记录
    /// </summary>
    public class Tokens : EntitiesMyPK
    {
        #region 构造方法..
        /// <summary>
        /// 登录记录s
        /// </summary>
        public Tokens() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Token();
            }
        }
        #endregion 构造方法..

        #region 查询..
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <param name="orderBy">排序</param>
        /// <returns></returns>
        public override int RetrieveAll(string orderBy)
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                return base.RetrieveAll(orderBy);

            //集团模式下的角色体系: @0=每套组织都有自己的角色体系@1=所有的组织共享一套岗则体系.
            if (BP.Difference.SystemConfig.GroupStationModel == 1)
                return base.RetrieveAll();

            //按照orgNo查询.
            return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo, orderBy);
        }
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                return base.RetrieveAll("Idx");

            //集团模式下的角色体系: @0=每套组织都有自己的角色体系@1=所有的组织共享一套岗则体系.
            if (BP.Difference.SystemConfig.GroupStationModel == 1)
                return base.RetrieveAll("Idx");

            //按照orgNo查询.
            return this.Retrieve("OrgNo", BP.Web.WebUser.OrgNo,"Idx");
        }
        #endregion 查询..

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Token> ToJavaList()
        {
            return (System.Collections.Generic.IList<Token>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Token> Tolist()
        {
            System.Collections.Generic.List<Token> list = new System.Collections.Generic.List<Token>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Token)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
