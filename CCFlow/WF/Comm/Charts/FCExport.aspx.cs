using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.IO;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace CCFlow.WF.Comm.Charts
{
    public partial class FCExport : System.Web.UI.Page
    {
        private const string SAVE_PATH = "./";

        /// <summary>
        /// IMPORTANT: This constant HTTP_URI stores the HTTP reference to 
        /// the folder where exported charts will be saved. 
        /// Please enter the HTTP representation of that folder 
        /// in this constant e.g., http://www.yourdomain.com/images/
        /// </summary>
        private const string HTTP_URI = "http://www.yourdomain.com/images/";

        /// <summary>
        /// OVERWRITEFILE sets whether the export handler would overwrite an existing file 
        /// the newly created exported file. If it is set to false the export handler would
        /// not overwrite. In this case, if INTELLIGENTFILENAMING is set to true the handler
        /// would add a suffix to the new file name. The suffix is a randomly generated GUID.
        /// Additionally, you add a timestamp or a random number as additional suffix.
        /// </summary>
        private bool OVERWRITEFILE = false;
        private bool INTELLIGENTFILENAMING = true;
        private string FILESUFFIXFORMAT = "TIMESTAMP";// // value can be either 'TIMESTAMP' or 'RANDOM'


        /// <summary>
        /// This is a constant list of the MIME types related to each export format this resource handles
        /// The value is semicolon separated key value pair for each format
        /// Each key is the format and value is the MIME type
        /// </summary>
        private const string MIMETYPES = "pdf=application/pdf;jpg=image/jpeg;jpeg=image/jpeg;gif=image/gif;png=image/png";

        /// <summary>
        /// This is a constant list of all the file extensions for the export formats
        /// The value is semicolon separated key value pair for each format
        /// Each key is the format and value is the file extension 
        /// </summary>
        private const string EXTENSIONS = "pdf=pdf;jpg=jpg;jpeg=jpg;gif=gif;png=png";

        /// <summary>
        /// Lists the default exportParameter values taken, if not provided by chart
        /// </summary>
        private const string DEFAULTPARAMS = "exportfilename=FusionCharts;exportformat=PDF;exportaction=download;exporttargetwindow=_self";

        /// <summary>
        /// Stores server notices, if any as string [ to be sent back to chart after save ] 
        /// </summary>
        private string notices = "";
        /// <summary>
        /// Whether the export action is download. Default value. Would change as per setting retrieved from chart.
        /// </summary>
        private bool isDownload = true;

        /// <summary>
        /// DOMId of the chart
        /// </summary>
        private string DOMId;


        /// <summary>
        /// The main function that handles all Input - Process - Output of this Export Architecture
        /// </summary>
        /// <param name="sender">FusionCharts chart SWF</param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {

            /**
             * Retrieve export data from POST Request sent by chart
             * Parse the Request stream into export data readable by this script
             */
            Hashtable exportData = parseExportRequestStream();

            // process export data and get the processed data (image/PDF) to be exported
            MemoryStream exportObject = exportProcessor(((Hashtable)exportData["parameters"])["exportformat"].ToString(), exportData["stream"].ToString(), (Hashtable)exportData["meta"]);


            /*
             * Send the export binary to output module which would either save to a server directory
             * or send the export file to download. Download terminates the process while
             * after save the output module sends back export status 
             */
            object exportedStatus = outputExportObject(exportObject, (Hashtable)exportData["parameters"]);

            // Dispose export object
            exportObject.Close();
            exportObject.Dispose();

            /*
             * Build Appropriate Export Status and send back to chart by flushing the  
             * procesed status to http response. This returns status back to chart. 
             * [ This is not applicable when Download action took place ]
             */
            flushStatus(exportedStatus, (Hashtable)exportData["meta"]);

        }

        /// <summary>
        /// Parses POST stream from chart and builds a Hashtable containing 
        /// export data and parameters in a format readable by other functions.
        ///  The Hashtable contains keys 'stream' (contains encoded 
        /// image data) ; 'meta' ( Hashtable with 'width', 'height' and 'bgColor' keys) ;
        /// and 'parameters' ( Hashtable of all export parameters from chart as keys, like - exportFormat, 
        /// exportFileName, exportAction etc.)
        /// </summary>
        /// <returns>Hashtable of processed export data and parameters.</returns>
        private Hashtable parseExportRequestStream()
        {
            // store all export data
            Hashtable exportData = new Hashtable();

            //String of compressed image data
            exportData["stream"] = Request["stream"];

            //Halt execution  if image stream is not provided.
            if (Request["stream"] == null || Request["stream"].Trim() == "") raise_error("100", true);

            //Get all export parameters into a Hastable
            Hashtable parameters = parseParams(Request["parameters"]);
            exportData["parameters"] = parameters;

            //get width and height of the chart
            Hashtable meta = new Hashtable();

            meta["width"] = Request["meta_width"];
            //Halt execution on error
            if (Request["meta_width"] == null || Request["meta_width"].Trim() == "") raise_error("101", true);

            meta["height"] = Request["meta_height"];
            //Halt execution on error
            if (Request["meta_height"] == null || Request["meta_height"].Trim() == "") raise_error("101", true);


            //Background color of chart
            meta["bgcolor"] = Request["meta_bgColor"];
            if (Request["meta_bgColor"] == null || Request["meta_bgColor"].Trim() == "")
            {
                // Send notice if BgColor is not provided
                raise_error(" Background color not specified. Taking White (FFFFFF) as default background color.");
                // Set White as Default Background color
                meta["bgcolor"] = meta["bgcolor"].ToString().Trim() == "" ? "FFFFFF" : meta["bgcolor"];
            }

            // DOMId of the chart
            meta["DOMId"] = Request["meta_DOMId"] == null ? "" : Request["meta_DOMId"];
            DOMId = meta["DOMId"].ToString();

            exportData["meta"] = meta;

            return exportData;
        }

        /// <summary>
        /// Parse export 'parameters' string into a Hashtable 
        /// Also synchronise default values from defaultparameterValues Hashtable
        /// </summary>
        /// <param name="strParams">A string with parameters (key=value pairs) separated  by | (pipe)</param>
        /// <returns>Hashtable containing parsed key = value pairs.</returns>
        private Hashtable parseParams(string strParams)
        {

            //default parameter values
            Hashtable defaultParameterValues = bang(DEFAULTPARAMS);

            // get parameters
            Hashtable parameters = bang(strParams, new char[] { '|', '=' });

            // sync with default values
            // iterate through each default parameter value
            foreach (DictionaryEntry param in defaultParameterValues)
            {
                // if a parameter from the defaultParameterValues Hashtable is not present
                // in the parameters hashtable take the parameter and value from default
                // parameter hashtable and add it to params hashtable
                // This is needed to ensure proper export
                if (parameters[param.Key] == null) parameters[param.Key] = param.Value.ToString();
            }

            // set a global flag which denotes whether the export is download or not
            // this is needed in many a functions 
            isDownload = parameters["exportaction"].ToString().ToLower() == "download";


            // return parameters
            return parameters;


        }

        /// <summary>
        /// Get Export data from and build the export binary/objct.
        /// </summary>
        /// <param name="strFormat">(string) Export format</param>
        /// <param name="stream">(string) Export image data in FusionCharts compressed format</param>
        /// <param name="meta">{Hastable)Image meta data in keys "width", "heigth" and "bgColor"</param>
        /// <returns></returns>
        private MemoryStream exportProcessor(string strFormat, string stream, Hashtable meta)
        {

            strFormat = strFormat.ToLower();
            // initilize memeory stream object to store output bytes
            MemoryStream exportObjectStream = new MemoryStream();

            // Handle Export class as per export format
            switch (strFormat)
            {
                case "pdf":
                    // Instantiate Export class for PDF, build Binary stream and store in stream object
                    FCPDFGenerator PDFGEN = new FCPDFGenerator(stream, meta["width"].ToString(), meta["height"].ToString(), meta["bgcolor"].ToString());
                    exportObjectStream = PDFGEN.getBinaryStream(strFormat);
                    break;
                case "jpg":
                case "jpeg":
                case "png":
                case "gif":
                    // Instantiate Export class for Images, build Binary stream and store in stream object
                    FCIMGGenerator IMGGEN = new FCIMGGenerator(stream, meta["width"].ToString(), meta["height"].ToString(), meta["bgcolor"].ToString());
                    exportObjectStream = IMGGEN.getBinaryStream(strFormat);
                    break;
                default:
                    // In case the format is not recognized
                    raise_error(" Invalid Export Format.", true);
                    break;
            }

            return exportObjectStream;
        }

        /// <summary>
        /// Checks whether the export action is download or save.
        /// If action is 'download', send export parameters to 'setupDownload' function.
        /// If action is not-'download', send export parameters to 'setupServer' function.
        /// In either case it gets exportSettings and passes the settings along with 
        /// processed export binary (image/PDF) to the output handler function if the
        /// export settings return a 'ready' flag set to 'true' or 'download'. The export
        /// process would stop here if the action is 'download'. In the other case, 
        /// it gets back success status from output handler function and returns it.
        /// </summary>
        /// <param name="exportObj">Export binary/object in memery stream</param>
        /// <param name="exportParams">Hashtable of export parameters</param>
        /// <returns>Export success status ( filename if success, false if not)</returns>
        private object outputExportObject(MemoryStream exportObj, Hashtable exportParams)
        {
            //pass export paramters and get back export settings as per export action
            Hashtable exportActionSettings = (isDownload ? setupDownload(exportParams) : setupServer(exportParams));

            // set default export status to true
            bool status = true;

            // filepath returned by server setup would be a string containing the file path
            // where the export file is to be saved.
            // If filepath is a boolean (i.e. false) the server setup must have failed. Hence, terminate process.
            if (exportActionSettings["filepath"] is bool)
            {
                status = false;
                raise_error(" Failed to export.", true);
            }
            else
            {
                // When 'filepath' is a sting write the binary to output stream
                try
                {
                    // Write export binary stream to output stream
                    Stream outStream = (Stream)exportActionSettings["outStream"];
                    exportObj.WriteTo(outStream);
                    outStream.Flush();
                    outStream.Close();
                    exportObj.Close();
                }
                catch (ArgumentNullException e)
                {
                    raise_error(" Failed to export. Error:" + e.Message);
                    status = false;
                }
                catch (ObjectDisposedException e)
                {
                    raise_error(" Failed to export. Error:" + e.Message);
                    status = false;
                }


                if (isDownload)
                {
                    // If 'download'- terminate imediately
                    // As nothing is to be written to response now.
                    Response.End();
                }

            }

            // This is the response after save action
            // If status remains true return the 'filepath'. Otherwise return false to denote failure.
            return (status ? exportActionSettings["filepath"] : false);


        }
        /// <summary>
        /// Flushes exported status message/or any status message to the chart or the output stream.
        /// It parses the exported status through parser function parseExportedStatus,
        /// builds proper response string using buildResponse function and flushes the response
        /// string to the output stream and terminates the program.
        /// </summary>
        /// <param name="filename">Name of the exported file or false on failure</param>
        /// <param name="meta">Image's meta data</param>
        /// <param name="msg">Additional messages</param>
        private void flushStatus(object filename, Hashtable meta, string msg)
        {
            // Process and flush message to response stream and terminate
            Response.Output.Write(buildResponse(parseExportedStatus(filename, meta, msg)));
            Response.Flush();
            Response.End();
        }

        /// <summary>
        /// Flushes exported status message/or any status message to the chart or the output stream.
        /// It parses the exported status through parser function parseExportedStatus,
        /// builds proper response string using buildResponse function and flushes the response
        /// string to the output stream and terminates the program.
        /// </summary>
        /// <param name="filename">Name of the exported file or false on failure</param>
        /// <param name="meta">Image's meta data</param>
        /// <param name="meta"></param>
        private void flushStatus(object filename, Hashtable meta)
        {
            flushStatus(filename, meta, "");
        }


        /// <summary>
        /// Parses the exported status and builds an array of export status information. As per
        /// status it builds a status array which contains statusCode (0/1), statusMesage, fileName,
        /// width, height and notice in some cases.
        /// </summary>
        /// <param name="filename">exported status ( false if failed/error, filename as stirng if success)</param>
        /// <param name="meta">Hastable containing meta descriptions of the chart like width, height</param>
        /// <param name="msg">custom message to be added as statusMessage.</param>
        /// <returns></returns>
        private ArrayList parseExportedStatus(object filename, Hashtable meta, string msg)
        {

            ArrayList arrStatus = new ArrayList();
            // get status
            bool status = (filename is string ? true : false);

            // add notices 
            if (notices.Trim() != "") arrStatus.Add("notice=" + notices.Trim());

            // DOMId of the chart
            arrStatus.Add("DOMId=" + (meta["DOMId"] == null ? DOMId : meta["DOMId"].ToString()));

            // add width and height
            // provide 0 as width and height on failure	
            if (meta["width"] == null) meta["width"] = "0";
            if (meta["height"] == null) meta["height"] = "0";
            arrStatus.Add("height=" + (status ? meta["height"].ToString() : "0"));
            arrStatus.Add("width=" + (status ? meta["width"].ToString() : "0"));

            // add file URI
            arrStatus.Add("fileName=" + (status ? (Regex.Replace(HTTP_URI, @"([^\/]$)", "${1}/") + filename) : ""));
            arrStatus.Add("statusMessage=" + (msg.Trim() != "" ? msg.Trim() : (status ? "Success" : "Failure")));
            arrStatus.Add("statusCode=" + (status ? "1" : "0"));

            return arrStatus;

        }


        /// <summary>
        /// Builds response from an array of status information. Joins the array to a string.
        /// Each array element should be a string which is a key=value pair. This array are either joined by 
        /// a & to build a querystring (to pass to chart) or joined by a HTML <BR> to show neat
        /// and clean status informaton in Browser window if download fails at the processing stage. 
        /// </summary>
        /// <param name="arrMsg">Array of string containing status data as [key=value ]</param>
        /// <returns>A string to be written to output stream</returns>
        private string buildResponse(ArrayList arrMsg)
        {
            // Join export status array elements into querystring key-value pairs in case of 'save' action
            // or separate with <BR> in case of 'download' action. This would make the imformation readable in browser window.
            string msg = isDownload ? "" : "&";
            msg += string.Join((isDownload ? "<br>" : "&"), (string[])arrMsg.ToArray(typeof(string)));
            return msg;
        }

        /// <summary>
        /// Finds if a directory is writable
        /// </summary>
        /// <param name="path">String Path</param>
        /// <returns></returns>
        private bool isDirectoryWritable(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            return (info.Attributes & FileAttributes.ReadOnly) != FileAttributes.ReadOnly;

        }
        /// <summary>
        /// check server permissions and settings and return ready flag to exportSettings 
        /// </summary>
        /// <param name="exportParams">Various export parameters</param>
        /// <returns>Hashtable containing various export settings</returns>
        private Hashtable setupServer(Hashtable exportParams)
        {

            //get export file name
            string exportFile = exportParams["exportfilename"].ToString();
            // get extension related to specified type 
            string ext = getExtension(exportParams["exportformat"].ToString());

            Hashtable retServerStatus = new Hashtable();

            //set server status to true by default
            retServerStatus["ready"] = true;

            // Open a FileStream to be used as outpur stream when the file would be saved
            FileStream fos;

            // process SAVE_PATH : the path where export file would be saved
            // add a / at the end of path if / is absent at the end

            string path = SAVE_PATH;
            // if path is null set it to folder where FCExporter.aspx is present
            if (path.Trim() == "") path = "./";
            path = Regex.Replace(path, @"([^\/]$)", "${1}/");

            try
            {
                // check if the path is relative if so assign the actual path to path
                path = HttpContext.Current.Server.MapPath(path);
            }
            catch (HttpException e)
            {
                //raise_error(e.Message);
            }


            // check whether directory exists
            // raise error and halt execution if directory does not exists
            if (!Directory.Exists(path)) raise_error(" Server Directory does not exist.", true);

            // check if directory is writable or not
            bool dirWritable = isDirectoryWritable(path);

            // build filepath
            retServerStatus["filepath"] = exportFile + "." + ext;

            // check whether file exists
            if (!File.Exists(path + retServerStatus["filepath"].ToString()))
            {
                // need to create a new file if does not exists
                // need to check whether the directory is writable to create a new file  
                if (dirWritable)
                {
                    // if directory is writable return with ready flag

                    // open the output file in FileStream
                    fos = File.Open(path + retServerStatus["filepath"].ToString(), FileMode.Create, FileAccess.Write);

                    // set the output stream to the FileStream object
                    retServerStatus["outStream"] = fos;
                    return retServerStatus;
                }
                else
                {
                    // if not writable halt and raise error
                    raise_error("403", true);
                }
            }

            // add notice that file exists 
            raise_error(" File already exists.");

            //if overwrite is on return with ready flag 
            if (OVERWRITEFILE)
            {
                // add notice while trying to overwrite
                raise_error(" Export handler's Overwrite setting is on. Trying to overwrite.");

                // see whether the existing file is writable
                // if not halt raising error message
                if ((new FileInfo(path + retServerStatus["filepath"].ToString())).IsReadOnly)
                    raise_error(" Overwrite forbidden. File cannot be overwritten.", true);

                // if writable return with ready flag 
                // open the output file in FileStream
                // set the output stream to the FileStream object
                fos = File.Open(path + retServerStatus["filepath"].ToString(), FileMode.Create, FileAccess.Write);
                retServerStatus["outStream"] = fos;
                return retServerStatus;
            }

            // raise error and halt execution when overwrite is off and intelligent naming is off 
            if (!INTELLIGENTFILENAMING)
            {
                raise_error(" Export handler's Overwrite setting is off. Cannot overwrite.", true);
            }

            raise_error(" Using intelligent naming of file by adding an unique suffix to the exising name.");
            // Intelligent naming 
            // generate new filename with additional suffix
            exportFile = exportFile + "_" + generateIntelligentFileId();
            retServerStatus["filepath"] = exportFile + "." + ext;

            // return intelligent file name with ready flag
            // need to check whether the directory is writable to create a new file  
            if (dirWritable)
            {
                // if directory is writable return with ready flag
                // add new filename notice
                // open the output file in FileStream
                // set the output stream to the FileStream object
                raise_error(" The filename has changed to " + retServerStatus["filepath"].ToString() + ".");
                fos = File.Open(path + retServerStatus["filepath"].ToString(), FileMode.Create, FileAccess.Write);

                // set the output stream to the FileStream object
                retServerStatus["outStream"] = fos;
                return retServerStatus;
            }
            else
            {
                // if not writable halt and raise error
                raise_error("403", true);
            }

            // in any unknown case the export should not execute	
            retServerStatus["ready"] = false;
            raise_error(" Not exported due to unknown reasons.");
            return retServerStatus;

        }
        /// <summary>
        /// setup download headers and return ready flag in exportSettings 
        /// </summary>
        /// <param name="exportParams">Various export parameters</param>
        /// <returns>Hashtable containing various export settings</returns>
        private Hashtable setupDownload(Hashtable exportParams)
        {

            //get export filename
            string exportFile = exportParams["exportfilename"].ToString();
            //exportFile = HttpUtility.UrlDecode(exportFile, System.Text.Encoding.GetEncoding("gb2312"));
            //exportFile = System.Text.Encoding.UTF8.GetString(System.Text.Encoding.UTF8.GetBytes(exportFile)); 

            //get extension
            string ext = getExtension(exportParams["exportformat"].ToString());
            //get mime type
            string mime = getMime(exportParams["exportformat"].ToString());
            // get target window
            string target = exportParams["exporttargetwindow"].ToString().ToLower();

            // set content-type header 
            Response.ContentType = mime;

            // set content-disposition header 
            // when target is _self the type is 'attachment'
            // when target is other than self type is 'inline'
            // NOTE : you can comment this line in order to replace present window (_self) content with the image/PDF  
            Response.AddHeader("Content-Disposition", (target == "_self" ? "attachment" : "inline") + "; filename=\"" + exportFile + "." + ext + "\"");

            // return exportSetting array. 'Ready' key should be set to 'download'
            Hashtable retStatus = new Hashtable();
            retStatus["filepath"] = "";

            // set the output strem to Response stream as the file is going to be downloaded
            retStatus["outStream"] = Response.OutputStream;
            return retStatus;

        }

        /// <summary>
        ///  gets file extension checking the export type. 
        /// </summary>
        /// <param name="exportType">(string) export format</param>
        /// <returns>string extension name</returns>
        private string getExtension(string exportType)
        {
            // get a Hashtable array of [type=> extension] 
            // from EXTENSIONS constant 
            Hashtable extensionList = bang(EXTENSIONS);
            exportType = exportType.ToLower();

            // if extension type is present in $extensionList return it, otherwise return the type 
            return (extensionList[exportType].ToString() != null ? extensionList[exportType].ToString() : exportType);
        }
        /// <summary>
        /// gets mime type for an export type
        /// </summary>
        /// <param name="exportType">Export format</param>
        /// <returns>Mime type as stirng</returns>
        private string getMime(string exportType)
        {
            // get a Hashtable array of [type=> extension] 
            // from MIMETYPES constant 
            Hashtable mimelist = bang(MIMETYPES);
            string ext = getExtension(exportType);

            // get mime type asociated to extension
            string mime = mimelist[ext].ToString() != null ? mimelist[ext].ToString() : "";
            return mime;
        }

        /// <summary>
        /// generates a file suffix for a existing file name to apply smart file naming 
        /// </summary>
        /// <returns>a string containing GUID and random number /timestamp</returns>
        private string generateIntelligentFileId()
        {
            // Generate Guid
            string guid = System.Guid.NewGuid().ToString("D");

            // check FILESUFFIXFORMAT type 
            if (FILESUFFIXFORMAT.ToLower() == "timestamp")
            {
                // Add time stamp with file name
                guid += "_" + DateTime.Now.ToString("ddMMyyyyHHmmssff");
            }
            else
            {
                // Add Random Number with fileName
                guid += "_" + (new Random()).Next().ToString();
            }

            return guid;
        }


        /// <summary>
        /// Helper function that splits a string containing delimiter separated key value pairs 
        /// into hashtable
        /// </summary>
        /// <param name="str">delimiter separated key value pairs</param>
        /// <param name="delimiterList">List of delimiters</param>
        /// <returns></returns>
        private Hashtable bang(string str, char[] delimiterList)
        {
            Hashtable retArray = new Hashtable();
            // split string as per first delimiter
            if (str == null || str.Trim() == "") return retArray;
            string[] tmpArray = str.Split(delimiterList[0]);


            // iterate through each element of split string
            for (int i = 0; i < tmpArray.Length; i++)
            {
                // split each element as per second delimiter
                string[] tmp2Array = tmpArray[i].Split(delimiterList[1]);

                if (tmp2Array.Length >= 2)
                {
                    // if the secondary split creats at-least 2 array elements
                    // make the fisrt element as the key and the second as the value
                    // of the resulting array
                    retArray[tmp2Array[0].ToLower()] = tmp2Array[1];
                }
            }
            return retArray;

        }
        private Hashtable bang(string str)
        {
            return bang(str, new char[2] { ';', '=' });
        }
        private void raise_error(string msg)
        {
            raise_error(msg, false);
        }
        /// <summary>
        /// Error reporter function that has a list of error messages. It can terminate the execution
        /// and send successStatus=0 along with a error message. It can also append notice to a global variable
        /// and continue execution of the program. 
        /// </summary>
        /// <param name="msg">error code as Integer (referring to the index of the errMessages
        /// array containing list of error messages) OR, it can be a string containing the error message/notice</param>
        /// <param name="halt">Whether to halt execution</param>
        private void raise_error(string msg, bool halt)
        {
            Hashtable errMessages = new Hashtable();

            //list of defined error messages
            errMessages["100"] = " Insufficient data.";
            errMessages["101"] = " Width/height not provided.";
            errMessages["102"] = " Insufficient export parameters.";
            errMessages["400"] = " Bad request.";
            errMessages["401"] = " Unauthorized access.";
            errMessages["403"] = " Directory write access forbidden.";
            errMessages["404"] = " Export Resource class not found.";

            // Find whether error message is passed in msg or it is a custom error string.
            string err_message = ((msg == null || msg.Trim() == "") ? "ERROR!" :
                    (errMessages[msg] == null ? msg : errMessages[msg].ToString())
                );

            // Halt executon after flushing the error message to response (if halt is true)
            if (halt)
            {
                flushStatus(false, new Hashtable(), err_message);

            }
            // add error to notices global variable
            else
            {
                notices += err_message;
            }

        }



    }


    /// <summary>
    /// FusionCharts Image Generator Class
    /// FusionCharts Exporter - 'Image Resource' handles 
    /// FusionCharts (since v3.1) Server Side Export feature that
    /// helps FusionCharts exported as Image files in various formats. 
    /// </summary>
    public class FCIMGGenerator
    {
        //Array - Stores multiple chart export data
        private ArrayList arrExportData = new ArrayList();
        //stores number of pages = length of $arrExportData array
        private int numPages = 0;


        /// <summary>
        /// Generates bitmap data for the image from a FusionCharts export format
        /// the height and width of the original export needs to be specified
        /// the default background color can also be specified
        /// </summary>
        public FCIMGGenerator(string imageData_FCFormat, string width, string height, string bgcolor)
        {
            setBitmapData(imageData_FCFormat, width, height, bgcolor);
        }

        /// <summary>
        /// Gets the binary data stream of the image
        /// The passed parameter determines the file format of the image
        /// to be exported
        /// </summary>
        public MemoryStream getBinaryStream(string strFormat)
        {

            // the image object 
            Bitmap exportObj = getImageObject();

            // initiates a new binary data sream
            MemoryStream outStream = new MemoryStream();

            // determines the image format
            switch (strFormat)
            {
                case "jpg":
                case "jpeg":
                    exportObj.Save(outStream, ImageFormat.Jpeg);
                    break;
                case "png":
                    exportObj.Save(outStream, ImageFormat.Png);
                    break;
                case "gif":
                    exportObj.Save(outStream, ImageFormat.Gif);
                    break;
                case "tiff":
                    exportObj.Save(outStream, ImageFormat.Tiff);
                    break;
                default:
                    exportObj.Save(outStream, ImageFormat.Bmp);
                    break;
            }
            exportObj.Dispose();

            return outStream;

        }


        /// <summary>
        /// Generates bitmap data for the image from a FusionCharts export format
        /// the height and width of the original export needs to be specified
        /// the default background color can also be specified
        /// </summary>
        private void setBitmapData(string imageData_FCFormat, string width, string height, string bgcolor)
        {
            Hashtable chartExportData = new Hashtable();
            chartExportData["width"] = width;
            chartExportData["height"] = height;
            chartExportData["bgcolor"] = bgcolor;
            chartExportData["imagedata"] = imageData_FCFormat;
            arrExportData.Add(chartExportData);
            numPages++;
        }

        /// <summary>
        /// Generates bitmap data for the image from a FusionCharts export format
        /// the height and width of the original export needs to be specified
        /// the default background color should also be specified
        /// </summary>
        private Bitmap getImageObject(int id)
        {
            Hashtable rawImageData = (Hashtable)arrExportData[id];

            // create blank bitmap object which would store image pixel data
            Bitmap image = new Bitmap(Convert.ToInt16(rawImageData["width"]), Convert.ToInt16(rawImageData["height"]), System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            // drwaing surface
            Graphics gr = Graphics.FromImage(image);

            // set background color
            gr.Clear(ColorTranslator.FromHtml("#" + rawImageData["bgcolor"].ToString()));

            string[] rows = rawImageData["imagedata"].ToString().Split(';');

            for (int yPixel = 0; yPixel < rows.Length; yPixel++)
            {
                //Split each row into 'color_count' columns.			
                String[] color_count = rows[yPixel].Split(',');
                //Set horizontal row index to 0
                int xPixel = 0;

                for (int col = 0; col < color_count.Length; col++)
                {
                    //Now, if it's not empty, we process it				
                    //Split the 'color_count' into color and repeat factor
                    String[] split_data = color_count[col].Split('_');

                    //Reference to color
                    string hexColor = split_data[0];
                    //refer to repeat factor
                    int fRepeat = int.Parse(split_data[1]);

                    //If color is not empty (i.e. not background pixel)
                    if (hexColor != "")
                    {
                        //If the hexadecimal code is less than 6 characters, pad with 0
                        hexColor = hexColor.Length < 6 ? hexColor.PadLeft(6, '0') : hexColor;
                        for (int k = 1; k <= fRepeat; k++)
                        {

                            //draw pixel with specified color
                            image.SetPixel(xPixel, yPixel, ColorTranslator.FromHtml("#" + hexColor));
                            //Increment horizontal row count
                            xPixel++;
                        }
                    }
                    else
                    {
                        //Just increment horizontal index
                        xPixel += fRepeat;
                    }
                }
            }
            gr.Dispose();
            return image;

        }

        /// <summary>
        /// Retreives the bitmap image object
        /// </summary>
        private Bitmap getImageObject()
        {
            return getImageObject(0);
        }

    }


    /// <summary>
    /// FusionCharts Exporter - 'PDF Resource' handles 
    /// FusionCharts (since v3.1) Server Side Export feature that
    /// helps FusionCharts exported as PDF file.
    /// </summary>
    public class FCPDFGenerator
    {

        //Array - Stores multiple chart export data
        private ArrayList arrExportData = new ArrayList();
        //stores number of pages = length of $arrExportData array
        private int numPages = 0;

        /// <summary>
        /// Generates a PDF file with the given parameters
        /// The imageData_FCFormat parameter is the FusionCharts export format data
        /// width, height are the respective width and height of the original image
        /// bgcolor determines the default background colour
        /// </summary>
        public FCPDFGenerator(string imageData_FCFormat, string width, string height, string bgcolor)
        {
            setBitmapData(imageData_FCFormat, width, height, bgcolor);
        }

        /// <summary>
        /// Gets the binary data stream of the image
        /// The passed parameter determines the file format of the image
        /// to be exported
        /// </summary>
        public MemoryStream getBinaryStream(string strFormat)
        {
            byte[] exportObj = getPDFObjects(true);

            MemoryStream outStream = new MemoryStream();

            outStream.Write(exportObj, 0, exportObj.Length);

            return outStream;

        }

        /// <summary>
        /// Generates bitmap data for the image from a FusionCharts export format
        /// the height and width of the original export needs to be specified
        /// the default background color should also be specified
        /// </summary>
        private void setBitmapData(string imageData_FCFormat, string width, string height, string bgcolor)
        {
            Hashtable chartExportData = new Hashtable();
            chartExportData["width"] = width;
            chartExportData["height"] = height;
            chartExportData["bgcolor"] = bgcolor;
            chartExportData["imagedata"] = imageData_FCFormat;
            arrExportData.Add(chartExportData);
            numPages++;
        }



        //create image PDF object containing the chart image 
        private byte[] addImageToPDF(int id, bool isCompressed)
        {

            MemoryStream imgObj = new MemoryStream();

            //PDF Object number
            int imgObjNo = 6 + id * 3;

            //Get chart Image binary
            byte[] imgBinary = getBitmapData24(id, isCompressed);

            //get the length of the image binary
            int len = imgBinary.Length;

            string width = getMeta("width", id);
            string height = getMeta("height", id);

            //Build PDF object containing the image binary and other formats required
            //string strImgObjHead = imgObjNo.ToString() + " 0 obj\n<<\n/Subtype /Image /ColorSpace /DeviceRGB /BitsPerComponent 8 /HDPI 72 /VDPI 72 " + (isCompressed ? "" : "") + "/Width " + width + " /Height " + height + " /Length " + len.ToString() + " >>\nstream\n";
            string strImgObjHead = imgObjNo.ToString() + " 0 obj\n<<\n/Subtype /Image /ColorSpace /DeviceRGB /BitsPerComponent 8 /HDPI 72 /VDPI 72 " + (isCompressed ? "/Filter /RunLengthDecode " : "") + "/Width " + width + " /Height " + height + " /Length " + len.ToString() + " >>\nstream\n";



            imgObj.Write(stringToBytes(strImgObjHead), 0, strImgObjHead.Length);
            imgObj.Write(imgBinary, 0, (int)imgBinary.Length);

            string strImgObjEnd = "endstream\nendobj\n";
            imgObj.Write(stringToBytes(strImgObjEnd), 0, strImgObjEnd.Length);

            imgObj.Close();
            return imgObj.ToArray();

        }
        private byte[] addImageToPDF(int id)
        {
            return addImageToPDF(id, true);
        }
        private byte[] addImageToPDF()
        {
            return addImageToPDF(0, true);
        }



        //Main PDF builder function
        private byte[] getPDFObjects()
        {
            return getPDFObjects(true);
        }

        private byte[] getPDFObjects(bool isCompressed)
        {
            MemoryStream PDFBytes = new MemoryStream();

            //Store all PDF objects in this temporary string to be written to ByteArray
            string strTmpObj = "";


            //start xref array
            ArrayList xRefList = new ArrayList();
            xRefList.Add("xref\n0 ");
            xRefList.Add("0000000000 65535 f \n"); //Address Refenrece to obj 0

            //Build PDF objects sequentially
            //version and header
            strTmpObj = "%PDF-1.3\n%{FC}\n";
            PDFBytes.Write(stringToBytes(strTmpObj), 0, strTmpObj.Length);

            //OBJECT 1 : info (optional)
            strTmpObj = "1 0 obj<<\n/Author (FusionCharts)\n/Title (FusionCharts)\n/Creator (FusionCharts)\n>>\nendobj\n";
            xRefList.Add(calculateXPos((int)PDFBytes.Length)); //refenrece to obj 1
            PDFBytes.Write(stringToBytes(strTmpObj), 0, strTmpObj.Length);

            //OBJECT 2 : Starts with Pages Catalogue
            strTmpObj = "2 0 obj\n<< /Type /Catalog /Pages 3 0 R >>\nendobj\n";
            xRefList.Add(calculateXPos((int)PDFBytes.Length)); //refenrece to obj 2
            PDFBytes.Write(stringToBytes(strTmpObj), 0, strTmpObj.Length);

            //OBJECT 3 : Page Tree (reference to pages of the catalogue)
            strTmpObj = "3 0 obj\n<<  /Type /Pages /Kids [";
            for (int i = 0; i < numPages; i++)
            {
                strTmpObj += (((i + 1) * 3) + 1) + " 0 R\n";
            }
            strTmpObj += "] /Count " + numPages + " >>\nendobj\n";

            xRefList.Add(calculateXPos((int)PDFBytes.Length)); //refenrece to obj 3
            PDFBytes.Write(stringToBytes(strTmpObj), 0, strTmpObj.Length);


            //Each image page
            for (int itr = 0; itr < numPages; itr++)
            {
                string iWidth = getMeta("width", itr);
                string iHeight = getMeta("height", itr);
                //OBJECT 4..7..10..n : Page config
                strTmpObj = (((itr + 2) * 3) - 2) + " 0 obj\n<<\n/Type /Page /Parent 3 0 R \n/MediaBox [ 0 0 " + iWidth + " " + iHeight + " ]\n/Resources <<\n/ProcSet [ /PDF ]\n/XObject <</R" + (itr + 1) + " " + ((itr + 2) * 3) + " 0 R>>\n>>\n/Contents [ " + (((itr + 2) * 3) - 1) + " 0 R ]\n>>\nendobj\n";
                xRefList.Add(calculateXPos((int)PDFBytes.Length)); //refenrece to obj 4,7,10,13,16...
                PDFBytes.Write(stringToBytes(strTmpObj), 0, strTmpObj.Length);


                //OBJECT 5...8...11...n : Page resource object (xobject resource that transforms the image)
                xRefList.Add(calculateXPos((int)PDFBytes.Length)); //refenrece to obj 5,8,11,14,17...
                string xObjR = getXObjResource(itr);
                PDFBytes.Write(stringToBytes(xObjR), 0, xObjR.Length);

                //OBJECT 6...9...12...n : Binary xobject of the page (image)
                byte[] imgBA = addImageToPDF(itr, isCompressed);
                xRefList.Add(calculateXPos((int)PDFBytes.Length));//refenrece to obj 6,9,12,15,18...
                PDFBytes.Write(imgBA, 0, imgBA.Length);
            }

            //xrefs	compilation
            xRefList[0] += ((xRefList.Count - 1) + "\n");

            //get trailer
            string trailer = getTrailer((int)PDFBytes.Length, xRefList.Count - 1);

            //write xref and trailer to PDF
            string strXRefs = string.Join("", (string[])xRefList.ToArray(typeof(string)));
            PDFBytes.Write(stringToBytes(strXRefs), 0, strXRefs.Length);
            //
            PDFBytes.Write(stringToBytes(trailer), 0, trailer.Length);

            //write EOF
            string strEOF = "%%EOF\n";
            PDFBytes.Write(stringToBytes(strEOF), 0, strEOF.Length);

            PDFBytes.Close();
            return PDFBytes.ToArray();

        }


        //Build Image resource object that transforms the image from First Quadrant system to Second Quadrant system
        private string getXObjResource()
        {
            return getXObjResource(0);
        }
        private string getXObjResource(int itr)
        {

            string width = getMeta("width", itr);
            string height = getMeta("height", itr);
            return (((itr + 2) * 3) - 1) + " 0 obj\n<< /Length " + (24 + (width + height).Length) + " >>\nstream\nq\n" + width + " 0 0 " + height + " 0 0 cm\n/R" + (itr + 1) + " Do\nQ\nendstream\nendobj\n";
        }

        private string calculateXPos(int posn)
        {
            return posn.ToString().PadLeft(10, '0') + " 00000 n \n";
        }


        private string getTrailer(int xrefpos)
        {
            return getTrailer(xrefpos, 7);
        }

        private string getTrailer(int xrefpos, int numxref)
        {
            return "trailer\n<<\n/Size " + numxref.ToString() + "\n/Root 2 0 R\n/Info 1 0 R\n>>\nstartxref\n" + xrefpos.ToString() + "\n";
        }


        private byte[] getBitmapData24()
        {
            return getBitmapData24(0, true);
        }
        private byte[] getBitmapData24(int id, bool isCompressed)
        {

            string rawImageData = getMeta("imagedata", id);
            string bgColor = getMeta("bgcolor", id);

            MemoryStream imageData24 = new MemoryStream();

            // Split the data into rows using ; as separator
            string[] rows = rawImageData.Split(';');

            for (int yPixel = 0; yPixel < rows.Length; yPixel++)
            {
                //Split each row into 'color_count' columns.			
                string[] color_count = rows[yPixel].Split(',');

                for (int col = 0; col < color_count.Length; col++)
                {
                    //Now, if it's not empty, we process it				
                    //Split the 'color_count' into color and repeat factor
                    string[] split_data = color_count[col].Split('_');

                    //Reference to color
                    string hexColor = split_data[0] != "" ? split_data[0] : bgColor;
                    //If the hexadecimal code is less than 6 characters, pad with 0
                    hexColor = hexColor.Length < 6 ? hexColor.PadLeft(6, '0') : hexColor;

                    //refer to repeat factor
                    int fRepeat = int.Parse(split_data[1]);

                    // convert color string to byte[] array
                    byte[] rgb = hexToBytes(hexColor);

                    // Set the repeated pixel in MemoryStream
                    for (int cRepeat = 0; cRepeat < fRepeat; cRepeat++)
                    {
                        imageData24.Write(rgb, 0, 3);
                    }

                }
            }

            int len = (int)imageData24.Length;
            imageData24.Close();

            //Compress image binary
            if (isCompressed)
            {
                return new PDFCompress(imageData24.ToArray()).RLECompress();
            }
            else
            {
                return imageData24.ToArray();
            }
        }

        // converts a hexadecimal colour string to it's respective byte value
        private byte[] hexToBytes(string strHex)
        {
            if (strHex == null || strHex.Trim().Length == 0) strHex = "00";
            strHex = Regex.Replace(strHex, @"[^0-9a-fA-f]", "");
            if (strHex.Length % 2 != 0) strHex = "0" + strHex;

            int len = strHex.Length / 2;
            byte[] bytes = new byte[len];

            for (int i = 0; i < len; i++)
            {
                string hex = strHex.Substring(i * 2, 2);
                bytes[i] = byte.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            }
            return bytes;

        }

        private string getMeta(string metaName)
        {
            return getMeta(metaName, 0);
        }

        private string getMeta(string metaName, int id)
        {
            if (metaName == null) metaName = "";
            Hashtable chartData = (Hashtable)arrExportData[id];
            return (chartData[metaName] == null ? "" : chartData[metaName].ToString());
        }

        private byte[] stringToBytes(string str)
        {
            if (str == null) str = "";
            return System.Text.Encoding.ASCII.GetBytes(str);
        }


    }


    /// <summary>
    /// This is an ad-hoc class to compress PDF stream.
    /// Currently this class compresses binary (byte) stream using RLE which 
    /// PDF 1.3 specification has thus formulated:
    /// 
    /// The RunLengthDecode filter decodes data that has been encoded in a simple 
    /// byte-oriented format based on run length. The encoded data is a sequence of 
    /// runs, where each run consists of a length byte followed by 1 to 128 bytes of data. If 
    /// the length byte is in the range 0 to 127, the following length + 1 (1 to 128) bytes 
    /// are copied literally during decompression. If length is in the range 129 to 255, the 
    /// following single byte is to be copied 257 − length (2 to 128) times during decompression. 
    /// A length value of 128 denotes EOD.
    /// 
    /// The chart image compression ratio comes to around 10:3 
    /// 
    /// </summary>
    public class PDFCompress
    {

        /// <summary>
        /// stores the output compressed data in MemoryStream object later to be converted to byte[] array
        /// </summary>
        private MemoryStream _Compressed = new MemoryStream();

        /// <summary>
        ///  Uncompresses data as byte[] array
        /// </summary>
        private byte[] _UnCompressed;


        /// <summary>
        /// Takes the uncompressed byte array
        /// </summary>
        /// <param name="UnCompressed">uncompressed data</param>
        public PDFCompress(byte[] UnCompressed)
        {
            _UnCompressed = UnCompressed;
        }

        /// <summary>
        /// Write compressed data as RunLength
        /// </summary>
        /// <param name="length">The length of repeated data</param>
        /// <param name="encodee">The byte to be repeated</param>
        /// <returns></returns>
        private int WriteRunLength(int length, byte encodee)
        {
            // write the repeat length
            _Compressed.WriteByte((byte)(257 - length));
            // write the byte to be repeated
            _Compressed.WriteByte(encodee);

            //re-set repeat length
            length = 1;
            return length;
        }

        private void WriteNoRepeater(MemoryStream NoRepeatBytes)
        {
            // write the length of non repeted data
            _Compressed.WriteByte((byte)((int)NoRepeatBytes.Length - 1));
            // write the non repeated data put literally
            _Compressed.Write(NoRepeatBytes.ToArray(), 0, (int)NoRepeatBytes.Length);

            // re-set non repeat byte storage stream
            NoRepeatBytes.SetLength(0);
        }

        /// <summary>
        /// compresses uncompressed data to compressed data in byte array
        /// </summary>
        /// <returns></returns>
        public byte[] RLECompress()
        {
            // stores non repeatable data
            MemoryStream NoRepeat = new MemoryStream();

            // repeat counter
            int _RL = 1;

            // 2 consecutive bytes to compare
            byte preByte = 0, postByte = 0;

            // iterate through the uncompressed bytes
            for (int i = 0; i < _UnCompressed.Length - 1; i++)
            {
                // get 2 consecutive bytes
                preByte = _UnCompressed[i];
                postByte = _UnCompressed[i + 1];

                // if both are same there is scope for repitition
                if (preByte == postByte)
                {
                    // but flush the non repeatable data (if present) to compressed stream 
                    if (NoRepeat.Length > 0) WriteNoRepeater(NoRepeat);

                    // increase repeat count
                    _RL++;

                    // if repeat count reaches limit of repeat i.e. 128 
                    // write the repeat data and reset the repeat counter
                    if (_RL > 128) _RL = WriteRunLength(_RL - 1, preByte);

                }
                else
                {
                    // when consecutive bytes do not match

                    // store non-repeatable data
                    if (_RL == 1) NoRepeat.WriteByte(preByte);

                    // write repeated length and byte (if present ) to output stream
                    if (_RL > 1) _RL = WriteRunLength(_RL, preByte);

                    // write non repeatable data to out put stream if the length reaches limit
                    if (NoRepeat.Length == 128) WriteNoRepeater(NoRepeat);
                }
            }

            // at the end of iteration 
            // take care of the last byte

            // if repeated 
            if (_RL > 1)
            {
                // write run length and byte (if present ) to output stream
                _RL = WriteRunLength(_RL, preByte);
            }
            else
            {
                // if non repeated byte is left behind
                // write non repeatable data to output stream 
                NoRepeat.WriteByte(postByte);
                WriteNoRepeater(NoRepeat);
            }


            // wrote EOD
            _Compressed.WriteByte((byte)128);

            //close streams
            NoRepeat.Close();
            _Compressed.Close();

            // return compressed data in byte array
            return _Compressed.ToArray();
        }

    }
}