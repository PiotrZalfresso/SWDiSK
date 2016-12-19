using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Symulator
{
    public class SimulatedAnnealing
    {
        double temperature;
        double lambda;
        int repetitions;
        private Random generator;

        public SimulatedAnnealing(double temperature, double lambda, int repetitions)
        {
            this.temperature = temperature;
            this.lambda = lambda;
            this.repetitions = repetitions;
            this.generator = new Random();
        }

        public double Temperature
        {
            get
            {
                return temperature;
            }
            set
            {
                temperature = value;
            }
        }

        public double Lambda
        {
            get
            {
                return lambda;
            }
            set
            {
                lambda = value;
            }
        }

        public int Repetitions
        {
            get
            {
                return repetitions;
            }
            set
            {
                repetitions = value;
            }
        }

        public int[] Calculate(ProblemInstance instance)
        {
            int[] solution = instance.GetInitialSolution();
            int count = solution.Length;
            int[] newSolution = new int[count]; //pi
            Array.Copy(solution, newSolution, count);
            int[] lastSolution;

            for (int r = 0; r < repetitions; r++)
            {
                
                int before = instance.GetCost(newSolution);
                lastSolution = instance.Randomize(newSolution);
                int after = instance.GetCost(newSolution);
                int actual = instance.GetCost(solution);
                if (instance.compareOldNewSolution(after, before) == 1)
                {
                    if (instance.compareOldNewSolution(after, actual) >= 0)
                    {
                        Array.Copy(newSolution, solution, count);
                    }
                }
                else if (instance.compareOldNewSolution(after, before) == -1)
                {
                    double p = genRandom(10000 + 1) - 1;
                    p /= 10000;
                    double e = -(after - before) / temperature;
                    double a = Math.Pow(Math.E, e);
                    if (p <= a)
                    {
                        Array.Copy(newSolution, solution, count);
                    }
                    else
                    {
                        Array.Copy(lastSolution, newSolution, count);
                    }
                    temperature = temperature * lambda;
                }
                else
                {
                    Array.Copy(lastSolution, newSolution, count);
                }
            }
            return instance.ConvertToFinalSolution(solution);
        }

        protected int genRandom(int max)
        {
            return generator.Next(1, max);
        }
    }
}
