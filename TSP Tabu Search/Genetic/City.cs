using System;

namespace TSP_Tabu_Search
{
    public class City // to wywalic, lista nodow
    {
        // Member variables
        public double x;

        public double y;

        public static Random r = new Random();
        // ctor
        public City(double x, double y)
        {
            this.x = x;
            this.y = y;
        }

        // Functionality
        public double distanceTo(City c)
        {
            return Math.Sqrt(Math.Pow((c.x - this.x), 2)
                             + Math.Pow((c.y - this.y), 2));
        }

        public static City random() // to wywalić - generuje losowe miasto
        {
            return new City(r.NextDouble(), r.NextDouble());
        }
    }
}