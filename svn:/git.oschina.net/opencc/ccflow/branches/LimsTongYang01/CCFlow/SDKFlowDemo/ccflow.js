
function Track(appPath, fk_flow, workid) {
    alert(appPath);
    var url =   '/WF/WorkFlow.aspx?FK_Flow=' + fk_flow + '&WorkID=' + workid;
    window.open(url, 'sd', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
    return false;
}

function Forward(appPath, fk_flow, workid) {
    alert(appPath);
    var url =    '/WF/WorkOpt/Forward.htm?FK_Flow=' + fk_flow + '&WorkID=' + workid;
    window.open(url, 'sd', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
    return false;
}

function Return(appPath, fk_flow,fk_node, workid) {
    var url = '/WF/WorkOpt/ReturnWork.htm?FK_Flow=' + fk_flow + '&FK_Node=' + fk_node + '&WorkID=' + workid;
    window.showModalDialog(url, 'sd', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
    window.location.href = window.location.href;
    return false;
}

function NewFlow(appPath, fk_flow) {
    alert(appPath);
    var url =   '/WF/MyFlow.aspx?FK_Flow=' + fk_flow ;
    window.open(url, 'sd', 'dialogHeight: 550px; dialogWidth: 650px; dialogTop: 100px; dialogLeft: 150px; center: yes; help: no');
    return false;
}
