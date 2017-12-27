using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using TspLibNet;
using TspLibNet.Graph.Nodes;

namespace TSP_Tabu_Search
{
    class TSPGeneticSymetric
    {
        public static int maxTimeInSec; // kryterium stopu
        public static double mutRate = 0.03; // wsp mutacji
        public static double canMutRate = 0.8; // wsp krzyzowania
        public static int elitism; // ile najlepszych miast wybieram
        public static int popSize; // rozmiar populacji
        public static int numCities; // ilość miast
        public const int unlimited = 999999999;
        public static List<Node2D> cityNodes = new List<Node2D>();
        public static int [,]nodesAsTab;
        public string SolveTSP(TravelingSalesmanProblem problem, int maxTime, int populationSize)
        {
            numCities = problem.NodeProvider.CountNodes();
            popSize = 50;//(int)(numCities*0.1); // ----------------------------
            elitism = popSize / 10; // -------------------------------- te liczby mają znaczenie
            maxTimeInSec = maxTime; // ------------------------------------
            foreach(var c in problem.NodeProvider.GetNodes()) // dodaję węzły do listy
            {
                cityNodes.Add((Node2D)c);
            }
            nodesAsTab = new int[numCities, numCities]; // tablic na odległości między miastami (już obliczone)
            calcDistances(); // wyliczam dystanse, aby nie liczyc ich kilka razy
            Tour dest = generateTour(numCities); // losuję pierwszą drogę
            Population p = Population.randomized(dest, popSize); // na jej podstawie losuję populację

            bool better = true;
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalSeconds <= maxTimeInSec)
            {

                if (better)
                {
                    display(p);
                    File.AppendAllText("1.txt", "\r\n" + (DateTime.Now - startTime).TotalSeconds.ToString());
                }

                    better = false;
                double oldFit = p.maxFit;

                p = p.evolve();
                if (p.maxFit > oldFit)
                    better = true;
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
                int numOfC = new Random().Next(citiesNotUsed.Count-1);
                t.Add(citiesNotUsed[numOfC]);
                citiesNotUsed.RemoveAt(numOfC);
            }
            return new Tour(t);
        }

        public static void calcDistances()
        {
            for (int i = 0; i < numCities; i++)
            {
                for (int j = 0; j < numCities ; j++)
                {
                    if(i == j)
                    {
                        nodesAsTab[i, j] = unlimited;
                        continue;
                    }
                    var xd = cityNodes[i].X - cityNodes[j].X;
                    var yd = cityNodes[i].Y - cityNodes[j].Y;
                    nodesAsTab[i,j] = (int)Math.Round(Math.Sqrt(xd * xd + yd * yd), MidpointRounding.AwayFromZero);
                }
            }
        }
        public static void display(Population p)
        {
            Tour best = p.findBest();
            string x = "\r\nGeneration " +
                                     "\r\nBest fitness:  " + best.fitness.ToString() +
                                     "\r\nBest distance: " + best.distance;
            File.AppendAllText("1.txt", x);
        }
    }
}