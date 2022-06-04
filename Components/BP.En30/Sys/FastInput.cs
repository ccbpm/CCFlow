using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Web;

namespace BP.Sys
{
    /// <summary>
    /// 常用语属性
    /// </summary>
    public class FastInputAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 表单ID.
        /// </summary>
        public const string EnsName = "EnsName";
        /// <summary>
        /// 字段
        /// </summary>
        public const string AttrKey = "AttrKey";
        /// <summary>
        /// 类型
        /// </summary>
        public const string CfgKey = "CfgKey";
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
        /// 表单ID
        /// </summary>
        public string EnsName
        {
            get
            {
                return this.GetValStringByKey(FastInputAttr.EnsName);
            }
            set
            {
                this.SetValByKey(FastInputAttr.EnsName, value);
            }
        }
        /// <summary>
        /// 属性
        /// </summary>
        public string AttrKey
        {
            get
            {
                return this.GetValStringByKey(FastInputAttr.AttrKey);
            }
            set
            {
                this.SetValByKey(FastInputAttr.AttrKey, value);
            }
        }
        /// <summary>
        /// 配置的变量
        /// </summary>
        public string CfgKey
        {
            get
            {
                return "CYY"; 
            }
            set
            {
                this.SetValByKey(FastInputAttr.CfgKey, value);
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
        public FastInput(string mypk) : base(mypk)
        {
        }
        /// <summary>
        /// 更新前做的事情
        /// </summary>
        /// <returns></returns>
       
        protected override bool beforeUpdateInsertAction()
        {
            if (DataType.IsNullOrEmpty(this.MyPK) == true)
                this.setMyPK(DBAccess.GenerGUID());

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

                /*
                 * 常用语分为两个模式: 流程的常用语，与表单字段的常用语. 
                 * 这两个模式都存储在同一个表里.
                 * 
                 * 流程的常用语存储格式为: 
                 *  CfgKey=Flow,  EnsName=Flow,  AttrKey=WorkCheck,FlowBBS,WorkReturn 三种类型.
                 *  
                 * 表单的常用语为存储格式为:
                 *  CfgKey=Frm,  EnsName=myformID, AttrKey=myFieldName, 
                 * 
                 */

                //该表单对应的表单ID ， 
                //CfgKey=Flow, EnsName=Flow 是流程的常用语.   Filed

                map.AddMyPK();
                map.AddTBString(FastInputAttr.CfgKey, null, "类型Flow,Frm", true, false, 0, 20, 20);

                map.AddTBString(FastInputAttr.EnsName, null, "表单ID", true, false, 0, 100, 4);
                map.AddTBString(FastInputAttr.AttrKey, null, "字段", true, false, 0, 100, 4);
                map.AddTBString(FastInputAttr.FK_Emp, null, "人员", true, false, 0, 100, 4);

                map.AddTBString(FastInputAttr.Vals, null, "值", true, false, 0, 500, 500);

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
              this.DoOrderUp(FastInputAttr.CfgKey, "CYY", 
                  FastInputAttr.EnsName,this.EnsName, 
                  FastInputAttr.AttrKey, this.AttrKey,
                FastInputAttr.FK_Emp, WebUser.No, "Idx") ;

            return "移动成功.";
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <returns></returns>
        public string DoDown()
        {
            this.DoOrderDown(FastInputAttr.CfgKey, "CYY",
                FastInputAttr.EnsName, this.EnsName,
                FastInputAttr.AttrKey, this.AttrKey,
              FastInputAttr.FK_Emp, WebUser.No, "Idx");
            return "移动成功.";
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

            int val= this.Retrieve(FastInputAttr.CfgKey, "CYY",
                FastInputAttr.FK_Emp, BP.Web.WebUser.No);

            if (val==0)
            {
                FastInput en = new FastInput();
                en.setMyPK(DBAccess.GenerGUID());
                en.Vals = "同意";
                en.FK_Emp = WebUser.No;
                en.Insert();

                en = new FastInput();
                en.setMyPK(DBAccess.GenerGUID());
                en.Vals = "不同意";
                en.FK_Emp = WebUser.No;
                en.Insert();

                en = new FastInput();
                en.setMyPK(DBAccess.GenerGUID());
                en.Vals = "同意，请领导批示";
                en.FK_Emp = WebUser.No;
                en.Insert();

                en = new FastInput();
                en.setMyPK(DBAccess.GenerGUID());
                en.Vals = "同意办理";
                en.FK_Emp = WebUser.No;
                en.Insert();

                en = new FastInput();
                en.setMyPK(DBAccess.GenerGUID());
                en.Vals = "情况属实报领导批准";
                en.FK_Emp = WebUser.No;
                en.Insert();

                val = this.Retrieve(FastInputAttr.CfgKey, "CYY",
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
