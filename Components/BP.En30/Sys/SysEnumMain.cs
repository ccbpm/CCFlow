using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// 枚举注册 属性
    /// </summary>
    public class SysEnumMainAttr : EntityNoNameAttr
    {
        /// <summary>
        /// 配置的值
        /// </summary>
        public const string CfgVal = "CfgVal";
        /// <summary>
        /// 语言
        /// </summary>
        public const string Lang = "Lang";
        /// <summary>
        /// 组织结构编码
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 真实的编号
        /// </summary>
        public const string EnumKey = "EnumKey";
        /// <summary>
        /// 有没有明细？
        /// </summary>
        public const string IsHaveDtl = "IsHaveDtl";
        /// <summary>
        /// 是否有参数.
        /// </summary>
        public const string AtPara = "AtPara";
    }
    /// <summary>
    /// 枚举注册
    /// </summary>
    public class SysEnumMain : EntityNoName
    {
        #region 实现基本的方法
        public int IsHaveDtl
        {
            get
            {
                return this.GetValIntByKey(SysEnumMainAttr.IsHaveDtl);
            }
            set
            {
                this.SetValByKey(SysEnumMainAttr.IsHaveDtl, value);
            }
        }
        /// <summary>
        /// 组织编号
        /// </summary>
        public string OrgNo
        {
            get
            {
                return this.GetValStrByKey(SysEnumMainAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(SysEnumMainAttr.OrgNo, value);
            }
        }
        /// <summary>
        /// 配置的值
        /// </summary>
        public string CfgVal
        {
            get
            {
                return this.GetValStrByKey(SysEnumMainAttr.CfgVal);
            }
            set
            {
                this.SetValByKey(SysEnumMainAttr.CfgVal, value);
            }
        }
        /// <summary>
        /// 语言
        /// </summary>
        public string Lang
        {
            get
            {
                return this.GetValStrByKey(SysEnumMainAttr.Lang);
            }
            set
            {
                this.SetValByKey(SysEnumMainAttr.Lang, value);
            }
        }
        /// <summary>
        /// 枚举值
        /// </summary>
        public string EnumKey
        {
            get
            {
                return this.GetValStrByKey(SysEnumMainAttr.EnumKey);
            }
            set
            {
                this.SetValByKey(SysEnumMainAttr.EnumKey, value);
            }
        }
        #endregion

        #region 构造方法
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.Readonly();
                return uac;
            }
        }

        /// <summary>
        /// SysEnumMain
        /// </summary>
        public SysEnumMain() { }
        /// <summary>
        /// SysEnumMain
        /// </summary>
        /// <param name="no"></param>
        public SysEnumMain(string no)
        {
            try
            {
                this.No = no;
                this.Retrieve();
            }
            catch (Exception ex)
            {
                SysEnums ses = new SysEnums(no);
                if (ses.Count == 0)
                    throw ex;

                this.No = no;
                this.Name = "未命名";
                string cfgVal = "";
                foreach (SysEnum item in ses)
                {
                    cfgVal += "@" + item.IntKey + "=" + item.Lab;
                }
                this.CfgVal = cfgVal;
                this.Insert();
            }
        }
        protected override bool beforeDelete()
        {
            // 检查这个类型是否被使用？
            MapAttrs mattrs = new MapAttrs();
            QueryObject qo = new QueryObject(mattrs);
            qo.AddWhere(MapAttrAttr.UIBindKey, this.No);
            int i = qo.DoQuery();
            if (i == 0)
            {
                BP.Sys.SysEnums ses = new SysEnums();
                ses.Delete(BP.Sys.SysEnumAttr.EnumKey, this.No);
            }
            else
            {
                string msg = "错误:下列数据已经引用了枚举您不能删除它。"; // "错误:下列数据已经引用了枚举您不能删除它。";
                foreach (MapAttr attr in mattrs)
                    msg += "\t\n" + attr.Field + "" + attr.Name + " Table = " + attr.FK_MapData;

                //抛出异常，阻止删除.
                throw new Exception(msg);
            }
            return base.beforeDelete();
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

                Map map = new Map("Sys_EnumMain", "枚举注册");

                /*
                 * 为了能够支持cloud 我们做了如下变更.
                 * 1. 增加了OrgNo, EnumKey 字段.
                 * 2. 如果是单机版用户,原来的业务逻辑不变化.
                 * 3. 如果是SAAS模式, No=  OrgNo+"_"+EnumKey ;
                 */

                map.AddTBStringPK(SysEnumMainAttr.No, null, "编号", true, false, 1, 40, 8);
                map.AddTBString(SysEnumMainAttr.Name, null, "名称", true, false, 0, 40, 8);
                map.AddTBString(SysEnumMainAttr.CfgVal, null, "配置信息", true, false, 0, 1500, 8);
                map.AddTBString(SysEnumMainAttr.Lang, "CH", "语言", true, false, 0, 10, 8);

                //枚举值.
                map.AddTBString(SysEnumMainAttr.EnumKey, null, "EnumKey", true, false, 0, 40, 8);
                //组织编号.
                map.AddTBString(SysEnumMainAttr.OrgNo, null, "OrgNo", true, false, 0, 50, 8);

                map.AddTBInt(SysEnumMainAttr.IsHaveDtl, 0, "是否有子集?", true, false);

                //参数.
                map.AddTBString(SysEnumMainAttr.AtPara, null, "AtPara", true, false, 0, 200, 8);

                for (var index = 0; index < 30; index++)
                {
                    map.AddTBString("Idx" + index, null, "EnumKey", false, false, 0, 50, 8);
                    map.AddTBString("Val" + index, null, "枚举值", false, false, 0, 500, 400);
                }

                this._enMap = map;
                return this._enMap;
            }
        }
        protected override void afterDelete()
        {
            //检查是否有，父表？
            string parentKey = this.GetParaString("ParentKey");
            if (DataType.IsNullOrEmpty(parentKey) == false)
            {
                SysEnumMain en = new SysEnumMain(parentKey);
                en.IsHaveDtl = 0;
                en.SetValByKey(SysEnumMainAttr.AtPara, "");
                en.Update();
            }

            base.afterDelete();
        }
        protected override bool beforeInsert()
        {
            if (DataType.IsNullOrEmpty(this.No) == true)
            {
                if (BP.Difference.SystemConfig.CCBPMRunModel == CCBPMRunModel.Single)
                    this.No = this.EnumKey;
                else
                    this.No = BP.Web.WebUser.OrgNo + "_" + this.EnumKey;
            }
            return base.beforeInsert();
        }


        protected override bool beforeUpdateInsertAction()
        {
            if (BP.Difference.SystemConfig.CCBPMRunModel != CCBPMRunModel.Single)
                this.OrgNo = BP.Web.WebUser.OrgNo;
            return base.beforeUpdateInsertAction();
        }

        protected override void afterUpdate()
        {   
            //清除所有的缓存，这个位置会造成拼接SQL错误 case When
            BP.DA.Cash.ClearCash();
            base.afterUpdate();
        }
        #endregion

        #region 业务操作。
        public string DoInitDtls()
        {
            if (DataType.IsNullOrEmpty(this.CfgVal) == true)
                return "err@没有cfg数据.";

            //首先删除原来的数据.
            SysEnums ses = new SysEnums();
            ses.Delete(SysEnumAttr.EnumKey, this.No);

            //创建对象.
            SysEnum se = new SysEnum();

            //CfgVal = @0=病假@1=事假
            string[] strs = this.CfgVal.Split('@');
            foreach (string str in strs)
            {
                if (DataType.IsNullOrEmpty(str))
                    continue;

                string[] kvs = str.Split('=');

                se.EnumKey = this.No;
                se.IntKey = int.Parse(kvs[0]);
                se.Lab = kvs[1].Trim();
                se.Lang = "CH";
                se.Insert();
                //   se.setMyPK(this.No+"_"+se
            }

            return "执行成功.";
        }
        #endregion 业务操作。

    }
    /// <summary>
    /// 枚举注册 s
    /// </summary>
    public class SysEnumMains : EntitiesNoName
    {
        /// <summary>
        /// SysEnumMains
        /// </summary>
        public SysEnumMains() { }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SysEnumMain();
            }
        }
        /// <summary>
        /// 查询所有枚举值，根据不同的运行平台.
        /// </summary>
        /// <returns></returns>
        public override int RetrieveAll()
        {
            // 获取平台的类型. 0=单机版, 1=集团版，2=SAAS。
            int val =  BP.Difference.SystemConfig.GetValByKeyInt("CCBPMRunModel", 0);
            if (val != 2)
                return base.RetrieveAll();

            // 返回他组织下的数据.
            return this.Retrieve(SysEnumMainAttr.OrgNo, BP.Web.WebUser.FK_Dept);
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SysEnumMain> ToJavaList()
        {
            return (System.Collections.Generic.IList<SysEnumMain>)this;
        }

        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SysEnumMain> Tolist()
        {
            System.Collections.Generic.List<SysEnumMain> list = new System.Collections.Generic.List<SysEnumMain>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SysEnumMain)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
