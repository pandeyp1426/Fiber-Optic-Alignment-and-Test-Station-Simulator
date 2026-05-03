# Fiber Optic Alignment and Test Station Simulator

A .NET 8 WinForms application that simulates a fiber optic alignment and test station used in laser manufacturing automation. The app generates or accepts a unit serial number, simulates X/Y alignment, measures optical power and temperature, validates the unit against station tolerances, and saves completed test runs to a local SQLite database.

## Screenshot

![Fiber Optic Alignment and Test Station Simulator](docs/screenshot.png)

## Features

- Generate or enter a fiber optic module serial number
- Simulate X/Y motion alignment using a peak-search routine
- Calculate optical power based on distance from the alignment center
- Simulate operating temperature readings
- Validate alignment, optical power, and temperature against station limits
- Display pass/fail status with failure reasons when limits are exceeded
- Save completed test runs to SQLite
- View saved test history in a WinForms table

## Pass/Fail Criteria

A test run passes only when all measured values are within tolerance:

- X and Y alignment offsets must be within `+/-0.08 mm`
- Optical power must be at least `1.80 mW`
- Temperature must be between `18.0 C` and `30.0 C`

The serial number text itself is not used to determine pass or fail. Blank serial numbers are replaced with an auto-generated value.

## Demo Failure Serial Numbers

For demonstrations, enter one of these serial numbers before generating and running a unit:

- `FAIL-ALIGN` shows an alignment tolerance failure
- `FAIL-POWER` shows a low optical power failure
- `FAIL-TEMP` shows a temperature range failure
- `FAIL-ALL` shows all failure reasons at once

## How To Run

1. Open `FiberOpticAlignmentSimulator.sln` in Visual Studio.
2. Restore NuGet packages if prompted.
3. Set `FiberOpticAlignmentSimulator` as the startup project.
4. Press `F5` to run.

The SQLite database is created automatically at startup under the application output folder:

```text
bin\Debug\net8.0-windows\Data\test_results.db
```

## Project Structure

- `Models` contains simple data classes.
- `Services` contains simulation, alignment, and validation logic.
- `Data` contains SQLite repository code.
- `MainForm.cs` contains the WinForms user interface.
