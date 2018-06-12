#region Using

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

#endregion // Using

//  http://en.wikipedia.org/wiki/Levenshtein_distance
//  Levenshtein distance is a metric for measuring the amount of difference between two sequences
//  The Levenshtein distance between two strings is defined as the minimum number of edits needed 
//  to transform one string into the other, with the allowable edit operations being insertion, deletion, or substitution of a single character

namespace LevenshtinSample
{
    class Program
    {
        private const int SPIN_MILLISECONDS = 1;
        private const string S1 = "Hello";
        private const string S2 = "World";

        #region Main

        static void Main(string[] args)
        {
            Measure(EditDistanceSequential, "Levenshtein distance", S1, S2, true);

            string s1 = "Levenshtein distance is a metric for measuring";
            string s2 = "The Levenshtein distance between two strings is defined as the minimum number of edits needed";
            
            Measure(EditDistanceSequential, "Sequential", s1, s2, false);
            Measure(EditDistanceParallel, "Parallel", s1, s2, false);
        }

        #endregion // Main

        #region Measure

        private static void Measure(Func<string, string, int[,]> what, string description, string s1, string s2, bool printMatrix)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            Thread.Sleep(2000); // for the profiler

            Stopwatch sw = Stopwatch.StartNew();
            int[,] result = what(s1, s2);
            Console.WriteLine("{0}: {1:N0}ms", description, sw.ElapsedMilliseconds);
            Console.WriteLine("Result = {0}", result[s1.Length, s2.Length]);
            if (printMatrix)
                WriteDistanceMatrix(s1, s2, result);
        }

        #endregion // Measure

        #region WriteDistanceMatrix

        static void WriteDistanceMatrix(string s1, string s2, int[,] distanceMatrix)
        {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write("  ");
            for (int i = 1; i <= s1.Length; i++) // rows
            {
                Console.Write(s1[i - 1]);
            }
            Console.WriteLine();
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write(" ");
            for (int i = 0; i <= s1.Length; i++) // rows
            {
                Console.Write(i);
            }

            int col = 0;
            for (int i = 1; i <= s1.Length; i++) // rows
            {
                for (int j = 1; j <= s2.Length; j++) // columns
                {
                    if (j == 1)
                    {
                        Console.WriteLine();
                        Console.ForegroundColor = ConsoleColor.Gray;
                        Console.Write(s2[col]);
                        col++;
                        Console.ForegroundColor = ConsoleColor.White;
                        Console.Write(col);
                    }
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    if (j == s2.Length && i == s1.Length)
                        Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write(distanceMatrix[i, j]);
                }
            }
            Console.WriteLine();
            Console.WriteLine();
            Console.ResetColor();
        }

        #endregion // WriteDistanceMatrix

        #region EditDistanceSequential

        static int[,] EditDistanceSequential(string s1, string s2)
        {
            var distanceMatrix = new int[s1.Length + 1, s2.Length + 1]; 

            for (int i = 0; i <= s1.Length; i++) distanceMatrix[i, 0] = i; // first row initialization
            for (int j = 0; j <= s2.Length; j++) distanceMatrix[0, j] = j; // first column initialization 

            for (int i = 1; i <= s1.Length; i++) // rows
            {
                for (int j = 1; j <= s2.Length; j++) // columns
                {
                    CalcDistance(s1, s2, distanceMatrix, i, j);
                }
            }
            return distanceMatrix;
        }

        #endregion // EditDistanceSequential

        #region EditDistanceParallel

        static int[,] EditDistanceParallel(string s1, string s2)
        {
            var distanceMatrix = new int[s1.Length + 1, s2.Length + 1]; 
            var taskMatrix = new Task[s1.Length, s2.Length]; 

            for (int i = 0; i <= s1.Length; i++) distanceMatrix[i, 0] = i; // first row initialization
            for (int j = 0; j <= s2.Length; j++) distanceMatrix[0, j] = j; // first column initialization 

            for (int i = 1; i <= s1.Length; i++) // rows
            {
                for (int j = 1; j <= s2.Length; j++) // columns
                {
                    int localI = i, localJ = j;

                    if (localI == 1 && j == 1)
                    {
                        taskMatrix[localI - 1, localJ - 1] = new Task(() =>
                            CalcDistance(s1, s2, distanceMatrix, localI, localJ));
                        taskMatrix[0, 0].RunSynchronously();
                    }
                    else
                    {
		                #region Task[] dependentTaskArray

                        var dependencies = new List<Task>();
                        if (localI > 1)
                            dependencies.Add(taskMatrix[localI - 2, localJ - 1]); // left task
                        if (localJ > 1)
                            dependencies.Add(taskMatrix[localI - 1, localJ - 2]); // up task
                        Task[] dependentTaskArray = dependencies.ToArray();

		                #endregion // Task[] dependentTaskArray

                        TaskContinuationOptions option = TaskContinuationOptions.ExecuteSynchronously;
                        if ((i * j) % 2 == 0)
                            option = TaskContinuationOptions.None;

                        Task t = Task.Factory.ContinueWhenAll(dependentTaskArray,
                            prevTasks => CalcDistance(s1, s2, distanceMatrix, localI, localJ), option);
                        taskMatrix[localI - 1, localJ - 1] = t;
                    }
                }
            }

            taskMatrix[s1.Length - 1, s2.Length - 1].Wait();
            return distanceMatrix;
        }

        #endregion // EditDistanceParallel

        #region CalcDistance

        private static void CalcDistance(string s1, string s2, int[,] distanceMatrix, int i, int j)
        {
            if (s1[i - 1] == s2[j - 1])
                distanceMatrix[i, j] = distanceMatrix[i - 1, j - 1];
            else
            {
                var left = distanceMatrix[i - 1, j];
                var up = distanceMatrix[i, j - 1];
                var leftUp = distanceMatrix[i - 1, j - 1];
                var min = Math.Min (left, up);
                min =  Math.Min (min, leftUp);

                distanceMatrix[i, j] = 1 + min;
            }

            SpinWork();
        }

        #endregion // CalcDistance

        #region SpinWork

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        public static void SpinWork()
        {
            if (SPIN_MILLISECONDS == 0)
                return;

            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < SPIN_MILLISECONDS) ; // spin for the duration
            sw.Stop();
        }

        #endregion // SpinWork
    }
}
