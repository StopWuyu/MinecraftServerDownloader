using System;
using System.IO;
using System.Net;
using System.Threading;

namespace MinecraftServerDownloader.Modules
{
    internal class MultiThreadedDownloader
    {
        private const int BufferSize = 4096; // 缓冲区大小

        private string downloadUrl; // 下载文件的URL
        private string savePath; // 保存文件的路径
        private int threadCount; // 线程数量

        private long totalSize; // 文件总大小
        private long downloadedSize; // 已下载的文件大小

        private ManualResetEvent[] downloadEvents; // 用于线程同步的 ManualResetEvent 数组
        private bool[] isThreadFinished; // 标记线程是否完成下载
        private Exception downloadException; // 下载过程中出现的异常

        public event Action<int, long, int> ProgressChanged; // 下载百分比改变事件
        public event Action<bool, Exception> DownloadCompleted; // 下载完成事件

        public MultiThreadedDownloader(string url, string savePath, int threadCount)
        {
            downloadUrl = url;
            this.savePath = savePath;
            this.threadCount = threadCount;
        }

        public void StartDownload()
        {
            try
            {
                using (var webClient = new WebClient())
                {
                    webClient.DownloadDataCompleted += WebClient_DownloadDataCompleted;

                    // 开始异步下载
                    webClient.DownloadDataAsync(new Uri(downloadUrl));
                }
            }
            catch (Exception ex)
            {
                OnDownloadCompleted(false, ex);
            }
        }

        private void WebClient_DownloadDataCompleted(object sender, DownloadDataCompletedEventArgs e)
        {
            try
            {
                if (e.Error != null)
                {
                    OnDownloadCompleted(false, e.Error);
                    return;
                }

                byte[] data = e.Result;

                // 获取文件的大小
                string contentLengthHeader = ((WebClient)sender).ResponseHeaders["Content-Length"];
                if (!string.IsNullOrEmpty(contentLengthHeader) && long.TryParse(contentLengthHeader, out long fileSize))
                {
                    totalSize = fileSize;
                }
                else
                {
                    throw new Exception("无法获取文件大小。");
                }

                downloadedSize = 0;
                downloadEvents = new ManualResetEvent[threadCount];
                isThreadFinished = new bool[threadCount];

                // 创建线程并开始下载
                int completedThreadCount = 0; // 完成下载的线程数量
                for (int i = 0; i < threadCount; i++)
                {
                    int threadId = i;
                    downloadEvents[threadId] = new ManualResetEvent(false);
                    isThreadFinished[threadId] = false;

                    ThreadPool.QueueUserWorkItem(state =>
                    {
                        DownloadPart(data, threadId);
                        Interlocked.Increment(ref completedThreadCount); // 增加已完成的线程数量

                        if (completedThreadCount == threadCount)
                        {
                            // 所有线程都已完成下载
                            if (downloadException != null)
                            {
                                OnDownloadCompleted(false, downloadException);
                            }
                            else
                            {
                                MergeFilesAndDeleteTempFiles();
                                OnDownloadCompleted(true, null);
                            }
                        }
                    });
                }

                // 等待所有下载线程完成
                WaitHandle.WaitAll(downloadEvents);
            }
            catch (Exception ex)
            {
                OnDownloadCompleted(false, ex);
            }
        }

        private void DownloadPart(byte[] data, int threadId)
        {
            try
            {
                // 计算每个线程负责下载的字节范围
                long start = (totalSize / threadCount) * threadId;
                long end = threadId == threadCount - 1 ? totalSize - 1 : (totalSize / threadCount) * (threadId + 1) - 1;

                // 截取数据范围
                byte[] partData = new byte[end - start + 1];
                Array.Copy(data, start, partData, 0, partData.Length);

                // 写入临时分块文件
                string tempFilePath = $"{savePath}.{threadId}";
                using (var outputStream = File.OpenWrite(tempFilePath))
                {
                    outputStream.Write(partData, 0, partData.Length);
                }

                // 更新已下载的文件大小和下载进度
                Interlocked.Add(ref downloadedSize, partData.Length);
                int progressPercentage = (int)((downloadedSize * 100) / totalSize);

                // 触发进度变化事件
                OnProgressChanged(threadId, downloadedSize, progressPercentage);

                isThreadFinished[threadId] = true;
                downloadEvents[threadId].Set();
            }
            catch (Exception ex)
            {
                downloadException = ex;
                downloadEvents[threadId].Set();
            }
        }

        private void MergeFilesAndDeleteTempFiles()
        {
            string[] tempFilePaths = new string[threadCount];
            for (int i = 0; i < threadCount; i++)
            {
                tempFilePaths[i] = $"{savePath}.{i}";
            }

            FileMerger.MergeFiles(tempFilePaths, savePath);

            // 删除临时分块文件
            foreach (var tempFilePath in tempFilePaths)
            {
                File.Delete(tempFilePath);
            }
        }

        private void OnProgressChanged(int threadId, long downloadedBytes, int progressPercentage)
        {
            ProgressChanged?.Invoke(threadId, downloadedBytes, progressPercentage);
        }

        private void OnDownloadCompleted(bool success, Exception error)
        {
            DownloadCompleted?.Invoke(success, error);
        }
    }

    internal class FileMerger
    {
        public static void MergeFiles(string[] filePaths, string savePath)
        {
            using (var outputStream = File.OpenWrite(savePath))
            {
                foreach (var filePath in filePaths)
                {
                    using (var inputStream = File.OpenRead(filePath))
                    {
                        inputStream.CopyTo(outputStream);
                    }
                }
            }
        }
    }
}
