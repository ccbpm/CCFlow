/*
 * 说用： 开发者:lizhen.
 *  1. 该文件被引用到，接受人规则，中按照SQL模式./
 *  2. 被引用位置： /WF/Admin/AttrNode/AccepterRole/2.BySQL.htm
 *  2. 系统预制了一些sql模版，以方便用户调用.
 *  3. 
 * */

var templates
[

    
    SELECT No, Name FROM Port_Emp WHERE FK_Dept = '@WebUser.FK_Dept' 

   发送给本部门里所有人员(不包含自己):
    SELECT No, Name FROM Port_Emp WHERE FK_Dept = '@WebUser.FK_Dept' AND No != '@WebUser.No'
]


