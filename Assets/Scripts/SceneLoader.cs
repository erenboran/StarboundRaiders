using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public GameObject ManagerContainer;
    public string SceneToLoad;

    IEnumerator Start()
    {
        yield return new WaitForEndOfFrame();

        DontDestroyOnLoad(ManagerContainer);

        SceneManager.LoadScene(SceneToLoad);
    }
}
