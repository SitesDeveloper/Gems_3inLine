using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class gameSceneLoader : MonoBehaviour
{

    //public GameObject LoadingScene;
    //public Image LoadingBar;
    public Text textPourcentage; //The text with the percentage increasing
    public Slider LoadingBar;

    private string nameScene;
    private bool estActive = false;

    public void Load_Scene(string name)
    {
        nameScene = name;
        estActive = true;
        textPourcentage.transform.parent.gameObject.SetActive(true);

        //textPourcentage.GetComponentInParent<GameObject>().SetActive(true);
        //LoadingBar.gameObject.SetActive(true);
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
        yield return new WaitForSeconds(0.2f);
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

            /*
            if (async.progress < 0.9f)
            {
                //LoadingBar.fillAmount = async.progress / 0.9f;
                LoadingBar.value = async.progress / 0.9f;
                pourcentage = async.progress * 100;
                textPourcentage.text = (int)pourcentage + "%";
                //yield return new WaitForSeconds(0.2f);
            }
            else
            {
                LoadingBar.value = async.progress / 0.9f;
                pourcentage = (async.progress / 0.9f) * 100;
                textPourcentage.text = (int)pourcentage + "%";
                //yield return new WaitForSeconds(0.30f);
                async.allowSceneActivation = true;
                textPourcentage.transform.parent.gameObject.SetActive(false);
                //textPourcentage.GetComponentInParent<GameObject>().SetActive(false);
                //textPourcentage.gameObject.SetActive(false);
                //LoadingBar.gameObject.SetActive(false);
                Debug.Log("scene active");

            }
            */
            yield return null;

        }

        //async.allowSceneActivation = true;
        textPourcentage.transform.parent.gameObject.SetActive(false);


    }
}
