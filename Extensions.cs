using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Airob {
    static class Extensions {
        public static object PropertyChangedHandler { get; private set; }

        public static Canvas FindParentCanvas(this DependencyObject obj) {
            // Find parent canvas
            var parent = obj;
            do {
                parent = VisualTreeHelper.GetParent(parent);
            }
            while (parent != null && !(parent is Canvas));

            return parent as Canvas;
        }

        public static double Distance(this Point p1, Point p2) {
            return Math.Sqrt((p1.X - p2.X) * (p1.X - p2.X) + (p1.Y - p2.Y) * (p1.Y - p2.Y));
        }

        public static double Distance(this Point p) {
            return Math.Sqrt(p.X * p.X + p.Y * p.Y );
        }

        public static Point Normalize(this Point p, double c) {
            var d = p.Distance();

            return new Point((p.X / d) * c, (p.Y / d) * c);
        }
        public static Point Add(this Point p, Point a) {
            return new Point(p.X + a.X, p.Y + a.Y);
        }

        public static int Round(this double n) {
            return (int)Math.Round(n, 0, MidpointRounding.AwayFromZero);
        }

        public static T[] RandomPermutation<T>(this IEnumerable<T> e) {
            Random random = new Random();
            var array = e.ToArray();

            int n = array.Count();
            while (n > 1) {
                n--;
                int i = random.Next(n + 1);
                var temp = array[i];
                array[i] = array[n];
                array[n] = temp;
            }

            return array;
        }

        public static IEnumerable<T> RandomPermutation<T>(this IEnumerable<T> e, Func<T, double> comp) {
            Random random = new Random();
            var arr = e.ToList();

            while (arr.Any()) {
                var sum = e.Select(x => comp(x)).Sum();
                var num = random.NextDouble() * sum;

                var q = 0d;
                foreach (var item in arr) {
                    q += comp(item);

                    if (num < q) {
                        arr.Remove(item);
                        yield return item;
                        break;
                    }
                }
            }

        }
    }
}
