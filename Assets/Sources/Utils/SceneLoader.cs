using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum SceneID
{
    Title,
    InGame,
}

[System.Serializable]
public class SceneSettings
{
    public SceneID defaultScene = SceneID.Title;
    private Dictionary<SceneID, string> _scenes = new Dictionary<SceneID, string>()
    {
        {SceneID.Title, "SceneTitle" },
        {SceneID.InGame, "SceneGame" }
    };

    public string GetSceneName(SceneID scene)
    {
        if (!_scenes.TryGetValue(scene, out var name))
            return "";
        return name;
    }
}
public class SceneLoader
{
    private SceneSettings _settings;
    private SceneBase _currentScene;
    private Queue<SceneBase> _nextScenes = new Queue<SceneBase>();

    public SceneBase CurrentScene => _currentScene;

    public SceneLoader(SceneSettings settings)
    {
        _settings = settings;
        Initialize();
    }

    public void Initialize() { }

    public void LoadScene(SceneID scene)
    {
        LoadScene(_settings.GetSceneName(scene));
    }

    private void LoadScene(string sceneName)
    {
        if (sceneName.Equals(""))
        {
            return;
        }

        if (SceneManager.GetActiveScene().name.Equals(sceneName))
        {
            _currentScene = GameObject.FindObjectOfType<SceneBase>();
            _currentScene.Initialize();
            return;
        }
        SceneManager.LoadScene(sceneName);
    }

    private void OnLoadScene(Scene scene, LoadSceneMode mode)
    {
        _currentScene = GameObject.FindObjectOfType<SceneBase>();
        _currentScene.Initialize();
    }
}
