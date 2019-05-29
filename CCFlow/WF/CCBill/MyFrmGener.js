
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
    var url = "../WorkOpt/Packup.htm?FrmID=" + GetQueryString("FrmID") + "&WorkID=" + GetQueryString("WorkID") + "&SourceType=Bill&FileType=pdf";
    OpenBootStrapModal(url, "eudlgframe", "打印PDF",600, 500, "icon-property", null, null, null, null, null, "black");

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
