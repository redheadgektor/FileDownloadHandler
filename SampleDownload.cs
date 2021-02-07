IEnumerator DownloadOrUpdate(string filename)
    {
        string filename = "file.bin";//file name
        using (UnityWebRequest web = UnityWebRequest.Get("http://localhost/"+filename))
        {
            downloadHandler = new BundleDownloadHandler(Path.Combine(Application.dataPath, filename)));
            UnityWebRequestAsyncOperation ao = web.SendWebRequest();

            Debug.Log("Trying download " + filename + " from remote storage...");

            ulong download_size = 0;
            ulong downloaded_bytes = 0;

            while (!web.isDone)
            {
                if (web.downloadProgress > 0.01f && (int)(web.downloadProgress * 1000f) != oldProgress)
                {
                    download_size = (web.downloadHandler as BundleDownloadHandler).contentSize;
                    downloaded_bytes = (ulong)((float)download_size * web.downloadProgress);
                    
                    Debug.Log("Downloading bundle " + filename + " from remote storage " + (web.downloadProgress * 100f).ToString("f1") + "%");

                }

                yield return null;
            }

            if (web.isNetworkError || web.isHttpError)
            {
                Debug.LogWarning("Bundle " + filename + " downloading error...   (" + web.error + " | responce_code " + web.responseCode + ")");
                (web.downloadHandler as BundleDownloadHandler).DeleteFile();
                yield return new WaitForSeconds(2);
            }

            if (web.isDone)
            {
                stream?.Close();
                (web.downloadHandler as BundleDownloadHandler).ReplaceFile();
            }
        }
    }
