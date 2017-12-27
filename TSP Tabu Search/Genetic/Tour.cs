using System;
using System.Collections.Generic;
using System.Linq;

namespace TSP_Tabu_Search
{
    public class Tour
    {

        // Member variables
        public List<int> tour { get; private set; }

        public double distance { get; private set; }
        public double fitness { get; private set; }
        public static Random r = new Random();
        // ctor
        public Tour(List<int> listOfCities)
        {
            tour = listOfCities;
            distance = calcDist();
            fitness = calcFit();
        }

        // Functionality


        public Tour randomTour() // losuję nową drogę zamieniając ze sobą wierzchołki
        {
            List<int> newTour = new List<int>(tour); // kopiuję listę, żeby nie przekazać wskaźnika
            int n = newTour.Count;

            while (n > 1) // losuję dowolną drogę
            {
                n--;
                int k = r.Next(n + 1); // losuję wierzchołek +1, bo naxValue in <0,n+1)
                int v = newTour[k];
                newTour[k] = newTour[n]; // zamieniam go z innym (z n)
                newTour[n] = v;
            } // mam nową drogę
            return new Tour(newTour); // zwracam nową losową drogę
        }

        public Tour crossover(Tour otherTour)
        {
            int i = r.Next(0, otherTour.tour.Count);
            int j = r.Next(i, otherTour.tour.Count);
            int numOfElements = j - i + 1;// |A-----i------j-------B| eg i = 0, j = 1 => elements 0 and 1 => j-i = 1 => nope, mamy 2 elementy, j-i+1 = 2 => 2 elements
            List<int> fromCurrentTour = tour.GetRange(i, numOfElements); // wycinam i-j z lokalnej drogi
            List<int> fromOtherTour = otherTour.tour.Except(fromCurrentTour).ToList(); // A-B bez i-j
            List<int> c = fromOtherTour.Take(i).ToList(); // c => biorę A-i z drugiej drogi (przesłanej jako argument)
            c = c.Concat(fromCurrentTour).ToList(); // c += i-j z aktualnej (lokalnej) drogi
            c = c.Concat(fromOtherTour.Skip(i)).ToList(); // c += j-B z drugiej drogi (argument)
            return new Tour(c); // gotowa nowa droga, zwracam
        }

        public Tour mutate()
        {
            List<int> tmp = new List<int>(tour);

            if (r.NextDouble() < TSPGeneticSymetric.mutRate)// warunek mutacji nr 2 - szansa na mutację genu 1%
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
                total += TSPGeneticSymetric.nodesAsTab[tour[i] - 1, tour[i + 1] - 1];
            total += TSPGeneticSymetric.nodesAsTab[tour[tour.Count - 1] - 1, tour[0] - 1];
            return total;
        }

        private double calcFit()
        {
            return 1 / distance;
        }
    }
}