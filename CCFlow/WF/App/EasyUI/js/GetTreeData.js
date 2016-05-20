$(function () {
    Application.data.treedata(callBack2, this);
});

function callBack2(jsonData, scope) {
    if (jsonData) {
        if (jsonData.length > 17) {
            var pushData = eval('(' + jsonData + ')');
            $("#tree").ligerTree({
                data: pushData,
                textFieldName: 'Name',
                checkbox: false,
                idFieldName: 'No',
                parentIDFieldName: 'ParentNo',
                parentIcon: 'folder',
                childIcon: 'leaf',
                nodeWidth: 500,
                single: true,
                onSelect: onSelect
            });
        }
    }
}
function onSelect(note) {
    if (note.data.MenuType == 4) {
        var fk_flow = note.data.Flag.replace('Flow', '');
        if (note.data.IsStart == "1") {
            f_addTab(fk_flow, "我发起的" + note.data.Name + "历史流程", "StartHistory.aspx?FK_Flow=" + fk_flow + "&Name=" + note.data.Name);
            return;
        }
        else if (note.data.IsStart == "0") {
            f_addTab(fk_flow, "我参与的" + note.data.Name + "历史流程", "../../Rpt/Search.aspx?DoType=My&FK_Flow=" + fk_flow);
            return;
        }
    }
}