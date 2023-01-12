using System;
using System.Linq;

namespace RobbyRobot
{
    public class MainClass
    {
        public static void Main (string[] args)
        {
            GeneticAlgorithm ga = new GeneticAlgorithm(200);
            ga.Run();
        }
    }
}
