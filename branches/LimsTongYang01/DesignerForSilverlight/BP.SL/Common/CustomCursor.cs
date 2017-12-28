using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;

namespace BP.SL
{
    public class CustomCursor
    {
        private FrameworkElement element;
        private static  System.Windows.Controls.Primitives.Popup cursorContainer;
        private static readonly DependencyProperty CustomCursorProperty =
            DependencyProperty.RegisterAttached("CustomCursor", typeof(CustomCursor), typeof(CustomCursor), null);

       
        public CustomCursor(FrameworkElement ele)
        {
            if (null != this.element && this.element != ele)
                (element.GetValue(CustomCursorProperty) as CustomCursor).Dispose();

            this.element = ele;
            element.SetValue(CustomCursorProperty, this);
            element.Cursor = Cursors.None;
            element.MouseMove += element_MouseMove;
            cursorContainer = new System.Windows.Controls.Primitives.Popup()
            {
                IsOpen = false,
                Child = new ContentControl()
                {
                    //ContentTemplate = template,
                    IsHitTestVisible = false,
                    RenderTransform = new TranslateTransform()
                },
                IsHitTestVisible = false
            };
        }

        private void element_MouseMove(object sender, MouseEventArgs e)
        {
            //cursorContainer.IsOpen = true;
            var p = e.GetPosition(null);
            var t = (cursorContainer.Child.RenderTransform as TranslateTransform);
            t.X = p.X;
            t.Y = p.Y;
        }

        public void Dispose()
        {
            element.MouseMove -= element_MouseMove;
            element.ClearValue(CustomCursorProperty);
            element.Cursor = Cursors.Arrow;
            cursorContainer.IsOpen = false;
            (cursorContainer.Child as ContentControl).ContentTemplate = null;
            cursorContainer.Child = null;
            cursorContainer = null;
        }

        public  DataTemplate GetCursorTemplate(DependencyObject obj)
        {
            return (DataTemplate)obj.GetValue(CursorTemplateProperty);
        }

       
       
        public void SetCursorDefault(Cursor cursor )
        {
            cursorContainer.IsOpen = false;
            this.element.Cursor = cursor;
        }


        public void SetCursorTemplate(DataTemplate value)
        {
            if (null != value)
            {
                element.Cursor = Cursors.None;
                this.element.SetValue(CursorTemplateProperty, value);
                cursorContainer.IsOpen = true;
            }

            //new Thread(() =>
            //{
            //    this.element.Dispatcher.BeginInvoke(() =>
            //    {
            //        this.element.SetValue(CursorTemplateProperty, value);
            //        cursorContainer.IsOpen = true;
            //    });
            //}).Start();
        }
       
        public readonly DependencyProperty CursorTemplateProperty =
            DependencyProperty.RegisterAttached("CursorTemplate", typeof(DataTemplate), typeof(CustomCursor), 
            new PropertyMetadata(
                (DependencyObject d, DependencyPropertyChangedEventArgs e)=>
        {
            if (!(d is FrameworkElement))
                throw new ArgumentOutOfRangeException("Property can only be attached to FrameworkElements");
            var element = (d as FrameworkElement);
            
            if (e.NewValue is DataTemplate)
            {
                element.Cursor = Cursors.None;
                (cursorContainer.Child as ContentControl).ContentTemplate = e.NewValue as DataTemplate;
            }
        }));

    }
}

