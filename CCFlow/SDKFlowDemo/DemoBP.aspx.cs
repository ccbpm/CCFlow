using System;
using System.Data;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using BP.WF;
using BP.WF.Template;
using BP.En;
using BP.DA;
using BP.Port;
using BP.WF.Template;
using BP.Demo.BPFramework;

public partial class SDKFlowDemo_DemoEntity : System.Web.UI.Page
{
    /// <summary>
    /// ccbpm API开发 Demo
    /// </summary>
    public void ccbpmDemo()
    {
        // 让用户登录.
        BP.WF.Dev2Interface.Port_Login("liyan");

        //定义一个测试的流程编号.
        string flowNo = "001";

        #region ccbpm 的最简单的API操作。
        //创建一个workid.
        Int64 workid = BP.WF.Dev2Interface.Node_CreateBlankWork(flowNo);
        
        //执行开始节点的发发送,获得一个发送对象.
        BP.WF.SendReturnObjs objs=  BP.WF.Dev2Interface.Node_SendWork(flowNo, workid);
        //输出发送对象,流程在发送的时候会把发送的过程记录在这个对象里，比如：发送给谁？发送到那个节点了？提示的信息是什么？是否发送成功？
        foreach (BP.WF.SendReturnObj obj in objs)
        {
            this.Response.Write("Key=" + obj.MsgFlag + " Value=" + obj.MsgOfText);
        }

        //获取发送到节点ID.
        int nextNodeID = objs.VarToNodeID;
        
        //获取接受人, 多个的话，用逗号分开的，比如： zhangsan,lisi  
        string nextWorker=  objs.VarAcceptersID;
        
        //如果发送人这个时间感到发送错误了，忘记了填写东西 ,需要撤销，就执行撤销API.
        //if (1 == 2)
        //{
        //    //执行撤销API.
        //    BP.WF.Dev2Interface.Flow_DoUnSend(flowNo, workid);
        //}

        ////让下一个接受人登录, 让其执行发送。
        //BP.WF.Dev2Interface.Port_Login(nextWorker);

        //// 如果你要退回，就执行退回接口.
        //if (1 == 2)
        //{
        //    BP.WF.Dev2Interface.Node_ReturnWork(flowNo, workid, 0, nextNodeID, 101, "退回测试", false);
        //    return;
        //}

        //// 如果你要删除，就执行删除接口.
        //if (1 == 2)
        //{
        //    BP.WF.Dev2Interface.Flow_DoDeleteFlowByReal(flowNo, workid, false);
        //    return;
        //}

        //// 如果你要移交，就执行移交接口.
        //if (1 == 2)
        //{
        //    BP.WF.Dev2Interface.Node_Shift(flowNo,nextNodeID,workid,0,"fuhui","移交测试.");
        //    return;
        //}

        // 执行下一步发送。
        objs = BP.WF.Dev2Interface.Node_SendWork(flowNo, workid);
        //objs.VarWorkID
        #endregion ccbpm 的最简单的API操作。

    }

