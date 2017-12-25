using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using TspLibNet;
using TspLibNet.Graph.Nodes;

namespace TSP_Tabu_Search
{
    class TSPSymetric
    {

        private readonly TravelingSalesmanProblem _problem;
        // ograniczenia
        private readonly int _maxTimeInSeconds;

        private int tabuNumber = 3;
        private int[,] tabuList;
        // tabele dla TSP
        private int[] currentSolution;
        private int[] bestSolution;
        // koszty dla tsp
        private int currentSolutionCost;
        private int bestSolutionCost;
        // ilość nodów
        private int numOfNodes;
        // tabela pomocnicza
        private int[,] swapNodes;

        public TSPSymetric(TravelingSalesmanProblem problem, int maxTimeInSeconds)
        {
            _problem = problem;
            _maxTimeInSeconds = maxTimeInSeconds;
        }

        public void initData()
        {
            // liczba nodów
            numOfNodes = _problem.NodeProvider.CountNodes();
            tabuNumber = (int)(numOfNodes * 0.2);
            // inicjuję tablicę na przejścia
            swapNodes = new int[numOfNodes, numOfNodes];
            // create and clear tabu list
            tabuList = new int[numOfNodes, numOfNodes];
            for (int i = 0; i < numOfNodes; i++)
            {
                for (int j = 0; j < numOfNodes; j++)
                {
                    tabuList[i, j] = 0;
                }
            }
            // create init solution
            currentSolution = new int[numOfNodes];
            bestSolution = new int[numOfNodes];
            for (int i = 0; i < numOfNodes; i++)
            {
                currentSolution[i] = i + 1;
                bestSolution[i] = i + 1;
            }
            // calculate cost of current solution
            currentSolutionCost = 0;
            for (int i = 0; i < numOfNodes - 1; i++)
            {
                currentSolutionCost += CalcDistance((Node2D)_problem.NodeProvider.GetNode(currentSolution[i]), (Node2D)_problem.NodeProvider.GetNode(currentSolution[i + 1]));
            }
            // dodaję koszt ostatniego do pierwszego noda (koszt powrotu)
            currentSolutionCost += CalcDistance((Node2D)_problem.NodeProvider.GetNode(currentSolution[numOfNodes - 1]), (Node2D)_problem.NodeProvider.GetNode(currentSolution[0]));
            bestSolutionCost = currentSolutionCost;
        }
        public int CalcDistance(Node2D x, Node2D y) // distance for euclidean (EUC2D)
        {
            var xd = x.X - y.X;
            var yd = x.Y - y.Y;
            var dij = (int)Math.Round(Math.Sqrt(xd * xd + yd * yd), MidpointRounding.AwayFromZero);
            return dij;
        }
        public string SolveTSP()
        {
            DateTime startTime = DateTime.Now; // zaczynam pomiar
            initData(); ; // to chyba też muszę doliczyć do czasu, jako że jest to pierwszy krok tabu
            while ((DateTime.Now - startTime).TotalSeconds < _maxTimeInSeconds) // kończę po określonym czasie
            {
                CalcSwapCost(); // liczę tabelkę kosztów zamian
                Point iANDj = FindBestSwap(); // znajduję najkorzystniejszą zmianę
                SwapBestSolution(iANDj); // swapuję current solution na najlepsze znalezione
            }
            return PrintBestSolution();
        }

