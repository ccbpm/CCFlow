using System;
using System.Collections;
using BP.DA;

namespace BP.En
{
    /// <summary>
    /// 属性列表
    /// </summary>
    public class EntityNoNameAttr
    {
        #region 基本属性.
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 编号
        /// </summary>
        public const string No = "No";
        /// <summary>
        /// 名称简称
        /// </summary>
        public const string NameOfS = "NameOfS";
        #endregion 基本属性.

        #region 附件属性.
        public const string MyFilePath = "MyFilePath";
        public const string MyFileName = "MyFileName";
        public const string MyFileExt = "MyFileExt";
        public const string MyFileH = "MyFileH";
        public const string MyFileW = "MyFileW";
        public const string MyFileSize = "MyFileSize";
        public const string WebPath = "WebPath";
        public const string MyFileNum = "MyFileNum";
        #endregion 附件属性.

    }
    /// <summary>
    /// 具有编号名称的基类实体
    /// </summary>
    abstract public class EntityNoName : Entity
    {
        #region 提供的属性
        public override string PK
        {
            get
            {
                return "No";
            }
        }
        /// <summary>
        /// 编号
        /// </summary>
        public string No
        {
            get
            {
                return this.GetValStringByKey(EntityNoNameAttr.No);
            }
            set
            {
                this.SetValByKey(EntityNoNameAttr.No, value);
            }
        }
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
        public void setName(string val)
        {
            this.SetValByKey(EntityNoNameAttr.Name, val);
        }
        /// <summary>
        /// 生成一个编号
        /// </summary>
        public string GenerNewNo
        {
            get
            {
                return this.GenerNewNoByKey("No");
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 
        /// </summary>
        public EntityNoName()
        {
        }

        /// <summary>
        /// 通过编号得到实体。
        /// </summary>
        /// <param name="_no">编号</param>
        public EntityNoName(string _no)
        {
            if (_no == null || _no == "")
                throw new Exception(this.EnDesc + "@对表[" + this.EnDesc + "]进行查询前必须指定编号。");

            this.No = _no;
            if (this.Retrieve() == 0)
            {
                throw new Exception("@没有" + this._enMap.PhysicsTable + ", No = " + No + "的记录。");
            }
        }
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
                    //throw new Exception("@没有给[" + this.EnDesc + "]ClassID=[" + this.ClassID + "]Name=[" + this.Name + "]设置编号,不能执行插入.");
                    throw new Exception("没有给当前操作设置编号和名称，保存失败。");
            }
            //if (this.EnMap.IsAllowRepeatName == false)
            //{
            //    if (this.PKCount == 1)
            //    {
            //        if (this.ExitsValueNum("Name", this.Name) >= 1)
            //            throw new Exception("@插入失败[" + this.EnMap.EnDesc + "] 编号[" + this.No + "]名称[" + Name + "]重复.");
            //    }
            //}
            return base.beforeInsert();
        }
        protected override bool beforeUpdate()
        {
            //if (this.EnMap.IsAllowRepeatName == false)
            //{
            //    if (this.PKCount == 1)
            //    {
            //        if (this.ExitsValueNum("Name", this.Name) >= 2)
            //            throw new Exception("@更新失败[" + this.EnMap.EnDesc + "] 编号[" + this.No + "]名称[" + Name + "]重复.");
            //    }
            //}
            return base.beforeUpdate();
        }
        public override int Save()
        {
            /*如果包含编号。 */
            if (this.IsExits)
            {
                return this.Update();
            }
            else
            {
                if (this.EnMap.IsAutoGenerNo
                    && this.EnMap.GetAttrByKey("No").UIIsReadonly)
                    this.No = this.GenerNewNo;

                this.Insert();
                return 0;
            }
        }
        #endregion
    }
    /// <summary>
    /// 具有编号名称的基类实体s
    /// </summary>
    abstract public class EntitiesNoName : Entities
    {
        #region 查询.
        /// <summary>
        /// 查询全部
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAllFromDBSource()
        {
            QueryObject qo = new QueryObject(this);
            qo.addOrderBy("No");
            return qo.DoQuery();
        }
        public override int RetrieveAll()
        {
            return base.RetrieveAll("No");
        }
        #endregion 查询.

        #region 构造高级方法.
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
        #endregion 构造高级方法.

    }
}
