using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.Generic;
using WF.WS;
using BP;

namespace BP
{
    public delegate void ColseAnimationCompletedDelegate(object sender, EventArgs e);

    public delegate void MoveDelegate(FlowNode a, MouseEventArgs e);
    public delegate void FlowNodeChangeDelegate(FlowNode a);

    public delegate void DeleteDelegate(IElement a);

    public delegate void DirectionChangeDelegate(Direction r);

    public interface IContainer
    {
        #region Properties

        FlowChartType Flow_ChartType { get; set; }
        string FlowID { get; set; }
        string FK_Flow { get; }
        string WorkID { get; }

        bool CtrlKeyIsPress { get; }
        double ContainerWidth { get; set; }
        double ContainerHeight { get; set; }
        double ScrollViewerHorizontalOffset { get; set; }
        double ScrollViewerVerticalOffset { get; set; }

        string GetNewElementName(WorkFlowElementType type);

        int NextMaxIndex { get; }

        bool IsNeedSave { get; set; }
        bool IsSomeChildEditing { get; set; }
        bool IsContainerRefresh { get; set; }
        bool MouseIsInContainer { get; set; }
        bool IsMouseSelecting { get; }
        bool IsReturnTypeDir { get; set; }
       
        List<FlowNode> FlowNodeCollections { get; }
        List<Direction> DirectionCollections { get; }
        List<NodeLabel> LableCollections { get; }
        Direction CurrentTemporaryDirection { get; set; }
        List<IElement> SelectedWFElements { get; }
        PageEditType EditType { get; set; }
        //List<IElement> CopyElementCollectionInMemory { get; set; }
        //Stack<string> WorkFlowXmlPreStack { get; }
        //Stack<string> WorkFlowXmlNextStack { get; }

        WSDesignerSoapClient _Service { get; set; }
        Canvas GridLinesContainer { get; }
        #endregion

        #region Methods
        void AddUI(FlowNode a);
        void AddUI(Direction r);
        void AddUI(NodeLabel l);

        void RemoveUI(FlowNode a);
        void RemoveUI(NodeLabel l);
       
        void CreateLabel(Point p);
        void RemoveUI(Direction r);
        void SelectDirection(Direction r, bool isSelected);
        void SelectFlowNode(FlowNode a, bool isSelected);

        void ShowMessage(string message);
        void ShowSetting(FlowNode ac);
        void ShowSetting(Direction rc);
        void ShowContentMenu(FlowNode a, object sender, MouseButtonEventArgs e);
        void ShowContentMenu(NodeLabel l, object sender, MouseButtonEventArgs e);
        void ShowContentMenu(Direction r, object sender, MouseButtonEventArgs e);

        void AddSelectedControl(IElement uc);
        void RemoveSelectedControl(IElement uc);
        void SelectedWorkFlowElement(IElement uc, bool isSelect);
        void MoveControlCollectionByDisplacement(double x, double y, IElement uc);
        void ClearSelected(IElement uc);
        void CopySelectedToMemory(IElement currentControl);
        void UpdateSelectedToMemory(IElement currentControl);
        void UpdateSelectedNode(IElement currentControl);
        void DeleteSeleted();
        void PastMemoryToContainer();
        void ActionPrevious();
        void ActionNext();

        bool Contains(UIElement uiel);
        CheckResult CheckSave();
        void SaveChange(HistoryType action);
        void SetProper(string lang, string dotype, string fk_flow, string node1, string node2, string title);
        void SetGridLines(bool isShow); 
        #endregion
    }

    public interface IElement
    {
        WorkFlowElementType ElementType { get; }
        PageEditType EditType { get; set; }
        bool IsSelectd { get; set; }
        bool IsDeleted { get; }
        IContainer Container { get; set; }

        void Delete();
        void UpperZIndex();
        void Zoom(double zoomDeep);
        void Edit();
        void ResetPosition(double Xoffset, double Yoffset);
        CheckResult CheckSave();
        UserStation Station();
    }

    public class CheckResult
    {
        private bool isPass = true;

        public bool IsPass
        {
            get { return isPass; }
            set { isPass = value; }
        }

        private string message = "";

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }

    public class UserStation
    {
        private bool isPass = true;

        public bool IsPass
        {
            get { return isPass; }
            set { isPass = value; }
        }

        private bool isSel = true;

        public bool IsSel
        {
            get { return isSel; }
            set { isSel = value; }
        }

        private string message = "";

        public string Message
        {
            get { return message; }
            set { message = value; }
        }
    }

}