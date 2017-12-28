
function DoAutoTo( fk_emp, empName )
{
   if (window.confirm('您确定要把您的工作授权给['+fk_emp+']吗？')==false)
       return;

    var url='Do.aspx?DoType=AutoTo&FK_Emp='+fk_emp;
    WinShowModalDialog(url,'');
    alert('授权成功，请别忘记收回。'); 
    window.location.href='Tools.aspx';
}

function ExitAuth(fk_emp) {
    if (window.confirm('您确定要退出授权登陆模式吗？') == false)
        return;
    var url = 'Do.aspx?DoType=ExitAuth&FK_Emp=' + fk_emp;
    WinShowModalDialog(url, '');
    window.location.href = 'Tools.aspx';
}

function TakeBack( fk_emp )
{
   if (window.confirm('您确定要取消对['+fk_emp+']的授权吗？')==false)
       return;
    var url='Do.aspx?DoType=TakeBack';
    WinShowModalDialog(url,'');
    alert('您已经成功的取消。'); 
    window.location.reload();
}

function LogAs( fk_emp )
{
   if (window.confirm('您确定要以['+fk_emp+']授权方式登陆吗？')==false)
       return;
       
    var url='Do.aspx?DoType=LogAs&FK_Emp='+fk_emp;
    WinShowModalDialog(url,'');
    alert('登陆成功，现在您可以以['+fk_emp+']处理工作。'); 
    window.location.href='EmpWorks.aspx';
}

function CHPass()
{
    var url='Do.aspx?DoType=TakeBack';
   // WinShowModalDialog(url,'');
    alert('密码修改成功，请牢记您的新密码。'); 
}