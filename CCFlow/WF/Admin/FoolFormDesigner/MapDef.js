document.onMouseDown = mouseDown;
document.onMouseUp = mouseUp;
//document.onmousemove = mouseMove;
// document.onDrag = mouseonDrag;
var currFieldID = null;
var moveToFieldID = null;
var isMove = false;
var currGID = null;
var moveToGID = null;
var DrgType = "";

function mouseMove() {
}

function CallState(ev) {
    return;
//    window.status = ev + ' = currFieldID ' + currFieldID + ' currGID=' + currGID + ' moveToFieldID=' + moveToFieldID + ' moveToGID=' + moveToGID;
}

function mouseDown() {
    return; 
//    document.all.T.style.cursor = "move";
    // document.curr cursor: pointer
    //alert("mouseDown = currFieldID=" + currFieldID);
    moveToFieldID = currFieldID;
    return true;
}

function onDragF(fid, gid) {
    // alert('sss');
    //document.all.T.style.cursor = "move";

    currFieldID = fid;
    currGID = gid;
    moveToFieldID = fid;
    CallState();
}

function onDragEndF(fid, gid) {

//    currFieldID = fid;
//    currGID = gid;
//    CallState();
//    alert(moveToFieldID);
//    return;
//    if (currFieldID == moveToFieldID ) {
//        currFieldID = null;
//        moveToFieldID = null;
//        currGID = null;
//        rowIdx = -1;
//        return true;
    //    }

    if (fid != moveToGID) {

    }

    moveToFieldID = window.status;
    if (moveToFieldID == null || moveToFieldID=='' || fid == moveToFieldID)
        return true;

    var url = 'Do.aspx?DoType=MoveTo&FromID=' + fid + '&ToID=' + moveToFieldID + '&FromGID=' + gid + '&ToGID=' + moveToGID;

    
    var b = window.showModalDialog(url, 'ass', 'dialogHeight: 500px; dialogWidth: 700px;center: yes; help: no');
    Reload();
    moveToFieldID = null;
    currGID = null;
    currFieldID = null;
    return;

    currFieldID = null;
    moveToFieldID = null;
    currGID = null;
    rowIdx = -1;
    return true;

    CallState();
}

function mouseUp(fid, gid) {
   
    moveToFieldID = fid;
    moveToGID = gid;
    CallState();
}

function onDragEnterF(fid, gid) {
    moveToFieldID = fid;
    moveToGID = gid;
    CallState();
}

function FieldOnMouseOver(id, gid) {
    //alert('FieldOnMouseOver= ' + id);
    window.status = id;
    moveToFieldID = id;
    moveToGID = gid;
    //  CallState('FieldOnMouseOver');
}

function FieldOnMouseOut() {
   // moveToFieldID = null;
    //moveToGID = null;
    CallState('FieldOnMouseOut');
}

function GFOnMouseOver(id, rowIdx) {
    moveToGID = id;
    rowIdx = id;
    CallState('GFOnMouseOver');
}

function GFOnMouseOut() {
    rowIdx = -1;
    CallState();
}