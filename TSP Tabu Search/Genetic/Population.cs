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
        public static Population randomized(Tour t, int n) // tworzę populację losowych dróg
        {
            List<Tour> tmp = new List<Tour>();

            for (int i = 0; i < n; ++i)
                tmp.Add(t.shuffle()); // losowe dogi

            return new Population(tmp); // zwracam populację
        }

        private double calcMaxFit()
        {
            return tourList.Max(t => t.fitness);
        }

        public Tour selectTour()
        {
            while (true)
            {
                int i = r.Next(0, TSPGeneticSymetric.popSize);

                if (r.NextDouble() < tourList[i].fitness / maxFit)
                    return new Tour(tourList[i].t);
            }
        }
        // 3 etap ewolucji: reprodukcja
        public Population genNewPop(int n) // generuje nowa populacje n elementowa
        {
            List<Tour> p = new List<Tour>();

            for (int i = 0; i < n; ++i)
            {
                Tour t = selectTour().crossover(selectTour()); // wybieram 2 drogi 
                foreach (int c in t.t)
                {
                    if (r.NextDouble() < TSPGeneticSymetric.canMutRate) t = t.mutate();
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
            return null; // jesli do tego dojdzie, to cos jest zle - najprawdopodobniej mamy wieksza populaje elit niz liczbe miast
        }

        public Population evolve()
        {
            Population best = findNBestTours(TSPGeneticSymetric.elitism);
            Population np = genNewPop(TSPGeneticSymetric.popSize - TSPGeneticSymetric.elitism); // nowa populacja, wielkosc populacji - ilosc "najlepszych"
            return new Population(best.tourList.Concat(np.tourList).ToList()); // połączenie powyższych populacji i zwrócenie sumy
        }
    }
}