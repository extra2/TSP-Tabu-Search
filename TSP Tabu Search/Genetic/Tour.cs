using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP_Tabu_Search
{
    public class Tour
    {

        // Member variables
        public List<int> t { get; private set; }

        public double distance { get; private set; }
        public double fitness { get; private set; }
        public static Random r = new Random();
        // ctor
        public Tour(List<int> listOfCities)
        {
            t = listOfCities;
            distance = calcDist();
            fitness = calcFit();
        }

        // Functionality


        public Tour shuffle() // losuję (tasuję) drogę
        {
            List<int> tmp = new List<int>(t); // kopiuję listę, żeby nie przekazać wskaźnika
            int n = tmp.Count;

            while (n > 1) // losuję dowolną drogę
            {
                n--;
                int k = r.Next(n + 1); // losuję wierzchołek
                int v = tmp[k]; // zamieniam go z innym
                tmp[k] = tmp[n];
                tmp[n] = v;
            }

            return new Tour(tmp); // zwracam nową losową drogę
        }

        public Tour crossover(Tour m)
        {
            int i = r.Next(0, m.t.Count);
            int j = r.Next(i, m.t.Count);
            List<int> s = t.GetRange(i, j - i + 1);
            List<int> ms = m.t.Except(s).ToList();
            List<int> c = ms.Take(i)
                .Concat(s)
                .Concat(ms.Skip(i))
                .ToList();
            return new Tour(c);
        }

        public Tour mutate()
        {
            List<int> tmp = new List<int>(t);

            if (r.NextDouble() < TSPGeneticSymetric.mutRate)
            {
                int i = r.Next(0, t.Count);
                int j = r.Next(0, t.Count);
                int v = tmp[i];
                tmp[i] = tmp[j];
                tmp[j] = v;
            }

            return new Tour(tmp);
        }

        private double calcDist()
        {
            double total = 0;
            for (int i = 0; i < t.Count - 2; ++i)
                total += TSPGeneticSymetric.nodesAsTab[t[i]-1, t[i+1]-1];
            total += TSPGeneticSymetric.nodesAsTab[t[t.Count - 1]-1, t[0]-1];
            return total;

            // Execution time is doubled by using linq in this case
            //return t.Sum( c => c.distanceTo(t[ (t.IndexOf(c) + 1) % t.Count] ) );
        }

        private double calcFit()
        {
            return 1 / distance;
        }
    }
}