using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    public GameSettings gameSettings;

    private void Awake()
    {
        GameManager.Instance.settings = gameSettings;
        GameManager.Instance.Initialize();

        GameManager.Instance.SceneManager.LoadScene(gameSettings.sceneSettings.defaultScene);

        DestroyImmediate(gameObject);
    }
}
