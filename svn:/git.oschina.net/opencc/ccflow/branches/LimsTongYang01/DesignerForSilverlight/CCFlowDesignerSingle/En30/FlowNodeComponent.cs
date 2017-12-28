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

namespace Ccflow.Web.Component.Workflow 
{
    /// <summary>
    /// 
    /// </summary>
    public class FlowNodeComponent
    {

        string _subFlow;
        public string SubFlow
        {
            get
            {
                return _subFlow;
            }
            set
            {
                _subFlow = value;
            }
        }

        string _repeatDirection = "Horizontal";
        public string RepeatDirection
        {
            get
            {
                return _repeatDirection;
            }
            set
            {
                _repeatDirection = value;
            }
        }

        string fk_Flow;
        public string FK_Flow
        {
            get
            {
                if (string.IsNullOrEmpty(fk_Flow))
                {
                    fk_Flow = Guid.NewGuid().ToString();
                }
                return fk_Flow;
            }
            set
            {
                fk_Flow = value;
            }

        }
        string nodeID;
        public string NodeID
        {
            get
            {
                return nodeID;
            }
            set
            {
                nodeID = value;
            }
        }

        string _nodeName;
        public string NodeName
        {
            get
            {
                return _nodeName;
            }
            set
            {
                _nodeName = value;
            }
        }
        string flowNodePosType;
        public string FlowNodeType
        {
            get
            {
                return flowNodePosType;
            }
            set
            {
                flowNodePosType = value;
            }
        }
    }
}
