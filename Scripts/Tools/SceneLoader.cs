using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;

public class SceneLoader : MonoBehaviour
{
    [SerializeField]
    [Tooltip("The object this script is attatched to")]
    GameObject loaderObject; //general slot for the gameobject controlling the scene load
    [SerializeField]
    Scene currentScene;
    [SerializeField]
    string currentSceneName;
    [Tooltip("Scene to be loaded currently")]    
    [SerializeField]
    string targetSceneName;
    
    void Start()
    {
        loaderObject = this.gameObject;
        currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;
    }

    void Update()
    {
        currentScene = SceneManager.GetActiveScene();
        currentSceneName = currentScene.name;
    }
    
    public void SceneTargeter(string target)
    {
        targetSceneName = target;
        StartCoroutine("LoadSceneAsync");
    }

    IEnumerator LoadSceneAsync()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(targetSceneName);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        if (asyncLoad.isDone)
        {
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(targetSceneName));
        }
        yield return null;
    }
}
