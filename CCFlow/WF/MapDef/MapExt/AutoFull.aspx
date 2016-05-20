<%@ Page Title="" Language="C#" MasterPageFile="~/WF/MapDef/WinOpen.master" AutoEventWireup="true" CodeBehind="AutoFull.aspx.cs" Inherits="CCFlow.WF.MapDef.AutoFullUI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">


    <table style="width: 100%;">
        <caption>为文本框【<%=this.Request.QueryString["RefNo"] %>】设置自动完成. </caption>
        <tr>
            <td style="width: 70%;" valign="top">
                <fieldset>
                    <legend>
                        <asp:RadioButton ID="RB_0" runat="server" Text="不设置(默认)" GroupName="xxxx" />
                    </legend>
                    <ul>
                        <li>不做任何处理.</li>
                    </ul>
                </fieldset>

                <fieldset>
                    <legend>
                        <asp:RadioButton ID="RB_1" runat="server" Text="使用Javascript 四则混合运算." GroupName="xxxx" /></legend>

                    <a href="javascript:ShowHidden('exp')">请输入表达式: </a>
                    <div id="exp" style="color: Gray; display: none">
                        <ul>
                            <li>用于处理主表同一个表单内的文本框与其他文本框数值的四则混合运算，或者明细表的一行数据的内的字段四则混合运算。</li>
                            <li>格式：@中文字段1 + @中文字段2  或者: @Filed1 + @Field2  </li>
                            <li>比如1：当前的文本框为“应纳税额” 您可以设置  @发票金额 * @增值税税率  也可设置为  @FPJE*@ZZSSL  </li>
                            <li>比如2：当前的文本框为“小计” 您可以设置  @数量*@单价  也可设置为  @ShuLiang*@DanJia  </li>
                            <li>重要提示: 表达式支持中文与英文的编写，但是需要特别注意，一个字段不能包含另外一个字段，这样就会出现错误。</li>
                            <li>比如： @Field1 与 @Field11 系统就区分不出来，因为@Field11 包含 @Field1  ，它会导致计算紊乱，所以需要正确的起名字。 </li>
                        </ul>
                    </div>

                    <asp:TextBox ID="TB_Exp" runat="server" TextMode="MultiLine" Rows="3" ToolTip="点击标签显示帮助." Width="98%"></asp:TextBox>
                </fieldset>
            </td>

        </tr>

        <tr>
            <td colspan="1">
                <asp:Button ID="Btn_Save" runat="server" Text="保存" OnClick="Btn_Save_Click" />
                <asp:Button ID="Btn_SaveAndClose" runat="server" Text="保存并关闭"
                    OnClick="Btn_SaveAndClose_Click" />
                <input value="关闭" type="button" onclick="javascript: window.close();" />
            </td>
        </tr>



    </table>

</asp:Content>
