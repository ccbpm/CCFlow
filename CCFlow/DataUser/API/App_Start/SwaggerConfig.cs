using System.Web.Http;
using WebActivatorEx;
using Swashbuckle.Application;
using CCFlow.DataUser.API;
using Swashbuckle.Swagger;
using System.Web.Http.Description;
using System.Linq;

[assembly: PreApplicationStartMethod(typeof(SwaggerConfig), "Register")]

namespace CCFlow.DataUser.API
{
    /// <summary>  
    /// ���ؽӿڣ������ɵ�swagger�ĵ�չʾ  
    /// </summary>  
    [System.AttributeUsage(System.AttributeTargets.Method | System.AttributeTargets.Class)]

    public partial class HiddenApiAttribute : System.Attribute { }
    public class HiddenApiFilter : IDocumentFilter
    {
        /// <summary>  
        /// ��дApply�������Ƴ����ؽӿڵ�����  
        /// </summary>  
        /// <param name="swaggerDoc">swagger�ĵ��ļ�</param>  
        /// <param name="schemaRegistry"></param>  
        /// <param name="apiExplorer">api�ӿڼ���</param>  
        public void Apply(SwaggerDocument swaggerDoc, SchemaRegistry schemaRegistry, IApiExplorer apiExplorer)
        {
            foreach (ApiDescription apiDescription in apiExplorer.ApiDescriptions)
            {
                if (Enumerable.OfType<HiddenApiAttribute>(apiDescription.GetControllerAndActionAttributes<HiddenApiAttribute>()).Any())
                {
                    string key = "/" + apiDescription.RelativePath;
                    if (key.Contains("?"))
                    {
                        int idx = key.IndexOf("?", System.StringComparison.Ordinal);
                        key = key.Substring(0, idx);
                    }
                    swaggerDoc.paths.Remove(key);
                }
            }
        }
    }
    public class SwaggerConfig
    {
        public static void Register()
        {
            var thisAssembly = typeof(SwaggerConfig).Assembly;
            GlobalConfiguration.Configuration
                .EnableSwagger(c =>
                    {
                        c.SingleApiVersion("v1", "ToolKit");
                        string path = System.AppDomain.CurrentDomain.BaseDirectory;
                        c.IncludeXmlComments(System.String.Format(@"{0}\bin\ToolKit.XML", path));
                        c.DocumentFilter<HiddenApiFilter>();
                    })
                .EnableSwaggerUi(c =>{});
        }
    }
}
