using System;
using System.Threading;

namespace RTS_PROJECT
{
    class Program
    {
        public static System.Object lockA = new System.Object();
        public static System.Object lockB = new System.Object();

        public static int ROW = 8;
        public static int COL = 7;
        public static int CYCLES = 20;
        public static int SLEEP = 1000;

        public static TrainTrack track = new TrainTrack(ROW, COL);
        public static char[,] currentPositions = new char[3, 3]
        {                                  
            {'X', '0', '0'},
            {'Y', '0', '2'},
            {'Z', '3', '6'}
        };

        static void Main(string[] args)
        {
            InitTrains();
            RunTrains();
        }

        static void InitTrains()
        {
            track.planeX[0, 0] = 'X';
            track.planeY[0, 2] = 'Y';
            track.planeZ[3, 6] = 'Z';
        }

        static void RunTrains()
        {
            for (int i = 0; i < CYCLES; i++)
            {
                PrintAllTracks();
                track.UpdateTrack(ref currentPositions);
                Thread.Sleep(SLEEP);
            }
        }

        static void PrintTrack(char[,] track)
        {
            for (int i = 0; i < ROW; i++)
            {
                for (int j = 0; j < COL; j++)
                {
                    Console.Write(track[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        static void PrintAllTracks()
        {
            PrintTrack(track.planeX);
            PrintTrack(track.planeY);
            PrintTrack(track.planeZ);
        }
    }
}
