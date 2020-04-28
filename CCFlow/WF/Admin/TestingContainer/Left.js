
        //页面启动函数.
        //var adminer = GetQueryString("Adminer"); //管理员.
        var sid = GetQueryString("SID"); //管理员SID.
       // var workID = GetQueryString("WorkID"); 
        var userNo = GetQueryString("UserNo"); 
        var flowNo = GetQueryString("FK_Flow");
       // var urlEnd = "&FK_Flow=" + flowNo + "&WorkID=" + workID + "&UserNo=" + userNo + "&SID=" + sid;
       function InitPageUserInfo() {
           webUserJsonString = null;

           var webUser = new WebUser();
          

            var html = "<table style='width:100%;'>";
            html += "<caption>当前用户</caption>";

            html += "<tr>";
            html += "<td colspan=2 style='text-align:center'>";
            html += "<img style='width:50%;' src='../../../DataUser/UserIcon/" + webUser.No + ".png' onerror=\"this.src='../../../DataUser/UserIcon/Default.png'\" />";
            html += "</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td>帐号</td>";
            html += "<td>" + webUser.No + "</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td>用户</td>";
            html += "<td>" + webUser.Name + "</td>";
            html += "</tr>";

            html += "<tr>";
            html += "<td>部门</td>";
            html += "<td>" + webUser.FK_DeptName + "</td>";
            html += "</tr>";
            html += "</table>";

            $("#userInfo").html(html);

            
           var html = "<ul class='nav' id='side-menu'>";
           html +="<li>"
           html = "<ul style='border:solid 1px #C2D5E3;'>";
           html += "<li style='padding:5px;'><a href='javaScript:void(0)'  onclick='chageFramPage(this)' data-info='SelectOneUser.html?1=2" + urlEnd + "' class='J_menuItem' >切换用户</a></li>";
           html += "<li style='padding:5px;'><a href='javaScript:void(0)'  onclick='chageFramPage(this)' data-info='DBInfo.html?1=2" + urlEnd + "' class='J_menuItem' >数据库信息</a></li>";
           html += "<li style='padding:5px;'><a href='javaScript:void(0)'  onclick='chageFramPage(this)' data-info='../../WFRpt.htm?1=2" + urlEnd + "' class='J_menuItem' >轨迹图</a></li>";
           html += "<li style='padding:5px;'><a href='javascript:Restart();' >重新启动 </a></li>";
           html += "<li style='padding:5px;'><a href='javascript:LetAdminerLoginLeft();' >安全退出 </a></li>";
           html += "</ul>";
           html += "</li>";
           html += "</ul>";

            $("#Info").html(html);

       }
       function chageFramPage(obj) {
           var url = $(obj).attr('data-info');
           $("#J_iframe").attr('src', url);
           return false
       }



        //重新启动.
        function Restart() {

            if (window.confirm('您确认要使用[' + userNo + ']创建一个新的流程吗？') == false)
                return;
            // 使用最初用户登录
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_TestingContainer");
            handler.AddPara("FK_Emp", userNo);
            //handler.AddPara("WorkID", workID);

            //handler.AddPara("Adminer", adminer);
            handler.AddPara("SID", sid);

            var webUser = new WebUser();
            $("#userInfo").html(webUser.No + "," + webUser.Name);
            var data = handler.DoMethodReturnString("SelectOneUser_ChangUser");

            // 进入流程页面
            //var url = "Default.html?RunModel=1&FK_Flow=" + flowNo + "&SID=" + sid + "&UserNo=" + userNo;
           // window.location.href = url;
            //访问后台，获得一个工作ID.
            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_TestingContainer");
            handler.AddUrlData();
            workID = handler.DoMethodReturnString("Default_Init");
            if (workID.indexOf('err@') == 0) {
                var url = 'error.htm?err=' + workID;

                confirm("测试容器发起错误，请参考以下信息：<\br>" + workID);

                // window.open(url, '错误信息', 'height=500,width=600,top=200,left=500,toolbar=no,menubar=no,scrollbars=no, resizable=no,location=no, status=no');

                window.close();

            }
            
            urlEnd = "&FK_Flow=" + flowNo + "&WorkID=" + workID + "&UserNo=" + userNo + "&SID=" + adminerSID;
            InitPageUserInfo();
            document.getElementById("J_iframe").src = "../../MyFlow.htm?FK_Flow=" + flowNo + "&WorkID=" + workID;
        }
      
        // 选择接收人.
        function SelectOneUser() {

            var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_TestingContainer");
            handler.AddUrlData();
            var workid = handler.DoMethodReturnString("Default_Init");

            var url = "SelectOneUser.html?1=2" + urlEnd;
            $("#J_iframe").attr('src', url);
          
        }
        //如果关闭的时候，就让admin登录.
        function LetAdminerLoginLeft() {

            if (window.confirm('您确定要退出到管理员[]吗？') == false)
                return;

            LetAdminerLogin();
        //window.parent.window.close();

        ////访问后台，获得一个工作ID.
        //var handler = new HttpHandler("BP.WF.HttpHandler.WF_Admin_TestingContainer");
        //handler.AddUrlData();
        //var data = handler.DoMethodReturnString("Default_LetAdminerLogin");

        //if (data.indexOf('err@') == 0) {
        //    alert(data);
        //    return;
        //}

        return;
    }
