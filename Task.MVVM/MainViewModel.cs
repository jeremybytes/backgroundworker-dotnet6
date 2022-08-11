using SlowLibrary;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Task.MVVM;

public class MainViewModel : INotifyPropertyChanged
{
    CancellationTokenSource? _cancelTokenSource;

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

    }

    public void StartProcess()
    {
        Output = string.Empty;
        StartEnabled = false;
        CancelEnabled = true;

        _cancelTokenSource = new();

        SlowProcessor processor = new(Iterations);
        var task = DoWorkAsync(processor, _cancelTokenSource.Token,
        new Progress<ProgressData>(UpdateProgress));
        task.ContinueWith(TaskComplete);
    }

    public void CancelProcess()
    {
        _cancelTokenSource?.Cancel();
    }

    public class ProgressData
    {
        public int Percentage { get; set; }
        public string? Message { get; set; }
    }

    // Runs on Background Thread
    private Task<int> DoWorkAsync(SlowProcessor model,
        CancellationToken cancelToken, IProgress<ProgressData> progress)
    {
        return Task<int>.Run(() =>
            {
                int result = 0;
                foreach (var val in model)
                {
                    cancelToken.ThrowIfCancellationRequested();
                    int percentComplete = (int)((float)val / (float)Iterations * 100);
                    string updateMessage =
                        string.Format("Iteration {0} of {1}", val, Iterations);
                    progress.Report(new ProgressData()
                    { Percentage = percentComplete, Message = updateMessage });
                    result = val;
                }
                return result;
            });
    }

    // Runs on UI Thread
    private void TaskComplete(Task<int> task)
    {
        if (task.IsFaulted)
        {
            // Handle exception state
        }
        else if (task.IsCanceled)
        {
            Output = "Canceled";
        }
        else if (task.IsCompleted)
        {
            Output = task.Result.ToString();
            ProgressPercentage = 0;
        }
        StartEnabled = true;
        CancelEnabled = false;
    }

    // Runs on UI Thread
    private void UpdateProgress(ProgressData data)
    {
        ProgressPercentage = data.Percentage;
        Output = data.Message;
    }


    public event PropertyChangedEventHandler? PropertyChanged;
    private void RaisePropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

