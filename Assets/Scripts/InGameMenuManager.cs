using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor.SceneManagement;
using TMPro;

public class InGameMenuManager : MonoBehaviour
{
    #region Variables
    private readonly int _answer;
    private readonly List<int> _dividingValueList = new();     //Unic Random Numbers List
    private int _divisiorNumber, _dividedNumber;
    private int _whichQuestion;
    private int _whichQuestionCount;
    private int _answerTemp;
    private int _trueAnswerTemp;
    private readonly int score1 = 5;
    private readonly int score2 = 10;
    private int _scoreTemp;
    int _wrongAnswerQuestionCount;
    bool clickTheButton = false; //Blocking the selection until the question panel arrives.

    [SerializeField]
    private GameObject PauseBtn, ResumeBtn, MenuBtn,ReplayBtn, BoxPrefab, LifeboxPrefab;

    public GameObject AlphaPanel, pauseBackground, gameBackground;

    [SerializeField]
    Transform BoxPanel, LifePanel;

    [SerializeField]
    private Transform questionPanel;

    public GameObject[] BoxArray = new GameObject[25];
    public GameObject[] LifeBoxArray = new GameObject[3];

    [SerializeField]
    private TextMeshProUGUI QuestionText;

    [SerializeField]
    private TextMeshProUGUI ScoreText, RemainingQuestionText;

    #endregion

    void Start()
    {
        questionPanel.GetComponent<RectTransform>().localScale = Vector3.zero;
        BoxCreated();
        LifeBoxCreated();
        FadeOut();
        _whichQuestionCount = 10;
        _wrongAnswerQuestionCount = 3;

    }

