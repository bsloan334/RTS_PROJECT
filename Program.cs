using System;
using System.Thread;
using System.Threading;

namespace RTS_Project
{
    class Program
    {
        private System.Object lockThis = new System.Object();
        private int[] buffer = new int[10];

        static void Main(string[] args)
        {
            Thread sender = new Thread(Sender);
            Thread receiver = new Thread(Receiver);

            sender.start();
            receiver.start();
        }

        static void Sender()
        {
            lock(lockThis)
            {
                for(int i = 0; i < 5; i++)
                {
                    buffer[i] = i;
                    Thread.Sleep(1000);
                }
            }
        }

        static void Receiver()
        {
            lock(lockThis)
            {
                for(int = 0; i < 5; i++)
                {
                    Console.WriteLine(i + " ");
                    Thread.Sleep(1000);
                }
            }
        }
    }
}
