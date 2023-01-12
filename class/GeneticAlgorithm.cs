using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace RobbyRobot
{
    public class GeneticAlgorithm
    {
        public List<Individual> Population { get; private set; }
        public Individual BestIndividual
        {
            get
            {
                return Population.OrderByDescending(x => x.Score).FirstOrDefault();
            }
        }
        public Individual WorstIndividual
        {
            get
            {
                return Population.OrderBy(x => x.Score).FirstOrDefault();
            }
        }

        public GeneticAlgorithm(int populationSize)
        {
            Population = new List<Individual>();

            for (int i = 0; i < populationSize; ++i)
            {
                Population.Add(new Individual());
            }
        }

        private Individual Select()
        {
            float sum = Population.Sum(x => GetFitness(x));

            Random rand = new Random();
            float cutoff = (float)rand.NextDouble() * sum;

            float accumulated = 0f;

            foreach (var individual in Population)
            {
                accumulated += GetFitness(individual);

                if (accumulated > cutoff)
                {
                    return individual;
                }
            }

            return null;
        }

        private Individual[] Mate(Individual first, Individual second)
        {
            Random rand = new Random();
            int cutoff = rand.Next(Constants.GenomeSize);
            var childs = new Individual[2];

            for (int i = 0; i < childs.Length; ++i)
            {
                childs[i] = new Individual();
            }

            // Mate

            for (int i = 0; i < cutoff; ++i)
            {
                childs[0].Genome[i] = first.Genome[i];
                childs[1].Genome[i] = second.Genome[i];
            }

            for (int i = cutoff; i < Constants.GenomeSize; ++i)
            {
                childs[0].Genome[i] = second.Genome[i];
                childs[1].Genome[i] = first.Genome[i];
            }

            // Mutation

            for (int i = 0; i < Constants.GenomeSize; ++i)
            {
                float probability = (float)rand.NextDouble();

                if (probability < Constants.MutationProbability)
                {
                    childs[0].Genome[i] = (Action)rand.Next(Enum.GetNames(typeof(Action)).Length);
                }

                probability = (float)rand.NextDouble();

                if (probability < Constants.MutationProbability)
                {
                    childs[1].Genome[i] = (Action)rand.Next(Enum.GetNames(typeof(Action)).Length);
                }
            }

            return childs;
        }

        private void ApplyEvolution()
        {
            var newPopulation = new List<Individual>();

            while (newPopulation.Count < Population.Count)
            {
                var parent1 = Select();
                var parent2 = Select();

                while (parent2 == parent1)
                {
                    parent2 = Select();
                }

                var childs = new Individual[2];
                childs[0] = parent1;
                childs[1] = parent2;

                Random rand = new Random();
                float probability = (float)rand.NextDouble();

                if (probability < Constants.MateRate)
                {
                    childs = Mate(parent1, parent2);
                    childs[0].UpdateScore();
                    childs[1].UpdateScore();
                }

                newPopulation.AddRange(childs);
            }

            Population = newPopulation;
        }

        public float GetFitness(Individual individual)
        {
            Individual best = BestIndividual;
            Individual worst = WorstIndividual;

            return (individual.Score - worst.Score) / (best.Score - worst.Score);
        }

        private string Summary(Individual individual)
        {
            string s = "[";

            for (int i = 0; i < 6; ++i)
            {
                s += (int)individual.Genome[i];
            }

            s += "...";

            for (int i = individual.Genome.Length - 6; i < individual.Genome.Length; ++i)
            {
                s += (int)individual.Genome[i];
            }

            return s + "]";
        }

        public void Run()
        {
            Console.WriteLine("GEN    BEST               FITNESS   WORST              FITNESS   EVOLUTION TIME");

            var best = BestIndividual;
            var worst = WorstIndividual;

            string row = string.Format("#{0,-4}  {1}  {2,8:0.00}  {3}  {4,8:0.00}  -", 0, Summary(best), best.Score, Summary(worst), worst.Score);

            Console.WriteLine(row);

            for (int i = 1; i < Constants.Executions; ++i)
            {
                best = BestIndividual;
                worst = WorstIndividual;

                var watch = new Stopwatch();
                watch.Start();
                ApplyEvolution();
                watch.Stop();
                var ts = watch.Elapsed;

                row = string.Format("#{0,-4}  {1}  {2,8:0.00}  {3}  {4,8:0.00}  {5:00}:{6:00}:{7:00}.{8:00}", i, Summary(best), best.Score, Summary(worst), worst.Score, ts.Hours, ts.Minutes, ts.Seconds, ts.Milliseconds / 10);

                Console.WriteLine(row);
            }
        }
    }
}