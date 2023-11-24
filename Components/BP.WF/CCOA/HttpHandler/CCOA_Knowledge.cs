using System;
using System.Collections;
using System.IO;
using System.Web;
using BP.DA;
using BP.Difference;
using BP.Sys;
using BP.Tools;
using BP.Web;
using BP.WF.HttpHandler;

namespace BP.CCOA.HttpHandler
{
    /// <summary>
    /// 知识库管理
    /// </summary>
    public class CCOA_Knowledge : DirectoryPageBase
    {
        public string Knowledge_UploadFile_MinAth()
        {
            // 获取RefPKVal
            string refPKVal = GetRequestVal("RefPKVal");
            // Knowledge_MinAth 表示知识库的多附件上传类型
            string sort = "Knowledge_MinAth";
            // 获取文件集合
            HttpFileCollection hfc = HttpContext.Current.Request.Files;
            // 获取DataUser目录路径
            string basePath = BP.Difference.SystemConfig.PathOfDataUser;

            for (int i = 0; i < hfc.Count; i++)
            {
                HttpPostedFile file = hfc[i];

                // 获取文件名
                string fileName = file.FileName;

                // 获取文件后缀
                string exts = System.IO.Path.GetExtension(fileName);

                // 生成GUID
                string guid = DBAccess.GenerGUID();

                // 附件存放路径
                string filepath = "UploadFile/Knowledge/" + refPKVal;

                // 判断路径是否存在
                if (!Directory.Exists(basePath + filepath))
                {
                    // 不存在就创建
                    Directory.CreateDirectory(basePath + filepath);
                }

                // 指定保存文件的路径
                string savePath = basePath + filepath + "/" + guid + "." + fileName;

                // 保存文件到指定路径
                file.SaveAs(savePath);

                //保存实体
                BP.Sys.FrmAttachmentDB dbUpload = new BP.Sys.FrmAttachmentDB();
                dbUpload.setMyPK(guid);
                dbUpload.SetValByKey(FrmAttachmentDBAttr.RefPKVal, refPKVal);
                dbUpload.SetValByKey(FrmAttachmentDBAttr.Sort, sort);
                dbUpload.SetValByKey(FrmAttachmentDBAttr.FileFullName, "/DataUser/" + filepath + "/" + guid + "." + fileName);
                dbUpload.SetValByKey(FrmAttachmentDBAttr.FileName, fileName);
                dbUpload.SetValByKey(FrmAttachmentDBAttr.FileExts, exts);
                dbUpload.SetValByKey(FrmAttachmentDBAttr.FileSize, file.ContentLength / 1024.0);
                dbUpload.SetValByKey(FrmAttachmentDBAttr.RDT, DataType.CurrentDateTime);
                dbUpload.SetValByKey(FrmAttachmentDBAttr.Rec, WebUser.No);
                dbUpload.SetValByKey(FrmAttachmentDBAttr.RecName, WebUser.Name);
                dbUpload.SetValByKey(FrmAttachmentDBAttr.FK_Dept, WebUser.DeptNo);
                dbUpload.SetValByKey(FrmAttachmentDBAttr.FK_DeptName, WebUser.DeptName);
                dbUpload.SetValByKey(FrmAttachmentDBAttr.UploadGUID, guid);

                //执行插入
                dbUpload.Insert();

            }

            return Return_Info(200, "附件上传成功", "");
        }

        private string Return_Info(int code, string msg, string data)
        {
            Hashtable ht = new Hashtable();
            ht.Add("code", code);
            ht.Add("message", msg);
            ht.Add("data", data);
            return Json.ToJson(ht);
        }
    }
}
