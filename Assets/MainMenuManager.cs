using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class MainMenuManager : MonoBehaviour
{
    public GameObject loadingPanel; // Yükleme ekranı paneli
    public Slider loadingBar; // Yükleme ilerleme çubuğu
    public TMP_Text loadingText; // Yüzdelik gösterge

    void Start()
    {
        loadingPanel.SetActive(false); // Yükleme ekranını başlangıçta gizle
    }

    public void PlayGame()
    {
        loadingPanel.SetActive(true); // Yükleme ekranını aç
        StartCoroutine(LoadGameScene());
    }

    IEnumerator LoadGameScene()
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync("Game");
        operation.allowSceneActivation = false; // Tam yüklenmeden sahneyi açma

        while (!operation.isDone)
        {
            float progress = Mathf.Clamp01(operation.progress / 0.9f); // İlerlemeyi hesapla
            loadingBar.value = progress;
            loadingText.text = "Loading... " + (progress * 100).ToString("F0") + "%";

            if (operation.progress >= 0.9f)
            {

                    operation.allowSceneActivation = true;
                
            }
            yield return null;
        }
    }
}
