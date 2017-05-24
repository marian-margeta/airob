using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Airob.Behaviours {
    interface IMoveable {
        bool CanMove { get; }

        void MoveTo(double x, double y);

        Rect Bounds { get; }
    }

    class MoveableBehaviour : Behavior<FrameworkElement> {
        private Vector delta;
        private Canvas canvas;
        private IMoveable thisObj;
        private bool hold;

        protected override void OnAttached() {
            thisObj = AssociatedObject.DataContext as IMoveable;

            if (thisObj == null)
                throw new NotSupportedException("Object with the MoveableBehaviour have to implement IMoveable");

            this.canvas = AssociatedObject.FindParentCanvas();
            
            AssociatedObject.MouseLeftButtonDown += MouseDown;
            AssociatedObject.MouseMove += MouseMove;
            AssociatedObject.MouseLeftButtonUp += MouseRelease;
        }

        private void MouseDown(object s, MouseButtonEventArgs e) {
            if (!thisObj.CanMove) return;

            AssociatedObject.CaptureMouse();
            hold = true;

            var currentPos = thisObj.Bounds;
            this.delta = new Point(currentPos.Left, currentPos.Top) - e.GetPosition(canvas);
        }

        private void MouseMove(object s, MouseEventArgs e) {
            if (!AssociatedObject.IsMouseCaptured || !hold) return;

            var p = e.GetPosition(canvas);
            var b = thisObj.Bounds;

            var x = (int)(p.X + delta.X);
            var y = (int)(p.Y + delta.Y);

            if (x <= 0 || b.Width + x >= canvas.ActualWidth) x = (int)b.Left;
            if (y <= 0 || b.Height + y >= canvas.ActualHeight) y = (int)b.Top;

            thisObj.MoveTo(x, y);
        }

        private void MouseRelease(object sender, MouseButtonEventArgs e) {
            AssociatedObject.ReleaseMouseCapture();
            hold = false;
        }
    }
}
