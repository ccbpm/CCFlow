
var col4Html = '';
col4Html += '<center><h3>通用的4列模式</h3>';
col4Html += '    <table style=width:90%;>';
col4Html += '<tbody>';

col4Html += '<tr>';
col4Html += '    <td  >申请人</td>';
col4Html += '    <td style=width:30% ></td>';
col4Html += '    <td  >申请日期</td>';
col4Html += '    <td  style=width:30% ></td>';
col4Html += '</tr>';
col4Html += '<tr>';
col4Html += '    <td >申请人部门</td>';
col4Html += '    <td colspan=3></td>';
col4Html += '</tr>';

col4Html += '<tr>';
col4Html += '    <td  >字段1</td>';
col4Html += '    <td style=width:30% ></td>';
col4Html += '    <td  >字段2</td>';
col4Html += '    <td  style=width:30% ></td>';
col4Html += '</tr>';

col4Html += '<tr>';
col4Html += '    <td  >字段3</td>';
col4Html += '    <td style=width:30% ></td>';
col4Html += '    <td  >字段4</td>';
col4Html += '    <td  style=width:30% ></td>';
col4Html += '</tr>';

col4Html += '</tbody>';
col4Html += '</table>';
col4Html += '</center> ';

var col6Html = '';
col6Html += '<center><h3>通用的6列模式</h3>';
col6Html += '    <table style=width:90%;>';
col6Html += '<tbody>';

col6Html += '<tr>';
col6Html += '    <td>申请人</td>';
col6Html += '    <td></td>';
col6Html += '    <td>申请日期</td>';
col6Html += '    <td></td>';
col6Html += '    <td>单据编号</td>';
col6Html += '    <td></td>';
col6Html += '</tr>';

col6Html += '<tr>';
col6Html += '    <td>字段1</td>';
col6Html += '    <td></td>';
col6Html += '    <td>字段2</td>';
col6Html += '    <td></td>';
col6Html += '    <td>字段3</td>';
col6Html += '    <td></td>';
col6Html += '</tr>';

col6Html += '<tr>';
col6Html += '    <td>字段4</td>';
col6Html += '    <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>';
col6Html += '    <td>字段5</td>';
col6Html += '    <td>&nbsp;&nbsp;&nbsp;</td>';
col6Html += '    <td>字段6</td>';
col6Html += '    <td>&nbsp;&nbsp;&nbsp;&nbsp;</td>';
col6Html += '</tr>';

col6Html += '</tbody>';
col6Html += '</table>';
col6Html += '</center> ';// JavaScript source code


var welcome = "";
welcome += "<h3>欢迎使用驰骋开发者表单，该表单由雷劈表单改造而来，特为作者表示感谢。</h3>";
welcome += "<ul>";
welcome += "<li>您可以定义表单模版,在 WF\Admin\DevelopDesigner\Template\config.js 文件中.</li>";
welcome += "<li>定义的模版可以显示在表单布局里面，可以方便您快速布局整体页面。</li>";
welcome += "<li>插入表单模版后您可以通过html进行微调。</li>";
welcome += "</ul>";

/**
 * Created with JetBrains PhpStorm.
 * User: xuheng
 * Date: 12-8-8
 * Time: 下午2:00
 * To change this template use File | Settings | File Templates.
 */
