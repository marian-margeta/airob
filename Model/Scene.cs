using ProtoBuf;
using ProtoBuf.Meta;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Airob.Model {
    [ProtoContract]
    class Scene {

        [ProtoMember(1)]
        public Robot Robot { get; set; }

        [ProtoMember(2)]
        public List<Point> Barriers { get; set; } = new List<Point>();

        [ProtoMember(3)]
        public List<Point> SplinePoints { get; set; }

        [ProtoMember(4)]
        public byte[][] SplineMap { get; set; }


        public void Save(String path) {
            using (var file = File.Create(path)) {
                Serializer.Serialize(file, this);
            }
        }

        internal Scene Clone() {
            return new Scene() {
                Barriers = Barriers.ToList(),
                Robot = new Robot(Robot.Barriers) {
                    X = Robot.X,
                    Y = Robot.Y
                },
                SplinePoints = SplinePoints.ToList(),
                SplineMap = SplineMap.Select(x => x.ToArray()).ToArray()
            };
        }

        public static Scene Load(String path) {
            using (var file = File.OpenRead(path)) {
                return Serializer.Deserialize<Scene>(file);
            }
        }
    }
}
