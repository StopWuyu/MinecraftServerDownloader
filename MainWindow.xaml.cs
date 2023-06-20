using MinecraftServerDownloader.Modules;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Newtonsoft.Json;
using System.Threading;
using System.Security.Policy;

namespace MinecraftServerDownloader
{
    public class Projects
    {
        public List<String> projects;
    }

    public class ProjectDetail
    {
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public List<string> VersionGroups { get; set; }
        public List<string> Versions { get; set; }
    }

    public class BuildData
    {
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Version { get; set; }
        public List<int> Builds { get; set; }
    }

    public class ProjectData
    {
        public string ProjectId { get; set; }
        public string ProjectName { get; set; }
        public string Version { get; set; }
        public int Build { get; set; }
        public DateTime Time { get; set; }
        public string Channel { get; set; }
        public bool Promoted { get; set; }
        public List<Change> Changes { get; set; }
        public Downloads Downloads { get; set; }
    }

    public class Change
    {
        public string Commit { get; set; }
        public string Summary { get; set; }
        public string Message { get; set; }
    }

    public class Downloads
    {
        public Application Application { get; set; }
    }

    public class Application
    {
        public string Name { get; set; }
        public string Sha256 { get; set; }
    }

    public partial class MainWindow : Window
    {
        string downloadUrl = "NO";

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        public async void Initialize()
        {
            // GET请求到https://api.papermc.io/v2/projects
            // 该API返回一个JSON字符串，包含了PaperMC的所有项目

            var projectsString = await ModNetwork.SendGetRequest("https://api.papermc.io/v2/projects");
            Console.WriteLine(projectsString);

            // 解析json
            var projects = JsonConvert.DeserializeObject<Projects>(projectsString);
            projectsSel.Items.Clear();
            projects.projects.ForEach((a) =>
            {
                projectsSel.Items.Add(a);
            });

            new Thread(() =>
            {
                while (true)
                {
                    if (downloadUrl.Equals("NO"))
                    {
                        System.Windows.Application.Current.Dispatcher.Invoke(() =>
                        {
                            buttonDownload.IsEnabled = false;
                        });
                    }
                    Thread.Sleep(100);
                }
            }).Start();
        }

        private async void projectsSel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;

            buildSel.Items.Clear();
            buildSel.SelectedItem = null;
            versionSel.Items.Clear();
            versionSel.SelectedItem = null;

            downloadUrl = "NO";
            typeHash.Text = "服务端SHA-256：";
            typeBuild.Text = "服务端构建号：";
            typeTime.Text = "服务端发布时间：";
            typeDownload.Text = "服务端下载地址：";
            typeVersion.Text = "服务端版本：";

            typeBuild.Visibility = Visibility.Hidden;
            typeHash.Visibility = Visibility.Hidden;
            typeTime.Visibility = Visibility.Hidden;
            typeDownload.Visibility = Visibility.Hidden;

            var projectDetailString = await ModNetwork.SendGetRequest("https://api.papermc.io/v2/projects/" + projectsSel.SelectedItem);
            Console.WriteLine(projectDetailString);

            // 解析
            var projectDetail = JsonConvert.DeserializeObject<ProjectDetail>(projectDetailString);
            versionSel.Items.Clear();
            projectDetail.Versions.ForEach((a) =>
            {
                versionSel.Items.Add(a);
            });
            typeName.Text = "服务端类型：" + projectsSel.SelectedItem;
            typeName.Visibility = Visibility.Visible;
        }

        private async void versionSel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;

            buildSel.Items.Clear();
            buildSel.SelectedItem = null;

            downloadUrl = "NO";
            typeHash.Text = "服务端SHA-256：";
            typeBuild.Text = "服务端构建号：";
            typeTime.Text = "服务端发布时间：";
            typeDownload.Text = "服务端下载地址：";

            typeBuild.Visibility = Visibility.Hidden;
            typeHash.Visibility = Visibility.Hidden;
            typeTime.Visibility = Visibility.Hidden;
            typeDownload.Visibility = Visibility.Hidden;

            var buildDataString = await ModNetwork.SendGetRequest("https://api.papermc.io/v2/projects/" + projectsSel.SelectedItem + "/versions/" + versionSel.SelectedItem);
            Console.WriteLine(buildDataString);

            // 解析
            var buildData = JsonConvert.DeserializeObject<BuildData>(buildDataString);
            buildSel.Items.Clear();
            var willAdd = buildData.Builds;
            willAdd.Sort((a, b) => b.CompareTo(a));
            willAdd.ForEach((a) =>
            {
                buildSel.Items.Add(a);
            });

