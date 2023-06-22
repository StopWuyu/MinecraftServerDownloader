using Avalonia;
using Avalonia.ReactiveUI;
using System;

namespace MinecraftServerDownloader;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args)
    {
        BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);

        System.Threading.Thread.CurrentThread.CurrentCulture = new("zh-CN");

        System.Threading.Thread.CurrentThread.CurrentUICulture = new("zh-CN");
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .UseReactiveUI()
            .With(new FontManagerOptions
            {
                DefaultFamilyName = "avares://MinecraftServerDownloader/Asstes/Fonts#Alibaba_PuHuiTi_2.0_55_Regular_55_Regular.ttf"
            });
}
