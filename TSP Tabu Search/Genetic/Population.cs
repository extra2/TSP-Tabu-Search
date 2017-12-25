using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP_Tabu_Search
{
    public class Population
    {
        public List<Tour> p;
        public double maxFit;
        public Random r = new Random();

        public Population(List<Tour> l)
        {
            p = l;
            maxFit = calcMaxFit();
        }

        // Functionality
        public static Population randomized(Tour t, int n)
        {
            List<Tour> tmp = new List<Tour>();

            for (int i = 0; i < n; ++i)
                tmp.Add(t.shuffle()); // losowe dogi

            return new Population(tmp);
        }

        private double calcMaxFit()
        {
            return p.Max(t => t.fitness);
        }

        public Tour selectTour()
        {
            while (true)
            {
                int i = r.Next(0, TSPGeneticSymetric.popSize);

                if (r.NextDouble() < p[i].fitness / maxFit)
                    return new Tour(p[i].t);
            }
        }

        public Population genNewPop(int n) // generuje nowa populacje n elementowa
        {
            List<int> p = new List<int>();

            for (int i = 0; i < n; ++i)
            {
                Tour t = selectTour().crossover(selectTour()); // wybieram 2 drogi 
                foreach (City c in t.t)
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
            Population tmp = new Population(p); // kopia obecnej

            for (int i = 0; i < n; ++i)
            {
                best.Add(tmp.findBest()); // najlepsza do listy
                tmp = new Population(tmp.p.Except(best).ToList()); // nowa populacja bez najlepszego
            }

            return new Population(best);
        }

        public Tour findBest() // szuka najlepszej populacji
        {
            foreach (Tour t in p)
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
            return new Population(best.p.Concat(np.p).ToList()); // połączenie powyższych populacji i zwrócenie sumy
        }
    }
}