var templates = [
    {
        "pre": "images/pre0.png",
        'title': lang.blank,
        'preHtml': welcome,
        "html": welcome
    },
    {
        "pre": "../Template/col4.png",
        'title': "通用4列模式",
        'preHtml': col4Html,
        "html": col4Html
    }
    ,
    {
        "pre": "../Template/col6.png",
        'title': "通用6列模式",
        'preHtml': col6Html,
        "html": col6Html
    },
    {
        "pre": "images/pre1.png",
        'title': lang.blog,
        'preHtml': '<h1 label="Title center" name="tc" style="border-bottom-color:#cccccc;border-bottom-width:2px;border-bottom-style:solid;padding:0px 4px 0px 0px;text-align:center;margin:0px 0px 20px;"><span style="color:#c0504d;">深入理解Range</span></h1><p style="text-align:center;"><strong class=" ">UEditor二次开发</strong></p><h3><span class=" " style="font-family:幼圆">什么是Range</span></h3><p style="text-indent:2em;">对于“插入”选项卡上的库，在设计时都充分考虑了其中的项与文档整体外观的协调性。 </p><br /><h3><span class=" " style="font-family:幼圆">Range能干什么</span></h3><p style="text-indent:2em;">在“开始”选项卡上，通过从快速样式库中为所选文本选择一种外观，您可以方便地更改文档中所选文本的格式。</p>',
        "html": '<h1 class="ue_t" label="Title center" name="tc" style="border-bottom-color:#cccccc;border-bottom-width:2px;border-bottom-style:solid;padding:0px 4px 0px 0px;text-align:center;margin:0px 0px 20px;"><span style="color:#c0504d;">[键入文档标题]</span></h1><p style="text-align:center;"><strong class="ue_t">[键入文档副标题]</strong></p><h3><span class="ue_t" style="font-family:幼圆">[标题 1]</span></h3><p class="ue_t"  style="text-indent:2em;">对于“插入”选项卡上的库，在设计时都充分考虑了其中的项与文档整体外观的协调性。 您可以使用这些库来插入表格、页眉、页脚、列表、封面以及其他文档构建基块。 您创建的图片、图表或关系图也将与当前的文档外观协调一致。</p><h3><span class="ue_t" style="font-family:幼圆">[标题 2]</span></h3><p class="ue_t"  style="text-indent:2em;">在“开始”选项卡上，通过从快速样式库中为所选文本选择一种外观，您可以方便地更改文档中所选文本的格式。 您还可以使用“开始”选项卡上的其他控件来直接设置文本格式。大多数控件都允许您选择是使用当前主题外观，还是使用某种直接指定的格式。 </p><h3><span class="ue_t" style="font-family:幼圆">[标题 3]</span></h3><p class="ue_t">对于“插入”选项卡上的库，在设计时都充分考虑了其中的项与文档整体外观的协调性。 您可以使用这些库来插入表格、页眉、页脚、列表、封面以及其他文档构建基块。 您创建的图片、图表或关系图也将与当前的文档外观协调一致。</p><p class="ue_t"><br /></p>'

    },
    {
        "pre": "images/pre2.png",
        'title': lang.resume,
        'preHtml': '<h1 label="Title left" name="tl" style="border-bottom-color:#cccccc;border-bottom-width:2px;border-bottom-style:solid;padding:0px 4px 0px 0px;margin:0px 0px 10px;"><span style="color:#e36c09;" class=" ">WEB前端开发简历</span></h1><table width="100%" border="1" bordercolor="#95B3D7" style="border-collapse:collapse;"><tbody><tr><td width="100" style="text-align:center;"><p><span style="background-color:transparent;">插</span><br /></p><p>入</p><p>照</p><p>片</p></td><td><p><span style="background-color:transparent;"> 联系电话：</span><span class="ue_t" style="background-color:transparent;">[键入您的电话]</span><br /></p><p><span style="background-color:transparent;"> 电子邮件：</span><span class="ue_t" style="background-color:transparent;">[键入您的电子邮件地址]</span><br /></p><p><span style="background-color:transparent;"> 家庭住址：</span><span class="ue_t" style="background-color:transparent;">[键入您的地址]</span><br /></p></td></tr></tbody></table><h3><span style="color:#E36C09;font-size:20px;">目标职位</span></h3><p style="text-indent:2em;" class=" ">WEB前端研发工程师</p><h3><span style="color:#e36c09;font-size:20px;">学历</span></h3><p><span style="display:none;line-height:0px;" id="_baidu_bookmark_start_26">﻿</span></p><ol style="list-style-type:decimal;"><li><p><span class="ue_t">[起止时间]</span> <span class="ue_t">[学校名称] </span> <span class="ue_t">[所学专业]</span> <span class="ue_t">[所获学位]</span></p></li></ol><h3><span style="color:#e36c09;font-size:20px;" class="ue_t">工作经验</span></h3><p><br /></p>',
        "html": '<h1 label="Title left" name="tl" style="border-bottom-color:#cccccc;border-bottom-width:2px;border-bottom-style:solid;padding:0px 4px 0px 0px;margin:0px 0px 10px;"><span style="color:#e36c09;" class="ue_t">[此处键入简历标题]</span></h1><p><span style="color:#e36c09;"><br /></span></p><table width="100%" border="1" bordercolor="#95B3D7" style="border-collapse:collapse;"><tbody><tr><td width="200" style="text-align:center;" class="ue_t">【此处插入照片】</td><td><p><br /></p><p> 联系电话：<span class="ue_t">[键入您的电话]</span></p><p><br /></p><p> 电子邮件：<span class="ue_t">[键入您的电子邮件地址]</span></p><p><br /></p><p> 家庭住址：<span class="ue_t">[键入您的地址]</span></p><p><br /></p></td></tr></tbody></table><h3><span style="color:#e36c09;font-size:20px;">目标职位</span></h3><p style="text-indent:2em;" class="ue_t">[此处键入您的期望职位]</p><h3><span style="color:#e36c09;font-size:20px;">学历</span></h3><p><span style="display:none;line-height:0px;" id="_baidu_bookmark_start_26">﻿</span></p><ol style="list-style-type:decimal;"><li><p><span class="ue_t">[键入起止时间]</span> <span class="ue_t">[键入学校名称] </span> <span class="ue_t">[键入所学专业]</span> <span class="ue_t">[键入所获学位]</span></p></li><li><p><span class="ue_t">[键入起止时间]</span> <span class="ue_t">[键入学校名称]</span> <span class="ue_t">[键入所学专业]</span> <span class="ue_t">[键入所获学位]</span></p></li></ol><h3><span style="color:#e36c09;font-size:20px;" class="ue_t">工作经验</span></h3><ol style="list-style-type:decimal;"><li><p><span class="ue_t">[键入起止时间]</span> <span class="ue_t">[键入公司名称]</span> <span class="ue_t">[键入职位名称]</span> </p></li><ol style="list-style-type:lower-alpha;"><li><p><span class="ue_t">[键入负责项目]</span> <span class="ue_t">[键入项目简介]</span></p></li><li><p><span class="ue_t">[键入负责项目]</span> <span class="ue_t">[键入项目简介]</span></p></li></ol><li><p><span class="ue_t">[键入起止时间]</span> <span class="ue_t">[键入公司名称]</span> <span class="ue_t">[键入职位名称]</span> </p></li><ol style="list-style-type:lower-alpha;"><li><p><span class="ue_t">[键入负责项目]</span> <span class="ue_t">[键入项目简介]</span></p></li></ol></ol><p><span style="color:#e36c09;font-size:20px;">掌握技能</span></p><p style="text-indent:2em;"> &nbsp;<span class="ue_t">[这里可以键入您所掌握的技能]</span><br /></p>'

    },
    {
        "pre": "images/pre3.png",
        'title': lang.richText,
        'preHtml': '<h1 label="Title center" name="tc" style="border-bottom-color:#cccccc;border-bottom-width:2px;border-bottom-style:solid;padding:0px 4px 0px 0px;text-align:center;margin:0px 0px 20px;" class="ue_t">[此处键入文章标题]</h1><p><img src="http://img.baidu.com/hi/youa/y_0034.gif" width="150" height="100" border="0" hspace="0" vspace="0" style="width:150px;height:100px;float:left;" />图文混排方法</p><p>图片居左，文字围绕图片排版</p><p>方法：在文字前面插入图片，设置居左对齐，然后即可在右边输入多行文</p><p><br /></p><p><img src="http://img.baidu.com/hi/youa/y_0040.gif" width="100" height="100" border="0" hspace="0" vspace="0" style="width:100px;height:100px;float:right;" /></p><p>还有没有什么其他的环绕方式呢？这里是居右环绕</p><p><br /></p><p>欢迎大家多多尝试，为UEditor提供更多高质量模板！</p>',
        "html": '<p><br /></p><h1 label="Title center" name="tc" style="border-bottom-color:#cccccc;border-bottom-width:2px;border-bottom-style:solid;padding:0px 4px 0px 0px;text-align:center;margin:0px 0px 20px;" class="ue_t">[此处键入文章标题]</h1><p><img src="http://img.baidu.com/hi/youa/y_0034.gif" width="300" height="200" border="0" hspace="0" vspace="0" style="width:300px;height:200px;float:left;" />图文混排方法</p><p>1. 图片居左，文字围绕图片排版</p><p>方法：在文字前面插入图片，设置居左对齐，然后即可在右边输入多行文本</p><p><br /></p><p>2. 图片居右，文字围绕图片排版</p><p>方法：在文字前面插入图片，设置居右对齐，然后即可在左边输入多行文本</p><p><br /></p><p>3. 图片居中环绕排版</p><p>方法：亲，这个真心没有办法。。。</p><p><br /></p><p><br /></p><p><img src="http://img.baidu.com/hi/youa/y_0040.gif" width="300" height="300" border="0" hspace="0" vspace="0" style="width:300px;height:300px;float:right;" /></p><p>还有没有什么其他的环绕方式呢？这里是居右环绕</p><p><br /></p><p>欢迎大家多多尝试，为UEditor提供更多高质量模板！</p><p><br /></p><p>占位</p><p><br /></p><p>占位</p><p><br /></p><p>占位</p><p><br /></p><p>占位</p><p><br /></p><p>占位</p><p><br /></p><p><br /></p>'
    },
    {
        "pre": "images/pre4.png",
        'title': lang.sciPapers,
        'preHtml': '<h2 style="border-bottom-color:#cccccc;border-bottom-width:2px;border-bottom-style:solid;padding:0px 4px 0px 0px;margin:0px 0px 10px;text-align:center;" class="ue_t">[键入文章标题]</h2><p><strong><span style="font-size:12px;">摘要</span></strong><span style="font-size:12px;" class="ue_t">：这里可以输入很长很长很长很长很长很长很长很长很差的摘要</span></p><p style="line-height:1.5em;"><strong>标题 1</strong></p><p style="text-indent:2em;"><span style="font-size:14px;" class="ue_t">这里可以输入很多内容，可以图文混排，可以有列表等。</span></p><p style="line-height:1.5em;"><strong>标题 2</strong></p><ol style="list-style-type:lower-alpha;"><li><p class="ue_t">列表 1</p></li><li><p class="ue_t">列表 2</p></li><ol style="list-style-type:lower-roman;"><li><p class="ue_t">多级列表 1</p></li><li><p class="ue_t">多级列表 2</p></li></ol><li><p class="ue_t">列表 3<br /></p></li></ol><p style="line-height:1.5em;"><strong>标题 3</strong></p><p style="text-indent:2em;"><span style="font-size:14px;" class="ue_t">来个文字图文混排的</span></p><p style="text-indent:2em;"><br /></p>',
        'html': '<h2 style="border-bottom-color:#cccccc;border-bottom-width:2px;border-bottom-style:solid;padding:0px 4px 0px 0px;margin:0px 0px 10px;text-align:center;" class="ue_t">[键入文章标题]</h2><p><strong><span style="font-size:12px;">摘要</span></strong><span style="font-size:12px;" class="ue_t">：这里可以输入很长很长很长很长很长很长很长很长很差的摘要</span></p><p style="line-height:1.5em;"><strong>标题 1</strong></p><p style="text-indent:2em;"><span style="font-size:14px;" class="ue_t">这里可以输入很多内容，可以图文混排，可以有列表等。</span></p><p style="line-height:1.5em;"><strong>标题 2</strong></p><p style="text-indent:2em;"><span style="font-size:14px;" class="ue_t">来个列表瞅瞅：</span></p><ol style="list-style-type:lower-alpha;"><li><p class="ue_t">列表 1</p></li><li><p class="ue_t">列表 2</p></li><ol style="list-style-type:lower-roman;"><li><p class="ue_t">多级列表 1</p></li><li><p class="ue_t">多级列表 2</p></li></ol><li><p class="ue_t">列表 3<br /></p></li></ol><p style="line-height:1.5em;"><strong>标题 3</strong></p><p style="text-indent:2em;"><span style="font-size:14px;" class="ue_t">来个文字图文混排的</span></p><p style="text-indent:2em;"><span style="font-size:14px;" class="ue_t">这里可以多行</span></p><p style="text-indent:2em;"><span style="font-size:14px;" class="ue_t">右边是图片</span></p><p style="text-indent:2em;"><span style="font-size:14px;" class="ue_t">绝对没有问题的，不信你也可以试试看</span></p><p><br /></p>'
    }
];