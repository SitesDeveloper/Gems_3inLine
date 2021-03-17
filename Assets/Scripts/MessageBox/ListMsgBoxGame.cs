using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

//список окон сообщений (для храннения их всех)
public class ListMsgBoxGame : MonoBehaviour
{
    List<MessageBoxGame> msgBG = new List<MessageBoxGame>();

    void Start()
    {
        //msgBG = new List<MessageBoxGame>();
    }

    public int Count()
    {
        return msgBG.Count;
    }

    private void Update()
    {
        if (msgBG.Count>0)
        {
            //myUtils.console_log("Update");
            MessageBoxGame.key_clicked_this_frame = false;
            for (int i = msgBG.Count-1; i>=0; i--)
            {
                if (msgBG[i].need_del)
                {
                    msgBG.RemoveAt(i);
                }
                else
                {
                    msgBG[i].Update();
                }
            }
        }
    }

    public void AddModal(Action action, string text, bool is_btn_no = false)
    {
        MessageBoxGame mbg = new MessageBoxGame(action, text, is_btn_no);
        msgBG.Add(mbg);
    }



}
