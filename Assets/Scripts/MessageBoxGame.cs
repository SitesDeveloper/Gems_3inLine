using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;



public class MessageBoxGame : UnityEngine.Object
{
    GameObject messageBox;
    Button btn_yes;
    Button btn_no;
    //флаг удаления для родительского класса
    public bool need_del = false;
    //флаг обработки клика кнопоки, если true, значит в этом фрейме уже одно окно обработало кнопки, другие это скипают
    //вообщем этот флаг, чтоб клик один раз по пробелу или ESC не закрыл сразу все окна (если их несколько открыто)
    public static bool key_clicked_this_frame = false;
    private int just_open = 0;

    public MessageBoxGame(Action action, string text, bool is_btn_no = false)
    {
        need_del = false;
        btn_yes = null;
        btn_no = null;
        messageBox = null;
        CreateWin(action, text, is_btn_no);
    }


    public void Update()
    {
        if (messageBox && (!key_clicked_this_frame) )
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                btn_yes.onClick.Invoke();
                key_clicked_this_frame = true;
            } else if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (btn_no.interactable)
                {
                    btn_no.onClick.Invoke();
                    key_clicked_this_frame = true;
                }
            }

            if (just_open<5) {
                //myUtils.console_log("value=", messageBox.transform.Find("mb_Window").Find("mb_Scrollbar").GetComponent<Scrollbar>().value);
                //ScrollRect sr = messageBox.transform.Find("mb_Window").Find("mb_scrollRect").GetComponent<ScrollRect>();
                //sr.normalizedPosition = new Vector2(sr.normalizedPosition.x, 0f);
                messageBox.transform.Find("mb_Window").Find("mb_Scrollbar").GetComponent<Scrollbar>().value = 1f;
                Transform panel = messageBox.transform.Find("mb_Window");
                Text txt = panel.Find("mb_scrollRect").Find("mb_content").Find("mb_Text").GetComponent<Text>();
                float h = txt.rectTransform.rect.height;
                //myUtils.console_log("h=", h);
                h += 100f;
                if (h > 600f)
                {
                    h = 600f;
                    panel.Find("mb_Scrollbar").GetComponent<Scrollbar>().interactable = true;
                }
                else
                {
                    panel.Find("mb_Scrollbar").GetComponent<Scrollbar>().interactable = false;
                }
                panel.GetComponent<RectTransform>().sizeDelta = new Vector2(panel.GetComponent<RectTransform>().sizeDelta.x, h );
                just_open++;
            }
        }
    }


    public void CreateWin(Action action, string text, bool is_btn_no = false)
    {
        need_del = false;
        messageBox = (GameObject)Instantiate(Resources.Load( GameConstants.PREFAB_GAME_MSG_BOX ) );
        //messageBox.GetComponent<Transform>().SetParent(GameObject.Find("Canvas").GetComponent<Transform>());
        Transform panel = messageBox.transform.Find("mb_Window");
        //myUtils.console_log(panel);
        Text msgBoxText = panel.Find("mb_scrollRect").Find("mb_content").Find("mb_Text").GetComponent<Text>();
        msgBoxText.text = text;
        //msgBoxText.GetComponent<ContentSizeFitter>().SetLayoutVertical();
        Transform btn_panel = panel.transform.Find("btn_panel");
        btn_yes = btn_panel.Find("mb_btn_yes").GetComponent<Button>();
        btn_no = btn_panel.Find("mb_btn_no").GetComponent<Button>();
        btn_no.interactable = is_btn_no;
        btn_yes.onClick.AddListener(() =>
        {
            action();
            CloseWin();
        });

        btn_no.onClick.AddListener(() =>
        {
            CloseWin();
        });

        just_open = 0;
    }

    public void CloseWin()
    {
        Destroy(messageBox);
        need_del = true;
    }

}


/*
public class MessageBoxGame : MonoBehaviour
{
    private static MessageBoxGame instance;
    public GameObject Template;

    static GameObject messageBox;
    static Button btn_yes;
    static Button btn_no;

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (messageBox)
        {

            if (Input.GetKeyDown(KeyCode.Space))
            {
                btn_yes.onClick.Invoke();
            }

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (btn_no.interactable)
                    btn_no.onClick.Invoke();
            }
        }
    }


    public static void ShowMessage(Action action, string text, bool is_btn_no = false)
    {
        messageBox = Instantiate(instance.Template);

        Transform panel = messageBox.transform.Find("mb_Window");
        // GetComponent<ScrollRect>().GetComponent<Text>(); 
        myUtils.console_log(panel);
        Text msgBoxText = panel.Find("mb_scrollRect").Find("mb_Text").GetComponent<Text>();
        msgBoxText.text = text;
        Transform btn_panel = panel.transform.Find("btn_panel");
        btn_yes = btn_panel.Find("mb_btn_yes").GetComponent<Button>();
        btn_no = btn_panel.Find("mb_btn_no").GetComponent<Button>();

        btn_no.interactable = is_btn_no;


        btn_yes.onClick.AddListener(() =>
        {
            action();
            Destroy(messageBox);
        });

        btn_no.onClick.AddListener(() =>
        {
            Destroy(messageBox);
        });

    }
}
*/