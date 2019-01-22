using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HiddenMarkowModel
{
    public class HMM
    {
        private List<int[]> sequences;
        private double[,] e, a;
        private const int BAUM_WELCH_ITERATIONS = 100;

        public HMM(List<int[]> trainingSequences)
        {
            e = new double[,]
            {
                { 0.2, 0.4 },
                { 0.2, 0.1 },
                { 0.1, 0.1 },
                { 0.2, 0.2 },
                { 0.1, 0.1 },
                { 0.2, 0.1 }
            };
            a = new double[,]
            {
                { 0.4, 0.6 },
                { 0.6, 0.4 }
            };

            sequences = trainingSequences;
        }

        // f - prawd emisji sekwencji
        // e - prawd emisji symbolu
        // p - prawd tranzycji

        public void TrainBaumWelch()
        {
            double[,] A = new double[Program.STATES_COUNT, Program.STATES_COUNT];
            double[,] E = new double[Program.NUMBERS_COUNT, Program.STATES_COUNT];
            double[] P = new double[sequences.Count];

            for (int it = 0; it < BAUM_WELCH_ITERATIONS; it++)
            {
                var newA = new double[Program.STATES_COUNT, Program.STATES_COUNT];
                var newE = new double[Program.NUMBERS_COUNT, Program.STATES_COUNT];

                for (int j = 0; j < sequences.Count; j++)
                {
                    var seq = sequences[j];
                    var f = CountPrefix(seq);
                    var b = CountSufix(seq);
                    P[j] = f[seq.Length - 1, 0] + f[seq.Length - 1, 1];

                    for (int i = 0; i < seq.Length - 1; i++)
                    {
                        newA[0, 0] += (f[i, 0] * a[0, 0] * e[seq[i + 1], 0] * b[i + 1, 0]) / P[j];
                        newA[0, 1] += (f[i, 0] * a[0, 1] * e[seq[i + 1], 1] * b[i + 1, 1]) / P[j];
                        newA[1, 0] += (f[i, 1] * a[1, 0] * e[seq[i + 1], 0] * b[i + 1, 0]) / P[j];
                        newA[1, 1] += (f[i, 1] * a[1, 1] * e[seq[i + 1], 1] * b[i + 1, 1]) / P[j];
                        for (int x = 0; x < Program.NUMBERS_COUNT; x++)
                        {
                            if (seq[i] == x)
                            {
                                newE[x, 0] += (f[i, 0] * b[i, 0]) / P[j];
                                newE[x, 1] += (f[i, 1] * b[i, 1]) / P[j];
                            }
                        }
                    }
                }

                A = newA;
                E = newE;

                // oblicz nowe parametry modelu
                double totalE0 = 0.0d;
                double totalE1 = 0.0d;

                for (int i = 0; i < Program.NUMBERS_COUNT; i++)
                {
                    totalE0 += E[i, 0];
                    totalE1 += E[i, 1];
                }

                for (int k = 0; k < Program.STATES_COUNT; k++)
                {
                    a[k, 0] = A[k, 0] / (A[k, 0] + A[k, 1]);
                    a[k, 1] = A[k, 1] / (A[k, 0] + A[k, 1]);
                    for (int i = 0; i < Program.NUMBERS_COUNT; i++)
                    {
                        e[i, 0] = E[i, 0] / totalE0;
                        e[i, 1] = E[i, 1] / totalE1;
                    }
                }

                // warunek zakończenia algorytmu
                double logSum = 0.0d;
                for (int j=0; j<sequences.Count; j++)
                {
                    logSum += Math.Log10(P[j]);
                }
                if (logSum > -1e3)
                {
                    break;
                }
            }
        }


        public void PrettyPrintModel()
        {
            Console.WriteLine("Prawdopodobieństwa przejść:");
            for (int i = 0; i < a.GetLength(0); i++)
            {
                for (int j = 0; j < a.GetLength(1); j++)
                {
                    Console.WriteLine("{0}->{1}: {2}", i, j, a[i, j]);
                }
            }

            Console.WriteLine("Prawdopodobieństwa emisji:");
            Console.WriteLine("");
            Console.WriteLine("Uczciwa kostka:");
            for (int i = 0; i < e.GetLength(0); i++)
            {
                Console.WriteLine("{0}: {1}", i, e[i, 0]);
            }
            Console.WriteLine("");
            Console.WriteLine("Oszukana kostka:");
            for (int i = 0; i < e.GetLength(0); i++)
            {
                Console.WriteLine("{0}: {1}", i, e[i, 1]);
            }
        }

        private double[,] CountPrefix(int[] sequence)
        {
            var f = new double[sequence.Length, Program.STATES_COUNT];
            f[0, 0] = 1;
            f[0, 1] = 0;
            for (int k = 0; k < Program.STATES_COUNT; k++)
            {
                for (int i = 1; i < sequence.Length; i++)
                {
                    double fsum = f[i - 1, 0] * a[0, k] + f[i - 1, 1] * a[1, k];
                    f[i, k] = e[sequence[i], k] * fsum;
                }
            }

            return f;
        }

        private double[,] CountSufix(int[] sequence)
        {
            var b = new double[sequence.Length, Program.STATES_COUNT];
            b[sequence.Length - 1, 0] = 1;
            b[sequence.Length - 1, 1] = 1;
            for (int k = 0; k < Program.STATES_COUNT; k++)
            {
                for (int i = sequence.Length - 2; i >= 0; i--)
                {
                    double b0 = a[k, 0] * e[sequence[i + 1], 0] * b[i + 1, 0];
                    double b1 = a[k, 1] * e[sequence[i + 1], 1] * b[i + 1, 1];
                    b[i, k] += b0 + b1;
                }
            }

            return b;
        }
    }
}
