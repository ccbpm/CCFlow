using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.Demo.YS
{
	/// <summary>
	/// ��Ŀvs��Ŀ
	/// </summary>
	public class PrjKMAttr  
	{
		#region ��������
		/// <summary>
		/// ������ԱID
		/// </summary>
		public const  string FK_Prj="FK_Prj";
		/// <summary>
		/// ������λ
		/// </summary>
		public const  string FK_KM="FK_KM";		 
		#endregion	
	}
	/// <summary>
    /// ��Ŀvs��Ŀ ��ժҪ˵����
	/// </summary>
    public class PrjKM : Entity
    {
        #region ��������
        /// <summary>
        /// UI�����ϵķ��ʿ���
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;

            }
        }
        /// <summary>
        /// ������ԱID
        /// </summary>
        public string FK_Prj
        {
            get
            {
                return this.GetValStringByKey(PrjKMAttr.FK_Prj);
            }
            set
            {
                SetValByKey(PrjKMAttr.FK_Prj, value);
            }
        }
        public string FK_KMT
        {
            get
            {
                return this.GetValRefTextByKey(PrjKMAttr.FK_KM);
            }
        }
        /// <summary>
        ///������λ
        /// </summary>
        public string FK_KM
        {
            get
            {
                return this.GetValStringByKey(PrjKMAttr.FK_KM);
            }
            set
            {
                SetValByKey(PrjKMAttr.FK_KM, value);
            }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// ������Ŀvs��Ŀ
        /// </summary> 
        public PrjKM() { }
        /// <summary>
        /// ������Ա������λ��Ӧ
        /// </summary>
        /// <param name="FK_Prj">������ԱID</param>
        /// <param name="FK_KM">������λ���</param> 	
        public PrjKM(string FK_Prj, string FK_KM)
        {
            this.FK_Prj = FK_Prj;
            this.FK_KM = FK_KM;
            this.Retrieve();
        }
        /// <summary>
        /// ��д���෽��
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Demo_YS_PrjKM", "��Ŀvs��Ŀ");
                map.Java_SetEnType(EnType.Dot2Dot);

              //  map.AddDDLEntitiesPK(PrjKMAttr.FK_Prj, null, "����Ա", new Emps(), true);
                map.AddTBStringPK(PrjKMAttr.FK_Prj, null, "��Ŀ", true, false, 0, 100, 100);
                map.AddDDLEntitiesPK(PrjKMAttr.FK_KM, null, "��Ŀ", new KMs(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// ��Ŀvs��Ŀ 
	/// </summary>
	public class PrjKMs : Entities
	{
		#region ����
		/// <summary>
		/// ������Ŀvs��Ŀ
		/// </summary>
		public PrjKMs()
		{
		}
		/// <summary>
		/// ������Ա�빤����λ����
		/// </summary>
		public PrjKMs(string stationNo)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(PrjKMAttr.FK_KM, stationNo);
			qo.DoQuery();
		}		 
		#endregion

		#region ����
		/// <summary>
		/// �õ����� Entity 
		/// </summary>
		public override Entity GetNewEntity
		{
			get
			{
				return new PrjKM();
			}
		}	
		#endregion 
	}
}
