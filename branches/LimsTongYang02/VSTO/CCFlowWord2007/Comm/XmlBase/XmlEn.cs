using System;
using System.IO;
using System.Collections;
using BP.DA;
using System.Data;
using BP.En;

namespace BP.XML
{
    /// <summary>
    /// XmlEn ��ժҪ˵����
    /// </summary>
    abstract public class XmlEn
    {
        #region ��ȡֵ
        private Row _row = null;
        public Row Row
        {
            get
            {
                if (this._row == null)
                    throw new Exception("xmlEn û�б�ʵ������");
                return this._row;
            }
            set
            {
                this._row = value;
            }
        }
        /// <summary>
        /// ��ȡһ������
        /// </summary>
        /// <param name="attrKey"></param>
        /// <returns></returns>
        public Object GetValByKey(string attrKey)
        {
            if (this._row == null)
                return null;

            return this.Row[attrKey];
        }
        public int GetValIntByKey(string key)
        {
            try
            {
                return int.Parse(this.GetValByKey(key).ToString().Trim());
            }
            catch
            {
                throw new Exception("key=" + key + "������int ����ת����val=" + this.GetValByKey(key));
            }
        }
        public decimal GetValDecimalByKey(string key)
        {
            return (decimal)this.GetValByKey(key);
        }
        /// <summary>
        /// ��ȡһ������
        /// </summary>
        /// <param name="attrKey"></param>
        /// <returns></returns>
        public string GetValStringByKey(string attrKey)
        {
            if (this._row == null)
                return "";

            try
            {
                return this.Row[attrKey].ToString();
            }
            catch (Exception ex)
            {
                throw new Exception(" @XMLEN Error Attr=[" + attrKey + "], ClassName= " + this.ToString() + " , File =" + this.GetNewEntities.File + " , Error = " + ex.Message);
            }
        }
        public string GetValStringHtmlByKey(string attrKey)
        {
            return this.GetValStringByKey(attrKey).Replace("\n", "<BR>").Replace(" ", "&nbsp;");
        }
        /// <summary>
        /// ��ȡһ������
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetValBoolByKey(string key)
        {
            string val = this.GetValStringByKey(key);
            if (val == "1" || val.ToUpper() == "TRUE")
                return true;
            else
                return false;
        }
        #endregion ��ȡֵ

        #region ���캯��
        /// <summary>
        /// ���캯��
        /// </summary>
        public XmlEn()
        {
        }

        /// <summary>
        /// ָ�����Բ�ѯ
        /// </summary>
        /// <param name="key">����ֵ</param>
        /// <param name="val">����</param>
        public int RetrieveBy_del(string key, string val)
        {
            XmlEns ens = this.GetNewEntities;
            ens.RetrieveAll();

            ens.RetrieveBy(key, val);
            if (ens.Count == 0)
                return 0;

            this.Row = ens[0].Row;
            return ens.Count;
        }
        public int RetrieveByPK(string key, string val)
        {
            XmlEns ens = null; // Cash.GetObj(this.GetNewEntities.ToString(), Depositary.Application) as XmlEns;
            if (ens == null)
            {
                ens = this.GetNewEntities;
                ens.RetrieveAll();
            }

            int i = 0;
            foreach (XmlEn en in ens)
            {
                if (en.GetValStringByKey(key) == val)
                {
                    this.Row = en.Row;
                    i++;
                }
            }
            if (i == 1)
                return 1;

            if (i > 1)
            {
               // BP.SystemConfig.DoClearCash();
                throw new Exception("@XML = " + this.ToString() + " �� PK=" + val + "��Ψһ��������");
            }
            return 0;
        }
        #endregion ���캯��

        #region ��Ҫ����ʵ�ֵķ���
        public abstract XmlEns GetNewEntities { get; }
        #endregion ��Ҫ����ʵ�ֵķ���
    }
    abstract public class XmlEnNoName : XmlEn
    {
        public string No
        {
            get
            {
                return this.GetValStringByKey("No");
            }
        }
        public string Name
        {
            get
            {
                return this.GetValStringByKey("Name");
            }
        }
        public XmlEnNoName()
        {
        }
        public XmlEnNoName(string no)
        {
            this.RetrieveByPK("No", no);
        }
    }
    /// <summary>
    /// XmlEn ��ժҪ˵����
    /// </summary>
    abstract public class XmlEns : System.Collections.CollectionBase
    {

        #region ����
        /// <summary>
        /// ����
        /// </summary>
        public XmlEns()
        {

        }
        #endregion ����

        #region ��ѯ����
        public string Tname
        {
            get
            {
                string tname = this.File.Replace(".TXT", "").Replace(".txt", "");
                tname = tname.Substring(tname.LastIndexOf("\\") + 1) + this.TableName + "_X";
                return tname;
            }
        }

