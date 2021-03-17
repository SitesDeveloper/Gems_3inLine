using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


public class SkillsTreeController : MonoBehaviour
{

    public GameObject ob_img;

    // Start is called before the first frame update
    void Start()
    {
        ob_img = GameObject.Find("b_Image");
        ob_img.GetComponent<Image>().sprite = Resources.Load("svitok", typeof(Sprite)) as Sprite;




    }

    // Update is called once per frame
    void Update()
    {
        ob_img.transform.Rotate(0,0,100 * Time.deltaTime);
    }


    void Test1()
    {
        /*
        Player playerInstance = new Player();
        playerInstance.playerId = "8484239823";
        playerInstance.playerLoc = "Powai";
        playerInstance.playerNick = "Random Nick";

        string playerToJson = JsonUtility.ToJson(playerInstance, true);
        Debug.Log(playerToJson);


        string jsonString = "{\"playerId\":\"3242\",\"playerLoc\":\"sadfsad\",\"playerNick\":\"fsadf\"}";
        Player player = JsonUtility.FromJson<Player>(jsonString);
        Debug.Log(player.playerId);

        Player[] playerInstance = new Player[2];

        playerInstance[0] = new Player();
        playerInstance[0].playerId = "8484239823";
        playerInstance[0].playerLoc = "Powai";
        playerInstance[0].playerNick = "Random Nick";

        playerInstance[1] = new Player();
        playerInstance[1].playerId = "512343283";
        playerInstance[1].playerLoc = "User2";
        playerInstance[1].playerNick = "Rand Nick 2";

        // Конвертируем в json
        string playerToJson = JsonHelper.ToJson(playerInstance, true);
        Debug.Log(playerToJson);



        string jsonString = "{\r\n    \"Items\": [\r\n        {\r\n            \"playerId\": \"2222\",\r\n            \"playerLoc\": \"Powai\",\r\n            \"playerNick\": \"Random Nick\"\r\n        },\r\n        {\r\n            \"playerId\": \"3333\",\r\n            \"playerLoc\": \"User2\",\r\n            \"playerNick\": \"Rand Nick 2\"\r\n        }\r\n    ]\r\n}";
        Player[] player = JsonHelper.FromJson<Player>(jsonString);
        Debug.Log(player[0].playerId);
        Debug.Log(player[1].playerId);
        */
    }
}


