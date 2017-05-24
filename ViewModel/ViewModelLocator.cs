/*
  In App.xaml:
  <Application.Resources>
      <vm:ViewModelLocator xmlns:vm="clr-namespace:Airob"
                           x:Key="Locator" />
  </Application.Resources>
  
  In the View:
  DataContext="{Binding Source={StaticResource Locator}, Path=ViewModelName}"

  You can also use Blend to do all this with the tool's support.
  See http://www.galasoft.ch/mvvm
*/

using Airob.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using Microsoft.Practices.ServiceLocation;

namespace Airob.ViewModel {
    /// <summary>
    /// This class contains static references to all the view models in the
    /// application and provides an entry point for the bindings.
    /// </summary>
    class ViewModelLocator {
        /// <summary>
        /// Initializes a new instance of the ViewModelLocator class.
        /// </summary>
        public ViewModelLocator() {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            SimpleIoc.Default.Register<IAppState, AppState>(); 

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<CanvasViewModel>();
            SimpleIoc.Default.Register<SplineViewModel>();
        }

        public MainViewModel Main => ServiceLocator.Current.GetInstance<MainViewModel>();
        public CanvasViewModel Canvas => ServiceLocator.Current.GetInstance<CanvasViewModel>();
        public SplineViewModel Spline => ServiceLocator.Current.GetInstance<SplineViewModel>();


        public static void Cleanup() {
            // TODO Clear the ViewModels
        }
    }
}