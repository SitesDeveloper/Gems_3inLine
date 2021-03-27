using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class myUtils : MonoBehaviour
{

    //=======================================================================
    //аналог debug.log для нескольких параметров
    public static void console_log(params object[] list)
    {
        string res = "";
        foreach (object item in list)
        {
            res = res + item.ToString() + " ";
        }
        Debug.Log(res);
    }


    //***********************************************************************
    //разбить строку (stroka) данными символами(ch) и вернуть id с заданным номером (indx, начиная с 0)
    //примеры:  category_<id>,  <id>: name
    public static int SplitStringAndGetFromIndx(string stroka, char[] ch = null, int indx=0)
    {
        if (ch == null)
            ch = new char[] { '_' }; 

        string[] words = stroka.Split(ch, StringSplitOptions.RemoveEmptyEntries);
        int id = 0; ;
        if ((words.Length >= indx) && Int32.TryParse(words[indx], out id))
        {
            return id;
        }
        return 0;
    }


    //=======================================================================
    //поиск всех неактивных объектов
    public static List<GameObject> FindInactiveGameObjects()
    {
        //Get all of them in the scene
        GameObject[] all = GameObject.FindObjectsOfType<GameObject>();
        //Create a list 
        List<GameObject> objs = new List<GameObject>();
        foreach (GameObject obj in all) 
        {
            objs.Add(obj);
        }
        //Create the Finder
        Predicate<GameObject> inactiveFinder = new Predicate<GameObject>((GameObject go) => { return !go.activeInHierarchy; });
        //And find inactive ones
        List<GameObject> results = objs.FindAll(inactiveFinder);
        return results;
    }

    public static GameObject FindObjByTag(string tag)
    {
        List<GameObject> all = FindInactiveGameObjects();
        foreach (GameObject obj in all) //Create a list 
        {
            if (obj.tag == tag)
                return obj;
        }
        return null;
    }




    //=======================================================================
    //получить рандомное число от енума, начиная с min-ключа по порядку
    public static T GetRandomEnum<T>(int min=0)
    {
        System.Array A = System.Enum.GetValues(typeof(T));
        T V = (T)A.GetValue(UnityEngine.Random.Range(min, A.Length));
        return V;
    }

    //=======================================================================
    public static AllUsersSaveData getAllUsersSaveData() {
        //adAllText(GameConstants.PATH_TO_USERS_SAVEDATA));
        //StreamReader strRead = new StreamReader( PathUtil.GetDocumentFilePath(GameConstants.PATH_TO_USERS_SAVEDATA) );
        //string json = strRead.ReadToEnd();
        //strRead.Close();
        //AllUsersSaveData saveData = JsonUtility.FromJson<AllUsersSaveData>(json);
        AllUsersSaveData saveData;

        string fname = PathUtil.GetDocumentFilePath(GameConstants.PATH_TO_USERS_SAVEDATA);
        string json = "";
        if (File.Exists(fname))
        {
             json = File.ReadAllText(fname);
        }
        //GameObject.Find("txt_dbg").GetComponent<Text>().text = "json=" + json + json.Length.ToString();
        if (json.Length > 0) {
            saveData = JsonUtility.FromJson<AllUsersSaveData>(json);
        } else { 
            saveData = new AllUsersSaveData();
        }
        return saveData;
    }

    //=======================================================================
    public static void saveAllUsersSaveData(AllUsersSaveData saveData)
    {
        string fname = PathUtil.GetDocumentFilePath(GameConstants.PATH_TO_USERS_SAVEDATA);
        File.WriteAllText(fname, JsonUtility.ToJson(saveData));
        
    }
    //=======================================================================
    //получить данные текущего юзера по его имени (возвращает объект юзера или null)
    public static OneUserSaveData GetCurUserSaveData()
    {
        AllUsersSaveData saveData = getAllUsersSaveData();
        if (saveData != null)
        {
            if (string.IsNullOrEmpty(saveData.curUserName))
            {
                return null;
            }
            foreach (OneUserSaveData user in saveData.arUsers)
            {
                if (saveData.curUserName == user.name)
                {
                    return user;
                }
            }
        }
        return null;
    }
    //=======================================================================
    //получить данные юзера по его имени (возвращает объект юзера или null)
    public static OneUserSaveData GetUserSaveData_byName(string user_name)
    {
        AllUsersSaveData saveData = getAllUsersSaveData();
        if ((saveData != null) && !string.IsNullOrEmpty(user_name))
        {
            foreach (OneUserSaveData user in saveData.arUsers)
            {
                if (user_name == user.name)
                {
                    return user;
                }
            }
        }
        return null;
    }
    //=======================================================================
    // сохранить данные текущего юзера (перезапись/вставка в файл сохранений)
    public static bool SaveUserData(OneUserSaveData user)
    {
        if ((user == null) || string.IsNullOrEmpty(user.name))
            return false;
        AllUsersSaveData saveData = getAllUsersSaveData();
        bool is_exists = false;
        if (saveData.arUsers.Length > 0)
        {
            //поиск юзера в массиве юзеров
            for (int i = 0; i < saveData.arUsers.Length; i++)
            {
                if (user.name == saveData.arUsers[i].name)
                {
                    saveData.arUsers[i] = user;
                    is_exists = true;
                }
            }
        }
        if (!is_exists)
        {
            //добавление юзера, если такого нету
            List<OneUserSaveData> aUsers = new List<OneUserSaveData>(saveData.arUsers);
            aUsers.Add(user);
            saveData.arUsers = aUsers.ToArray();
        }
        saveData.curUserName = user.name;
        File.WriteAllText(  PathUtil.GetDocumentFilePath( GameConstants.PATH_TO_USERS_SAVEDATA ), JsonUtility.ToJson(saveData));
        return true;
    }



    //=======================================================================
    public static OneSquare create_OneSquare( Vector3 crd, Transform parent, Vector3 scale, int brick_type = -1, string animation = GameConstants.sANIM_BASE)
    {
        OneSquare square = new OneSquare();
        square.Create(crd, parent, scale, brick_type, animation);
        /*
        if (brick_type < 0) {
            brick_type = (int)UnityEngine.Random.Range(0f, 5f);
        }
        square.type = brick_type;
        square.brick_name = "Brick_" + square.type.ToString();
        string path_to_prefab = "Prefab/bricks/" + square.brick_name;
        GameObject prefab = (GameObject)Instantiate(Resources.Load(path_to_prefab));  //, parent
        prefab.transform.SetParent(parent);
        prefab.transform.localPosition = crd;
        prefab.transform.localScale = scale;

        Animator anim = prefab.GetComponent<Animator>();
        //myUtils.console_log(animation);
        anim.runtimeAnimatorController = (RuntimeAnimatorController) Instantiate(Resources.Load(animation)) as RuntimeAnimatorController;
        anim.Play("Idle");
        */
        return square;
    }


    //=======================================================================
    //отправка запроса на сервер
    public void SendRQ()
    {
        StartCoroutine(loadData());
    }


    //=======================================================================
    IEnumerator loadData()
    {
        WWWForm form = new WWWForm();
        form.AddField("action", "test_unity");
        form.AddField("name1", "Сергей Романов");

        UnityWebRequest www = UnityWebRequest.Post("http://test.bible-code.ru/obetovania/ajax", form);
        yield return www.SendWebRequest();// www.Send();

        if (www.isNetworkError)  //www.isError
        {
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("Form upload complete!");
            Debug.Log(www.downloadHandler.text);
            RequstDataList result = RequstDataList.CreateFromJSON(www.downloadHandler.text);
            Debug.Log(result);
            Debug.Log(www.downloadHandler.data);
        }
    }


}
