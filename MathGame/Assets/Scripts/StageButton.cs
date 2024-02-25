using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StageButton : MonoBehaviour
{
    public List<Image> starImages = new List<Image>();
    public Button stageBtn;
    public string id;

    public void Init(StageData initData)
    {
        id = initData.id;
        stageBtn = gameObject.GetComponent<Button>();
        for(var i = 0; i < starImages.Count; i++)
        {
            var index = i < initData.clearedHeart ? 0 : 1;
            Debug.Log($"Stage{i}: Get Star IMG[{index}]");
            starImages[i].sprite = AssetDownloadManager.Instance.GetAssetsWithPath<Sprite>("star")[index];
        }
        gameObject.SetActive(initData.isUnlocked);
        stageBtn.onClick.AddListener(() => GameManager.Instance.SelectStage(id));
    }
}
