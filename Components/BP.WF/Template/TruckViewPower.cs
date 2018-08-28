using System;
using System.Collections;
using BP.DA;
using BP.Sys;
using BP.En;
using BP.WF.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 流程轨迹权限属性
    /// </summary>
    public class TruckViewPowerAttr : EntityNoNameAttr
    {
        #region 权限组.
        /// <summary>
        /// 发起人可看
        /// </summary>
        public const string PStarter = "PStarter";
        /// <summary>
        /// 参与人可看
        /// </summary>
        public const string PWorker = "PWorker";
        /// <summary>
        /// 被抄送人可看
        /// </summary>
        public const string PCCer = "PCCer";
        /// <summary>
        /// 本部门人可看
        /// </summary>
        public const string PMyDept = "PMyDept";
        /// <summary>
        /// 部门编号
        /// </summary>
        public const string PSpecDeptExt = "PSpecDeptExt";
        /// <summary>
        /// 直属上级部门可看
        /// </summary>
        public const string PPMyDept = "PPMyDept";
        /// <summary>
        /// 上级部门可看
        /// </summary>
        public const string PPDept = "PPDept";
        /// <summary>
        /// 平级部门可看
        /// </summary>
        public const string PSameDept = "PSameDept";
        /// <summary>
        /// 指定部门可看
        /// </summary>
        public const string PSpecDept = "PSpecDept";
        /// <summary>
        /// 指定的岗位可看
        /// </summary>
        public const string PSpecSta = "PSpecSta";
        /// <summary>
        /// 岗位编号
        /// </summary>
        public const string PSpecStaExt = "PSpecStaExt";

        /// <summary>
        /// 指定的权限组可看
        /// </summary>
        public const string PSpecGroup = "PSpecGroup";
        /// <summary>
        /// 指定的权限组编号
        /// </summary>
        public const string PSpecGroupExt = "PSpecGroupExt";
        /// <summary>
        /// 指定的人员可看
        /// </summary>
        public const string PSpecEmp = "PSpecEmp";
        /// <summary>
        /// 人员编号
        /// </summary>
        public const string PSpecEmpExt = "PSpecEmpExt";
        #endregion 权限组.
    }
    /// <summary>
    /// 流程轨迹权限
    /// </summary>
    public class TruckViewPower : EntityNoName
    {
        #region 属性
        /// <summary>
        /// 发起人可看
        /// </summary>
        public bool PStarter
        {
            get
            {
                return this.GetValBooleanByKey(TruckViewPowerAttr.PStarter);
            }
            set
            {
                this.SetValByKey(TruckViewPowerAttr.PStarter, value);
            }
        }
        /// <summary>
        /// 参与人可见
        /// </summary>
        public bool PWorker
        {
            get
            {
                return this.GetValBooleanByKey(TruckViewPowerAttr.PWorker);
            }
            set
            {
                this.SetValByKey(TruckViewPowerAttr.PWorker, value);
            }
        }
        /// <summary>
        /// 被抄送人可见
        /// </summary>
        public bool PCCer
        {
            get
            {
                return this.GetValBooleanByKey(TruckViewPowerAttr.PCCer);
            }
            set
            {
                this.SetValByKey(TruckViewPowerAttr.PCCer, value);
            }
        }
        /// <summary>
        /// 本部门可见
        /// </summary>
        public bool PMyDept
        {
            get
            {
                return this.GetValBooleanByKey(TruckViewPowerAttr.PMyDept);
            }
            set
            {
                this.SetValByKey(TruckViewPowerAttr.PMyDept, value);
            }
        }
        /// <summary>
        /// 直属上级部门可见
        /// </summary>
        public bool PPMyDept
        {
            get
            {
                return this.GetValBooleanByKey(TruckViewPowerAttr.PPMyDept);
            }
            set
            {
                this.SetValByKey(TruckViewPowerAttr.PPMyDept, value);
            }
        }
        /// <summary>
        /// 上级部门可见
        /// </summary>
        public bool PPDept
        {
            get
            {
                return this.GetValBooleanByKey(TruckViewPowerAttr.PPDept);
            }
            set
            {
                this.SetValByKey(TruckViewPowerAttr.PPDept, value);
            }
        }
        /// <summary>
        /// 平级部门可见
        /// </summary>
        public bool PSameDept
        {
            get
            {
                return this.GetValBooleanByKey(TruckViewPowerAttr.PSameDept);
            }
            set
            {
                this.SetValByKey(TruckViewPowerAttr.PSameDept, value);
            }
        }
        /// <summary>
        /// 指定部门可见
        /// </summary>
        public bool PSpecDept
        {
            get
            {
                return this.GetValBooleanByKey(TruckViewPowerAttr.PSpecDept);
            }
            set
            {
                this.SetValByKey(TruckViewPowerAttr.PSpecDept, value);
            }
        }
        /// <summary>
        /// 部门编号
        /// </summary>
        public string PSpecDeptExt
        {
            get
            {
                return this.GetValStrByKey(TruckViewPowerAttr.PSpecDeptExt);
            }
            set
            {
                this.SetValByKey(TruckViewPowerAttr.PSpecDeptExt, value);
            }
        }
        /// <summary>
        /// 指定岗位可见
        /// </summary>
        public bool PSpecSta
        {
            get
            {
                return this.GetValBooleanByKey(TruckViewPowerAttr.PSpecSta);
            }
            set
            {
                this.SetValByKey(TruckViewPowerAttr.PSpecSta, value);
            }
        }
        /// <summary>
        /// 岗位编号
        /// </summary>
        public string PSpecStaExt
        {
            get
            {
                return this.GetValStrByKey(TruckViewPowerAttr.PSpecStaExt);
            }
            set
            {
                this.SetValByKey(TruckViewPowerAttr.PSpecStaExt, value);
            }
        }

        /// <summary>
        /// 权限组
        /// </summary>
        public bool PSpecGroup
        {
            get
            {
                return this.GetValBooleanByKey(TruckViewPowerAttr.PSpecGroup);
            }
            set
            {
                this.SetValByKey(TruckViewPowerAttr.PSpecGroup, value);
            }
        }

        /// <summary>
        /// 权限组编号
        /// </summary>
        public string PSpecGroupExt
        {
            get
            {
                return this.GetValStrByKey(TruckViewPowerAttr.PSpecGroupExt);
            }
            set
            {
                this.SetValByKey(TruckViewPowerAttr.PSpecGroupExt, value);
            }
        }

        /// <summary>
        /// 指定的人员
        /// </summary>
        public bool PSpecEmp
        {
            get
            {
                return this.GetValBooleanByKey(TruckViewPowerAttr.PSpecEmp);
            }
            set
            {
                this.SetValByKey(TruckViewPowerAttr.PSpecEmp, value);
            }
        }
        /// <summary>
        /// 指定编号
        /// </summary>
        public string PSpecEmpExt
        {
            get
            {
                return this.GetValStrByKey(TruckViewPowerAttr.PSpecEmpExt);
            }
            set
            {
                this.SetValByKey(TruckViewPowerAttr.PSpecEmpExt, value);
            }
        }








        #endregion

        #region 构造方法
        /// <summary>
        /// 流程轨迹权限
        /// </summary>
        public TruckViewPower()
        {
        }

        public TruckViewPower(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("WF_Flow", "流程模版主表");

                map.Java_SetDepositaryOfEntity(Depositary.Application);


                map.AddTBStringPK(TruckViewPowerAttr.No, null, "编号", true, true, 1, 10, 3);
                map.AddTBString(TruckViewPowerAttr.Name, null, "名称", true, false, 0, 50, 10, true);


                #region 权限控制. 此部分与流程属性同步.
                map.AddBoolean(TruckViewPowerAttr.PStarter, true, "发起人可看(必选)", true, false, true);
                map.AddBoolean(TruckViewPowerAttr.PWorker, true, "参与人可看(必选)", true, false, true);
                map.AddBoolean(TruckViewPowerAttr.PCCer, true, "被抄送人可看(必选)", true, false, true);

                map.AddBoolean(TruckViewPowerAttr.PMyDept, true, "本部门人可看", true, true, true);
                map.AddBoolean(TruckViewPowerAttr.PPMyDept, true, "直属上级部门可看(比如:我是)", true, true, true);

                map.AddBoolean(TruckViewPowerAttr.PPDept, true, "上级部门可看", true, true, true);
                map.AddBoolean(TruckViewPowerAttr.PSameDept, true, "平级部门可看", true, true, true);

                map.AddBoolean(TruckViewPowerAttr.PSpecDept, true, "指定部门可看", true, true, false);
                map.AddTBString(TruckViewPowerAttr.PSpecDeptExt, null, "部门编号", true, false, 0, 200, 100, false);


                map.AddBoolean(TruckViewPowerAttr.PSpecSta, true, "指定的岗位可看", true, true, false);
                map.AddTBString(TruckViewPowerAttr.PSpecStaExt, null, "岗位编号", true, false, 0, 200, 100, false);

                map.AddBoolean(TruckViewPowerAttr.PSpecGroup, true, "指定的权限组可看", true, true, false);
                map.AddTBString(TruckViewPowerAttr.PSpecGroupExt, null, "权限组", true, false, 0, 200, 100, false);

                map.AddBoolean(TruckViewPowerAttr.PSpecEmp, true, "指定的人员可看", true, true, false);
                map.AddTBString(TruckViewPowerAttr.PSpecEmpExt, null, "指定的人员编号", true, false, 0, 200, 100, false);
                #endregion 权限控制.


                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 公用方法.
        /// <summary>
        /// 检查指定的人员是否可以产看该轨迹图.
        /// </summary>
        /// <param name="workid">流程ID</param>
        /// <param name="userNo">操作员</param>
        /// <returns></returns>
        public bool CheckICanView(Int64 workid, string userNo)
        {
            if (userNo == null)
                userNo = BP.Web.WebUser.No;
            return true;
        }
        #endregion
    }
    /// <summary>
    /// 流程轨迹权限s
    /// </summary>
    public class TruckViewPowers : EntitiesNoName
    {
        /// <summary>
        /// 流程轨迹权限s
        /// </summary>
        public TruckViewPowers()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new TruckViewPower();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<TruckViewPower> ToJavaList()
        {
            return (System.Collections.Generic.IList<TruckViewPower>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<TruckViewPower> Tolist()
        {
            System.Collections.Generic.List<TruckViewPower> list = new System.Collections.Generic.List<TruckViewPower>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((TruckViewPower)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
