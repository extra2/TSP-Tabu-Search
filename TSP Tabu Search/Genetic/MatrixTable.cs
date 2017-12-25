using System;
using System.Collections.Generic;
using System.IO;

namespace TSP_Tabu_Search
{
    public class MatrixTable
    {
        public int[,] Matrix;
        public int Dimension;
        public MatrixTable(string filename)
        {
            AsymetricMatrix(filename);
        }
        private void AsymetricMatrix(string filename)
        {
            var matrixLines = File.ReadAllLines(filename);
            // find dimension
            foreach (var line in matrixLines)
            {
                if (line.Contains("DIMENSION: "))
                {
                    Dimension = Int32.Parse(line.Replace(" ", "").Split(':')[1]);
                    break;
                }
            }
            Matrix = new int[Dimension, Dimension];
            // find first line with data
            int startLine = 0;
            for (int i = 0; i < Dimension; i++)
            {
                if (matrixLines[i].Split(' ')[0] == "EDGE_WEIGHT_SECTION")
                {
                    startLine = i + 1;
                    break;
                }
            }
            string numbers = "";
            // make it one line contains all data
            for (int i = startLine; i < matrixLines.Length - 1; i++)
                numbers += matrixLines[i];

            // get data from that line
            List<int> tabForMatrix = new List<int>();
            var linew = numbers.Split(' ');
            foreach (var value in linew)
            {
                if (value != "") tabForMatrix.Add(Int32.Parse(value));
            }
            // add data to matrix
            int index = 0;
            for (int i = 0; i < Dimension; i++)
            {
                for (int j = 0; j < Dimension; j++)
                {
                    if (i == j) Matrix[i, j] = -1;
                    else
                    {
                        Matrix[i, j] = tabForMatrix[index];
                    }
                    index++;
                }
            }
        }
    }
}