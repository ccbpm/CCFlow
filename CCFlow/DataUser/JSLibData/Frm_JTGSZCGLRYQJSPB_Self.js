$(function () {
 
    var webuser = new WebUser();  
    var sql1 = "select name from port_dept where No = (select FK_Dept  from port_emp where no =" + webuser.No + ")";
    var dbs = DBAccess.RunSQLReturnTable(sql1);
    var bumen = dbs[0].Name;
    var sql2 = "select name,no,parentno from port_org where No = (select orgno  from port_emp where no =" + webuser.No + ")";
    var dbs2 = DBAccess.RunSQLReturnTable(sql2);
  
    var ejorg = dbs2[0].name;
    var parentno = dbs2[0].parentno;
    if (parentno == dbs2[0].no) {
        var yiorg = "";   
            }
    else {
        var sql3 = "select name from port_org where no=" + parentno;
        var dbs3 = DBAccess.RunSQLReturnTable(sql3);
        var yjorg = dbs3[0].name;

    }


    var danwei = yjorg + "." + ejorg + "." + bumen;
    $("#TB_SZSWBM").val(danwei);
    
}); 
