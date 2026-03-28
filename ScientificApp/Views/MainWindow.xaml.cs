using System.Windows;
using ScientificApp.ViewModels;
using ScientificApp.Models;

namespace ScientificApp.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        // This connects the UI to your logic
        var mainViewModel = new MainViewModel();
        DataContext = mainViewModel;

        // Initialize VisualizationView with services
        VisualizationViewControl.InitializeViewModel(
            mainViewModel.RegressionService,
            mainViewModel.SampleData
        );
    }
}