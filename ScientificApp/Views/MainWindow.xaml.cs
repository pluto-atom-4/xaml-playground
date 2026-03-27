using System.Windows;
using ScientificApp.ViewModels;

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
        DataContext = new MainViewModel(); 
    }
}