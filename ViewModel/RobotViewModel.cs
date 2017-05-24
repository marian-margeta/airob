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
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Airob.ViewModel {
    class RobotViewModel : ViewModelBase, IMoveable, IRotatable {
        protected Robot robot;
        protected bool visible;
        protected EditState state;

        public Robot Robot {
            get => robot;
            set {
                this.robot = value;
                RaiseAllPropertiesChanged();
            }
        }

        private void RaiseAllPropertiesChanged() {
            PropertyChangedHandler?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
        }

        public RobotViewModel(Collection<BarrierViewModel> barriers) {
            this.robot = new Robot(barriers);

            Messenger.Default.Register<(EditState, EditState)>(this, "ChangedState", ChangedState);
            Messenger.Default.Register<SplineViewModel>(this, "Completed", spline => {
                this.visible = spline.Completed;

                RaisePropertyChanged("IsVisible");
                if (spline.Completed) {
                    var firstPoint = spline.Points[0];

                    this.X = firstPoint.X;
                    this.Y = firstPoint.Y;
                    RaisePropertyChanged("X");
                    RaisePropertyChanged("Y");
                }
            });
        }
        
        private void ChangedState((EditState active, EditState old) state) {
            this.state = state.active;
            RaisePropertyChanged("CanMove");
            RaisePropertyChanged("IsVisible");
        }

        public double X {
            get => robot.X;
            set {
                robot.X = value;
                RaisePropertyChanged("X");
            }
        }


        public double Y {
            get => robot.Y;
            set {
                robot.Y = value;
                RaisePropertyChanged("Y");
            }
        }

        public bool CanMove => state == EditState.Motion;
        public bool IsVisible => visible;

        public Rect Bounds => new Rect(X, Y, 30, 30);

        public bool CanRotate => state == EditState.Rotation;

        public Point Center => new Point(X, Y);

        public double Angle {
            get => robot.Angle;
            set {
                robot.Angle = value;
                RaisePropertyChanged("Angle");
            }
        }

        internal void MoveMotors(double speedL, double speedR) {
            double oldAngle = Angle;

            robot.Move(speedL, speedR);

            RaisePropertyChanged("X");
            RaisePropertyChanged("Y");

            if (Angle != oldAngle) {
                RaisePropertyChanged("Angle");
            }
        }

        internal void Move(double x, double y) {
            X += x;
            Y += y;
        }

        public void MoveTo(double x, double y) {
            X = x;
            Y = y;
        }

        public void DoAction(RobotAction action, double speed) {
            robot.DoAction(action, speed);

            RaisePropertyChanged("X");
            RaisePropertyChanged("Y");
            RaisePropertyChanged("Angle");
        }
    }
}
