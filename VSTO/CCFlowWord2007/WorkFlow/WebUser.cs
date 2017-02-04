using System;
using System.IO;
using BP.DA;
using BP.WF;
using BP.Port;
using CCFlowWord2007.WorkFlow;

namespace BP.Web
{
    public class WebUser
    {
        #region 个人的属性
        private static int _FK_Node;
        /// <summary>
        /// 执行什么？
        /// </summary>
        public static string DoWhat;
        public static bool isLogin;
        /// <summary>
        /// 人员编号
        /// </summary>
        public static string No;
        public static string Name;
        public static string Pass;
        public static Dept HisDept;
        public static string FK_DeptOfShiJu
        {
            get
            {
                return FK_Dept.Substring(0, 2);
            }
        }
       
        public static string FK_Dept;
        public static string FK_DeptName;
        public static Int64 WorkID;
        public static Int64 FID=0;
        private static string _FK_Flow;
        private static string _FK_FlowName;
        private static string _FK_NodeName;
        /// <summary>
        /// 当前流程节点
        /// </summary>
        public static WFNode CurrentNode;
        public static string FK_Flow
        {
            get
            {
                return _FK_Flow;
            }
            set
            {
                _FK_Flow = value;
            }
        }
        public static string FK_FlowName
        {
            get
            {
                return _FK_FlowName;
            }
            set
            {
                _FK_FlowName = value;
            }
        }
        /// <summary>
        /// 节点名称
        /// </summary>
        public static string FK_NodeName
        {
            get
            {
                return _FK_NodeName;
            }
            set
            {
                _FK_NodeName = value;
            }
        }

        public static int FK_Node
        {
            get
            {
                if (FK_Flow == null)
                    return 0;

                if (_FK_Node == 0 || _FK_Node == 1)
                {
                    _FK_Node = int.Parse(FK_Flow + "01");
                }

                return _FK_Node;
            }
            set
            {
                _FK_Node = value;
            }
        }

        public static bool IsStartNode
        {
            get
            {
                return int.Parse(FK_Flow + "01") == FK_Node;
            }
        }
        public static bool IsSavePass;
        public static bool IsSaveInfo;

        public static string DoType;
        public static string SID;
        private static Work _HisWork;
        public static Work HisWork
        {
            get
            {
                if (WorkID == 0)
                    return null;

                return _HisWork ?? (_HisWork = new Work(FK_Node, WorkID));
            }
            set
            {

                _HisWork = value;
                WorkID = _HisWork.OID;
            }
        }
        public static CCFlowWord2007.Ribbon1 HisRib;
        #endregion

        #region 公共的属性
        public static string AppServWorkID
        {
            get
            {
                return DBAccess.GetWebConfigByKey("WorkID");
            }
        }
        public static string AppServFK_Flow
        {
            get
            {
                return DBAccess.GetWebConfigByKey("FK_Flow");
            }
        }
        public static string AppServFtpUser
        {
            get
            {
                return DBAccess.GetWebConfigByKey("FtpUser");
            }
        }
        public static string AppServFtpPass
        {
            get
            {
                return DBAccess.GetWebConfigByKey("FtpPass");
            }
        }
        #endregion

        #region Methods

        public static bool LoadProfile()
        {
            if (Profile.IsExitProfile)
            {
                Profile.ProfileDoc = null;
                try
                {
                    if (Directory.Exists(Glo.PathOfTInstall) == false)
                        Directory.CreateDirectory(Glo.PathOfTInstall);

                    No = Profile.GetValByKey("No");
                    Name = Profile.GetValByKey("Name");
                    FK_Dept = Profile.GetValByKey("FK_Dept");

                    //FK_Node = Profile.GetValIntByKey("FK_Node");
                    //WorkID = Profile.GetValIntByKey("WorkID");
                    //FK_Flow = Profile.GetValByKey("FK_Flow");

                    FK_Node = 0; // Profile.GetValIntByKey("FK_Node");
                    WorkID = 0; // Profile.GetValIntByKey("WorkID");
                    FK_Flow = null; // nuProfile.GetValByKey("FK_Flow");

                    switch (DoType)
                    {
                        case DoTypeConst.DoStartFlowByTemple: //我要打开ppt.
                        case DoTypeConst.DoStartFlow: //我要打开ppt.
                            break;
                        default:
                            break;
                    }


                    if (DoType != null)
                    {
                        DoType = "";  // 清除标记。
                        WriterIt();
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show("@加载个人信心时间出现错误，有可能个人信息文件遭到破坏：" + ex.Message);
                    return false;
                }
            }
            return false;
        }

        /// <summary>
        /// 登陆
        /// </summary>
        /// <param name="emp"></param>
        public static void Sigin(Emp emp)
        {
            isLogin = true;
            No = emp.No;
            Name = emp.Name;
            FK_Dept = emp.FK_Dept;
            DoType = "";
            CurrentNode = null; //置空

            GetFtpInfomation();

            WriterIt();
        }

        /// <summary>
        /// 登出
        /// </summary>
        public static void SignOut()
        {
            No = null;
            Name = null;
            Pass = null;
            WorkID = 0;
            FK_Dept = null;
            FK_DeptName = null;
            FK_Flow = null;
            FK_Node = 0;
            HisDept = null;
            isLogin = false;
            IsSaveInfo = false;
            IsSavePass = false;
            SID = null;

            if (File.Exists(Glo.Profile))
                File.Delete(Glo.Profile);
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        public static void WriterIt()
        {
            WriterIt(DoType, FK_Flow, FK_Node, WorkID);
        }

        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="dotype"></param>
        /// <param name="fk_flow"></param>
        /// <param name="fk_node"></param>
        /// <param name="workid"></param>
        public static void WriterIt(string dotype, string fk_flow, int fk_node, Int64 workid)
        {
            if (IsSavePass == false)
                return;

            string strLocalPath = Glo.PathOfTInstall;
            if (Directory.Exists(strLocalPath) == false)
                Directory.CreateDirectory(strLocalPath);

            string strContent = "@No=" + No + "@Name=" + Name + "@FK_Dept=" +
                FK_Dept + "@FK_DeptName=" + FK_DeptName
                + "@WorkID=" + workid + "@FK_Flow=" +
             fk_flow + "@FK_Node=" + fk_node + "@DoType=" + dotype;
            File.WriteAllText(Glo.Profile, strContent);
        }

        public static void WriterCookes()
        {
            if (IsSaveInfo == false)
                return;

            string strLocalPath = Glo.PathOfTInstall;
            if (Directory.Exists(strLocalPath) == false)
                Directory.CreateDirectory(strLocalPath);

            string strContent = "@No=" + No + "@Name=" + Name;
            File.WriteAllText(Glo.ProfileLogin, strContent);
        }

        /// <summary>
        /// 获取FTP信息
        /// </summary>
        private static void GetFtpInfomation()
        {
            Glo.FtpIP = DBAccess.GetWebConfigByKey("FtpIP");
            Glo.FtpUser = DBAccess.GetWebConfigByKey("FtpUser");
            Glo.FtpPass = DBAccess.GetWebConfigByKey("FtpPass");
        }

        /// <summary>
        /// 获取流程节点信息
        /// </summary>
        /// <param name="nodeId">节点id</param>
        public static void RetrieveWFNode(int nodeId)
        {
            try
            {
                CurrentNode = new WFNode(nodeId);
            }
            catch (Exception ex)
            {
                throw new Exception("获取流程节点时出现错误", ex);
            }
        }

        #endregion
    }
}
