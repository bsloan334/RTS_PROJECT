using System;
using System.Collections.Generic;
using System.Threading;

namespace RTS_PROJECT
{
    class Program
    {
        //Globals for the program
        public const int ROW = 8;
        public const int COL = 7;
        public const int CYCLES = 20;
        public const int SLEEP = 1000;
        public static int TIME = 0;

        //Variables for the Central Command Center (P3)
        public static Dictionary<int, bool> threadsRan = new Dictionary<int, bool>
        {
            {1, false},
            {2, false},
            {3, false}
        };
        public static Dictionary<char, bool> whoToStop = new Dictionary<char, bool>
        {
            {'X', false},
            {'Y', false},
            {'Z', false}
        };

        //TrainTrack is a class that encapsulates the grids
        public static TrainTrack bufferA = new TrainTrack(ROW, COL);
        public static TrainTrack bufferB = new TrainTrack(ROW, COL);

        //Positions is a class that encapsulates the coordinate arrays
        public static Positions bufferC = new Positions();
        public static Positions bufferD = new Positions();

        //the locks for the threads to access the buffers
        public static Object lockA = new object();
        public static Object lockB = new object();
        public static Object lockC = new object();
        public static Object lockD = new object();

        //Written by Zach and Bert
        static void Main(string[] args)
        {
            //Set the initial coordinates of the trains
            Positions startPositions = new Positions();
            startPositions.UpdatePosition('X', 0, 0);
            startPositions.UpdatePosition('Y', 0, 2);
            startPositions.UpdatePosition('Z', 3, 6);
            bufferA.SetTrack(startPositions);

            while(TIME != CYCLES)
            {
                //Start the threads
                ThreadPool.QueueUserWorkItem(P1);
                ThreadPool.QueueUserWorkItem(P2);
                ThreadPool.QueueUserWorkItem(P3);
                
                //So the program happens in 1 second intervals
                while(WaitForThreads() == false) {}
                Thread.Sleep(1000);
                lock(threadsRan)
                {
                    threadsRan[1] = false;
                    threadsRan[2] = false;
                    threadsRan[3] = false;
                }
                TIME++;
            }
            Console.ReadLine();
        }

        static public bool WillTrainStop(char train)
        {
            //x - 10%
            //y - 5%
            //z - 1%
            Random trainStopper = new Random();
            int value = trainStopper.Next(1, 101);
            switch (train)
            {
                case 'X':
                    if (value >= 1 && value <= 10)
                    {
                        return false;
                    }
                    break;
                case 'Y':
                    if (value >= 1 && value <= 5)
                    {
                        return false;
                    }
                    break;
                case 'Z':
                    if (value == 1)
                    {
                        return false;
                    }
                    break;
                default:
                    Console.Error.WriteLine("Not a valid train character!");
                    break;
            }
            return true;
        }


        //Written by Bert
        static public int IncrementRow(int row)
        {
            return (row + 1) % 8;
        }

        //Written by Bert
        static public int IncrementCol(int col)
        {
            return (col + 1) % 7;
        }

        //Written by Zach
        static public bool WaitForThreads()
        {
            lock(threadsRan)
            {
                foreach (var thread in threadsRan.Values)
                {
                    if (thread == false)
                        return false;
                }
                return true;
            }
        }

        //Written by Zach
        static void P1(object state)
        {
            //Only occurs for even seconds
            if (TIME % 2 == 0)
            {
                lock(lockA)
                {
                    //Get the current track from plane A
                    Dictionary<char, Plain> plains = bufferA.ReadTrack();

                    //Set the coordinates
                    Coordinate coordinateX = plains['X'].FindTrain('X');
                    Coordinate coordinateY = plains['Y'].FindTrain('Y');
                    Coordinate coordinateZ = plains['Z'].FindTrain('Z');

                    //Add the coordinates
                    Positions positions = new Positions();
                    positions.UpdatePosition('X', coordinateX.GetRow(), coordinateX.GetCol());
                    positions.UpdatePosition('Y', coordinateY.GetRow(), coordinateY.GetCol());
                    positions.UpdatePosition('Z', coordinateZ.GetRow(), coordinateZ.GetCol());

                    //Update buffer B with the new coordinates
                    lock (lockB)
                    {
                        bufferB.UpdateTrack(positions);
                    }
                }
            }

            //Only occurs for odd seconds
            else if (TIME % 2 == 1)
            {
                lock(lockB)
                {
                    Dictionary<char, Plain> plains = bufferB.ReadTrack();
                    Coordinate coordinateX = plains['X'].FindTrain('X');
                    Coordinate coordinateY = plains['Y'].FindTrain('Y');
                    Coordinate coordinateZ = plains['Z'].FindTrain('Z');
                    Positions positions = new Positions();
                    positions.UpdatePosition('X', coordinateX.GetRow(), coordinateX.GetCol());
                    positions.UpdatePosition('Y', coordinateY.GetRow(), coordinateY.GetCol());
                    positions.UpdatePosition('Z', coordinateZ.GetRow(), coordinateZ.GetCol());

                    lock (lockA)
                    {
                        bufferA.UpdateTrack(positions);
                    }
                }
            }
            lock(threadsRan)
            {
                threadsRan[1] = true;
            }
        }

