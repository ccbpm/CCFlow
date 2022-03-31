/**
 * 设置栅栏格数量
 * @param {表单ID 比如ND101 } frmID
 * @param {字段ID 比如BillNo } ctrlID
 * @param {跨的列数} col
 */
function SetCtrlColSpan(frmID, ctrlID, col)
{
    var mypk = frmID + "_" + ctrlID;
    var en = new Entity("BP.Sys.FrmUI.MapAttrString", mypk);
    en.ColSpan = col;
    en.Update();
}