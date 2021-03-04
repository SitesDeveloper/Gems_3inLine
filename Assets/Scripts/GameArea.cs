using System;
//using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


//================================================
public class GameArea : MonoBehaviour
{

    public GameController gameController; 
    public ScoresController scoresController;
    public FilledLinesController filledLinesController;

    //создать класс одного куба и юзать список его экземпляров как наполнения области сборки
    GameAreaSquare[,] area = new GameAreaSquare[ GameConstants.AREA_WIDTH, GameConstants.AREA_REAL_HEIGHT];

    public float time_end_global_animation = 0f;
    public string time_end_triger_name = "CapacityFull"; //or LevelUp


    private void Awake()
    {
        //gameController = GetComponent<GameController>();
        //scoresController = GetComponent<ScoresController>();
        for (int x = 0; x < GameConstants.AREA_WIDTH; x++)
            for (int y = 0; y < GameConstants.AREA_REAL_HEIGHT; y++)
            {
                area[x, y] = new GameAreaSquare();
                //myUtils.console_log(area[x, y]);
            }
    }

    // Use this for initialization
    void Start () {
    }

    // Update is called once per frame
    void Update()
    {
        if ( (GameController.gameState == GameState.AnimAreaOverfill) ||
            (GameController.gameState == GameState.AnimLevelUp)  )
        {
            //процесс анимации удаления камней
            if (Time.time>=time_end_global_animation)
            {
                //сообщение в п-окне о рестарте уровня
                EventManager.TriggerEvent(time_end_triger_name);
            }
        }
        else if (GameController.gameState == GameState.Play)
        {
            //run_time_animation();
            for (int x = 0; x < GameConstants.AREA_WIDTH; x++)
                for (int y = 0; y < GameConstants.AREA_VISUAL_HEIGHT; y++)
                    area[x, y].Update();
        }
    }

    //запуск анимации удаления всех камней с области
    public void StartAnimationClear(string triger_name = "CapacityFull")
    {
        //перепрошивка аниматора для каждого камня
        for (int x = 0; x < GameConstants.AREA_WIDTH; x++)
            for (int y = 0; y < GameConstants.AREA_VISUAL_HEIGHT; y++)
            {
                area[x, y].SetAnimator(GameConstants.sANIM_AREA_OVERFILL);
            }
        time_end_global_animation = Time.time + GameConstants.AREA_OVERFILL_ANM.interval;
        time_end_triger_name = triger_name;
    }

    //инициализация нового уровня
    public void Init_New_Level( int Level )
    {
        //Debug.Log("GameArea: init level" + Level);
        //Debug.Log("W x H = " + GameConstants.AREA_WIDTH + " x " + GameConstants.AREA_VISUAL_HEIGHT);
        for (int x = 0; x < GameConstants.AREA_WIDTH; x++)
            for (int y = 0; y < GameConstants.AREA_REAL_HEIGHT; y++)
            {
                area[x, y].Empty();
                if (y < 3)
                {
                    float is_e = UnityEngine.Random.Range(0.2f, 1f);
                    if (is_e > 0.5)
                    {
                        SetSquare(x, y, GameConstants.SQUARE_RANDOM);
                    }
                }
            }
    }

    //установка в области квадрата заданного типа
    public void SetSquare( int x, int y, int type = GameConstants.SQUARE_RANDOM)
    {
        //сначала очистка квадрата
        area[x, y].Empty();
        Vector2 crd = new Vector2(
            GameConstants.AREA_START.x + GameConstants.AREA_SQUARE_SIZE.x * ((float)x + 0.5f),
            GameConstants.AREA_START.y + GameConstants.AREA_SQUARE_SIZE.y * ((float)y + 0.5f)
        );
        //теперь генерация
        area[x, y].Setup(crd, transform, GameConstants.AREA_SQUARE_SCALE, type, GameConstants.sANIM_BASE);
    }



    //проверка занятости квадрата
    public bool is_disabled(int x, int y)
    {
        if ((y < 0) || (x < 0) || (x >= GameConstants.AREA_WIDTH))
            return true;
        if (y >= GameConstants.AREA_REAL_HEIGHT)
            return false; // сверху вне области
        if (area[x, y].is_Empty())
            return false;
        return true;
    }


    //размещение фигуры в области сборки
    public void put_figura( Vector2Int m, FiguraCarcas fd, List<OneSquare> figuraSquares )
    {
        SoundManager.PlaySound("figura_landed");
        for (int i=0; i<fd.numPoints; i++)
        {
            int x = m.x + fd.coords[i, 0];
            int y = m.y + fd.coords[i, 1];
            if (y >= GameConstants.AREA_REAL_HEIGHT) continue;
            SetSquare(x, y, figuraSquares[i].type);
        }
        int figure_score = (int)UnityEngine.Random.Range(5, 20);
        scoresController.AddScoreLevel(figure_score);
        scoresController.AddGraphicsItem(m, figure_score.ToString(), ScoreType.FigurePut);

        //проверка на заполненные линии
        check_filled_lines();

        //проверка на финиш (заполнение 0 линии), 
        if (is_CapacityFull())
        {
            //область переполнена, запуск анимации удаления камней, а затем рестарт уровня 
            EventManager.TriggerEvent("Anim_Overfill_area");
        }
        //EventManager.TriggerEvent("CanStartNewFigura");
    }


    public bool is_CapacityFull()
    {
        int x;
        int y = GameConstants.AREA_REAL_HEIGHT - 1;
        bool is_exists = false;
        for(x=0; x<GameConstants.AREA_WIDTH; x++)
        {
            if (!area[x, y].is_Empty())
            {
                is_exists = true;
                break;
            }
        }
        return is_exists;
    }




    public void check_filled_lines()
    {
        int x;
        for (int y = GameConstants.AREA_VISUAL_HEIGHT-2; y >= 0; y--)
        {
            bool isLineFilled = true;
            for (x = 0; x < GameConstants.AREA_WIDTH; x++)
            {
                if (area[x,y].is_Empty())
                {
                    isLineFilled = false;
                    break;
                }
            }

            if (isLineFilled)
            {
                Debug.Log("line-filled = " + y);
                //получение камней ряда
                List<int> lineSquares = new List<int>();
                for (x = 0; x < GameConstants.AREA_WIDTH; x++)
                {
                    //помещение в область для анимации
                    lineSquares.Add(area[x, y].type);
                    //удаление камней из ряда
                    area[x, y].Empty(); // false);
                }
                //создание линии анимации с камнями ряда
                filledLinesController.AddLine(y, lineSquares, transform);
                //начисление очков
                int line_score = (int)UnityEngine.Random.Range(100, 100+GameConstants.AREA_WIDTH*10);
                //создание анимации очков
                scoresController.AddScoreLevel(line_score);
                scoresController.AddGraphicsItem(new Vector2Int(5,y), line_score.ToString(), ScoreType.LineFill);

                //перемещение вышестоящих квадратов вниз на 1
                for (int yy = y; yy <= GameConstants.AREA_VISUAL_HEIGHT-2; yy++)
                {
                    for (x = 0; x < GameConstants.AREA_WIDTH; x++)
                    {
                        if (!area[x, yy + 1].is_Empty())
                        {
                            area[x, yy].getFromSquare(area[x, yy + 1]);
                            area[x, yy + 1].Empty(false);
                        }
                    }
                }
            }

        }

    }


}
