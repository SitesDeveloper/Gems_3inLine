using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class DialogSelectUser : MonoBehaviour
{

    //для скрытия при продолжении
    public GameObject pnlSelectUser; 
    public InputField inpUserName;
    ///public GameObject UsersSrollContent;
    public ColorBlock btn_defaultColorBlock = ColorBlock.defaultColorBlock;
    public GameObject pnl_menu;

    List<OneUserSaveData> arUsers;
    AllUsersSaveData allUsers;

    private void Start()
    {
        Reload_AllUsersData();
        SetInputName("");
    }

    private void Update()
    {

        GameObject itemsArea = GameObject.FindGameObjectWithTag("users_list");
        if ( (itemsArea!=null) && (itemsArea.GetComponentsInChildren<Button>().Length > 0) )
        {
            Button[] btns = itemsArea.GetComponentsInChildren<Button>();
            foreach (Button b in btns)
            {
                if (b)
                    b.transform.localScale = new Vector3(1f, 1f, 1f);
            }
        }

    }


    public void SetInputName(string new_user_name) {
        inpUserName.text = new_user_name;
        allUsers.curUserName = new_user_name;
    }

    public string GetInputName() {
        return inpUserName.text;
    }


    public void Reload_AllUsersData()
    {
        allUsers = myUtils.getAllUsersSaveData();
        arUsers = new List<OneUserSaveData>(); // allUsers.arUsers
        if (allUsers.arUsers!=null)
        {
            for (int i = 0; i < allUsers.arUsers.Length; i++)
            {
                arUsers.Add(allUsers.arUsers[i]);
            }
        }
        myUtils.console_log(arUsers);

        GameObject itemsArea = GameObject.FindGameObjectWithTag("users_list");


        if (itemsArea.GetComponentsInChildren<Button>().Length > 0)
        {
            Button[] btns = itemsArea.GetComponentsInChildren<Button>();
            foreach (Button b in btns)
            {
                //myUtils.console_log("free btn " + b.name);
                //Destroy(b.GetComponentInParent<GameObject>());
                b.onClick = null;
                Destroy(b.gameObject);
            }
        }

        /*
        //Button btn = new Button()
        GameObject newButton = new GameObject("New button", typeof(Image), typeof(Button), typeof(LayoutElement));
        newButton.transform.SetParent(itemsArea.transform);
        newButton.GetComponent<LayoutElement>().minHeight = 35;
        GameObject newText = new GameObject("New text", typeof(Text));
        newText.transform.SetParent(newButton.transform);
        newText.GetComponent<Text>().text = "New button";
        //newText.GetComponent<Text>().font = font;
        RectTransform rt = newText.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0, 0);
        rt.anchorMax = new Vector2(1, 1);
        rt.anchoredPosition = new Vector2(0, 0);
        rt.sizeDelta = new Vector2(0, 0);
        newText.GetComponent<Text>().color = new Color(0, 0, 0);
        newText.GetComponent<Text>().alignment = TextAnchor.MiddleCenter;
        //newButton.GetComponent<Button>().onClick.AddListener(delegate { press(); });
        */

        foreach (OneUserSaveData user in arUsers)
        {
            //GameObject btn = new GameObject();
            GameObject btn = (GameObject)Instantiate(Resources.Load(GameConstants.PREFAB_BTN_USER_NAME));
            btn.transform.SetParent(itemsArea.transform);
            string name = user.name; // "Вася" + i.ToString();
            btn.name = name;
            btn.GetComponentInChildren<Text>().text = name;
            btn.GetComponent<Button>().onClick.AddListener(delegate { Select_User_inList(name); });
        }

        //Select_User_inList(GetInputName());
    }


    public void Save_AllUsersData(bool flag = false) {
        allUsers.arUsers = arUsers.ToArray();
        myUtils.saveAllUsersSaveData(allUsers);
    }


    public void Select_User_inList(string btn_name)
    {
        Debug.Log("клик по кнопке "+ btn_name);
        //UsersSrollContent.GetComponentsInChildren<Button>();
        GameObject itemsArea = GameObject.FindGameObjectWithTag("users_list");
        Button[] btns = itemsArea.GetComponentsInChildren<Button>(); 
        for (int j=0; j<btns.Length; j++)
        {
            if (btns[j].name.Equals(btn_name) )
            {
                ColorBlock cb = btns[j].colors;
                cb.normalColor = new Color(0.4f, 0.8f, 0.9f, 1f);
                cb.highlightedColor = new Color(0.6f, 0.8f, 0.9f, 1f);
                btns[j].colors = cb;
                btns[j].tag = "btn_selected";
                SetInputName(arUsers[j].name);
                //Save_AllUsersData(false); //сохранение имени текущего юзера
            }
            else
            {
                btns[j].colors = btn_defaultColorBlock;
                btns[j].tag = "btn_not_selected";
            }
        }
    }

    

    //********************************************************
    public void AddOneUser(OneUserSaveData user)
    {
    }
    //********************************************************
    //клик по кнопке удалить юзера
    public void User_Delete()
    {

        myUtils.console_log("delete_btn");
        //проверка, есть ли выбранных юзер
        //UsersSrollContent.GetComponentsInChildren<Button>();
        GameObject itemsArea = GameObject.FindGameObjectWithTag("users_list");
        Button[] btns = itemsArea.GetComponentsInChildren<Button>();
        int i = -1;
        for (int j = 0; j < btns.Length; j++)
        {
            if (btns[j].tag.Equals("btn_selected") )
            {
                i = j;
            }
        }
        if (i>=0)
        {
            arUsers.RemoveAt(i);
            SetInputName("");
            Save_AllUsersData();
            Reload_AllUsersData();
        }
        else
        {
            MessageBox.ShowMessage(() =>
            {
                Debug.Log("mb Delete click");
            }, "Не выбрана запись из списка юзеров." );
        }
    }


    //********************************************************
    //создание юзера
    public void User_Create()
    {
        string err_msg = "";
        string input_name = GetInputName();
        Debug.Log("имя: " + input_name);
        //проверки существования имени, 
        if (input_name.Length < 3)
        {
            err_msg = "Слишком короткое имя. Имя должно содержать 3+ знаков.";
        }
        else
        {
            if (arUsers.Count > 0)
            {
                //а также совпадения с уже имеющимися
                for (int i=0; i<arUsers.Count; i++)
                //foreach (OneUserSaveData user in arUsers)
                {
                    if (arUsers[i].name.Equals(input_name) )
                    {
                        err_msg = "Такое имя уже есть в списке юзеров.";
                        break;
                    }
                }
            }
        }


        if (err_msg.Length > 0)
        {
            //если есть, показ сообщения и продолжение
            MessageBox.ShowMessage(() =>
            {
                Debug.Log("mb Create click");
            }, err_msg);
        } else
        {
            //если нет, создание юзера и сохранение его в файл и сообщение об этом
            var newUser = new OneUserSaveData();
            newUser.name = input_name;
            newUser.id = 0;
            newUser.score = 0;
            newUser.level = 0;
            arUsers.Add(newUser);
            allUsers.curUserName = newUser.name;
            Save_AllUsersData();
            Reload_AllUsersData();
        }
    }

    //********************************************************
    //дальнейшие действия юзера
    public void User_Continue()
    {
        //проверка, есть ли выбранных юзер
        //UsersSrollContent.GetComponentsInChildren<Button>();
        GameObject itemsArea = GameObject.FindGameObjectWithTag("users_list");
        Button[] btns = itemsArea.GetComponentsInChildren<Button>();
        int i = -1;
        for (int j = 0; j < btns.Length; j++)
        {
            if (btns[j].tag.Equals("btn_selected") )
            {
                i = j;
            }
        }
        if (i >= 0)
        {
            Save_AllUsersData();
            pnlSelectUser.SetActive(false);  //скрытие окна выбора юзера
            pnl_menu.SetActive(true);  //открытие панели меню
            Text uname = pnl_menu.transform.Find("lbl_UserName").GetComponent<Text>();
            uname.text = arUsers[i].name;
        }
        else
        {
            MessageBox.ShowMessage(() =>
            {
                Debug.Log("mb Continue click");
            }, "Создайте новый или выберите профиль из списка."); 
        }
    }


}


[Serializable]
public class OneUserSaveData
{
    public int id;
    public string name;
    public int score;
    public int level;

    override public string  ToString()
    {
        return "id: " + id.ToString() + ", name: "+name+ ", lvl: "+level+", score:"+score;
    }
}


[Serializable]
public class AllUsersSaveData
{
    public OneUserSaveData[] arUsers;
    public string curUserName;
    /*
    public AllUsersSaveData(int num=1)
    {
        arUsers = new OneUserSaveData[num];
        arUsers[0].id = 1;
        arUsers[0].name = "Intruder";
        arUsers[0].score = 0;
        arUsers[0].level = 0;
        curUserName = arUsers[0].name;
    }
    */
}