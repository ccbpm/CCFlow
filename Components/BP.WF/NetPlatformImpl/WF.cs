using BP.Sys;
using BP.WF.HttpHandler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Services.Description;
using System.Xml.Schema;

namespace BP.WF.NetPlatformImpl
{
   public class WF_Admin_FoolFormDesigner
    {
        /// <summary>
        /// 获取webservice方法列表
        /// </summary>
        /// <param name="dbsrc">WebService数据源</param>
        /// <returns></returns>
        public static List<WSMethod> GetWebServiceMethods(SFDBSrc dbsrc)
        {
                if (dbsrc == null || string.IsNullOrWhiteSpace(dbsrc.IP))
                    return new List<WSMethod>();

                var wsurl = dbsrc.IP.ToLower();
                if (!wsurl.EndsWith(".asmx") && !wsurl.EndsWith(".svc"))
                    throw new Exception("@失败:" + dbsrc.No + " 中WebService地址不正确。");

                wsurl += wsurl.EndsWith(".asmx") ? "?wsdl" : "?singleWsdl";

                #region //解析WebService所有方法列表
                //var methods = new Dictionary<string, string>(); //名称Name，全称Text
                List<WSMethod> mtds = new List<WSMethod>();
                WSMethod mtd = null;
                var wc = new WebClient();
                var stream = wc.OpenRead(wsurl);
                var sd = ServiceDescription.Read(stream);
                var eles = sd.Types.Schemas[0].Elements.Values.Cast<XmlSchemaElement>();
                var s = new StringBuilder();
                XmlSchemaComplexType ctype = null;
                XmlSchemaSequence seq = null;
                XmlSchemaElement res = null;

                foreach (var ele in eles)
                {
                    if (ele == null) continue;

                    var resType = string.Empty;
                    var mparams = string.Empty;

                    //获取接口返回元素
                    res = eles.FirstOrDefault(o => o.Name == (ele.Name + "Response"));

                    if (res != null)
                    {
                        mtd = new WSMethod();
                        //1.接口名称 ele.Name
                        mtd.No = ele.Name;
                        mtd.ParaMS = new Dictionary<string, string>();
                        //2.接口返回值类型
                        ctype = res.SchemaType as XmlSchemaComplexType;
                        seq = ctype.Particle as XmlSchemaSequence;

                        if (seq != null && seq.Items.Count > 0)
                            mtd.Return = resType = (seq.Items[0] as XmlSchemaElement).SchemaTypeName.Name;
                        else
                            continue;// resType = "void";   //去除不返回结果的接口

                        //3.接口参数
                        ctype = ele.SchemaType as XmlSchemaComplexType;
                        seq = ctype.Particle as XmlSchemaSequence;

                        if (seq != null && seq.Items.Count > 0)
                        {
                            foreach (XmlSchemaElement pe in seq.Items)
                            {
                                mparams += pe.SchemaTypeName.Name + " " + pe.Name + ", ";
                                mtd.ParaMS.Add(pe.Name, pe.SchemaTypeName.Name);
                            }

                            mparams = mparams.TrimEnd(", ".ToCharArray());
                        }

                        mtd.Name = string.Format("{0} {1}({2})", resType, ele.Name, mparams);
                        mtds.Add(mtd);
                        //methods.Add(ele.Name, string.Format("{0} {1}({2})", resType, ele.Name, mparams));
                    }
                }

                stream.Close();
                stream.Dispose();
                wc.Dispose();
                #endregion

                return mtds;
        }
    }
}
