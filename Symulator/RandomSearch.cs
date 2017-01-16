using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulator
{
    class RandomSearch
    {
        public int[] Calculate(IRandomSearch instance)
        {
            int[] solution = instance.GetInitialSolution();
            return instance.ConvertToFinalSolution(solution);
        }
    }
}
