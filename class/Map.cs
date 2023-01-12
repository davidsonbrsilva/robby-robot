using System;

namespace RobbyRobot
{
    public class Map
    {
        public State[,] Area { get; }

        public Map(int xSize, int ySize, float canProbability = 0.2f)
        {
            Area = new State[xSize, ySize];
            Random rand = new Random();

            for (int x = 0; x < xSize; ++x)
            {
                for (int y = 0; y < ySize; ++y)
                {
                    float cutoff = (float)rand.NextDouble();
                    
                    if (cutoff < canProbability)
                    {
                        Area[x, y] = State.Can;
                    }
                }
            }
        }

        public override string ToString()
        {
            string s = "";

            for (int x = 0; x < Area.GetLength(0); ++x)
            {
                for (int y = 0; y < Area.GetLength(1); ++y)
                {
                    if (Area[x, y] == State.Empty)
                    {
                        s += "+ ";
                    }
                    else if (Area[x, y] == State.Can)
                    {
                        s += "0 ";
                    }
                    else
                    {
                        s += "# ";
                    }
                }

                s += "\n";
            }

            s += "\n";

            return s;
        }
    }
}