using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.Web;
using BP.En;
using BP.Port;
using BP.Sys;
using BP.CCBill.Template;

namespace BP.CCFast
{
    /// <summary>
    /// 记事本 属性
    /// </summary>
    public class NotepadAttr : EntityMyPKAttr
    {
        /// <summary>
        /// 名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 功能ID
        /// </summary>
        public const string Docs = "Docs";
        /// <summary>
        /// 组织编号
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string Rec = "Rec";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 年月
        /// </summary>
        public const string NianYue = "NianYue";
        public const string IsStar = "IsStar";

    }
    /// <summary>
    /// 记事本
    /// </summary>
    public class Notepad : EntityMyPK
    {
        #region 基本属性
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get { return this.GetValStrByKey(NotepadAttr.OrgNo); }
            set { this.SetValByKey(NotepadAttr.OrgNo, value); }
        }
        /// <summary>
        /// 记录人
        /// </summary>
        public string Rec
        {
            get { return this.GetValStrByKey(NotepadAttr.Rec); }
            set { this.SetValByKey(NotepadAttr.Rec, value); }
        }
        /// <summary>
        /// 记录日期
        /// </summary>
        public string RDT
        {
            get { return this.GetValStrByKey(NotepadAttr.RDT); }
            set { this.SetValByKey(NotepadAttr.RDT, value); }
        }
        /// <summary>
        /// 年月
        /// </summary>
        public string NianYue
        {
            get { return this.GetValStrByKey(NotepadAttr.NianYue); }
            set { this.SetValByKey(NotepadAttr.NianYue, value); }
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
        /// 记事本
        /// </summary>
        public Notepad()
        {
        }
        public Notepad(string mypk)
        {
            this.setMyPK(mypk);
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

                Map map = new Map("OA_Notepad", "记事本");

                map.AddMyPK();
                map.AddTBString(NotepadAttr.Name, null, "标题", true, false, 0, 300, 10, true);

                map.AddTBStringDoc(NotepadAttr.Docs, null, "内容", true, false);

                map.AddTBString(NotepadAttr.OrgNo, null, "OrgNo", false, false, 0, 100, 10);
                map.AddTBString(NotepadAttr.Rec, null, "记录人", false, false, 0, 100, 10, true);
                map.AddTBDateTime(NotepadAttr.RDT, null, "记录时间", false, false);
                map.AddTBString(NotepadAttr.NianYue, null, "NianYue", false, false, 0, 10, 10);

                map.AddTBInt(NotepadAttr.IsStar, 0, "是否标星", false, false);



                //RefMethod rm = new RefMethod();
                //rm.Title = "方法参数"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoParas";
                //rm.Visable = true;
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Target = "_blank";
                ////rm.GroupName = "开发接口";
                ////  map.AddRefMethod(rm);

                //rm = new RefMethod();
                //rm.Title = "方法内容"; // "设计表单";
                //rm.ClassMethodName = this.ToString() + ".DoDocs";
                //rm.Visable = true;
                //rm.RefMethodType = RefMethodType.RightFrameOpen;
                //rm.Target = "_blank";
                ////rm.GroupName = "开发接口";
                //map.AddRefMethod(rm);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 执行方法.
        protected override bool beforeInsert()
        {
            this.setMyPK(DBAccess.GenerGUID());
            this.Rec = WebUser.No;
            this.RDT = DataType.CurrentDateTime;

            this.NianYue = DataType.CurrentYearMonth;


            return base.beforeInsert();
        }
        #endregion 执行方法.
    }
    /// <summary>
    /// 记事本 s
    /// </summary>
    public class Notepads : EntitiesMyPK
    {
        /// <summary>
        /// 查询前30个数据.
        /// </summary>
        /// <returns></returns>
        public string RetrieveTop30()
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NotepadAttr.Rec, WebUser.No);
            //qo.addAnd();
            //qo.AddWhere(NotepadAttr.IsStar, 0);
            qo.Top = 30;
            qo.addOrderBy("RDT");
            qo.DoQuery();

            return this.ToJson();
        }
        public string RetrieveTop30Stars()
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(NotepadAttr.Rec, WebUser.No);
            qo.addAnd();
            qo.AddWhere(NotepadAttr.IsStar, 1);
            qo.Top = 30;
            qo.addOrderBy("RDT");
            qo.DoQuery();

            return this.ToJson();
        }
        /// <summary>
        /// 记事本
        /// </summary>
        public Notepads() { }
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Notepad();
            }
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Notepad> ToJavaList()
        {
            return (System.Collections.Generic.IList<Notepad>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Notepad> Tolist()
        {
            System.Collections.Generic.List<Notepad> list = new System.Collections.Generic.List<Notepad>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Notepad)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
