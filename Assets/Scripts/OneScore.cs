using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OneScore : UnityEngine.Object
{
    float last_move_time = 0f;
    float max_move_time = 0f;
    GameObject flyText;

    //===================================
    public OneScore(Vector2Int crd, string Value, ScoreType type)
    {
        //flyText = null;
        //last_move_time = 0f;
        //max_move_time = Time.time + 5f;
        Create_Score(crd, Value, type);
    }

    //===================================
    public bool isLife()
    {
        return (Time.time < max_move_time);
    }

    //===================================
    public void Update()
    {
        if ( (GameController.gameState == GameState.Play) ||
            (GameController.gameState == GameState.AnimAreaOverfill) ||
            (GameController.gameState == GameState.AnimLevelUp) )
        {
            if (Time.time >= last_move_time )
            {
                last_move_time = Time.time + GameConstants.SCORE_SPD.interval; // .SCORE_MOVE_TIME_INTERVAL;
                flyText.transform.localPosition += new Vector3(0f, GameConstants.SCORE_SPD.delta, 0f); // .SCORE_MOVE_SPD, 0f);
                flyText.transform.localScale += new Vector3(0.05f, 0.05f, 0f);
            }
        }
    }
    //===================================
    public void Destroy()
    {
        Destroy(flyText);
    }


    //===================================
    //добавить очки
    public void Create_Score(Vector2Int crd, string Value, ScoreType type)
    {
        Vector2 AreaScrSize = GameConstants.AREA_SCR_SIZE;
        Vector3 wp = new Vector3(
            -AreaScrSize.x / 2f + ((float)crd.x + 0.5f) * AreaScrSize.x / (float)GameConstants.AREA_WIDTH,
            -AreaScrSize.y / 2f + ((float)crd.y + 0.5f) * AreaScrSize.y / (float)GameConstants.AREA_VISUAL_HEIGHT,
            0
        );
        //Canvas canv = GameObject.FindObjectOfType<Canvas>().GetComponent<Canvas>();
        GameObject prn = GameObject.Find("GameAreaUI");
        
        //myUtils.console_log("wp=", wp, "canv=",canv); //, "src_wp=", scr_wp);
        string path_to_prefab = "Prefab/text_Score";
        flyText = (GameObject)Instantiate(Resources.Load(path_to_prefab));  //, parent
        flyText.transform.SetParent(prn.transform);
        flyText.transform.localPosition = wp;
        Text t = flyText.GetComponent<Text>();
        t.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        t.text = Value; 
        max_move_time = Time.time + Random.Range(GameConstants.SCORE_LIFETIME_RANGE.min, GameConstants.SCORE_LIFETIME_RANGE.max);
        last_move_time = 0f;
        if (type == ScoreType.LevelUp)
        {
            //max_move_time = Time.time + 5f;
            flyText.transform.localScale *= 1.1f;
        }
        else if (type == ScoreType.LineFill)
        {
            //max_move_time = ;
            flyText.transform.localScale *= 2f;
        }
    }

}
