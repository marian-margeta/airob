using Airob.ViewModel;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using static System.Math;

namespace Airob.Model {

    public enum RobotAction {
        F, R, B, L, FL, BL, BR, FR, Stop
    }

    [ProtoContract]
    class Robot {
        public const double LENGTH = 60;
        public const double WALL_RADIUS = 20;
        public const double SONIC_RADIUS = 30;
        public const double LINE_SENSOR_DISTANCE = 10;
        public const double LINE_SENSOR_INNER_DISTANCE = 10;

        public const double SPEED = 1;
        public const double BSPEED = 3.7;
        public const double SLOWDOWN = 0.01;

        private readonly Dictionary<RobotAction, (double left, double right)> ACTIONS =
            new Dictionary<RobotAction, (double left, double right)>() {
                [RobotAction.F] = (SPEED, SPEED),
                [RobotAction.B] = (-BSPEED, -BSPEED),
                [RobotAction.L] = (-SPEED, SPEED),
                [RobotAction.R] = (SPEED, -SPEED),
                [RobotAction.FL] = (SPEED * SLOWDOWN, SPEED),
                [RobotAction.FR] = (SPEED, SPEED * SLOWDOWN),
                [RobotAction.BL] = (-BSPEED * SLOWDOWN, -BSPEED),
                [RobotAction.BR] = (-BSPEED, -BSPEED * SLOWDOWN),
                [RobotAction.Stop] = (0, 0),
            };
        
        public IList<BarrierViewModel> Barriers { get; set; }

        [ProtoMember(1)]
        public double X { get; set; }

        [ProtoMember(2)]
        public double Y { get; set; }
        [ProtoMember(3)]
        public double Angle { get; set; }

        private Robot() { }

        public Robot(IList<BarrierViewModel> barriers) {
            this.Barriers = barriers;
        }

        public bool DoAction(RobotAction action, double speed) {
            var (left, right) = ACTIONS[action];

            return Move(left * speed, right * speed);
        }

        public bool Move(double speedL, double speedR) {
            double dx = 0;
            double dy = 0;
            double da = 0;
            double ar = (PI * (Angle - 90)) / 180;

            if (speedL == speedR) {
                dx = speedL * Cos(ar);
                dy = speedR * Sin(ar);
            } else {
                double R = ((speedL + speedR) * LENGTH) / ((speedR - speedL) * 2);
                da = (speedR - speedL) / LENGTH;
                dx = R * Sin(da + ar) - R * Sin(ar);
                dy = -R * Cos(da + ar) + R * Cos(ar);
            }

            bool colistion = HasColistion(X + dx, Y + dy);

            // Update
            if (!colistion) {
                Angle += ((180 * da) / PI) % 360;
                X += dx;
                Y += dy;
            }

            return colistion;
        }

        public bool HasColistion(double x, double y) {
            Point point = new Point(x, y);

            return Barriers
                .Select(z => new Point(z.X, z.Y))
                .Any(z => z.Distance(point) <= WALL_RADIUS + LENGTH / 2);
        }

        public (byte, byte, byte) LineSensor(byte[][] map) {
            double ar = (PI * (Angle - 90)) / 180;

            var cx = X + LINE_SENSOR_DISTANCE * Cos(ar);
            var cy = Y + LINE_SENSOR_DISTANCE * Sin(ar);

            var center = new Point(cx, cy);

            var left = new Point(-cy, cx)
                .Normalize(LINE_SENSOR_INNER_DISTANCE)
                .Add(center);

            var right = new Point(cy, -cx)
                .Normalize(LINE_SENSOR_INNER_DISTANCE)
                .Add(center);
            
            return (DetectLine(map, left), DetectLine(map, center), DetectLine(map, right));
        }

        public byte DetectLine(byte[][] map, Point p) {
            int x = p.X.Round();
            int y = p.Y.Round();

            return (y < 0 || y >= map.Length || x < 0 || x >= map[0].Length)
                ? default(Byte)
                : map[y][x];
        }

        public int MarkVisitedArea(bool[,] map) {
            int sx = (int)Max(0, X - LENGTH / 2);
            int ex = (int)Min(map.GetLength(1), X + LENGTH / 2);

            int sy = (int)Max(0, Y - LENGTH / 2);
            int ey = (int)Min(map.GetLength(0), Y + LENGTH / 2);

            Point p = new Point(X, Y);

            int newlyRevealed = 0;
            for (var y = sy; y < ey; y++) {
                for (var x = sx; x < ex; x++) {
                    if (p.Distance(new Point(x, y)) <= LENGTH / 2) {
                        if (map[y, x]) {
                            map[y, x] = false;
                            newlyRevealed++;
                        }
                    }
                }
            }

            return newlyRevealed;
        }

        public double DistanceFromPath(byte[][] map) {
            var diag = Sqrt(1);
            var states = new Queue<(int x, int y)>();
            var visited = new bool[map.Length, map[0].Length];
            states.Enqueue((X.Round(), Y.Round()));

            while (states.Any()) {
                var (x, y) = states.Dequeue();

                if (x < 0 || x >= visited.GetLength(1) || y < 0 || y >= visited.GetLength(0) || visited[y, x]) {
                    continue;
                }

                if (map[y][x] > 0) {
                    return new Point(X, Y).Distance(new Point(x, y));
                }

                visited[y, x] = true;

                states.Enqueue((x,     y + 1));
                states.Enqueue((x + 1, y + 1));
                states.Enqueue((x + 1, y));
                states.Enqueue((x + 1, y - 1));
                states.Enqueue((x,     y - 1));
                states.Enqueue((x - 1, y - 1));
                states.Enqueue((x - 1, y));
                states.Enqueue((x - 1, y + 1));
            }

            return double.PositiveInfinity;
        }

        public double UltraSonicSensor() {
            var barriers = Barriers
                .Select(x => new Point(x.X, x.Y))
                .ToList();

            if (!barriers.Any()) return double.PositiveInfinity;

            return barriers
                .Select(z => {
                    var dx = z.X - X;
                    var dy = z.Y - Y;

                    var a2 = (Angle - 90) % 360;
                    var a1 = ((Atan2(dy, dx) * 180) / PI);

                    var d1 = Abs(a1 - a2) % 360;
                    var d2 = Abs(360 - d1);

                    return Min(d1, d2) <= SONIC_RADIUS
                        ? z.Distance(new Point(X, Y)) - WALL_RADIUS - LENGTH / 2
                        : double.PositiveInfinity;
                })
                .Min();
        }

    }
}
