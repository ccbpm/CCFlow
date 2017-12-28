using System; 
using System.Collections;
using BP.DA; 
using BP.Web.Controls;

namespace BP.En
{
	/// <summary>
	/// EnDtl 的摘要说明。
	/// </summary>
	public class EnDtl
	{
		/// <summary>
		/// 明细
		/// </summary>
		public EnDtl()
		{			  
		}
		/// <summary>
		/// 明细
		/// </summary>
		/// <param name="className">类名称</param>
		public EnDtl(string className)
		{
			this.Ens=ClassFactory.GetEns(className);
		}
		/// <summary>
		/// 类名称
		/// </summary>
		public string EnsName
		{
			get
			{
				return this.Ens.ToString();
			}
		}
		/// <summary>
		/// 明细
		/// </summary>
		private Entities _Ens=null;
		/// <summary>
		/// 获取或设置 他的集合
		/// </summary>
		public Entities Ens
		{
			get
			{
				return _Ens;
			}
			set			
			{
				_Ens=value;
			}
		}
		/// <summary>
		/// 他关连的key
		/// </summary>
		private string _refKey=null;
		/// <summary>
		/// 他关连的 key
		/// </summary>
		public string RefKey
		{
			get
			{
				return _refKey;
			}
			set
			{
				this._refKey =value; 
			}
		}
		/// <summary>
		/// 描述
		/// </summary>
		private string _Desc=null;
		/// <summary>
		/// 描述
		/// </summary>
		public string Desc
		{
			get
			{
				if (this._Desc==null)
					this._Desc=this.Ens.GetNewEntity.EnDesc;
				return _Desc;
			}
			set
			{
				_Desc=value;
			}
		}
        /// <summary>
        /// 显示到分组
        /// </summary>
        private string _groupName = null;
        /// <summary>
        /// 显示到分组
        /// </summary>
        public string GroupName
        {
            get
            {
                return _groupName;
            }
            set
            {
                this._groupName = value;
            }
        }
	}
	/// <summary>
	/// 
	/// </summary>
	[Serializable]
	public class EnDtls: CollectionBase
	{
		/// <summary>
		/// 是不是包含className
		/// </summary>
		/// <param name="className"></param>
		/// <returns></returns>
		public bool IsContainKey(string className)		
		{
			foreach(EnDtl ed in this)
			{
				if (ed.EnsName==className)
					return true;
			}
			return false;
		}	 
		/// <summary>
		/// 加入
		/// </summary>
		/// <param name="attr">attr</param>
		public void Add(EnDtl en)
		{
			 if (this.IsExits(en))
				 return ;
			this.InnerList.Add(en);
		}
		/// <summary>
		/// 是不是存在集合里面
		/// </summary>
		/// <param name="en">要检查的EnDtl</param>
		/// <returns>true/false</returns>
		public bool IsExits(EnDtl en)
		{
			foreach (EnDtl dtl in this )
			{
				if (dtl.Ens== en.Ens )
				{
					return true;
				}
			}
			return false;
		}
	 
		/// <summary>
		/// 通过一个key 得到它的属性值。
		/// </summary>
		/// <param name="key">key</param>
		/// <returns>EnDtl</returns>
		public EnDtl GetEnDtlByKey(string key)
		{		
			foreach (EnDtl dtl in this )
			{
				if (dtl.RefKey.Equals(key))
				{
					return dtl;
				}
			}
			throw new Exception("@没有找到 key=["+key+"]的属性，请检查map文件。");
		}
		/// <summary>
		/// 根据索引访问集合内的元素Attr。
		/// </summary>
		public EnDtl this[int index]
		{			
			get
			{	
				return (EnDtl)this.InnerList[index];
			}
		}
		/// <summary>
		/// className
		/// </summary>
		/// <param name="className">类名称</param>
		/// <returns></returns>
		public EnDtl GetEnDtlByEnsName(string className)
		{
			foreach( EnDtl en in this)
			{
				if (en.EnsName==className)
					return en;
			}
			throw new Exception("@没有找到他的明细:"+className);
		}
		
	}
}
