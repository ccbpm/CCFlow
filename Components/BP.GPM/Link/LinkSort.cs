using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.GPM
{
    /// <summary>
    /// 超链接
    /// </summary>
    public class LinkSortAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 顺序
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 链接
        /// </summary>
        public const string Url = "Url";
        /// <summary>
        /// 打开方式
        /// </summary>
        public const string OpenWay = "OpenWay";
    }
    /// <summary>
    /// 超链接
    /// </summary>
    public class LinkSort : EntityNoName
    {
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }

        #region 属性
        /// <summary>
        /// 链接
        /// </summary>
        public string Url
        {
            get
            {
                return this.GetValStrByKey(LinkSortAttr.Url);
            }
            set
            {
                this.SetValByKey(LinkSortAttr.Url, value);
            }
        }
        /// <summary>
        /// 打开方式
        /// </summary>
        public int OpenWay
        {
            get
            {
                return this.GetValIntByKey(LinkSortAttr.OpenWay);
            }
            set
            {
                this.SetValByKey(LinkSortAttr.OpenWay, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(LinkSortAttr.Idx);
            }
            set
            {
                this.SetValByKey(LinkSortAttr.Idx, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 超链接
        /// </summary>
        public LinkSort()
        {
        }
        /// <summary>
        /// 超链接
        /// </summary>
        /// <param name="mypk"></param>
        public LinkSort(string no)
        {
            this.No = no;
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
                Map map = new Map("GPM_LinkSort");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "超链接";
                map.EnType = EnType.Sys;

                map.AddTBStringPK(LinkSortAttr.No, null, "编号", true, false, 2, 2, 2);
                map.AddTBString(LinkSortAttr.Name, null, "名称", true, false, 0, 3900, 20);
                map.AddTBString(LinkSortAttr.Url, null, "链接", true, false, 0, 3900, 20);

                map.AddDDLSysEnum(LinkSortAttr.OpenWay, 0, "打开方式", true, true,
                  LinkSortAttr.OpenWay, "@0=新窗口@1=本窗口@2=覆盖新窗口");

                map.AddTBInt(LinkSortAttr.Idx, 0, "显示顺序", false, true);

                map.AddSearchAttr(LinkSortAttr.OpenWay);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 超链接s
    /// </summary>
    public class LinkSorts : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 超链接s
        /// </summary>
        public LinkSorts()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new LinkSort();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<LinkSort> ToJavaList()
        {
            return (System.Collections.Generic.IList<LinkSort>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<LinkSort> Tolist()
        {
            System.Collections.Generic.List<LinkSort> list = new System.Collections.Generic.List<LinkSort>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((LinkSort)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
