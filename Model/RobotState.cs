using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Airob.Model {
    struct RobotState {
        public double UltraSonic { get; private set; }
        public (bool, bool, bool) Line { get; private set; }

        public RobotState(double ultraSonic, (bool, bool, bool) line) {
            UltraSonic = ultraSonic;
            Line = line;
        }


    }
}
