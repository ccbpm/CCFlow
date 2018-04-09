using System;
using System.Collections;
using BP.DA;

namespace BP.En
{
	/// <summary>
	/// 属性列表
	/// </summary>
    public class EntityMultiTreeAttr
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
        /// <summary>
        /// 树编号
        /// </summary>
        public const string TreeNo = "TreeNo";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 是否是目录
        /// </summary>
        public const string IsDir = "IsDir";
    }
	/// <summary>
	/// 多个树实体
	/// </summary>
    abstract public class EntityMultiTree : Entity
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
                return this.GetValStringByKey(EntityMultiTreeAttr.No);
            }
            set
            {
                this.SetValByKey(EntityMultiTreeAttr.No, value);
            }
        }
        /// <summary>
        /// 树结构编号
        /// </summary>
        public string TreeNo
        {
            get
            {
                return this.GetValStringByKey(EntityMultiTreeAttr.TreeNo);
            }
            set
            {
                this.SetValByKey(EntityMultiTreeAttr.TreeNo, value);
            }
        }
        /// <summary>
        /// 名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStringByKey(EntityMultiTreeAttr.Name);
            }
            set
            {
                this.SetValByKey(EntityMultiTreeAttr.Name, value);
            }
        }
        /// <summary>
        /// 父节点编号
        /// </summary>
        public string ParentNo
        {
            get
            {
                return this.GetValStringByKey(EntityMultiTreeAttr.ParentNo);
            }
            set
            {
                this.SetValByKey(EntityMultiTreeAttr.ParentNo, value);
            }
        }
        /// <summary>
        /// 是否是目录
        /// </summary>
        public bool IsDir
        {
            get
            {
                return this.GetValBooleanByKey(EntityMultiTreeAttr.IsDir);
            }
            set
            {
                this.SetValByKey(EntityMultiTreeAttr.IsDir, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(EntityMultiTreeAttr.Idx);
            }
            set
            {
                this.SetValByKey(EntityMultiTreeAttr.Idx, value);
            }
        }
        /// <summary>
        /// 级别
        /// </summary>
        public int Grade
        {
            get
            {
                return this.TreeNo.Length / 2;
            }
        }
        #endregion

        #region 需要重写.
        /// <summary>
        /// 关联的主题字段.
        /// 比如在独立表单树中, 就是流程编号字段，需要实体类重写.
        /// </summary>
        public abstract string RefObjField
        {
            get;
        }
        #endregion 需要重写.

        #region 构造函数
        /// <summary>
        /// 主键
        /// </summary>
        public override string PK
        {
            get
            {
                return EntityMultiTreeAttr.No;
            }
        }
        /// <summary>
        /// 树结构编号
        /// </summary>
        public EntityMultiTree()
        {
        }
        /// <summary>
        /// 树结构编号
        /// </summary>
        /// <param name="no">编号</param>
        public EntityMultiTree(string no)
        {
            if (DataType.IsNullOrEmpty(no))
                throw new Exception(this.EnDesc + "@对表[" + this.EnDesc + "]进行查询前必须指定编号。");

            this.No = no;
            if (this.Retrieve() == 0)
                throw new Exception("@没有" + this._enMap.PhysicsTable + ", No = " + this.No + "的记录。");
        }
        #endregion

        #region 业务逻辑处理
        /// <summary>
        /// 重新设置treeNo
        /// </summary>
        public void ResetTreeNo()
        {
        }
        /// <summary>
        /// 检查名称的问题.
        /// </summary>
        /// <returns></returns>
        protected override bool beforeInsert()
        {
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

        #region 可让子类调用的方法
        /// <summary>
        /// 新建同级节点
        /// </summary>
        /// <returns></returns>
        public EntityMultiTree DoCreateSameLevelNode()
        {
            EntityMultiTree en = this.CreateInstance() as EntityMultiTree;
            en.No = BP.DA.DBAccess.GenerOID(this.ToString()).ToString(); // en.GenerNewNoByKey(EntityMultiTreeAttr.No);
            en.Name = "新建节点" + en.No;
            en.ParentNo = this.ParentNo;
            en.IsDir = false;
            en.TreeNo = this.GenerNewNoByKey(EntityMultiTreeAttr.TreeNo, EntityMultiTreeAttr.ParentNo, this.ParentNo);

             //给实体类赋值.
            en.SetValByKey(this.RefObjField, this.GetValStringByKey(this.RefObjField) ); 

            en.Insert();
            return en;
        }
        /// <summary>
        /// 新建子节点
        /// </summary>
        /// <returns></returns>
        public EntityMultiTree DoCreateSubNode()
        {
            EntityMultiTree en = this.CreateInstance() as EntityMultiTree;
            en.No = BP.DA.DBAccess.GenerOID(this.ToString()).ToString(); // en.GenerNewNoByKey(EntityMultiTreeAttr.No);
            en.Name = "新建节点" + en.No;
            en.ParentNo = this.No;
            en.IsDir = false;

            //给实体类赋值.
            en.SetValByKey(this.RefObjField, this.GetValStringByKey(this.RefObjField) ); 

            en.TreeNo = this.GenerNewNoByKey(EntityMultiTreeAttr.TreeNo, EntityMultiTreeAttr.ParentNo, this.No);
            if (en.TreeNo.Substring(en.TreeNo.Length - 2) == "01")
                en.TreeNo = this.TreeNo + "01";
            en.Insert();

            // 设置此节点是目录
            if (this.IsDir == false)
            {
                this.IsDir = true;
                this.Update(EntityMultiTreeAttr.IsDir, true);
            }
            return en;
        }
        /// <summary>
        /// 上移
        /// </summary>
        /// <returns></returns>
        public string DoUp()
        {
            this.DoOrderUp(EntityMultiTreeAttr.ParentNo, this.ParentNo,
                this.RefObjField, this.GetValStringByKey(RefObjField), EntityMultiTreeAttr.Idx);
            return null;
        }
        /// <summary>
        /// 下移
        /// </summary>
        /// <returns></returns>
        public string DoDown()
        {
            this.DoOrderDown(EntityMultiTreeAttr.ParentNo, this.ParentNo,
                this.RefObjField, this.GetValStringByKey(RefObjField), EntityMultiTreeAttr.Idx);
            return null;
        }
        #endregion
    }
	/// <summary>
    /// 多个树实体s
	/// </summary>
    abstract public class EntitiesMultiTree : Entities
    {
        /// <summary>
        /// 查询他的子节点
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public int RetrieveHisChinren(EntityMultiTree en)
        {
            int i=this.Retrieve(EntityMultiTreeAttr.ParentNo, en.No);
            this.AddEntity(en);
            return i + 1;
        }
        /// <summary>
        /// 获取它的子节点
        /// </summary>
        /// <param name="en"></param>
        /// <returns></returns>
        public EntitiesTree GenerHisChinren(EntityMultiTree en)
        {
            EntitiesTree ens = this.CreateInstance() as EntitiesTree;
            foreach (EntityMultiTree item in ens)
            {
                if (en.ParentNo == en.No)
                    ens.AddEntity(item);
            }
            return ens;
        }
        /// <summary>
        /// 根据位置取得数据
        /// </summary>
        public new EntityMultiTree this[int index]
        {
            get
            {
                return (EntityMultiTree)this.InnerList[index];
            }
        }
        /// <summary>
        /// 构造
        /// </summary>
        public EntitiesMultiTree()
        {
        }
    }
}
