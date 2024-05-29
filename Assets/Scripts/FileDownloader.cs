using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Net;
using System.IO;
using System;
using System.Threading.Tasks;
using Unity.VisualScripting;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class FileDownloader : MonoBehaviour
{
    public static FileDownloader Instance { get; private set; }

    [Header("Reference")]
    public GameObject FileDownloadPrefab;
    public Transform DownloaderParent;
    public Image mainProgressBar;
    public TextMeshProUGUI mainText;
    
    [Header("Global Variable")]
    [HideInInspector] public List<Task> downloadTasks = new List<Task>();

    [Header("Local Variable")]
    int totalfiles = 0;
    public int downloadedfiles = 0;
    float percentage = 0;
    List<string> fileUrls = new List<string>();

    [Header("Local Variable")]
    public Int64 sizeOfTotalDownload = 0;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        fileUrls = new List<string>
        {
            "https://demomultithread.web.app/Demo2.zip",
            "https://demomultithread.web.app/characters.zip",
            "https://demomultithread.web.app/Demo.zip",
            "https://imarticusgames.s3.ap-south-1.amazonaws.com/stratonboard/play/launchgame.html"
        };
        totalfiles = fileUrls.Count;
        StartCoroutine(DownloadFiles());
    }

    public IEnumerator MainThreadProgress()
    {
        if (totalfiles > 0)
        {
            percentage = (float)downloadedfiles / totalfiles;
            mainText.text = downloadedfiles + " Files Downloaded";
            mainProgressBar.DOFillAmount(percentage, 2f);
            yield return new WaitForSeconds(2f);
            if (percentage == 1f) 
            { 
                mainProgressBar.color = Color.green;
                mainText.text = "Download is Completed";
            }
        }
    }

    public void TextChanger(TextMeshProUGUI placeHolder, string val, string preText="")
    {
        placeHolder.text = preText+val;
    }

    public void Progressbar(Image image,float val)
    {
        image.fillAmount = val;
    }
    IEnumerator DownloadFiles()
    {
        yield return new WaitForSeconds(0.1f);

        foreach (string url in fileUrls)
        {
            GameObject child= Instantiate(FileDownloadPrefab,DownloaderParent);
            FileDownload fileDownload = child.GetComponent<FileDownload>();
            ThreadPool.QueueUserWorkItem(fileDownload.RunThread, url);

            Debug.Log(url);
            yield return new WaitForSeconds(0.1f); // Delay between downloads
        }
        Task.WaitAll(downloadTasks.ToArray());

    }

}