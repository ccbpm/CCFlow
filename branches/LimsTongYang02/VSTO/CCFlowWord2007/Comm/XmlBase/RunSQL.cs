using System;
using System.Collections;
using System.Data;
using BP.DA;
using BP.En;
using BP.XML;
namespace BP.HTTP.Xml
{
	/// <summary>
	/// ����
	/// </summary>
    public class RunSQLAttr
    {
        public const string Url = "Url";
        public const string FK_Img = "FK_Img";
        public const string Para = "Para";
        public const string ValType = "ValType";
        public const string Val = "Val";
        public const string Encode = "Encode";
    }
	/// <summary>
	/// AD ��ժҪ˵����
	/// ���˱�ŵ�����Ԫ��
	/// 1������ AD ��һ����ϸ��
	/// 2������ʾһ������Ԫ�ء�
	/// </summary>
	public class RunSQL:XmlEnNoName
	{
		#region ����
		public RunSQL()
		{
		}
		/// <summary>
		/// ��ȡһ��ʵ��
		/// </summary>
        public override XmlEns GetNewEntities
        {
            get
            {
                return new RunSQLs();
            }
        }
		#endregion
	}
	/// <summary>
    /// RunSQL
	/// </summary>
    public class RunSQLs : XmlEns
    {
        #region ����
        /// <summary>
        /// ���˱�ŵ�����Ԫ��
        /// </summary>
        public RunSQLs() { }
     
        #endregion

        #region ��д�������Ի򷽷���
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override XmlEn GetNewEntity
        {
            get
            {
                return new RunSQL();
            }
        }
        /// <summary>
        /// �ļ�
        /// </summary>
        public override string File
        {
            get
            {
                return SystemConfig.PathOfWebApp + "\\RunSQL\\RunSQL.xml";
            }
        }
        /// <summary>
        /// �������
        /// </summary>
        public override string TableName
        {
            get
            {
                return "RunSQL";
            }
        }
        
        #endregion
    }
}
