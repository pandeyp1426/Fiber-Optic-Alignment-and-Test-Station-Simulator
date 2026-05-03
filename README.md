# Fiber Optic Alignment and Test Station Simulator

A small .NET 8 WinForms student project that simulates a fiber optic alignment and test station used in laser manufacturing automation.

## Features

- Generates a simulated fiber optic or laser module serial number
- Simulates X/Y motion alignment with a peak-search routine
- Calculates optical power based on distance from the alignment center
- Simulates temperature readings
- Validates alignment, optical power, and temperature against station tolerances
- Saves completed test runs to SQLite using `Microsoft.Data.Sqlite`
- Displays saved test history in a WinForms `DataGridView`

## How To Run

1. Open `FiberOpticAlignmentSimulator.slnx` in Visual Studio.
2. Restore NuGet packages if prompted.
3. Set `FiberOpticAlignmentSimulator` as the startup project.
4. Press `F5` to run.

The SQLite database is created automatically at startup under the application output folder:

`bin\Debug\net8.0-windows\Data\test_results.db`

## Project Structure

- `Models` contains simple data classes.
- `Services` contains simulation, alignment, and validation logic.
- `Data` contains SQLite repository code.
- `MainForm.cs` contains the single WinForms user interface.
