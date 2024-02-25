using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageScene : MonoBehaviour
{
    [SerializeField] List<StageButton> stages = new List<StageButton>();

    void Start()
    {
        var stageData = DataManager.Instance.GetMstData().stageDatas;
        var index = 0;
        for (; index < stageData.Count; index++)
        {
            if (stages.Count == index)
            {
                Debug.LogError($"[StageScene] Add Stage Button : Stage {index} Button is null");
                break;
            }
            Debug.Log($"Init Stage Data: id={index}");
            stages[index].Init(stageData[index]);
        }
        while(index < stages.Count)
        {
            Debug.LogError($"[StageScene] Add Stage Data : Stage {index} Data is null");
            stages[index].Init(StageData.Dummy());
            index++;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
