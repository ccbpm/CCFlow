using System.Windows;
using System.Windows.Browser;
using System.Windows.Controls;
using System.Windows.Input;
using Liquid;
using WF.Frm;
using MenuItem = Liquid.MenuItem;

namespace BP
{
    public partial class FlowNodeMenu : UserControl
    {
        public FlowNodeMenu()
        {
            InitializeComponent();
        }
       
        private bool isMultiControlSelect = false;
        public IContainer Container;
        public FlowNode RelatedFlowNode;
        public Point CenterPoint
        {
            get { return new Point((double)this.GetValue(Canvas.LeftProperty), (double)this.GetValue(Canvas.TopProperty)); }
            set
            {
                // 调整x,y 值 ，以防止菜单被遮盖住.
                var x = value.X;
                var y = value.Y;
                var menuHeight = 250;
                var menuWidth = 170;
                var hostWidth = Application.Current.Host.Content.ActualWidth - 250;
                var hostHeight = Application.Current.Host.Content.ActualHeight - 135;
                if (x + menuWidth > hostWidth)
                {
                    x = x - (x + menuWidth - hostWidth);
                }
                if (y + menuHeight > hostHeight)
                {
                    y = y - (y + menuHeight - hostHeight);
                }
                this.SetValue(Canvas.TopProperty, y);
                this.SetValue(Canvas.LeftProperty, x);
            }
        }

        public void ApplyCulture()
        {
        }

        public void ShowMenu()
        {
            #region 批量选择 ？
            isMultiControlSelect = false;
            if (Container.SelectedWFElements != null
                && Container.SelectedWFElements.Count > 0
                )
            {
                if (!Container.SelectedWFElements.Contains(RelatedFlowNode))
                {
                    Container.ClearSelected(null);
                    isMultiControlSelect = false;
                }
                else
                {
                    isMultiControlSelect = true;
                }
            }
            else
            {
                isMultiControlSelect = false;
            }
            #endregion

            #region 开始节点不允许删除
            if (NodePosType.Start == RelatedFlowNode.HisPosType)
                MuContentMenu.SetEnabledStatus("menuDeleteNode", false);
            else
                MuContentMenu.SetEnabledStatus("menuDeleteNode", true);
            #endregion
            #region 非图标流程不允许编辑节点图标
            if (RelatedFlowNode.Container.Flow_ChartType == FlowChartType.UserIcon)
                MuContentMenu.SetEnabledStatus("menuNodeIcon", true);
            else
                MuContentMenu.SetEnabledStatus("menuNodeIcon", false);
            #endregion

            // 还原节点类型
            relatedNodeType(RelatedFlowNode.NodeType);

            this.Visibility = Visibility.Visible;
            MuContentMenu.Show();
        }


        /// 显示快捷方式时还原节点类型
        private void relatedNodeType(FlowNodeType type)
        {
            var menuType = (MuContentMenu.Items[1] as MenuItem).Content as Menu;
            if (null == menuType)
                return;

            MenuItem mItem = null;
            foreach (var item in menuType.Items)
            {
                mItem = item as MenuItem;
                if (null != mItem)
                {
                    mItem.FontWeight = FontWeights.Normal;
                    mItem.CheckBoxVisibility = System.Windows.Visibility.Collapsed;
                    mItem.IsChecked = false;
                }
            }

            switch (type)
            {
                case FlowNodeType.Ordinary:
                    mItem = (menuType.Items[0] as MenuItem);
                    break;
                case FlowNodeType.FL:
                    mItem = (menuType.Items[1] as MenuItem);
                    break;
                case FlowNodeType.HL:
                    mItem = (menuType.Items[2] as MenuItem);
                    break;
                case FlowNodeType.FHL:
                    mItem = (menuType.Items[3] as MenuItem);
                    break;
                case FlowNodeType.SubThread:
                    mItem = (menuType.Items[4] as MenuItem);
                    break;
            }

            mItem.FontWeight = FontWeights.ExtraBold;
            mItem.CheckBoxVisibility = System.Windows.Visibility.Visible;
            mItem.IsChecked = true;
        }

