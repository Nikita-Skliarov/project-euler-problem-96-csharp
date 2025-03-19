using System.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using sudoku.Models;

namespace sudoku.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        private readonly ILogger<HomeController> _logger;

        private readonly string _sudokuTextFileLocation;

        public HomeController(ILogger<HomeController> logger, IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
            _logger = logger;
            string SudokuTextFileLocation = Path.Combine(_webHostEnvironment.WebRootPath, "files", "Sudoku.txt"); // get text file from wwwroot/files/Sudoku.txt
            _sudokuTextFileLocation = SudokuTextFileLocation;
        }

        public IActionResult Index()
        {
            // Default grid 01 with first page visit
            SudokuTable sudokuTable = new SudokuTable(_sudokuTextFileLocation, 01);
            return View(sudokuTable);
        }

        [HttpPost]
        public IActionResult Index(int gridNumber)
        {
            SudokuTable sudokuTable = new SudokuTable(_sudokuTextFileLocation, gridNumber);
            return View(sudokuTable);
        }


        [HttpPost]
        public IActionResult SolveAll()
        {
            var solvedSudokus = new List<object>(); // Use object to store anonymous types
            int solvedCount = 0;
            int sumAnswer = 0;

            for (int i = 1; i <= 50; i++)
            {
                SudokuTable sudokuTable = new SudokuTable(_sudokuTextFileLocation, i);
                solvedSudokus.Add(new { GridNumber = i, Solved = sudokuTable.isSolved });

                if (sudokuTable.isSolved)
                {
                    solvedCount++;
                    sumAnswer += int.Parse(sudokuTable.Table[0].Value.ToString() + sudokuTable.Table[1].Value.ToString() + sudokuTable.Table[2].Value.ToString());
                }
            }

            ViewBag.SolvedSudokus = solvedSudokus;
            ViewBag.SolvedCount = solvedCount;
            ViewBag.sumAnswer = sumAnswer;

            return View("Index", new SudokuTable(_sudokuTextFileLocation, 1));
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
