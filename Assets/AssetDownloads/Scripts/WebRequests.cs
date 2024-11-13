using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public static class WebRequests {

    private class WebRequestsMonoBehaviour : MonoBehaviour { }

    private static WebRequestsMonoBehaviour webRequestsMonoBehaviour;

    private static void Init() {
        if (webRequestsMonoBehaviour == null) {
            GameObject gameObject = new GameObject("WebRequests");
            webRequestsMonoBehaviour = gameObject.AddComponent<WebRequestsMonoBehaviour>();
        }
    }

    public static void Get(string url, Action<string> onError, Action<string> onSuccess) {
        Init();
        webRequestsMonoBehaviour.StartCoroutine(GetCoroutine(url, null, onError, onSuccess));
    }

    public static void Get(string url, Action<UnityWebRequest> setHeaderAction, Action<string> onError, Action<string> onSuccess) {
        Init();
        webRequestsMonoBehaviour.StartCoroutine(GetCoroutine(url, setHeaderAction, onError, onSuccess));
    }

    private static IEnumerator GetCoroutine(string url, Action<UnityWebRequest> setHeaderAction, Action<string> onError, Action<string> onSuccess) {
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Get(url)) {
            if (setHeaderAction != null) {
                setHeaderAction(unityWebRequest);
            }
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                unityWebRequest.result == UnityWebRequest.Result.DataProcessingError ||
                unityWebRequest.result == UnityWebRequest.Result.ProtocolError) {
                // Error
                onError(unityWebRequest.error);
            } else {
                onSuccess(unityWebRequest.downloadHandler.text);
            }
        }
    }

    public static void Post(string url, Dictionary<string, string> formFields, Action<string> onError, Action<string> onSuccess) {
        Init();
        webRequestsMonoBehaviour.StartCoroutine(GetCoroutinePost(url, formFields, onError, onSuccess));
    }

    public static void Post(string url, string postData, Action<string> onError, Action<string> onSuccess) {
        Init();
        webRequestsMonoBehaviour.StartCoroutine(GetCoroutinePost(url, postData, onError, onSuccess));
    }

    public static void PostJson(string url, string jsonData, Action<string> onError, Action<string> onSuccess) {
        Init();
        webRequestsMonoBehaviour.StartCoroutine(GetCoroutinePostJson(url, null, jsonData, onError, onSuccess));
    }

    public static void PostJson(string url, Action<UnityWebRequest> setHeaderAction, string jsonData, Action<string> onError, Action<string> onSuccess) {
        Init();
        webRequestsMonoBehaviour.StartCoroutine(GetCoroutinePostJson(url, setHeaderAction, jsonData, onError, onSuccess));
    }

    private static IEnumerator GetCoroutinePost(string url, Dictionary<string, string> formFields, Action<string> onError, Action<string> onSuccess) {
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Post(url, formFields)) {
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                unityWebRequest.result == UnityWebRequest.Result.DataProcessingError ||
                unityWebRequest.result == UnityWebRequest.Result.ProtocolError) {
                // Error
                onError(unityWebRequest.error);
            } else {
                onSuccess(unityWebRequest.downloadHandler.text);
            }
        }
    }

    private static IEnumerator GetCoroutinePost(string url, string postData, Action<string> onError, Action<string> onSuccess) {
        using (UnityWebRequest unityWebRequest = UnityWebRequest.PostWwwForm(url, postData)) {
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                unityWebRequest.result == UnityWebRequest.Result.DataProcessingError ||
                unityWebRequest.result == UnityWebRequest.Result.ProtocolError) {
                // Error
                onError(unityWebRequest.error);
            } else {
                onSuccess(unityWebRequest.downloadHandler.text);
            }
        }
    }

    private static IEnumerator GetCoroutinePostJson(string url, Action<UnityWebRequest> setHeaderAction, string jsonData, Action<string> onError, Action<string> onSuccess) {
        using (UnityWebRequest unityWebRequest = new UnityWebRequest(url, "POST")) {
            byte[] bodyRaw = Encoding.UTF8.GetBytes(jsonData);
            unityWebRequest.uploadHandler = (UploadHandler)new UploadHandlerRaw(bodyRaw);
            unityWebRequest.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            unityWebRequest.SetRequestHeader("Content-Type", "application/json");

            if (setHeaderAction != null) {
                setHeaderAction(unityWebRequest);
            }

            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                unityWebRequest.result == UnityWebRequest.Result.DataProcessingError ||
                unityWebRequest.result == UnityWebRequest.Result.ProtocolError) {
                // Error
                onError(unityWebRequest.error);
            } else {
                onSuccess(unityWebRequest.downloadHandler.text);
            }
        }
    }

    public static void Put(string url, string bodyData, Action<string> onError, Action<string> onSuccess) {
        Init();
        webRequestsMonoBehaviour.StartCoroutine(GetCoroutinePut(url, bodyData, onError, onSuccess));
    }

    private static IEnumerator GetCoroutinePut(string url, string bodyData, Action<string> onError, Action<string> onSuccess) {
        using (UnityWebRequest unityWebRequest = UnityWebRequest.Put(url, bodyData)) {
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                unityWebRequest.result == UnityWebRequest.Result.DataProcessingError ||
                unityWebRequest.result == UnityWebRequest.Result.ProtocolError) {
                // Error
                onError(unityWebRequest.error);
            } else {
                onSuccess(unityWebRequest.downloadHandler.text);
            }
        }
    }

    public static void GetTexture(string url, Action<string> onError, Action<Texture2D> onSuccess) {
        Init();
        webRequestsMonoBehaviour.StartCoroutine(GetTextureCoroutine(url, onError, onSuccess));
    }

    private static IEnumerator GetTextureCoroutine(string url, Action<string> onError, Action<Texture2D> onSuccess) {
        using (UnityWebRequest unityWebRequest = UnityWebRequestTexture.GetTexture(url)) {
            yield return unityWebRequest.SendWebRequest();

            if (unityWebRequest.result == UnityWebRequest.Result.ConnectionError ||
                unityWebRequest.result == UnityWebRequest.Result.DataProcessingError ||
                unityWebRequest.result == UnityWebRequest.Result.ProtocolError) {
                // Error
                onError(unityWebRequest.error);
            } else {
                DownloadHandlerTexture downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
                onSuccess(downloadHandlerTexture.texture);
            }
        }
    }

}
