using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;

namespace TSP_Tabu_Search
{
    class TSPAsymetric
    {
        // obiekt matrixTable posiada tablicę dla atsp, miało być jako interface noda, ale problemy w implementacji
        private MatrixTable _matrixTable;

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

        public TSPAsymetric(int maxTimeInSeconds, MatrixTable matrixTable)
        {
            _maxTimeInSeconds = maxTimeInSeconds;
            _matrixTable = matrixTable;
        }

        public void initData()
        {
            // liczba nodów
            numOfNodes = _matrixTable.Dimension;
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
                currentSolution[i] = i;
                bestSolution[i] = i;
            }
            // calculate cost of current solution
            currentSolutionCost = 0;
            for (int i = 0; i < numOfNodes - 1; i++)
            {
                currentSolutionCost += _matrixTable.Matrix[currentSolution[i], currentSolution[i + 1]];
            }
            // dodaję koszt ostatniego do pierwszego noda (koszt powrotu)
            currentSolutionCost += _matrixTable.Matrix[currentSolution[numOfNodes - 1], currentSolution[0]];
            bestSolutionCost = currentSolutionCost;
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
            int nodeBefore_i;
            int nodeAfter_i;
            int nodeBefore_j;
            int nodeAfter_j;
            int node_i;
            int node_j;
            for (int i = 0; i < numOfNodes - 1; i++)
            {
                for (int j = i + 2; j < numOfNodes; j++) // !!!!!! tylko dla symetrycznego, dla asymetrycznego j = 0 !!!!!!
                {
                    newDistance = currentSolutionCost;
                    // przypisuję nody i,j
                    node_i = i;
                    node_j = j;
                    // dla node i, i jest przed j
                    if (i == 0) nodeBefore_i = numOfNodes - 1;
                    else nodeBefore_i = i - 1;
                    nodeAfter_i = i + 1;
                    // dla noda j, j jest po i
                    if (j == numOfNodes - 1) nodeAfter_j = 0;
                    else nodeAfter_j = j + 1;
                    nodeBefore_j = j - 1;
                    if (i == 0 && numOfNodes - 1 == j) // jeśli wybrany pierwszy i ostatni wierzchołek rozpatruję osobno
                    {
                        // usuwam ścieżki
                        newDistance -= _matrixTable.Matrix[currentSolution[node_i], currentSolution[nodeAfter_j]];
                        newDistance -= _matrixTable.Matrix[currentSolution[nodeBefore_j], currentSolution[node_j]];
                        // dodaję
                        newDistance += _matrixTable.Matrix[currentSolution[node_j], currentSolution[nodeAfter_i]];
                        newDistance += _matrixTable.Matrix[currentSolution[nodeBefore_j], currentSolution[node_i]];
                    }
                    else
                    {
                        newDistance -= _matrixTable.Matrix[currentSolution[nodeBefore_i], currentSolution[node_i]];
                        if (nodeAfter_i != node_j) newDistance -= _matrixTable.Matrix[currentSolution[node_i], currentSolution[nodeAfter_i]];
                        if (nodeBefore_j != node_i) newDistance -= _matrixTable.Matrix[currentSolution[nodeBefore_j], currentSolution[node_j]];
                        newDistance -= _matrixTable.Matrix[currentSolution[node_j], currentSolution[nodeAfter_j]];
                        // --------------------
                        if (nodeBefore_j != node_i) newDistance += _matrixTable.Matrix[currentSolution[nodeBefore_j], currentSolution[node_i]];
                        newDistance += _matrixTable.Matrix[currentSolution[node_i], currentSolution[nodeAfter_j]];
                        newDistance += _matrixTable.Matrix[currentSolution[nodeBefore_i], currentSolution[node_j]];
                        if (nodeAfter_i != node_j) newDistance += _matrixTable.Matrix[currentSolution[node_j], currentSolution[nodeAfter_i]];
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
                for (int j = i + 2; j < numOfNodes; j++) // only for symetric
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
            tabuList[(int)iANDj.X, (int)iANDj.Y] = tabuNumber;
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
