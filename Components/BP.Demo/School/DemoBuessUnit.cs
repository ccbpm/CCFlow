using System;
using System.Data;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.Demo
{
    /// <summary>
    /// ҵ��ԪDemo
    /// </summary>
    public class DemoBuessUnit : BP.Sys.BuessUnitBase
    {
        public override string Title
        {
            get
            {
                return "ҵ��Ԫ����:BP.Demo.DemoBuessUnit";
            }
        }
        public override string DoIt()
        {
            //��ҵ���߼�д������,ͨ��workid��������ø��������.

            //������.
            return "ִ�гɹ�.WorkID:" + this.WorkID;
            // return base.DoIt();
        }
    }

}
