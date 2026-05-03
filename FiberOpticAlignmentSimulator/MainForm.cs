using FiberOpticAlignmentSimulator.Data;
using FiberOpticAlignmentSimulator.Models;
using FiberOpticAlignmentSimulator.Services;

namespace FiberOpticAlignmentSimulator;

/// <summary>
/// Main WinForms screen for running the virtual alignment and test station.
/// </summary>
public class MainForm : Form
{
    private readonly TestStation _testStation = new();
    private readonly ResultRepository _resultRepository;

    private FiberPart? _currentPart;
    private TestResult? _currentResult;

    private readonly TextBox _serialNumberTextBox = new();
    private readonly Label _xOffsetValueLabel = new();
    private readonly Label _yOffsetValueLabel = new();
    private readonly Label _powerValueLabel = new();
    private readonly Label _temperatureValueLabel = new();
    private readonly Label _resultValueLabel = new();
    private readonly Label _failureReasonValueLabel = new();
    private readonly Button _runTestButton = new();
    private readonly Button _saveResultButton = new();
    private readonly DataGridView _historyGrid = new();

    public MainForm()
    {
        Text = "Fiber Optic Alignment and Test Station Simulator";
        StartPosition = FormStartPosition.CenterScreen;
        Width = 1040;
        Height = 720;
        MinimumSize = new Size(920, 620);

        string databasePath = Path.Combine(AppContext.BaseDirectory, "Data", "test_results.db");
        _resultRepository = new ResultRepository(databasePath);

        BuildLayout();
        InitializeDatabase();
        LoadSavedResults();
        ClearDisplayedMeasurements();
    }

    private void BuildLayout()
    {
        TableLayoutPanel rootLayout = new()
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Padding = new Padding(12)
        };
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rootLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        Label titleLabel = new()
        {
            Text = "Fiber Optic Alignment and Test Station Simulator",
            AutoSize = true,
            Font = new Font(Font.FontFamily, 14, FontStyle.Bold),
            Margin = new Padding(0, 0, 0, 12)
        };

        GroupBox stationGroup = new()
        {
            Text = "Station Controls",
            Dock = DockStyle.Top,
            AutoSize = true,
            Padding = new Padding(12)
        };

