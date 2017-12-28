function ChangeState(s) {
    val=s.value;
    alert(val);

    var objDDL= ReqDDLObj('FK_SF');

    if (val=='1')
           objDDL.Enable=true;
    else
         objDDL.Enable=false;
 
 
} 
