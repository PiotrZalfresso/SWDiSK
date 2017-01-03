using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulator
{
    public interface IGeneticAlgorithm : ICalculationAlgorithm
    {
        int[] Mutate(int[] solution);
        Tuple<int[], int[]> Crossover(int[] parent1, int[] parent2);
        bool Check(int[][] a, int n);  // return true if found duplicates
    }
}
