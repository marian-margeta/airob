using Airob.Behaviours;
using Airob.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Airob.ViewModel {
    class SplineViewModel : ViewModelBase, IMoveable {
        protected EditState state;
        private bool isClosedCurve = false;

        private bool completed;
        public bool Completed {
            get => completed;
            set {
                completed = value;
                Messenger.Default.Send(this, "Completed");
            }
        }

        public SplineViewModel() {
            Messenger.Default.Register<(EditState, EditState)>(this, "ChangedState", ChangedState);
        }

        public ObservableCollection<PointViewModel> Points { get; set; } =
            new ObservableCollection<PointViewModel>() {
            };

        private void ChangedState((EditState active, EditState old) state) {
            this.state = state.active;
            RaisePropertyChanged("CanMove");
        }    

        public bool IsClosedCurve {
            get { return isClosedCurve; }
            set {
                if (isClosedCurve != value) {
                    isClosedCurve = value;
                    RaisePropertyChanged("IsClosedCurve");
                }
            }
        }

        public bool CanMove => state == EditState.Motion;

        public Rect Bounds { 
            get {
                var x = Points.Min(z => z.X);
                var y = Points.Min(z => z.Y);

                var mx = Points.Max(z => z.X);
                var my = Points.Max(z => z.Y);

                return new Rect(x, y, mx - x, my - y);
            }
        }

        public void MoveTo(double x, double y) {
            var b = Bounds;

            foreach (var p in Points) {
                p.Move(x - b.Left, y - b.Top);
            }
        }

        internal void RemoveLastPoint() {
            for (int i = Points.Count - 1; i >= 0; i--) {
                if (Points[i] is LastPointViewModel)
                    Points.RemoveAt(i);
            }
        }

        internal void AddNew(double x, double y) {
            var newPoint = (Points.Count == 1)
                ? new FirstPointViewModel(x, y, this)
                : new PointViewModel(x, y, this);

            Points.Insert(Points.Count - 1, newPoint);
        }

        internal void AddLastPoint(double x, double y) {
            Points.Add(new LastPointViewModel(x, y, this));
        }

        internal void ConvertFristPointToNormal() {
            for (int i = Points.Count - 1; i >= 0; i--) {
                if (Points[i] is FirstPointViewModel) {
                    var p = Points[i];

                    Points.RemoveAt(i);
                    Points.Insert(i, new PointViewModel(p.X, p.Y, this));
                }
            }
        }
    }
}
