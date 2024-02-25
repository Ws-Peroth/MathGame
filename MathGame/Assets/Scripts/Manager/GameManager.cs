
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    public bool isGameEnd = false;
    public bool inputLock = true;
    private StageData currentStage;
    private int currentPlayerHp;
    private int currentEnemyHp;
    public int[] currentQue;
    public int[] currentSelections;


    public void SelectStage(string id)
    {
        Debug.Log($"[GameManager] stage id = {id}");
        currentStage = DataManager.Instance.GetStageData(DataManager.DataType.StageData, id);
        SetCurrentEnemyHp(currentStage.enemyLife);
        SetCurrentPlayerHp(currentStage.playerLife);
        Debug.Log($"[GameManager] Set Hp: Enemy={currentStage.enemyLife}, Player={currentStage.playerLife}");
        SceneManagerEx.Instance.LoadScene(SceneManagerEx.Scenes.Game);
    }

    public StageData GetCurrentStageData() => currentStage;
    public int GetCurrentPlayerHp() => currentPlayerHp;
    public int GetCurrentEnemyHp() => currentEnemyHp;
    public void SetCurrentEnemyHp(int hp) => currentEnemyHp = hp;
    public void SetCurrentPlayerHp(int hp) => currentPlayerHp = hp;

    public int Calc(int a, int b, int op) => op switch
    {
        (int)DataManager.Operation.Plus => a + b,
        (int)DataManager.Operation.Minus => a - b,
        (int)DataManager.Operation.Multi => a * b,
        _ => -99999999,
    };

    public void MakeQuestions()
    {
        // int maxNum, int maxOp, int blankCount
        var maxNum = currentStage.usingMaxNum;
        var maxOp = currentStage.usingMaxOp;
        var blankCount = currentStage.blankNum;
        var ret = new int[blankCount + blankCount - 1];

        for (var i = 0; i < ret.Length; i++)
        {
            int n;
            if (i % 2 == 0)
            {
                n = Random.Range(0, maxNum + 1);
            }
            else
            {
                n = Random.Range(0, maxOp + 1);
            }
            ret[i] = n;
        }

        currentQue = ret;
        Debug.Log($"[GameManager] QueNumber=[{currentQue[0]}, {currentQue[1]}, {currentQue[2]}]");
        Debug.Log($"[GameManager] QueString=[{currentQue[0]} {DataManager.OpNumberToString(currentQue[1])} {currentQue[2]} = {Calc(currentQue[0], currentQue[2], currentQue[1])}]");
    }
    public bool IsGuessNumber() => Random.Range(0, 2) == 0;

    public int[] GetNumberCells()
    {
        List<int> ret = new();
        for (int i = 0; i < currentQue.Length; i++)
        {
            if (i % 2 == 0) ret.Add(currentQue[i]);
        }
        return ret.ToArray();
    }
    public int[] GetOpCells()
    {
        List<int> ret = new();
        for (int i = 0; i < currentQue.Length; i++)
        {
            if (i % 2 == 1) ret.Add(currentQue[i]);
        }
        return ret.ToArray();
    }

    public void MakeSlections(bool guessNumber) // isShowNumbr true = show number in selections
    {
        int[] ret = new int[] { 0, 0, 0 };
        var targetList = guessNumber ? GetNumberCells() : GetOpCells();
        foreach (var t in targetList) { Debug.Log($"target={t}"); }

        for (var i = 0; i < ret.Length; i++)
        {
            if (i < targetList.Length)
            {
                ret[i] = targetList[i];
                Debug.Log($"[GameManager] selection[{i}]=Answer:{targetList[i]}");
            }
            else
            {
                int max = guessNumber ? currentStage.usingMaxNum : currentStage.usingMaxOp;
                ret[i] = Random.Range(0, max + 1);

                Debug.Log($"[GameManager] selection[{i}]=random:{ret[i]}");
            }
        }
        if (guessNumber == false) ret = new int[] { 0, 1, 2 };
        currentSelections = ret.OrderBy(x => System.Guid.NewGuid()).ToArray();

        Debug.Log($"[GameManager] Selections=[{currentSelections[0]}, {currentSelections[1]}, {currentSelections[2]}]");
    }

    public void GameOver()
    {
        isGameEnd = true;
        GlobalEventController.Instance.SendEvent("Effect", "Clear", new object[] { "STAGE\nFAIL ", new Color32(215, 0, 33, 255) });
    }

    public void SendAnswer(int answer)
    {
        if (answer == Calc(currentQue[0], currentQue[2], currentQue[1]))
        {
            currentEnemyHp--;
            GlobalEventController.Instance.SendEvent("Effect", "Hit", new object[] { false });

            if (currentEnemyHp == 0)
            {
                // CLEAR
                isGameEnd = true;
                Debug.Log($"Save Clear Data: stage{currentStage.id} => {currentPlayerHp}");
                DataManager.Instance.SetStageClearHp(currentPlayerHp, currentStage.id);
                var nextStage = DataManager.Instance.GetNextStageId(currentStage.id);
                Debug.Log($"Open Next Stage: stage{nextStage}");
                if (nextStage != "-1") DataManager.Instance.SetStageUnlock(true, nextStage);
                GlobalEventController.Instance.SendEvent("Effect", "Clear", new object[] { "STAGE\nCLEAR ", new Color32(126, 255, 135, 255) });
            }
        }
        else
        {
            GlobalEventController.Instance.SendEvent("Effect", "Hit", new object[] { true });
            if (currentPlayerHp > 0)
            {
                currentPlayerHp--;
            }
            if (currentPlayerHp == 0)
            {
                // FAIL
                GameOver();
            }
        }
    }
}
