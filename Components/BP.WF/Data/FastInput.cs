using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.WF.Port;
using BP.WF;
using BP.Sys;
using BP.Web;

namespace BP.WF.Data
{
    /// <summary>
    /// 常用语属性
    /// </summary>
    public class FastInputAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 类型
        /// </summary>
        public const string ContrastKey = "ContrastKey";
        /// <summary>
        /// Vals
        /// </summary>
        public const string Vals = "Vals";
        /// <summary>
        /// 流程编号
        /// </summary>
        public const string FK_Emp = "FK_Emp";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
    }
	/// <summary>
	/// 常用语
	/// </summary>
    public class FastInput : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 字段
        /// </summary>
        public string ContrastKey
        {
            get
            {
                return this.GetValStringByKey(FastInputAttr.ContrastKey);
            }
            set
            {
                this.SetValByKey(FastInputAttr.ContrastKey, value);
            }
        }
        /// <summary>
        /// 人员
        /// </summary>
        public string FK_Emp
        {
            get
            {
                return this.GetValStringByKey(FastInputAttr.FK_Emp);
            }
            set
            {
                this.SetValByKey(FastInputAttr.FK_Emp, value);
            }
        }
        public string Vals
        {
            get
            {
                return this.GetValStringByKey(FastInputAttr.Vals);
            }
            set
            {
                this.SetValByKey(FastInputAttr.Vals, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 常用语
        /// </summary>
        public FastInput()
        {
        }
        /// <summary>
        /// 常用语
        /// </summary>
        /// <param name="no"></param>
        public FastInput(string mypk)
            : base(mypk)
        {
        }
        /// <summary>
        /// 更新前做的事情
        /// </summary>
        /// <returns></returns>
       
        protected override bool beforeUpdateInsertAction()
        {
            if (this.MyPK.Equals(""))
            {
                this.MyPK = DBAccess.GenerGUID();
            }

            return base.beforeUpdateInsertAction();
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

                Map map = new Map("Sys_UserRegedit", "常用语");

                map.AddMyPK();

                //该表单对应的表单ID
                map.AddTBString(FastInputAttr.ContrastKey, null, "类型CYY", true, false, 0, 20, 20);
                map.AddTBString(FastInputAttr.FK_Emp, null, "人员编号", true, false, 0, 100, 4);
                map.AddTBString(FastInputAttr.Vals, null, "值", true, false, 0, 500, 500);
                //map.AddTBInt(FastInputAttr.Idx, 0, "Idx", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        /// <summary>
        /// 上移
        /// </summary>
        /// <returns></returns>
        public string DoUp()
        {
              this.DoOrderUp(FastInputAttr.ContrastKey, "CYY",
                FastInputAttr.FK_Emp, WebUser.No, "Idx") ;

            return "移动成功.";
        }

        public string DoDown()
        {
            this.DoOrderDown(FastInputAttr.ContrastKey, "CYY",
              FastInputAttr.FK_Emp, WebUser.No, "Idx");
            return "移动成功.";
        }
        public void Add(string groupKey)
        {
            if (groupKey == "Comment")
            {
                FastInput en = new FastInput();
                en.MyPK = DBAccess.GenerGUID();
                en.ContrastKey = groupKey;
                en.Vals = "已阅";
                en.FK_Emp = WebUser.No;
                en.Insert();
            }
                if (groupKey == "CYY")
            {
                FastInput en = new FastInput();
                en.MyPK = DBAccess.GenerGUID();
                en.ContrastKey = groupKey;
                en.Vals = "同意";
                en.FK_Emp = WebUser.No;
                en.Insert();

                en = new FastInput();
                en.MyPK = DBAccess.GenerGUID();
                en.ContrastKey = groupKey;
                en.Vals = "不同意";
                en.FK_Emp = WebUser.No;
                en.Insert();

                en = new FastInput();
                en.MyPK = DBAccess.GenerGUID();
                en.ContrastKey = groupKey;
                en.Vals = "同意，请领导批示";
                en.FK_Emp = WebUser.No;
                en.Insert();

                en = new FastInput();
                en.MyPK = DBAccess.GenerGUID();
                en.ContrastKey = groupKey;
                en.Vals = "同意办理";
                en.FK_Emp = WebUser.No;
                en.Insert();

                en = new FastInput();
                en.MyPK = DBAccess.GenerGUID();
                en.ContrastKey = groupKey;
                en.Vals = "情况属实报领导批准";
                en.FK_Emp = WebUser.No;
                en.Insert();
            }
            
        }

    }
	/// <summary>
    /// 常用语s
	/// </summary>
    public class FastInputs : EntitiesMyPK
    {
        /// <summary>
        /// 常用语s
        /// </summary>
        public FastInputs()
        {
        }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new FastInput();
            }
        }
        
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {

            int val= this.Retrieve(FastInputAttr.ContrastKey, "CYY",
                FastInputAttr.FK_Emp, BP.Web.WebUser.No);

            if (val==0)
            {
                FastInput en = new FastInput();
                en.MyPK = DBAccess.GenerGUID();
                en.Vals = "同意";
                en.FK_Emp = WebUser.No;
                en.Insert();

                en = new FastInput();
                en.MyPK = DBAccess.GenerGUID();
                en.Vals = "不同意";
                en.FK_Emp = WebUser.No;
                en.Insert();

                en = new FastInput();
                en.MyPK = DBAccess.GenerGUID();
                en.Vals = "同意，请领导批示";
                en.FK_Emp = WebUser.No;
                en.Insert();

                en = new FastInput();
                en.MyPK = DBAccess.GenerGUID();
                en.Vals = "同意办理";
                en.FK_Emp = WebUser.No;
                en.Insert();

                en = new FastInput();
                en.MyPK = DBAccess.GenerGUID();
                en.Vals = "情况属实报领导批准";
                en.FK_Emp = WebUser.No;
                en.Insert();

                val = this.Retrieve(FastInputAttr.ContrastKey, "CYY",
                FastInputAttr.FK_Emp, BP.Web.WebUser.No);
            }
            return val;
        }
        
        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<FastInput> ToJavaList()
        {
            return (System.Collections.Generic.IList<FastInput>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<FastInput> Tolist()
        {
            System.Collections.Generic.List<FastInput> list = new System.Collections.Generic.List<FastInput>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((FastInput)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
