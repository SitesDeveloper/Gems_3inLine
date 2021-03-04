using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class GameController : MonoBehaviour
{

    public static GameState gameState = GameState.NotPlay;

    public GameArea gameArea;
    public FiguresQueueList figuresQueueList;
    public FiguraController figura;
    public ScoresController scoresController;
    public RunSacredList sacredList;
    public ListMsgBoxGame listModalWins;
    public GameObject pnl_Options;

    //время последнего опроса клавиш
    float last_key_time = 0; 

    void Start()
    {
        Init();
    }

    //=============================================
    private void Init()
    {
        GameConstants.Init();
        figuresQueueList.set_TopPoint( GameConstants.QUEUE_ITEM_HEIGHT * (float)GameConstants.QUEUE_NUM_ITEMS );
        EventManager.TriggerEvent("InitLevel");
    }

    //=============================================
    private void Update()
    {
        if (Time.time > last_key_time)
        {
            last_key_time = Time.time + GameConstants.WAIT_GET_KEY;
            if (Input.GetKey("n"))
            {
                EventManager.TriggerEvent("InitLevel");
            }

            if (Input.GetKey("f"))
            {
                
                EventManager.TriggerEvent("NewFigura");
            }

            if (Input.GetKey("q"))
            {
                figuresQueueList.Reinit();
            }

        }

        if ( gameState == GameState.Play ) 
        {
            if (Input.GetKeyDown("p"))
            {
                //пауза
                EventManager.TriggerEvent("GamePause");
            }

            //добавление до 5 обетований, если их меньше 5
            if (sacredList.Count()<2)
            {
                EventManager.TriggerEvent("NewSacredText");
            }

        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            EventManager.TriggerEvent("ExitGame");
        }

    }

    //=============================================
    //инит обработчиков событий
    void OnEnable()
    {
        EventManager.StartListening("InitLevel", event_InitLevel);
        EventManager.StartListening("NewFigura", event_NewFigura);
        EventManager.StartListening("Anim_Overfill_area", event_AnimOverfillArea);
        EventManager.StartListening("CapacityFull", event_CapacityFull);
        EventManager.StartListening("RestartLevel", event_RestartLevel);
        EventManager.StartListening("LevelUp", event_LevelUp);
        EventManager.StartListening("GamePause", event_GamePause);
        EventManager.StartListening("ExitGame", event_ExitGame);
        EventManager.StartListening("NewSacredText", event_NewSacredText);
        EventManager.StartListening("OptionsOpen", event_OptionsOpen);
        EventManager.StartListening("OptionsClose", event_OptionsClose);
    }

    //=============================================
    //останов обработчиков событий
    void OnDisable()
    {
        EventManager.StopListening("InitLevel", event_InitLevel);
        EventManager.StopListening("NewFigura", event_NewFigura);
        EventManager.StopListening("Anim_Overfill_area", event_AnimOverfillArea);
        EventManager.StopListening("CapacityFull", event_CapacityFull);
        EventManager.StopListening("RestartLevel", event_RestartLevel);
        EventManager.StopListening("LevelUp", event_LevelUp);
        EventManager.StopListening("GamePause", event_GamePause);
        EventManager.StopListening("ExitGame", event_ExitGame);
        EventManager.StopListening("NewSacredText", event_NewSacredText);
        EventManager.StopListening("OptionsOpen", event_OptionsOpen);
        EventManager.StopListening("OptionsClose", event_OptionsClose);
    }

    //=============================================
    //инициализация нового уровня
    void event_InitLevel()
    {
        SoundManager.PlayMusicRandom();
        //SoundManager.PlayMusic("music_background2");
        GamePause();
        scoresController.Init();
        myUtils.console_log("event InitLevel " + scoresController.user.level);
        gameState = GameState.NotPlay;
        //очков за текущий уровень
        scoresController.AddScoreLevel(0, true);
        //init GameArea
        gameArea.Init_New_Level(scoresController.user.level);
        figuresQueueList.Fill_Figures_Queue();
        //FiguraController.GetQueueFigura();
        //gameState = GameState.Play;
        GameContinue();
        EventManager.TriggerEvent("NewFigura");
    }

    
    //=============================================
    //рестарт уровня - запускается когда область сборки переполнена
    void event_RestartLevel()
    {
        //только 50% очков берутся, остальные сгорают, как штраф за ненабранный уровень
        scoresController.AddTotalScore(scoresController.scoreLevel / 2);
        Debug.Log("event RestartLevel " + scoresController.user.level);
        EventManager.TriggerEvent("InitLevel");
    }


    //=============================================
    void event_NewFigura()
    {
        //плей звук - появления фигуры
        SoundManager.PlaySound("figura_new");
        //myUtils.console_log("event NewFigura, GameState=", gameState);
        if (gameState != GameState.Play)
            return;
        figura.delete_Figura();
        figura.Create_Figura(figuresQueueList.get_FirstFigure(), gameArea.transform);
        figuresQueueList.remove_FirstFigure();
        figuresQueueList.Fill_Figures_Queue();
    }

    //=============================================
    //область сборки переполнена, запуск анимации очистки
    void event_AnimOverfillArea()
    {
        GamePause();
        //плей звук очистка области
        SoundManager.PlaySound("level_fail");
        figura.delete_Figura();
        GameController.gameState = GameState.AnimAreaOverfill;
        gameArea.StartAnimationClear();
    }

    //=============================================
    //действия после анимации очистки области от переполнения
    void event_CapacityFull()
    {
        gameState = GameState.NotPlay;
        myUtils.console_log("event CapacityFull, GameState=", gameState);
        //показ сообщения и рестарт уровня
        listModalWins.AddModal(() =>
        {
            myUtils.console_log("Юзер прочитал сообщение");
            EventManager.TriggerEvent("RestartLevel");

        }, "Область сборки переполнена. Будет произведен рестарт уровня. За этот уровень вы набрали: "+ scoresController.scoreLevel.ToString() + " очков, но 50% из них будут принесены Господу во всесожжение, чтобы в следующий раз вы показали лучший результат.");
    }

    //=============================================
    //достигнут новый уровень
    void event_LevelUp()
    {
        GamePause();
        SoundManager.PlaySound("level_up");
        gameState = GameState.NotPlay;
        myUtils.console_log("event LevelUp, GameState=", gameState);
        scoresController.AddTotalScore( Mathf.RoundToInt( (float)scoresController.scoreLevel*0.9f ) );
        scoresController.LevelUp();

        string sacred = sacredList.getRandomSacred();
        //показ сообщения и запуск нового уровня
        listModalWins.AddModal(() =>
        {
            sacredList.AddSacredText(sacred);
            myUtils.console_log("Юзер прочитал сообщение");
            EventManager.TriggerEvent("InitLevel");

        }, "Поздравляем, вы достигли нового уровня! За этот уровень вы набрали: " + scoresController.scoreLevel.ToString() 
            + " очков, 10% из них будут отданы Господу, как дань по Его закону, чтобы ваш успех продолжался и далее."
            + "\n\n "+sacred );
    }

    //=============================================
    // добавление еще одного обетования
    void event_NewSacredText()
    {
        GamePause();
        //myUtils.console_log("event NewSacredText");
        SoundManager.PlaySound("sacred_open");
        string sacred = sacredList.getRandomSacred();
        //показ сообщения и запуск нового уровня
        listModalWins.AddModal(() =>
        {
            sacredList.AddSacredText(sacred);
            if (listModalWins.Count() <= 1)
            {
                GameContinue();
            }
        }, "Доступно обетование: " + sacred);

    }


    //=============================================
    //пауза игры
    void event_GamePause()
    {
        GamePause();
        figura.anti_click_space_first_frame = true;
        myUtils.console_log("event Pause, GameState=", gameState);
        listModalWins.AddModal(() =>
        {
            myUtils.console_log("Юзер прочитал сообщение");
            GameContinue();
        }, "Пауза игры. Отдохните и за дело!");
    }


    //=============================================
    //событие выход из игры
    void event_ExitGame()
    {
        myUtils.console_log("ExitGame.");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }

    //=============================================
    //событие открытие окна опций
    public void event_OptionsOpen()
    {
        GamePause();
        //оставить музыку включенной
        SoundManager.SetMusicMuted(false);
        pnl_Options.transform.Find("pnl_music_option").GetComponentInChildren<Slider>().value = SoundManager.GetMusicVolume();
        pnl_Options.transform.Find("pnl_sound_option").GetComponentInChildren<Slider>().value = SoundManager.GetSoundVolume();

        myUtils.console_log("event OptionsOpen, GameState=", gameState);
        pnl_Options.SetActive(true);
    }
    //=============================================
    //событие закрытие окна опций
    public void event_OptionsClose()
    {
        pnl_Options.SetActive(false);
        GameContinue();
        myUtils.console_log("event OptionsClose, GameState=", gameState);
    }

    //=============================================
    public void GamePause()
    {
        gameState = GameState.Pause;
        //SoundManager.Pause();
        SoundManager.SetMusicMuted(true);
        //GetComponent<AudioSource>().Pause();
    }

    //=============================================
    public void GameContinue()
    {
        gameState = GameState.Play;
        //GetComponent<AudioSource>().Play();
        SoundManager.SetMusicMuted(false);
        //SoundManager.UnPause();
    }


    //=============================================
    //проверка успешного окончания уровня
    public void check_level_up()
    {
        if ((1000 + 100 * scoresController.user.level) <= scoresController.scoreLevel)
        {
            figura.delete_Figura();
            gameState = GameState.AnimLevelUp;
            scoresController.AddGraphicsItem(new Vector2Int(5, 5), "Новый Уровень", ScoreType.LevelUp);
            gameArea.StartAnimationClear("LevelUp");
            //EventManager.TriggerEvent("");
        }
    }

    /*
    void TestScaleBrick()
    {
        if (Time.time >= time_next_move)
        {
            time_next_move = Time.time + 0.03f;
            if (scale_dir == 1)
                scale += 0.02f;
            else
                scale -= 0.02f;
            scale_dir_iterations++;
            if (scale_dir_iterations > 10)
            {
                scale_dir_iterations = 0;
                scale_dir = -scale_dir;
            }
            Vector3 sc = new Vector3(scale, scale, 0.0f);
            //Brick.transform.localScale  = sc;
           // GameObject.Find("Brick_1").transform.localScale = sc;
        }
    }
    */




}
