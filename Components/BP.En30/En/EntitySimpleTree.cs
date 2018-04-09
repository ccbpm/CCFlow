using System;
using System.Collections;
using BP.DA;

namespace BP.En
{
	/// <summary>
	/// 属性列表
	/// </summary>
    public class EntitySimpleTreeAttr
    {
        /// <summary>
        /// 树结构编号
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 父节编号
        /// </summary>
        public const string ParentNo = "ParentNo";
    }
	/// <summary>
	/// 树实体
	/// </summary>
    abstract public class EntitySimpleTree : Entity
    {
        #region 属性
        public bool IsRoot
        {
            get
            {
                if (this.ParentNo == "-1" || this.ParentNo == "0")
                    return true;

                if (this.ParentNo == this.No)
                    return true;

                return false;
            }
        }
        /// <summary>
        /// 唯一标示
        /// </summary>
        public string No
        {
            get
            {
                return this.GetValStringByKey(EntitySimpleTreeAttr.No);
            }
            set
            {
                this.SetValByKey(EntitySimpleTreeAttr.No, value);
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey(EntitySimpleTreeAttr.Name);
            }
            set
            {
                this.SetValByKey(EntitySimpleTreeAttr.Name, value);
            }
        }
        /// <summary>
        /// 父节点编号
        /// </summary>
        public string ParentNo
        {
            get
            {
                return this.GetValStringByKey(EntitySimpleTreeAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(EntitySimpleTreeAttr.ParentNo, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 主键
        /// </summary>
        public override string PK
        {
            get
            {
                return EntitySimpleTreeAttr.No;
            }
        }
        /// <summary>
        /// 树结构编号
        /// </summary>
        public EntitySimpleTree()
        {
        }
        /// <summary>
        /// 树结构编号
        /// </summary>
        /// <param name="no">编号</param>
        public EntitySimpleTree(string no)
        {
            if (DataType.IsNullOrEmpty(no))
                throw new Exception(this.EnDesc + "@对表[" + this.EnDesc + "]进行查询前必须指定编号。");

            this.No = no;
            if (this.Retrieve() == 0)
                throw new Exception("@没有" + this._enMap.PhysicsTable + ", No = " + this.No + "的记录。");
        }
        #endregion

    }
	/// <summary>
    /// 树实体s
	/// </summary>
    abstract public class EntitiesSimpleTree : Entities
    {
        /// <summary>
        /// 查询他的子节点
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public int RetrieveHisChinren(EntitySimpleTree en)
        {
            int i=this.Retrieve(EntitySimpleTreeAttr.ParentNo, en.No);
            this.AddEntity(en);
            return i + 1;
        }

        /// <summary>
        /// 获取它的子节点
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public EntitiesTree GenerHisChinren(EntitySimpleTree en)
        {
            EntitiesTree ens = this.CreateInstance() as EntitiesTree;
            foreach (EntitySimpleTree item in ens)
            {
                if (en.ParentNo == en.No)
                    ens.AddEntity(item);
            }
            return ens;
        }
        /// <summary>
        /// 根据位置取得数据
        /// </summary>
        public new EntitySimpleTree this[int index]
        {
            get
            {
                return (EntitySimpleTree)this.InnerList[index];
            }
        }
        /// <summary>
        /// 构造
        /// </summary>
        public EntitiesSimpleTree()
        {
        }
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            return base.RetrieveAll("No");
        }
       
    }
}
