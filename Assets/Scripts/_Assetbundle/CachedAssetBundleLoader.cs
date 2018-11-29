﻿using UnityEngine;
using System.Collections;
using System;

public abstract class CachedAssetBundleLoader : MonoBehaviour{

    public string BundleURL;
    public string AssetName;
    public int version;
    private GameObject obj;
    //[SerializeField]
    //private ProgressBar pb;
    public AssetBundle bundle = null;
    public delegate void downloadFinished();
    public static event downloadFinished OnFinished;
    public delegate void download(float progress);
    public static event download OnDownload;

    public bool success = false;
    bool downloadInterrupted = false;
    public GameObject Obj
    {
        get { return obj; }
    }

    public AssetBundle AssetBundle { get { return bundle; } }
    public bool IsDone()
    {
        return false;
    }


    public IEnumerator IEGetFromCache(string url)
    {
        BundleURL = url;
        //DownloadProgress progress = new DownloadProgress();
        //yield return StartCoroutine(progress.IEGetAssetBundleSize(BundleURL));
        //print("Asset bundle size: " + progress.ContentLength);
        while (!Caching.ready)
            yield return null;
        using (WWW www = WWW.LoadFromCacheOrDownload(BundleURL, version))
        {
            if (www.isDone) print("dowload finished");
            else print("downloading " + url);
            while(!www.isDone)
            {
                
                //progress.Progress = www.progress;
                //print("download progress " + www.progress*100);
                //if (pb != null){pb.SetProgress(www.progress);}
                if(OnDownload != null){ OnDownload(www.progress);}
                if (Application.internetReachability == NetworkReachability.NotReachable)
                {
                    // 'downloadInterrupted' is a member variable of this very class and made 'false' in start.
                    // Application.internetReachability == NetworkReachability.NotReachable means that Internet got Disconnected.
                    // So making 'downloadInterrupted' = true and breaking the while loop.
                    downloadInterrupted = true;
                    break;
                }
                yield return new WaitForFixedUpdate();
            }

            if(Application.internetReachability != NetworkReachability.NotReachable && www.isDone){
                downloadInterrupted = false;
            }

            if (www.error != null || downloadInterrupted)
            {
                //throw new Exception("WWW download had an error:" + www.error);
                success = false;
                //MessageBox.ins.ShowOk("INTERNET CONNECTION FAILED.", MessageBox.MsgIcon.msgError, null);
            }
            else
            {
                success = true;
            }
            //else SetAssetBundle(www);
           
        }
    }

    


    void SetAssetBundle(WWW www){
       
        try {
            bundle = www.assetBundle;
           
        }
        catch(Exception ex){
            print(ex);
        }
      

        if (bundle != null)
        {
            //if (AssetName != "")
            //{
            //    obj = (GameObject)bundle.LoadAsset(AssetName);
            //    Instantiate(obj);
            //}

            //print("SL " +progress.Progress);
            //if (pb != null){ pb.SetProgress(1);}
            bundle.Unload(true);
            if (OnDownload != null) { OnDownload(1); } //sets download to 100%
            if (OnFinished != null) { OnFinished(); }
            print("bundle loaded");
        }
        else
        {
           print("bundle empty"); 
        }
    }
    
    
    
}
