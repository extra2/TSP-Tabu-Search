using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP_Tabu_Search
{
    public class Tour
    {
        public List<int> tour { get; private set; }
        public double distance { get; private set; }
        public static Random r = new Random();
        public Tour(List<int> listOfCities)
        {
            tour = listOfCities;
            distance = calcDist();
        }
        public Tour randomTour() // losuję nową drogę zamieniając ze sobą wierzchołki
        {
            List<int> newTour = new List<int>(tour); // kopiuję listę, żeby nie przekazać wskaźnika
            int i = newTour.Count;

            while (i > 1) // losuję drogę, robię n zamian
            {
                i--;
                int j = r.Next(i + 1); // losuję wierzchołek (+1, bo naxValue in <0,n+1)
                int backup = newTour[j];
                newTour[j] = newTour[i]; // zamieniam go z innym (z n)
                newTour[i] = backup;
            } // mam nową drogę
            return new Tour(newTour); // zwracam nową losową drogę
        }

        public Tour cross(Tour otherTour)// wycina i łączy dwa chromosomy
        {
            int i = r.Next(0, otherTour.tour.Count);
            int j = r.Next(i, otherTour.tour.Count);
            int numOfElements = j - i + 1;// |A-----i------j-------B| eg i = 0, j = 1 => elements 0 and 1 => j-i = 1 => nope, mamy 2 elementy, j-i+1 = 2 => 2 elements
            List<int> fromCurrentTour = tour.GetRange(i, numOfElements); // wycinam i-j z lokalnej drogi
            List<int> fromOtherTour = otherTour.tour.Except(fromCurrentTour).ToList(); // A-B bez i-j
            List<int> newTourAfterCrossover = fromOtherTour.Take(i).ToList(); // c => biorę A-i z drugiej drogi (przesłanej jako argument)
            newTourAfterCrossover = newTourAfterCrossover.Concat(fromCurrentTour).ToList(); // c += i-j z aktualnej (lokalnej) drogi
            newTourAfterCrossover = newTourAfterCrossover.Concat(fromOtherTour.Skip(i)).ToList(); // c += j-B z drugiej drogi (argument)
            return new Tour(newTourAfterCrossover); // gotowa nowa droga, zwracam
        }

        public Tour mutation() // jeśli następuje mutacja (prawdop. 1%), zamieniam miejscami dwa geny
        {
            List<int> tmp = new List<int>(tour);

            if (r.NextDouble() < TSPGenetic.mutRate)// warunek mutacji nr 2 - szansa na mutację genu 1%
            {
                // losuję 2 geny i zamieniam je ze sobą miejszami - następuje mutacja
                int i = r.Next(0, tour.Count);
                int j = r.Next(0, tour.Count);
                int v = tmp[i];
                tmp[i] = tmp[j];
                tmp[j] = v;
            }
            return new Tour(tmp);
        }

        private double calcDist() // liczę całkowity koszt drogi
        {
            double total = 0;
            for (int i = 0; i < tour.Count - 2; ++i)
                total += TSPGenetic.nodesAsTab[tour[i] - 1, tour[i + 1] - 1];
            total += TSPGenetic.nodesAsTab[tour[tour.Count - 1] - 1, tour[0] - 1];
            return total;
        }
    }
}