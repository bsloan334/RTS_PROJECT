namespace RTS_PROJECT
{
    class TrainTrack
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
