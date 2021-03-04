using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class SceneLoaderWithProgressBar : MonoBehaviour
{

    //public GameObject LoadingScene;
    //public Image LoadingBar;
    public TMP_Text textPourcentage; //The text with the percentage increasing
    public Slider LoadingBar;

    private string nameScene;
    private bool estActive = false;

    public void Load_Scene(string name)
    {
        nameScene = name;
        estActive = true;
        textPourcentage.transform.parent.gameObject.SetActive(true);
    }

    void Update()
    {
        if (estActive)
        {
            StartCoroutine(LevelCoroutine(nameScene));
            estActive = false;
        }
    }


    IEnumerator Pause()
    {
        yield return new WaitForSeconds(0.5f);
    }

    IEnumerator LevelCoroutine(string namScene)
    {

        //LoadingScene.SetActive(true);
        AsyncOperation async = SceneManager.LoadSceneAsync(namScene);

        //async.allowSceneActivation = false;

        while (!async.isDone)
        {

            float pourcentage = 0;
            myUtils.console_log("progress ", async.progress);

            float val = Mathf.Clamp01(async.progress / 0.9f);
            LoadingBar.value = val;
            pourcentage = val * 100;
            textPourcentage.text = (int)pourcentage + "%";

            //Thread.Sleep(500);
            yield return null;

        }

        //async.allowSceneActivation = true;
        textPourcentage.transform.parent.gameObject.SetActive(false);


    }
}
