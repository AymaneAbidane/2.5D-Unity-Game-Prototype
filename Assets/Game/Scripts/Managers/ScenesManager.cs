using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    public static ScenesManager Instance { get; private set; }

    private const string BATTLE_SCENE_NAME = "battleScene";
    private const string OPEN_WORLD_SCENE = "OpenWorldScene";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)
        {
            Destroy(this);
        }
        DontDestroyOnLoad(this);
    }

    public void LoadBattleScene()
    {
        SceneManager.LoadScene(BATTLE_SCENE_NAME);
    }

    public void LoadOpenWorldScene()
    {
        SceneManager.LoadScene(OPEN_WORLD_SCENE);
    }
}
