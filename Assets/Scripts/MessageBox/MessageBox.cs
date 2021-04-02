using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class MessageBox : MonoBehaviour {

    public static MessageBox instance;
    public GameObject Template;

    private void Awake()
    {
        instance = this;
    }


    public static void ShowMessage(Action action, string title, bool is_btn_no = false)
    {
        //GameObject messageBox = Instantiate(instance.Template);
        GameObject messageBox = (GameObject)Instantiate(Resources.Load(GameConstants.PREFAB_SIMPLE_MESSAGE_BOX));


        Transform wrapper = messageBox.transform.Find("pnl_win_wrapper");
        Transform panel = wrapper.transform.Find("mb_Window");
        var pnl_txt = panel.Find("pnl_text");
        //TMP_Text msgBoxText = pnl_txt.Find("mb_Text_tmp").GetComponent<TMP_Text>();
        TMP_Text msgBoxText = pnl_txt.GetComponentInChildren<TMP_Text>();
        msgBoxText.text = title;
        Transform btn_panel = panel.transform.Find("btn_panel");
        Button yes = btn_panel.Find("mb_btn_yes").GetComponent<Button>();
        Button no = btn_panel.Find("mb_btn_no").GetComponent<Button>();

        no.gameObject.SetActive(is_btn_no);
        //no.interactable = is_btn_no;
        

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