        private void Menu_ItemSelected(object sender, MenuEventArgs e)
        {
            MuContentMenu.Hide();
            if (e.Tag == null)
                return;
            FlowNodeType type = FlowNodeType.UnKnown;
            
            switch (e.Tag.ToString())
            {
                #region NodeIcon and NodeType
                case "menuNodeIcon":
                    NodeIconUpdate.Instance.SetSelected(this.RelatedFlowNode);
                    break;
                case "menuNodeTypePT":
                    type = FlowNodeType.Ordinary;

                    break;
                case "menuNodeTypeFL":
                    type = FlowNodeType.FL;

                    break;
                case "menuNodeTypeHL":
                    type = FlowNodeType.HL;

                    break;
                case "menuNodeTypeFHL":
                    type = FlowNodeType.FHL;

                    break;
                case "menuNodeTypeZLC":
                    type = FlowNodeType.SubThread;
                    break;
                #endregion

                #region
                case "menuExp": // 导出/共享:流程模板
                    BP.Frm.FrmExp exp = new BP.Frm.FrmExp();
                    exp.Show();
                    break;
                case "menuImp": //  导入/查找:流程模板
                    BP.Frm.FrmImp imp = new BP.Frm.FrmImp();
                    imp.Show();
                    break;
                case "menuModifyName":
                     this.Visibility = Visibility.Collapsed;
                     Container.ShowSetting(RelatedFlowNode);
                    break;
                case "menuDeleteNode":
                    delete();
                    break;
                case "menuDesignNodeFixModel":
                    Glo.OpenWinByDoType("CH", "MapDefFixModel", Container.FlowID, RelatedFlowNode.NodeID, null);
                    break;
                case "menuDesignNodeFreeModel":
                    Glo.OpenWinByDoType("CH", "MapDefFreeModel", Container.FlowID, RelatedFlowNode.NodeID, RelatedFlowNode.NodeName);
                    break;
                case "menuDesignFlowFrm": // 表单库
                    Glo.OpenWinByDoType("CH", "FrmLib", Container.FlowID, RelatedFlowNode.NodeID, null);
                    break;
                case "menuDesignBindFlowFrm": //独立表单
                    Glo.OpenWinByDoType("CH", "FlowFrms", Container.FlowID, RelatedFlowNode.NodeID, null);
                    break;
                case "menuJobStation": // 节点工作岗位。
                    Glo.OpenWinByDoType("CH", "StaDef", Container.FlowID, RelatedFlowNode.NodeID, null);
                    break;
                case "menuNodeProperty":
                    Glo.OpenWinByDoType("CH", "NodeP", Container.FlowID, RelatedFlowNode.NodeID, null);
                    break;
                case "menuNodePropertyNew":
                    Glo.OpenWinByDoType("CH", "NodePNew", Container.FlowID, RelatedFlowNode.NodeID, null);
                    break;
                case "menuFlowProperty":
                    Glo.OpenWinByDoType("CH", "FlowP", Container.FlowID, RelatedFlowNode.NodeID, null);
                    break;
                #endregion
            }

            if (type !=  FlowNodeType.UnKnown && type != RelatedFlowNode.NodeType)
            {
                RelatedFlowNode.IsIconNeedUpdate = true;
                RelatedFlowNode.NodeType = type;
            }
        }

        private void delete()
        {
            if (RelatedFlowNode != null)
            {
                if (HtmlPage.Window.Confirm("您确认要删除节点吗？"))
                {
                    this.Visibility = Visibility.Collapsed;
                    IElement iel;
                    foreach (System.Windows.Controls.Control c in Container.SelectedWFElements)
                    {
                        iel = c as IElement;
                        if (iel != null)
                        {
                            iel.Delete();
                        }
                    }
                    RelatedFlowNode.Delete();
                    Container.SaveChange(HistoryType.New);
                    Container.IsNeedSave = true;
                }
            }
        }
  
        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            this.Visibility = Visibility.Collapsed;
        }
        private void UserControl_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
        }
    }
}
