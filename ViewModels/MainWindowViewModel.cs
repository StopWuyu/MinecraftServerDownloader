namespace MinecraftServerDownloader.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public MainWindowViewModel()
    {
        selection.SelectionChanged += SelectionChanged;

        MyItems = new()
        {
            "我超冰！"
        };
    }

    private ObservableCollection<string>? _myItem;

    public ObservableCollection<string>? MyItems
    {
        get => _myItem;

        set => this.RaiseAndSetIfChanged(ref _myItem, value);
    }

    public string Greeting => "Welcome to Avalonia!";

    public string[] items { get; set; } = new[] { "yee", "哇袄！" };

    public List<string> list { get; set; } = new()
    {
        "我超冰",

        "我去冰"
    };

    public SelectionModel<object> selection { get; } = new();

    public OpenFileDialog fileDialog = new()
    {
        AllowMultiple = true
    };

    void SelectionChanged(object? sender, SelectionModelSelectionChangedEventArgs e)
    {
        switch (e.SelectedIndexes[0])
        {
            case 0:

                MyItems?.Add("我超！！！！冰！！！");

                break;

            case 1:

                MyItems?.Add("从未有如此美妙的开局！！");

                break;
        }
    }
}