    protected void Page_Load(object sender, EventArgs e)
    {

        BP.Demo.BPFramework.Student st = new BP.Demo.BPFramework.Student();
        st.No = "sssss";
        st.Name = "xcxcvasd";
        st.Insert();

        Student en = new Student("sssss");

    }
    /// <summary>
    /// 写入日志
    /// </summary>
    public void WriteLogApp()
    {
        // 写入一条消息.
        BP.Sys.Glo.WriteLineInfo("这是一条消息. ");

        // 写入一条警告.
        BP.Sys.Glo.WriteLineWarning("这是一条警告. ");

        // 写入一条异常或者错误.
        BP.Sys.Glo.WriteLineError("这是一条错误. ");  // 以上这些日志写入了 \DataUser\Log\*.*

        //写入用户日志
        BP.Sys.Glo.WriteUserLog("Login", "stone", "系统登录");
        BP.Sys.Glo.WriteUserLog("Login", "stone", "系统登录", "192.168.1.100");  // 以上用户日志写入了 Sys_UserLog 表里.)
    }
    /// <summary>
    /// 全局的基本应用,获取当前操作员的信息.
    /// </summary>
    public void GloBaseApp()
    {
        // 执行登陆。
        BP.WF.Dev2Interface.Port_Login("guobaogeng");

        // 当前登陆人员编号.
        string currLoginUserNo = BP.Web.WebUser.No;
        // 登陆人员名称
        string currLoginUserName = BP.Web.WebUser.Name;
        // 登陆人员部门编号.
        string currLoginUserDeptNo = BP.Web.WebUser.FK_Dept;
        // 登陆人员部门名称
        string currLoginUserDeptName = BP.Web.WebUser.FK_DeptName;

        //以上可以用到ccflow的表达式里, 使用@WebUser.No, @WebUser.Name, @WebUser.FK_Dept, @WebUser.FK_DeptName, 
        // 表达式里就会自动替换以上变量.

        //退出.
        BP.WF.Dev2Interface.Port_SigOut();
    }
    /// <summary>
    /// 数据库操作访问
    /// </summary>
    public void DataBaseAccess()
    {
        #region 执行不带有参数.
        // 执行Insert ,delete, update 语句.
        int result = BP.DA.DBAccess.RunSQL("DELETE FROM Port_Emp WHERE 1=2");

        // 执行多个sql
        string sqls = "DELETE FROM Port_Emp WHERE 1=2";
        sqls += "@DELETE FROM Port_Emp WHERE 1=2";
        sqls += "@DELETE FROM Port_Emp WHERE 1=2";
        BP.DA.DBAccess.RunSQLs(sqls);

        //执行查询返回datatable.
        string sql = "SELECT * FROM Port_Emp";
        DataTable dt = BP.DA.DBAccess.RunSQLReturnTable(sql);

        //执行查询返回 string 值.
        sql = "SELECT FK_Dept FROM Port_Emp WHERE No='" + BP.Web.WebUser.No + "'";
        string fk_dept = BP.DA.DBAccess.RunSQLReturnString(sql);

        //执行查询返回 int 值. 也可以返回float, string
        sql = "SELECT count(No) as Num FROM Port_Emp ";
        int empNum = BP.DA.DBAccess.RunSQLReturnValInt(sql);

        //运行存储过程.
        string spName = "MySp";
        BP.DA.DBAccess.RunSP(spName);
        #endregion 执行不带有参数.

        #region 执行带有参数.
        // 执行Insert ,delete, update 语句.
        // 已经明确数据库 sqlserver 类型.
        Paras ps = new Paras();
        ps.SQL = "DELETE FROM Port_Emp WHERE No=@UserNo";
        ps.Add("UserNo", "abc");
        BP.DA.DBAccess.RunSQL(ps);

        // 不知道数据库类型.
        ps = new Paras();
        ps.SQL = "DELETE FROM Port_Emp WHERE No=" + BP.Sys.SystemConfig.AppCenterDBVarStr + "UserNo";
        ps.Add("UserNo", "abc");
        BP.DA.DBAccess.RunSQL(ps);


        //执行查询返回datatable.
        ps = new Paras();
        ps.SQL = "SELECT * FROM Port_Emp WHERE FK_Dept=@DeptNoVar";
        ps.Add("DeptNoVar", "0102");
        DataTable dtDept = BP.DA.DBAccess.RunSQLReturnTable(ps);

        //运行存储过程.
        ps = new Paras();
        ps.Add("DeptNoVar", "0102");
        spName = "MySp";
        BP.DA.DBAccess.RunSP(spName, ps);
        #endregion 执行带有参数.

        #region 向数据库存储、读写文件.
        //写入实例.
        string tmpFile = "c:\\111.txt";
        BP.DA.DBAccess.SaveFileToDB(tmpFile, "WF_Flow", "No", "001", "FlowJson"); // FlowJson是一个已经存在的img字段.

        //读取文件到指定的位置.
        string saveToFile = "c:\\222.txt";
        byte[] byt = BP.DA.DBAccess.GetFileFromDB(saveToFile, "WF_Flow", "No", "001", "FlowJson"); 
        // FlowJson是一个已经存在的img字段, 系统就会生成一个 temp文件在指定的路径下. 返回的是一个byt 类型的数据流.
        #endregion 向数据库存储文件.

    }
   
