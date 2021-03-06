﻿using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Airob.View.Utils;

namespace Airob.View
{
    /// <summary>
    /// Interaction logic for Spline.xaml
    /// </summary>
    public partial class Spline : UserControl
    {
        public Spline()
        {
            InitializeComponent();
        }

        public IEnumerable Points
        {
            get { return (IEnumerable)GetValue(PointsProperty); }
            set { SetValue(PointsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Points. This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PointsProperty =
            DependencyProperty.Register("Points", typeof(IEnumerable),
            typeof(Spline), new PropertyMetadata(null, PropertyChangedCallback));

        private static void PropertyChangedCallback(DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var landmarkControl = dependencyObject as Spline;
            if (landmarkControl == null)
                return;

            if (dependencyPropertyChangedEventArgs.NewValue is INotifyCollectionChanged)
            {
                (dependencyPropertyChangedEventArgs.NewValue as
                INotifyCollectionChanged).CollectionChanged += landmarkControl.OnPointCollectionChanged;
                landmarkControl.RegisterCollectionItemPropertyChanged
                (dependencyPropertyChangedEventArgs.NewValue as IEnumerable);
            }

            if (dependencyPropertyChangedEventArgs.OldValue is INotifyCollectionChanged)
            {
                (dependencyPropertyChangedEventArgs.OldValue as
                INotifyCollectionChanged).CollectionChanged -= landmarkControl.OnPointCollectionChanged;
                landmarkControl.UnRegisterCollectionItemPropertyChanged
                (dependencyPropertyChangedEventArgs.OldValue as IEnumerable);
            }

            if (dependencyPropertyChangedEventArgs.NewValue != null)
                landmarkControl.SetPathData();
        }
        

        public Brush PathColor
        {
            get { return (Brush)GetValue(PathColorProperty); }
            set { SetValue(PathColorProperty, value); }
        }

        // Using a DependencyProperty as the backing store for PathColor.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PathColorProperty =
            DependencyProperty.Register("PathColor", typeof(Brush), typeof(Spline),
                                        new PropertyMetadata(Brushes.Black));
        

        public static readonly DependencyProperty IsClosedCurveProperty =
            DependencyProperty.Register("IsClosedCurve", typeof(bool), typeof(Spline),
                                        new PropertyMetadata(default(bool), OnIsClosedCurveChanged));

        private static void OnIsClosedCurveChanged(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs dependencyPropertyChangedEventArgs)
        {
            var landmarkControl = dependencyObject as Spline;
            if (landmarkControl == null)
                return;
            landmarkControl.SetPathData();
        }

        public bool IsClosedCurve
        {
            get { return (bool)GetValue(IsClosedCurveProperty); }
            set { SetValue(IsClosedCurveProperty, value); }
        }

        void SetPathData()
        {
            if (Points == null) return;
            var points = new List<Point>();

            foreach (var point in Points)
            {
                var pointProperties = point.GetType().GetProperties();
                if (pointProperties.All(p => p.Name != "X") ||
                pointProperties.All(p => p.Name != "Y"))
                    continue;
                var x = (double)point.GetType().GetProperty("X").GetValue(point, new object[] { });
                var y = (double)point.GetType().GetProperty("Y").GetValue(point, new object[] { });
                points.Add(new Point(x, y));
            }

            if (points.Count <= 1)
                return;

            var myPathFigure = new PathFigure { StartPoint = points.FirstOrDefault() };


            var myPathSegmentCollection = new PathSegmentCollection();

            var beizerSegments = InterpolationUtils.InterpolatePointWithBeizerCurves(points, IsClosedCurve);

            if (beizerSegments == null || beizerSegments.Count < 1)
            {
                //Add a line segment <this is generic for more than one line>
                foreach (var point in points.GetRange(1, points.Count - 1))
                {

                    var myLineSegment = new LineSegment { Point = point };
                    myPathSegmentCollection.Add(myLineSegment);
                }
            }
            else
            {
                foreach (var beizerCurveSegment in beizerSegments)
                {
                    var segment = new BezierSegment
                    {
                        Point1 = beizerCurveSegment.FirstControlPoint,
                        Point2 = beizerCurveSegment.SecondControlPoint,
                        Point3 = beizerCurveSegment.EndPoint
                    };
                    myPathSegmentCollection.Add(segment);
                }
            }


            myPathFigure.Segments = myPathSegmentCollection;

            var myPathFigureCollection = new PathFigureCollection { myPathFigure };

            var myPathGeometry = new PathGeometry { Figures = myPathFigureCollection };

            path.Data = myPathGeometry;
        }

        private void RegisterCollectionItemPropertyChanged(IEnumerable collection)
        {
            if (collection == null)
                return;
            foreach (INotifyPropertyChanged point in collection)
                point.PropertyChanged += OnPointPropertyChanged;
        }

        private void UnRegisterCollectionItemPropertyChanged(IEnumerable collection)
        {
            if (collection == null)
                return;
            foreach (INotifyPropertyChanged point in collection)
                point.PropertyChanged -= OnPointPropertyChanged;
        }

        private void OnPointCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            RegisterCollectionItemPropertyChanged(e.NewItems);

            UnRegisterCollectionItemPropertyChanged(e.OldItems);

            SetPathData();
        }

        private void OnPointPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "X" || e.PropertyName == "Y")
                SetPathData();
        }
    }

}
