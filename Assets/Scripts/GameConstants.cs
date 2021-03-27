using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;




//константы
public class GameConstants : MonoBehaviour  
{

    //для графической системы очков
    public static Vector2 AREA_SCR_SIZE = new Vector2(510f, 680f);
    //время движения очков
    public static RangeAttribute SCORE_LIFETIME_RANGE = new RangeAttribute(0.5f, 1f);
    //итервал времени и дельта движения очков
    public static DeltaInterval SCORE_SPD = new DeltaInterval(0.02f, 2.5f);

    public const int AREA_WIDTH = 10;
    public const int AREA_VISUAL_HEIGHT = 12;
    public const int AREA_REAL_HEIGHT = AREA_VISUAL_HEIGHT + 1;


    public const int SQUARE_NUM_TYPES = 6;
    public const int SQUARE_EMPTY = -2;
    public const int SQUARE_RANDOM = -1;

    //область сборки: стартовая точка, размер квадрата и масштаб префаба камня для помения в область
    public static Vector3 AREA_START = new Vector3(0f, 0f, 0);
    public static Vector2 AREA_SQUARE_SIZE = new Vector2(0f,0f);
    public static Vector3 AREA_SQUARE_SCALE = new Vector3(0.58f, 0.28f, 0);
    public static DeltaInterval AREA_LINES_DOWN_SPD = new DeltaInterval(0.02f,0.1f);

    //заполненная линия (анимация уменьшения масштаба - второе число delta юзается: scale -= scale/delta)
    public static DeltaInterval AREA_FILLED_LINE_ANM = new DeltaInterval(0.02f, 50f);
    //время анимации очистки области от камней
    public static DeltaInterval AREA_OVERFILL_ANM = new DeltaInterval(1f, 0f);

    //итервал времени и дельта движения фигуры в области сборки
    public static DeltaInterval FIGURA_SPD = new DeltaInterval(0.02f, 0.1f);
    //время паузы, прибазирования фигуры, в течении которого ее можно двигать
    public static float FIGURA_AREA_ACEPT_TIME = 0.3f;

    //очередь фигур
    //кол-во фигур в очереди
    public const int QUEUE_NUM_ITEMS = 4;
    //высота одной плашки + отступ
    public static float QUEUE_ITEM_HEIGHT = 0f;
    public static Vector2 QUEUE_START = new Vector2(0f, 0f);
    //итервал времени и дельта движения плашки с фигурой в очереди
    public static DeltaInterval QUEUE_SPD = new DeltaInterval(0.02f, 0.1f);
    //размер и масштаб для префаба квадрата фигуры на плашке
    public static Vector2 QUEUE_SQUARE_SIZE = new Vector2(0.2f, 0.26f);
    public static Vector3 QUEUE_SQUARE_SCALE = new Vector3(0.05f, 0.08f, 0);
    //масштаб для префаба плашки
    public static Vector3 PLASHKA_SCALE = new Vector3(18f, 2.5f, 0);

    public const int DIR_LEFT = 1;
    public const int DIR_RIGHT = 2;

    public const int SKILLS_NODE_IS_CATEGORY = 1;


    //range интервалов когда возможна следующая анимация типа IDLE, для квадрата области сборки
    public static RangeAttribute ANIM_IDLE_RANGE = new RangeAttribute(3f, 10f);


    public const float WAIT_GET_KEY = 0.1f;
    //интервал времени добавления в бегущий текст новых текстов
    public const float SACRED_TEXT_TIME_INTERVAL = 60f * 5f;

    public static string PATH_TO_USERS_SAVEDATA = "Save.json";

    public static string PATH_TO_SKILLS_TREE = "SkillsTree.json";


    public const string sANIM_BASE = "Animations/base_stones_cntr";
    public const string sANIM_FILL_LINE = "Animations/line_fill_cntr";
    public const string sANIM_FIGURA = "Animations/figura_move_cntr";
    public const string sANIM_AREA_OVERFILL = "Animations/area_overfill";


    public const string PREFAB_BTN_USER_NAME = "Prefab/Profile/btn_UserName_tmp";
    public const string PREFAB_PLASHKA = "Prefab/plashka";
    public const string PATH_TO_SQUARES_PREFABS = "Prefab/bricks/";
    public const string PREFAB_SACRED_TEXT = "Prefab/SacredText";
    public const string PREFAB_GAME_MSG_BOX = "Prefab/MessageBoxGame";
    public const string PREFAB_SIMPLE_MESSAGE_BOX = "Prefab/PopupWindows/MessageBox";


    public const string PREFAB_EDITOR_CATEGORY = "Prefab/SkillsTreeEditor/category";
    public const string PREFAB_EDITOR_SUBCATEGORY = "Prefab/SkillsTreeEditor/subcategory";
    public const string PREFAB_EDITOR_ELEMENT = "Prefab/SkillsTreeEditor/element";


