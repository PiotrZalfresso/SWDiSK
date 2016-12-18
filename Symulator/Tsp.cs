using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulator
{
    class Tsp : ProblemInstance
    {
        class getNeighborsIComparer : IComparer
        {
            int IComparer.Compare(Object x, Object y)
            {
                Tuple<int, int> t1 = (Tuple<int, int>)x;
                Tuple<int, int> t2 = (Tuple<int, int>)y;
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
        private int pointsNumber;

        static public int[] getNeighbors(int number)
        {
            int[] neighbors = new int[number];
            Random gen = new Random();
            int first = gen.Next(PackagesList.numberOfPackages);

           
            Tuple<int, int>[] temp = new Tuple<int, int>[PackagesList.numberOfPackages - 1]; // Pierwszy element to numer miasta, drugi to odległość
            for (int i = 0; i < PackagesList.numberOfPackages; i++)
            {
                if (i < number)
                {
                    temp[i] = new Tuple<int, int>(i, Matrices.Distance[first, i]);
                }
                else if (i > number)
                {
                    temp[i-1] = new Tuple<int, int>(i, Matrices.Distance[first, i]);
                }
                
            }
            Array.Sort(temp, new getNeighborsIComparer());

            for (int i = 0; i < number; i++)
            {
                neighbors[i] = temp[i].Item1;
            }

            return neighbors;
        }

        public int[] GetInitialSolution()
        {
            throw new NotImplementedException();
        }

        public int GetCost(int[] solution)
        {
            throw new NotImplementedException();
        }

        public int[] Randomize(int[] solution)
        {
            int[] copy = new int[pointsNumber];
            Array.Copy(solution, copy, pointsNumber);

            int i = 0;
            int j = 0;
            while (i == j)
            {
                i = genRandom(pointsNumber + 1) - 1;
                j = genRandom(pointsNumber + 1) - 1;
            }
            int t = solution[i];
            solution[i] = solution[j];
            solution[j] = t;

            return copy;
        }

        public void SetFinalSolution(int[] solution)
        {
            throw new NotImplementedException();
        }

        protected int genRandom(int max)
        {
            return generator.Next(1, max);
        }
    }
}
