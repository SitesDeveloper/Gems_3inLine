using System;
//using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class DialogMainMenu : MonoBehaviour
{
    public GameObject progress_block;

    public void Start_New_Game()
    {
        myUtils.console_log("main menu dialog select: start new game");
        //gameSceneLoader SceneLoader = GetComponent<gameSceneLoader>();
        //GameObject.FindGameObjectWithTag("game_area_graph");
        //SceneLoader.Load_Scene("main_scene");

        progress_block.GetComponent<gameSceneLoader>().Load_Scene("main_scene");

    }

    public void Exit_Game()
    {
        myUtils.console_log("main menu dialog select: exit game");
    }

    public void Options_Game()
    {
        myUtils.console_log("main menu dialog select: options game");
    }

}