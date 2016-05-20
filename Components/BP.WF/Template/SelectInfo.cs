using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
	/// <summary>
	/// 接受人信息属性
	/// </summary>
    public class SelectInfoAttr
    {
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string WorkID = "WorkID";
        /// <summary>
        /// 接受节点
        /// </summary>
        public const string AcceptNodeID = "AcceptNodeID";
        /// <summary>
        /// left信息
        /// </summary>
        public const string InfoLeft = "InfoLeft";
        /// <summary>
        /// 中间信息
        /// </summary>
        public const string InfoCenter = "InfoCenter";
        public const string InfoRight = "InfoRight";
        public const string AccType = "AccType";
    }
	/// <summary>
	/// 接受人信息
	/// </summary>
    public class SelectInfo : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        ///工作ID
        /// </summary>
        public Int64 WorkID
        {
            get
            {
                return this.GetValInt64ByKey(SelectInfoAttr.WorkID);
            }
            set
            {
                this.SetValByKey(SelectInfoAttr.WorkID, value);
            }
        }
        /// <summary>
        ///选择节点
        /// </summary>
        public int AcceptNodeID
        {
            get
            {
                return this.GetValIntByKey(SelectInfoAttr.AcceptNodeID);
            }
            set
            {
                this.SetValByKey(SelectInfoAttr.AcceptNodeID, value);
            }
        }
        public int AccType
        {
            get
            {
                return this.GetValIntByKey(SelectInfoAttr.AccType);
            }
            set
            {
                this.SetValByKey(SelectInfoAttr.AccType, value);
            }
        }
        /// <summary>
        /// 信息
        /// </summary>
        public string Info
        {
            get
            {
                return this.GetValStringByKey(SelectInfoAttr.InfoLeft);
            }
            set
            {
                this.SetValByKey(SelectInfoAttr.InfoLeft, value);
            }
        }
        public string InfoCenter
        {
            get
            {
                return this.GetValStringByKey(SelectInfoAttr.InfoCenter);
            }
            set
            {
                this.SetValByKey(SelectInfoAttr.InfoCenter, value);
            }
        }
        public string InfoRight
        {
            get
            {
                return this.GetValStringByKey(SelectInfoAttr.InfoRight);
            }
            set
            {
                this.SetValByKey(SelectInfoAttr.InfoRight, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 接受人信息
        /// </summary>
        public SelectInfo()
        {
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

                Map map = new Map("WF_SelectInfo", "选择接受/抄送人节点信息");
                 
                map.AddMyPK();
                map.AddTBInt(SelectInfoAttr.AcceptNodeID, 0, "接受节点", true, false);
                map.AddTBInt(SelectInfoAttr.WorkID, 0, "工作ID", true, false);
                map.AddTBString(SelectInfoAttr.InfoLeft, null, "InfoLeft", true, false, 0, 200, 10);
                map.AddTBString(SelectInfoAttr.InfoCenter, null, "InfoCenter", true, false, 0, 200, 10);
                map.AddTBString(SelectInfoAttr.InfoRight, null, "InfoLeft", true, false, 0, 200, 10);
                map.AddTBInt(SelectAccperAttr.AccType, 0, "类型(@0=接受人@1=抄送人)", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        protected override bool beforeUpdateInsertAction()
        {
            this.MyPK = this.AcceptNodeID + "_" + this.WorkID + "_" + this.AccType; ;
            return base.beforeUpdateInsertAction();
        }
    }
	/// <summary>
	/// 接受人信息
	/// </summary>
    public class SelectInfos : EntitiesMyPK
    {
        /// <summary>
        /// 接受人信息
        /// </summary>
        public SelectInfos() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SelectInfo();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SelectInfo> ToJavaList()
        {
            return (System.Collections.Generic.IList<SelectInfo>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SelectInfo> Tolist()
        {
            System.Collections.Generic.List<SelectInfo> list = new System.Collections.Generic.List<SelectInfo>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SelectInfo)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
