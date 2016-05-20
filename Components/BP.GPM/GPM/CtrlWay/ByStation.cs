using System;
using System.Data;
using BP.DA;
using BP.En;

namespace BP.GPM
{
	/// <summary>
	/// ����λ
	/// </summary>
	public class ByStationAttr  
	{
		#region ��������
		/// <summary>
		/// ���ƶ���
		/// </summary>
		public const  string RefObj="RefObj";
		/// <summary>
		/// ������λ
		/// </summary>
		public const  string FK_Station="FK_Station";		 
		#endregion	
	}
	/// <summary>
    /// ����λ
	/// </summary>
    public class ByStation : Entity
    {
        #region ����Ȩ�޿���
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForSysAdmin();
                return uac;
            }
        }
        #endregion ����Ȩ�޿���

        #region ��������
        /// <summary>
        /// ���ƶ���
        /// </summary>
        public string RefObj
        {
            get
            {
                return this.GetValStringByKey(ByStationAttr.RefObj);
            }
            set
            {
                SetValByKey(ByStationAttr.RefObj, value);
            }
        }
        public string FK_StationT
        {
            get
            {
                return this.GetValRefTextByKey(ByStationAttr.FK_Station);
            }
        }
        /// <summary>
        ///������λ
        /// </summary>
        public string FK_Station
        {
            get
            {
                return this.GetValStringByKey(ByStationAttr.FK_Station);
            }
            set
            {
                SetValByKey(ByStationAttr.FK_Station, value);
            }
        }
        #endregion

        #region ���캯��
        /// <summary>
        /// ����λ
        /// </summary> 
        public ByStation() { }
        /// <summary>
        /// ����λ
        /// </summary>
        /// <param name="RefObj"></param>
        /// <param name="FK_Station"></param>
        public ByStation(string RefObj, string FK_Station)
        {
            this.RefObj = RefObj;
            this.FK_Station = FK_Station;
            if (this.Retrieve() == 0)
                this.Insert();
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

                Map map = new Map("GPM_ByStation");
                map.EnDesc = "����λ";
                map.EnType = EnType.Dot2Dot;

                map.AddTBStringPK(ByStationAttr.RefObj, null, "���ƶ���", false, false, 1, 15, 1);
                map.AddDDLEntitiesPK(ByStationAttr.FK_Station, null, "������λ", new Stations(), true);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
	/// <summary>
    /// ����λ 
	/// </summary>
	public class ByStations : Entities
        /// <summary>
        /// ת���� java list,C#���ܵ���.
        /// </summary>
        /// <returns>List</returns>
	{
		#region ����
		/// <summary>
        /// ����λ
		/// </summary>
		public ByStations()
		{
		}
		/// <summary>
        /// ����λs
		/// </summary>
		public ByStations(string stationNo)
		{
			QueryObject qo = new QueryObject(this);
			qo.AddWhere(ByStationAttr.FK_Station, stationNo);
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
				return new ByStation();
			}
		}	
		#endregion 

        #region Ϊ����Ӧ�Զ������java����Ҫ,��ʵ��ת����List.
        public System.Collections.Generic.IList<ByStation> ToJavaList()
        {
            return (System.Collections.Generic.IList<ByStation>)this;
        }
        /// <summary>
        /// ת����list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<ByStation> Tolist()
        {
            System.Collections.Generic.List<ByStation> list = new System.Collections.Generic.List<ByStation>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((ByStation)this[i]);
            }
            return list;
        }
        #endregion Ϊ����Ӧ�Զ������java����Ҫ,��ʵ��ת����List.
	}
}
