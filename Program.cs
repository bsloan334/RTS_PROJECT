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

    struct TrainTrack
    {
        private int m_Row;
        private int m_Col;
        public char[,] planeX;
        public char[,] planeY;
        public char[,] planeZ;

        public TrainTrack(int row, int col)
        {
            planeX = new char[row, col];
            planeY = new char[row, col];
            planeZ = new char[row, col];
            m_Row = row;
            m_Col = col;
            ClearTrack();
        }

        public void ClearTrack()
        {
            for (int i = 0; i < m_Row; i++)
            {
                for (int j = 0; j < m_Col; j++)
                {
                    planeX[i, j] = '.';
                    planeY[i, j] = '.';
                    planeZ[i, j] = '.';
                }
            }
        }
        public char IntToChar(int i)
        {
            return (char)(i + 48);
        }

        public int CharToInt(char c)
        {
            return c - '0';
        }

        public int IncrementRow(int row)
        {
            return (row + 1) % 8;
        }

        public int IncrementCol(int col)
        {
            return (col + 1) % 7;
        }

        public void UpdateTrack(ref char[,] currentPositions)
        {
            UpdateX(ref currentPositions);
            UpdateY(ref currentPositions);
            UpdateZ(ref currentPositions);

            ClearTrack();

            planeX[CharToInt(currentPositions[0, 1]), CharToInt(currentPositions[0, 2])] = 'X';
            planeY[CharToInt(currentPositions[1, 1]), CharToInt(currentPositions[1, 2])] = 'Y';
            planeZ[CharToInt(currentPositions[2, 1]), CharToInt(currentPositions[2, 2])] = 'Z';
        }

        public void UpdateX(ref char[,] currentPositions)
        {
            char rowChar = currentPositions[0, 1];
            int rowInt = IncrementRow(CharToInt(rowChar));
            currentPositions[0, 1] = IntToChar(rowInt);

            char colChar = currentPositions[0, 2];
            int colInt = IncrementCol(CharToInt(colChar));
            currentPositions[0, 2] = IntToChar(colInt);
        }

        public void UpdateY(ref char[,] currentPositions)
        {
            char rowChar = currentPositions[1, 1];
            int rowInt = IncrementRow(CharToInt(rowChar));
            currentPositions[1, 1] = IntToChar(rowInt);
        }

        public void UpdateZ(ref char[,] currentPositions)
        {
            char colChar = currentPositions[2, 2];
            int colInt = IncrementCol(CharToInt(colChar));
            currentPositions[2, 2] = IntToChar(colInt);
        }
    }
}