        //Written by Bert
        static void P2(object state)
        {
            if (TIME % 2 == 0)
            { 
                lock (lockC)
                {
                    Dictionary<char, Plain> plains = bufferA.ReadTrack();
                    Coordinate coordinateX = plains['X'].FindTrain('X');
                    Coordinate coordinateY = plains['Y'].FindTrain('Y');
                    Coordinate coordinateZ = plains['Z'].FindTrain('Z');

                    if (whoToStop['X'] == false)
                        bufferC.UpdatePosition('X', coordinateX.GetRow(), coordinateX.GetCol());
                    if (whoToStop['Y'] == false)
                        bufferC.UpdatePosition('Y', coordinateY.GetRow(), coordinateY.GetCol());
                    if (whoToStop['Z'] == false)
                        bufferC.UpdatePosition('Z', coordinateZ.GetRow(), coordinateZ.GetCol());

                    whoToStop['X'] = false;
                    whoToStop['Y'] = false;
                    whoToStop['Z'] = false;

                }
            }
            else if (TIME % 2 == 1)
            {
                lock (lockD)
                {
                    Dictionary<char, Plain> plains = bufferB.ReadTrack();
                    Coordinate coordinateX = plains['X'].FindTrain('X');
                    Coordinate coordinateY = plains['Y'].FindTrain('Y');
                    Coordinate coordinateZ = plains['Z'].FindTrain('Z');

                    if (whoToStop['X'] == false)
                        bufferD.UpdatePosition('X', coordinateX.GetRow(), coordinateX.GetCol());
                    if (whoToStop['Y'] == false)
                        bufferD.UpdatePosition('Y', coordinateY.GetRow(), coordinateY.GetCol());
                    if (whoToStop['Z'] == false)
                        bufferD.UpdatePosition('Z', coordinateZ.GetRow(), coordinateZ.GetCol());

                    whoToStop['X'] = false;
                    whoToStop['Y'] = false;
                    whoToStop['Z'] = false;
                }
            }
            lock(threadsRan)
            {
                threadsRan[2] = true;
            }
        }

