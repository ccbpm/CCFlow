
function keyDown(e){
    e.preventDefault();
	var currKey=0, e=e||event||window.event;
	currKey = e.keyCode||e.which||e.charCode;
	if (currKey == 83 && (e.ctrlKey || e.metaKey)) {

		//$('#Btn_Save').click();
	//	return false;
	}
	return true;
}
function SearchDict() {
    var url = "SearchDict.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = url;
}
//document.onkeydown = keyDown;
 
function SearchBill() {
    var url = "SearchBill.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = url;
}

function Group() {
    var url = "Group.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = url;
}

function DraftBox() {
    var url = "Draft.htm?FrmID=" + GetQueryString("FrmID");
    window.location.href = url;
}

function RefBill(frmID) {
    //关联单据
   
    var W = document.body.clientWidth - 40;
    var H = document.body.clientHeight - 40;
    var url = "Opt/RefBill.htm?PFrmID=" + frmID + "&WorkID=" + GetQueryString("WorkID") + "&FrmID=" + GetQueryString("FrmID");
    OpenBootStrapModal(url, "eudlgframe", "关联单据", W, H, "icon-property", null, null, null, function () {
        window.location.href = window.location.href;
    }, null, "black");
}

//查看关联单据的信息
function ShowRefBillInfo(frmID) {
    var workID = frmData.MainTable[0].PWorkID;
    var url = "MyBill.htm?WorkID=" + workID + "&FrmID=" + frmID + "&FK_MapData=" + frmID;
    var W = document.body.clientWidth - 40;
    var H = document.body.clientHeight - 40;

    OpenBootStrapModal(url, "eudlgframe", "关联单据信息", W, H, "icon-property", null, null, null, null, null, "black"); 
}


function StartFlow() {
    alert('尚未完成.');
}

function PrintHtml() {
    var W = document.body.clientWidth - 120;
    var H = document.body.clientHeight -120;
    var url = "../WorkOpt/Packup.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID") + "&SourceType=Bill&FileType=htm";
    OpenBootStrapModal(url, "eudlgframe", "打印PDF", W, H, "icon-property", null, null, null, null, null, "black");
}

function PrintPDF() {
    var W = document.body.clientWidth - 40;
    var H = document.body.clientHeight - 40;
    $("#Btn_PrintPdf").val("PDF打印中...");
    $("#Btn_PrintPdf").attr("disabled", true);
    var _html = document.getElementById("divCurrentForm").innerHTML;
    _html = _html.replace("height: " + $("#topContentDiv").height() + "px", "");
    _html = _html.replace("height: " + $("#contentDiv").height() + "px", "");
    _html = _html.replace("height: " + $("#divCCForm").height() + "px", "");

    var handler = new HttpHandler("BP.WF.HttpHandler.WF_WorkOpt");
    handler.AddPara("html", _html);
    handler.AddPara("FrmID", GetQueryString("FrmID"));
    handler.AddPara("WorkID", GetQueryString("WorkID"));
    handler.AddPara("SourceType", "Bill");
    var data = handler.DoMethodReturnString("Packup_Init");
    if (data.indexOf("err@") != -1) {
        alert(data);
    } else {
        $("#Btn_PrintPdf").val("PDF打印成功");
        $("#Btn_PrintPdf").attr("disabled", false);
        $("#Btn_PrintPdf").val("打印pdf");
        var urls = JSON.parse(data);
        for (var i = 0; i < urls.length; i++) {
            if (urls[i].No == "pdf") {
                window.open(urls[i].Name);
                break;
            }
               
        }
    }
    
    //var url = "../WorkOpt/Packup.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID") + "&SourceType=Bill&FileType=pdf";
   // OpenBootStrapModal(url, "eudlgframe", "打印PDF",600, 500, "icon-property", null, null, null, null, null, "black");

}

function PrintRTF() {
    var url = "../WorkOpt/PrintDoc.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID") + "&SourceType=Bill";
    OpenBootStrapModal(url, "eudlgframe", "打印PDF", 600, 500, "icon-property", null, null, null, null, null, "black");
}

function PrintCCWord() {
    var url = "../WorkOpt/PrintDoc.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID") + "&SourceType=Bill";
    OpenBootStrapModal(url, "eudlgframe", "打印PDF", 600, 500, "icon-property", null, null, null, null, null, "black");
}

function ExpToZip() {
    var url = "../WorkOpt/Packup.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID") + "&SourceType=Bill&FileType=zip";
    OpenBootStrapModal(url, "eudlgframe", "打印PDF",600, 500, "icon-property", null, null, null, null, null, "black");
}
