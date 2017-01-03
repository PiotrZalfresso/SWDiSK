using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulator
{
    public interface ICalculationAlgorithm
    {
        int[] GetInitialSolution();
        long GetCost(int[] solution);
        int[] ConvertToFinalSolution(int[] solution);
        int compareOldNewSolution(long newS, long oldS); // 1 jeśli newS lepsze, -1 gorsze i 0 takie same
        int GetInstanceSize();
    }
}