        TableLayoutPanel controlsLayout = new()
        {
            Dock = DockStyle.Top,
            ColumnCount = 6,
            RowCount = 4,
            AutoSize = true
        };
        controlsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        controlsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 180));
        controlsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 150));
        controlsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170));
        controlsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
        controlsLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));

        Button generateUnitButton = new()
        {
            Text = "Generate Unit",
            Width = 130,
            Height = 32
        };
        generateUnitButton.Click += GenerateUnitButton_Click;

        _runTestButton.Text = "Run Alignment Test";
        _runTestButton.Width = 150;
        _runTestButton.Height = 32;
        _runTestButton.Enabled = false;
        _runTestButton.Click += RunTestButton_Click;

        _saveResultButton.Text = "Save Result";
        _saveResultButton.Width = 120;
        _saveResultButton.Height = 32;
        _saveResultButton.Enabled = false;
        _saveResultButton.Click += SaveResultButton_Click;

        _serialNumberTextBox.Width = 170;

        AddControlRow(controlsLayout, 0, "Serial Number", _serialNumberTextBox);
        controlsLayout.Controls.Add(generateUnitButton, 2, 0);
        controlsLayout.Controls.Add(_runTestButton, 3, 0);
        controlsLayout.Controls.Add(_saveResultButton, 4, 0);

        AddValueRow(controlsLayout, 1, "X Offset", _xOffsetValueLabel, "Y Offset", _yOffsetValueLabel);
        AddValueRow(controlsLayout, 2, "Power", _powerValueLabel, "Temperature", _temperatureValueLabel);
        AddValueRow(controlsLayout, 3, "Result", _resultValueLabel, "Failure Reason", _failureReasonValueLabel);

        stationGroup.Controls.Add(controlsLayout);

        GroupBox historyGroup = new()
        {
            Text = "Saved Test Runs",
            Dock = DockStyle.Fill,
            Padding = new Padding(12)
        };

        _historyGrid.Dock = DockStyle.Fill;
        _historyGrid.ReadOnly = true;
        _historyGrid.AllowUserToAddRows = false;
        _historyGrid.AllowUserToDeleteRows = false;
        _historyGrid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        _historyGrid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        _historyGrid.MultiSelect = false;

        historyGroup.Controls.Add(_historyGrid);

        rootLayout.Controls.Add(titleLabel, 0, 0);
        rootLayout.Controls.Add(stationGroup, 0, 1);
        rootLayout.Controls.Add(historyGroup, 0, 2);

        Controls.Add(rootLayout);
    }

    private static void AddControlRow(TableLayoutPanel layout, int row, string labelText, Control inputControl)
    {
        Label label = CreateFieldLabel(labelText);
        layout.Controls.Add(label, 0, row);
        layout.Controls.Add(inputControl, 1, row);
    }

    private static void AddValueRow(
        TableLayoutPanel layout,
        int row,
        string firstLabelText,
        Label firstValueLabel,
        string secondLabelText,
        Label secondValueLabel)
    {
        layout.Controls.Add(CreateFieldLabel(firstLabelText), 0, row);
        layout.Controls.Add(PrepareValueLabel(firstValueLabel), 1, row);
        layout.Controls.Add(CreateFieldLabel(secondLabelText), 2, row);
        layout.Controls.Add(PrepareValueLabel(secondValueLabel), 3, row);
    }

    private static Label CreateFieldLabel(string text)
    {
        return new Label
        {
            Text = text,
            AutoSize = true,
            Anchor = AnchorStyles.Left,
            Margin = new Padding(0, 8, 8, 8)
        };
    }

    private static Label PrepareValueLabel(Label label)
    {
        label.AutoSize = true;
        label.Anchor = AnchorStyles.Left;
        label.Margin = new Padding(0, 8, 16, 8);
        label.Font = new Font(label.Font, FontStyle.Bold);
        return label;
    }

    private void InitializeDatabase()
    {
        try
        {
            _resultRepository.InitializeDatabase();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Database setup failed: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void GenerateUnitButton_Click(object? sender, EventArgs e)
    {
        _currentPart = _testStation.GeneratePart(_serialNumberTextBox.Text);
        _currentResult = null;
        _serialNumberTextBox.Text = _currentPart.SerialNumber;
        _runTestButton.Enabled = true;
        _saveResultButton.Enabled = false;

        DisplayPartBeforeTest(_currentPart);
    }

    private void RunTestButton_Click(object? sender, EventArgs e)
    {
        if (_currentPart is null)
        {
            _currentPart = _testStation.GeneratePart(_serialNumberTextBox.Text);
            _serialNumberTextBox.Text = _currentPart.SerialNumber;
        }

        _currentResult = _testStation.RunTest(_currentPart);
        DisplayResult(_currentResult);
        _saveResultButton.Enabled = true;
    }

    private void SaveResultButton_Click(object? sender, EventArgs e)
    {
        if (_currentResult is null)
        {
            return;
        }

        try
        {
            _resultRepository.Save(_currentResult);
            _saveResultButton.Enabled = false;
            LoadSavedResults();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Save failed: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void LoadSavedResults()
    {
        try
        {
            List<TestResult> results = _resultRepository.GetAll();
            _historyGrid.DataSource = results
                .Select(result => new
                {
                    result.Id,
                    result.SerialNumber,
                    StartTime = result.StartTime.ToString("g"),
                    EndTime = result.EndTime.ToString("g"),
                    XOffset = result.XOffset.ToString("F3"),
                    YOffset = result.YOffset.ToString("F3"),
                    PowerMW = result.PowerMW.ToString("F3"),
                    TemperatureC = result.TemperatureC.ToString("F1"),
                    Result = result.Passed ? "Pass" : "Fail",
                    result.FailureReason
                })
                .ToList();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not load saved results: {ex.Message}", "Database Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void DisplayPartBeforeTest(FiberPart part)
    {
        _xOffsetValueLabel.Text = $"{part.XOffset:F3} mm";
        _yOffsetValueLabel.Text = $"{part.YOffset:F3} mm";
        _powerValueLabel.Text = $"{part.PowerMW:F3} mW";
        _temperatureValueLabel.Text = $"{part.TemperatureC:F1} C";
        _resultValueLabel.Text = "Not tested";
        _failureReasonValueLabel.Text = string.Empty;
    }

    private void DisplayResult(TestResult result)
    {
        _xOffsetValueLabel.Text = $"{result.XOffset:F3} mm";
        _yOffsetValueLabel.Text = $"{result.YOffset:F3} mm";
        _powerValueLabel.Text = $"{result.PowerMW:F3} mW";
        _temperatureValueLabel.Text = $"{result.TemperatureC:F1} C";
        _resultValueLabel.Text = result.Passed ? "Pass" : "Fail";
        _resultValueLabel.ForeColor = result.Passed ? Color.DarkGreen : Color.DarkRed;
        _failureReasonValueLabel.Text = result.Passed ? string.Empty : result.FailureReason;
    }

    private void ClearDisplayedMeasurements()
    {
        _xOffsetValueLabel.Text = "-";
        _yOffsetValueLabel.Text = "-";
        _powerValueLabel.Text = "-";
        _temperatureValueLabel.Text = "-";
        _resultValueLabel.Text = "No unit loaded";
        _failureReasonValueLabel.Text = string.Empty;
    }
}
