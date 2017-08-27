using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using BP.En;
using BP.DA;
using BP.Web;
using BP.Web.Controls;
using BP.Sys;
using BP.Tools;

namespace CCFlow.WF.Comm
{
    public partial class SearchEUI : System.Web.UI.Page
    {
        /// <summary>
        /// 获取传入参数
        /// </summary>
        /// <param name="param">参数名</param>
        /// <returns></returns>
        public string getUTF8ToString(string param)
        {
            return HttpUtility.UrlDecode(Request[param], System.Text.Encoding.UTF8);
        }
        /// <summary>
        /// 实体名称
        /// </summary>
        public  string EnsName
        {
            get
            {
                return getUTF8ToString("EnsName");
            }
        }
        /// <summary>
        ///实体名称
        /// </summary>
        public  string EnName
        {
            get
            {
                Entity en = GetEntityByEnName(EnsName);
                if (en != null)
                    return en.ClassID;
                return "";
            }
        }
        /// <summary>
        /// 解释
        /// </summary>
        public string EnsDesc
        {
            get
            {
                Entity en = GetEntityByEnName(EnsName);
                if (this.EnsName == null || en == null)
                    return "实体名";
                return en.EnDesc;
            }
        }
        /// <summary>
        /// 实体主键
        /// </summary>
        public string EnPK
        {
            get
            {
                Entity en = GetEntityByEnName(EnsName);
                if (this.EnsName == null || en == null)
                    return "No";
                return en.PK;
            }
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            string method = string.Empty;
            //返回值
            string s_responsetext = string.Empty;
            if (string.IsNullOrEmpty(Request["method"]))
                return;

            method = Request["method"].ToString();
            switch (method)
            {
                case "getensgriddata"://获取列表数据
                    s_responsetext = GetEnsGridData();
                    break;
                case "delSelected"://获取列表数据
                    s_responsetext = EnsDel();
                    break;
                case "getuserrole"://获取是否具有发起流程及配置，added by liuxc,2017-1-5
                    s_responsetext = GetUserRole();
                    break;
            }
            if (string.IsNullOrEmpty(s_responsetext))
                s_responsetext = "";
            //组装ajax字符串格式,返回调用客户端
            Response.Charset = "UTF-8";
            Response.ContentEncoding = System.Text.Encoding.UTF8;
            Response.ContentType = "text/html";
            Response.Expires = 0;
            Response.Write(s_responsetext);
            Response.End();
        }
        protected override void OnInitComplete(EventArgs e)
        {
            InitToolBar();
            base.OnInitComplete(e);
        }

        protected void ToolBar1_ButtonClick(object sender, System.EventArgs e)
        {
            this.ToolBar1.SaveSearchState(this.EnsName, null);
        }
        /// <summary>
        /// 删除选择数据
        /// </summary>
        /// <returns></returns>
        private string EnsDel()
        {
            Entity en = GetEntityByEnName(EnsName);
            Entities ens = en.GetNewEntities;

            string GetDelOid = getUTF8ToString("GetDelOid");
            if (!string.IsNullOrEmpty(GetDelOid))
            {
                return en.Delete(en.PKField, GetDelOid).ToString();
            }
            return "0";
        }

        /// <summary>
        /// 获取用户是否可以发起流程/配置
        /// </summary>
        /// <returns></returns>
        private string GetUserRole()
        {
            Hashtable ht = new Hashtable();
            Entity en = GetEntityByEnName(EnsName);

            ht.Add("IsView", en.HisUAC.IsView);
            ht.Add("IsInsert", en.HisUAC.IsInsert);
            ht.Add("IsUpdate", en.HisUAC.IsUpdate);
            ht.Add("IsDelete", en.HisUAC.IsDelete);

            return BP.Tools.Json.ToJsonEntityModel(ht);
        }

        /// <summary>
        /// 获取ens数据
        /// </summary>
        /// <returns></returns>
        private string GetEnsGridData()
        {
            Entity en = GetEntityByEnName(EnsName);
            Entities ens = en.GetNewEntities;

            //总行数
            int RowCount = 0;
            try
            {
                //当前页
                string pageNumber = getUTF8ToString("pageNumber");
                int iPageNumber = string.IsNullOrEmpty(pageNumber) ? 1 : Convert.ToInt32(pageNumber);
                //每页多少行
                string pageSize = getUTF8ToString("pageSize");
                int iPageSize = string.IsNullOrEmpty(pageSize) ? 9999 : Convert.ToInt32(pageSize);
                string sortBy = getUTF8ToString("SortBy");
                string sortType = getUTF8ToString("SortType");

                if (string.IsNullOrWhiteSpace(sortType))
                    sortType = "ASC";

                QueryObject obj = new QueryObject(ens);
                obj = this.ToolBar1.GetnQueryObject(ens, en);
                RowCount = obj.GetCount();

                //增加排序,added by liuxc,2017-05-22
                obj.ClearOrderBy();

                if (!string.IsNullOrWhiteSpace(sortBy))
                {
                    string[] sortbys = sortBy.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);

                    if (sortType == "DESC")
                    {
                        if (sortbys.Length > 1)
                            obj.addOrderByDesc(sortbys[0], sortbys[1]);
                        else
                            obj.addOrderByDesc(sortbys[0]);
                    }
                    else
                    {
                        if (sortbys.Length > 1)
                            obj.addOrderBy(sortbys[0], sortbys[1]);
                        else
                            obj.addOrderBy(sortBy);
                    }
                }

                //查询
                obj.DoQuery(en.PK, iPageSize, iPageNumber, string.IsNullOrWhiteSpace(sortBy) ? en.PK : sortBy, sortType == "DESC");

                return Entitis2Json.ConvertEntitis2GridJsonAndData(ens, RowCount);
            }
            catch
            {
                try
                {
                    en.CheckPhysicsTable();
                }
                catch (Exception wx)
                {
                    BP.DA.Log.DefaultLogWriteLineError(wx.Message);
                }
            }
            return "{[]}";
        }

        //初始化查询
        private void InitToolBar()
        {
            Entity en = GetEntityByEnName(EnsName);
            if (en != null)
            {
                this.ToolBar1._AddSearchBtn = false;
                this.ToolBar1.InitByMapV2(en.EnMap, 1);
            }
        }

        /// <summary>
        /// 根据名称获取实体
        /// </summary>
        /// <param name="EnName"></param>
        /// <returns></returns>
        private Entity GetEntityByEnName(string EnName)
        {
            Entities _HisEns = null;
            Entity _HisEn = null;
            if (EnName != null)
            {
                _HisEns = ClassFactory.GetEns(EnName.Replace("#", ""));
                if (_HisEns == null)
                {
                    _HisEn = ClassFactory.GetEn(EnName.Replace("#", ""));
                    if (_HisEn == null)
                        throw new Exception("在此项目中没有找到命名空间及符合的类：" + EnName);
                }
                else
                    _HisEn = _HisEns.GetNewEntity;
            }
            return _HisEn;
        }
    }
}