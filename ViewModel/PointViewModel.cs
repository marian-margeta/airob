using System;
using GalaSoft.MvvmLight;
using Airob.Behaviours;
using System.Windows;
using Airob.Services;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;

namespace Airob.ViewModel {
    class PointViewModel : ViewModelBase, IMoveable {
        protected double x;
        protected double y;
        protected EditState state;
        protected SplineViewModel spline;
        

        public PointViewModel(double x, double y, SplineViewModel spline) {
            this.x = x;
            this.y = y;
            this.spline = spline;
            
            Messenger.Default.Register<(EditState, EditState)>(this, "ChangedState", ChangedState);
        }

        public PointViewModel(Point point, SplineViewModel spline) :this(point.X, point.Y, spline) {

        }

        private void ChangedState((EditState active, EditState old) state) {
            this.state = state.active;
            RaisePropertyChanged("CanMove");
        }

        public double X {
            get => x;
            set {
                Set(ref x, value);
                spline.RaisePropertyChanged("CanMove");
            }
        }


        public double Y {
            get => y;
            set {
                Set(ref y, value);
                spline.RaisePropertyChanged("CanMove");
            }
        }

        public bool IsFirstPoint => this is FirstPointViewModel;

        public virtual bool CanMove => state == EditState.Motion || state == EditState.AddPoint;

        public Rect Bounds => new Rect(X, Y, 0, 0);

        internal void Move(double x, double y) {
            X += x;
            Y += y;
        }

        public void MoveTo(double x, double y) {
            X = x;
            Y = y;
        }
    }
}
