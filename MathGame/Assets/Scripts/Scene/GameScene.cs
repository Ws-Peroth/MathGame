using DG.Tweening;
using DG.Tweening.Core;
using DG.Tweening.Core.Easing;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameScene : MonoBehaviour, IGlobalEventReceiver
{
    public IEnumerator HitEffect(Vector2 pos)
    {
        Debug.Log("Play Effect");
        List<ParticleSystem> temp = new List<ParticleSystem>();
        for (var i = 0; i < 3; i++)
        {
            temp.Add(Instantiate(hitPrefab, pos, Quaternion.identity, effects));
            hitPrefab.Play();
            yield return new WaitForSeconds(0.15f);
        }
        foreach(var t in temp)
        {
            Destroy(t.gameObject);
        }
    }

    [SerializeField] ParticleSystem hitPrefab;
    [SerializeField] private Transform effects;
    [SerializeField] private Image systemNotice;
    [SerializeField] private Text systemNoticeText;
    [SerializeField] private Text bossHpText;
    [SerializeField] private Text bossNumberText;
    [SerializeField] private List<Image> playerHeart;
    [SerializeField] private ShowQuestionArea showQuestionArea;
    [SerializeField] private SelectAnswerArea selectNumberArea;
    [SerializeField] private Button doneButton;
    [SerializeField] private Button returnToStageButton;
    [SerializeField] private Text answerNumberText;

    [SerializeField] private Image enemyImage;
    [SerializeField] private Image playerImage;
    [SerializeField] private Image bgImage;
    [SerializeField] private Image bushImage;
    [SerializeField] private Image groundImage;

    readonly string[] EventIds = new string[]
    {
        "Select",
        "Effect",
    };

    #region ImplementIGlobalEventReceiver
    private void OnEnable()
    {
        if (this is IGlobalEventReceiver Interface)
        {
            Interface.Regist(Interface, EventIds);
        }
    }
    private void OnDestroy()
    {
        if (this is IGlobalEventReceiver Interface)
        {
            Debug.Log("OnDestroy: Unregist()");
            Debug.Log($"Manager is null = {GlobalEventController.Instance == null}");
            Interface.Unregist(Interface, EventIds);
        }
    }

    public IEnumerator StageChangeEffect(string text)
    {
        yield return new WaitForSeconds(1);
        for(int i = 0; i < text.Length; i++)
        {
            systemNoticeText.text = $"{text.Substring(0, i)}";
            yield return new WaitForSeconds(0.15f);
        }

        yield return new WaitForSeconds(1);
        SceneManagerEx.Instance.LoadScene(SceneManagerEx.Scenes.Stage);

    }

    void IGlobalEventReceiver.ReceiveEvent(string EventId, string name, object[] param)
    {
        if (EventId == "Effect")
        {
            if (name == "Hit")
            {
                bool isPlayerHit = (bool)param[0];
                StartCoroutine(nameof(HitEffect), isPlayerHit ? new Vector2(-1.85f, -0.13f) : new Vector2(1.28f, 3.13f));

            }
            if (name == "Clear")
            {
                Debug.Log("Effect Clear");

                systemNotice.gameObject.SetActive(true);

                var origin = systemNotice.color;
                origin.a = 0;
                systemNotice.color = origin;
                systemNoticeText.text = "";
                systemNoticeText.color = (Color32)param[1];
                StartCoroutine(nameof(StageChangeEffect), param[0] as string);
                systemNotice.DOFade(0.9f, 1);
            }
        }
        else if (EventId == "Select")
        {
            if(name == "Number")
            {
                if (GameManager.Instance.inputLock) return;

                var numData = param[0] as NumData;
                if (numData.selectId == -1)
                {
                    var blankCells = showQuestionArea.GetBlankCells();
                    if (blankCells == null || blankCells.Count == 0) return;

                    showQuestionArea.UpdateCellData(blankCells[0].id, numData.cell.data);
                    showQuestionArea.UpdateCellText(blankCells[0].id, $"{blankCells[0].data}");

                    numData.selectId = blankCells[0].id;
                    selectNumberArea.UpdateCellData(numData, "");
                }
                else
                {
                    var removeCell = showQuestionArea.GetNumberCell(numData.selectId);
                    showQuestionArea.UpdateCellData(removeCell.id, -1);
                    showQuestionArea.UpdateCellText(removeCell.id, "?");

                    numData.selectId = -1;
                    selectNumberArea.UpdateCellData(numData, $"{numData.cell.data}");
                }

                var checkBlankCell = showQuestionArea.GetBlankCells();
                if (checkBlankCell == null || checkBlankCell.Count == 0)
                {
                    // Show Done Button
                    doneButton.gameObject.SetActive(true);
                }
                else
                {
                    // Hide Done Button
                    doneButton.gameObject.SetActive(false);
                }
            }
        }
    }

    object IGlobalEventReceiver.GetOriginObject()
    {
        return this;
    }
    #endregion

    // Start is called before the first frame update
    void Start()
    {
        int stageNumber = int.Parse(GameManager.Instance.GetCurrentStageData().id);
        int bgNumber = Mathf.Min(Mathf.Max(stageNumber / 3, 1), 3);
        bgImage.sprite = AssetDownloadManager.Instance.GetAssetsWithPath<Sprite>($"bg{bgNumber}")[0];
        bushImage.sprite = AssetDownloadManager.Instance.GetAssetsWithPath<Sprite>($"floor{bgNumber}")[0];
        groundImage.sprite = AssetDownloadManager.Instance.GetAssetsWithPath<Sprite>($"floor{bgNumber}")[1];
        enemyImage.sprite = AssetDownloadManager.Instance.GetAssetsWithPath<Sprite>($"boss{stageNumber}")[0];

        Debug.Log($"bgImage=bg{Mathf.Max(Mathf.Min(stageNumber / 3, 1), 3)}");
        
        Debug.Log($"enemyImage=boss{stageNumber}");

        systemNotice.gameObject.SetActive(false);
        returnToStageButton.onClick.AddListener(() =>
        {
            // Do Something
            if (GameManager.Instance.inputLock) return;
            GameManager.Instance.inputLock = true;

            GameManager.Instance.GameOver();
        });
        doneButton.onClick.AddListener(() =>
        {
            // Do Something
            if (GameManager.Instance.inputLock) return;

            GameManager.Instance.inputLock = true;
            var answer = showQuestionArea.CalcAnswer();
            answerNumberText.text = $"{answer}";

            GameManager.Instance.SendAnswer(answer);
            Invoke(nameof(Init), 0.5f);
        });

        Init();
    }

    public void Init()
    {
        GameManager.Instance.MakeQuestions();
        showQuestionArea.CellClear();
        selectNumberArea.CellClear();

        doneButton.gameObject.SetActive(false);
        bossNumberText.text = "";
        answerNumberText.text = "?";
        bossHpText.text = $"x{GameManager.Instance.GetCurrentEnemyHp()}";
        InitPlayerHp(GameManager.Instance.GetCurrentPlayerHp());

        Debug.Log($"x{GameManager.Instance.GetCurrentEnemyHp()}");

        if (GameManager.Instance.isGameEnd) return;
        Invoke(nameof(ShowEnemyNumber), 0.5f);
    }

    public void InitPlayerHp(int hp)
    {
        for (var i = 0; i < playerHeart.Count; i++)
        {
            var path = i < hp || hp == -1 ? "heart" : "deadheart";
            playerHeart[i].sprite = AssetDownloadManager.Instance.GetAssetsWithPath<Sprite>(path)[0];
        }
    }

    

    public void ShowEnemyNumber()
    {
        var q = GameManager.Instance.currentQue;
        var bossAns = GameManager.Instance.Calc(q[0], q[2], q[1]);
        bossNumberText.text = $"{bossAns}";
        Invoke(nameof(ShowPlayerControlArea), 0.5f);
    }

    public void ShowPlayerControlArea()
    {
        bool guessNumber = GameManager.Instance.IsGuessNumber();
        Debug.Log($"[GameScene] guessNumber={(guessNumber ? "true" : "false")}");
        GameManager.Instance.MakeSlections(guessNumber);
        selectNumberArea.Init(GameManager.Instance.currentSelections, guessNumber);
        showQuestionArea.Init(GameManager.Instance.currentQue, guessNumber);

        GameManager.Instance.inputLock = false;
    }
}
