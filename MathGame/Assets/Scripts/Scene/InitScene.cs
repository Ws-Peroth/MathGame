using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitScene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManagerEx.Instance.LoadScene(SceneManagerEx.Scenes.Title);
    }
}
