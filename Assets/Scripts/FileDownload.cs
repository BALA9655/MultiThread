using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class FileDownload : MonoBehaviour
{
    [Header("Reference")]
    [SerializeField] private TextMeshProUGUI ThreadId; 
    [SerializeField] private TextMeshProUGUI ThreadProgress;
    [SerializeField] private Image progressBar;

    [Header("Local Variable")]
    bool isTotalSizeAdded;
    int tID = 0;
    private void Awake()
    {

    }

    public void RunThread(object url)
    {
        Task downloadTask = Task.Run( () =>
        {
            // Download the file using WebClient
            using (WebClient client = new WebClient())
            {
                client.DownloadProgressChanged += DownloadProgressHandler;
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadFileCompleted);
                tID =Thread.CurrentThread.ManagedThreadId;
                string fileName = Path.GetFileName((string)url);
                Debug.Log("Download Initiated "+ tID);
                client.DownloadFileAsync(new Uri((string)url),Application.dataPath + "/" + Path.GetFileName((string)url));
            }

        });
        ExecuteOnMainThread.RunOnMainThread.Enqueue(() =>
        {
            FileDownloader.Instance.TextChanger(ThreadId, tID.ToString(),"Thread Id : ");
        });
        
        FileDownloader.Instance.downloadTasks.Add(downloadTask);

       
    }

    private void DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
    {
        if(e.Error != null)
        {
            Debug.Log("Failed");
            return;
        }
        Debug.Log("File Download Completed");
        
        ExecuteOnMainThread.RunOnMainThread.Enqueue(() =>
        {
            progressBar.color = Color.green;
            FileDownloader.Instance.downloadedfiles++;
           StartCoroutine(FileDownloader.Instance.MainThreadProgress());
        } );
    }

    private void DownloadProgressHandler(object sender, DownloadProgressChangedEventArgs e)
    {
        long downloadedBytes = e.BytesReceived;
        long totalBytes = e.TotalBytesToReceive;
        ExecuteOnMainThread.RunOnMainThread.Enqueue(() =>
        {
            if(!isTotalSizeAdded)
            {
                FileDownloader.Instance.sizeOfTotalDownload += totalBytes;
                isTotalSizeAdded = true;
            }
            FileDownloader.Instance.TextChanger(ThreadProgress, (((float)downloadedBytes / totalBytes) * 100f).ToString(),"Progress : ");
            FileDownloader.Instance.Progressbar(progressBar, ((float)downloadedBytes / totalBytes));
        });
        
        //Debug.Log(downloadedBytes+"---"+totalBytes+"--" + Thread.CurrentThread.ManagedThreadId);
    }
}
