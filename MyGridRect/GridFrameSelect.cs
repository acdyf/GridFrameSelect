using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace GridRect
{
    /// <summary>
    /// 框选Grid控件
    /// </summary>
    public class GridFrameSelect : Grid
    {
        #region Constructor
        /// <summary>
        /// 框选Grid控件
        /// </summary>
        public GridFrameSelect()
        {
            Background = Brushes.Transparent;
            this.Loaded += GridRect_Loaded;
        }
        #endregion

        #region Fileds
        /// <summary>
        /// 框选矩形框
        /// </summary>
        private Rectangle rect;

        /// <summary>
        /// 矩形框父容器
        /// </summary>
        private Grid rectgrid;

        /// <summary>
        /// 矩形框起点
        /// </summary>
        private Point RectStartPoint = new Point();

        /// <summary>
        /// 矩形框终点
        /// </summary>
        private Point RectEndPoint = new Point();
        #endregion

        #region AdditionalProperties
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static bool GetCanFrameSelect(DependencyObject obj)
        {
            return (bool)obj.GetValue(CanFrameSelectProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetCanFrameSelect(DependencyObject obj, bool value)
        {
            obj.SetValue(CanFrameSelectProperty, value);
        }

        /// <summary>
        /// 是否可以框选
        /// </summary>
        public static readonly DependencyProperty CanFrameSelectProperty = DependencyProperty.RegisterAttached("CanFrameSelect", typeof(bool), typeof(GridFrameSelect), new PropertyMetadata());

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static FrameworkElement GetSelectType(DependencyObject obj)
        {
            return (FrameworkElement)obj.GetValue(SelectTypeProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetSelectType(DependencyObject obj, FrameworkElement value)
        {
            obj.SetValue(SelectTypeProperty, value);
        }

        /// <summary>
        /// 是否可以框选
        /// </summary>
        public static readonly DependencyProperty SelectTypeProperty = DependencyProperty.RegisterAttached("SelectType", typeof(FrameworkElement), typeof(GridFrameSelect), new PropertyMetadata());

        #endregion

        #region RoutedEvent

        /// <summary>
        /// 选中子集控件路由事件
        /// </summary>
        public static readonly RoutedEvent SelectChildChangeEvent = EventManager.RegisterRoutedEvent("SelectChildChange", RoutingStrategy.Bubble, typeof(RoutedEventArgs), typeof(GridFrameSelect));

        /// <summary>
        /// CLR事件包装
        /// </summary>
        public event RoutedEventHandler SelectChildChange
        {
            add { this.AddHandler(SelectChildChangeEvent, value); }
            remove { this.RemoveHandler(SelectChildChangeEvent, value); }
        }

        /// <summary>
        /// 触发路由事件
        /// </summary>
        /// <param name="frameworkElements"></param>
        protected void OnMyEvent(List<FrameworkElement> frameworkElements)
        {
            SelectedChangedEventArgs args = new SelectedChangedEventArgs(SelectChildChangeEvent, this);
            args.SelectedControls = frameworkElements;
            this.RaiseEvent(args);
        }
        #endregion

        #region Loaded
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void GridRect_Loaded(object sender, RoutedEventArgs e)
        {
            rectgrid = new Grid();
            rect = new Rectangle()
            {
                IsHitTestVisible = false,
                StrokeThickness = 1,
                Fill = Brushes.Transparent,
                Visibility = Visibility.Collapsed,
                Stroke = Brushes.Red,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top
            };

            Grid.SetRowSpan(rectgrid, 100);
            Grid.SetColumnSpan(rectgrid, 100);

            rectgrid.Children.Add(rect);
            this.Children.Add(rectgrid);

            Panel.SetZIndex(rectgrid, 999);
        }
        #endregion 

        #region FrameSelect
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!GetCanFrameSelect(this)) return;
            var StartPoint = e.GetPosition(this);
            RectStartPoint.X = Math.Truncate(StartPoint.X);
            RectStartPoint.Y = Math.Truncate(StartPoint.Y);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (!GetCanFrameSelect(this)) return;
            if (e.Key == Key.Escape && rect.Visibility == Visibility.Visible)
            {
                rect.Visibility = Visibility.Collapsed;
            }

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (!GetCanFrameSelect(this)) return;
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                //该事件在界面加载完后会马上出发，因为鼠标相对于grid的位置会更新，且Pressed,此时红框不该显示
                if (rect.Visibility != Visibility.Visible
                && RectStartPoint.X + RectStartPoint.Y != 0)
                {
                    rect.Visibility = Visibility.Visible;
                }

                Point p = e.GetPosition(this);
                if (p.X >= this.ActualWidth || p.Y >= this.ActualHeight || p.X <= 0 || p.Y <= 0)
                {
                    rect.Visibility = Visibility.Collapsed;
                    return;
                }

                double width = Math.Truncate(Math.Abs(p.X - RectStartPoint.X));
                double height = Math.Truncate(Math.Abs(p.Y - RectStartPoint.Y));

                RectEndPoint.X = p.X;
                RectEndPoint.Y = p.Y;

                double left, top;
                if (RectEndPoint.X < RectStartPoint.X)
                {
                    left = RectEndPoint.X;
                }
                else
                {
                    left = RectStartPoint.X;
                }

                if (RectEndPoint.Y < RectStartPoint.Y)
                {
                    top = RectEndPoint.Y;
                }
                else
                {
                    top = RectStartPoint.Y;
                }

                rect.Margin = new Thickness(left, top, 0, 0);
                rect.Height = height;
                rect.Width = width;

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnPreviewMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (!GetCanFrameSelect(this)) return;
            if (rect.Visibility == Visibility.Visible && (rect.Width != 0 || rect.Height != 0))
            {
                List<FrameworkElement> SelectedControlsTmp = new List<FrameworkElement>();
                foreach (FrameworkElement item in this.Children)
                {
                    if (item == rect || item == rectgrid)
                    {
                        continue;
                    }
                    var v = item.FindChildsBy<CheckBox>(typeof(CheckBox));
                    if (v != null && v.Count > 0)
                    {
                        foreach (var cb in v)
                        {
                            var generalTransform = cb.TransformToVisual(this);

                            var lefttop = generalTransform.Transform(new Point(0, 0));
                            var rightbottom = generalTransform.Transform(new Point(cb.ActualWidth, cb.ActualHeight));

                            var rectTmp = new Rect(lefttop, rightbottom);
                            var rectRed = new Rect(RectStartPoint, RectEndPoint);

                            if (rectTmp.IntersectsWith(rectRed))
                            {
                                SelectedControlsTmp.Add(cb);
                            }
                        }
                    }
                }
                OnMyEvent(SelectedControlsTmp);
                rect.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (!GetCanFrameSelect(this)) return;
            rect.Visibility = Visibility.Collapsed;
            RectStartPoint = new Point();
        }
        #endregion 
    }

    public static class Common
    {
        static public List<T> FindChildsBy<T>(this DependencyObject obj, Type type) where T : FrameworkElement
        {
            List<T> childList = new List<T>();

            for (int i = 0; i <= VisualTreeHelper.GetChildrenCount(obj) - 1; i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);

                if (child is T && (((T)child).GetType() == type))
                {
                    childList.Add((T)child);
                }
                childList.AddRange(child.FindChildsBy<T>(type));
            }
            return childList;
        }
    }

    public class SelectedChangedEventArgs : RoutedEventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="routedEvent"></param>
        /// <param name="source"></param>
        public SelectedChangedEventArgs(RoutedEvent routedEvent, object source)
            : base(routedEvent, source) { }

        /// <summary>
        /// 
        /// </summary>
        public List<FrameworkElement> SelectedControls { get; set; }
    }

    public class ContextMenuClickEventArgs : RoutedEventArgs
    { 
        
    }
}