        private DataTable GetTableTxt(DataTable dt, FileInfo file)
        {
            StreamReader sr = new StreamReader(file.FullName, System.Text.ASCIIEncoding.GetEncoding("GB2312"));
            Hashtable ht = new Hashtable();
            string key = "";
            string val = "";
            while (true)
            {
                if (sr.EndOfStream)
                    break;
                string lin = sr.ReadLine();
                if (lin == "" || lin == null)
                    continue;


                if (lin.IndexOf("*") == 0)
                {
                    /* ����ע���ļ� */
                    continue;
                }

                if (lin.IndexOf("=") == 0 || sr.EndOfStream)
                {


                    /* Լ�����м�¼, ��ʼ�� = ��ʼ����Ϊ��һ���µļ�¼�� */
                    // �����ṹ��
                    foreach (string ojbkey in ht.Keys)
                    {
                        if (dt.Columns.Contains(ojbkey) == false)
                        {
                            dt.Columns.Add(new DataColumn(ojbkey, typeof(string)));
                        }
                    }

                    DataRow dr = dt.NewRow();
                    foreach (string ojbkey in ht.Keys)
                    {
                        dr[ojbkey] = ht[ojbkey];
                    }

                    if (ht.Keys.Count > 1)
                        dt.Rows.Add(dr);


                    ht.Clear(); // clear hashtable.
                    if (sr.EndOfStream)
                        break;
                    continue;
                }

                int idx = lin.IndexOf("=");
                if (idx == -1)
                {
                    throw new Exception(this.File + "@�����Ϲ��� key =val �Ĺ���");
                }

                key = lin.Substring(0, idx);
                if (key == "")
                    continue;

                val = lin.Substring(idx + 1);
                ht.Add(key, val);
            }


            return dt;
        }
        public DataTable GetTable()
        {

            DataTable cdt = null; // BP.DA.Cash.GetObj(this.Tname, Depositary.Application) as DataTable;
            if (cdt != null)
                return cdt;

            if (this.File.ToLower().IndexOf(".xml") > 0)
            {
                DataSet ds1 = new DataSet();
                ds1.ReadXml(this.File);
                DataTable mdt = ds1.Tables[this.TableName];
                if (mdt == null)
                    mdt = new DataTable();

                //BP.DA.Cash.AddObj(this.Tname,
                //    Depositary.Application, mdt);
                return ds1.Tables[this.TableName];
            }

            /* ˵�������Ŀ¼ */
            System.IO.DirectoryInfo di = new System.IO.DirectoryInfo(this.File);
            if (di.Exists == false)
                throw new Exception("�ļ�������:" + this.File);

            FileInfo[] fis = di.GetFiles("*.xml");
          
            DataTable dt = new DataTable(this.TableName);
            if (fis.Length == 0)
                return dt;

            DataTable tempDT = new DataTable();
            foreach (FileInfo fi in fis)
            {

                DataSet ds = new DataSet("myds");
                try
                {
                    ds.ReadXml(this.File + "\\" + fi.Name);
                }
                catch (Exception ex)
                {
                    throw new Exception("��ȡ�ļ�:" + fi.Name + "����Exception=" + ex.Message);
                }
                try
                {
                    //ds.
                    if (dt.Columns.Count == 0)
                    {
                        /* ������ǿյģ�û���κνṹ��*/
                        try
                        {
                            dt = ds.Tables[this.TableName];
                        }
                        catch (Exception ex)
                        {
                            throw new Exception("������û����" + fi.Name + "�ļ����ҵ���:" + this.TableName + " exception=" + ex.Message);
                        }
                        tempDT = dt.Clone();
                        continue;
                    }

                    DataTable mydt = ds.Tables[this.TableName];
                    if (mydt == null)
                        throw new Exception("�޴˱�:" + this.TableName);

                    if (mydt.Rows.Count == 0)
                        continue;

                    foreach (DataRow mydr in mydt.Rows)
                    {
                        //dt.ImportRow(mydr);
                        DataRow dr = dt.NewRow();

                        foreach (DataColumn dc in tempDT.Columns)
                        {
                            //string "sd".Clone();
                            if (dc.ColumnName.IndexOf("_Id") != -1)
                                continue;

                            try
                            {
                                object obj = mydr[dc.ColumnName];
                                dr[dc.ColumnName] = obj;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception("xml ���ô��󣬶���ļ��е����Բ��Գơ�" + ex.Message);
                            }
                        }

                        dt.Rows.Add(dr);
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("��ȡ���ݳ��ִ���:fileName=" + fi.Name + " clasName=" + this.ToString() + " MoreInfo=" + ex.Message);
                }
            }
            //BP.DA.Cash.AddObj(this.Tname,
            //    Depositary.Application,
            //    dt);
            return dt;
        }
        public virtual int RetrieveAllFromDBSource()
        {
           // BP.DA.Cash.RemoveObj(this.Tname);
            return this.RetrieveAll();
        }
        /// <summary>
        /// װ��XML
        /// </summary>
        public virtual int RetrieveAll()
        {
            this.Clear(); // �����е���Ϣ��
            XmlEns ens = null;// BP.DA.Cash.GetObj(this.ToString(), Depositary.Application) as XmlEns;
            if (ens != null)
            {
                foreach (XmlEn en in ens)
                    this.Add(en);
                return ens.Count;
            }

            // ���ڴ����ҡ�
            DataTable dt = this.GetTable();
            foreach (DataRow dr in dt.Rows)
            {
                XmlEn en = this.GetNewEntity;
                en.Row = new Row(dt, dr);
                this.Add(en);
            }

           // BP.DA.Cash.AddObj(this.ToString(), Depositary.Application, this);
            return dt.Rows.Count;
        }
       

        public int RetrieveByLength(string key, int len)
        {
            this.Clear(); //�����е���Ϣ
            DataTable dt = this.GetTable();
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[key].ToString().Length == len)
                {
                    XmlEn en = this.GetNewEntity;
                    en.Row = new Row(dt, dr);
                    this.Add(en);
                    i++;
                }
            }
            return i;
        }

