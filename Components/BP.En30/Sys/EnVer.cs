using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.Web;
using Newtonsoft.Json;

namespace BP.Sys
{
    /// <summary>
    /// 实体版本号属性
    /// </summary>
    public class EnVerAttr : EntityMyPKAttr
    {
        /// <summary>
        ///  实体类
        /// </summary>
        public const string FrmID = "FrmID";
        /// <summary>
        /// 关联主键
        /// </summary>
        public const string RefPK = "RefPK";
        /// <summary>
        ///  实体类名称
        /// </summary>
        public const string Name = "Name";
        /// <summary>
        /// 主键值
        /// </summary>
        public const string PKValue = "PKValue";
        /// <summary>
        /// 版本号
        /// </summary>
        public const string EnVer = "EnVer";
        /// <summary>
        /// 说明
        /// </summary>
        public const string MyNote = "MyNote";
        /// <summary>
        /// 记录人
        /// </summary>
        public const string RecNo = "RecNo";
        /// <summary>
        /// 记录人名字
        /// </summary>
        public const string RecName = "RecName";
        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";

        public const string EnPKValue = "EnPKValue";
        /// <summary>
        /// DBJSON
        /// </summary>
        public const string DBJSON = "DBJSON";
    }
    /// <summary>
    /// 实体版本号
    /// </summary>
    public class EnVer : EntityMyPK
    {
        #region 方法.
        /// <summary>
        /// 创建新版本
        /// </summary>
        /// <returns></returns>
        public static string NewVer(Entity myen)
        {
            string frmID = myen.ToString();
            string frmName = myen.EnDesc;
            string pkval = myen.PKVal.ToString();
            // 获得最大的版本号.
            int maxVer = DBAccess.RunSQLReturnValInt("SELECT MAX(EnVer) as Num FROM Sys_EnVer WHERE  FrmID='" + frmID + "' AND EnPKValue='" + pkval + "'", 0);
            //最大版本号>1,存在历史版本，获取上一个版本的数据
            string oldStr = "";
            EnVer oldVer = null;
            if (maxVer >0)
            {
                string mypk = frmID + "_" + pkval + "_" + (maxVer);
                oldVer = new EnVer();
                oldVer.MyPK = mypk;
                if (oldVer.RetrieveFromDBSources() != 0)
                {
                    oldStr = DBAccess.GetBigTextFromDB("Sys_EnVer", "MyPK", mypk, EnVerAttr.DBJSON);
                }
                if (oldStr.Equals(myen.ToJson()))
                    return "";
            }
            //创建实体.
            EnVer ev = new EnVer();
            
            ev.RecNo = WebUser.No;
            ev.RecName = WebUser.Name;
            ev.RDT = DataType.CurrentDateTime;
            ev.FrmID = frmID;
            ev.EnPKValue = pkval;
            ev.MyNote = "";
            ev.Name = frmName;
            
            
            ev.Ver = maxVer + 1; //设置版本号.
            ev.MyPK= ev.FrmID + "_"+ ev.EnPKValue + "_"+ ev.Ver;
            ev.Insert(); //执行插入.
            //存储数据JSON
            DBAccess.SaveBigTextToDB(myen.ToJson(), "Sys_EnVer", "MyPK", ev.MyPK, EnVerAttr.DBJSON);

            ////历史数据JSON字符串转HashTable
            //Hashtable ht = new Hashtable();
            //if(DataType.IsNullOrEmpty(oldStr)==false)
            //    ht = JsonConvert.DeserializeObject<Hashtable>(oldStr);

            //EnVerDtl dtl = new EnVerDtl();

            ////不需要存储的字段.
            //string sysFiels = ",AtPara,OID,WorkID,WFState,BillNo,Title,RDT,CDT,OrgNo,Starter,StarterName,BillState,FK_Dept,";

            //Attrs attrs = myen.EnMap.Attrs;
            //foreach (Attr attr in attrs)
            //{
            //    //如果是非数据控件.
            //    if ((int)attr.UIContralType >= 4)
            //        continue;

            //    if (sysFiels.Contains("," + attr.Key + ",") == true)
            //        continue;

            //    dtl.setMyPK(DBAccess.GenerGUID());
            //    dtl.RefPK = ev.MyPK; //设置关联主键.

            //    dtl.FrmID = ev.FrmID;
            //    dtl.EnPKValue = pkval; //设置为主键.

            //    dtl.AttrKey = attr.Key;
            //    dtl.AttrName = attr.Desc;

            //    //逻辑类型.
            //    dtl.LGType = (int)attr.MyFieldType; //  (int)attr.LGType;

            //    //设置值.
            //    dtl.EnPKValue = myen.PKVal.ToString();
            //    dtl.FrmID = myen.ClassID;

            //    //设置外键.
            //    dtl.BindKey = attr.UIBindKey;
            //    string val = myen.GetValByKey(attr.Key).ToString();
            //    if (ht[attr.Key] != null && ht[attr.Key].ToString().Equals(val) == true)
            //        continue;
            //    if (attr.UIContralType == UIContralType.TB || attr.UIContralType == UIContralType.CheckBok)
            //    {
            //        //设置值.
            //        dtl.MyVal = val;
            //        dtl.Insert();
            //        continue;
            //    }
            //    //枚举值
            //    if (attr.IsEnum)
            //    {
            //        AtPara apcfg = new AtPara(attr.UITag);
                       
            //        dtl.MyVal = "[" + val + "][" + apcfg.GetValStrByKey(val) + "]";
            //        dtl.Insert();
            //    }
            //    //设置值.
            //    if (attr.IsFK)
            //    {
            //        Entity en = attr.HisFKEns.GetEntityByKey(val);
            //        dtl.MyVal = "[" + val + "][" + en.GetValByKey("Name") + "]";
            //        dtl.Insert();
            //    }
               

            //}

            return "版本创建成功.";
        }
        #endregion 方法.

