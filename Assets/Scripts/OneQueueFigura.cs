using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//фигура в области очереди фигур
public class OneQueueFigura : UnityEngine.Object
{

    public CarcasType carcas_type;
    public FiguraCarcas carcas;
    public List<OneSquare> squaresList = new List<OneSquare>();
    GameObject plashka;

    //=============================================
    //удалить квадраты фигуры
    public void DestroyGraphicsObjects()
    {
        if (squaresList.Count > 0)
        {
            while (squaresList.Count > 0)
            {
                Destroy(squaresList[0].brick);
                squaresList.RemoveAt(0);
            }
        }
        Destroy(plashka);
    }

    //=============================================
    // конструктор
    public void Create_Queue_Figura( CarcasType type, Transform parent )
    {
        carcas_type = type;
        carcas = FiguraCarcasesList.get_Carcas_byType(carcas_type);
        // создание плашки 
        DestroyGraphicsObjects();
        plashka = (GameObject)Instantiate( Resources.Load(GameConstants.PREFAB_PLASHKA), parent); //, parent
        plashka.transform.SetParent(parent);
        plashka.transform.localPosition = new Vector3(0f, 0f, 0f);  //localPosition  GameConstants.QUEUE_ITEM_HEIGHT / 2f
        plashka.GetComponent<SpriteRenderer>().maskInteraction = SpriteMaskInteraction.VisibleInsideMask;
        //plashka.transform.localScale = GameConstants.PLASHKA_SCALE;
        //Animator anim = prefab.GetComponent<Animator>();
        //anim.runtimeAnimatorController = (RuntimeAnimatorController)Instantiate(Resources.Load(animation)) as RuntimeAnimatorController;
        //anim.Play("Idle");

        //создание фигуры по центру плашки
        Vector2 min = new Vector2((float)carcas.getMinD(0), (float)carcas.getMinD(1));
        Vector2 size = new Vector2((float)carcas.getMaxD(0) - min.x + 1f, (float)carcas.getMaxD(1) - min.y + 1f);
        //Ширина плашки = 5 x Ширина квадрата , поэтому  crd =  Xn-Xmin - FIGURA_WIDTH/2 +  1/2
        //Высота плашки = 3.9 x Высоты квадрата
        for (int i = 0; i < carcas.numPoints; i++)
        {
            float dx = (0.5f - min.x - size.x/2f + (float)carcas.coords[i, 0])* GameConstants.QUEUE_SQUARE_SIZE.x;    //f - maxwidth
            float dy = (0.45f - min.y - size.y/2f + (float)carcas.coords[i, 1])* GameConstants.QUEUE_SQUARE_SIZE.y ;
            //myUtils.console_log(dx, dy);
            OneSquare square = myUtils.create_OneSquare(new Vector3(dx, dy, 0f), plashka.transform, GameConstants.QUEUE_SQUARE_SCALE, -1, GameConstants.sANIM_BASE);
            squaresList.Add(square);
        }
        //plashka.transform.localPosition = plashka.transform.localPosition + new Vector3(0f, GameConstants.QUEUE_ITEM_HEIGHT/2f, 0f);  //localPosition

    }

    //=============================================
    public void MoveTo(Vector3 crd)
    {
        plashka.transform.localPosition = crd;  //local
    }
    //=============================================
    public void MoveBy(Vector3 delta)
    {
        plashka.transform.localPosition = plashka.transform.localPosition + delta;
    }

    //=============================================
    public void GetDataForGameArea()
    {
        //список типов камней, которые использовались в фигуре
    }

}