        /// <summary>
        /// ���ռ�ֵ��ѯ
        /// </summary>
        /// <param name="key">Ҫ��ѯ�Ľ�</param>
        /// <param name="val">ֵ</param>
        /// <returns>���ز�ѯ�ĸ���</returns>
        public int RetrieveBy(string key, object val)
        {
            if (val == null)
                return 0;

            this.Clear(); //�����е���Ϣ
            DataTable dt = this.GetTable();
            if (dt == null)
                throw new Exception("@������" + this.GetNewEntity.ToString() + " File= " + this.File + " Table=" + this.TableName + " ��û��ȡ�����ݡ�");

            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[key].ToString() == val.ToString())
                {
                    XmlEn en = this.GetNewEntity;
                    en.Row = new Row(dt, dr);
                    this.Add(en);
                    i++;
                }
            }
            return i;
        }

        public int RetrieveBy(string key, object val, string orderByAttr)
        {
            DataTable dt = this.GetTable();
            DataView dv = new DataView(dt, orderByAttr, orderByAttr, DataViewRowState.Unchanged);

            this.Clear(); //�����е���Ϣ.
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                if (dr[key].ToString() == val.ToString())
                {
                    XmlEn en = this.GetNewEntity;
                    en.Row = new Row(dt, dr);
                    this.Add(en);
                    i++;
                }
            }
            return i;
        }
        #endregion

        #region ��������
        public XmlEn Find(string key, object val)
        {
            foreach (XmlEn en in this)
            {
                if (en.GetValStringByKey(key) == val.ToString())
                    return en;
            }
            return null;

        }
        public bool IsExits(string key, object val)
        {
            foreach (XmlEn en in this)
            {
                if (en.GetValStringByKey(key) == val.ToString())
                    return true;
            }
            return false;
        }
        #endregion


        #region  ���� ��������
        public XmlEn GetEnByKey(string key, string val)
        {
            foreach (XmlEn en in this)
            {
                if (en.GetValStringByKey(key) == val)
                    return en;
            }
            return null;

        }
        /// <summary>
        /// ����λ��ȡ������
        /// </summary>
        public XmlEn this[int index]
        {
            get
            {
                return (XmlEn)this.InnerList[index];
            }
        }
        /// <summary>
        /// ��ȡ����
        /// </summary>
        public XmlEn this[string key, string val]
        {
            get
            {
                foreach (XmlEn en in this)
                {
                    if (en.GetValStringByKey(key) == val)
                        return en;
                }
                throw new Exception("��[" + this.TableName + "," + this.File + "," + this.ToString() + "]û���ҵ�key=" + key + ", val=" + val + "��ʵ����");
            }
        }
        /// <summary>
        /// ����һ��xml en to Ens.
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public virtual int Add(XmlEn entity)
        {
            return this.InnerList.Add(entity);
        }
        #endregion


        #region ����ʵ��xml ��Ϣ������.
        public abstract XmlEn GetNewEntity 
        { get; }
        /// <summary>
        /// �ļ�
        /// </summary>
        protected string _File = null;
        /// <summary>
        /// ��ȡ�����ڵ�xml file λ��.
        /// </summary>
        public abstract string File
        {
            get;
        }
        /// <summary>
        /// 
        /// </summary>
        protected string _TableName = null;
        /// <summary>
        /// ���������(����һ��xml�ļ�����n��Table.)
        /// </summary>
        public abstract string TableName
        {
            get;
        }
        #endregion
    }



}
