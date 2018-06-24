using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 个人设置
    /// </summary>
    public class PerSettingAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 系统
        /// </summary>
        public const string FK_App = "FK_App";
        /// <summary>
        /// 人员
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// UserNo
        /// </summary>
        public const string UserNo = "UserNo";
        /// <summary>
        /// 密码
        /// </summary>
        public const string UserPass = "UserPass";
        /// <summary>
        /// Idx
        /// </summary>
        public const string Idx = "Idx";
    }
    /// <summary>
    /// 个人设置
    /// </summary>
    public class PerSetting : EntityMyPK
    {
        #region 属性
        /// <summary>
        /// 系统
        /// </summary>
        public string FK_App
        {
            get
            {
                return this.GetValStringByKey(PerSettingAttr.FK_App);
            }
            set
            {
                this.SetValByKey(PerSettingAttr.FK_App, value);
            }
        }
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(PerSettingAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(PerSettingAttr.FK_Emp, value);
            }
        }
        public string UserNo
        {
            get
            {
                return this.GetValStringByKey(PerSettingAttr.UserNo);
            }
            set
            {
                this.SetValByKey(PerSettingAttr.UserNo, value);
            }
        }
        public string UserPass
        {
            get
            {
                return this.GetValStringByKey(PerSettingAttr.UserPass);
            }
            set
            {
                this.SetValByKey(PerSettingAttr.UserPass, value);
            }
        }
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(PerSettingAttr.Idx);
            }
            set
            {
                this.SetValByKey(PerSettingAttr.Idx, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 个人设置
        /// </summary>
        public PerSetting()
        {
        }
        /// <summary>
        /// 个人设置
        /// </summary>
        /// <param name="mypk"></param>
        public PerSetting(string no)
        {
            this.MyPK = no;
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

                Map map = new Map("GPM_PerSetting");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "个人设置";
                map.EnType = EnType.Sys;
                map.AddMyPK();

                map.AddTBString(PerSettingAttr.FK_Emp, null, "人员", true, false, 0, 200, 20);
                map.AddTBString(PerSettingAttr.FK_App, null, "系统", true, false, 0, 200, 20);

                map.AddTBString(PerSettingAttr.UserNo, null, "UserNo", true, false, 0, 200, 20, true);
                map.AddTBString(PerSettingAttr.UserPass, null, "UserPass", true, false, 0, 200, 20, true);
                map.AddTBInt(PerSettingAttr.Idx, 0, "显示顺序", false, true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            this.MyPK = this.FK_Emp + "_" + this.FK_App;
            return base.beforeUpdateInsertAction();
        }
    }
    /// <summary>
    /// 个人设置s
    /// </summary>
    public class PerSettings : EntitiesMyPK
    {
        #region 构造
        /// <summary>
        /// 个人设置s
        /// </summary>
        public PerSettings()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new PerSetting();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<PerSetting> ToJavaList()
        {
            return (System.Collections.Generic.IList<PerSetting>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<PerSetting> Tolist()
        {
            System.Collections.Generic.List<PerSetting> list = new System.Collections.Generic.List<PerSetting>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((PerSetting)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
