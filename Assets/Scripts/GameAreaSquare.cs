using System;
//using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

//================================================
public class GameAreaSquare : OneSquare
{
    //смещение на которое нужно плавно подвинуть камень (например, если была заполнена линия(и) снизу)
    public float ostatokMove;
    //текущая скорость
    public float cur_spd;
    //время последнего движения
    public float last_move_time = 0f;
    // время последней idle-анимации (небольшой анимации време от времени, пока объект на месте)
    public float laste_time_idle_animation = 0;

    //================================================
    //создание квадрата
    public void Setup(Vector3 crd, Transform parent, Vector3 scale, int _type = GameConstants.SQUARE_RANDOM, string animation = GameConstants.sANIM_BASE)
    {
        Create(crd, parent, scale, _type, animation);
        ostatokMove = 0f; // new Vector3(0f, 0f, 0f);
        last_move_time = 0f;
        laste_time_idle_animation = 0;
        cur_spd = 0f;
    }

    //================================================
    //функция для запуска каждый фрейм
    public void Update()
    {
        //запуск время от времени у камней области анимации небольшого сверкания
        if ((!is_Empty()) && (Time.time >= laste_time_idle_animation))
        {
            laste_time_idle_animation = Time.time + UnityEngine.Random.Range(GameConstants.ANIM_IDLE_RANGE.min, GameConstants.ANIM_IDLE_RANGE.max);
            brick.GetComponent<Animator>().SetTrigger("little_sparke");
        }

        if (!is_Empty() && (Time.time >= last_move_time) && (ostatokMove > 0))
        {
            last_move_time = Time.time + GameConstants.AREA_LINES_DOWN_SPD.interval;
            float delta = cur_spd;
            if (delta > ostatokMove)
            {
                delta = ostatokMove;
                cur_spd = 0f;
            }
            else
            {
                cur_spd += GameConstants.AREA_LINES_DOWN_SPD.delta;
            }
            ostatokMove -= delta;
            //движение если нужно
            brick.transform.localPosition += new Vector3(0f, -delta, 0f);
        }
    }


    //================================================
    //увеличить остаток движения по Y
    public void IncOstatokMove(float delta)
    {
        ostatokMove = ostatokMove + delta;
    }

    //================================================
    //получить камень из квадрата выше
    public void getFromSquare( GameAreaSquare square )
    {
        type = square.type;
        brick_name = square.brick_name;
        brick = square.brick;
        ostatokMove = square.ostatokMove + GameConstants.AREA_SQUARE_SIZE.y;
        cur_spd = 0f;
        //float delta = (square.ostatokMove > 0f) ? square.ostatokMove : GameConstants.AREA_SQUARE_SIZE.y;
        //IncOstatokMove( delta ); 
    }

}

