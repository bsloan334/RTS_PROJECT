using System;
using System.Threading;

namespace RTS_Project
{
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
            Thread sender = new Thread(Sender);
            Thread receiver = new Thread(Receiver);

            sender.Start();
            receiver.Start();
        }

        static void Sender()
        {
            while (iA <= 50 && iB <= 50)
            {
                lock (lockA)
                {
                    if ((iA % 10) == 0)
                    {
                        bufferA[9] = iA;
                        Console.WriteLine("write to bufferA " + bufferA[9]);
                    }
                    else
                    {
                        bufferA[(iA % 10) - 1] = iA;
                        Console.WriteLine("write to bufferA " + bufferA[(iA % 10) - 1]);
                    }
                    iA++;
                    Thread.Sleep(1000);
                }
                lock (lockB)
                {
                    if ((iB % 10) == 0)
                    {
                        bufferB[9] = iB;
                        Console.WriteLine("write to bufferB " + bufferB[9]);
                    }
                    else
                    {
                        bufferB[(iB % 10) - 1] = iB;
                        Console.WriteLine("write to bufferB " + bufferB[(iB % 10) - 1]);
                    }
                    iB++;
                    Thread.Sleep(1000);
                }
            }
        }

        static void Receiver()
        {
            while(iA <= 50 && iB <= 50)
            {
                lock (lockA)
                {
                    if (IsBufferFull(bufferA) == true)
                    {
                        Console.Write("Buffer A: ");
                        for (int i = 0; i < 10; i++)
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
                            for (int i = 0; i < 10; i++)
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
            for (int i = 0; i < 10; i++)
            {
                if (buffer[i] == 0)
                    return false;
            }
            return true;
        }

        static void ClearBuffer(int[] buffer)
        {
            for (int i = 0; i < 10; i++)
            {
                buffer[i] = 0;
            }
        }
    }
}
