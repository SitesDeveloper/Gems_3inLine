using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoresController : MonoBehaviour
{
    List<OneScore> scoresList = new List<OneScore>();
    public GameController gameController;

    public Text txt_CurLevel;
    public Text txt_ScoreLevel;
    public Text txt_ScoreTotal;
    public int scoreLevel = 0;   //текущее кол-во очков за уровень

    public OneUserSaveData user;
    public int cur_scoreLevel = 0;  //кол-во очков в счетчике
    int scoreLevel_delta = 0; //дельта для кол-ва на каждом шаге
    int scoreLevel_step = 0;  //всего шагов, когда была дельта добавлена
    float next_change_time = 0f;

    // инициализация (перед началом уровня)
    public void Init()
    {
        user = myUtils.GetCurUserSaveData();
        if (user.level <= 0)
        {
            user.level = 1;
            myUtils.SaveUserData(user);
        }
        scoreLevel = 0;
        cur_scoreLevel = 0;
        scoreLevel_delta = 0;
        next_change_time = 0f;
    }

    //======================================
    // Update is called once per frame
    void Update()
    {
        int i=0;
        while (i<scoresList.Count) 
        {
            if (scoresList[i].isLife())
            {
                scoresList[i].Update();
                i++;
            }
            else
            {
                scoresList[i].Destroy();
                scoresList.RemoveAt(i);
            }
        }    

        //инкремент очков
        if (Time.time >= next_change_time)
        {
            next_change_time = Time.time + 0.02f;
            if (cur_scoreLevel != scoreLevel)
            {
                int dir = Mathf.RoundToInt(Mathf.Sign((float)(scoreLevel - cur_scoreLevel)));
                cur_scoreLevel += scoreLevel_delta * dir;
                if (((dir == -1) && (cur_scoreLevel <= scoreLevel)) ||
                     ((dir == 1) && (cur_scoreLevel >= scoreLevel)))
                {
                    cur_scoreLevel = scoreLevel;
                    scoreLevel_delta = 0;
                    scoreLevel_step = 0;
                }
                else
                {
                    scoreLevel_step++;
                    //каждый 3 шаг - дельта увеличивается
                    if (scoreLevel_step % 3 == 0)
                    {
                        scoreLevel_delta++;
                    }
                }
            }
            Update_Score_Texts();
        }

    }

    //=============================================
    //обновление текстов баллов и уровня
    void Update_Score_Texts()
    {
        txt_CurLevel.text = user.level.ToString(); //"Уровень:\n" + 
        txt_ScoreLevel.text = cur_scoreLevel.ToString();  //"За уровень:\n" + 
        txt_ScoreTotal.text = user.score.ToString();  //"Всего благ:\n" + 
    }

    //======================================
    //добавить или установить (при setup=true) очки
    //и проверка достижения нового уровня
    public void AddScoreLevel(int Val, bool setup=false)
    {
        if (setup)
        {
            scoreLevel = Val;
        } else
        {
            scoreLevel += Val;
        }
        gameController.check_level_up();
    }

    //======================================
    //добавление к общему кол-ву очков юзера
    public void AddTotalScore( int Val )
    {
        user.score += Val;
        myUtils.SaveUserData(user);
        Update_Score_Texts();
    }
    //======================================
    //добавление еще одного уровня
    public void LevelUp()
    {
        user.level++;
        myUtils.SaveUserData(user);
    }

    //======================================
    //добавить графический вид - всплывающий текст в области сборки
    public void AddGraphicsItem( Vector2Int crd, string Value, ScoreType type)
    {
        OneScore scoreItem = new OneScore(crd, Value, type);
        scoresList.Add(scoreItem);
    }
}