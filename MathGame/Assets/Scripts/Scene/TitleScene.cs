using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScene : MonoBehaviour
{
    [SerializeField] Button startButton;

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(() => SceneManagerEx.Instance.LoadScene(SceneManagerEx.Scenes.Stage));
    }
}
