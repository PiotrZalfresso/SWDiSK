using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulator
{
    public interface ISimulatedAnnealing : ICalculationAlgorithm
    {
        int[] Randomize(int[] solution); // zwaraca kopie oryginalnej tablicy - backup
    }
}
