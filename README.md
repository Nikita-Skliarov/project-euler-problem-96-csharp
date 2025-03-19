# Sudoku Solver - ASP.NET Website

![image](https://github.com/user-attachments/assets/1be92668-4152-4c8a-8502-0e8edb1450be)


## Overview

This is a Sudoku Solver application developed using ASP.NET. It solves a 9x9 Sudoku puzzle using a combination of **backtracking** and **constraint propagation** techniques (e.g., naked singles and hidden singles). The website allows users to input Sudoku puzzles and get solutions.

## Features

- **Sudoku Solver**: Uses both constraint propagation and backtracking algorithms to solve Sudoku puzzles.
- **Web Interface**: Users can input a Sudoku puzzle and view the solution on a web page.
- **Validations**: The solver checks the validity of the puzzle and ensures the solution follows Sudoku rules.
- **Interactive UI**: Enter values for the Sudoku grid and click "Solve" to get the solution.

## Technologies Used

- **ASP.NET Core**: Backend framework for developing the web application.
- **C#**: Programming language used for the business logic and algorithm implementation.

## Setup

### Prerequisites

- .NET Core SDK (version 5.0 or above)
- Visual Studio or Visual Studio Code (optional but recommended for development)
- Browser for testing the web application

### Steps to Run Locally

1. **Clone the Repository**  
   Clone this repository to your local machine:
   ```bash
   git clone https://github.com/your-username/sudoku-solver.git
   cd sudoku-solver
