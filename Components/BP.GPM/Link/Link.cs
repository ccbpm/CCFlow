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
    public class LinkAttr : EntityNoNameAttr
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
        /// <summary>
        /// 类别
        /// </summary>
        public const string FK_Sort = "FK_Sort";
    }
    /// <summary>
    /// 超链接
    /// </summary>
    public class Link : EntityNoName
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
                return this.GetValStrByKey(LinkAttr.Url);
            }
            set
            {
                this.SetValByKey(LinkAttr.Url, value);
            }
        }
        /// <summary>
        /// 类别
        /// </summary>
        public string FK_Sort
        {
            get
            {
                return this.GetValStrByKey(LinkAttr.FK_Sort);
            }
            set
            {
                this.SetValByKey(LinkAttr.FK_Sort, value);
            }
        }
        /// <summary>
        /// 打开方式
        /// </summary>
        public int OpenWay
        {
            get
            {
                return this.GetValIntByKey(LinkAttr.OpenWay);
            }
            set
            {
                this.SetValByKey(LinkAttr.OpenWay, value);
            }
        }
        /// <summary>
        /// 顺序号
        /// </summary>
        public int Idx
        {
            get
            {
                return this.GetValIntByKey(LinkAttr.Idx);
            }
            set
            {
                this.SetValByKey(LinkAttr.Idx, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 超链接
        /// </summary>
        public Link()
        {
        }
        /// <summary>
        /// 超链接
        /// </summary>
        /// <param name="mypk"></param>
        public Link(string no)
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
                Map map = new Map("GPM_Link");
                map.DepositaryOfEntity = Depositary.None;
                map.DepositaryOfMap = Depositary.Application;
                map.EnDesc = "超链接";
                map.EnType = EnType.Sys;

                map.AddTBStringPK(LinkAttr.No, null, "编号", true, false, 2, 2, 2);
                map.AddTBString(LinkAttr.Name, null, "名称", true, false, 0, 50, 20);
                map.AddTBString(LinkAttr.Url, null, "链接", true, false, 0, 200, 20);

                map.AddDDLSysEnum(LinkAttr.OpenWay, 0, "打开方式", true, true,
                  LinkAttr.OpenWay, "@0=新窗口@1=本窗口@2=覆盖新窗口");

                map.AddDDLEntities(LinkAttr.FK_Sort, null, "类别", new LinkSorts(),true);
                map.AddTBInt(LinkAttr.Idx, 0, "显示顺序", false, true);

                map.AddSearchAttr(LinkAttr.FK_Sort);


                RefMethod rm = new RefMethod();
                rm.Title = "上移";
                rm.ClassMethodName = this.ToString() + ".DoUp";
                map.AddRefMethod(rm);

                rm = new RefMethod();
                rm.Title = "下移";
                rm.ClassMethodName = this.ToString() + ".DoDown";
                map.AddRefMethod(rm);

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
            this.DoOrderUp(LinkAttr.FK_Sort, this.FK_Sort, LinkAttr.Idx);
            return "执行上移成功";
        }
        /// <summary>
        /// 执行下移
        /// </summary>
        /// <returns></returns>
        public string DoDown()
        {
            this.DoOrderDown(LinkAttr.FK_Sort, this.FK_Sort, LinkAttr.Idx);
            return "执行下移成功";
        }
    }
    /// <summary>
    /// 超链接s
    /// </summary>
    public class Links : EntitiesNoName
    {
        #region 构造
        /// <summary>
        /// 超链接s
        /// </summary>
        public Links()
        {
        }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Link();
            }
        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Link> ToJavaList()
        {
            return (System.Collections.Generic.IList<Link>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Link> Tolist()
        {
            System.Collections.Generic.List<Link> list = new System.Collections.Generic.List<Link>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Link)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
