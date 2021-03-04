using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//==============================================
//структура одного квадрата
public class OneSquare : UnityEngine.Object
{
    public int type = GameConstants.SQUARE_EMPTY;  //номер префаба
    public string brick_name = ""; //имя префаба для повторного юза
    public GameObject brick = null;  //префаб брика

    public OneSquare()
    {
        type = GameConstants.SQUARE_EMPTY;
        brick_name = "";
        brick = null;
    }

    override public string ToString()
    {
        return type.ToString() + " " + brick_name; // + ", x: " + x + ", y: " + y;
    }
    //=======================================================================
    //очистка квадрата (удаление графических данных)
    public bool is_Empty()
    {
        return (type == GameConstants.SQUARE_EMPTY);
    }

    //=======================================================================
    //очистка квадрата (удаление графических данных)
    public void Empty(bool del_brick = true)
    {
        if (del_brick && (brick) )
        {
            //myUtils.console_log("delete");
            Destroy(brick);
        }
        type = GameConstants.SQUARE_EMPTY;
        brick_name = "";
        brick = null;
    }


    //=======================================================================
    // пересоздание квадрата заданного типа  или создание случайного типа ( при type = SQUARE_RANDOM)
    public void Create(Vector3 crd, Transform parent, Vector3 scale, int _type = GameConstants.SQUARE_RANDOM, string animator = GameConstants.sANIM_BASE)
    {
        Empty();
        if (_type == GameConstants.SQUARE_RANDOM)
        {
            _type = (int)UnityEngine.Random.Range(0f, (float)GameConstants.SQUARE_NUM_TYPES - 1f);
        }
        type = _type;
        brick_name = "Brick_" + type.ToString();
        string path_to_prefab = GameConstants.PATH_TO_SQUARES_PREFABS + brick_name;
        brick = (GameObject)Instantiate(Resources.Load(path_to_prefab));  //, parent
        brick.transform.SetParent(parent);
        brick.transform.localPosition = crd;
        brick.transform.localScale = scale;
        brick.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;

        SetAnimator(animator);
        //Animator anim = brick.GetComponent<Animator>();
        //anim.runtimeAnimatorController = (RuntimeAnimatorController)Instantiate(Resources.Load(animator)) as RuntimeAnimatorController;
        //anim.Play(0); // "Idle");
    }


    public void SetAnimator(string animator = GameConstants.sANIM_BASE)
    {
        if (brick) //!is_Empty())
        {
            Animator anim = brick.GetComponent<Animator>();
            anim.runtimeAnimatorController = (RuntimeAnimatorController)Instantiate(Resources.Load(animator)) as RuntimeAnimatorController;
            anim.Play(0); // "Idle");
        }
    }


}

