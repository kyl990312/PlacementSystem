using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameSettings
{
    public SceneSettings sceneSettings = new SceneSettings();
}

public class GameManager : MonoSingleton<GameManager>
{
    public GameSettings settings;
    
    private SceneLoader _sceneManager;
    private PageManager _pageManager;

    public SceneLoader SceneManager => _sceneManager;
    public PageManager PageManager => _pageManager;

    public void Initialize()
    {
        _sceneManager = new SceneLoader(settings.sceneSettings);
        _pageManager = new PageManager();
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void Quit()
    {
        Application.Quit();
    }
}
