using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// 1. Add Data Class in here
#region DATA_DEFINED

[Serializable]
public class DefaultData
{
    public string id;
    public string name;
    public int data;

    public static DefaultData Dummy()
    {
        return new DefaultData("", "", 0);
    }

    public DefaultData(string id, string name, int data)
    {
        this.id = id;
        this.name = name;
        this.data = data;
    }
}
[Serializable]
public class StageData
{
    // When stage select
    // - get lock/unlock stage
    // - lock: lock icon
    // - unlock: show cleared Heart : if not cleared clearedHeart = 0

    public string id = "";
    public bool isUnlocked = false;
    public bool isCleared = false;
    public int clearedHeart = 0;
    public int usingMaxNum = 0;
    public int usingMaxOp = 0; // 0:+,  1:-,  2:*,  3:/
    public int blankNum = 2;
    public int enemyLife = 3;
    public int playerLife = 3;

    public static StageData Dummy()
    {
        return new StageData("", false, false, 0, 0, 0, 2, 3, 3);
    }

    public StageData(string id, bool isUnlocked, bool isCleared, int clearedHeart, int usingMaxNum, int usingMaxOp, int blankNum, int enemyLife, int playerLife)
    {
        this.id = id;
        this.isUnlocked = isUnlocked;
        this.isCleared = isCleared;
        this.clearedHeart = clearedHeart;
        this.usingMaxNum = usingMaxNum;
        this.usingMaxOp = usingMaxOp;
        this.blankNum = blankNum;
        this.enemyLife = enemyLife;
        this.playerLife = playerLife;
    }
}

#endregion

// 2. Add Datas in JsonData
#region ADD_JSONDATA

[Serializable]
public class JsonData
{
    public List<StageData> stageDatas;


    public void Init(
        List<StageData> stageDatas
        )
    {
        this.stageDatas = stageDatas;
    }
}

# endregion

public class DataManager : Singleton<DataManager>
{
    private JsonData MstData;
    public JsonData GetMstData() { return MstData; }
    public bool RESET_JSON_ON = false;

// 3. Add Data Type Enums in here
#region DATA_TYPES

    public enum DataType
    {
        StageData
    }

    public enum Operation
    {
        Plus,
        Minus,
        Multi
    }

#endregion

    public void SaveMstData() => JsonManager.Instance.SaveJsonData(MstData, Path.Combine(Application.persistentDataPath, "mstData.json"));


    public void Start()
    {
        Debug.Log(Path.Combine(Application.persistentDataPath, "mstData.json"));
        MstData = null;
        MstData = JsonManager.Instance.LoadJsonData<JsonData>(Path.Combine(Application.persistentDataPath, "mstData.json"));
        if (MstData == null || RESET_JSON_ON)
        {
            // Data Init

            MstData = new JsonData();

            var stageDatas = new List<StageData> {
                // StageID, IsUnlockedStage, IsClearedStage, ClearHeart, MaxNum, MaxOp, BlankNumCnt, EnemyLife, PlayerLife
                //            id, unlock, clear,hp, Numlim, usingMaxOp, blankNum, enemyLife, playerLife
                new StageData("0", true, false, 0, 3, (int)Operation.Plus, 2, 1, -1), // -1 = tutorial stage
                new StageData("1", false, false, 0, 3, (int)Operation.Plus, 2, 3, 3),
                new StageData("2", false, false, 0, 6, (int)Operation.Plus, 2, 3, 3),
                new StageData("3", false, false, 0, 10, (int)Operation.Plus, 2, 5, 3),
                new StageData("4", false, false, 0, 3, (int)Operation.Minus, 2, 5, 3),
                new StageData("5", false, false, 0, 6, (int)Operation.Minus, 2, 7, 3),
                new StageData("6", false, false, 0, 10, (int)Operation.Minus, 2, 7, 3),
                new StageData("7", false, false, 0, 3, (int)Operation.Multi, 2, 9, 3),
                new StageData("8", false, false, 0, 6, (int)Operation.Multi, 2, 10, 3),
                new StageData("9", false, false, 0, 10, (int)Operation.Multi, 2, 12, 3),
                new StageData("10", false, false, 0, 10, (int)Operation.Multi, 2, 15, 3),
            };

            MstData.Init(
                stageDatas: stageDatas
            );
            JsonManager.Instance.SaveJsonData(MstData, Path.Combine(Application.persistentDataPath, "mstData.json"));
            MstData = JsonManager.Instance.LoadJsonData<JsonData>(Path.Combine(Application.persistentDataPath, "mstData.json"));
        }

        var data = JsonManager.Instance.ReadJson(Path.Combine(Application.persistentDataPath, "mstData.json"));
        Debug.Log(data);
    }
    

    public DefaultData GetDefaultData(DataType type, string id)
    {
        // Find Data with id
        // if (type == DataType.Type) { return GetMstData().targetData.Find((m) => m.id == id); }

        // Else case
        Debug.LogError($"Type Undefined: {type} is not defined in Default Data Types");
        return null;
    }

    public StageData GetStageData(DataType type, string id)
    {
        // Find Data with id
        if (type == DataType.StageData) { return GetMstData().stageDatas.Find((m) => m.id == id); }

        // Else case
        Debug.LogError($"Type Undefined: {type} is not defined in Default Data Types");
        return null;
    }
    public void SetStageClearHp(int clearHp, string id)
    {
        clearHp = clearHp == -1 ? 3 : clearHp;
        // Find Data with id
        var data = GetStageData(DataType.StageData, id);
        if(data == null)
        {
            Debug.LogError($"[DataManager] Stage Data Not Found. id={id}");
            return;
        }
        data.isCleared = true;
        data.clearedHeart = Math.Max(data.clearedHeart, clearHp);
        SaveMstData();
    }

    public void SetStageUnlock(bool isUnlock, string id)
    {
        // Find Data with id
        var data = GetStageData(DataType.StageData, id);
        if (data == null)
        {
            Debug.LogError($"[DataManager] Stage Data Not Found. id={id}");
            return;
        }
        data.isUnlocked = isUnlock;
        SaveMstData();
    }

    public string GetNextStageId(string currentId)
    {
        var currentIndex = int.Parse(currentId);
        var length = GetMstData().stageDatas.Count;
        if(length <= currentIndex + 1)
        {
            // Last Stage
            return "-1";
        }
        return $"{currentIndex + 1}";
    }

    public static string OpNumberToString(int op) => op switch
    {
        (int)Operation.Plus => "+",
        (int)Operation.Minus => "-",
        (int)Operation.Multi => "x",
        _ => "Error",
    };
}