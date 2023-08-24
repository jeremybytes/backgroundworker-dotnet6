using SlowLibrary;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace BGW.MVVM;

public class MainViewModel : INotifyPropertyChanged
{
    private readonly BackgroundWorker backgroundWorker = new();

    private int iterations = 50;
    public int Iterations
    {
        get => iterations;
        set { iterations = value; RaisePropertyChanged(); }
    }

    private int progressPercentage = 0;
    public int ProgressPercentage
    {
        get => progressPercentage;
        set { progressPercentage = value; RaisePropertyChanged(); }
    }

    private string? output = string.Empty;
    public string? Output
    {
        get => output;
        set { output = value; RaisePropertyChanged(); }
    }

    private bool startEnabled = true;
    public bool StartEnabled
    {
        get => startEnabled;
        set { startEnabled = value; RaisePropertyChanged(); }
    }

    private bool cancelEnabled = false;
    public bool CancelEnabled
    {
        get => cancelEnabled;
        set { cancelEnabled = value; RaisePropertyChanged(); }
    }


    public MainViewModel()
    {
        // Background Process
        backgroundWorker.DoWork += BackgroundWorker_DoWork;
        backgroundWorker.RunWorkerCompleted += BackgroundWorker_RunWorkerCompleted;

        // Progress
        backgroundWorker.WorkerReportsProgress = true;
        backgroundWorker.ProgressChanged += BackgroundWorker_ProgressChanged;

        // Cancellation
        backgroundWorker.WorkerSupportsCancellation = true;
    }


    public void StartProcess()
    {
        if (backgroundWorker.IsBusy)
            return;

        backgroundWorker.RunWorkerAsync();

        StartEnabled = !backgroundWorker.IsBusy;
        CancelEnabled = backgroundWorker.IsBusy;
        Output = string.Empty;
    }

    public void CancelProcess()
    {
        backgroundWorker.CancelAsync();
    }


    // Runs on Background Thread
    private void BackgroundWorker_DoWork(object? sender, DoWorkEventArgs e)
    {
        if (sender is not BackgroundWorker worker) return;

        int result = 0;

        SlowProcessor processor = new(Iterations);
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
                string progressMessage = 
                    $"Iteration {current} of {iterations}";
                worker.ReportProgress(percentageComplete, progressMessage);
            }

            result = current;
        }

        e.Result = result;
    }

    // Runs on UI Thread
    private void BackgroundWorker_RunWorkerCompleted(object? sender,
        RunWorkerCompletedEventArgs e)
    {
        if (e.Error != null)
        {
            Output = e.Error.Message;
        }
        else if (e.Cancelled)
        {
            Output = "Canceled";
        }
        else
        {
            Output = e.Result?.ToString();
            ProgressPercentage = 0;
        }
        StartEnabled = !backgroundWorker.IsBusy;
        CancelEnabled = backgroundWorker.IsBusy;
    }

    // Runs on UI Thread
    private void BackgroundWorker_ProgressChanged(object? sender,
        ProgressChangedEventArgs e)
    {
        ProgressPercentage = e.ProgressPercentage;
        Output = e.UserState?.ToString();
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

