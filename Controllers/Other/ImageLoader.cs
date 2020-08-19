using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;
public class ImageLoader : MonoBehaviour
{
    public string URL = "";

    public void LoadImage(string URL, Action<bool> onResponse = null)
    {
        StartCoroutine(StartDownloadingImage(URL, onResponse));
    }

    private RawImage _rawImage = null;
    IEnumerator StartDownloadingImage(string URL, Action<bool> onResponse)
    {
        if (_rawImage != null)
            Destroy(_rawImage.gameObject);

        UnityWebRequest request = UnityWebRequestTexture.GetTexture(URL);
        yield return request.SendWebRequest();
        if (request.isNetworkError || request.isHttpError)
            Debug.Log(request.error);
        else
        {
            _rawImage = Instantiate(PrefabsController.GetInstance().GetPrefab("OptionalRawImage"), transform).GetComponent<RawImage>();
            _rawImage.texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
        }

        if (onResponse != null)
            onResponse.Invoke(!(request.isNetworkError || request.isHttpError));
    }
}
