using Airob.Model;
using Airob.Services;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using Microsoft.Win32;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Text;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Xml.Serialization;

namespace Airob.ViewModel {
    class CanvasViewModel : ViewModelBase {
        private IAppState appState;
        private Canvas canvas;

        public SplineViewModel Spline { get; set; } = new SplineViewModel();
        public RobotViewModel Robot { get; set; }
        public ObservableCollection<BarrierViewModel> Barriers { get; set; } = new ObservableCollection<BarrierViewModel>();
        public RelayCommand<MouseEventArgs> CanvasClickCommand { get; private set; }
        public RelayCommand<MouseEventArgs> CanvasMouseMoveCommand { get; private set; }
        public RelayCommand<Canvas> CanvasLoadedCommand { get; private set; }

        public ITrainable Policy { get; set; }

        private DispatcherTimer simulationTimer = new DispatcherTimer();
        private byte[][] splineMap;

        public CanvasViewModel(IAppState appState) {
            this.appState = appState;
            this.Robot = new RobotViewModel(Barriers);

            this.CanvasLoadedCommand = new RelayCommand<Canvas>(c => canvas = c);

            this.CanvasClickCommand = new RelayCommand<MouseEventArgs>(execute: AddNew);

            this.CanvasMouseMoveCommand = new RelayCommand<MouseEventArgs>(
                execute: ShowNewPoint,
                canExecute: e => !Spline.Completed && this.appState.EditState == EditState.AddPoint);


            simulationTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
            simulationTimer.Tick += SimulationTick;

            Messenger.Default.Register<(EditState, EditState)>(this, "ChangedState", ChangedState);
            Messenger.Default.Register<string>(this, "DeleteSpline", DeleteSpline);
            Messenger.Default.Register<string>(this, "SaveCanvas", SaveCanvas);
            Messenger.Default.Register<string>(this, "OpenCanvas", OpenCanvas);
            Messenger.Default.Register<string>(this, "SaveTrain", SaveTrain);
            Messenger.Default.Register<string>(this, "OpenTrain", OpenTrain);
            Messenger.Default.Register<string>(this, "Train", Train);

            RuntimeTypeModel.Default
                .Add(typeof(Point), false)
                .Add("X", "Y");
        }

        private void SaveTrain(string s) {
            if (Policy == null) {
                MessageBox.Show("Before saving decision tree you must train it", "Warning",
                    MessageBoxButton.OK, MessageBoxImage.Warning);

                return;
            }

            var dialog = new SaveFileDialog() {
                FileName = "model.xml",
                Filter = "Model|*.xml"
            };

            var d = dialog.ShowDialog();
            if (d != null && d.Value) {
                XmlSerializer serializer = new XmlSerializer(typeof(DecisionTreeDto));
                using (TextWriter writer = new StreamWriter(dialog.FileName)) {
                    var policy = (DecisionTree)Policy;

                    var treeDto = new DecisionTreeDto() {
                        Distance = policy.Distance,
                        Speed = policy.Speed,
                        LineActions = policy.LineActions
                    };

                    serializer.Serialize(writer, treeDto);
                }
            }
        }

        private void OpenTrain(string s) {
            var dialog = new OpenFileDialog() {
                FileName = "model.xml",
                Filter = "Model|*.xml"
            };

            var d = dialog.ShowDialog();
            if (d != null && d.Value) {
                XmlSerializer deserializer = new XmlSerializer(typeof(DecisionTreeDto));
                using (TextReader reader = new StreamReader(dialog.FileName)) {
                    var treeDto = (DecisionTreeDto)deserializer.Deserialize(reader);

                    Policy = new DecisionTree() {
                        Distance = treeDto.Distance,
                        LineActions = treeDto.LineActions,
                        Speed = treeDto.Speed
                    };
                }
            }
        }