        static void P3(object state)
        {
            //We add 1 to the time output for P3 because TIME doesn't increment
            //until after all threads have ran in the current cycle

            //We began to have synchronization issues where P3 might run first 
            //before P2 is able to update bufferC so we created a lock to ensure
            //P3 will always run after P2
            while ((threadsRan[1] == false) && (threadsRan[2] == false)) { }
            if (TIME % 2 == 0)
            {
                lock(lockC)
                {
                    //Written by Bert
                    Dictionary<char, Coordinate> positions = bufferC.GetPositions();
                    Coordinate coordX = positions['X'];
                    Coordinate coordY = positions['Y'];
                    Coordinate coordZ = positions['Z'];

                    if (coordX.GetRow() == coordY.GetRow() && coordX.GetCol() == coordY.GetCol())
                    {
                        Console.WriteLine("There was a collsion between X and Y at (" + coordX.GetRow() + ", " + coordX.GetCol() + ") at time " + (TIME + 1));
                    }
                    else if (coordX.GetRow() == coordZ.GetRow() && coordX.GetCol() == coordZ.GetCol())
                    {
                        Console.WriteLine("There was a collsion between X and Z at (" + coordX.GetRow() + ", " + coordX.GetCol() + ") at time " + (TIME + 1));
                    }
                    else if (coordY.GetRow() == coordZ.GetRow() && coordY.GetCol() == coordZ.GetCol())
                    {
                        Console.WriteLine("There was a collsion between Y and Z at (" + coordZ.GetRow() + ", " + coordZ.GetCol() + ") at time " + (TIME + 1));
                    }
                    else
                    {
                        Console.WriteLine("There was no collision at time " + (TIME + 1));
                    }

                    //Written by Zach
                    lock(lockB)
                    {
                        //Get the positions of the train at TIME + 1
                        Dictionary<char, Plain> plains = bufferB.ReadTrack();
                        Coordinate coordinateX = plains['X'].FindTrain('X');
                        Coordinate coordinateY = plains['Y'].FindTrain('Y');
                        Coordinate coordinateZ = plains['Z'].FindTrain('Z');

                        if (coordinateX.Equals(coordinateY))
                        {
                            lock (lockA)
                            {
                                Dictionary<char, Plain> stoppedPlains = bufferA.ReadTrack();
                                Coordinate stoppedCoordinateX = stoppedPlains['X'].FindTrain('X');
                                Coordinate stoppedCoordinateY = stoppedPlains['Y'].FindTrain('Y');
                                Coordinate stoppedCoordinateZ = stoppedPlains['Z'].FindTrain('Z');
                                Positions stoppedPositions = new Positions();
                                stoppedPositions.UpdatePosition('X', coordinateX.GetRow(), coordinateX.GetCol());
                                stoppedPositions.UpdatePosition('Y', coordinateY.GetRow(), coordinateY.GetCol());
                                stoppedPositions.UpdatePosition('Z', coordinateZ.GetRow(), coordinateZ.GetCol());

                                //calculate if train will stop, if it doesn't stop, update
                                if(WillTrainStop('Y'))
                                {
                                    bufferB.UpdateTrack(stoppedPositions, 'Y');
                                    Console.WriteLine("Prevented a collsion between X and Y at " + coordinateX + " at time " + (TIME + 1) + "by stopping Y");
                                }
                                else if (WillTrainStop('X'))
                                {
                                    bufferB.UpdateTrack(stoppedPositions, 'X');
                                    Console.WriteLine("Prevented a collsion between X and Y at " + coordinateX + " at time " + (TIME + 1) + "by stopping X");
                                }
                                else
                                {
                                    Console.WriteLine("Unavoidable collision between X and Y at " + coordinateX + " at time" + (TIME + 1));
                                }
                            }
                        }
                        if (coordinateX.Equals(coordinateZ))
                        {
                            lock (lockA)
                            {
                                Dictionary<char, Plain> stoppedPlains = bufferA.ReadTrack();
                                Coordinate stoppedCoordinateX = stoppedPlains['X'].FindTrain('X');
                                Coordinate stoppedCoordinateY = stoppedPlains['Y'].FindTrain('Y');
                                Coordinate stoppedCoordinateZ = stoppedPlains['Z'].FindTrain('Z');
                                Positions stoppedPositions = new Positions();
                                stoppedPositions.UpdatePosition('X', coordinateX.GetRow(), coordinateX.GetCol());
                                stoppedPositions.UpdatePosition('Y', coordinateY.GetRow(), coordinateY.GetCol());
                                stoppedPositions.UpdatePosition('Z', coordinateZ.GetRow(), coordinateZ.GetCol());

                                //calculate if train will stop, if it doesn't stop, update
                                if (WillTrainStop('Z'))
                                {
                                    bufferB.UpdateTrack(stoppedPositions, 'Z');
                                    Console.WriteLine("Prevented a collsion between X and Z at " + coordinateX + " at time " + (TIME + 1) + "by stopping Z");
                                }
                                else if (WillTrainStop('X'))
                                {
                                    bufferB.UpdateTrack(stoppedPositions, 'X');
                                    Console.WriteLine("Prevented a collsion between X and Z at " + coordinateX + " at time " + (TIME + 1) + "by stopping X");
                                }
                                else
                                {
                                    Console.WriteLine("Unavoidable collision between X and Z at " + coordinateX + " at time" + (TIME + 1));
                                }
                            }
                        }
                        if (coordinateY.Equals(coordinateZ))
                        {
                            lock (lockA)
                            {
                                Dictionary<char, Plain> stoppedPlains = bufferA.ReadTrack();
                                Coordinate stoppedCoordinateX = stoppedPlains['X'].FindTrain('X');
                                Coordinate stoppedCoordinateY = stoppedPlains['Y'].FindTrain('Y');
                                Coordinate stoppedCoordinateZ = stoppedPlains['Z'].FindTrain('Z');
                                Positions stoppedPositions = new Positions();
                                stoppedPositions.UpdatePosition('X', coordinateX.GetRow(), coordinateX.GetCol());
                                stoppedPositions.UpdatePosition('Y', coordinateY.GetRow(), coordinateY.GetCol());
                                stoppedPositions.UpdatePosition('Z', coordinateZ.GetRow(), coordinateZ.GetCol());

                                //calculate if train will stop, if it doesn't stop, update
                                if (WillTrainStop('Y'))
                                {
                                    bufferB.UpdateTrack(stoppedPositions, 'Y');
                                    Console.WriteLine("Prevented a collsion between Y and Z at " + coordinateX + " at time " + (TIME + 1) + "by stopping Y");
                                }
                                else if (WillTrainStop('X'))
                                {
                                    bufferB.UpdateTrack(stoppedPositions, 'X');
                                    Console.WriteLine("Prevented a collsion between Y and Z at " + coordinateX + " at time " + (TIME + 1) + "by stopping X");
                                }
                                else
                                {
                                    Console.WriteLine("Unavoidable collision between Y and Z at " + coordinateX + " at time" + (TIME + 1));
                                }
                            }
                        }
                    }
                }
            }
            else if(TIME % 2 == 1)
            {
                lock(lockD)
                {
                    //Written by Bert
                    Dictionary<char, Coordinate> positions = bufferD.GetPositions();
                    Coordinate coordX = positions['X'];
                    Coordinate coordY = positions['Y'];
                    Coordinate coordZ = positions['Z'];

                    if (coordX.GetRow() == coordY.GetRow() && coordX.GetCol() == coordY.GetCol())
                    {
                        Console.WriteLine("There was a collsion between X and Y at (" + coordX.GetRow() + ", " + coordX.GetCol() + ") at time " + (TIME + 1));
                    }
                    else if (coordX.GetRow() == coordZ.GetRow() && coordX.GetCol() == coordZ.GetCol())
                    {
                        Console.WriteLine("There was a collsion between X and Z at (" + coordX.GetRow() + ", " + coordX.GetCol() + ") at time " + (TIME + 1));
                    }
                    else if (coordY.GetRow() == coordZ.GetRow() && coordY.GetCol() == coordZ.GetCol())
                    {
                        Console.WriteLine("There was a collsion between Y and Z at (" + coordZ.GetRow() + ", " + coordZ.GetCol() + ") at time " + (TIME + 1));
                    }
                    else
                    {
                        Console.WriteLine("There was no collision at time " + (TIME + 1));
                    }

                    //Written by Zach
                    lock(lockA)
                    {
                        //Get the positions of the train at TIME + 1
                        Dictionary<char, Plain> plains = bufferA.ReadTrack();
                        Coordinate coordinateX = plains['X'].FindTrain('X');
                        Coordinate coordinateY = plains['Y'].FindTrain('Y');
                        Coordinate coordinateZ = plains['Z'].FindTrain('Z');

                        if (coordinateX.Equals(coordinateY))
                        {
                            lock (lockB)
                            {
                                Dictionary<char, Plain> stoppedPlains = bufferA.ReadTrack();
                                Coordinate stoppedCoordinateX = stoppedPlains['X'].FindTrain('X');
                                Coordinate stoppedCoordinateY = stoppedPlains['Y'].FindTrain('Y');
                                Coordinate stoppedCoordinateZ = stoppedPlains['Z'].FindTrain('Z');
                                Positions stoppedPositions = new Positions();
                                stoppedPositions.UpdatePosition('X', coordinateX.GetRow(), coordinateX.GetCol());
                                stoppedPositions.UpdatePosition('Y', coordinateY.GetRow(), coordinateY.GetCol());
                                stoppedPositions.UpdatePosition('Z', coordinateZ.GetRow(), coordinateZ.GetCol());

                                //calculate if train will stop, if it doesn't stop, update
                                if (WillTrainStop('Y'))
                                {
                                    bufferA.UpdateTrack(stoppedPositions, 'Y');
                                    Console.WriteLine("Prevented a collsion between X and Y at " + coordinateX + " at time " + (TIME + 1) + "by stopping Y");
                                }
                                else if (WillTrainStop('X'))
                                {
                                    bufferA.UpdateTrack(stoppedPositions, 'X');
                                    Console.WriteLine("Prevented a collsion between X and Y at " + coordinateX + " at time " + (TIME + 1) + "by stopping X");
                                }
                                else
                                {
                                    Console.WriteLine("Unavoidable collision between X and Y at " + coordinateX + " at time" + (TIME + 1));
                                }
                            }
                        }
                        if (coordinateX.Equals(coordinateZ))
                        {
                            lock (lockB)
                            {
                                Dictionary<char, Plain> stoppedPlains = bufferA.ReadTrack();
                                Coordinate stoppedCoordinateX = stoppedPlains['X'].FindTrain('X');
                                Coordinate stoppedCoordinateY = stoppedPlains['Y'].FindTrain('Y');
                                Coordinate stoppedCoordinateZ = stoppedPlains['Z'].FindTrain('Z');
                                Positions stoppedPositions = new Positions();
                                stoppedPositions.UpdatePosition('X', coordinateX.GetRow(), coordinateX.GetCol());
                                stoppedPositions.UpdatePosition('Y', coordinateY.GetRow(), coordinateY.GetCol());
                                stoppedPositions.UpdatePosition('Z', coordinateZ.GetRow(), coordinateZ.GetCol());

                                //calculate if train will stop, if it doesn't stop, update
                                if (WillTrainStop('Z'))
                                {
                                    bufferA.UpdateTrack(stoppedPositions, 'Z');
                                    Console.WriteLine("Prevented a collsion between X and Z at " + coordinateX + " at time " + (TIME + 1) + "by stopping Z");
                                }
                                else if (WillTrainStop('X'))
                                {
                                    bufferA.UpdateTrack(stoppedPositions, 'X');
                                    Console.WriteLine("Prevented a collsion between X and Z at " + coordinateX + " at time " + (TIME + 1) + "by stopping X");
                                }
                                else
                                {
                                    Console.WriteLine("Unavoidable collision between X and Z at " + coordinateX + " at time" + (TIME + 1));
                                }
                            }
                        }
                        if (coordinateY.Equals(coordinateZ))
                        {
                            lock (lockB)
                            {
                                Dictionary<char, Plain> stoppedPlains = bufferA.ReadTrack();
                                Coordinate stoppedCoordinateX = stoppedPlains['X'].FindTrain('X');
                                Coordinate stoppedCoordinateY = stoppedPlains['Y'].FindTrain('Y');
                                Coordinate stoppedCoordinateZ = stoppedPlains['Z'].FindTrain('Z');
                                Positions stoppedPositions = new Positions();
                                stoppedPositions.UpdatePosition('X', coordinateX.GetRow(), coordinateX.GetCol());
                                stoppedPositions.UpdatePosition('Y', coordinateY.GetRow(), coordinateY.GetCol());
                                stoppedPositions.UpdatePosition('Z', coordinateZ.GetRow(), coordinateZ.GetCol());

                                //calculate if train will stop, if it doesn't stop, update
                                if (WillTrainStop('Y'))
                                {
                                    bufferA.UpdateTrack(stoppedPositions, 'Y');
                                    Console.WriteLine("Prevented a collsion between Y and Z at " + coordinateX + " at time " + (TIME + 1) + "by stopping Y");
                                }
                                else if (WillTrainStop('X'))
                                {
                                    bufferA.UpdateTrack(stoppedPositions, 'X');
                                    Console.WriteLine("Prevented a collsion between Y and Z at " + coordinateX + " at time " + (TIME + 1) + "by stopping X");
                                }
                                else
                                {
                                    Console.WriteLine("Unavoidable collision between Y and Z at " + coordinateX + " at time" + (TIME + 1));
                                }
                            }
                        }
                    }
                }
            }
            lock(threadsRan)
            {
                threadsRan[3] = true;
            }
        }
    }

