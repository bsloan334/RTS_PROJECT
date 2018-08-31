using System;
using System.Threading;

namespace RTS_Project
{
    struct TrainTrack
    {
        public char[,] planeX;
        public char[,] planeY;
        public char[,] planeZ;

        public TrainTrack(int row, int col)
        {
            planeX = new char[row, col];
            planeY = new char[row, col];
            planeZ = new char[row, col];
            ClearTrack(row, col);
        }

        public void ClearTrack(int row, int col)
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

        public void UpdateTrack(ref char[,] currentPositions)
        {
            UpdateX(ref currentPositions);
            UpdateY(ref currentPositions);
            UpdateZ(ref currentPositions);

            ClearTrack(8, 7);
            /*
            The currentpositions array has a value for every slot except the ones we modify in the Update funcitons. I think something is going wrong with the updates
             */

            // Console.WriteLine(currentPositions[0,0]); 
            // //Console.WriteLine(currentPositions[0,1]); 
            // //Console.WriteLine(currentPositions[0,2]); 
            // Console.WriteLine(currentPositions[1,0]); 
            // Console.WriteLine(currentPositions[1,1]); 
            // //Console.WriteLine(currentPositions[1,2]); 
            // Console.WriteLine(currentPositions[2,0]); 
            // //Console.WriteLine(currentPositions[2,1]); 
            // Console.WriteLine(currentPositions[2,2]); 
            planeX[currentPositions[0, 2], currentPositions[0, 1]] = 'X'; 
            planeY[currentPositions[1, 2], currentPositions[1, 1]] = 'Y';
            planeZ[currentPositions[2, 2], currentPositions[2, 1]] = 'Z'; 
        }

        //I updated these functions to correctly swap from char to int and back.
        //They were not converting to the correct type when I tested.
        public void UpdateX(ref char[,] currentPositions)
        {
            char xChar = currentPositions[0, 2];
            Console.WriteLine(xChar);
            int xInt = int.Parse(xChar.ToString());
            xInt = (xInt + 1) % 8;
            Console.WriteLine(xInt);
            /*
            Everything checks out fine up to this point. The next line causes the index out of bounds exception. I do not know why yet.
             */
            currentPositions[0, 2] = Convert.ToChar(xInt);
            Console.WriteLine(currentPositions[0,2]);

            char yChar = currentPositions[0, 1];
            int yInt = int.Parse(yChar.ToString());
            yInt = (yInt + 1) % 7;
            currentPositions[0, 1] = Convert.ToChar(yInt);
        }

        public void UpdateY(ref char[,] currentPositions)
        {
            char yChar = currentPositions[1, 2];
            int yInt = int.Parse(yChar.ToString());
            yInt = (yInt + 1) % 8;
            currentPositions[1, 2] = Convert.ToChar(yInt);
        }

        public void UpdateZ(ref char[,] currentPositions)
        {
            char xChar = currentPositions[2, 1];
            int xInt = int.Parse(xChar.ToString());
            xInt = (xInt + 1) % 7;
            currentPositions[2, 1] = Convert.ToChar(xInt);
        }
    }

    class Program
    {
        public static System.Object lockA = new System.Object();
        public static System.Object lockB = new System.Object();

        public static TrainTrack track = new TrainTrack(8, 7);

        /*

        This array is initialized properly. The reason it goes through without problems when it is not given initial values is because they are all blank without a value to begin with.

        */
        public static char[,] currentPositions = new char[3, 3] 
        { 
            {'X', '0', '0'},
            {'Y', '0', '2'},
            {'Z', '3', '6'} }
        ;

        static void Main(string[] args)
        {

            //This will only throw an index out of bounds error if you initialize currentpositions with values
            InitTrains();

            for(int i = 0; i < 10; i++) 
            {
                PrintAllTracks();
                track.UpdateTrack(ref currentPositions);
                Thread.Sleep(1000);
            }

            //sender.Start();
            //receiver.Start();
        }

        static void InitTrains()
        {
            track.planeX[0, 0] = 'X'; 
            track.planeY[0, 2] = 'Y'; 
            track.planeZ[3, 6] = 'Z'; 
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

        static void PrintAllTracks()
        {
            PrintTrack(track.planeX);
            PrintTrack(track.planeY);
            PrintTrack(track.planeZ);
        }
    }
}
