using System;

namespace BP.En
{
	/// <summary>
	/// 多对多的集合。
	/// </summary>
	abstract public class EntityMM:Entity
	{
		/// <summary>
		/// 多对多的集合
		/// </summary>
		public EntityMM()
		{
		}
	}
	/// <summary>
	/// 多对多的集合
	/// </summary>
	abstract public class EntitiesMM : Entities
	{
		/// <summary>
		/// 多对多的集合
		/// </summary>
		protected EntitiesMM() 
		{
		}
		/// <summary>
		/// 提供通过一个实体的 val 查询另外的实体集合。
		/// </summary>
		/// <param name="attr">属性</param>
		/// <param name="val">植</param>
		/// <param name="refEns">关联的集合</param>
		/// <returns>关联的集合</returns>
		protected Entities throwOneKeyValGetRefEntities(string attr , int val, Entities refEns )
		{																									
			QueryObject qo = new QueryObject(refEns);
			qo.AddWhere(attr, val);
			return refEns;
		}
		/// <summary>
		/// 提供通过一个实体的 val 查询另外的实体集合。
		/// </summary>
		/// <param name="attr">属性</param>
		/// <param name="val">植</param>
		/// <param name="refEns">关联的集合</param>
		/// <returns>关联的集合</returns>
		protected Entities throwOneKeyValGetRefEntities(string attr, string val, Entities  refEns) 	 
		{
			QueryObject qo = new QueryObject(refEns);
			qo.AddWhere(attr, val);
			return refEns;
		}
	}
}
