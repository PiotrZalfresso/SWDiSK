using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulator
{
    class Knapsack : ProblemInstance
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
            for (int i = 0; i < points.Length; i++)
            {
                solution[i] = (genetaror.Next(0,2) == 0) ? points[i] : - points[i];
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

                int i = genetaror.Next(0, solution.Length);

                // find one that isn't being taken
                while (solution[i] > 0)
                {
                    i = genetaror.Next(0, solution.Length);
                }

                solution[i] = - solution[i]; // take random item -> make it positive

                while(GetTotalWeight(solution) > maxSize)
                {
                    int idx = genetaror.Next(0, solution.Length);
                    if (solution[idx] > 0)
                        solution[idx] = - solution[idx];
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
    }
}
