using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectAnswerArea : MonoBehaviour
{
    public NumberCell cellPrefab;
    public List<NumData> ansNums = new List<NumData>();
    private bool isGuessNumber;
    public void CellClear()
    {
        foreach (var cell in ansNums)
        {
            Destroy(cell.cell.gameObject);
        }
        ansNums = new List<NumData>();
    }
    public void Init(int[] selections, bool isGuessNumber)
    {
        this.isGuessNumber = isGuessNumber;
        CellClear();



        for (var i = 0; i < selections.Length; i++)
        {
            var cell = Instantiate(cellPrefab, Vector2.zero, Quaternion.identity, transform);
            cell.Init(i, "", selections[i]);
            if (isGuessNumber)
            {
                cell.text.text = $"{selections[i]}";
            }
            else if (!isGuessNumber)
            {
                cell.text.text = DataManager.OpNumberToString(selections[i]);
                Debug.Log($"Init OpText: {DataManager.OpNumberToString(selections[i])}");
            }
            ansNums.Add(new NumData(cell));
            cell.button.onClick.AddListener(() =>
            {
                Debug.Log($"Select Number Btn: {cell.id}");
                GlobalEventController.Instance.SendEvent("Select", "Number", new object[] { ansNums[cell.id] });
            });

        }
    }

    public void UpdateCellData(NumData cellData, string text)
    {
        var index = cellData.cell.id;
        ansNums[index].selectId = cellData.selectId;
        ansNums[index].cell.text.text = text == "" || isGuessNumber ? text : DataManager.OpNumberToString(int.Parse(text));
    }
}

public class NumData
{
    public int selectId; // inputedCellIndex
    public NumberCell cell; // object

    public NumData(NumberCell cell, int selectId = -1)
    {
        this.cell = cell;
        this.selectId = selectId;
    }
}