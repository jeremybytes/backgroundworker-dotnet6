using System.Windows;

namespace BGW.MVVM;

public partial class MainView : Window
{
    private readonly MainViewModel viewModel = new();

    public MainView()
    {
        InitializeComponent();
        DataContext = viewModel;
    }

    private void StartButton_Click(object sender, RoutedEventArgs e)
    {
        viewModel.StartProcess();
    }

    private void CancelButton_Click(object sender, RoutedEventArgs e)
    {
        viewModel.CancelProcess();
    }
}
