using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace MinecraftServerDownloader
{
    public partial class App : System.Windows.Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            // 订阅 DispatcherUnhandledException 事件
            Current.DispatcherUnhandledException += Current_DispatcherUnhandledException;
        }

        private void Current_DispatcherUnhandledException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)
        {
            // 处理异常
            Exception exception = e.Exception;
            // 进行异常处理逻辑，如记录异常信息、显示错误消息等

            // 标记为已处理，以防止应用程序崩溃
            e.Handled = true;
        }
    }
}
