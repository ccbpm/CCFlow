using System;
using System.Data;
using System.Collections;
using BP.DA;
using BP.En;
using BP.Port;

namespace BP.WF.Template
{
    /// <summary>
    /// 泳道属性
    /// </summary>
    public class LaneAttr : BP.En.EntityOIDNameAttr
    {
        #region 基本属性
        /// <summary>
        /// 节点
        /// </summary>
        public const string FK_Flow = "FK_Flow";
        /// <summary>
        /// 到达目标
        /// </summary>
        public const string Target = "Target";
        /// <summary>
        /// 标题
        /// </summary>
        public const string Title = "Title";
        /// <summary>
        /// url
        /// </summary>
        public const string NodeIDs = "NodeIDs";
        /// <summary>
        /// 顺序号
        /// </summary>
        public const string Idx = "Idx";
        /// <summary>
        /// 显示在那里？
        /// </summary>
        public const string ShowWhere = "ShowWhere";
        /// <summary>
        /// 在工作处理器显示
        /// </summary>
        public const string IsMyFlow = "IsMyFlow";
        /// <summary>
        ///  在工作查看器显示
        /// </summary>
        public const string IsMyView = "IsMyView";
        /// <summary>
        ///  在树形表单显示
        /// </summary>
        public const string IsMyTree = "IsMyTree";
        /// <summary>
        ///  在抄送功能显示
        /// </summary>
        public const string IsMyCC = "IsMyCC";
        /// <summary>
        /// IconPath 图片附件
        /// </summary>
        public const string IconPath = "IconPath";
        /// 执行类型
        /// </summary>
        public const string ExcType = "ExcType";
        #endregion
    }
    /// <summary>
    /// 泳道.
    /// </summary>
    public class Lane : EntityOID
    {
        #region 基本属性
        /// <summary>
        /// UI界面上的访问控制
        /// </summary>
        public override UAC HisUAC
        {
            get
            {
                UAC uac = new UAC();
                uac.OpenForAdmin();// 2020.5.15zsy修改
                return uac;
            }
        }
        /// <summary>
        /// 泳道的事务编号
        /// </summary>
        public string FK_Flow
        {
            get
            {
                return this.GetValStringByKey(LaneAttr.FK_Flow);
            }
            set
            {
                SetValByKey(LaneAttr.FK_Flow, value);
            }
        }
        /// <summary>
        /// 节点s
        /// </summary>
        public string NodeIDs
        {
            get
            {
                return this.GetValStringByKey(LaneAttr.NodeIDs);
            }
            set
            {
                SetValByKey(LaneAttr.NodeIDs, value);
            }
        }

        public int Idx
        {
            get
            {
                return this.GetValIntByKey(LaneAttr.Idx);
            }
            set
            {
                SetValByKey(LaneAttr.Idx, value);
            }
        }
        public string Title
        {
            get
            {
                return this.GetValStringByKey(LaneAttr.Title);
            }
            set
            {
                SetValByKey(LaneAttr.Title, value);
            }
        }
        #endregion