        private void SimulationTick(object sender, EventArgs e) {
            if (Policy == null) {
                Robot.MoveMotors(1.0, 0.3);
            } else {
                var robot = Robot.Robot;
                var (l1, l2, l3) = robot.LineSensor(splineMap);

                var state = new RobotState(
                    ultraSonic: robot.UltraSonicSensor(),
                    line: (l1 > 128, l2 > 128, l3 > 128)
                );

                var (action, speed) = Policy.Decide(state);
                Robot.DoAction(action, speed);
                Console.WriteLine((l1, l2, l3));
            }
        }

        private void Train(string s) {
            var dialog = new OpenFileDialog() {
                FileName = "scene.bin",
                Filter = "Scene|*.bin",
                Multiselect = true
            };

            var d = dialog.ShowDialog();
            if (d != null && d.Value) {
                var scenes = dialog.FileNames
                                .Select(x => Scene.Load(x))
                                .ToList();

                foreach (var scene in scenes) {
                    scene.Robot.Barriers = scene.Barriers
                        .Select(barrier => new BarrierViewModel(barrier))
                        .ToList();
                }

                this.Policy = DecisionTree.Train(scenes);
            }
        }

        private void OpenCanvas(string s) {
            var dialog = new OpenFileDialog() {
                FileName = "scene.bin",
                Filter = "Scene|*.bin"
            };

            var d = dialog.ShowDialog();
            if (d != null && d.Value) {
                var scene = Scene.Load(dialog.FileName);

                Barriers.Clear();
                foreach (var barrier in scene.Barriers) {
                    Barriers.Add(new BarrierViewModel(barrier));
                }

                Spline.Points.Clear();
                foreach (var point in scene.SplinePoints) {
                    Spline.Points.Add(new PointViewModel(point, Spline));
                }
                Spline.IsClosedCurve = true;
                Spline.Completed = true;

                Robot.Robot = scene.Robot;
                Robot.Robot.Barriers = Barriers;
            }
        }

