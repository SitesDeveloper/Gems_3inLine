using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RunSacredList : MonoBehaviour
{
    public float next_move_time = 0f;

    //время добавления нового обетования
    public float time_next_add_new_sacred = 0f;

    public SacredTexts sacredTexts;

    //================================================
    // Start is called before the first frame update
    void Start()
    {
        sacredTexts.Init();
        /*
        set_next_sacred_time(3600f);
        EventManager.TriggerEvent("NewSacredText");
        */
    }
    //================================================
    public void set_next_sacred_time(float val)
    {
        time_next_add_new_sacred = Time.time + val;
    }

    //================================================
    public string getRandomSacred()
    {
        return sacredTexts.getRandomSacred();
    }

    //================================================
    public int Count()
    {
        Text[] objs = GetComponentsInChildren<Text>();
        return objs.Length;
    }


    //================================================
    // Update is called once per frame
    void Update()
    {
        if (GameController.gameState == GameState.Play)
        {

            if (Time.time >= next_move_time)
            {
                next_move_time = Time.time + 0.05f;
                ArrangeList();
                float btm_y = getBottomY();
                //движение и зацикливание текстов
                Text[] objs = GetComponentsInChildren<Text>();
                //движение? если текст ушел вверх, перенос его вниз (карусель)
                for (int i = 0; i < objs.Length; i++)
                {
                    objs[i].transform.localScale = new Vector3(1f, 1f, 1f);
                    objs[i].transform.localPosition += new Vector3(0f, 1f, 0f);
                    if (objs[i].rectTransform.rect.height > 0f)
                    {
                        float btm = objs[i].transform.localPosition.y - objs[i].rectTransform.rect.height;
                        //myUtils.console_log("btm=", btm, objs[i].rectTransform.rect.height);
                        if (btm >= 0f)
                        {
                            objs[i].transform.localPosition = new Vector3(0f, btm_y, 0f);
                            btm_y -= objs[i].rectTransform.rect.height - 20f;
                            objs[i].color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
                        }
                    }
                }
            }

            if (Time.time >= time_next_add_new_sacred)
            {
                //добавление еще одного обетования по таймеру
                Text[] objs = GetComponentsInChildren<Text>();
                if (objs.Length < 100)
                {
                    //чтобы еще раз не сработало, пока открыто одно окно
                    set_next_sacred_time(3600f);
                    EventManager.TriggerEvent("NewSacredText");
                }
            }

        }
    }

    //================================================
    //получить нижнюю ординату
    public float getBottomY(bool is_btm = true)
    {
        Text[] objs = GetComponentsInChildren<Text>();
        float btm_y = 0f;
        for (int i = 0; i < objs.Length; i++)
        {
            float btm = objs[i].transform.localPosition.y - objs[i].rectTransform.rect.height - 20f;
            if (btm_y > btm)
                btm_y = btm;
        }
        if ((is_btm || (objs.Length <= 0)) && (Mathf.Abs(btm_y) < GetComponent<RectTransform>().rect.height))
            btm_y = -GetComponent<RectTransform>().rect.height;
        //myUtils.console_log("btm_y=", btm_y);
        return btm_y;
    }

    //================================================
    //добавить текст
    public void AddSacredText(string new_text)
    {
        //myUtils.console_log("rect=", GetComponent<RectTransform>().rect);
        float btm_y = getBottomY(false);

        GameObject flyText = (GameObject)Instantiate(Resources.Load(GameConstants.PREFAB_SACRED_TEXT));
        flyText.transform.SetParent(transform);
        flyText.transform.localPosition = new Vector3(0f, btm_y, 0f);
        Text t = flyText.GetComponent<Text>();
        t.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        t.text = new_text;
        //время добавления следующего обетования (через 5 минут)
        set_next_sacred_time( GameConstants.SACRED_TEXT_TIME_INTERVAL );
    }


    //================================================
    //выравнивание блоков текста друг под дружкой (если высота у них у всех > 0 )
    public void ArrangeList()
    {
        int i, j, iterations = 0;
        Text[] objs = GetComponentsInChildren<Text>();
        //проверка все ли тексты в норме (если высота 0, значит расчета еще не было)
        float top_y = -100000f;
        for (i = 0; i < objs.Length; i++)
        {
            if (objs[i].rectTransform.rect.height <= 0f)
                return;
            if (top_y < objs[i].transform.localPosition.y)
                top_y = objs[i].transform.localPosition.y;
        }

        //определить порядок следования элементов по y ординате по убыванию
        List<int> iy = new List<int>();   //массив индексов
        iterations = 0;
        while (iy.Count < objs.Length)
        {
            for (i = 0; i < objs.Length; i++)
            {
                if (iy.IndexOf(i) >= 0)
                    continue;
                bool is_biggest = true;
                for (j = 0; j < objs.Length; j++)
                {
                    if ((j == i) || (iy.IndexOf(j) >= 0))
                        continue;
                    if (objs[i].transform.localPosition.y < objs[j].transform.localPosition.y)
                    {
                        is_biggest = false;
                        break;
                    }
                }
                if (is_biggest)
                {
                    iy.Add(i);
                }
            }
            iterations++;
            if (iterations >= objs.Length)
                break;
        }

        //установка текстов друг за дружкой сверху вниз
        for (i = 0; i < iy.Count; i++)
        {
            j = iy[i];
            objs[j].transform.localPosition = new Vector3(0f, top_y, 0f);
            top_y -= objs[j].rectTransform.rect.height + 20f;
        }

    }


    //================================================
    //смена цвета текста
    public void change_colors()
    {
        //защита от клика по пробелу
        if (Input.GetKey(KeyCode.Space))
            return;
        Text[] objs = GetComponentsInChildren<Text>();
        for (int i = 0; i < objs.Length; i++)
        {
            objs[i].color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f), 1f);
        }
    }



}