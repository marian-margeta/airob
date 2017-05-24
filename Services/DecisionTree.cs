using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Win32;
using System.IO;
using Airob.Model;
using System;
using Troschuetz.Random;
using System.Collections.Concurrent;
using System.Xml.Serialization;

namespace Airob.Services {
    interface ITrainable {
        (RobotAction, double speed) Decide(RobotState state);
        double Loss(Scene scene);
    }

    [XmlRoot("decision_tree")]
    public class DecisionTreeDto {
        [XmlElement("line_actions")]
        public List<RobotAction> LineActions { get; set; }
        [XmlElement("distance")] 
        public double Distance { get; set; }
        [XmlElement("speed")]
        public double Speed { get; set; }
    }

    class DecisionTree : ITrainable {
        private static int INIT_POPULATION = 150;
        private static int ELITE_POPULATION = 20;
        private static int REMAINING_POPULATION = 80;
        private static int ITERATIONS = 20;
        
        public List<RobotAction> LineActions { get; set; }
        public double Distance { get; set; }
        public double Speed { get; set; }

        private static TRandom random = new TRandom();

        private double loss = -1;

        public static ITrainable Train(IEnumerable<Scene> scenes) {
            var population = Enumerable.Range(0, INIT_POPULATION)
                .Select(x => GenerateRandom())
                .ToList();

            DecisionTree best = null;

            for (int l = 0; l < ITERATIONS; l++) {
                var dict = new ConcurrentDictionary<DecisionTree, double>();

                Parallel.ForEach(population, instance => {
                    if (instance.loss != -1) {
                        dict.AddOrUpdate(instance, instance.loss, (s, v) => instance.loss);
                        return;
                    }

                    foreach (var scene in scenes) {
                        var score = instance.Loss(scene);
                        dict.AddOrUpdate(instance, score, (s, v) => v + score);
                    }
                });

                foreach (var v in dict) {
                    v.Key.loss = v.Value;
                }

                var bestPop = dict
                    .Select(x => x.Key)
                    .Aggregate((a, b) => a.loss < b.loss ? a : b);

                if (best == null || best.loss > bestPop.loss) {
                    best = bestPop;
                }

                // Selection
                var sum = dict
                    .Select(x => x.Value)
                    .Sum();

                var ordered = dict
                    .OrderBy(x => x.Value)
                    .ToList();

                var elite = ordered
                    .Take(ELITE_POPULATION)
                    .Select(x => x.Key)
                    .ToList();

                var remaining = ordered
                    .Skip(ELITE_POPULATION)
                    .RandomPermutation(x => x.Value)
                    .Select(x => x.Key)
                    .Take(REMAINING_POPULATION)
                    .ToList();

                var survived = elite
                    .Concat(remaining)
                    .Select(x => random.Next(100) < 50
                        ? x.Mutate()
                        : x)
                    .ToList();

                var selectionPerm = Enumerable
                    .Range(0, ELITE_POPULATION + REMAINING_POPULATION)
                    .RandomPermutation()
                    .ToList();

                var newGeneration = new List<DecisionTree>();
                for (int i = 0; i < selectionPerm.Count; i += 2) {
                    var a1 = selectionPerm[i];
                    var a2 = selectionPerm[i + 1];

                    var child = Crossover(survived[a1], survived[a2]);
                    newGeneration.Add(child);
                }

                population = survived
                    .Concat(newGeneration)
                    .ToList();
            }

            return best;
        }

        private static DecisionTree Crossover(DecisionTree t1, DecisionTree t2) {
            var b = random.Next(1, 9);

            var newLineActions = Enumerable.Range(0, 8)
                .Select(x => b > x
                    ? t1.LineActions[x]
                    : t2.LineActions[x])
                .ToList();

            var newSpeed = b == 8
                ? t1.Speed
                : t2.Speed;

            return new DecisionTree() {
                Distance = t2.Distance,
                Speed = newSpeed,
                LineActions = newLineActions
            };
        }

        private static DecisionTree GenerateRandom() {
            return new DecisionTree() {
                Distance = random.NextDouble(5, 50),
                Speed = random.NextDouble(0.3, 2),
                LineActions = Enumerable.Range(0, 8)
                    .Select(x => (RobotAction)random.Next(8))
                    .ToList()
            };
        }

        public double Loss(Scene scene) {
            scene = scene.Clone();

            var splineMap = scene.SplineMap;

            var map = new bool[splineMap.Length, splineMap[0].Length];
            for (int i = 0; i < splineMap.Length; i++) {
                for (int j = 0; j < splineMap[0].Length; j++) {
                    if (splineMap[i][j] > 128) {
                        map[i, j] = true;
                    }
                }
            }

            var robot = scene.Robot;
            var revealed = 0;
            var toReveal = map.Cast<bool>()
                .Where(x => x)
                .Sum(x => 1);

            var maxIter = toReveal / 2;

            int it = 0;
            while (revealed != toReveal && it <= maxIter) {
                var (l1, l2, l3) = robot.LineSensor(splineMap);

                var state = new RobotState(
                    ultraSonic: robot.UltraSonicSensor(),
                    line: (l1 > 128, l2 > 128, l3 > 128)
                );

                var (action, speed) = Decide(state);

                var collision = robot.DoAction(action, speed);

                if (collision 
                        || robot.X < 0 
                        || robot.Y < 0 
                        || robot.X.Round() >= map.GetLength(1) 
                        || robot.Y.Round() >= map.GetLength(0)) {
                    return toReveal;
                }

                revealed += robot.MarkVisitedArea(map);

                if (robot.DistanceFromPath(splineMap) > Robot.LENGTH / 2) {
                    return toReveal - revealed;
                }

                it++;
            }

            return toReveal - revealed;
        }

        private DecisionTree Mutate() {
            var newDistance = Distance;
            var newSpeed = Speed;
            var newLineActions = new List<RobotAction>(LineActions);

            if (random.Next(100) < 10) {
                newDistance += random.Normal(mu: 0, sigma: 1);
            }

            if (random.Next(100) < 10) {
                newSpeed += random.Normal(mu: 0, sigma: 0.2);
            }

            for (int i = 0; i < LineActions.Count; i++) {
                if (random.Next(100) < 10) {
                    newLineActions[i] = (RobotAction)random.Next(7);
                }
            }

            return new DecisionTree() {
                Distance = newDistance,
                Speed = newSpeed,
                LineActions = newLineActions
            };
        }

        public (RobotAction, double speed) Decide(RobotState state) {
            if (state.UltraSonic <= Distance) {
                return (RobotAction.Stop, 0);
            }

            var (l1, l2, l3) = state.Line;

            uint line = 0;
            if (l1) line += 1 << 0;
            if (l2) line += 1 << 1;
            if (l3) line += 1 << 2;
            
            return (LineActions[(int)line], Speed);
        }

        
    }



}
