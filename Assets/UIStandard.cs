using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIStandard : MonoBehaviour
{
    public GameObject UIBackground;
    public GameObject UIInfo;
    public GameObject UIError;
    public GameObject UIConfirm;
    public GameObject UIPrompt;

    void Start()
    {
        UIInfo.SetActive(false);
        UIError.SetActive(false);
        UIConfirm.SetActive(false);
        UIPrompt.SetActive(false);

    }
    
    public void Info(string message, string title = "INFORMATION", string buttonCaption = "OK",Action onOK = null)
    {
        DisplayBackground();

        UIBackground.SetActive(true);
        CanvasGroup canvasGroup = UIBackground.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
        UIInfo.SetActive(true);
        UIInfo.transform.Find("Title/Text").GetComponent<Text>().text = title;
        UIInfo.transform.Find("Text").GetComponent<Text>().text = message;

        Transform button = UIInfo.transform.Find("BtnOk");
        button.transform.Find("Text").GetComponent <Text>().text = buttonCaption;
        button.GetComponent<Button>().onClick.RemoveAllListeners();
        button.GetComponent<Button>().onClick.AddListener(() =>
        {
           // CloseElement(UIInfo, onOK);
        });



    }

    private void DisplayBackground()
    {
        UIBackground.SetActive(true);
        CanvasGroup canvasGroup = UIBackground.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 0.0f;
    }


    public void Error(string message, string title = "ERROR" , string buttonCaption = "OK",Action onOK = null)
    {

    }
    public void Confirm(string message, string title = "CONFIRM", string buttonCaptionYes = "YES", string buttonCaptionNo = "NO", Action onYes = null, Action onNo = null)
    {

    }
    public void Prompt(string message, string title = "ENTER INFORMATION", string description = "", string buttonCaption = "OK", string value = "", Action<string> onValueSet = null)
    {

    }
}
