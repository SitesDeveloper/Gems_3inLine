using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class MessageBox : MonoBehaviour {

    private static MessageBox instance;
    public GameObject Template;

    private void Awake()
    {
        instance = this;
    }


    public static void ShowMessage(Action action, string title, bool is_btn_no = false)
    {
        GameObject messageBox = Instantiate(instance.Template);

        Transform panel = messageBox.transform.Find("mb_Window");
        Text msgBoxText = panel.Find("mb_Text").GetComponent<Text>();
        msgBoxText.text = title;
        Transform btn_panel = panel.transform.Find("btn_panel");
        Button yes = btn_panel.Find("mb_btn_yes").GetComponent<Button>();
        Button no = btn_panel.Find("mb_btn_no").GetComponent<Button>();

        no.interactable = is_btn_no;
        

        yes.onClick.AddListener(() =>
        {
            action();
            Destroy(messageBox);
        });

        no.onClick.AddListener(() =>
        {
            Destroy(messageBox);
        });

    }
}
