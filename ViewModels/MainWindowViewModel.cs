namespace MinecraftServerDownloader.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public string Greeting => "Welcome to Avalonia!";

    public MainWindowViewModel()
    {
        selection0.SelectionChanged += SelectionChanged0;

        selection1.SelectionChanged += SelectionChanged1;

        selection2.SelectionChanged += SelectionChanged2;

        selection3.SelectionChanged += SelectionChanged3;

        TaskAsync();
    }

    public /* async */ void TaskAsync()
    {
        // Items = new(JsonDocument
        //     .Parse(await client
        //         .GetStringAsync("https://api.papermc.io/v2/projects/"))
        //         .RootElement
        //         .GetProperty("projects")
        //         .Deserialize<List<string>>()!);
    }

    private ObservableCollection<string>? _Items0 = new()
    {
        "paper",
        
        "travertine",
        
        "waterfall",
        
        "velocity",
        
        "folia"
    };

    private ObservableCollection<string>? _Items1 = new();

    private ObservableCollection<string>? _Items2 = new();

    private ObservableCollection<string>? _Items3 = new();

    public ObservableCollection<string>? Items0
    {
        get => _Items0;

        set => this.RaiseAndSetIfChanged(ref _Items0, value);
    }

    public ObservableCollection<string>? Items1
    {
        get => _Items1;

        set => this.RaiseAndSetIfChanged(ref _Items1, value);
    }

    public ObservableCollection<string>? Items2
    {
        get => _Items2;

        set => this.RaiseAndSetIfChanged(ref _Items2, value);
    }

    public ObservableCollection<string>? Items3
    {
        get => _Items3;

        set => this.RaiseAndSetIfChanged(ref _Items3, value);
    }

    public SelectionModel<object> selection0 { get; } = new();

    public SelectionModel<object> selection1 { get; } = new();

    public SelectionModel<object> selection2 { get; } = new();

    public SelectionModel<object> selection3 { get; } = new();

    public SaveFileDialog fileDialog = new()
    {
        Title = "保存文件"
    };

    async void SelectionChanged0(object? sender, SelectionModelSelectionChangedEventArgs e)
    {
        Items1 = new(JsonDocument
            .Parse(await client
                .GetStringAsync($"https://api.papermc.io/v2/projects/{e.SelectedItems.Single()}"))
                .RootElement
                .GetProperty("versions")
                .Deserialize<List<string>>()!);
    }

    async void SelectionChanged1(object? sender, SelectionModelSelectionChangedEventArgs e)
    {
        Items2 = new(JsonDocument
            .Parse(await client
                .GetStringAsync($"https://api.papermc.io/v2/projects/{selection0.SelectedItem}/versions/{e.SelectedItems.Single()}"))
                .RootElement
                .GetProperty("builds")
                .Deserialize<List<int>>()!.Select(x => x.ToString()));
    }

    async void SelectionChanged2(object? sender, SelectionModelSelectionChangedEventArgs e)
    {
        const string project_name = "project_name";

        const string downloads = "downloads";

        const string application = "application";

        const string name = "name";

        const string sha256 = "sha256";

        var json = JsonDocument
            .Parse(await client
                .GetStringAsync($"https://api.papermc.io/v2/projects/{selection0.SelectedItem}/versions/{selection1.SelectedItem}/builds/{e.SelectedItems.Single()}"))
                .RootElement;

        Items3 = new()
        {
            $"服务端类型：{json.GetProperty(project_name)}",
            
            $"点击下载：{json.GetProperty(downloads).GetProperty(application).GetProperty(name)}",
            
            $"SHA256校验：{json.GetProperty(downloads).GetProperty(application).GetProperty(sha256)}",
        };
    }

    async void SelectionChanged3(object? sender, SelectionModelSelectionChangedEventArgs e)
    {
        fileDialog.InitialFileName = $"{selection0.SelectedItem}-{selection1.SelectedItem}-{selection2.SelectedItem}.jar";

        var path = await fileDialog.ShowAsync(new());

        if (path is null)
            return;

        var response = await client.GetAsync($"https://api.papermc.io/v2/projects/{selection0.SelectedItem}/versions/{selection1.SelectedItem}/builds/{selection2.SelectedItem}/downloads/{selection0.SelectedItem}-{selection1.SelectedItem}-{selection2.SelectedItem}.jar");
        
        if (response.IsSuccessStatusCode)
        {
            Console.WriteLine("下一下先");

            File.WriteAllBytes(path, await response.Content.ReadAsByteArrayAsync());

            new Window()
            {
                Title = "下载完成！",

                MinHeight = 50,

                MinWidth = 100,

                Height = 100,

                Width = 300,

                Content = "下载完成！"
            }.Show();
        }
    }
}
