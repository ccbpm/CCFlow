﻿//------------------------------------------------------------------------------
// <auto-generated>
//     此代码由工具生成。
//     运行时版本:4.0.30319.42000
//
//     对此文件的更改可能会导致不正确的行为，并且如果
//     重新生成代码，这些更改将会丢失。
// </auto-generated>
//------------------------------------------------------------------------------

namespace WindowsFormsApplication.ServiceReference {
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    [System.ServiceModel.ServiceContractAttribute(ConfigurationName="ServiceReference.WindowsFormsApplicationDemoSoap")]
    public interface WindowsFormsApplicationDemoSoap {
        
        [System.ServiceModel.OperationContractAttribute(Action="http://tempuri.org/HelloWorld", ReplyAction="*")]
        [System.ServiceModel.XmlSerializerFormatAttribute(SupportFaults=true)]
        string HelloWorld();
    }
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public interface WindowsFormsApplicationDemoSoapChannel : WindowsFormsApplication.ServiceReference.WindowsFormsApplicationDemoSoap, System.ServiceModel.IClientChannel {
    }
    
    [System.Diagnostics.DebuggerStepThroughAttribute()]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("System.ServiceModel", "4.0.0.0")]
    public partial class WindowsFormsApplicationDemoSoapClient : System.ServiceModel.ClientBase<WindowsFormsApplication.ServiceReference.WindowsFormsApplicationDemoSoap>, WindowsFormsApplication.ServiceReference.WindowsFormsApplicationDemoSoap {
        
        public WindowsFormsApplicationDemoSoapClient() {
        }
        
        public WindowsFormsApplicationDemoSoapClient(string endpointConfigurationName) : 
                base(endpointConfigurationName) {
        }
        
        public WindowsFormsApplicationDemoSoapClient(string endpointConfigurationName, string remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WindowsFormsApplicationDemoSoapClient(string endpointConfigurationName, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(endpointConfigurationName, remoteAddress) {
        }
        
        public WindowsFormsApplicationDemoSoapClient(System.ServiceModel.Channels.Binding binding, System.ServiceModel.EndpointAddress remoteAddress) : 
                base(binding, remoteAddress) {
        }
        
        public string HelloWorld() {
            return base.Channel.HelloWorld();
        }
    }
}