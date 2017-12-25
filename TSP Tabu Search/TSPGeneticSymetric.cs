using System;

namespace TSP_Tabu_Search
{
    class TSPGeneticSymetric
    {
        public const int maxTimeInSec = 60; // kryterium stopu
        public const double mutRate = 0.03; // wsp mutacji
        public const double canMutRate = 0.8; // wsp krzyzowania
        public const int elitism = 6; // ile najlepszych miast wybieram
        public const int popSize = 60;
        public const int numCities = 40;

        public TSPGeneticSymetric()
        {
        }

        public void SolveTSP()
        {
            Tour dest = Tour.random(numCities);
            Population p = Population.randomized(dest, popSize);

            int gen = 0;
            bool better = true;
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalSeconds <= maxTimeInSec)
            {
                if (better)
                    display(p, gen);

                better = false;
                double oldFit = p.maxFit;

                p = p.evolve();
                if (p.maxFit > oldFit)
                    better = true;

                gen++;
            }
        }

        public static void display(Population p, int gen)
        {
            Tour best = p.findBest();
            Console.WriteLine("Generation {0}\n" +
                                     "Best fitness:  {1}\n" +
                                     "Best distance: {2}\n", gen, best.fitness, best.distance);
        }
    }
}