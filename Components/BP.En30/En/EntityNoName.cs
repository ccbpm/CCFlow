using System;
using System.Collections;
using BP.DA;

namespace BP.En
{
	/// <summary>
	/// 属性列表
	/// </summary>
	public class EntityNoNameAttr : EntityNoAttr
	{	
		/// <summary>
		/// 名称
		/// </summary>
		public const string Name="Name";
        /// <summary>
        /// 名称简称
        /// </summary>
        public const string NameOfS = "NameOfS";

	}
    public class EntityNoNameMyFileAttr : EntityNoMyFileAttr
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
    }
	/// <summary>
	/// 具有编号名称的基类实体
	/// </summary>
    abstract public class EntityNoName : EntityNo
    {
        #region 属性
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey(EntityNoNameAttr.Name);
            }
            set
            {
                this.SetValByKey(EntityNoNameAttr.Name, value);
            }
        }
        //public string NameE
        //{
        //    get
        //    {
        //        return this.GetValStringByKey("NameE");
        //    }
        //    set
        //    {
        //        this.SetValByKey("NameE", value);
        //    }
        //}
        #endregion

        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        public EntityNoName()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="_No"></param>
        protected EntityNoName(string _No) : base(_No) { }
        #endregion

        #region 业务逻辑处理
        /// <summary>
        /// 检查名称的问题.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
            if (this.No.Trim().Length == 0)
            {
                if (this.EnMap.IsAutoGenerNo)
                    this.No = this.GenerNewNo;
                else
                    throw new Exception("@没有给[" + this.EnDesc+" " + this.ToString() + " , " + this.Name + "]设置主键,能执行插入.");
            }

            if (this.EnMap.IsAllowRepeatName == false)
            {
                if (this.PKCount == 1)
                {
                    if (this.ExitsValueNum("Name", this.Name) >= 1)
                        throw new Exception("@插入失败[" + this.EnMap.EnDesc + "] 编号[" + this.No + "]名称[" + Name + "]重复.");
                }
            }
            return base.beforeInsert();
        }
        protected override bool beforeUpdate()
        {
            if (this.EnMap.IsAllowRepeatName == false)
            {
                if (this.PKCount == 1)
                {
                    if (this.ExitsValueNum("Name", this.Name) >= 2)
                        throw new Exception("@更新失败[" + this.EnMap.EnDesc + "] 编号[" + this.No + "]名称[" + Name + "]重复.");
                }
            }
            return base.beforeUpdate();
        }
        #endregion

       
    }
	/// <summary>
    /// 具有编号名称的基类实体s
	/// </summary>
    abstract public class EntitiesNoName : EntitiesNo
    {
        /// <summary>
        /// 将对象添加到集合尾处，如果对象已经存在，则不添加.
        /// </summary>
        /// <param name="entity">要添加的对象</param>
        /// <returns>返回添加到的地方</returns>
        public virtual int AddEntity(Entity entity)
        {
            foreach (Entity en in this)
            {
                if (en.GetValStrByKey("No") == entity.GetValStrByKey("No"))
                    return 0;
            }
            return this.InnerList.Add(entity);
        }
        /// <summary>
        /// 将对象添加到集合尾处，如果对象已经存在，则不添加
        /// </summary>
        /// <param name="entity">要添加的对象</param>
        /// <returns>返回添加到的地方</returns>
        public virtual void Insert(int index, EntityNoName entity)
        {
            foreach (EntityNoName en in this)
            {
                if (en.No == entity.No)
                    return;
            }

            this.InnerList.Insert(index, entity);
        }
        /// <summary>
        /// 根据位置取得数据
        /// </summary>
        public new EntityNoName this[int index]
        {
            get
            {
                return (EntityNoName)this.InnerList[index];
            }
        }
        /// <summary>
        /// 构造
        /// </summary>
        public EntitiesNoName()
        {
        }
        /// <summary> 
        /// 按照名称模糊查询
        /// </summary>
        /// <param name="likeName">likeName</param>
        /// <returns>返回查询的Num</returns>
        public int RetrieveByLikeName(string likeName)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere("Name", "like", " %" + likeName + "% ");
            return qo.DoQuery();
        }
        public override int RetrieveAll()
        {
            return base.RetrieveAll("No");
        }
    }
}
