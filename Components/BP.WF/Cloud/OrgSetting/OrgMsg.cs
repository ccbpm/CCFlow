using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Sys;
using BP.Web;

namespace BP.Cloud.OrgSetting
{
    /// <summary>
    /// 组织 属性
    /// </summary>
    public class OrgMsgAttr : OrgAttr
    {
        #region 基本属性
        public const string SendToAccepter = "SendToAccepter";
        public const string SendToReturner = "SendToReturner";
        public const string SendToStarter = "SendToStarter";
        #endregion
    }
    /// <summary>
    /// 组织 的摘要说明。
    /// </summary>
    public class OrgMsg : EntityNoName
    {
        #region 扩展属性
        public string Addr
        {
            get
            {

                return this.GetValStrByKey(OrgMsgAttr.Addr);
            }
            set
            {
                this.SetValByKey(OrgMsgAttr.Addr, value);
            }
        }
        public string GUID
        {
            get
            {
                return this.GetValStrByKey(OrgMsgAttr.GUID);
            }
            set
            {
                this.SetValByKey(OrgMsgAttr.GUID, value);
            }
        }
        /// <summary>
        /// 拼音
        /// </summary>
        public string Adminer
        {
            get
            {
                return this.GetValStrByKey(OrgMsgAttr.Adminer);
            }
            set
            {
                this.SetValByKey(OrgMsgAttr.Adminer, value);
            }
        }
        /// <summary>
        /// 全名
        /// </summary>
        public string NameFull
        {
            get
            {
                return this.GetValStrByKey(OrgMsgAttr.NameFull);
            }
            set
            {
                this.SetValByKey(OrgMsgAttr.NameFull, value);
            }
        }

        #endregion

        #region 构造函数
        /// <summary>
        /// 组织
        /// </summary>
        public OrgMsg()
        {
        }
        public OrgMsg(string no)
        {
            this.No = no;
            this.Retrieve();
        }
        /// <summary>
        /// 权限
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();

                if (WebUser.No.Equals("admin") == true)
                {
                    uac.OpenAll();
                    uac.IsInsert = false;
                    uac.IsDelete = false;
                    return uac;
                }

                uac.IsInsert = false;
                uac.IsDelete = false;
                if (this.No.Equals(WebUser.OrgNo) == true)
                {
                    uac.IsUpdate = true;
                    return uac;
                }

                //删除.
                uac.IsInsert = false;
                uac.IsDelete = false;
                uac.IsView = false;
                return uac;
            }
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

                Map map = new Map("Port_Org", "消息机制");
                map.setEnType(EnType.App);

                #region 字段.
                /*关于字段属性的增加 */
                map.AddTBStringPK(OrgMsgAttr.No, null, "OrgNo", true, true, 1, 50, 90);
                map.AddTBString(OrgMsgAttr.Name, null, "简称", true, true, 0, 200, 130);

                map.AddBoolean(OrgMsgAttr.SendToAccepter, true, "发送成功后:发送个所有下一步接受人", true, true,true);
                map.AddBoolean(OrgMsgAttr.SendToReturner, true, "退回后:发送个所有被退回人", true, true, true);
                map.AddBoolean(OrgMsgAttr.SendToStarter, true, "流程结束后:发送消息给流程发起人", true, true, true);
                #endregion 字段.

                this._enMap = map;
                return this._enMap;
            }
        }
        /// <summary>
        /// 获取集合
        /// </summary>
        public override Entities GetNewEntities
        {
            get { return new OrgMsgs(); }
        }
        #endregion 构造函数
    }
    /// <summary>
    /// 组织s
    // </summary>
    public class OrgMsgs : EntitiesNoName
    {
        #region 构造方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new OrgMsg();
            }
        }
        /// <summary>
        /// 组织s
        /// </summary>
        public OrgMsgs()
        {
        }
        #endregion 构造方法

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<OrgMsg> ToJavaList()
        {
            return (System.Collections.Generic.IList<OrgMsg>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<OrgMsg> Tolist()
        {
            System.Collections.Generic.List<OrgMsg> list = new System.Collections.Generic.List<OrgMsg>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((OrgMsg)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