        private void SaveCanvas(string s) {
            if (!Spline.Completed) {
                MessageBox.Show("Path must be completed before saving", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var dialog = new SaveFileDialog() {
                FileName = "scene.bin",
                Filter = "Scene|*.bin"
            };

            var d = dialog.ShowDialog();
            if (d != null && d.Value) {
                var scene = new Scene() {
                    Robot = Robot.Robot,
                    Barriers = Barriers.Select(x => new Point(x.X, x.Y)).ToList(),
                    SplineMap = GenerateSplineMap(),
                    SplinePoints = Spline.Points.Select(x => new Point(x.X, x.Y)).ToList()
                };

                scene.Save(dialog.FileName);
            }
        }

        public byte[][] GenerateSplineMap() {
            appState.VisibleOnlySpline = true;

            // Save current canvas transform
            Transform transform = canvas.LayoutTransform;
            // reset current transform (in case it is scaled or rotated)
            canvas.LayoutTransform = null;

            // Get the size of canvas
            Size size = new Size(canvas.ActualWidth, canvas.ActualHeight);
            // Measure and arrange the surface
            canvas.Measure(size);
            canvas.Arrange(new Rect(size));

            // Create a render bitmap and push the surface to it
            int w = (int)size.Width;
            int h = (int)size.Height;

            RenderTargetBitmap renderBitmap =
              new RenderTargetBitmap(
                pixelWidth: w,
                pixelHeight: h,
                dpiX: 96d,
                dpiY: 96d,
                pixelFormat: PixelFormats.Pbgra32);
            renderBitmap.Render(canvas);

            // Creating pixel matrix
            int stride = (int)renderBitmap.PixelWidth * (renderBitmap.Format.BitsPerPixel / 8);
            byte[] pixels = new byte[(int)renderBitmap.PixelHeight * stride];

            renderBitmap.CopyPixels(pixels, stride, 0);
            var bmp = pixels;

            // Breating byte matrix of the intensity of the line
            var splineMap = new byte[h][];
            for (int i = 0; i < h; i++) {
                splineMap[i] = new byte[w];
            }

            for (int i = 3; i < bmp.Length; i += 4) {
                int idx = i / 4;

                splineMap[idx / w][idx % w] = bmp[i];
            }

            // Restore previously saved layout
            canvas.LayoutTransform = transform;

            appState.VisibleOnlySpline = false;

            return splineMap;
        }

        
        public void DeleteSpline(string s) {
            Spline.Points.Clear();
            Spline.Completed = false;
            Spline.IsClosedCurve = false;

            Barriers.Clear();
        }

        public void ChangedState((EditState active, EditState old) state) {
            if (!Spline.Completed) {
                if (state.active == EditState.AddPoint && state.old != EditState.AddPoint) {
                    Spline.AddLastPoint(0, 0);
                } else if (state.active != EditState.AddPoint && state.old == EditState.AddPoint) {
                    Spline.RemoveLastPoint();
                }
            }

            if (state.active == EditState.Simulation && state.old != EditState.Simulation) {
                if (Policy != null) {
                    this.splineMap = GenerateSplineMap();
                    simulationTimer.Start();
                } else {
                    MessageBox.Show("No robot policy", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            } else if (state.active != EditState.Simulation && state.old == EditState.Simulation) {
                simulationTimer.Stop();
            }
            
        }

        public CompositeCollection AllData => new CompositeCollection()
        {
            new CollectionContainer() { Collection = new[] { Spline } },
            new CollectionContainer() { Collection = Spline.Points },
            new CollectionContainer() { Collection = new[] { Robot } },
            new CollectionContainer() { Collection = Barriers },
        };

        private void AddNew(MouseEventArgs e) {
            if (this.appState.EditState == EditState.AddPoint && !Spline.Completed) {
                AddNewPoint(e);
            } else if (this.appState.EditState == EditState.AddBarrier) {
                AddNewBarrier(e);
            }
        }

        private void AddNewBarrier(MouseEventArgs e) {
            if (e.Handled) return;

            if (canvas == null)
                throw new NotSupportedException("Canvas is not set");

            var mousePos = Mouse.GetPosition(canvas);
            var x = mousePos.X;
            var y = mousePos.Y;

            Barriers.Add(new BarrierViewModel(x, y));
        }

        private void AddNewPoint(MouseEventArgs e) {
            if (e.Handled) return;

            if (canvas == null)
                throw new NotSupportedException("Canvas is not set");

            var mousePos = Mouse.GetPosition(canvas);
            var x = mousePos.X;
            var y = mousePos.Y;

            if (Spline.IsClosedCurve) {
                Spline.Completed = true;
                Spline.ConvertFristPointToNormal();
            } else {
                Spline.AddNew(x, y);
            }
        }

        private void ShowNewPoint(MouseEventArgs e) {
            if (e.Handled || Spline.Points.Count <= 1) return;

            var mousePos = Mouse.GetPosition(canvas);
            var x = mousePos.X;
            var y = mousePos.Y;

            if (x < 0 || y < 0) return;

            var firstPoint = Spline.Points.FirstOrDefault(z => z is FirstPointViewModel);
            var dx = x - firstPoint.X;
            var dy = y - firstPoint.Y;
            var distFromFirstPoint = Math.Sqrt((dx * dx) + (dy * dy));

            if (distFromFirstPoint <= 8 && Spline.Points.Count(z => !(z is LastPointViewModel)) >= 3) {
                Spline.RemoveLastPoint();
                Spline.IsClosedCurve = true;
            } else {
                var lastPoint = Spline.Points.LastOrDefault(z => z is LastPointViewModel);

                if (lastPoint == null) {
                    Spline.AddLastPoint(x, y);
                } else {
                    Spline.IsClosedCurve = false;
                    lastPoint.MoveTo(x, y);
                }
            }

        }
    }
}
