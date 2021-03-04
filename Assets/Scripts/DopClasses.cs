using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DopClasses : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
}






//==============================================
[Serializable]
public class RequstDataList
{
    public string IS_OK;
    public string msg;
    //public RequstData[] post;
    public string post;

    public static RequstDataList CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<RequstDataList>(jsonString);
    }

    override public string ToString()
    {
        return "IS_OK: " + IS_OK + ", msg: " + msg + ", post: " + post;
    }

}
[System.Serializable]
public class RequstData
{
    public string action;
    public string name1;

    public static RequstData CreateFromJSON(string jsonString)
    {
        return JsonUtility.FromJson<RequstData>(jsonString);
    }
}


//=============================================
//класс скорости
public class DeltaInterval
{
    //интервал времени через которое осуществлять движение
    public float interval;
    //длина отрезка на сколько нужно переместить объект
    public float delta;
    public DeltaInterval(float _interval, float _delta)
    {
        interval = _interval;
        delta = _delta;
    }
}



//=============================================
//статусы игры 
public enum GameState
{
    NotPlay,  //не в процессе игры 
    Play,   //процесс игры
    AnimAreaOverfill, //проигрывание глобальных анимаций (типа рестарт уровня, удаление линий, юзер в это время не играет)
    AnimLevelUp,
    Pause,  //пауза
    Menu
}

//=============================================
//типы очков
public enum ScoreType
{
    FigurePut,
    LineFill,
    LevelUp
}


