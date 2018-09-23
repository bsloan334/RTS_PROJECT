using System.Collections.Generic;
using System.Threading;
using System;

namespace RTS_PROJECT
{
    //Class created by Zach and Bert
    public class Plain
    {
        private int mNumCols, mNumRows;
        private char[,] mPlain;

        public Plain(int row, int col)
        {
            mNumRows = row;
            mNumCols = col;
            mPlain = new char[row, col];
        }

        //Written by Zach
        public void ClearPlain()
        {
            for (int i = 0; i < mNumRows; i++)
            {
                for (int j = 0; j < mNumCols; j++)
                {
                    mPlain[i, j] = '.';
                }
            }
        }

        //Written by Bert
        public void UpdatePlain(char trian, int row, int col)
        {
            mPlain[row, col] = trian;
        }

        //Written by Bert
        public void PrintPlain()
        {
            for(int i = 0; i < mNumRows; i++)
            {
                for(int j = 0; j < mNumCols; j++)
                {
                    Console.Write(mPlain[i, j]);
                }
                Console.WriteLine();
            }
            Console.WriteLine();
        }

        //Written by Zach
        public Coordinate FindTrain(char train)
        {
            Coordinate coordinate = new Coordinate();
            for (int i = 0; i < mNumRows; i++)
            {
                for (int j = 0; j < mNumCols; j++)
                {
                    if (mPlain[i, j] == train)
                        coordinate.UpdateCoordinate(i, j);
                }
            }
            return coordinate;
        }
    }

    //The TrainTrack class is used for our A and B buffers
    //Each track as a plain for each train to run on that is a 
    //dictionary that holds the train letter as the key and
    //the 8x7 plain in which it will move
    //Class created by Zach and Bert
    class TrainTrack
    {
        ReaderWriterLockSlim mLock = new ReaderWriterLockSlim();
        Dictionary<char, Plain> mPlains;

        public TrainTrack(int row, int col)
        {
            mPlains = new Dictionary<char, Plain>();
            mPlains.Add('X', new Plain(row, col));
            mPlains.Add('Y', new Plain(row, col));
            mPlains.Add('Z', new Plain(row, col));
            foreach(Plain plain in mPlains.Values)
            {
                plain.ClearPlain();
            }
        }

        //Written by Zach
        public void PrintTracks()
        {
            foreach (Plain plain in mPlains.Values)
                plain.PrintPlain();
        }

        //Written by Zach
        public void UpdatePlain(char train, int row, int col)
        {
            mPlains[train].ClearPlain();
            mPlains[train].UpdatePlain(train, row, col);
        }

        //Written by Bert
        public int IncrementRow(int row)
        {
            return (row + 1) % 8;
        }

        //Written by Bert
        public int IncrementCol(int col)
        {
            return (col + 1) % 7;
        }

        public void UpdateTrack(Positions positions, char stoppedTrain1, char stoppedTrain2)
        {
            mLock.EnterWriteLock();
            try
            {
                Dictionary<char, Coordinate> coordinates = positions.GetPositions();
                if (stoppedTrain1 != 'X' && stoppedTrain2 != 'X')
                    UpdateX(coordinates['X']);
                else if (stoppedTrain1 != 'Y' && stoppedTrain2 != 'Y')
                    UpdateY(coordinates['Y']);
                else if (stoppedTrain1 != 'Z' && stoppedTrain2 != 'Z')
                    UpdateZ(coordinates['Z']);
            }
            finally
            {
                mLock.ExitWriteLock();
            }
        }

        public void UpdateTrack(Positions positions, char stoppedTrain)
        {
            mLock.EnterWriteLock();
            try
            {
                Dictionary<char, Coordinate> coordinates = positions.GetPositions();
                if (stoppedTrain != 'X')
                    UpdateX(coordinates['X']);
                else if (stoppedTrain != 'Y')
                    UpdateY(coordinates['Y']);
                else if (stoppedTrain != 'Z')
                    UpdateZ(coordinates['Z']);
            }
            finally
            {
                mLock.ExitWriteLock();
            }
        }

        public void UpdateTrack(Positions positions)
        {
            mLock.EnterWriteLock();
            try
            {
                Dictionary<char, Coordinate> coordinates = positions.GetPositions();
                UpdateX(coordinates['X']);
                UpdateY(coordinates['Y']);
                UpdateZ(coordinates['Z']);
            }
            finally
            {
                mLock.ExitWriteLock();
            }
        }

        //Written by Zach
        public void SetTrack(Positions positions)
        {
            Dictionary<char, Coordinate> coordinates = positions.GetPositions();
            mLock.EnterWriteLock();
            try
            {
                UpdatePlain('X', coordinates['X'].GetRow(), coordinates['X'].GetCol());
                UpdatePlain('Y', coordinates['Y'].GetRow(), coordinates['Y'].GetCol());
                UpdatePlain('Z', coordinates['Z'].GetRow(), coordinates['Z'].GetCol());
            }
            finally
            {
                mLock.ExitWriteLock();
            }
        }

        //Written by Zach
        public Dictionary<char, Plain> ReadTrack()
        {
            mLock.EnterReadLock();
            try
            {
                return mPlains;
            }
            finally
            {
                mLock.ExitReadLock();
            }
        }

        //Written by Bert
        public void UpdateX(Coordinate coordinate)
        {
            int row = coordinate.GetRow();
            int col = coordinate.GetCol();

            row = IncrementRow(row);
            col = IncrementCol(col);
            UpdatePlain('X', row, col);
        }

        //Written by Bert
        public void UpdateY(Coordinate coordinate)
        {
            int row = coordinate.GetRow();
            int col = coordinate.GetCol();

            row = IncrementRow(row);
            UpdatePlain('Y', row, col);
        }

        //Written by Bert
        public void UpdateZ(Coordinate coordinate)
        {
            int row = coordinate.GetRow();
            int col = coordinate.GetCol();

            col = IncrementCol(col);
            UpdatePlain('Z', row, col);
        }
    }
}
