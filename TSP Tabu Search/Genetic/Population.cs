using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP_Tabu_Search
{
    public class Population
    {
        public List<Tour> tourList;
        public double bestDistance;
        public Random r = new Random();

        public Population(List<Tour> l)
        {
            tourList = l;
            bestDistance = findBestDistance();
        }

        // Functionality
        public static Population randomPopulation(Tour t, int n) // tworzę populację losowych dróg
        {
            List<Tour> tmp = new List<Tour>();
            for (int i = 0; i < n; ++i)
            {
                tmp.Add(t.randomTour()); // losowe dogi
            }
            return new Population(tmp); // zwracam populację
        }

        private double findBestDistance()
        {
            return tourList.Min(t => t.distance);
        }

        public Tour selectRandomTour()
        {
            return new Tour(tourList[r.Next(0, tourList.Count)].tour);
        }
        // 3 etap ewolucji: reprodukcja
        public Population genNewPop(int n) // generuje nowa populacje n elementowa 
        {
            List<Tour> p = new List<Tour>();

            for (int i = 0; i < n; ++i)
            {
                // warunek mutacji nr 1 - możliwość mutacji 80% (z założeń zadania):
                if (r.NextDouble() > TSPGenetic.canMutRate) continue;  // nie mutuję

                Tour t = selectRandomTour().cross(selectRandomTour()); // wybieram 2 drogi, krzyżuję je ze sobą
                foreach (int c in t.tour) // mutuję 
                {
                    t = t.mutation(); 
                }
                p.Add(t);
            }

            return new Population(p);
        }

        public Population findNBestTours(int n) // szuka n najlepszych drog w obecnej populacji
        {
            List<Tour> best = new List<Tour>(); // najlepsze drogi
            Population tmp = new Population(tourList); // kopia obecnej

            for (int i = 0; i < n; ++i)
            {
                best.Add(tmp.findBest()); // najlepsza do listy
                tmp = new Population(tmp.tourList.Except(best).ToList()); // nowa populacja bez najlepszego
            }

            return new Population(best);
        }

        public Tour findBest() // szuka najlepszej drogi
        {
            foreach (Tour t in tourList)
            {
                if (t.distance == bestDistance)
                    return t;
            }
            return null; // jesli do tego dojdzie, to cos jest zle
        }

        public Population createNewPopulation() // nowa populacja = N najlepszych dróg + nowe drogi (rozmiar populacji - N najlepszych)
        {
            Population bestTours = findNBestTours(TSPGenetic.numOfBestRoads); // znajduję N najlepszych dróg
            Population newTours = genNewPop(TSPGenetic.popSize - TSPGenetic.numOfBestRoads); // nowa populacja, wielkosc populacji - ilosc "najlepszych" dróg
            return new Population(bestTours.tourList.Concat(newTours.tourList).ToList()); // połączenie powyższych populacji i zwrócenie sumy
        }
    }
}