        #region 属性
        public string DBJSON
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.DBJSON);
            }
            set
            {
                this.SetValByKey(EnVerAttr.DBJSON, value);
            }
        }
        /// <summary>
        /// 实体类名称
        /// </summary>
        public string Name
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.Name);
            }
            set
            {
                this.SetValByKey(EnVerAttr.Name, value);
            }
        }
        public string EnPKValue
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.EnPKValue);
            }
            set
            {
                this.SetValByKey(EnVerAttr.EnPKValue, value);
            }
        }
        /// <summary>
        /// 版本号
        /// </summary>
        public int Ver
        {
            get
            {
                return this.GetValIntByKey(EnVerAttr.EnVer);
            }
            set
            {
                this.SetValByKey(EnVerAttr.EnVer, value);
            }
        }
        /// <summary>
        /// 修改人
        /// </summary>
        public string RecNo
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.RecNo);
            }
            set
            {
                this.SetValByKey(EnVerAttr.RecNo, value);
            }
        }
        public string RecName
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.RecName);
            }
            set
            {
                this.SetValByKey(EnVerAttr.RecName, value);
            }
        }
        public string MyNote
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.MyNote);
            }
            set
            {
                this.SetValByKey(EnVerAttr.MyNote, value);
            }
        }
        public string FrmID
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.FrmID);
            }
            set
            {
                this.SetValByKey(EnVerAttr.FrmID, value);
            }
        }
        /// <summary>
        /// 修改日期
        /// </summary>
        public string RDT
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.RDT);
            }
            set
            {
                this.SetValByKey(EnVerAttr.RDT, value);
            }
        }

        /// <summary>
        /// 主键值
        /// </summary>
        public string PKValue
        {
            get
            {
                return this.GetValStrByKey(EnVerAttr.PKValue);
            }
            set
            {
                this.SetValByKey(EnVerAttr.PKValue, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 实体版本号
        /// </summary>
        public EnVer() { }
        public EnVer(string mypk)
        {
            this.setMyPK(mypk);
            this.Retrieve();
        }

        #endregion

        #region 重写方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                uac.IsDelete = false;
                uac.IsInsert = false;
                uac.IsUpdate = false;
                return uac;
            }
        }
        /// <summary>
        /// Map
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Sys_EnVer", "实体版本号");
                map.EnDBUrl = new DBUrl(DBUrlType.AppCenterDSN); //连接到的那个数据库上. (默认的是: AppCenterDSN )

                map.AddMyPK();

                map.AddTBString(EnVerAttr.FrmID, null, "实体类", true, true, 1, 50, 20, true);
                map.AddTBString(EnVerAttr.Name, null, "实体名", true, true, 0, 100, 30, true);

                map.AddTBString(EnVerAttr.EnPKValue, null, "主键值", true, true, 0, 40, 100);
                map.AddTBInt(EnVerAttr.EnVer, 0, "版本号", true, true);

                map.AddTBString(EnVerAttr.RecNo, null, "修改人账号", true, true, 0, 100, 30);
                map.AddTBString(EnVerAttr.RecName, null, "修改人名称", true, true, 0, 100, 30);

                //需要存储二进制文件.
                //map.AddTBString(EnVerAttr.DBJSON, null, "数据JSON", true, true, 0, 4000, 30);

                map.AddTBString(EnVerAttr.MyNote, null, "备注", true, true, 0, 100, 30);

                map.AddTBDateTime(EnVerAttr.RDT, null, "创建日期", true, true);

                RefMethod rm = new RefMethod();
                rm.Title = "快照";
                rm.ClassMethodName = this.ToString() + ".ShowVer";
                rm.RefMethodType = RefMethodType.RightFrameOpen;
                map.AddRefMethod(rm);

                //数据.
                map.AddDtl(new EnVerDtls(), EnVerAttr.RefPK, null, DtlEditerModel.DtlSearch, null);

                map.DTSearchKey = "RDT";
                map.DTSearchWay = DTSearchWay.ByDateTime;
                map.DTSearchLabel = "日期";

                this._enMap = map;

                return this._enMap;
            }
        }
        #endregion

        public string ShowVer()
        {
            return "/WF/CCBill/OptComponents/DataVer.htm?FrmID=" + this.FrmID + "&WorkID=" + this.EnPKValue;
        }

        protected override bool beforeInsert()
        {
            return base.beforeInsert();
        }
        protected override void afterDelete()
        {
            //删除数据.
            EnVerDtls dtls = new EnVerDtls();
            dtls.Delete(EnVerDtlAttr.RefPK, this.MyPK);
            base.afterDelete();
        }
    }
    /// <summary>
    ///实体版本号s
    /// </summary>
    public class EnVers : EntitiesMyPK
    {
        /// <summary>
        /// 得到一个新实体
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new EnVer();
            }
        }
        /// <summary>
        /// 实体版本号集合
        /// </summary>
        public EnVers()
        {
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<EnVer> ToJavaList()
        {
            return (System.Collections.Generic.IList<EnVer>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<EnVer> Tolist()
        {
            System.Collections.Generic.List<EnVer> list = new System.Collections.Generic.List<EnVer>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((EnVer)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
