using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberCell : MonoBehaviour
{
    public int id; // index
    public Image image;
    public Text text;
    public Button button;
    public int data;

    public void Init(int id, string text, int data, string path = "", int imgIndex = 0)
    {
        this.id = id;
        this.text.text = text;
        this.data = data;
        if (path == "") return;
        image.sprite = AssetDownloadManager.Instance.GetAssetsWithPath<Sprite>(path)[imgIndex];
    }
}
