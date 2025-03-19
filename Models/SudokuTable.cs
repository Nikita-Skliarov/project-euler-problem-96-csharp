namespace sudoku.Models
{
    public class SudokuTable
    {
        /// <summary>
        /// Property which being used for saving selected grid in select input (in view)
        /// </summary>
        public int CurrentGridNumber;

        /// <summary>
        /// One sudoku table, contains 81 item by default where each of it - Cell Model with int - value and int[] possible values [1;9]
        /// </summary>
        public List<Cell> Table { get; private set; } = new();

        /// <summary>
        /// List of each step of solved Sudoku, where each step different from the other one with 1 solved cell.
        /// Actually it's List of Table grids from step 0 - begin and last one - full solution
        /// </summary>
        public List<List<Cell>> SudokuSolvedSteps;

        /// <summary>
        /// Property - status of Table. False by default.
        /// </summary>
        public bool isSolved = false;

        /// <summary>
        /// Count of cells solved. Used to indicate that Sudoku Table is solved
        /// </summary>
        public int cellsSolved;

        /// <summary>
        /// True if brutforce was used to solve sudoku
        /// </summary>
        public bool brutforceUsed = false;

        public SudokuTable(string filePath, int gridNumber)
        {
            LoadSudokuGrid(filePath, gridNumber);
            CurrentGridNumber = gridNumber;
            SolveSudoku();
        }

        /// <summary>
        /// Function that gets selected grid from text file and assigns all its cells (81) to values from given grid
        /// </summary>
        /// <algo>
        /// 1. Read text file given with 50 grids.
        /// 2. Search for selected number and if it is found, read next 9 lines and assign each value to Table property.
        /// 3. Give exception if grid was not found
        /// </algo>
        private void LoadSudokuGrid(string filePath, int gridNumber)
        {
            string[] lines = File.ReadAllLines(filePath);
            string searchGrid = $"Grid {gridNumber:D2}";

            for (int i = 0; i < lines.Length; i++)
            {
                if (lines[i].Trim() == searchGrid)
                {
                    //int index = 0;
                    for (int j = 1; j <= 9; j++)
                    {
                        foreach (char c in lines[i + j])
                        {
                            int number = c - '0';
                            if (number != 0) cellsSolved++;
                            Table.Add(new Cell(number));
                        }
                    }
                    return;
                }
            }

            throw new Exception($"Grid {gridNumber} not found in file.");
        }

        /// <summary>
        /// Function that solves sudoku table given by List<Cell> grid, where each of item - number from 1 to 9 (0 - empty cell)
        /// Returns solved grid and saves each solved cell in property SudokuSolvedSteps
        /// </summary>
        /// <algo>
        ///    1. Start a loop with updated bool false which will only stop if they were no changes made after all algorithms.
        ///    2. Begin with Naked singles check
        ///         - Get empty cell, look for it's possible values.
        ///         - Eliminate all possible values because of existing cells in the same row/column/block.
        ///         - If cell has only one option left, write value in cell, change update bool and go next empty cell.
        ///    3. Hidden singles col.
        ///         - Loop for each column
        ///         - Inside first loop, loop for options from 1 to 9 and search for cell that can have only one value in current column.
        ///         - If count of possibilities is 1, write value to cell and go for next column.
        ///    4. Hidden singles row
        ///         - The same as previous, but for rows.
        ///    5. Hidden singles block.
        ///         - Loop for each block starting point (for example (0,3), (0,6), (0,9), (3,0) etc)
        ///         - Search in each block for cell that can contain only one value in block context.
        ///         - If there is such cell, write value.
        ///    6. If any of these methods didn't change updated to true and count of solved cells is not 81, go try brut force (backtracking)
        ///         - Search for cell, which has less options possible.
        ///         - Make a choice, make backup of table, worked cell and chosen value (save as choice[] property of Cell).
        ///         - Validate table. 
        ///         - Valid? Make new choice, add new step to backups and validate again.
        ///         - Not valid? Go back to previous backup, look for cell, choice made and make another choice. No options anymore? Go to previous again.
        ///         - With each go-back action, last action must be deleted, so that backups are always up to date.
        /// </algo>
        public void SolveSudoku()
        {
            bool updated;

            do
            {
                updated = false;

                // NAKED SINGLES CHECK
                for (int row = 0; row < 9; row++)
                {
                    for (int col = 0; col < 9; col++)
                    {
                        Cell currentCell = Table[(row * 9) + col];

                        if (currentCell.Value != 0) continue;


                        // Same row check
                        for (int checkCol = 0; checkCol < 9; checkCol++)
                        {
                            if (checkCol == col) continue;
                            Cell checkCell = Table[(row * 9) + checkCol];
                            if (checkCell.Value == 0) continue;
                            currentCell.PossibleValues = currentCell.PossibleValues.Except(new int[] { checkCell.Value }).ToArray();
                        }

                        // Same column check
                        for (int checkRow = 0; checkRow < 9; checkRow++)
                        {
                            if (checkRow == row) continue;
                            Cell checkCell = Table[(checkRow * 9) + col];
                            if (checkCell.Value == 0) continue;
                            currentCell.PossibleValues = currentCell.PossibleValues.Except(new int[] { checkCell.Value }).ToArray();
                        }

                        // Same block 3x3 check
                        int blockRow = (row / 3) * 3;
                        int blockCol = (col / 3) * 3; // Get start cords of current block

                        for (int currentBlockRow = blockRow; currentBlockRow < blockRow + 3; currentBlockRow++)
                        {
                            for (int currentBlockCol = blockCol; currentBlockCol < blockCol + 3; currentBlockCol++)
                            {
                                if (currentBlockRow == row && currentBlockCol == col) continue; // changed
                                Cell checkCell = Table[(currentBlockRow * 9) + currentBlockCol];
                                if (checkCell.Value == 0) continue;
                                currentCell.PossibleValues = currentCell.PossibleValues.Except(new int[] { checkCell.Value }).ToArray();
                            }
                        }

                        if (currentCell.PossibleValues.Length == 1)
                        {
                            WriteValue(currentCell, currentCell.PossibleValues[0], row, col);
                            updated = true; // reset loop
                        }

                    }
                }

                // HIDDEN SINGLES COL CHECK
                for (int col = 0; col < 9; col++)
                {
                    for (int num = 1; num <= 9; num++)
                    {
                        int count = 0;
                        int lastRow = -1;
                        int lastCol = col;

                        for (int row = 0; row < 9; row++)
                        {
                            Cell checkCell = Table[(row * 9) + col];

                            if (checkCell.Value == 0 && checkCell.PossibleValues.Contains(num))
                            {
                                count++;
                                lastRow = row;
                            }
                        }

                        // If 'num' is only possible in one cell of the column
                        if (count == 1)
                        {
                            WriteValue(Table[(lastRow * 9) + lastCol], num, lastRow, lastCol);
                            updated = true;
                        }
                    }
                }

                // HIDDEN SINGLES ROW CHECK
                for (int row = 0; row < 9; row++)
                {
                    for (int num = 1; num <= 9; num++)
                    {
                        int count = 0;
                        int lastRow = row;
                        int lastCol = -1;

                        for (int col = 0; col < 9; col++)
                        {
                            Cell checkCell = Table[(row * 9) + col];

                            if (checkCell.Value == 0 && checkCell.PossibleValues.Contains(num))
                            {
                                count++;
                                lastCol = col;
                            }
                        }

                        // If 'num' is only possible in one cell of the row
                        if (count == 1)
                        {
                            WriteValue(Table[(lastRow * 9) + lastCol], num, lastRow, lastCol);
                            updated = true;
                        }
                    }
                }

                // HIDDEN SINGLES BLOCK CHECK
                for (int blockRow = 0; blockRow < 9; blockRow += 3)
                {
                    for (int blockCol = 0; blockCol < 9; blockCol += 3)
                    {
                        for (int num = 1; num <= 9; num++)
                        {
                            int count = 0;
                            int lastRow = -1;
                            int lastCol = -1;

                            for (int row = blockRow; row < blockRow + 3; row++)
                            {
                                for (int col = blockCol; col < blockCol + 3; col++)
                                {
                                    Cell checkCell = Table[(row * 9) + col];

                                    if (checkCell.Value == 0 && checkCell.PossibleValues.Contains(num))
                                    {
                                        count++;
                                        lastRow = row;
                                        lastCol = col;
                                    }
                                }
                            }

                            // If 'num' can only be placed in one cell in the block
                            if (count == 1)
                            {
                                WriteValue(Table[(lastRow * 9) + lastCol], num, lastRow, lastCol);
                                updated = true;
                            }
                        }
                    }
                }

                

            } while (updated);

            if (cellsSolved == 81) return;

            brutforceUsed = true;
            BacktrackSolve();

        }


        /// <summary>
        /// Solves sudoku with backtracking method. 
        /// Last function used to solve sudoku. Being used if this is not possible to solve sudoku with any other ways.
        /// </summary>
        /// <algo>
        /// 1. Go to first found empty cell from (0,0)
        /// 2. Try to put a value from 1 to 9 and validate sudoku.
        ///     - If sudoku is still valid, call this function recursive to fill next cell out.
        ///     - If new cell violates sudoku rules, reassign it back to 0 and go to previous step.
        /// </algo>
        public bool BacktrackSolve()
        {
            for (int row = 0; row < 9; row++)
            {
                for (int col = 0; col < 9; col++)
                {
                    Cell currentCell = Table[(row * 9) + col];
                    if (currentCell.Value != 0) continue;

                    for (int num = 1; num <= 9; num++)
                    {
                        if (IsValid(row, col, num))
                        {
                            WriteValue(currentCell, num, row, col);
                            if (BacktrackSolve()) return true;
                            currentCell.Value = 0;
                            currentCell.PossibleValues = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
                        }
                    }
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        /// Function that checks if sudoku valid with new inserted cell (used for backtracking function)
        /// </summary>
        /// <algo>
        /// 1. Go for row and check if cell doesn't occurs more than one time.
        /// 2. Go for col.
        /// 3. Go for block.
        /// 4. If sudoku is still valid, return true. Otherwise false.
        /// </algo>
        public bool IsValid(int row, int col, int num)
        {
            // Row check
            for (int checkCol = 0; checkCol < 9; checkCol++)
            {
                if (Table[(row * 9) + checkCol].Value == num)
                {
                    return false;
                }
            }

            // Column check
            for (int checkRow = 0; checkRow < 9; checkRow++)
            {
                if (Table[(checkRow * 9) + col].Value == num)
                {
                    return false;
                }
            }

            // Block check
            int blockRow = (row / 3) * 3;
            int blockCol = (col / 3) * 3;
            for (int currentBlockRow = blockRow; currentBlockRow < blockRow + 3; currentBlockRow++)
            {
                for (int currentBlockCol = blockCol; currentBlockCol < blockCol + 3; currentBlockCol++)
                {
                    if (Table[(currentBlockRow * 9) + currentBlockCol].Value == num)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Function that inputs value in cell and uncheck all possibilities by row/column/block
        /// </summary>
        /// <algo>
        /// 1. Write new value for cel.
        /// 2. Go to its column/row/block and remove all same possible values.
        /// </algo>
        public void WriteValue(Cell cellToWrite, int value, int row, int col)
        {
            cellToWrite.Value = value;

            for (int checkCol = 0; checkCol < 9; checkCol++)
            {
                if (checkCol == col) continue;
                Cell checkCell = Table[(row * 9) + checkCol];
                if (checkCell.Value != 0) continue;
                checkCell.PossibleValues = checkCell.PossibleValues.Except(new int[] { cellToWrite.Value }).ToArray();
            }

            for (int checkRow = 0; checkRow < 9; checkRow++)
            {
                if (checkRow == row) continue;
                Cell checkCell = Table[(checkRow * 9) + col];
                if (checkCell.Value != 0) continue;
                checkCell.PossibleValues = checkCell.PossibleValues.Except(new int[] { cellToWrite.Value }).ToArray();
            }

            int blockRow = (row / 3) * 3;
            int blockCol = (col / 3) * 3; // Get start cords of current block

            for (int currentBlockRow = blockRow; currentBlockRow < blockRow + 3; currentBlockRow++)
            {
                for (int currentBlockCol = blockCol; currentBlockCol < blockCol + 3; currentBlockCol++)
                {
                    if (currentBlockRow == row && currentBlockCol == col) continue; // changed
                    Cell checkCell = Table[(currentBlockRow * 9) + currentBlockCol];
                    if (checkCell.Value != 0) continue;
                    checkCell.PossibleValues = checkCell.PossibleValues.Except(new int[] { cellToWrite.Value }).ToArray();
                }
            }

            cellsSolved++;
            if (cellsSolved == 81)
            {
                isSolved = true;
            }
        }
    }
}
