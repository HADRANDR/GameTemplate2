using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor.SceneManagement;

public class MenuManagerScript : MonoBehaviour
{
    #region Variables
    [SerializeField]
    private GameObject startBtn, exitBtn;
    #endregion
    
    void Start()
    {
        FadeOut();
    }

    #region Functions
    void FadeOut()
    {
        startBtn.GetComponent<CanvasGroup>().DOFade(1, 0.8f);
        exitBtn.GetComponent<CanvasGroup>().DOFade(1, 0.8f).SetDelay(0.5f);
    } //Animation of buttons appearing on the screen
    public void ExitGame()
    {
        Application.Quit();
    }
    public void StartGame()
    {
        EditorSceneManager.LoadScene(1);
    }
    #endregion

}
