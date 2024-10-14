using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class SceneManagerUtil
{
    private static string _targetScene;

    public static void LoadNetworkAsync(this MonoBehaviour mono, string scene)
    {
        Debug.Log($"Target Scene: {scene}");

        _targetScene = scene;

        NetworkManager.Singleton.SceneManager.OnSceneEvent += SceneManager_OnSceneEvent;

        mono.StartCoroutine(LoadNetworkSceneAsyncCoroutine(scene));
    }

    private static void SceneManager_OnSceneEvent(SceneEvent sceneEvent)
    {
        Debug.Log($"Load Scene Callback: {sceneEvent.SceneName} | ClientId: {sceneEvent.ClientId}");
    }

    private static IEnumerator LoadNetworkSceneAsyncCoroutine(string scene, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
    {

        Debug.Log($"Loading Scene: {scene}");

        _ = NetworkManager.Singleton.SceneManager.LoadScene(scene, loadSceneMode);

        yield return null;
    }
}
