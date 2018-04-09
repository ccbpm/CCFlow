using System;
using System.Collections;
using BP.DA;
using BP.En;
using BP;

namespace BP.Tools
{
    /// <summary>
    /// 表单签名
    /// </summary>
    public class WFSealDataAttr
    {
        /// <summary>
        /// 工作ID
        /// </summary>
        public const string OID = "OID";
        /// <summary>
        /// 节点ID
        /// </summary>
        public const string FK_Node = "FK_Node";

        /// <summary>
        /// 记录日期
        /// </summary>
        public const string RDT = "RDT";
        /// <summary>
        /// 签名信息
        /// </summary>
        public const string SealData = "SealData";
        /// <summary>
        /// 表单名称
        /// </summary>
        public const string FK_MapData = "FK_MapData";

    }
    /// <summary>
    /// 表单签名信息
    /// </summary>
    public class WFSealData : EntityMyPK
    {

        #region 用户日志信息键值列表
        #endregion

        #region 基本属性
        public string SealData
        {
            get
            {
                return this.GetValStringByKey(WFSealDataAttr.SealData);
            }
            set
            {
                this.SetValByKey(WFSealDataAttr.SealData, value);
            }
        }
        /// <summary>
        /// FK_Emp
        /// </summary>
        public string FK_Node
        {
            get
            {
                return this.GetValStringByKey(WFSealDataAttr.FK_Node);
            }
            set
            {
                this.SetValByKey(WFSealDataAttr.FK_Node, value);
            }
        }
        public string RDT
        {
            get
            {
                return this.GetValStringByKey(WFSealDataAttr.RDT);
            }
            set
            {
                this.SetValByKey(WFSealDataAttr.RDT, value);
            }
        }

        public string FK_MapData
        {
            get
            {
                return this.GetValStringByKey(WFSealDataAttr.FK_MapData);
            }
            set
            {
                this.SetValByKey(WFSealDataAttr.FK_MapData, value);
            }
        }

        public string OID
        {
            get
            {
                return this.GetValStringByKey(WFSealDataAttr.OID);
            }
            set
            {
                this.SetValByKey(WFSealDataAttr.OID, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// 签名信息
        /// </summary>
        public WFSealData()
        {
        }
        /// <summary>
        /// 特殊处理 将SealData 字段变为大文本存储
        /// </summary>
        /// <returns></returns>
        public void UpdateColumn()
        {

            string sql = "";

            switch (DBAccess.AppCenterDBType)
            {
                case DBType.Oracle:
                    sql = string.Format("ALTER TABLE {0} modify({1} {2})", "Sys_WFSealData", WFSealDataAttr.SealData, "CLOB");
                    break;
                case DBType.Informix:
                 //   DBAccess.RunSQL(SqlBuilder.GenerCreateTableSQLOfInfoMix(this));
                    break;
                case DBType.MSSQL:
                    sql = string.Format("alter table {0} alter column {1}  {2}", "Sys_WFSealData", WFSealDataAttr.SealData, "text");
                    break;
                case DBType.MySQL:
                    sql = string.Format("ALTER TABLE {0} MODIFY COLUMN {1} {2}", "Sys_WFSealData", WFSealDataAttr.SealData, "text");
                    break;
                case DBType.Access:
                    //DBAccess.RunSQL(SqlBuilder.GenerCreateTableSQLOf_OLE(this));
                    break;
                default:
                    throw new Exception("@未判断的数据库类型。");
            }
            if(!DataType.IsNullOrEmpty(sql))
                DBAccess.RunSQL(sql);
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
                Map map = new Map("Sys_WFSealData", "签名信息");
                map.Java_SetDepositaryOfEntity(Depositary.None);
                map.Java_SetDepositaryOfMap( Depositary.Application);

                map.Java_SetEnType(EnType.Sys);
                map.AddMyPK();
                map.AddTBString(WFSealDataAttr.OID, null, "OID", false, false, 0, 200, 20);
                map.AddTBString(WFSealDataAttr.FK_Node, null, "FK_Node", true, false, 0, 200, 20);
                map.AddTBString(WFSealDataAttr.FK_MapData, null, "FK_MapData", true, false, 0, 100, 20);
                map.AddTBString(WFSealDataAttr.SealData, null, "SealData", true, false, 0, 4000, 20);
                map.AddTBString(WFSealDataAttr.RDT, null, "记录日期", true, false, 0, 20, 20);
                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion

        #region 重写
        public override Entities GetNewEntities
        {
            get { return new WFSealDatas(); }
        }
        #endregion 重写
    }
    /// <summary>
    /// 用户日志s
    /// </summary>
    public class WFSealDatas : EntitiesMyPK
    {
        #region 构造
        public WFSealDatas()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="emp"></param>
        public WFSealDatas(string workID, string node)
        {
            QueryObject qo = new QueryObject(this);
            qo.AddWhere(WFSealDataAttr.OID, workID);
            qo.AddWhere(WFSealDataAttr.FK_Node, node);
            qo.DoQuery();
        }
        #endregion

        #region 重写
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new WFSealData();
            }
        }
        #endregion

    }
}
