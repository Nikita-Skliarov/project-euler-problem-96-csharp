namespace sudoku.Models
{
    public class Cell
    {
        /// <summary>
        /// Value of cell. Can be 0-9
        /// </summary>
        public int Value;

        /// <summary>
        /// Array which stays for possible options for cell left.
        /// </summary>
        public int[]? PossibleValues;

        /// <summary>
        /// Used for backtracking if it's necessary. Used to track options which already were used for backtracking, if it fails in one of the moments.
        /// </summary>
        public int[]? ChoicesMade;

        /// <summary>
        /// Instances new Cell
        /// </summary>
        /// <algo>
        /// If cell's value if 0, that means that cell is empty a need to have possible values defines. Otherwise there's no need in this.
        /// </algo>
        public Cell(int value)
        {
            if (value == 0)
            {
                PossibleValues = [1, 2, 3, 4, 5, 6, 7, 8, 9];
            }
            Value = value;
        }
    }
}
