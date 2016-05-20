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
    public class Configure
    {
        static System.Globalization.CultureInfo currentCulture;
        public static System.Globalization.CultureInfo CurrentCulture
        {
            get
            {
                if (currentCulture == null)
                {
                    try
                    {
                        System.IO.IsolatedStorage.IsolatedStorageSettings appSetting = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings;
                        if (appSetting.Contains("language"))
                        {
                            currentCulture = new System.Globalization.CultureInfo((string)appSetting["language"]);
                        }
                    }
                    catch 
                    {
                    }
                } 
                if (currentCulture == null)
                {
                    currentCulture =new System.Globalization.CultureInfo("en-us");
                }
                return currentCulture;
            }
            set
            {
                currentCulture = value;
                System.Threading.Thread.CurrentThread.CurrentCulture = currentCulture;
                System.Threading.Thread.CurrentThread.CurrentUICulture = currentCulture;

                try
                {
                    System.IO.IsolatedStorage.IsolatedStorageSettings appSetting = System.IO.IsolatedStorage.IsolatedStorageSettings.ApplicationSettings;
                    if (appSetting.Contains("language"))
                    {
                        appSetting["language"] = currentCulture.Name;
                    }
                    else
                    {
                        appSetting.Add("language", currentCulture.Name);
                    }
                }
                catch 
                {
                }
            }
        }
    }
    public class Utility
    {
        public static void SetOnMouseEnter(UIElement element)
        {
        }
    }
}