    /// <summary>
    /// Entity 的基本应用.
    /// </summary>
    public void EntityBaseApp()
    {
        #region  直接插入一条数据.
        BP.Port.Emp emp = new BP.Port.Emp();
        emp.CheckPhysicsTable();
        /*  检查物理表是否与Map一致 
         *  1，如果没有这个物理表则创建。
         *  2，如果缺少字段则创建。
         *  3，如果字段类型不一直则删除创建，比如原来是int类型现在map修改成string类型。
         *  4，map字段减少则不处理。
         *  5，手工的向物理表中增加的字段则不处理。
         *  6，数据源是视图字段不匹配则创建失败。
         * */
        emp.No = "zhangsan";
        emp.Name = "张三";
        emp.FK_Dept = "01";
        emp.Pass = "pub";
        emp.Insert();  // 如果主键重复要抛异常。
        #endregion  直接插入一条数据.

        #region  保存的方式插入一条数据.
        emp = new BP.Port.Emp();
        emp.No = "zhangsan";
        emp.Name = "张三";
        emp.FK_Dept = "01";
        emp.Pass = "pub";
        emp.Save();  // 如果主键重复直接更新，不会抛出异常。
        #endregion  保存的方式插入一条数据.

        #region  其他方法.
        BP.Port.Emp  myEmp2 = new BP.Port.Emp();
        myEmp2.No = "zhangsan";

        //检查主键数据是否存在 ? 
        bool isExit = myEmp2.IsExits;
        if (myEmp2.RetrieveFromDBSources() == 0)
        {
            /*说明没有查询到数据。*/
        }
        #endregion.


        #region  数据复制.
        /*
         * 如果一个实体与另外的一个实体两者的属性大致相同，就可以执行copy.
         *  比如：在创建人员时，张三与李四两者只是编号与名称不同，只是改变不同的属性就可以执行相关的业务操作。
         */
        Emp emp1 = new BP.Port.Emp("zhangsan");
        emp = new BP.Port.Emp();
        emp.Copy(emp1); // 同实体copy, 不同的实体也可以实现copy.
        emp.No = "lisi";
        emp.Name = "李四";
        emp.Insert();
       
        // copy 在业务逻辑上会经常应用，比如: 在一个流程中A节点表单与B节点表单字段大致相同，ccflow就是采用的copy方式处理。
        #endregion  数据复制.

        #region 单个实体查询.
        string msg = "";     // 查询这条数据.
        BP.Port.Emp myEmp = new BP.Port.Emp();
        myEmp.No = "zhangsan";
        if (myEmp.RetrieveFromDBSources() == 0)  // RetrieveFromDBSources() 返回来的是查询数量.
        {
            this.Response.Write("没有查询到编号等于zhangsan的人员记录.");
            return;
        }
        else
        {
            msg = "";
            msg += "<BR>编号:" + myEmp.No;
            msg += "<BR>名称:" + myEmp.Name;
            msg += "<BR>密码:" + myEmp.Pass;
            msg += "<BR>部门编号:" + myEmp.FK_Dept;
            msg += "<BR>部门名称:" + myEmp.FK_DeptText;
            this.Response.Write(msg);
        }

        myEmp = new BP.Port.Emp();
        myEmp.No = "zhangsan";
        myEmp.Retrieve(); // 执行查询，如果查询不到则要抛出异常。

        msg = "";
        msg += "<BR>编号:" + myEmp.No;
        msg += "<BR>名称:" + myEmp.Name;
        msg += "<BR>密码:" + myEmp.Pass;
        msg += "<BR>部门编号:" + myEmp.FK_Dept;
        msg += "<BR>部门名称:" + myEmp.FK_DeptText;
        this.Response.Write(msg);
        #endregion 查询.

        #region 两种方式的删除。
        // 删除操作。
        emp = new BP.Port.Emp();
        emp.No = "zhangsan";
        int delNum = emp.Delete(); // 执行删除。
        if (delNum == 0)
            this.Response.Write("删除 zhangsan 失败.");

        if (delNum == 1)
            this.Response.Write("删除 zhangsan 成功..");
        if (delNum > 1)
            this.Response.Write("不应该出现的异常。");
        // 初试化实例后，执行删除，这种方式要执行两个sql.
        emp = new BP.Port.Emp("abc");
        emp.Delete();
        
        #endregion 两种方式的删除。

        #region 更新。
        emp = new BP.Port.Emp("zhangyifan"); // 事例化它.
        emp.Name = "张一帆123"; //改变属性.
        emp.Update();   // 更新它，这个时间BP将会把所有的属性都要执行更新，UPDATA 语句涉及到各个列。

        emp = new BP.Port.Emp("fuhui"); // 事例化它.
        emp.Update("Name", "福慧123");   //仅仅更新这一个属性。.UPDATA 语句涉及到Name列。
        #endregion 更新。
    }
    /// <summary>
    /// Entities 的基本应用.
    /// </summary>
    public void EntitiesBaseApp()
    {
        #region 查询全部
        /* 查询全部分为两种方式，1 从缓存里查询全部。2，从数据库查询全部。  */
        Emps emps = new Emps();
        int num = emps.RetrieveAll(); //从缓存里查询全部数据.

        this.Response.Write("RetrieveAll查询出来(" + num + ")个");
        foreach (Emp emp in emps)
        {
            this.Response.Write("<hr>人员名称:" + emp.Name);
            this.Response.Write("<br>人员编号:" + emp.No);
            this.Response.Write("<br>部门编号:" + emp.FK_Dept);
            this.Response.Write("<br>部门名称:" + emp.FK_DeptText);
        }

        //把entities 数据转入到DataTable里。
        DataTable empsDTfield = emps.ToDataTableField(); //以英文字段做为列名。
        DataTable empsDTDesc = emps.ToDataTableDesc(); //以中文字段做为列名。
       
        // 从数据库里查询全部。
        emps = new Emps();
        num = emps.RetrieveAllFromDBSource();
        this.Response.Write("RetrieveAllFromDBSource查询出来(" + num + ")个");
        foreach (Emp emp in emps)
        {
            this.Response.Write("<hr>人员名称:" + emp.Name);
            this.Response.Write("<br>人员编号:" + emp.No);
            this.Response.Write("<br>部门编号:" + emp.FK_Dept);
            this.Response.Write("<br>部门名称:" + emp.FK_DeptText);
        }
        #endregion 查询全部

        #region 按条件查询
        // 单个条件查询。
        Emps myEmps = new Emps();
        QueryObject qo = new QueryObject(myEmps);
        qo.AddWhere(EmpAttr.FK_Dept, "01");
        qo.addOrderBy(EmpAttr.No); // 增加排序规则,Order  OrderByDesc, addOrderByDesc addOrderByRandom. 
        num = qo.DoQuery();  // 返回查询的个数.
        this.Response.Write("查询出来(" + num + ")个，部门编号=01的人员。");
        foreach (Emp emp in myEmps)
        {
            this.Response.Write("<hr>人员名称:" + emp.Name);
            this.Response.Write("<br>人员编号:" + emp.No);
            this.Response.Write("<br>部门编号:" + emp.FK_Dept);
            this.Response.Write("<br>部门名称:" + emp.FK_DeptText);
        }

     //   DataTable mydt = qo.DoQueryToTable();  // 查询出来的数据转入到datatable里。.

        Emps myEmp1s = new Emps();
        myEmp1s.Retrieve(EmpAttr.FK_Dept, "01");
        foreach (Emp  item in myEmp1s)
        {
            this.Response.Write("<hr>人员名称:" + item.Name);
            this.Response.Write("<br>人员编号:" + item.No);
            this.Response.Write("<br>部门编号:" + item.FK_Dept);
            this.Response.Write("<br>部门名称:" + item.FK_DeptText);
        }

        // 多个条件查询。
        myEmps = new Emps();
        qo = new QueryObject(myEmps);
        qo.AddWhere(EmpAttr.FK_Dept, "01");
        qo.addAnd();
        qo.AddWhere(EmpAttr.No, "guobaogen");
        num = qo.DoQuery();  // 返回查询的个数.
        this.Response.Write("查询出来(" + num + ")个，部门编号=01并且编号=guobaogen的人员。");
        foreach (Emp emp in myEmps)
        {
            this.Response.Write("<hr>人员名称:" + emp.Name);
            this.Response.Write("<br>人员编号:" + emp.No);
            this.Response.Write("<br>部门编号:" + emp.FK_Dept);
            this.Response.Write("<br>部门名称:" + emp.FK_DeptText);
        }
        // 具有括号表达式的查询。
        myEmps = new Emps();
        qo = new QueryObject(myEmps);
        qo.addLeftBracket(); // 加上左括号.
        qo.AddWhere(EmpAttr.FK_Dept, "01");
        qo.addAnd();
        qo.AddWhere(EmpAttr.No, "guobaogen");
        qo.addRightBracket();  // 加上右括号.
        num = qo.DoQuery();  // 返回查询的个数.
        this.Response.Write("查询出来(" + num + ")个，部门编号=01并且编号=guobaogen的人员。");
        foreach (Emp emp in myEmps)
        {
            this.Response.Write("<hr>人员名称:" + emp.Name);
            this.Response.Write("<br>人员编号:" + emp.No);
            this.Response.Write("<br>部门编号:" + emp.FK_Dept);
            this.Response.Write("<br>部门名称:" + emp.FK_DeptText);

        }
        // 具有where in 方式的查询。
        myEmps = new Emps();
        qo = new QueryObject(myEmps);
        qo.AddWhereInSQL(EmpAttr.No, "SELECT No FROM Port_Emp WHERE FK_Dept='02'");
        num = qo.DoQuery();  // 返回查询的个数.
        this.Response.Write("查询出来(" + num + ")个，WHERE IN (SELECT No FROM Port_Emp WHERE FK_Dept='02')人员。");
        foreach (Emp emp in myEmps)
        {
            this.Response.Write("<hr>人员名称:" + emp.Name);
            this.Response.Write("<br>人员编号:" + emp.No);
            this.Response.Write("<br>部门编号:" + emp.FK_Dept);
            this.Response.Write("<br>部门名称:" + emp.FK_DeptText);
        }

        // 具有LIKE 方式的查询。
        myEmps = new Emps();
        qo = new QueryObject(myEmps);
        qo.AddWhere(EmpAttr.No, " LIKE ", "guo");
        num = qo.DoQuery();  // 返回查询的个数.
        this.Response.Write("查询出来(" + num + ")个，人员编号包含guo的人员。");
        foreach (Emp emp in myEmps)
        {
            this.Response.Write("<hr>人员名称:" + emp.Name);
            this.Response.Write("<br>人员编号:" + emp.No);
            this.Response.Write("<br>部门编号:" + emp.FK_Dept);
            this.Response.Write("<br>部门名称:" + emp.FK_DeptText);
        }
        #endregion 按条件查询

        #region 集合业务处理.
        myEmps = new Emps();
        myEmps.RetrieveAll(); // 查询全部出来.
        // 遍历集合是常用的处理方法。
        foreach (Emp emp in myEmps)
        {
            this.Response.Write("<hr>人员名称:" + emp.Name);
            this.Response.Write("<br>人员编号:" + emp.No);
            this.Response.Write("<br>部门编号:" + emp.FK_Dept);
            this.Response.Write("<br>部门名称:" + emp.FK_DeptText);
        }
        // 判断是否包含是指定的主键值.
        bool isHave = myEmps.Contains("Name", "郭宝庚"); //判断是否集合里面包含Name=郭宝庚的实体.
        bool isHave1 = myEmps.Contains("guobaogeng"); //判断是否集合里面主键No=guobaogeng的实体.

        // 获取Name=郭宝庚的实体，如果没有就返回空。
        Emp empFind = myEmps.GetEntityByKey("Name", "郭宝庚") as Emp;
        if (empFind == null)
            this.Response.Write("<br>没有找到: Name =郭宝庚 的实体.");
        else
            this.Response.Write("<br>已经找到了: Name =郭宝庚 的实体. 他的部门编号="+empFind.FK_Dept+"，部门名称="+empFind.FK_DeptText);
        // 批量更新实体。
        myEmps.Update(); // 等同于下一个循环。
        foreach (Emp emp in myEmps)
            emp.Update();
        // 删除实体.
        myEmps.Delete(); // 等同于下一个循环。
        foreach (Emp emp in myEmps)
        {
            emp.Delete();
        }
        // 执行数据库删除，类于执行 DELETE Port_Emp WHERE FK_Dept='01' 的sql.
        myEmps.Delete("FK_Dept", "01");
        #endregion
    }
    /// <summary>
    /// 展示EnttiyNo自动编号
    /// </summary>
    public void EnttiyNo()
    {
        // 创建一个空的实体.
        BP.Demo.BPFramework.Student en = new BP.Demo.BPFramework.Student();
        // 给各个属性赋值，但是不要给编号赋值.
        en.Name = "张三";
        en.FK_BanJi = "001";
        en.Age = 19;
        en.XB = 1;
        en.Tel = "0531-82374939";
        en.Addr = "山东.济南.高新区";
        en.Insert(); //这里会自动给该学生编号，从0001开始，编号规则打开该类的Map.

        string xuehao = en.No;
        this.Response.Write("信息已经加入,该学生的学好为:" + xuehao);

        //查询出来该实体。
        BP.Demo.BPFramework.Student myen = new BP.Demo.BPFramework.Student(xuehao);

        this.Response.Write("学生姓名:" + myen.Name);
        this.Response.Write("地址:" + myen.Addr);
    }
    /// <summary>
    /// 实体OID
    /// </summary>
    public void EnttiyOID()
    {
        // 创建一个空的简历实体.
        BP.Demo.Resume dtl = new BP.Demo.Resume();
        dtl.FK_Stu = "zhangsan"; //给关联的主键赋值.
        dtl.NianYue = "2014年4月";
        dtl.GongZuoDanWei = "济南驰骋公司"; // 工作单位.
        dtl.ZhengMingRen = "李四";  //证明人,李四.
        dtl.Insert(); //这里会自动给该实体主键OID赋值， 他是一个自动增长的列.
        this.Response.Write("信息已经加入,OID:" + dtl.OID);

        //初始化该实体，并把它显示出来.
        BP.Demo.Resume mydtl = new BP.Demo.Resume(dtl.OID);
        this.Response.Write("工作单位:" + dtl.GongZuoDanWei + "证明人:" + dtl.ZhengMingRen);
    }
    /// <summary>
    /// 具有MyPK类型的实体，该类实体的主键是MyPK.
    /// 它的主键是本表的2个或者3个以上的字段组合得来的.
    /// </summary>
    public void EnttiyMyPK()
    {
        // 创建一个员工考核实体.
        BP.Demo.BPFramework.EmpCent en = new BP.Demo.BPFramework.EmpCent();
        en.FK_Emp = "zhangsan";
        en.FK_NY = "2003-01";
        en.MyPK = en.FK_NY + "_" + en.FK_Emp;
        en.Cent = 100;
        en.Insert();  // 插入到数据库里.
        this.Response.Write("信息已经加入,Cent:" + en.Cent);

        BP.Demo.BPFramework.EmpCent myen = new BP.Demo.BPFramework.EmpCent(en.MyPK);
        this.Response.Write("人员:" + myen.FK_Emp + ",月份:" + myen.FK_NY+", 得分:"+myen.Cent);
    }
    /// <summary>
    /// 树的实体包含了No,Name,ParentNo,Idx 必须的属性(字段),它是树结构的描述.
    /// </summary>
    public void EnttiyTree()
    {
        //创建父节点, 父节点的编号必须为1 ,父节点的ParentNo 必须是 0.
        FlowSort en = new FlowSort("1");
        en.Name = "根目录";

        //创建子目录节点.
        FlowSort subEn = (FlowSort)en.DoCreateSubNode();
        subEn.Name = "行政类";
        subEn.Update();

        //创建子目录的评级节点.
        FlowSort sameLevelSubEn = (FlowSort)subEn.DoCreateSameLevelNode();
        sameLevelSubEn.Name = "业务类";
        sameLevelSubEn.Update();

        //创建子目录的下一级节点1.
        FlowSort sameLevelSubSubEn = (FlowSort)subEn.DoCreateSameLevelNode();
        sameLevelSubSubEn.Name = "日常办公";
        sameLevelSubSubEn.Update();

        //创建子目录的下一级节点1.
        FlowSort sameLevelSubSubEn2 = (FlowSort)subEn.DoCreateSameLevelNode();
        sameLevelSubSubEn2.Name = "人力资源";
        sameLevelSubSubEn2.Update();
        /**
         *   根目录
         *     行政类
         *        日常办公
         *        人力资源
         *     业务类
         * 
         */
    }
    /// <summary>
    /// 多对多的关系,这种实体就有两个列(属性)
    /// 这俩个列都是外键,并且也是该表的主键.
    /// </summary>
    public void EnttiyMM()
    {
        BP.Port.EmpStation en = new BP.Port.EmpStation();
        en.FK_Emp = "zhangsan";
        en.FK_Station = "01";
        en.Insert();
    }
    /// <summary>
    /// 与文件相关的操作.
    /// </summary>
    public void EnttiyOptionWithFile()
    {
        //把一个文件存入到entity.
        string fileFullName = "c:\\temp.doc";
        Flow fl = new Flow("001");
        fl.SaveFileToDB("ABC",fileFullName); //如果没有ABC 字段，系统就会自动创建。


        //把流存入数据库entity.
        byte[] betys = null; ;
        fl = new Flow("001");
        fl.SaveFileToDB("ABC", betys); //如果没有ABC 字段，系统就会自动创建。

        //获取文件.
        string saveTo="c:\\tempfile.doc";
        fl.GetFileFromDB("ABC", saveTo);

        //特别说明，使用该方法，必须是已经存在数据库的一个实体，在没有插入之前，是不能调用的。
    }
}

 