using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//для анимации одного заполненного ряда
public class OneFilledLine : UnityEngine.Object
{
    public int y = -1;
    public float next_anim_time = 0f;
    public float end_anim_time = 0f;
    List<GameAreaSquare> squaresList;

    //создание линии анимации
    public OneFilledLine(int _y, List<int> _squaresTypes, Transform parent)
    {
        //myUtils.console_log("add line y=", _y);
        y = _y;
        next_anim_time = 0;
        end_anim_time = Time.time + 1f;
        squaresList = new List<GameAreaSquare>();
        for (int x = 0; x < _squaresTypes.Count; x++)
        {
            GameAreaSquare sq = new GameAreaSquare();
            Vector2 crd = new Vector2(
                GameConstants.AREA_START.x + GameConstants.AREA_SQUARE_SIZE.x * ((float)x + 0.5f),
                GameConstants.AREA_START.y + GameConstants.AREA_SQUARE_SIZE.y * ((float)y + 0.5f)
            );
            //теперь генерация
            sq.Setup(crd, parent, GameConstants.AREA_SQUARE_SCALE, _squaresTypes[x], GameConstants.sANIM_FILL_LINE);
            squaresList.Add(sq);
            //_squares[0].SetAnimator(GameConstants.sANIM_FILL_LINE);
            //squaresList.Add(_squares[0]);
            //_squares.RemoveAt(0);
        }
    }

    //очередной шаг анимации, возвращает true, если закончена
    public bool Update()
    {
        if (Time.time >= end_anim_time)
            return true;

        if (Time.time >= next_anim_time)
        {
            next_anim_time = Time.time + GameConstants.AREA_FILLED_LINE_ANM.interval;
            //myUtils.console_log("animation line");

            for (int i = 0; i < squaresList.Count; i++)
            {
                squaresList[i].brick.transform.localScale -= squaresList[i].brick.transform.localScale / GameConstants.AREA_FILLED_LINE_ANM.delta;
            }
        }

        return false;
    }

    //удаление всех камней линии
    public void Clear()
    {
        for (int i = squaresList.Count-1; i>=0;  i--)
        {
            squaresList[0].Empty();
            squaresList.RemoveAt(0);
        }
        y = -1;
    }
}
