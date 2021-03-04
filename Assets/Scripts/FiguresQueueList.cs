using System;
//using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class FiguresQueueList : MonoBehaviour
{
    //очередь фигур
    List<OneQueueFigura> figures_Queue = new List<OneQueueFigura>();

    public float top_point_y = 0;
    public float last_move_time = 0;

    void Start()
    {
    }

    void Update()
    {
        float delta = 0;
        if (Time.time >= last_move_time)
        {
            last_move_time = Time.time + GameConstants.QUEUE_SPD.interval;
            if (top_point_y > 0)
            {
                delta = GameConstants.QUEUE_SPD.delta;
                set_TopPoint( top_point_y - delta );
                if (top_point_y < 0)
                {
                    delta = delta + top_point_y; // GameConstants.QUEUE_ANIM_SPEED - delta;
                    set_TopPoint( 0 );
                }
                MoveBy(delta);
            }

            if (top_point_y > GameConstants.QUEUE_ITEM_HEIGHT * (GameConstants.QUEUE_NUM_ITEMS+1f) )
            {
                //если плашки фигур в очереди ушли далеко вниз (из-за частых изъятий фигур), то выравнять первую плашку непосредственно под область
                delta = top_point_y - GameConstants.QUEUE_ITEM_HEIGHT * GameConstants.QUEUE_NUM_ITEMS;
                set_TopPoint(top_point_y - delta);
                MoveBy(delta);
            }

        }
    }
    //=========================================
    //задать переменную верха
    public void set_TopPoint(float y) {
        //myUtils.console_log("top_point_y=", y);
        top_point_y = y;
    }


    //=========================================
    //переопределить область сборки
    public void Reinit()
    {
        RemoveAll();
        set_TopPoint( GameConstants.QUEUE_ITEM_HEIGHT * (float) GameConstants.QUEUE_NUM_ITEMS );
        Fill_Figures_Queue();
    }

    //=========================================
    //удалить все фигуры из очереди
    public void RemoveAll()
    {
        while (figures_Queue.Count > 0)
        {
            remove_FirstFigure();
        }
    }
    //=========================================
    //получить первую фигуру в очереди
    public OneQueueFigura get_FirstFigure()
    {
        OneQueueFigura figura = figures_Queue[0];
        return figura;
    }
    //=========================================
    //извлечь первую фигуру в очереди
    public void remove_FirstFigure()
    {
        figures_Queue[0].DestroyGraphicsObjects();
        figures_Queue.RemoveAt(0);
        set_TopPoint(top_point_y + GameConstants.QUEUE_ITEM_HEIGHT);
    }



    //get_first
    //=========================================
    //заполнить очередь фигурами
    public void Fill_Figures_Queue()
    {
        //myUtils.console_log("fill_queue");
        while ( figures_Queue.Count < GameConstants.QUEUE_NUM_ITEMS )
        {
            Add_Random_Figura();
        }
    }

    //=========================================
    //добавить случайную фигуру в конец очереди
    public void Add_Random_Figura()
    {
        CarcasType carcase_type = FiguraCarcasesList.get_RandomCarcasType();
        //myUtils.console_log("random carcase_type =", carcase_type);
        Vector3 crd = new Vector3(0f, -top_point_y - GameConstants.QUEUE_START.y - (float)figures_Queue.Count * GameConstants.QUEUE_ITEM_HEIGHT, 0f);
        OneQueueFigura qfigura = new OneQueueFigura();
        Transform parent = GameObject.FindGameObjectWithTag("queue_figures").GetComponent<Transform>();
        qfigura.Create_Queue_Figura(carcase_type, parent);
        qfigura.MoveTo(crd);
        figures_Queue.Add(qfigura);

    }

    //=========================================
    //сдвиг всех плашек с фигурами
    public void MoveBy(float delta)
    {
        foreach (OneQueueFigura fg in figures_Queue)
        {
            fg.MoveBy( new Vector3(0f, delta, 0f));
        }
    }





}