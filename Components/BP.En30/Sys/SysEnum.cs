﻿using System;
using System.Collections;
using BP.DA;
using BP.En;

namespace BP.Sys
{
    /// <summary>
    /// sss
    /// </summary>
    public class SysEnumAttr
    {
        /// <summary>
        /// 标题  
        /// </summary>
        public const string Lab = "Lab";
        /// <summary>
        /// Int key
        /// </summary>
        public const string IntKey = "IntKey";
        /// <summary>
        /// EnumKey
        /// </summary>
        public const string EnumKey = "EnumKey";
        /// <summary>
        /// Language
        /// </summary>
        public const string Lang = "Lang";
        /// <summary>
        /// OrgNo
        /// </summary>
        public const string OrgNo = "OrgNo";
        /// <summary>
        /// 关联主键
        /// </summary>
        public const string RefPK = "RefPK";
    }
    /// <summary>
    /// SysEnum
    /// </summary>
    public class SysEnum : EntityMyPK
    {
        /// <summary>
        /// 得到一个String By LabKey.
        /// </summary>
        /// <param name="EnumKey"></param>
        /// <param name="intKey"></param>
        /// <returns></returns>
        public static string GetLabByPK(string EnumKey, int intKey)
        {
            SysEnum en = new SysEnum(EnumKey, intKey);
            return en.Lab;
        }

        #region 实现基本的方法
        /// <summary>
        /// 关联外键
        /// </summary>
        public string RefPK
        {
            get
            {
                return this.GetValStringByKey(SysEnumAttr.RefPK);
            }
            set
            {
                this.SetValByKey(SysEnumAttr.RefPK, value);
            }
        }
        public string OrgNo
        {
            get
            {
                return this.GetValStringByKey(SysEnumAttr.OrgNo);
            }
            set
            {
                this.SetValByKey(SysEnumAttr.OrgNo, value);
            }
        }
        /// <summary>
        /// 标签
        /// </summary>
        public string Lab
        {
            get
            {
                return this.GetValStringByKey(SysEnumAttr.Lab);
            }
            set
            {
                this.SetValByKey(SysEnumAttr.Lab, value);
            }
        }
        /// <summary>
        /// 标签
        /// </summary>
        public string Lang
        {
            get
            {
                return this.GetValStringByKey(SysEnumAttr.Lang);
            }
            set
            {
                this.SetValByKey(SysEnumAttr.Lang, value);
            }
        }
        /// <summary>
        /// Int val
        /// </summary>
        public int IntKey
        {
            get
            {
                return this.GetValIntByKey(SysEnumAttr.IntKey);
            }
            set
            {
                this.SetValByKey(SysEnumAttr.IntKey, value);
            }
        }
        /// <summary>
        /// EnumKey
        /// </summary>
        public string EnumKey
        {
            get
            {
                return this.GetValStringByKey(SysEnumAttr.EnumKey);
            }
            set
            {
                this.SetValByKey(SysEnumAttr.EnumKey, value);
            }
        }
        #endregion

