using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulator
{
    class Tsp : ISimulatedAnnealing, IGeneticAlgorithm
    {
        class getNeighborsIComparer : IComparer
        {
            int IComparer.Compare(Object x, Object y)
            {
                Tuple<int, long> t1 = (Tuple<int, long>)x;
                Tuple<int, long> t2 = (Tuple<int, long>)y;
                if (t1.Item2 < t2.Item2)               // Item2 zawiera odległości między miastami
                {
                    return -1;
                }
                else if (t1.Item2 > t2.Item2)
                {
                    return 1;
                }
                else
                    return 0;
            }
        }

        private Random generator = new Random();
        private int[] points;
        private readonly long[,] costs;
        private int pointsNumber;
        private bool fromHub;
        Dictionary<packageSize, long> timeMap = null;

        static public int[] getNeighbors(int number, long[,] costs)
        {
            int[] neighbors = new int[number];
            Random gen = new Random();
            int first = gen.Next(PackagesList.numberOfPackages);
            while (Graph.deliveredItems.Contains(new DeliveryItem(PackagesList.packagesList[first])))
            {
                first = gen.Next(PackagesList.numberOfPackages);
            }
            
            Tuple<int, long>[] temp = new Tuple<int, long>[PackagesList.numberOfPackages - 1]; // Pierwszy element to numer miasta, drugi to odległość
            for (int i = 0; i < PackagesList.numberOfPackages; i++)
            {
                if (i < first)
                {
                    temp[i] = new Tuple<int, long>(i, costs[first, i]);
                }
                else if (i > first)
                {
                    temp[i-1] = new Tuple<int, long>(i, costs[first, i]);
                }
                
            }
            Array.Sort(temp, new getNeighborsIComparer());

            neighbors[0] = first;
            int x = 1;
            int z = 0;
            while (x < number && z < PackagesList.numberOfPackages - 1)
            {
                if (!Graph.deliveredItems.Contains(new DeliveryItem(PackagesList.packagesList[temp[z].Item1])))
                {
                    neighbors[x++] = temp[z].Item1;
                }
                z++;
            }

            return neighbors;
        }

        public Tsp(int[] points, long [,] costs, Dictionary<packageSize, long> timeMap = null, bool fromHub = false)
        {
            this.points = new int[points.Length];
            Array.Copy(points, this.points, points.Length);
            this.costs = costs;

            this.pointsNumber = this.points.Length;
            this.timeMap = timeMap;
            this.fromHub = fromHub;
        }

        public int[] GetInitialSolution()
        {
            int[] solution = Enumerable.Repeat(-1, pointsNumber).ToArray(); // Inicjalizacja za pomocą -1 
            int i = 0;
            while (i < pointsNumber)
            {
                int n = genRandom(pointsNumber); //losuje liczby
                if (!solution.Contains(points[n])) //sprawdza czy już jest, nie ma wpisujemy jest od nowa
                {
                    solution[i] = points[n];
                    i++;
                }
            }
            return solution;
        }

        public long GetCost(int[] solution)
        {
            long cost = 0;
            if (fromHub)
            {
                cost = costs[PackagesList.numberOfPackages, solution[0]];
                if (timeMap != null)
                {
                    long time = timeMap[PackagesList.packagesList[solution[0]].Size];
                    cost += time;
                }
            }

            for (int i = 0; i < solution.Length - 1; i++)
            {
                cost += costs[solution[i], solution[i + 1]];
                if (timeMap != null)
                {
                    long time = timeMap[PackagesList.packagesList[solution[i + 1]].Size];
                    cost += time;
                }
            }

            if (fromHub)
            {
                cost += costs[solution.Length - 1, PackagesList.numberOfPackages];
            }
            else
            {
                cost += costs[solution.Length - 1, solution[0]];
            }

            return cost;
        }

        public int[] Randomize(int[] solution)
        {
            int[] copy = new int[pointsNumber];
            Array.Copy(solution, copy, pointsNumber);
            
            if (pointsNumber > 1) {
                int i = 0;
                int j = 0;
                while (i == j)
                {
                    i = genRandom(pointsNumber) ;
                    j = genRandom(pointsNumber) ;
                }
                int t = copy[i];
                copy[i] = copy[j];
                copy[j] = t;
            }

            return copy;
        }

        public int[] ConvertToFinalSolution(int[] solution)
        {
            int[] final = new int[solution.Length];
            Array.Copy(solution, final, solution.Length);

            return final;
        }

        protected int genRandom(int max)
        {
            return generator.Next(max);
        }

        public int compareOldNewSolution(long newS, long oldS)
        {
            int ans = 0;
            if (newS < oldS)
                ans = 1;
            else if (newS > oldS)
                ans = -1;

            return ans;
        }

        public int[] Mutate(int[] solution)
        {
            return Randomize(solution); 
        }

        public Tuple<int[], int[]> Crossover(int[] parent1, int[] parent2)
        {
            if (parent1.Length != parent2.Length)
            {
                throw new ArgumentException("Parents have different sizes");
            }

            int a, b, tmp;
            int elementsNmb = parent1.Length;
            int[] par1 = new int[elementsNmb];
            Array.Copy(parent1, par1, elementsNmb);
            int[] par2 = new int[elementsNmb];
            Array.Copy(parent2, par2, elementsNmb);

            b = genRandom(elementsNmb);
            do
            {
                a = b;
                tmp = par2[a];
                par2[a] = par1[a];
                par1[a] = tmp;
                b = -1;
                for (int i = 0; i < elementsNmb; i++)
                    if (par1[i] == tmp && i != a)
                        b = i;
            } while (b >= 0);

            return new Tuple<int[], int[]>(par1, par2);
        }

        public bool Check(int[][] a, int n)
        {
            bool b;
            for (int i = 0; i < n; i++)
            {
                b = true;
                for (int j = 0; j < a[0].Length; j++)
                    if (a[i][j] != a[n][j])
                    {
                        b = false;
                        break;
                    }
                if (b)
                    return true;
            }
            return false;
        }

        public int GetInstanceSize()
        {
            return points.Length;
        }
    }
}
