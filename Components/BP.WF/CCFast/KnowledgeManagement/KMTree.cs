using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;
using BP.CCBill.Template;

namespace BP.CCFast.KnowledgeManagement
{
    /// <summary>
    /// 知识树 属性
    /// </summary>
    public class KMTreeAttr : EntityTreeAttr
    {
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// <summary>
        /// 文件类型
        /// </summary>
        public const string FileType = "FileType";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 记录人名称
        /// </summary>
        public const string RecName = "RecName";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 知识编号
        /// </summary>
        public const string KnowledgeNo = "KnowledgeNo";
        /// <summary>
        /// 是否删除.
        /// </summary>
        public const string IsDel = "IsDel";
    }
    /// <summary>
    /// 知识树
    /// </summary>
    public class KMTree : EntityTree
    {
        #region 基本属性
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get { return this.GetValStrByKey(KMTreeAttr.OrgNo); }
            set { this.SetValByKey(KMTreeAttr.OrgNo, value); }
        }
        public string Rec
        {
            get { return this.GetValStrByKey(KMTreeAttr.Rec); }
            set { this.SetValByKey(KMTreeAttr.Rec, value); }
        }
        public string RecName
        {
            get { return this.GetValStrByKey(KMTreeAttr.RecName); }
            set { this.SetValByKey(KMTreeAttr.RecName, value); }
        }
        public string RDT
        {
            get { return this.GetValStrByKey(KMTreeAttr.RDT); }
            set { this.SetValByKey(KMTreeAttr.RDT, value); }
        }
        public string KnowledgeNo
        {
            get { return this.GetValStrByKey(KMTreeAttr.KnowledgeNo); }
            set { this.SetValByKey(KMTreeAttr.KnowledgeNo, value); }
        }
        
        #endregion

        #region 构造方法
        /// <summary>
        /// 权限控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                if (WebUser.IsAdmin)
                {
                    uac.IsUpdate = true;
                    return uac;
                }
                return base.HisUAC;
            }
        }
        /// <summary>
        /// 知识树
        /// </summary>
        public KMTree()
        {
        }
        public KMTree(string mypk)
        {
            this.Retrieve();
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

                Map map = new Map("OA_KMTree", "知识树");

                map.AddTBStringPK(KMTreeAttr.No, null, "编号", false, false, 0, 50, 10);
                map.AddTBString(KMTreeAttr.Name, null, "名称", false, false, 0, 500, 10);
                map.AddTBString(KMTreeAttr.ParentNo, null, "父节点编号", false, false, 0, 50, 10);


                map.AddTBString(KMTreeAttr.KnowledgeNo, null, "知识编号", false, false, 0, 50, 10);              
                map.AddTBInt(KMTreeAttr.FileType, 1, "文件类型", false, false);
                map.AddTBInt(KMTreeAttr.Idx, 0, "Idx", false, false);


                map.AddTBString(KMTreeAttr.OrgNo, null, "组织编号", false, false, 0, 100, 10);
                map.AddTBString(KMTreeAttr.Rec, null, "记录人", false, false, 0, 100, 10);
                map.AddTBString(KMTreeAttr.RecName, null, "记录人名称", false, false, 0, 100, 10, true);
                map.AddTBDateTime(KMTreeAttr.RDT, null, "记录时间", false, false);
                map.AddTBInt(KMTreeAttr.IsDel, 0, "IsDel", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行方法.
        protected override bool beforeInsert()
        {
            this.No = DBAccess.GenerGUID();
            this.Rec = WebUser.No;
            this.RecName = WebUser.Name;
            if (SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                this.OrgNo = WebUser.OrgNo;

            return base.beforeInsert();
        }
        protected override bool beforeUpdate()
        {
            return base.beforeUpdate();
        }
        #endregion 执行方法.
    }
    /// <summary>
    /// 知识树 s
    /// </summary>
    public class KMTrees : EntitiesTree
    {

        #region 查询.
        #endregion 重写.

        #region 重写.
        /// <summary>
        /// 知识树
        /// </summary>
        public KMTrees() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new KMTree();
            }
        }
        #endregion 重写.


        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<KMTree> ToJavaList()
        {
            return (System.Collections.Generic.IList<KMTree>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<KMTree> Tolist()
        {
            System.Collections.Generic.List<KMTree> list = new System.Collections.Generic.List<KMTree>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((KMTree)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
