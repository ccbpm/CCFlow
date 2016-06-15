using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.Generic;
using System.Windows.Media.Imaging;

namespace BP.Controls
{
    public class ToolbarItem
    {

        public string No { get; set; }

        public string Name { get; set; }

        public bool IsEnable { get; set; }

        #region 单一实例
        public static readonly ToolbarItem Instance = new ToolbarItem();
        #endregion

        #region 公共方法
        public List<ToolbarItem> GetLists()
        {
            List<ToolbarItem> ToolList = new List<ToolbarItem>()
            {
                new ToolbarItem(){No="ToolBarLogin", Name = " 登录", IsEnable = true},
                new ToolbarItem(){No="ToolBarToolBox", Name="工具箱" },
                new ToolbarItem(){No="ToolBarSave", Name="保存"},
                new ToolbarItem(){No="ToolBarDesignReport", Name="设计报表"},
                new ToolbarItem(){No="ToolBarCheck", Name="检查"},
                new ToolbarItem(){No="ToolBarRun", Name="运行"},
                new ToolbarItem(){No="ToolBarEditFlow", Name="属性"},
                new ToolbarItem(){No="ToolBarEditFlowNew", Name="属性New"},
                new ToolbarItem(){No="ToolBarDeleteFlow", Name="删除"},
                new ToolbarItem(){No="ToolBarGenerateModel", Name="导出"},
                new ToolbarItem(){No="ToolBarShareModel", Name="模板库", IsEnable = true},
                //new ToolbarItem(){No="ToolBarFrmLab", Name="表单库"},
                //new ToolbarItem(){No="ToolBarReleaseToFTP", Name="共享流程"}, 
                new ToolbarItem(){No="ToolBarFlowUI", Name="节点样式"},
                //new ToolbarItem(){No="ToolBarSystem", Name="系统维护"},
                new ToolbarItem(){No="ToolBarHelp", Name="帮助", IsEnable = true},
                
            };
            return ToolList;
        }
        #endregion
    }
}