            typeVersion.Text = "服务端版本：" + versionSel.SelectedItem;
            typeVersion.Visibility = Visibility.Visible;
        }

        private async void buildSel_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            e.Handled = true;
            downloadUrl = "NO";
            if (buildSel.SelectedItems.Count <= 0)
            {
                return;
            }
            var projectDataString = await ModNetwork.SendGetRequest("https://api.papermc.io/v2/projects/" + projectsSel.SelectedItem + "/versions/" + versionSel.SelectedItem + "/builds/" + buildSel.SelectedItem);

            Console.WriteLine(projectDataString);
            // 解析
            try
            {
                var projectData = JsonConvert.DeserializeObject<ProjectData>(projectDataString);
                typeBuild.Text = "服务端构建号：" + buildSel.SelectedItem;
                typeHash.Text = "服务端SHA-256：" + projectData.Downloads.Application.Sha256;
                typeTime.Text = "服务端发布时间：" + projectData.Time.ToString("yyyy-MM-dd HH:mm:ss");
                typeDownload.Text = "服务端下载地址：" + "https://api.papermc.io/v2/projects/" + projectsSel.SelectedItem + "/versions/" + versionSel.SelectedItem + "/builds/" + buildSel.SelectedItem + "/downloads/" + projectData.Downloads.Application.Name;
                typeDownload.Tag = "https://api.papermc.io/v2/projects/" + projectsSel.SelectedItem + "/versions/" + versionSel.SelectedItem + "/builds/" + buildSel.SelectedItem + "/downloads/" + projectData.Downloads.Application.Name;
                typeHash.Tag = projectData.Downloads.Application.Sha256;

                typeBuild.Visibility = Visibility.Visible;
                typeHash.Visibility = Visibility.Visible;
                typeTime.Visibility = Visibility.Visible;
                typeDownload.Visibility = Visibility.Visible;

                downloadUrl = "https://api.papermc.io/v2/projects/" + projectsSel.SelectedItem + "/versions/" + versionSel.SelectedItem + "/builds/" + buildSel.SelectedItem + "/downloads/" + projectData.Downloads.Application.Name;
                buttonDownload.IsEnabled = true;
            }
            catch
            {

            }
        }

        private void typeDownload_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetDataObject(typeDownload.Tag);
            var temp = typeDownload.Text;
            typeDownload.Text = "已复制";

            // 创建一个定时器，3秒后恢复原始文本
            var timer = new Timer(state =>
            {
                Dispatcher.Invoke(() =>
                {
                    typeDownload.Text = temp;
                });
            }, null, TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(-1));
        }

        private void typeHash_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Clipboard.SetDataObject(typeHash.Tag);

            var temp = typeHash.Text;
            typeHash.Text = "已复制";

            // 创建一个定时器，3秒后恢复原始文本
            var timer = new Timer(state =>
            {
                Dispatcher.Invoke(() =>
                {
                    typeHash.Text = temp;
                });
            }, null, TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(-1));
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(0);
        }

        private async void buttonDownload_Click(object sender, RoutedEventArgs e)
        {
            ProgressDownload.Value = 0;
            ProgressDownload.Visibility = Visibility.Visible;
            buttonDownload.Visibility = Visibility.Collapsed;
            Console.WriteLine(downloadUrl);
            var downloader = new MultiThreadedDownloader(downloadUrl,
                System.IO.Path.Combine(Environment.CurrentDirectory, string.Format("{0}-{1}-{2}.jar", projectsSel.SelectedItem, versionSel.SelectedItem, buildSel.SelectedItem)),
                4);
            downloader.ProgressChanged += (progress) =>  // Broken (I dont know how to fix it)
            {
                Console.WriteLine($"Thread {progress.ThreadId}: Downloaded {progress.DownloadedBytes} bytes. Downloaded {progress.ProgressPercentage} %");

                Dispatcher.Invoke(() =>
                {
                    ProgressDownload.Value = progress.ProgressPercentage;
                });
            };
            downloader.DownloadCompleted += (success, error) =>
            {
                if (success)
                {
                    Console.WriteLine("Download completed successfully.");
                    Dispatcher.Invoke(() =>
                    {
                        //buttonDownload.Content = "下载成功";
                        buttonDownload.Text = "下载成功";
                        Console.WriteLine(System.IO.Path.Combine(Environment.CurrentDirectory, string.Format("{0}-{1}-{2}.jar", projectsSel.SelectedItem, versionSel.SelectedItem, buildSel.SelectedItem)));
                        buttonDownload.Visibility = Visibility.Visible;
                        ProgressDownload.Visibility = Visibility.Collapsed;
                    });
                    var timer = new Timer(state =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            //buttonDownload.Content = "下载";
                            buttonDownload.Text = "下载";
                        });
                    }, null, TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(-1));
                }
                else
                {
                    Console.WriteLine($"Download failed with error: {error.Message}");
                    Console.WriteLine(error.StackTrace);
                    Dispatcher.Invoke(() =>
                    {
                        //buttonDownload.Content = "下载失败";
                        buttonDownload.Text = "下载失败";

                        buttonDownload.Visibility = Visibility.Visible;
                        ProgressDownload.Visibility = Visibility.Collapsed;
                    });
                    var timer = new Timer(state =>
                    {
                        Dispatcher.Invoke(() =>
                        {
                            //buttonDownload.Content = "下载";
                            buttonDownload.Text = "下载";
                        });
                    }, null, TimeSpan.FromSeconds(3), TimeSpan.FromMilliseconds(-1));
                }
            };
            await downloader.StartDownloadAsync();
        }
    }
}
