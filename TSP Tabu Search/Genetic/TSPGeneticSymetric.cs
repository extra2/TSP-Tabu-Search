using System;
using System.Collections.Generic;
using System.Linq;
using TspLibNet;
using TspLibNet.Graph.Nodes;

namespace TSP_Tabu_Search
{
    class TSPGeneticSymetric
    {
        public static int maxTimeInSec; // kryterium stopu
        public static double mutRate = 0.03; // wsp mutacji
        public static double canMutRate = 0.8; // wsp krzyzowania
        public static int elitism = 6; // ile najlepszych miast wybieram
        public static int popSize = 60; // rozmiar populacji
        public static int numCities = 40; // ilość miast
        public List<Node2D> cityNodes = new List<Node2D>();

        public string SolveTSP(TravelingSalesmanProblem problem, int maxTime, int populationSize)
        {
            numCities = problem.NodeProvider.CountNodes();
            popSize = populationSize;
            maxTimeInSec = maxTime;
            foreach(var c in problem.NodeProvider.GetNodes()) // dodaję węzły do listy
            {
                cityNodes.Add((Node2D)c);
            }


            Tour dest = generateTour(numCities);
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
            return "";
        }
        public static Tour generateTour(int n)
        {
            List<int> t = new List<int>();
            List<int> citiesNotUsed = new List<int>();
            for (int i = 0; i < n; ++i)
            {
                citiesNotUsed.Add(i + 1);
            }
            while (citiesNotUsed.Count > 0)
            {
                int numOfC = new Random().Next() % citiesNotUsed.Count;
                if (numOfC == 0) continue;
                t.Add(numOfC);
                citiesNotUsed.Remove(numOfC);
            }
            return new Tour(t);
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