    #region Functions
    public void PauseGame()
    {
        pauseBackground.SetActive(true);
        gameBackground.SetActive(false);
        PauseBtn.SetActive(false);

        ResumeBtn.SetActive(true);
        if (_wrongAnswerQuestionCount == 0)
        {
            ResumeBtn.SetActive(false);
        }
        if (_whichQuestionCount == 0)
        {
            ResumeBtn.SetActive(false);
        }
        MenuBtn.SetActive(true);
        Time.timeScale = 0;
        ResumeBtn.GetComponent<CanvasGroup>().DOFade(1, 0f);
        MenuBtn.GetComponent<CanvasGroup>().DOFade(1, 0f);
    } //When the Pause button is pressed. They will be on a win-lose situation.
    public void ResumeGame()
    {
        gameBackground.SetActive(true);
        pauseBackground.SetActive(false);
        ResumeBtn.SetActive(false);
        MenuBtn.SetActive(false);
        PauseBtn.SetActive(true);
        Time.timeScale = 1;
    } //When the Resume button is pressed..
    public void ReplayGame()
    {
        gameBackground.SetActive(true);
        pauseBackground.SetActive(false);
        ResumeBtn.SetActive(false);
        MenuBtn.SetActive(false);
        PauseBtn.SetActive(true);
        EditorSceneManager.LoadScene(1);
        Time.timeScale = 1;
    } //When the Replay button is pressed..
    public void MenuGame()
    {
        gameBackground.SetActive(false);
        pauseBackground.SetActive(false);
        EditorSceneManager.LoadScene(0);
        Time.timeScale = 1;
    } //When the Menu button is pressed..
    void FadeOut()
    {
        AlphaPanel.GetComponent<CanvasGroup>().DOFade(0, 2f);
        PauseBtn.GetComponent<CanvasGroup>().DOFade(1, 2f);
    } //Animations that will occur in the in-game scene.
    public void BoxCreated()
    {
        for (int i = 0; i < BoxArray.Length; i++)
        {
            GameObject box = Instantiate(BoxPrefab, BoxPanel);
            box.transform.GetComponent<Button>().onClick.AddListener(() => OnClickButton());
            BoxArray[i] = box;
        }
        TextWriter();
        StartCoroutine(FadeRoutine());
        Invoke(nameof(QuestionPanelOpen), 1.5f);
    }  //Algorithms for generating number boxes.
    public void LifeBoxCreated()
    {
        for (int i = 0; i < LifeBoxArray.Length; i++)
        {
            GameObject lifebox = Instantiate(LifeboxPrefab, LifePanel);
            LifeBoxArray[i] = lifebox;
        }
        StartCoroutine(LifeFadeRoutine());
        Invoke(nameof(QuestionPanelOpen), 1.5f);
    } //Algorithms for generating number Lifeboxes.
    void OnClickButton()
    {
        if (clickTheButton == true)
        {
            _answerTemp = int.Parse(UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject.transform.GetChild(0).GetComponent<TMP_Text>().text);
            if (_answerTemp == _trueAnswerTemp)
            {
                QuestionText.GetComponent<TMP_Text>().fontSize = 50;
                QuestionText.text = "Congratulations, you answered the questions correctly!";
                if (_dividedNumber < 40)
                {
                    _scoreTemp += score1;
                    ScoreText.text = "SCORE = " + _scoreTemp.ToString();
                }
                else if (_dividedNumber >= 40)
                {
                    _scoreTemp += score2;
                    ScoreText.text = "SCORE = " + _scoreTemp.ToString();
                }
                AskQuestion();
                _dividingValueList.Clear();
                TextWriter();
                _whichQuestionCount--;
                RemainingQuestionText.text = "REMAINING QUESTION = " + _whichQuestionCount.ToString();
            }
            else
            {
                QuestionText.GetComponent<TMP_Text>().fontSize = 70;
                QuestionText.text = "Sorry. Wrong answer!";
                Invoke(nameof(AskQuestion), 1.5f);
                TextWriter();
                _wrongAnswerQuestionCount--;
                LifeBoxArray[_wrongAnswerQuestionCount].SetActive(false);
                if (_wrongAnswerQuestionCount == 0)
                {
                    QuestionText.GetComponent<TMP_Text>().fontSize = 70;
                    QuestionText.text = "You Lost, try again!";
                    Invoke(nameof(PauseGame), 1f);
                }
            }
            if (_whichQuestionCount == 0)
            {
                QuestionText.GetComponent<TMP_Text>().fontSize = 70;
                QuestionText.text = "Congratulations, you answered all the questions correctly!";
                Invoke(nameof(PauseGame), 1f);
            }
        }

    } //All the streams that can take place in the game.
    IEnumerator FadeRoutine()
    {
        foreach (var box in BoxArray)
        {
            box.GetComponent<CanvasGroup>().DOFade(1, 0.15f);
            yield return new WaitForSeconds(0.15f);
        }
    } //Loading animation of number boxes.
    IEnumerator LifeFadeRoutine()
    {
        foreach (var lifebox in LifeBoxArray)
        {
            lifebox.GetComponent<CanvasGroup>().DOFade(1, 0.15f);
            yield return new WaitForSeconds(0.15f);
        }
    } //Loading animation of number Lifeboxes

    private void TextWriter()
    {
        _dividingValueList.Clear();
        foreach (var box in BoxArray)
        {
            int randomvalue = Random.Range(1, 26);
            while (_dividingValueList.Contains(randomvalue))
            {
                randomvalue = Random.Range(1, 26);
            }
            _dividingValueList.Add(randomvalue);
            box.transform.GetChild(0).GetComponent<TMP_Text>().text = randomvalue.ToString();
        }
    } //Writing numbers in boxes and number generation algorithm.
    void QuestionPanelOpen()
    {
        AskQuestion();
        questionPanel.GetComponent<RectTransform>().DOScale(1, 0.5f).SetEase(Ease.OutBack);
        clickTheButton = true;
    } //Question panel opening animation.
    void AskQuestion()

    {
        QuestionText.GetComponent<TMP_Text>().fontSize = 100;
        _divisiorNumber = Random.Range(2, 11);
        _whichQuestion = Random.Range(0, _dividingValueList.Count);
        _dividedNumber = _divisiorNumber * _dividingValueList[_whichQuestion];
        _trueAnswerTemp = _dividedNumber / _divisiorNumber;
        QuestionText.text = _dividedNumber.ToString() + " : " + _divisiorNumber.ToString();
    } //Algorithm of the question to be asked in the question panel.
    #endregion


}
