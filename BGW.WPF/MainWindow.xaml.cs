using SlowLibrary;
using System.ComponentModel;
using System.Windows;

namespace BGW.WPF;

public partial class MainWindow : Window
{
    private readonly BackgroundWorker backgroundWorker = new();

    public MainWindow()
    {
        InitializeComponent();
        InitializeBackgroundWorker();
    }

    private void InitializeBackgroundWorker()
    {
        // Background Process
        backgroundWorker.DoWork += BackgroundWorker_DoWork;
        backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

        // Progress Reporting
        backgroundWorker.WorkerReportsProgress = true;
        backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;

        // Cancellation
        backgroundWorker.WorkerSupportsCancellation = true;
    }

    // Runs on UI Thread
    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        if (int.TryParse(InputTextBox.Text, out int iterations))
        {
            if (backgroundWorker.IsBusy)
                return;

            backgroundWorker.RunWorkerAsync(iterations);

            StartButton.IsEnabled = !backgroundWorker.IsBusy;
            CancelButton.IsEnabled = backgroundWorker.IsBusy;
            OutputTextBox.Text = string.Empty;
        }
    }

    // Runs on UI Thread
    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        backgroundWorker.CancelAsync();
    }

    // Runs on Background Thread
    private void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
    {
        if (sender is not BackgroundWorker worker) return;

        int result = 0;
        if (!int.TryParse(e.Argument?.ToString(), out int iterations)) return;

        SlowProcessor processor = new(iterations);
        foreach (var current in processor)
        {
            if (worker.CancellationPending)
            {
                e.Cancel = true;
                break;
            }
            if (worker.WorkerReportsProgress)
            {
                int percentageComplete =
                    (int)((float)current / iterations * 100);
                string progressMessage = $"Iteration {current} of {iterations}";
                worker.ReportProgress(percentageComplete, progressMessage);
            }

            result = current;
        }
        e.Result = result;
    }

    // Runs on UI Thread
    private void BackgroundWorker_RunWorkerCompleted(object? sender, RunWorkerCompletedEventArgs e)
    {
        if (e.Error != null)
        {
            OutputTextBox.Text = e.Error.Message;
        }
        else if (e.Cancelled)
        {
            OutputTextBox.Text = "Canceled";
        }
        else
        {
            OutputTextBox.Text = e.Result?.ToString();
            MainProgressBar.Value = 0;
        }
        StartButton.IsEnabled = !backgroundWorker.IsBusy;
        CancelButton.IsEnabled = backgroundWorker.IsBusy;
    }

    // Runs on UI Thread
    private void BackgroundWorker_ProgressChanged(object? sender, ProgressChangedEventArgs e)
    {
        MainProgressBar.Value = e.ProgressPercentage;
        OutputTextBox.Text = e.UserState?.ToString();
    }
}
