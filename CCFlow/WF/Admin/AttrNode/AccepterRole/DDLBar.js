
function InitBar() {

    var html = "请选择:";
    html += "<select >";

    html += "<option value=null >+组织结构</option>";
    html += "<option value=0 >   按岗位智能计算</option>";
    html += "<option value=1 >   按照部门计算</option>";

    html += "<option value=null >+按访问规则选项</option>";
    html += "<option value=0 >   与开始节点相同</option>";
    html += "<option value=1 >   与开始节点相同</option>";
    html += "</select >";

    html += "<input type=button onclick='Save()' value='保存' />";
    html += "<input type=button onclick='SaveAndClose()' value='保存&关闭' />";

    document.getElementById("bar").appendChild(html);
}

function SaveAndClose() {

    Save();
    window.close();
}