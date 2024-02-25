using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerEx : Singleton<SceneManagerEx>
{
    public enum Scenes
    {
        // Add Scenes by Build Index
        init,
        Title,
        Stage,
        Game,
    };

    public void LoadScene(Scenes scene)
    {
        GameManager.Instance.isGameEnd = false;
        SceneManager.LoadScene((int)scene);
    }
}