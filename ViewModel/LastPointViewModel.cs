using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airob.ViewModel {
    class LastPointViewModel : PointViewModel {
        public LastPointViewModel(double x, double y, SplineViewModel spline) : base(x, y, spline) {

        }

        public override bool CanMove => false;
    }
}
