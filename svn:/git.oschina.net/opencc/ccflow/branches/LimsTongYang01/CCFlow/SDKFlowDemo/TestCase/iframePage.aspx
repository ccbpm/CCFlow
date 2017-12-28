<%@ Page Title="" Language="C#" MasterPageFile="~/SDKFlowDemo/WinOpen.master" AutoEventWireup="true" CodeBehind="iframePage.aspx.cs" Inherits="CCFlow.SDKFlowDemo.TestCase.iframePage" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">

<script type="text/javascript">
    function SaveDtlData() {
        alert('SaveDtlData的方法已经被执行了.');
//        var div = document.getElementById('Msg');
//        var dt = new Date();
//        div.innerHTML = '执行了保存方法:时间' + dt.getTime();
    }
</script>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">

<fieldset>
<legend>关于框架的使用说明</legend>
<ul>
<li>如果当现有ccform功能不能100%的满足用户的功能时候，或者需要展示其他系统的数据界面时，就需要使用框架来实现，特殊的个性开发问题。</li>
<li>框架可以解决特殊的个性化需求，如果ccform解决不了，就考虑使用框架。</li>
<li>ccform与框架内部有保存接口，失去焦点接口。就是说：当用户点击保存时，系统就会触发框架内部网页自定义的Save函数，开发者只要重写这个函数就可以完成保存。同样在离开焦点的时候也会触发执行Save函数，从而让她自动保存。</li>
</ul>
</fieldset>


<fieldset>
<legend>测试区域</legend>
<div id="Msg">当您点击保存按钮时，系统会出发该page上的sava javascript function ，只要你重写这个方法，就可以完成保存。 </div>
</fieldset>

 
 <br>
 该页面位于:/SDKFlowDemo/TestCase/iframePage.aspx

</asp:Content>
