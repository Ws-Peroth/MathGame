using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShowQuestionArea : MonoBehaviour
{
    [SerializeField] List<NumberCell> cellList = new List<NumberCell>();
    [SerializeField] NumberCell numberPrefab;
    [SerializeField] NumberCell OpPrefab;
    private bool isGuessNumber;

    public NumberCell GetNumberCell(int index) => cellList[index];
    public List<NumberCell> GetNumberCells() => cellList.Where(x => x.id % 2 == 0).ToList();
    public List<NumberCell> GetOpCells() => cellList.Where(x => x.id % 2 == 1).ToList();
    public List<NumberCell> GetBlankCells() => cellList.Where(x => x.data == -1).ToList();

    public int CalcAnswer() => GameManager.Instance.Calc(cellList[0].data, cellList[2].data, cellList[1].data);
    

    public void CellClear()
    {
        foreach (var cell in cellList)
        {
            Destroy(cell.gameObject);
        }
        cellList = new List<NumberCell>();
    }

    public void UpdateCellText(int index, string text)
    {
        string temp;
        if(text == "?")
        {
            temp = text;
        }
        else if (index % 2 == 0)
        {
            temp = text;
        }
        else
        {
            temp = DataManager.OpNumberToString(int.Parse(text));
        }
        cellList[index].text.text = temp;
    }
    public void UpdateCellData(int index, int data) => cellList[index].data = data;
    public void Init(int[] que, bool isGuessNumber)
    {
        this.isGuessNumber = isGuessNumber;
        CellClear();

        for (var i = 0; i < que.Length; i++)
        {
            if(i % 2 == 0)
            {
                var cell = Instantiate(numberPrefab, Vector2.zero, Quaternion.identity, transform);
                if (isGuessNumber) cell.Init(i, "?", -1); 
                else cell.Init(i, $"{que[i]}", que[i]);
                cellList.Add(cell);
            }
            else if(i % 2 == 1)
            {
                var cell = Instantiate(OpPrefab, Vector2.zero, Quaternion.identity, transform);
                if (isGuessNumber) cell.Init(i, DataManager.OpNumberToString(que[i]), que[i]);
                else cell.Init(i, "?", -1);
                cellList.Add(cell);
            }
        }
    }
}
