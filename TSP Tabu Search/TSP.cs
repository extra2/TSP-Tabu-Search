﻿using System;
using System.Windows.Forms;
using TspLibNet;

namespace TSP_Tabu_Search
{
    class TSP
    {
        private TravelingSalesmanProblem problem;
        private MatrixTable _matrixTable;
        private string status = "";
        public string StartTsp(int MaxTime, string filename, bool isGenetic)
        {
            //try
            //{
                problem = TravelingSalesmanProblem.FromFile(filename); // create TravelingSalesmanProblem object
                switch (problem.Type) // type of nodes
                {
                    case ProblemType.TSP:
                        if(isGenetic == false)status = new TSPSymetric(problem, MaxTime).SolveTSP();
                        else status = new TSPGeneticSymetric().SolveTSP(problem, MaxTime, 60, null, false);
                        break;
                    case ProblemType.ATSP:
                        _matrixTable = new MatrixTable(filename);
                        if (isGenetic == false) status = new TSPAsymetric(MaxTime, _matrixTable).SolveTSP();
                        else status = new TSPGeneticSymetric().SolveTSP(null, MaxTime, 60, _matrixTable.Matrix, true);
                    break;
                    default:
                        MessageBox.Show("Loaded file is not tsp/atsp file. returning.");
                        return status;
                }
            //}
            //catch (Exception)
            //{
            //    MessageBox.Show("Can't load file " + filename);
            //}
            return status;
        }
    }
}