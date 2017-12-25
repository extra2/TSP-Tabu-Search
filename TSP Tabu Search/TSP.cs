using System;
using System.Windows.Forms;
using TspLibNet;

namespace TSP_Tabu_Search
{
    class TSP
    {
        private TravelingSalesmanProblem problem;
        private TSPSymetric tspSymetric;
        private TSPAsymetric tspAsymetric;
        private MatrixTable _matrixTable;
        private string status = "";
        public string StartTsp(int MaxTime, string filename)
        {

            try
            {
                problem = TravelingSalesmanProblem.FromFile(filename); // create TravelingSalesmanProblem object
                switch (problem.Type) // type of nodes
                {
                    case ProblemType.TSP:
                        status = new TSPSymetric(problem, MaxTime).SolveTSP();
                        break;
                    case ProblemType.ATSP:
                        _matrixTable = new MatrixTable(filename);
                        status = new TSPAsymetric(MaxTime, _matrixTable).SolveTSP();
                        break;
                    default:
                        MessageBox.Show("Loaded file is not tsp/atsp file. returning.");
                        return status;
                }
            }
            catch (Exception)
            {
                MessageBox.Show("Can't load file " + filename);
            }
            return status;
        }
    }
}