using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo.YS
{
    /// <summary>
    /// ��Ŀ����
    /// </summary>
    public class KMAttr : EntityTreeAttr
    {
    }
    /// <summary>
    ///  ��Ŀ
    /// </summary>
    public class KM : EntityTree
    {
        #region ���췽��
        /// <summary>
        /// ��Ŀ
        /// </summary>
        public KM()
        {
        }
        /// <summary>
        /// ��Ŀ
        /// </summary>
        /// <param name="_No"></param>
        public KM(string _No) : base(_No) { }
        #endregion

        /// <summary>
        /// ��ĿMap
        /// </summary>
        public override Map EnMap
        {
            get
            {
                if (this._enMap != null)
                    return this._enMap;

                Map map = new Map("Demo_YS_KM", "��Ŀ");
                map.Java_SetCodeStruct("2");
                map.IsAllowRepeatName = false;

                map.Java_SetDepositaryOfEntity( Depositary.Application);
                map.Java_SetDepositaryOfMap( Depositary.Application);

                map.AddTBStringPK(KMAttr.No, null, "���", true, true, 1, 10, 20);
                map.AddTBString(KMAttr.Name, null, "����", true, false, 0, 100, 30);
                map.AddTBString(KMAttr.ParentNo, null, "���ڵ�No", false, false, 0, 100, 30);

                map.AddTBInt(KMAttr.Idx, 0, "Idx", false, false);

                this._enMap = map;
                return this._enMap;
            }
        }

        
    }
    /// <summary>
    /// ��Ŀ
    /// </summary>
    public class KMs : EntitiesTree
    {
        /// <summary>
        /// ��Ŀs
        /// </summary>
        public KMs() { }
        /// <summary>
        /// �õ����� Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new KM();
            }

        }
        /// <summary>
        /// ��Ŀs
        /// </summary>
        /// <param name="no">ss</param>
        /// <param name="name">anme</param>
        public void AddByNoName(string no, string name)
        {
            KM en = new KM();
            en.No = no;
            en.Name = name;
            this.AddEntity(en);
        }
        public override int RetrieveAll()
        {
            int i = base.RetrieveAll();
            if (i == 0)
            {
                KM fs = new KM();
                fs.Name = "������";
                fs.No = "01";
                fs.Insert();

                fs = new KM();
                fs.Name = "�칫��";
                fs.No = "02";
                fs.Insert();
                i = base.RetrieveAll();
            }

            return i;
        }
    }
}
