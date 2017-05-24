using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airob.Services {
    public enum EditState {
        Motion, Rotation, AddPoint, RemovePoints, AddBarrier, Simulation
    }

    interface IAppState {
        EditState EditState { get; set; }
        bool VisibleOnlySpline { get; set; }
    }

    class AppState : ViewModelBase, IAppState {
        private EditState editState;
        public EditState EditState {
            get => editState; 
            set {
                Messenger.Default.Send((value, editState), "ChangedState");
                Set(ref editState, value);
            }
        }

        private bool visibleOnlySpline;
        public bool VisibleOnlySpline {
            get => visibleOnlySpline;
            set {
                Messenger.Default.Send((value, visibleOnlySpline), "ChangedVisibility");
                Set(ref visibleOnlySpline, value);
            }
        }
    }
}
