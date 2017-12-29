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
        public static Population randomPopulation(Tour t, int n) // tworzę populację losowych dróg
        {
            List<Tour> tmp = new List<Tour>();
            for (int i = 0; i < n; ++i)
            {
                tmp.Add(t.randomTour()); // losowe dogi
            }
            return new Population(tmp); // zwracam populację
        }

        private double findBestDistance() // najmniejszy dystans
        {
            return tourList.Min(t => t.distance);
        }

        public Tour selectRandomTour()
        {
            return new Tour(tourList[r.Next(0, tourList.Count)].tour);
        }
        // 3 etap ewolucji: reprodukcja
        public Population genNewPop(int n, Population bestPopulation) // generuje nowa populacje n elementowa jako krzyzowanie 2 osobnikow
        {
            List<Tour> p = new List<Tour>(); // lista dla nowej populacji
            foreach (var pop in bestPopulation.tourList) // krzyżuję z najlepszymi osobnikami
            {
                // warunek mutacji nr 1 - możliwość krzyżowania 80% (z założeń zadania):
                if (r.NextDouble() < TSPGenetic.canMutRate)
                {
                    Tour randomTour = selectRandomTour();
                    Tour[] t = pop.cross(randomTour);
                    t[0] = t[0].mutation();
                    t[1] = t[1].mutation();
                    tourList.Remove(randomTour);
                    p.Add(t[0]);
                    p.Add(t[1]);
                }
            }
            while(tourList.Count > 1) { // krzyżuję z pozostałymi drogami
                // warunek mutacji nr 1 - możliwość krzyżowania 80% (z założeń zadania):
                if (r.NextDouble() < TSPGenetic.canMutRate)
                {
                    // nie mutuję
                    Tour randomTour = selectRandomTour(); // losuję drogę inną niż aktualnie rozpatrywana
                    while (randomTour == tourList[0]) randomTour = selectRandomTour();
                    Tour[] t = tourList[0].cross(randomTour); // wybieram 2 drogi, krzyżuję je ze sobą
                    // mutuję geny:
                    t[0] = t[0].mutation();
                    t[1] = t[1].mutation();
                    tourList.Remove(randomTour); // usuwam z listy populacji stare osobniki
                    tourList.Remove(tourList[0]);
                    if (p.Count == n) break;
                    p.Add(t[0]); // dodaję nowe osobniki do nowej populacji
                    if (p.Count == n) break;
                    p.Add(t[1]);
                }
                else // nie krzyżuję
                {
                    if (p.Count == n) break;
                    p.Add(tourList[0]);
                    tourList.Remove(tourList[0]);
                }
            }
            if(tourList.Count == 1) p.Add(tourList[0]); // jeśli zachowała się jedna droga, dodaję ją

            return new Population(p); // nowa populacja
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
            tourList = tmp.tourList;
            findBestDistance(); // zmodyfikowałem populację, więc muszę wyszukać nową najlepszą drogę
            return new Population(best);
        }

        public Tour findBest() // szuka najlepszej drogi
        {
            foreach (Tour t in tourList)
            {
                if (t.distance == bestDistance)
                    return t;
            }
            return null; // jesli do tego dojdzie, to cos jest nie tak (musi zostać gdyż funkcja musi coś zwrócić)
        }

        public Population createNewPopulation() // nowa populacja = N najlepszych dróg + nowe drogi (rozmiar populacji - N najlepszych)
        {
            Population bestTours = findNBestTours(TSPGenetic.numOfBestRoads); // znajduję N najlepszych dróg
            Population newTours = genNewPop(TSPGenetic.popSize - TSPGenetic.numOfBestRoads, bestTours); // nowa populacja, wielkosc populacji - ilosc "najlepszych" dróg
            return new Population(bestTours.tourList.Concat(newTours.tourList).ToList()); // połączenie powyższych populacji i zwrócenie sumy
        }
    }
}