using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenMarkowModel
{
    class Program
    {
        static void Main(string[] args)
        {
            //var h = new HiddenMarkowModel();
            Simulate();
        }

        static void Simulate()
        {
            bool isFair = true;
            double[] unfairProbabilities = new double[] { 0.1, 0.1, 0.1, 0.1, 0.1, 0.5 };
            double swapToFairProb = 0.1;
            double swapToUnfairProb = 0.05;
            const int ROLLS_NO = 1000;
            Random rnd = new Random();

            for (int i = 0; i < ROLLS_NO; i++)
            {
                int result;
                if (isFair)
                {
                    result = RollFairDice(rnd);
                    if (CheckDiceSwap(rnd, swapToUnfairProb))
                    {
                        isFair = false;
                        Console.WriteLine("Zaczynam kantować!");
                    }
                }
                else
                {
                    result = RollUnfairDice(rnd, unfairProbabilities);
                    if (CheckDiceSwap(rnd, swapToFairProb))
                    {
                        isFair = true;
                        Console.WriteLine("Już będę uczciwy!");
                    }
                }
                Console.Write(result);
            }
        }

        static int RollFairDice(Random rnd)
        {
            return rnd.Next(1, 7);
        }

        static int RollUnfairDice(Random rnd, double[] probabilities)
        {
            double randomNumber = rnd.NextDouble();
            double sum = 0.0;
            for (int i = 0; i < probabilities.Length; i++)
            {
                sum += probabilities[i];
                if (sum > randomNumber)
                {
                    return i + 1;
                }
            }
            return -1;
        }

        static bool CheckDiceSwap(Random rnd, double prob)
        {
            return rnd.NextDouble() < prob;
        }

        static void BaumWelch(double[] probFair, double[] probUnfair, double[] probTransition)
        {
            //double 
            //double a0 = 
        }


    }
}
