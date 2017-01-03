using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulator
{
    class Knapsack : ISimulatedAnnealing, IGeneticAlgorithm
    {
        private int[] points; // All point have + 1 and are nagated if not taken
        private Dictionary<packageSize, int> sizes;
        private int maxSize;
        private Random genetaror = new Random();

        public Knapsack(int[] points, Dictionary<packageSize, int> sizes, int maxSize) // points are packages numbers
        {
            this.points = new int[points.Length];
            Array.Copy(points, this.points, points.Length);

            for (int i = 0; i < this.points.Length; i++)
            {
                this.points[i] += 1; // because 0 can't be -0
                this.points[i] = - this.points[i];
            }
            
            this.sizes = sizes;
            this.maxSize = maxSize;
        }

        public long GetCost(int[] solution)
        {
            if (GetTotalWeight(solution) > maxSize)
            {
                return 0;
            }

            long sum = 0; 
            for (int i = 0; i < solution.Length; i++)
            {
                if (solution[i] > 0)
                {
                    sum += GetSizeOfPackage(solution[i] - 1);
                }
            }

            return sum; 
        }

        public int[] GetInitialSolution()
        {
            int[] solution = new int[points.Length];
            if (points.Length == 1)
            {
                solution[0] = - points[0]; // Take it -> in constructor was set to negative value
            }
            else
            {
                for (int i = 0; i < points.Length; i++)
                {
                    solution[i] = (genetaror.Next(0, 2) == 0) ? points[i] : -points[i];
                }                
            }
            return solution;
        }

        public int[] Randomize(int[] solution)
        {
            bool holdingEverything = solution.All(sol => sol > 0);
            int[] returnedSolution = new int[solution.Length];
            Array.Copy(solution, returnedSolution, solution.Length);

            if (!holdingEverything)
            { // prevent infinite loop

                int i = genetaror.Next(0, returnedSolution.Length);

                // find one that isn't being taken
                while (returnedSolution[i] > 0)
                {
                    i = genetaror.Next(0, returnedSolution.Length);
                }

                returnedSolution[i] = - returnedSolution[i]; // take random item -> make it positive

                while(GetTotalWeight(returnedSolution) > maxSize)
                {
                    int idx = genetaror.Next(0, returnedSolution.Length);
                    if (returnedSolution[idx] > 0)
                        returnedSolution[idx] = - returnedSolution[idx];
                }
            }

            return returnedSolution;
        }

        public int[] ConvertToFinalSolution(int[] solution)
        {
            int counter = 0;
            for (int i = 0; i < solution.Length; i++)
            {
                if (solution[i] > 0)
                    counter++;
            }
            int[] final = new int[counter];

            // Selecting solution
            int p = 0;
            for (int i = 0; i < solution.Length; i++)
            {
                if (solution[i] > 0)
                {
                    final[p++] = solution[i] - 1;
                }
            }

            return final;
        }

        public int compareOldNewSolution(long newS, long oldS)
        {
            int ans = 0;
            if (newS > oldS)
                ans = 1;
            else if (newS < oldS)
                ans = -1;

            return ans;
        }

        private int GetTotalWeight(int[] solution)
        {
            int sum = 0;
            for (int i = 0; i < solution.Length; i++)
            {
                if (solution[i] > 0)
                {
                    sum += GetSizeOfPackage(solution[i] - 1); // because real packages are numerated from 0
                }
            }
            return sum;
        }

        private int GetSizeOfPackage(int i)
        {
            int size = 0;
            packageSize sizeEnum = PackagesList.packagesList[i].Size;
            size = sizes[sizeEnum];
            return size;
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

            b = genetaror.Next(elementsNmb);
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
            for (int i = 0; i < n; i++)
            {
                while (GetTotalWeight(a[i]) > maxSize)
                {
                    return true;
                }
            }
            return false;
        }

        public int GetInstanceSize()
        {
            return points.Length;
        }
    }
}
