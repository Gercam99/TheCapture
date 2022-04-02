using UnityEngine;
using  UnityEngine.SceneManagement;

public class SceneSystem : Singleton<SceneSystem>
{
    public void LoadScene(string nameScene, bool async = false)
    {
        if (async) SceneManager.LoadSceneAsync(nameScene);
        else SceneManager.LoadScene(nameScene);
    }
    public void LoadScene(int numScene, bool async = false)
    {
        if (async) SceneManager.LoadSceneAsync(numScene);
        else SceneManager.LoadScene(numScene);
    }
    public void LoadScene(string nameScene, LoadSceneMode sceneMode = LoadSceneMode.Single, bool async = false)
    {
        if (async) SceneManager.LoadSceneAsync(nameScene, sceneMode);
        else SceneManager.LoadScene(nameScene, sceneMode);
    }
    public void LoadScene(int numScene, LoadSceneMode sceneMode = LoadSceneMode.Single, bool async = false)
    {
        if (async) SceneManager.LoadSceneAsync(numScene, sceneMode);
        else SceneManager.LoadScene(numScene, sceneMode);
    }


    public void LoadScene(string nameScene, float timeScale, bool async = false)
    {
        if (async) SceneManager.LoadSceneAsync(nameScene);
        else SceneManager.LoadScene(nameScene);

        Time.timeScale = timeScale;
    }
    
    public void LoadScene(int numScene, float timeScale = 1f, bool async = false)
    {
        if (async) SceneManager.LoadSceneAsync(numScene);
        else SceneManager.LoadScene(numScene);
        
        Time.timeScale = timeScale;
    }
    public void LoadScene(string nameScene, LoadSceneMode sceneMode = LoadSceneMode.Single, float timeScale = 1f,  bool async = false)
    {
        if (async) SceneManager.LoadSceneAsync(nameScene, sceneMode);
        else SceneManager.LoadScene(nameScene, sceneMode);
        
        Time.timeScale = timeScale;
    }
    public void LoadScene(int numScene, LoadSceneMode sceneMode = LoadSceneMode.Single, float timeScale = 1f, bool async = false)
    {
        if (async) SceneManager.LoadSceneAsync(numScene, sceneMode);
        else SceneManager.LoadScene(numScene, sceneMode);
        
        Time.timeScale = timeScale;
    }
}
