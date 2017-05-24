using System;
using GalaSoft.MvvmLight;
using Airob.Behaviours;
using System.Windows;
using Airob.Services;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Command;
using System.Windows.Input;
using System.Linq;
using Airob.Model;

namespace Airob.ViewModel {

    class BarrierViewModel : ViewModelBase, IMoveable {
        protected double x;
        protected double y;
        protected EditState state;

        public BarrierViewModel(double x, double y) {
            this.x = x;
            this.y = y;

            Messenger.Default.Register<(EditState, EditState)>(this, "ChangedState", ChangedState);
        }

        public BarrierViewModel(Point point): this(point.X, point.Y) {

        }

        private void ChangedState((EditState active, EditState old) state) {
            this.state = state.active;
            RaisePropertyChanged("CanMove");
        }

        public double X {
            get => x;
            set => Set(ref x, value);
        }


        public double Y {
            get => y;
            set => Set(ref y, value);
        }

        public bool CanMove => state == EditState.Motion;

        public Rect Bounds => new Rect(X, Y, 20, 20);
        
        
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
