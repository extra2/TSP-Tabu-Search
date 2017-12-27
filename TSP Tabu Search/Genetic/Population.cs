using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP_Tabu_Search
{
    public class Population
    {
        public List<Tour> tourList;
        public double maxFit;
        public Random r = new Random();

        public Population(List<Tour> l)
        {
            tourList = l;
            maxFit = calcMaxFit();
        }

        // Functionality
        public static Population randomPopulation(Tour t, int n) // tworzę populację losowych dróg
        {
            List<Tour> tmp = new List<Tour>();

            for (int i = 0; i < n; ++i)
                tmp.Add(t.randomTour()); // losowe dogi

            return new Population(tmp); // zwracam populację
        }

        private double calcMaxFit()
        {
            return tourList.Max(t => t.fitness);
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
                if (r.NextDouble() > TSPGeneticSymetric.canMutRate) continue;  // nie mutuję

                Tour t = selectRandomTour().crossover(selectRandomTour()); // wybieram 2 drogi, krzyżuję je ze sobą
                foreach (int c in t.tour) // mutuję 
                {
                    t = t.mutate(); 
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

        public Tour findBest() // szuka najlepszej populacji
        {
            foreach (Tour t in tourList)
            {
                if (t.fitness == maxFit)
                    return t;
            }
            return null; // jesli do tego dojdzie, to cos jest zle
        }

        public Population evolve()
        {
            Population best = findNBestTours(TSPGeneticSymetric.numOfBestCities);
            Population np = genNewPop(TSPGeneticSymetric.popSize - TSPGeneticSymetric.numOfBestCities); // nowa populacja, wielkosc populacji - ilosc "najlepszych"
            return new Population(best.tourList.Concat(np.tourList).ToList()); // połączenie powyższych populacji i zwrócenie sumy
        }
    }
}