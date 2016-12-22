using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulator
{
    public interface ProblemInstance
    {
        int[] GetInitialSolution();
        long GetCost(int[] solution);
        int[] Randomize(int[] solution); // zwaraca kopie oryginalnej tablicy - backup
        int[] ConvertToFinalSolution(int[] solution);
        int compareOldNewSolution(long newS, long oldS); // 1 jeśli newS lepsze, -1 gorsze i 0 takie same
    }
}
