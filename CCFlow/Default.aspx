<%@ Page Language="C#" AutoEventWireup="true" Inherits="CCFlow.Default" CodeBehind="Default.aspx.cs" %>

<html>
<head>
    <title>ccbpm导航页</title>
    <link href="DataUser/Style/ccbpm.css" rel="stylesheet" type="text/css" />
    <style type="text/css">
        body {
        }

        li {
            font-size: 14px;
            margin: 5px;
            border-style: none;
            color: Gray;
        }
    </style>
    <base target="_blank" />
</head>

<body>
    <center>
    <table style="text-align: center; border: 1px; width: 70%;">
        <tr>
            <td>
                <a href="http://ccflow.org" target="_blank" >
                <img src="../DataUser/Icon/LogBig.png"  style="width:30%;  float:left; "/>
                </a>
                <div  style=" float:right;text-align:center">
                    <br />
                    <h1>驰骋BPM&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</h1></div>
                 
            </td>
        </tr>

        <tr>
            <td style="text-align: left">
                <div style="float:left">
                <ul>
                    <li><font size="30" color="green"><a href="WF/Admin/CCBPMDesigner/Login.htm" ><b>进入流程设计器</b></a></font>用户名:admin 密码:123 </li>
                    <li><a href='./WF/AppClassic/Login.htm'>前台登录-经典模式</a> </li>
                    <li>  手机端演示,扫描关注：=》开发者=》手机端 </li>
                </ul>
                    </div>

                <div style="float:right">
                <ul>
                    <li><a href="http://ccflow.org" target="_blank">ccflow 官方网站</a></li>
                    <li>商务问题 QQ:<span class="style4">793719823</span> 电话:<span class="style5">0531-82374939 , 18660153393</span>  - <a href="http://ccflow.org/WeiXin/About.htm" target="_blank">常见问题</a> </li>
                </ul>
                    </div>
            </td>
        </tr>

        <tr>
            <td><h3>相关资源</h3></td>
        </tr>

        <tr>
            <td>
                   <div style="float:left">
                <ul style=" voice-duration:initial ">
                    <li><a href="http://ccbpm.mydoc.io/">驰骋流程引擎文档</a></li>
                    <li><a href="http://ccform.mydoc.io/">驰骋表单引擎文档</a></li>
                    <li><a href="http://ccflow.ke.qq.com/">视频教程</a></li>
                </ul>
                    </div>

                <div style="float:right">
                       <ul style=" voice-duration:initial ">
                    <li><a href="http://ccflow.org/docs/" target="_blank">最新文档 - 下载地址</a> </li>
                    <li><a href="http://ccflow.org/Down.htm" target="_blank" >多语言，多版本下载.</a></li>
                </ul>

                    </div>


            </td>
        </tr>

        <tr>
            <td><h3>系统演示的组织结构</h3>  </td>
        </tr>

        <tr>

            <td style="text-align: center">
                <div style="float: left">
                    <table>
                        <tr>
                            <td>登录帐号</td>
                            <td>密码</td>
                            <td>名称</td>
                            <td>部门</td>
                            <td>岗位</td>
                        </tr>

                        <tr>
                            <td>zhoupeng</td>
                            <td>123</td>
                            <td>周朋</td>
                            <td>总部</td>
                            <td>总经理</td>
                        </tr>

                        <tr>
                            <td>zhanghaicheng</td>
                            <td>123</td>
                            <td>张海成</td>
                            <td>市场部</td>
                            <td>市场部经理</td>
                        </tr>

                        <tr>
                            <td>zhangyifan</td>
                            <td>123</td>
                            <td>张一帆</td>
                            <td>市场部</td>
                            <td>销售人员岗</td>
                        </tr>

                        <tr>
                            <td>zhoushengyu</td>
                            <td>123</td>
                            <td>周升雨</td>
                            <td>市场部</td>
                            <td>销售人员岗</td>
                        </tr>


                        <tr>
                            <td>qifenglin</td>
                            <td>123</td>
                            <td>祁凤林</td>
                            <td>研发部</td>
                            <td>研发部经理</td>
                        </tr>

                        <tr>
                            <td>zhoutianjiao</td>
                            <td>123</td>
                            <td>周天娇</td>
                            <td>研发部</td>
                            <td>程序员岗</td>
                        </tr>

                        <tr>
                            <td>guoxiangbin</td>
                            <td>123</td>
                            <td>郭祥斌</td>
                            <td>服务部</td>
                            <td>客服部经理</td>
                        </tr>

                        <tr>
                            <td>fuhui</td>
                            <td>123</td>
                            <td>福惠</td>
                            <td>服务部</td>
                            <td>技术服务岗</td>
                        </tr>

                        <tr>
                            <td>yangyilei</td>
                            <td>123</td>
                            <td>杨依雷</td>
                            <td>财务部</td>
                            <td>财务部经理</td>
                        </tr>

                        <tr>
                            <td>guobaogeng</td>
                            <td>123</td>
                            <td>郭宝庚</td>
                            <td>财务部</td>
                            <td>出纳岗</td>
                        </tr>

                        <tr>
                            <td>liping</td>
                            <td>123</td>
                            <td>李萍</td>
                            <td>人力资源部</td>
                            <td>人力资源部经理</td>
                        </tr>

                        <tr>
                            <td>liyan</td>
                            <td>123</td>
                            <td>李言</td>
                            <td>人力资源部</td>
                            <td>人力资源助理岗</td>
                        </tr>
                    </table>
                </div>

                <div style="float: right; width: 50%;">

                    <fieldset>
                        <legend>扫描微信关注ccbpm最新动态</legend>
                        <img src="http://ccflow.org/WeiXin/WeiXinBiger.jpg" width="300px;" />
                    </fieldset>
                </div>

            </td>
        </tr>

        <tr>
            <td style="text-align: center">济南驰骋信息技术有限公司 @2003-2019</td>
        </tr>
    </table>
        </center>

</body>
</html>
