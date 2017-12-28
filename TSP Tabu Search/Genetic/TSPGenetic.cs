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
    class TSPGenetic
    {
        public static int maxTimeInSec; // kryterium stopu
        public static double mutRate = 0.01; // wsp mutacji
        public static double canMutRate = 0.8; // wsp krzyzowania
        public static int numOfBestRoads; // ile najlepszych dróg wybieram
        public static int popSize; // rozmiar populacji
        public static int numCities; // ilość miast
        public const int unlimited = 999999999;
        public static List<Node2D> cityNodes = new List<Node2D>();
        public static int [,]nodesAsTab;
        public string SolveTSP(TravelingSalesmanProblem problem, int maxTime, int populationSize, int [,]asymetricMateix, bool isAsimetric)
        {
            popSize = 50;// rozmiar populacji, wpływa na prędkość działania oraz wynik
            numOfBestRoads = popSize / 10; // "Strategia elitarna" - zachowuje najlepsze geny
            maxTimeInSec = maxTime; // ------------------------------------
            if (isAsimetric == false) // jeśli wczytano problem symetryczny
            {
                numCities = problem.NodeProvider.CountNodes();
                foreach (var c in problem.NodeProvider.GetNodes()) // dodaję węzły do listy
                {
                    cityNodes.Add((Node2D) c);
                }
                nodesAsTab = new int[numCities, numCities]; // tablic na odległości między miastami (już obliczone)
                calcDistances(); // wyliczam dystanse, aby nie liczyc ich kilka razy
            }
            else // jeśli wczytano problem asymetryczny
            {
                numCities = (int)Math.Sqrt(asymetricMateix.Length);
                nodesAsTab = asymetricMateix;
            }
            Tour firstTour = generateTour(numCities); // losuję pierwszą drogę
            Population population = Population.randomPopulation(firstTour, popSize); // na jej podstawie losuję populację

            bool better = true;
            DateTime startTime = DateTime.Now;
            while ((DateTime.Now - startTime).TotalSeconds <= maxTimeInSec)
            {

                if (better)
                {
                    saveToFile(population);
                    File.AppendAllText("1.txt", "\r\n" + (DateTime.Now - startTime).TotalSeconds.ToString());
                }

                    better = false;
                double oldBestTour = population.bestDistance;

                population = population.createNewPopulation();
                if (population.bestDistance < oldBestTour) better = true;
            }
            return prepareStatus(population);
        }
        public static Tour generateTour(int n) // generuję drogę 1...n
        {
            List<int> t = new List<int>();
            for (int i = 0; i < n; ++i)
            {
                t.Add(i+1);
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

        public string prepareStatus(Population p)
        {
            string status = "";
            Tour best = p.findBest();
            status += "Najlepszy koszt drogi: " + best.distance.ToString();
            status += "\r\nDroga: ";
            for(int i = 0; i < numCities-1; i++)
            {
                status += best.tour[i].ToString() + "->";
            }
            status += best.tour.Last().ToString();
            return status;

        }
        public static void saveToFile(Population p) // to collect data
        {
            Tour best = p.findBest();
            string x =  "\r\nBest found distance: " + best.distance;
            File.AppendAllText("1.txt", x);
        }
    }
}