        #region 构造方法
        /// <summary>
        /// SysEnum
        /// </summary>
        public SysEnum() { }
        /// <summary>
        /// 税务编号
        /// </summary>
        /// <param name="_No">编号</param>
        public SysEnum(string enumKey, int val)
        {
            this.EnumKey = enumKey;
            this.Lang = BP.Web.WebUser.SysLang;
            this.IntKey = val;
            this.MyPK = this.EnumKey + "_" + this.Lang + "_" + this.IntKey;
            int i = this.RetrieveFromDBSources();
            if (i == 0)
            {
                i = this.Retrieve(SysEnumAttr.EnumKey, enumKey, SysEnumAttr.Lang, BP.Web.WebUser.SysLang,
                     SysEnumAttr.IntKey, this.IntKey);
                SysEnums ses = new SysEnums();
                ses.Full(enumKey);
                if (i == 0)
                {
                    //尝试注册系统的枚举的配置.
                    BP.Sys.SysEnums myee = new SysEnums(enumKey);
                    throw new Exception("@ EnumKey=" + EnumKey + " Val=" + val + " Lang=" + Web.WebUser.SysLang + " ...Error");
                }
            }
        }
        public SysEnum(string enumKey, string Lang, int val)
        {
            this.EnumKey = enumKey;
            this.Lang = Lang;
            this.IntKey = val;
            this.MyPK = this.EnumKey + "_" + this.Lang + "_" + this.IntKey;
            int i = this.RetrieveFromDBSources();
            if (i == 0)
            {
                i = this.Retrieve(SysEnumAttr.EnumKey, enumKey, SysEnumAttr.Lang, Lang,
                    SysEnumAttr.IntKey, this.IntKey);

                SysEnums ses = new SysEnums();
                ses.Full(enumKey);

                if (i == 0)
                    throw new Exception("@ EnumKey=" + enumKey + " Val=" + val + " Lang=" + Lang + " Error");
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

                Map map = new Map("Sys_Enum", "枚举数据");

                /*
                * 为了能够支持 cloud 我们做了如下变更.
                * 1. 增加了OrgNo 字段.
                * 2. 如果是单机版用户,原来的业务逻辑不变化. MyPK= EnumKey+"_"+IntKey+'_'+Lang 
                * 3. 如果是SAAS模式, MyPK= EnumKey+"_"+IntKey+'_'+Lang +"_"+OrgNo 
                */

                map.AddMyPK();
                map.AddTBString(SysEnumAttr.Lab, null, "Lab", true, false, 1, 300, 8);
                map.AddTBString(SysEnumAttr.EnumKey, null, "EnumKey", true, false, 1, 100, 8);
                map.AddTBInt(SysEnumAttr.IntKey, 0, "Val", true, false);
                map.AddTBString(SysEnumAttr.Lang, "CH", "语言", true, false, 0, 10, 8);

                map.AddTBString(SysEnumMainAttr.OrgNo, null, "OrgNo", true, false, 0, 50, 8);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion


        protected override bool beforeUpdateInsertAction()
        {
            if (this.Lang == null && this.Lang == "")
                this.Lang = BP.Web.WebUser.SysLang;

            this.MyPK = this.EnumKey + "_" + this.Lang + "_" + this.IntKey;

            if (SystemConfig.CCBPMRunModel != 0)
                this.RefPK = this.OrgNo + "_" + this.EnumKey; //关联的主键.
                                                              //  else
                                                              //    this.RefPK = this.OrgNo + "_" + this.EnumKey; //关联的主键.

            return base.beforeUpdateInsertAction();
        }

        /// <summary>
        /// 枚举类型新增保存后在Frm_RB中增加新的枚举值
        /// </summary>
        protected override void afterInsert()
        {
            //获取引用枚举的表单
            string sql = " select  distinct(FK_MapData)from Sys_FrmRB where EnumKey='" + this.EnumKey + "'";
            System.Data.DataTable dt = DBAccess.RunSQLReturnTable(sql);
            if (dt.Rows.Count == 0)
            {
                base.afterInsert();
                return;
            }

            foreach (System.Data.DataRow dr in dt.Rows)
            {
                if (DataType.IsNullOrEmpty(dr[0].ToString()))
                    continue;
                string fk_mapdata = dr[0].ToString();
                string mypk = fk_mapdata + "_" + this.EnumKey + "_" + this.IntKey;
                FrmRB frmrb = new FrmRB();
                if (frmrb.IsExit("MyPK", mypk) == true)
                {
                    frmrb.Lab = this.Lab;
                    frmrb.Update();
                    continue;
                }
                //获取mapAttr 
                MapAttr mapAttr = new MapAttr();
                mapAttr.MyPK = fk_mapdata + "_" + this.EnumKey;
                int i = mapAttr.RetrieveFromDBSources();
                if (i != 0)
                {

                    int RBShowModel = mapAttr.GetParaInt("RBShowModel");
                    FrmRB frmrb1 = new FrmRB();
                    frmrb1.MyPK = fk_mapdata + "_" + this.EnumKey + "_" + this.IntKey;


                    frmrb.FK_MapData = fk_mapdata;
                    frmrb.KeyOfEn = this.EnumKey;
                    frmrb.EnumKey = this.EnumKey;
                    frmrb.Lab = this.Lab;
                    frmrb.IntKey = this.IntKey;
                    if (RBShowModel == 0)
                    {
                        frmrb.X = frmrb1.X;
                        frmrb.Y = frmrb1.Y + 40;
                    }
                    if (RBShowModel == 3)
                    {
                        frmrb.X = frmrb1.X + 100;
                        frmrb.Y = frmrb1.Y;
                    }
                    frmrb.Insert();
                }

            }

            base.afterInsert();
        }
    }
    /// <summary>
    /// 纳税人集合 
    /// </summary>
    public class SysEnums : Entities
    {
        /// <summary>
        /// 此枚举类型的个数
        /// </summary>
        public int Num = -1;
        public string ToDesc()
        {
            string strs = "";
            foreach (SysEnum se in this)
            {
                strs += se.IntKey + " " + se.Lab + ";";
            }
            return strs;
        }
        public string GenerCaseWhenForOracle(string enName, string mTable, string key, string field, string enumKey, int def)
        {
            string sql = (string)Cash.GetObjFormApplication("ESQL" + enName + mTable + key + "_" + enumKey, null);
            // string sql = "";
            if (sql != null)
                return sql;

            if (this.Count == 0)
                throw new Exception("@枚举值" + enumKey + "已被删除。");

            sql = " CASE NVL(" + mTable + field + "," + def + ")";
            foreach (SysEnum se1 in this)
            {
                sql += " WHEN " + se1.IntKey + " THEN '" + se1.Lab + "'";
            }

            SysEnum se = (SysEnum)this.GetEntityByKey(SysEnumAttr.IntKey, def);
            if (se == null)
                sql += " END \"" + key + "Text\"";
            else
                sql += " WHEN NULL THEN '" + se.Lab + "' END \"" + key + "Text\"";

            Cash.AddObj("ESQL" + enName + mTable + key + "_" + enumKey, Depositary.Application, sql);
            return sql;
        }

        public string GenerCaseWhenForOracle(string mTable, string key, string field, string enumKey, int def)
        {
            if (this.Count == 0)
                throw new Exception("@枚举值（" + enumKey + "）已被删除，无法形成期望的SQL。");


            string sql = "";
            sql = " CASE " + mTable + field;
            foreach (SysEnum se1 in this)
                sql += " WHEN " + se1.IntKey + " THEN '" + se1.Lab + "'";

            SysEnum se = (SysEnum)this.GetEntityByKey(SysEnumAttr.IntKey, def);
            if (se == null)
                sql += " END \"" + key + "Text\"";
            else
                sql += " WHEN NULL THEN '" + se.Lab + "' END \"" + key + "Text\"";

            // Cash.AddObj("ESQL" + enName + key + "_" + enumKey, Depositary.Application, sql);
            return sql;
        }
        public void LoadIt(string enumKey)
        {
            if (this.Full(enumKey) == false)
            {

                try
                {
                    DBAccess.RunSQL("UPDATE Sys_Enum SET Lang='" + Web.WebUser.SysLang + "' WHERE LANG IS NULL ");

                    DBAccess.RunSQL("UPDATE Sys_Enum SET MyPK=EnumKey+'_'+Lang+'_'+cast(IntKey as NVARCHAR )");

                    //增加数据库类型判断
                    DBUrl dbUrl = new DBUrl();
                    if (DBType.MSSQL == dbUrl.DBType)
                    {
                        DBAccess.RunSQL("UPDATE Sys_Enum SET MyPK=EnumKey+'_'+Lang+'_'+cast(IntKey as NVARCHAR )");
                    }
                    else if (DBType.Oracle == dbUrl.DBType)
                    {
                        DBAccess.RunSQL("UPDATE Sys_Enum SET MyPK = EnumKey || '_' || Lang || '_' || cast(IntKey  as VARCHAR(5))");
                    }
                    else if (DBType.MySQL == dbUrl.DBType)
                    {
                        DBAccess.RunSQL("UPDATE Sys_Enum SET MyPK = CONCAT (EnumKey,'_', Lang,'_',CAST(IntKey AS CHAR(5)))");
                    }

                }
                catch
                {

                }

                try
                {
                    BP.Sys.XML.EnumInfoXml xml = new BP.Sys.XML.EnumInfoXml(enumKey);
                    this.RegIt(enumKey, xml.Vals);
                }
                catch (Exception ex)
                {
                    throw new Exception("@你没有预制[" + enumKey + "]枚举值。@在修复枚举值出现错误:" + ex.Message);
                }
            }
        }
        /// <summary>
        /// 把所有的枚举注册一遍.
        /// </summary>
        public static void RegAll()
        {
            BP.Sys.XML.EnumInfoXmls xmls = new BP.Sys.XML.EnumInfoXmls();
            xmls.RetrieveAll();
            SysEnums ses = new SysEnums();
            foreach (BP.Sys.XML.EnumInfoXml xml in xmls)
                ses.RegIt(xml.Key, xml.Vals);
        }
        /// <summary>
        /// SysEnums
        /// </summary>
        /// <param name="EnumKey"></param>
        public SysEnums(string enumKey)
        {
            this.LoadIt(enumKey);
        }
        public SysEnums(string enumKey, string vals)
        {
            if (vals == null || vals == "")
            {
                this.LoadIt(enumKey);
                return;
            }

            if (this.Full(enumKey) == false)
                this.RegIt(enumKey, vals);
        }
        public void RegIt(string EnumKey, string vals)
        {
            try
            {
                string[] strs = vals.Split('@');
                SysEnums ens = new SysEnums();
                ens.Delete(SysEnumAttr.EnumKey, EnumKey);
                this.Clear();

                foreach (string s in strs)
                {
                    if (DataType.IsNullOrEmpty(s) ==true)
                        continue;

                    string[] vk = s.Split('=');
                    SysEnum se = new SysEnum();
                    se.IntKey = int.Parse(vk[0]);
                    //杨玉慧
                    //解决当  枚举值含有 ‘=’号时，保存不进去的方法
                    string[] kvsValues = new string[vk.Length - 1];
                    for (int i = 0; i < kvsValues.Length; i++)
                    {
                        kvsValues[i] = vk[i + 1];
                    }
                    se.Lab = string.Join("=", kvsValues);
                    se.EnumKey = EnumKey;
                    se.Lang = BP.Web.WebUser.SysLang;
                    se.Insert();
                    this.AddEntity(se);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message + " - " + vals);
            }
            //  this.Full(EnumKey);
        }
        public bool Full(string enumKey)
        {
            Entities ens = (Entities)Cash.GetObjFormApplication("EnumOf" + enumKey + Web.WebUser.SysLang, null);
            if (ens != null)
            {
                this.AddEntities(ens);
                return true;
            }

            QueryObject qo = new QueryObject(this);
            qo.AddWhere(SysEnumAttr.EnumKey, enumKey);
            qo.addAnd();
            qo.AddWhere(SysEnumAttr.Lang, Web.WebUser.SysLang);
            qo.addOrderBy(SysEnumAttr.IntKey);
            if (qo.DoQuery() == 0)
            {
                /* 看看xml配置里面是否有?*/
                return false;
            }

            Cash.AddObj("EnumOf" + enumKey + Web.WebUser.SysLang, Depositary.Application, this);
            return true;
        }
        ///// <summary>
        ///// DBSimpleNoNames
        ///// </summary>
        ///// <returns></returns>
        //public DBSimpleNoNames ToEntitiesNoName()
        //{
        //    DBSimpleNoNames ens = new DBSimpleNoNames();
        //    foreach (SysEnum en in this)
        //    {
        //        ens.AddByNoName(en.IntKey.ToString(), en.Lab);
        //    }
        //    return ens;
        //}
        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public int Delete(string key, object val)
        {
            try
            {
                Entity en = this.GetNewEntity;
                Paras ps = new Paras();

                ps.SQL = "DELETE FROM " + en.EnMap.PhysicsTable + " WHERE " + key + "=" + en.HisDBVarStr + "p";
                ps.Add("p", val);
                return en.RunSQL(ps);
            }
            catch
            {
                Entity en = this.GetNewEntity;
                en.CheckPhysicsTable();

                Paras ps = new Paras();
                ps.SQL = "DELETE FROM " + en.EnMap.PhysicsTable + " WHERE " + key + "=" + en.HisDBVarStr + "p";
                ps.Add("p", val);
                return en.RunSQL(ps);
            }
        }
        /// <summary>
        /// SysEnums
        /// </summary>
        public SysEnums() { }
        /// <summary>
        /// 得到它的 Entity
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new SysEnum();
            }
        }
        /// <summary>
        /// 通过int 得到Lab
        /// </summary>
        /// <param name="val">val</param>
        /// <returns>string val</returns>
        public string GetLabByVal(int val)
        {
            foreach (SysEnum en in this)
            {
                if (en.IntKey == val)
                    return en.Lab;
            }
            return null;
        }

        #region 为了适应自动翻译成java的需要,把实体转换成List.
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<SysEnum> ToJavaList()
        {
            return (System.Collections.Generic.IList<SysEnum>)this;
        }
        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<SysEnum> Tolist()
        {
            System.Collections.Generic.List<SysEnum> list = new System.Collections.Generic.List<SysEnum>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((SysEnum)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