    //Helper class that holds the row/column position of each train
    //Written by Zach
    public class Coordinate
    {
        private int mRow;
        private int mCol;

        public Coordinate()
        {
            mRow = 0;
            mCol = 0;
        }

        public override bool Equals(object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            Coordinate coordinate = (Coordinate)obj;
            return (mRow == coordinate.GetRow()) && (mCol == coordinate.GetCol());
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return "(" + mRow + ", " + mCol + ")";
        }

        public void UpdateCoordinate(int row, int col)
        {
            mRow = row;
            mCol = col;
        }

        public int GetRow()
        {
            return mRow;
        }
        public int GetCol()
        {
            return mCol;
        }
    }

    //Written by Zach
    public class Positions
    {
        private ReaderWriterLockSlim mLock = new ReaderWriterLockSlim();
        
        //Instead of a 3x3 character array, Positions is incapsulated by a dictionary
        //where the key is the letter of the train and the value is a coordinate pair
        private Dictionary<char, Coordinate> mPositions = new Dictionary<char, Coordinate>();

        public Positions()
        {
            mPositions.Add('X', new Coordinate());
            mPositions.Add('Y', new Coordinate());
            mPositions.Add('Z', new Coordinate());
        }

        public void UpdatePosition(char train, int row, int col)
        {
            mLock.EnterWriteLock();
            try
            {
                mPositions[train].UpdateCoordinate(row, col);
            }
            finally
            {
                mLock.ExitWriteLock();
            }
        }

        public Dictionary<char, Coordinate> GetPositions()
        {
            mLock.EnterReadLock();
            try
            {
                return mPositions;
            }
            finally
            {
                mLock.ExitReadLock();
            }
        }
    }
}