        public void CalcSwapCost()
        {
            int newDistance;
            INode nodeBefore_i;
            INode nodeAfter_i;
            INode nodeBefore_j;
            INode nodeAfter_j;
            INode node_i;
            INode node_j;
            for (int i = 0; i < numOfNodes - 1; i++)
            {
                for (int j = i + 1; j < numOfNodes; j++) // !!!!!! tylko dla symetrycznego, dla asymetrycznego j = 0 !!!!!!
                {
                    newDistance = currentSolutionCost;
                    // przypisuję nody i,j
                    node_i = _problem.NodeProvider.GetNode(currentSolution[i]);
                    node_j = _problem.NodeProvider.GetNode(currentSolution[j]);
                    // dla node i, i jest przed j
                    if (i == 0) nodeBefore_i = _problem.NodeProvider.GetNode(currentSolution[numOfNodes - 1]);
                    else nodeBefore_i = _problem.NodeProvider.GetNode(currentSolution[i - 1]);
                    nodeAfter_i = _problem.NodeProvider.GetNode(currentSolution[i + 1]);
                    // dla noda j, j jest po i
                    if (j == numOfNodes - 1) nodeAfter_j = _problem.NodeProvider.GetNode(currentSolution[0]);
                    else nodeAfter_j = _problem.NodeProvider.GetNode(currentSolution[j + 1]);
                    nodeBefore_j = _problem.NodeProvider.GetNode(currentSolution[j - 1]);
                    // usuwam ścieżki
                    if (i == 0 && numOfNodes - 1 == j) // jeśli wybrany pierwszy i ostatni wierzchołek rozpatruję osobno
                    {
                        newDistance -= CalcDistance((Node2D)node_i, (Node2D)nodeAfter_j);
                        newDistance -= CalcDistance((Node2D)nodeBefore_j, (Node2D)node_j);
                        newDistance += CalcDistance((Node2D)node_j, (Node2D)nodeAfter_i);
                        newDistance += CalcDistance((Node2D)nodeBefore_j, (Node2D)node_i);
                    }
                    else
                    {
                        newDistance -= CalcDistance((Node2D)nodeBefore_i, (Node2D)node_i);
                        if (nodeAfter_i != node_j) newDistance -= CalcDistance((Node2D)node_i, (Node2D)nodeAfter_i);
                        if (nodeBefore_j != node_i) newDistance -= CalcDistance((Node2D)nodeBefore_j, (Node2D)node_j);
                        newDistance -= CalcDistance((Node2D)node_j, (Node2D)nodeAfter_j);
                        // dodaję ścieżki
                        if (nodeBefore_j != node_i) newDistance += CalcDistance((Node2D)nodeBefore_j, (Node2D)node_i);
                        //else newDistance += CalcDistance((Node2D)node_i, (Node2D)node_j);
                        newDistance += CalcDistance((Node2D)node_i, (Node2D)nodeAfter_j);
                        newDistance += CalcDistance((Node2D)nodeBefore_i, (Node2D)node_j);
                        if (nodeAfter_i != node_j) newDistance += CalcDistance((Node2D)node_j, (Node2D)nodeAfter_i);
                        //else newDistance += CalcDistance((Node2D)node_i, (Node2D)node_j);
                    }
                    swapNodes[i, j] = newDistance;
                }
            }
        }

        private Point FindBestSwap()
        {
            Point iANDj = new Point(0, 0);
            int newBestSoluionCost = 999999999;
            for (int i = 0; i < numOfNodes - 1; i++)
            {
                for (int j = i + 1; j < numOfNodes; j++) // only for symetric
                {
                    // jeśli koszt po swapie < bestSolutionCost & tabu nie zabrania (tabuList[i,j] == 0) to najlepszy koszt = koszt po swapie & nowe dane punktu
                    if (swapNodes[i, j] < newBestSoluionCost && tabuList[i, j] == 0)
                    {
                        newBestSoluionCost = swapNodes[i, j];
                        iANDj.X = i;
                        iANDj.Y = j;
                    }
                }
            }
            //Console.WriteLine(iANDj.X + " " + iANDj.Y);
            return iANDj;
        }

        private void SwapBestSolution(Point iANDj)
        {
            // redukcja tabu
            for (int i = 0; i < numOfNodes - 1; i++)
            {
                for (int j = i + 1; j < numOfNodes; j++)
                {
                    if (tabuList[i, j] > 0) tabuList[i, j]--;
                }
            }
            // nowy tabu
            tabuList[(int) iANDj.X, (int)iANDj.Y] = tabuNumber;
            // swap
            int backup = currentSolution[(int)iANDj.X];
            currentSolution[(int)iANDj.X] = currentSolution[(int)iANDj.Y];
            currentSolution[(int)iANDj.Y] = backup;
            // nowy koszt
            currentSolutionCost = swapNodes[(int)iANDj.X, (int)iANDj.Y];
            if (bestSolutionCost > currentSolutionCost)
            {
                bestSolution = (int[])currentSolution.Clone();
                bestSolutionCost = currentSolutionCost;
            }
        }

        public string PrintBestSolution()
        {
            string status = "";
            status = "Status:\n";
            status += "Time: " + _maxTimeInSeconds.ToString() + "s";
            status += "\nBest cost: " + bestSolutionCost.ToString();
            string road = "";
            for (int i = 0; i < numOfNodes - 1; i++)
            {
                road += bestSolution[i].ToString() + "->";
            }
            road += bestSolution[0];
            status += "\nRoad: " + road;
            return status;
        }
    }
}