    private static Sprite[] allSprites;

    //инициализация
    public static void Init() {

        GameObject gameAreaGraph = GameObject.FindGameObjectWithTag("game_area_graph");
        Vector3 AreaBounds = gameAreaGraph.GetComponent<SpriteRenderer>().sprite.bounds.size;
        Camera cam = GameObject.FindObjectOfType<Camera>().GetComponent<Camera>();

        //Vector3 ScreenSize = new Vector3(Screen.width, Screen.height, 0.0f);
        //myUtils.console_log("screen_size", ScreenSize);
        //Vector3 ScreenSizeWP = cam.ScreenToWorldPoint(ScreenSize);
        //myUtils.console_log("screen_size WP", ScreenSizeWP);

        AREA_SQUARE_SIZE.x = AreaBounds.x / (float)AREA_WIDTH;
        AREA_SQUARE_SIZE.y = AreaBounds.y / (float)AREA_VISUAL_HEIGHT;
        FIGURA_SPD.delta = AREA_SQUARE_SIZE.y / 25f;
        AREA_LINES_DOWN_SPD.delta = AREA_SQUARE_SIZE.y / 300f;
        //myUtils.console_log("cub size = ", AREA_SQUARE_SIZE);
        AREA_START.x = -AREA_SQUARE_SIZE.x * (float)AREA_WIDTH / 2f;
        AREA_START.y = -AREA_SQUARE_SIZE.y * (float)AREA_VISUAL_HEIGHT / 2f;
        //myUtils.console_log("start = ", AREA_START);


        //AREA_SQUARE_SCALE
        /*
        string prefab_name = "Prefab/bricks/Brick_0";
        GameObject prefab = (GameObject)Instantiate(Resources.Load(prefab_name, typeof(GameObject)), new Vector3(0, 0, 0), Quaternion.identity) as GameObject;
        Vector3 prefBounds = prefab.GetComponent<SpriteRenderer>().sprite.bounds.size;
        myUtils.console_log("prefBounds", prefBounds);
        AREA_SQUARE_SCALE.x = AREA_SQUARE_SIZE.x / prefBounds.x;
        AREA_SQUARE_SCALE.y = AREA_SQUARE_SIZE.y / prefBounds.y;
        myUtils.console_log("scale = ", AREA_SQUARE_SCALE);
        Destroy(prefab);
        */


        GameObject gameQueueFigures = GameObject.FindGameObjectWithTag("queue_figures");
        Vector3 QueueAreaBounds = gameQueueFigures.GetComponent<SpriteRenderer>().sprite.bounds.size;
        //myUtils.console_log("queue_area", QueueAreaBounds);
        //QueueAreaBounds.x = QueueAreaBounds.x * gameQueueFigures.GetComponent<SpriteRenderer>().transform.localScale.x;
        //QueueAreaBounds.y = QueueAreaBounds.y * gameQueueFigures.GetComponent<SpriteRenderer>().transform.localScale.y;
        //QueueAreaBounds.z = 0;
        //myUtils.console_log("queue_area_scale", QueueAreaBounds);
        QUEUE_ITEM_HEIGHT = QueueAreaBounds.y / (float)QUEUE_NUM_ITEMS;
        //myUtils.console_log("queue_item_height", QUEUE_ITEM_HEIGHT);
        QUEUE_START.x = -0.5f; // -QUEUE_SQUARE_SIZE.x; //  -QueueAreaBounds.x / 2f + (QueueAreaBounds.x - (QUEUE_SQUARE_SIZE.x * 4f)) / 2f;
        QUEUE_START.y = -QUEUE_ITEM_HEIGHT * (float)QUEUE_NUM_ITEMS / 2f + QUEUE_ITEM_HEIGHT/2f;
        //QUEUE_SQUARE_SIZE = Vector2( 4f, 0.8f );
        QUEUE_SPD.delta = QUEUE_ITEM_HEIGHT / 40f;
        //myUtils.console_log("queue_anim_speed", QUEUE_ANIM_SPEED);


        GameObject bkFon = GameObject.FindGameObjectWithTag("background_fon");
        Vector3 bkFonBounds = bkFon.GetComponent<SpriteRenderer>().sprite.bounds.size;
        //myUtils.console_log("bkFonBounds", bkFonBounds);

    }



    //**************************************************************
    public static Sprite GetSprite(string sprite_name)
    {
        if (allSprites == null)
        {
            allSprites = Resources.LoadAll<Sprite>("Sprites");  // / SkillsTreeEditor /
        }
        foreach (Sprite spr in allSprites)
        {
            if (spr.name == sprite_name)
            {
                return spr;
            }
        }
        return null;  //or may be Excepcion
    }


}



