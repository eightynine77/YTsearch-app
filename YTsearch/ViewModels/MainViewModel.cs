using CommunityToolkit.Mvvm.ComponentModel;

namespace YTsearch.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}
