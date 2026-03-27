using CommunityToolkit.Mvvm.ComponentModel;

namespace ScientificApp.ViewModels;

public partial class MainViewModel: ObservableObject
{
    [ObservableProperty]
    private string _statusMessage = "Ready for Data Analysis";
}