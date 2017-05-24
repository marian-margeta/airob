using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interactivity;
using static System.Math;

namespace Airob.Behaviours {

    interface IRotatable {
        bool CanRotate { get; }
        double Angle { get; set; }
        Point Center { get; }
    }

    class RotationBehaviour : Behavior<FrameworkElement> {
        private Point center;
        private Point delta;
        private double alpha;
        private Canvas canvas;
        private IRotatable thisObj;
        private bool hold;

        protected override void OnAttached() {
            thisObj = AssociatedObject.DataContext as IRotatable;

            if (thisObj == null)
                throw new NotSupportedException("Object with MoveableBehaviour have to implement IMoveable");

            this.canvas = AssociatedObject.FindParentCanvas();

            if (canvas == null)
                throw new NotSupportedException("Parent object of object with MoveableBehaviour have to be Canvas");

            AssociatedObject.MouseLeftButtonDown += MouseDown;
            AssociatedObject.MouseMove += MouseMove;
            AssociatedObject.MouseLeftButtonUp += MouseRelease;
        }

        private void MouseDown(object s, MouseButtonEventArgs e) {
            if (!thisObj.CanRotate) return;

            delta = e.GetPosition(canvas);
            center = thisObj.Center;
            alpha = thisObj.Angle;

            AssociatedObject.CaptureMouse();
            hold = true;
        }

        private void MouseMove(object s, MouseEventArgs e) {
            if (!AssociatedObject.IsMouseCaptured || !hold) return;

            var p = e.GetPosition(canvas);

            var a = center.Distance(delta);
            var b = center.Distance(p);
            var c = p.Distance(delta);

            var k = (delta.Y - center.Y) / (delta.X - center.X);
            var q = center.Y - k * center.X;

            Func<double, double> line = x => k * x + q;

            var angle = Acos((a * a + b * b - c * c) / (2 * a * b));

            if (line(p.X) > p.Y && center.X < delta.X ||
                line(p.X) < p.Y && center.X > delta.X)
                angle = -angle;
            
            thisObj.Angle = alpha + (180 * angle) / PI;
        }

        private void MouseRelease(object sender, MouseButtonEventArgs e) {
            AssociatedObject.ReleaseMouseCapture();
            hold = false;
        }
    }
}
