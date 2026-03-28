using System.Windows.Controls;
using ScientificApp.ViewModels;
using ScientificApp.Models;

namespace ScientificApp.Views;

/// <summary>
/// Interaction logic for VisualizationView.xaml
/// </summary>
public partial class VisualizationView : UserControl
{
    public VisualizationView()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Initialize ViewModel with required services.
    /// Called from MainWindow when setting up the view.
    /// </summary>
    public void InitializeViewModel(RegressionService regressionService, SampleData sampleData)
    {
        DataContext = new VisualizationViewModel(regressionService, sampleData);
    }
}
