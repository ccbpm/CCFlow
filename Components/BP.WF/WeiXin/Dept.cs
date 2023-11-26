using BP.DA;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BP.GPM.WeiXin
{
    /// <summary>
    /// 部门列表
    /// </summary>
    public class DeptList
    {
        #region 部门属性.
        /// <summary>
        /// 返回码
        /// </summary>
        public int errcode { get; set; }
        /// <summary>
        /// 对返回码的文本描述内容
        /// </summary>
        public string errmsg { get; set; }
        /// <summary>
        /// 部门列表数据
        /// </summary>
        public List<DeptEntity> department { get; set; }
        #endregion 部门属性.

        public DeptList()
        {
        }
        /// <summary>
        /// 查询所有的部门
        /// </summary>
        /// <returns></returns>
        public int RetrieveAll()
        {
            string access_token = BP.GPM.WeiXin.WeiXinEntity.getAccessToken();
            string url = "https://qyapi.weixin.qq.com/cgi-bin/department/list?access_token=" + access_token;

            //读取数据.
            string str = DataType.ReadURLContext(url, 9999, null);
            DeptList departMentList = BP.Tools.FormatToJson.ParseFromJson<DeptList>(str);

            if (departMentList.errcode != 0)
                throw new Exception("err@获得部门信息错误:code" + departMentList.errcode + ",Msg=" + departMentList.errmsg);


            this.errcode = departMentList.errcode;
            this.errmsg = departMentList.errmsg;
            this.department = departMentList.department;

            return this.department.Count;
        }
    }
    /// <summary>
    /// 部门信息
    /// </summary>
    public class DeptEntity
    {
        #region 属性.
        /// <summary>
        ///  部门id 
        /// </summary>
        public string id { get; set; }
        /// <summary>
        /// 部门名称 
        /// </summary>
        public string name { get; set; }
        /// <summary>
        ///  父亲部门id。根部门为1 
        /// </summary>
        public string parentid { get; set; }
        /// <summary>
        ///  在父部门中的次序值。order值小的排序靠前
        /// </summary>
        public string order { get; set; }
        #endregion 属性.

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="id"></param>
        public DeptEntity(string id)
        {
        }
    }
}
