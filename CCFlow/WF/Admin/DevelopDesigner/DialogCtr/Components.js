
    var oNode = null, thePlugins = 'component';
    dialog.oncancel = function () {
        if (UE.plugins[thePlugins].editdom) {
            delete UE.plugins[thePlugins].editdom;
        }
    };

    dialog.onok = function () {
       
        var _html = Save();
        if (_html == "")
            return;
         editor.execCommand('insertHtml', _html);     
}



    function GetHtmlByMapAttrAndFrmComponent(mapData, frmComponent) {
        var _html = "";
       
        switch (frmComponent) {
            case 4: //地图控件
                _html = "<div style='text-align:left;padding-left:0px' id='Map_" + mapData.KeyOfEn + "' data-type='Map' data-key='" + mapData.MyPK + "' leipiplugins='component'>";
                _html += "<input type='button' name='select' value='选择'  style='background: #fff;color: #545454;font - size: 12px;padding: 4px 15px;margin: 5px 3px 5px 3px;border - radius: 3px;border: 1px solid #d2cdcd;'/>";
                _html += "<input type = text style='width:200px' maxlength=" + mapData.MaxLen + "  id='TB_" + mapData.KeyOfEn + "' name='TB_" + mapData.KeyOfEn + "' />";
                _html += "</div>";
                break;
            case 5://录音控件
                break;
            case 6: //字段附件
                _html +=mapData.Name+"<input type='text'  id='TB_"+mapData.KeyOfEn+"' name='TB_"+mapData.KeyOfEn+"' data-key='"+mapData.KeyOfEn+"' data-name='"+mapData.Name+"' data-type='AthShow'   leipiplugins='component' style='width:98%' placeholder='请上传附件'/>";
                break;
            case 7:
                break;
            case 8://签字版 
                _html = "<img src='../../../DataUser/Siganture/admin.jpg' onerror=\"this.src='../../../DataUser/Siganture/UnName.jpg'\"  style='border:0px;height:" + mapData.UIHeight + "px;' id='Img" + mapData.KeyOfEn + "' data-type='HandWriting' data-key='" + mapData.MyPK + "'  leipiplugins='component'/>";
                break;
            case 9://超链接
                break;
            case 10://文本
                break;
            case 11://图片
                _html = "<img src='../CCFormDesigner/Controls/basic/Img.png' style='width:" + mapData.UIWidth + "px;height:" + mapData.UIHeight + "px'  leipiplugins='component' data-key='" + mapData.MyPK + "' data-type='Img'/>"
                break;
            case 12://图片附件
                _html = "<img src='../CCFormDesigner/Controls/DataView/AthImg.png' style='width:" + mapData.W + "px;height:" + mapData.H + "px'  leipiplugins='component' data-key='" + mapData.MyPK + "' data-type='AthImg'/>"
                break;
            case 13://身份证

                break;
            case 14://签批组件
                _html = "<textarea id='TB_" + mapData.KeyOfEn + "' name='TB_" + mapData.KeyOfEn + "' data-key='" + mapData.KeyOfEn + "' data-name='" + mapData.Name + "' data-type='SignCheck'  leipiplugins='textarea' value='' orgrich='0' orgfontsize='12' orgwidth='600' orgheight='80' style='font-size: 12px; width: 528px; height: 59px; margin: 0px;' placeholder='签批组件'></textarea>";
                break;
            case 15://评论组件
                _html = "<textarea id='TB_FlowBBS' name='TB_FlowBBS' data-key='FlowBBS' data-name='评论组件' data-type='FlowBBS'  leipiplugins='component' value='' orgrich='0' orgfontsize='12' orgwidth='600' orgheight='80' style='font-size: 12px; width: 528px; height: 59px; margin: 0px;' placeholder='评论组件'></textarea>" ;
                break;
            case 16://系统定位
                break;
            case 17:// 发文字号
                _html = "<input type='text'  id='TB_DocWord' name='TB_DocWord' data-key='DocWord' data-name='公文字号' data-type='DocWord'   leipiplugins='component' style='width:98%' placeholder='发文字号'/>";
                break;
            case 170:// 收文字号
                _html = "<input type='text'  id='TB_DocWordReceive' name='TB_DocWordReceive' data-key='DocWordReceive' data-name='收文字号' data-type='DocWordReceive'   leipiplugins='component' style='width:98%' placeholder='收文字号'/>";
                break;
            case 18:
                _html = "<input type='button'  id='TB_" + mapData.KeyOfEn + "' name='TB_" + mapData.KeyOfEn + "' data-key='" + mapData.KeyOfEn + "' data-name='" + mapData.Name + "' data-type='Btn' value='" + mapData.Name +"'   leipiplugins='component' class='Btn'/>";
                break;
            case 50://流程进度图
                _html = "<img src='../FoolFormDesigner/Img/JobSchedule.png' style='border:0px;height:" + mapData.UIHeight + "px;width:100%;' id='Img" + mapData.KeyOfEn + "' leipiplugins='component' data-key='" + mapData.MyPK + "' data-type='JobSchedule'/>";
                break;
            case 60://大文本
                break;
            case 70: //
                _html = "<img src='../CCFormDesigner/Controls/DataView/AthMulti.png' style='width:67%;height:200px'  leipiplugins='ath' data-key='" + mapData + "' />"
                break;
            case 80://从表
                _html = "<img src='../CCFormDesigner/Controls/DataView/Dtl.png' style='width:67%;height:200px'  leipiplugins='dtl' data-key='" + mapData + "'/>"
                break;
            case 90://框架
                _html = "<img src='../CCFormDesigner/Controls/DataView/iFrame.png' style='width:67%;height:200px'  leipiplugins='component' data-key='" + mapData+ "' data-type='IFrame'/>"
                break;
            case 101://评分控件
                _html = "<span class='score-star' style='text-align:left;padding-left:0px'   data-key='" + mapData.MyPK + "' id='SC_" + mapData.KeyOfEn + "'>";
                _html += "<span class='simplestar' data-type='Score'  leipiplugins='component'  data-key='" + mapData.MyPK + "' id='Star_" + mapData.KeyOfEn + "'>";

                var num = mapData.Tag2;
                for (var i = 0; i < num; i++) {

                    _html += "<img src='../../Style/Img/star_1.png'  data-type='Score'  leipiplugins='component'  data-key='" + mapData.MyPK + "'/>";
                }
                _html += "&nbsp;&nbsp;<span class='score-tips' style='vertical-align: middle;color:#ff6600;font: 12px/1.5 tahoma,arial,\"Hiragino Sans GB\",宋体,sans-serif;'><strong>" + num + "  分</strong></span>";
                _html += "</span></span>";
                _Html += "<input type='text' id='TB_" + mapData.KeyOfEn + "' name='TB_" + mapData.KeyOfEn + "' style='display:none'/>"

                break;
            case 110://公文正文组件
                _html = "<input type='text'  id='TB_" + mapData.KeyOfEn + "' name='TB_" + mapData.KeyOfEn + "' data-key='" + mapData.KeyOfEn + "' data-name='" + mapData.Name + "' data-type='GovDocFile'   leipiplugins='component' style='width:98%' placeholder='公文正文组件'/>";
                break;
            case 120://父子流程
                _html = "<img src='../CCFormDesigner/Controls/DataView/SubFlowDtl.png' style='width:67%;height:200px'  leipiplugins='component'   data-type='SubFlow'/>"
                break;
        }
        return _html;
    }
 