using System;

namespace RobbyRobot
{
    public class Individual
    {
        public Action[] Genome { get; private set; }
        public float Score { get; private set; }

        public Individual()
        {
            Genome = new Action[Constants.GenomeSize];
            Random rand = new Random();
            int actions = Enum.GetNames(typeof(Action)).Length;

            for (int i = 0; i < Genome.Length; ++i)
            {
                Genome[i] = (Action)rand.Next(actions);
            }

            Score = CalculateSessionScoreAverage();
        }

        public Individual[] MateWith(Individual other)
        {
            return null;
        }

        public override string ToString()
        {
            string s = "";

            foreach (var gene in Genome)
            {
                s += (int)gene;
            }

            return s;
        }

        public void UpdateScore()
        {
            Score = CalculateSessionScoreAverage();
        }

        private float CalculateSessionScoreAverage()
        {
            int scoreOfSessions = 0;

            for (int i = 0; i < Constants.Sessions; ++i)
            {
                scoreOfSessions += CalculateSessionScore();
            }

            return (float)scoreOfSessions / Constants.Sessions;
        }

        private int CalculateSessionScore()
        {
            Map map = new Map(10, 10, Constants.CanProbability);
            var robotPosition = new Coordinate(0, 0);
            int score = 0;

            for (int i = 0; i < Constants.ActionsPerSession; ++i)
            {
                var neighborhood = GetNeighborhood(map, robotPosition);
                int gene = GetGeneFromNeighborhood(neighborhood);

                if (Genome[gene] == Action.MoveNorth)
                {
                    robotPosition.Y -= 1;
                }
                else if (Genome[gene] == Action.MoveSouth)
                {
                    robotPosition.Y += 1;
                }
                else if (Genome[gene] == Action.MoveEast)
                {
                    robotPosition.X += 1;
                }
                else if (Genome[gene] == Action.MoveWeast)
                {
                    robotPosition.X -= 1;
                }
                else if (Genome[gene] == Action.PickUp)
                {
                    if (map.Area[robotPosition.X, robotPosition.Y] == State.Can)
                    {
                        score += 10;
                        map.Area[robotPosition.X, robotPosition.Y] = State.Empty;
                    }
                    else
                    {
                        score -= 1;
                    }
                }
                else if (Genome[gene] == Action.MoveRandom)
                {
                    Random rand = new Random();
                    var direction = (Direction)rand.Next(2);
                    var distance = rand.Next(-1, 2);

                    if (direction == Direction.Horizontal)
                    {
                        robotPosition.X += distance;
                    }
                    else if (direction == Direction.Vertical)
                    {
                        robotPosition.Y += distance;
                    }
                }

                if (robotPosition.Y < 0)
                {
                    robotPosition.Y = 0;
                    score -= 5;
                }

                if (robotPosition.Y >= map.Area.GetLength(0))
                {
                    robotPosition.Y = map.Area.GetLength(0) - 1;
                    score -= 5;
                }

                if (robotPosition.X >= map.Area.GetLength(1))
                {
                    robotPosition.X = map.Area.GetLength(1) - 1;
                    score -= 5;
                }

                if (robotPosition.X < 0)
                {
                    robotPosition.X = 0;
                    score -= 5;
                }
            }

            return score;
        }

        public State[] GetNeighborhood(Map map, Coordinate currentPosition)
        {
            return new State[]
            {
                currentPosition.Y - 1 >= 0 ? map.Area[currentPosition.X, currentPosition.Y - 1] : State.Wall,
                currentPosition.Y + 1 < map.Area.GetLength(0) ? map.Area[currentPosition.X, currentPosition.Y + 1] : State.Wall,
                currentPosition.X + 1 < map.Area.GetLength(1) ? map.Area[currentPosition.X, currentPosition.X + 1] : State.Wall,
                currentPosition.X - 1 >= 0 ? map.Area[currentPosition.X, currentPosition.X - 1] : State.Wall,
                map.Area[currentPosition.X, currentPosition.Y]
            };
        }

        private int GetGeneFromNeighborhood(State[] neighborhood)
        {
            //Array.Reverse(neighborhood);
            int gene = 0;

            for (int i = 0; i < neighborhood.Length; ++i)
            {
                gene += (int)neighborhood[neighborhood.Length - i - 1] * (int)Math.Pow(3, i);
            }

            return gene;
        }
    }
}