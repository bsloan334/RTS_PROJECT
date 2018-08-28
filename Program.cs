using System;
using System.Threading;

namespace RTS_Project
{
    struct TrainTrack
    {
        public char[,] planeX;
        public char[,] planeY;
        public char[,] planeZ;

        public TrainTrack(ushort row, ushort col)
        {
            planeX = new char[row, col];
            planeY = new char[row, col];
            planeZ = new char[row, col];
            ClearTrack(row, col);
        }

        public void ClearTrack(ushort row, ushort col)
        {
            for (int i = 0; i < row; i++)
            {
                for (int j = 0; j < col; j++)
                {
                    planeX[i, j] = '.';
                    planeY[i, j] = '.';
                    planeZ[i, j] = '.';
                }
            }
        }

        public void UpdateTrack()
        {
            UpdateX();
            UpdateY();
            UpdateZ();
        }

        public void UpdateX()
        {

        }

        public void UpdateY()
        {

        }

        public void UpdateZ()
        {

        }
    }

    class Program
    {
        public static System.Object lockA = new System.Object();
        public static System.Object lockB = new System.Object();

        public static int[] bufferA = new int[10];
        public static int[] bufferB = new int[10];

        public static int iA = 1;
        public static int iB = 1;

        static void Main(string[] args)
        {
            Thread sender = new Thread(Producer);
            Thread receiver = new Thread(Consumer);

            TrainTrack track = new TrainTrack(8, 7);
            RunTrains(track);

            //sender.Start();
            //receiver.Start();
        }

        static void RunTrains(TrainTrack buffer)
        {
            buffer.planeX[0, 0] = 'X';
            buffer.planeY[0, 2] = 'Y';
            buffer.planeZ[3, 6] = 'Z';

            PrintTrack(buffer.planeX);
            PrintTrack(buffer.planeY);
            PrintTrack(buffer.planeZ);
        }

        static void PrintTrack(char[,] track)
        {
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 7; j++)
                {
                    Console.Write(track[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        static void Producer()
        {
            while (iA <= 50 && iB <= 50)
            {
                lock (lockA)
                {
                    if ((iA % 10) == 0)
                    {
                        bufferA[9] = iA;
                        Console.WriteLine("write to Buffer A " + bufferA[9]);
                    }
                    else
                    {
                        bufferA[(iA % 10) - 1] = iA;
                        Console.WriteLine("write to Buffer A " + bufferA[(iA % 10) - 1]);
                    }
                    iA++;
                    Thread.Sleep(1000);
                }
                lock (lockB)
                {
                    if ((iB % 10) == 0)
                    {
                        bufferB[9] = iB;
                        Console.WriteLine("write to Buffer B " + bufferB[9]);
                    }
                    else
                    {
                        bufferB[(iB % 10) - 1] = iB;
                        Console.WriteLine("write to Buffer B " + bufferB[(iB % 10) - 1]);
                    }
                    iB++;
                    Thread.Sleep(1000);
                }
            }
        }

        static void Consumer()
        {
            while(iA <= 50 && iB <= 50)
            {
                lock (lockA)
                {
                    if (IsBufferFull(bufferA) == true)
                    {
                        Console.Write("Buffer A: ");
                        foreach (int i in bufferA)
                        {
                            Console.Write(bufferA[i] + " ");
                            Thread.Sleep(1000);
                        }
                        Console.WriteLine();
                        ClearBuffer(bufferA);
                    }

                    lock (lockB)
                    {
                        if (IsBufferFull(bufferB) == true)
                        {
                            Console.Write("Buffer B: ");
                            foreach (int i in bufferB)
                            {
                                Console.Write(bufferB[i] + " ");
                                Thread.Sleep(1000);
                            }
                            Console.WriteLine();
                            ClearBuffer(bufferB);
                        }
                    }
                }
            }
        }

        static bool IsBufferFull(int[] buffer)
        {
            foreach (int i in buffer)
            {
                if (buffer[i] == 0)
                    return false;
            }
            return true;
        }

        static void ClearBuffer(int[] buffer)
        {
            foreach (int i in buffer) 
            {
                buffer[i] = 0;
            }
        }
    }
}
