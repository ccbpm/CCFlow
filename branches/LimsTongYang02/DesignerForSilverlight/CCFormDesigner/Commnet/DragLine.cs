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

namespace CCForm
{
    public enum LineDirection
    {
        None,
        Horizontal,
        Vertical
    }

    public class DragLine
    {
        MousePosition _Direction = MousePosition.None;
        bool StartDrag = false;
        bool StartMove = false;
        double mouseVerticalPosition;
        double mouseHorizontalPosition;
        LineDirection _LineDirection = LineDirection.None;
        Line line { get; set; }
        int MinSize = 20;

        public DragLine(Line _line)
        {
            this.line = _line;
        }
        public DragLine(Line _line, LineDirection _lineDirection)
        {
            this.line = _line;
            this._LineDirection = _lineDirection;
        }

        public void BindDrag()
        {
            line.MouseMove += new MouseEventHandler(line_MouseMove);
            line.MouseLeftButtonDown += new MouseButtonEventHandler(line_MouseLeftButtonDown);
            line.MouseLeftButtonUp += new MouseButtonEventHandler(line_MouseLeftButtonUp);
            line.KeyDown += new KeyEventHandler(line_KeyDown);
        }

        void line_KeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
            // 获取 textBox 对象的相对于 Canvas 的 x坐标 和 y坐标
            double x = (double)line.GetValue(Canvas.LeftProperty);
            double y = (double)line.GetValue(Canvas.TopProperty);
            switch (e.Key)
            {
                case Key.Up:
                    line.SetValue(Canvas.TopProperty, y - 1);
                    break;
                case Key.Down:
                    line.SetValue(Canvas.TopProperty, y + 1);
                    break;
                case Key.Left:
                    line.SetValue(Canvas.LeftProperty, x - 1);
                    break;
                case Key.Right:
                    line.SetValue(Canvas.LeftProperty, x + 1);
                    break;
                case Key.Delete:
                    Canvas c = line.Parent as Canvas;
                    c.Children.Remove(line);
                    break;
                case Key.C:
                    break;
                case Key.A:

                    break;
                case Key.V:
                    if (Keyboard.Modifiers == ModifierKeys.Control)
                    {
                        BPLine tb = new BPLine();
                        tb.Cursor = Cursors.Hand;
                        tb.SetValue(Canvas.LeftProperty, (double)line.GetValue(Canvas.LeftProperty) + 15);
                        tb.SetValue(Canvas.TopProperty, (double)line.GetValue(Canvas.TopProperty) + 15);
                        Canvas s1c = line.Parent as Canvas;
                        try
                        {
                            s1c.Children.Add(tb);
                        }
                        catch
                        {
                            s1c.Children.Remove(tb);
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        void line_MouseMove(object sender, MouseEventArgs e)
        {
            if (this._LineDirection == LineDirection.None)
            {
                GetDirection();
            }
            if (StartMove)
            {
                // Calculate the current position of the object.
                double deltaV = e.GetPosition(null).Y - mouseVerticalPosition;
                double deltaH = e.GetPosition(null).X - mouseHorizontalPosition;
                double newTop = deltaV + (double)line.GetValue(Canvas.TopProperty);
                double newLeft = deltaH + (double)line.GetValue(Canvas.LeftProperty);
                // Set new position of object.
                line.SetValue(Canvas.TopProperty, newTop);
                line.SetValue(Canvas.LeftProperty, newLeft);
                // Update position global variables.
                mouseVerticalPosition = e.GetPosition(null).Y;
                mouseHorizontalPosition = e.GetPosition(null).X;

            }
            else if (StartDrag)
            {
                Point p = e.GetPosition(line);
                if (_Direction == MousePosition.SizeLeft)
                {
                    if (p.X <= line.X2 - MinSize)
                    {
                        line.X1 = p.X;
                    }
                }
                else if (_Direction == MousePosition.SizeRight)
                {
                    if (p.X >= line.X1 + MinSize)
                    {
                        line.X2 = p.X;
                    }
                }
                else if (_Direction == MousePosition.SizeBottom)
                {
                    if (p.Y <= line.Y2 - MinSize)
                    {
                        line.Y1 = p.Y;
                    }
                }
                else if (_Direction == MousePosition.SizeTop)
                {
                    if (p.Y >= line.Y1 + MinSize)
                    {
                        line.Y2 = p.Y;
                    }
                }
            }
            else
            {
                double x = Convert.ToDouble(line.GetValue(Canvas.LeftProperty)) + e.GetPosition(line).X;
                double y = Convert.ToDouble(line.GetValue(Canvas.TopProperty)) + e.GetPosition(line).Y;
                //Line 相对于父容器的实际坐标
                double x1 = Convert.ToDouble(line.GetValue(Canvas.LeftProperty)) + line.X1;
                double y1 = Convert.ToDouble(line.GetValue(Canvas.TopProperty)) + line.Y1;
                double x2 = Convert.ToDouble(line.GetValue(Canvas.LeftProperty)) + line.X2;
                double y2 = Convert.ToDouble(line.GetValue(Canvas.TopProperty)) + line.Y2;

                if (_LineDirection == LineDirection.Horizontal)
                {
                    if (x - x1 <= 5)
                    {
                        line.Cursor = Cursors.SizeWE;
                        _Direction = MousePosition.SizeLeft;
                    }
                    else if (x2 - x <= 5)
                    {
                        line.Cursor = Cursors.SizeWE;
                        _Direction = MousePosition.SizeRight;
                    }
                    else
                    {
                        line.Cursor = Cursors.Hand;
                        _Direction = MousePosition.None;
                    }
                }
                else if (_LineDirection == LineDirection.Vertical)
                {
                    if (y - y1 <= 5)
                    {
                        line.Cursor = Cursors.SizeNS;
                        _Direction = MousePosition.SizeBottom;
                    }
                    else if (y2 - y <= 5)
                    {
                        line.Cursor = Cursors.SizeNS;
                        _Direction = MousePosition.SizeTop;
                    }
                    else
                    {
                        line.Cursor = Cursors.Hand;
                        _Direction = MousePosition.None;
                    }
                }
            }
        }

        void line_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            StartDrag = false;
            line.ReleaseMouseCapture();
            _Direction = MousePosition.None;
            StartMove = false;
            line.ReleaseMouseCapture();
            mouseVerticalPosition = -1;
            mouseHorizontalPosition = -1;
        }

        void line_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_Direction != MousePosition.None)
            {
                StartDrag = true;
                line.CaptureMouse();
            }
            else
            {
                mouseVerticalPosition = e.GetPosition(null).Y;
                mouseHorizontalPosition = e.GetPosition(null).X;
                StartMove = true;
                line.CaptureMouse();
            }
        }

        void Attatch(MouseEventArgs e, Line line, UIElementCollection UICollection)
        {
            double x1 = Convert.ToDouble(line.GetValue(Canvas.LeftProperty)) + line.X1;
            double y1 = Convert.ToDouble(line.GetValue(Canvas.TopProperty)) + line.Y1;

            foreach (UIElement ui in UICollection)
            {
                double X1 = (double)ui.GetValue(Canvas.LeftProperty);
                double X2 = X1 + (double)ui.GetValue(FrameworkElement.HeightProperty);
                double Y1 = (double)ui.GetValue(Canvas.TopProperty);
                double Y2 = Y1 + (double)ui.GetValue(FrameworkElement.WidthProperty);

                if (x1 - X1 <= 50)
                {
                    line.X1 = X1;
                }
            }
        }
        void GetDirection()
        {
            if (Math.Abs(line.X1 - line.X2) > Math.Abs(line.Y1 - line.Y2))
            {
                this._LineDirection = LineDirection.Horizontal;
            }
            else
            {
                this._LineDirection = LineDirection.Vertical;
            }
        }
    }
}
