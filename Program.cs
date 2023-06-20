﻿using Avalonia;
using Avalonia.Media;
using System;

namespace MinecraftServerDownloader;

class Program
{
    // Initialization code. Don't use any Avalonia, third-party APIs or any
    // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
    // yet and stuff might break.
    [STAThread]
    public static void Main(string[] args) => BuildAvaloniaApp()
        .StartWithClassicDesktopLifetime(args);

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .LogToTrace()
            .With(new FontManagerOptions
            {
                DefaultFamilyName = "avares://MinecraftServerDownloader/Assets/Fonts#msyh.ttf"
            });
}