        #region 构造函数
        /// <summary>
        /// 泳道
        /// </summary>
        public Lane() { }
        /// <summary>
        /// 泳道
        /// </summary>
        /// <param name="_oid">泳道ID</param>	
        public Lane(int oid)
        {
            this.OID = oid;
            this.Retrieve();
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

                Map map = new Map("WF_Lane", "泳道");

                map.AddTBIntPKOID();
                map.AddTBString(LaneAttr.FK_Flow, null, "流程No", true, false, 0, 100, 100, true);
                map.AddTBString(LaneAttr.Title, null, "标题", true, false, 0, 100, 100, true);
                map.AddTBString(LaneAttr.NodeIDs, null, "节点IDs", true, false, 0, 500, 300, true);
                map.AddTBInt(LaneAttr.Idx, 0, "顺序", true, false);

                this._enMap = map;
                return this._enMap;
            }
        }
        #endregion
    }
    /// <summary>
    /// 泳道集合
    /// </summary>
    public class Lanes : EntitiesOID
    {
        #region 方法
        /// <summary>
        /// 得到它的 Entity 
        /// </summary>
        public override Entity GetNewEntity
        {
            get
            {
                return new Lane();
            }
        }
        #endregion

        /// <summary>
        /// 按照流程编号查询.
        /// </summary>
        /// <param name="no"></param>
        /// <returns></returns>
        public string RetrieveByFlowNo(string no)
        {
            int i = this.Retrieve(LaneAttr.FK_Flow, no);
            if (i == 0)
                InitData(no);

            return this.ToJson();
        }

        #region 构造方法
        /// <summary>
        /// 泳道集合
        /// </summary>
        public Lanes()
        {
        }
        /// <summary>
        /// 泳道集合.
        /// </summary>
        /// <param name="flowNo"></param>
        public Lanes(string flowNo)
        {
            int i = this.Retrieve(LaneAttr.FK_Flow, flowNo);
            if (i == 0)
                InitData(flowNo);
        }
        /// <summary>
        /// 初始化数据.
        /// </summary>
        public void InitData(string flowNo)
        {
            Nodes nds = new Nodes(flowNo);

            //Node mynd = new Node();
           // mynd.HisDeliveryWay

            #region 1.求出来最大的集合,按照节点名字合并.
            //如果名称相同,求出最大的集合。
            string sql = "SELECT DISTINCT Name FROM WF_Node WHERE FK_Flow='" + flowNo + "'  ";
            DataTable dt = DBAccess.RunSQLReturnTable(sql);

            Lanes ens = new Lanes();
            int idx = 0;
            foreach (DataRow dr in dt.Rows)
            {
                string name = dr[0].ToString();
                Lane lan = new Lane();
                lan.Title = name;
                lan.FK_Flow = flowNo;
                lan.Idx = idx++;

                //获取相同名字的节点ID.
                string nodeIDs = "";
                foreach (Node nd in nds)
                {
                    if (nd.Name.Equals(name) == true)
                        nodeIDs += nd.NodeID + ",";
                }

                lan.NodeIDs = nodeIDs;
                lan.Insert();
                ens.AddEntity(lan);
            }
            #endregion 求出来最大的集合.

            #region 按照 NodeID 排序. 
            sql = "SELECT OID FROM WF_Lane WHERE FK_Flow='"+flowNo+"' ORDER BY NodeIDs";
            dt = DBAccess.RunSQLReturnTable(sql);
            idx = 0;
            foreach (DataRow dr in dt.Rows)
            {
                idx++;
                sql = "UPDATE WF_Lane SET Idx="+idx+" WHERE OID="+dr[0].ToString();
                DBAccess.RunSQL(sql);
            }
            ens.Retrieve("FK_Flow", flowNo, "Idx");
            #endregion 按照 NodeID 排序.

            #region 合并-与开始节点相同.
            sql = "SELECT NodeID,Name FROM WF_Node WHERE FK_Flow='"+flowNo+"' AND DeliveryWay=7";
            dt = DBAccess.RunSQLReturnTable(sql);
            Lane lanfirst = ens[0] as Lane;
            foreach (DataRow dr in dt.Rows)
            {
                lanfirst.NodeIDs += "" + dr[0].ToString() + ",";
                //删除原来的分组.
                DBAccess.RunSQL("DELETE FROM WF_Lane WHERE NodeIDs='" + dr[0].ToString() + ",'");
            }
            lanfirst.Update();
            #endregion 合并-与开始节点相同.


            #region 合并按[岗位]计算，并且岗位集合相同的节点，进行合并. 
            //按照岗位计算有关的节点.
            sql = "SELECT DISTINCT HisStas FROM WF_Node WHERE FK_Flow='" + flowNo + "' AND DeliveryWay IN (0,9,10,11,14,20)";
            dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                string stas = dr[0].ToString();
                if (BP.DA.DataType.IsNullOrEmpty(stas)==true)
                    continue;

                //获得该岗位下的节点集合.
                sql = "SELECT NodeID,Name FROM WF_Node WHERE FK_Flow='" + flowNo + "' AND HisStas='" + stas + "' AND DeliveryWay IN (0,9,10,11,14,20)  ORDER BY NodeID,Step ";
                DataTable myDT = DBAccess.RunSQLReturnTable(sql);
                if (myDT.Rows.Count <= 1)
                    continue;

                //求出来他们的IDs.
                string nodeIDs = "";
                foreach (DataRow drID in myDT.Rows)
                {
                    nodeIDs += drID[0].ToString() + ",";
                }

                //求出来第1个lan根据分组的名字.
                Lane lanFirst = null;
                string firstNodeID = myDT.Rows[0][0].ToString();
                string firstNodeName = myDT.Rows[0][1].ToString();
                foreach (Lane lan in ens)
                {
                    if (lan.Title.Equals(firstNodeName) == true)
                    {
                        lanFirst = lan;
                        break;
                    }
                }
                if (lanFirst == null)
                    throw new Exception("err@不应该出现的异常.");

                lanFirst.NodeIDs = nodeIDs;
                lanFirst.Update();

                //删除其余的数据.
                foreach (DataRow mydr in myDT.Rows)
                {
                    string nodeid = mydr[0].ToString();
                    DBAccess.RunSQL("DELETE FROM WF_Lane WHERE NodeIDs='"+ nodeid + ",'");
                }
            }
            #endregion 合并按照岗位计算，并且岗位集合相同的节点，进行合并.

            #region 合并按[部门]计算，并且集合相同的节点，进行合并. 
            //按照岗位计算有关的节点.
            sql = "SELECT DISTINCT HisDeptStrs FROM WF_Node WHERE FK_Flow='" + flowNo + "' AND DeliveryWay=1 ";
            dt = DBAccess.RunSQLReturnTable(sql);
            foreach (DataRow dr in dt.Rows)
            {
                string stas = dr[0].ToString();
                if (BP.DA.DataType.IsNullOrEmpty(stas) == true)
                    continue;

                //获得该岗位下的节点集合.
                sql = "SELECT NodeID,Name FROM WF_Node WHERE FK_Flow='" + flowNo + "' AND HisDeptStrs='" + stas + "' AND DeliveryWay=1  ORDER BY NodeID,Step ";
                DataTable myDT = DBAccess.RunSQLReturnTable(sql);
                if (myDT.Rows.Count <= 1)
                    continue;

                //求出来他们的IDs.
                string nodeIDs = "";
                foreach (DataRow drID in myDT.Rows)
                {
                    nodeIDs += drID[0].ToString() + ",";
                }

                //求出来第1个lan根据分组的名字.
                Lane lanFirst = null;
                string firstNodeID = myDT.Rows[0][0].ToString();
                string firstNodeName = myDT.Rows[0][1].ToString();
                foreach (Lane lan in ens)
                {
                    if (lan.Title.Equals(firstNodeName) == true)
                    {
                        lanFirst = lan;
                        break;
                    }
                }
                if (lanFirst == null)
                    throw new Exception("err@不应该出现的异常.");

                lanFirst.NodeIDs = nodeIDs;
                lanFirst.Update();

                //删除其余的数据.
                foreach (DataRow mydr in myDT.Rows)
                {
                    string nodeid = mydr[0].ToString();
                    DBAccess.RunSQL("DELETE FROM WF_Lane WHERE NodeIDs='" + nodeid + ",'");
                }
            }
            #endregion 合并按照 部门 计算，并且 部门 集合相同的节点，进行合并.

        }
        #endregion

        #region 为了适应自动翻译成java的需要,把实体转换成List 
        /// <summary>
        /// 转化成 java list,C#不能调用.
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.IList<Lane> ToJavaList()
        {
            return (System.Collections.Generic.IList<Lane>)this;
        }

        /// <summary>
        /// 转化成list
        /// </summary>
        /// <returns>List</returns>
        public System.Collections.Generic.List<Lane> Tolist()
        {
            System.Collections.Generic.List<Lane> list = new System.Collections.Generic.List<Lane>();
            for (int i = 0; i < this.Count; i++)
            {
                list.Add((Lane)this[i]);
            }
            return list;
        }
        #endregion 为了适应自动翻译成java的需要,把实体转换成List.
    }
}
