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
using BP;

namespace Ccflow.Web.Component.Workflow 
{
    public class DirectionComponent
    {
        public string LineType { get; set; }

        string flowID;
        public string FlowID
        {
            get
            {
                if (string.IsNullOrEmpty(flowID))
                {
                    flowID = Guid.NewGuid().ToString();
                }
                return flowID;
            }
            set
            {
                flowID = value;
            }

        }
        string ruleID;

        public string DirectionID
        {
            get
            {
                return ruleID;
            }
            set
            {
                ruleID = value;
            }
        } 
         string ruleName;

        public string DirectionName
        {
            get
            {
                return ruleName;
            }
            set
            {
                ruleName = value;
            }
        }
         string ruleCondition;
        public string DirectionCondition
        {
            get
            {
                return ruleCondition;
            }
            set
            {
                ruleCondition = value;
            }
        }
    }
}
