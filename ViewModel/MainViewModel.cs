using Airob.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows;

namespace Airob.ViewModel {
    class MainViewModel : ViewModelBase {
        public IAppState AppState { get; set; }

        public RelayCommand RemoveCommand { get; private set; }
        public RelayCommand SaveSceneCommand { get; }
        public RelayCommand OpenSceneCommand { get; }
        public RelayCommand SaveTrainCommand { get; }
        public RelayCommand OpenTrainCommand { get; }
        public RelayCommand TrainCommand { get; }
        public RelayCommand CloseAppCommand { get; }

        public MainViewModel(IAppState appState) {
            this.AppState = appState;

            this.RemoveCommand = new RelayCommand(() => {
                Messenger.Default.Send("", "DeleteSpline");
                AppState.EditState = EditState.Motion;
            });

            this.SaveSceneCommand = new RelayCommand(() => {
                Messenger.Default.Send("", "SaveCanvas");
            });

            this.OpenSceneCommand = new RelayCommand(() => {
                Messenger.Default.Send("", "OpenCanvas");
            });

            this.TrainCommand = new RelayCommand(() => {
                Messenger.Default.Send("", "Train");
            });

            this.SaveTrainCommand = new RelayCommand(() => {
                Messenger.Default.Send("", "SaveTrain");
            });

            this.OpenTrainCommand = new RelayCommand(() => {
                Messenger.Default.Send("", "OpenTrain");
            });

            this.CloseAppCommand = new RelayCommand(() => {
                Application.Current.Shutdown();
            });
        }



    }
}