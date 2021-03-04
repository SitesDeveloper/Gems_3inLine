using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiguraController: MonoBehaviour  
{

    public CarcasType carcas_type;

    int rotate_index = 0;  //0..3  = 0,90,180,270
    FiguraCarcas carcas;
    List<OneSquare> squaresList = new List<OneSquare>();
    //координаты в области сборки
    Vector2Int coords = new Vector2Int(0, 0);
    //дельта координаты движения
    public Vector2 delta = new Vector2(0f, 0f);

    float last_move_time = 0;
    float last_click_time = 0;

    //запрет нажатия space при первом фреме (который мог быть с какой либо формы
    public bool anti_click_space_first_frame = true;

    GameController gameController;
    GameArea gameArea;

    //флаг суперскоростного режима
    bool flagFastSpeed = false;
    //интервал времени после прибазирования фигуры в течении которого можно ее двигать по свободным квадратам
    public float figure_area_acept_time = 0f;

    private void Awake()
    { 
        gameArea = FindObjectOfType<GameArea>();   // GetComponent
        gameController = FindObjectOfType<GameController>();  // GetComponent

        carcas_type = CarcasType.empty;
        carcas = null;
        squaresList = new List<OneSquare>();
    }

    //=============================================
    public bool is_Empty()
    {
        return (carcas_type == CarcasType.empty);
    }

    //=============================================
    //удалить квадраты фигуры
    public void delete_Figura()
    {

        if (squaresList.Count > 0)
        {
            while (squaresList.Count > 0)
            {
                Destroy(squaresList[0].brick);
                squaresList.RemoveAt(0);
            }
        }
        carcas_type = CarcasType.empty;
    }

    //=============================================
    //создать новую фигуру на базе фигуры из очереди (взять из нее каркас и типы камней)
    //queueFigura - каркас + типы квадратов (без графики)
    public void Create_Figura(OneQueueFigura queueFigura, Transform parent)
    {
        carcas_type = queueFigura.carcas_type;
        carcas = queueFigura.carcas;
        //myUtils.console_log(carcas_type, carcas);
        //создание фигуры по центру плашки
        Vector2 min = new Vector2((float)carcas.getMinD(0), (float)carcas.getMinD(1));
        Vector2 pt_center = new Vector2( (float)GameConstants.AREA_WIDTH / 2f,  -min.y + (float)GameConstants.AREA_VISUAL_HEIGHT);
        for (int i = 0; i < carcas.numPoints; i++)
        {
            int type = queueFigura.squaresList[i].type;
            float dx = GameConstants.AREA_START.x + ((float)carcas.coords[i, 0] + pt_center.x + 0.5f) * GameConstants.AREA_SQUARE_SIZE.x;  
            float dy = GameConstants.AREA_START.y + ((float)carcas.coords[i, 1] + pt_center.y + 0.5f) * GameConstants.AREA_SQUARE_SIZE.y;
            OneSquare square = myUtils.create_OneSquare(new Vector3(dx, dy, 0f), parent, GameConstants.AREA_SQUARE_SCALE, type, GameConstants.sANIM_BASE);
            squaresList.Add(square);
        }
        coords = new Vector2Int( 
            Mathf.RoundToInt(pt_center.x),
            Mathf.RoundToInt(pt_center.y)
        );
        delta = new Vector2(0f, 0f);
        last_move_time = 0f;
        rotate_index = 0;
        flagFastSpeed = false;
        figure_area_acept_time = 0;
        anti_click_space_first_frame = true;
    }

    //=============================================
    //возвращает центр фигуры
    public Vector3 get_CenterFigure()
    {
        return new Vector3(
            GameConstants.AREA_START.x + (float)coords.x * GameConstants.AREA_SQUARE_SIZE.x + 0.5f,  // - delta.x
            GameConstants.AREA_START.y + (float)coords.y * GameConstants.AREA_SQUARE_SIZE.y - delta.y + 0.5f, 
            0
        );
    }

    //=============================================
    public void MoveTo(Vector3 crd)
    {
        Vector3 pt_center = new Vector3(
            (float)coords.x * GameConstants.AREA_SQUARE_SIZE.x,
            (float)coords.y * GameConstants.AREA_SQUARE_SIZE.y, 0
        );
        pt_center = get_CenterFigure();
        for (int i = 0; i < squaresList.Count; i++)
        {
            squaresList[i].brick.transform.localPosition  = squaresList[i].brick.transform.localPosition - pt_center + crd;
        }
    }
    //=============================================
    public void MoveBy(Vector3 delta)
    {
        for (int i = 0; i < squaresList.Count; i++)
        {
            squaresList[i].brick.transform.localPosition = squaresList[i].brick.transform.localPosition + delta;
        }
    }


    //=============================================
    float fmod(float x, float y)
    {
        while (x >= y)
            x -= y;
        return x;
    }
    //=============================================
    int fnum(float x, float y)
    {
        int i=0;
        while (x >= y)
        {
            i++;
            x -= y;
        }
        return i;
    }

    //=============================================
    // Update is called once per frame
    private void Update()
    {
        if (GameController.gameState == GameState.Play)
        {
            if (is_Empty())
                return;

            if (is_blocked_on_pos(coords, carcas, 1f)) //delta.y))
            {
                //delta.y = 0f;
                //если фигура уже приземлилась (и ниже не может опуститься) 
                if (Time.time >= figure_area_acept_time)
                {
                    //и если прошло некое дельта времени, то прибазирование камней фигуры в обасти сборки
                    anti_click_space_first_frame = true;
                    gameArea.put_figura(coords, carcas, squaresList);
                    EventManager.TriggerEvent("NewFigura");
                }
            }
            else if (Time.time >= last_move_time)
            {
                //движение фигуры 
                last_move_time = Time.time + GameConstants.FIGURA_SPD.interval;
                float dy = 0;
                float spd = GameConstants.FIGURA_SPD.delta;
                if (flagFastSpeed)
                    spd = spd * 9.9f; //суммарная spd - должна быть меньше высоты 1 квадрата
                dy = delta.y + spd;
                if (dy >= GameConstants.AREA_SQUARE_SIZE.y )
                {
                    //кол-во квадратов на которое пролетела фигура
                    int num_squares = fnum(dy, GameConstants.AREA_SQUARE_SIZE.y);
                    //myUtils.console_log("num_squares", num_squares);
                    //по каждому квадрату проверка базирования фигуры
                    while (num_squares >0 )
                    {
                        Vector2Int new_coords = new Vector2Int(coords.x, coords.y - 1);
                        //myUtils.console_log("new_figure_crds", new_coords);
                        float ndy = (num_squares > 1) ? 0f : dy - GameConstants.AREA_SQUARE_SIZE.y; // только на последнем больше 0
                        if (is_blocked_on_pos(new_coords, carcas, ndy)) // delta.y)) 
                        {
                            //точка позиционирования - то насколько фигуру нужно подвинуть до выравнивания с рядом
                            dy = GameConstants.AREA_SQUARE_SIZE.y - delta.y;
                            MoveBy(new Vector3(0f, -dy, 0f));
                            coords = new_coords;
                            delta.y = 0f;
                            //ниже, после опроса клавиш - идет проверка базирования
                            //правило: фигуру можно можно считать прибазированной, если на ее новой позиции есть хоть один занятый квадрат в области или достигнута база
                            figure_area_acept_time = Time.time + GameConstants.FIGURA_AREA_ACEPT_TIME;
                            break;
                        }
                        else
                        {
                            if (num_squares > 1)
                            {
                                MoveBy(new Vector3(0f, -GameConstants.AREA_SQUARE_SIZE.y, 0f));
                                dy -= GameConstants.AREA_SQUARE_SIZE.y;
                            } else
                            {
                                //dy -= delta.y; //остаток преступающий линию ряда
                                MoveBy(new Vector3(0f, - (dy - delta.y), 0f));
                                delta.y = dy - GameConstants.AREA_SQUARE_SIZE.y;
                            }
                            coords.y = coords.y - 1;
                        }
                        num_squares--;
                    }

                } else
                {
                    if (!is_blocked_on_pos(coords, carcas, dy))
                    {
                        MoveBy(new Vector3(0f, -spd, 0f));
                        delta.y = dy;
                    }
                }
            }


            if (Time.time >= last_click_time)
            {
                bool changeTime = false;
                if (Input.GetKey("left"))
                {
                    changeTime = true;
                    MoveFiguraHorizontal(GameConstants.DIR_LEFT);
                }
                else if (Input.GetKey("right"))
                {
                    changeTime = true;
                    MoveFiguraHorizontal(GameConstants.DIR_RIGHT);
                }

                if (changeTime)
                {
                    last_click_time = Time.time + GameConstants.WAIT_GET_KEY;
                }
            }


            if (Input.GetKeyDown("space") && (!anti_click_space_first_frame) )
            {
                flagFastSpeed = !flagFastSpeed;
            }
            if (Input.GetKeyDown("down"))
            {
                RotateFigura(GameConstants.DIR_LEFT);
            }
            else if (Input.GetKeyDown("up"))
            {
                RotateFigura(GameConstants.DIR_RIGHT);
            }


            anti_click_space_first_frame = false;


        }
    }

    //=============================================
    //проверка наличия заполненных квадратов на новом месте фигуры
    bool is_blocked_on_pos( Vector2Int m, FiguraCarcas fd, float dy )
    {
        for (int i = 0; i < fd.numPoints; i++)
        {
            if ((gameArea.is_disabled(m.x + fd.coords[i, 0], m.y + fd.coords[i, 1])) ||
                     ((dy > 0f) && gameArea.is_disabled(m.x + fd.coords[i, 0], m.y + fd.coords[i, 1] - 1)))
                return true;
        }
        return false;
    }


    //=============================================
    //движение влево-вправо
    void MoveFiguraHorizontal(int dir)
    {
        if ((dir == GameConstants.DIR_LEFT) && (coords.x + carcas.getMinD(0) > 0))
        {
            if (!is_blocked_on_pos(new Vector2Int(coords.x - 1, coords.y), carcas, delta.y))
            {
                coords.x--;
                MoveBy(new Vector3(-GameConstants.AREA_SQUARE_SIZE.x, 0f, 0f));
            }
        }
        else if ((dir == GameConstants.DIR_RIGHT) && ((coords.x + carcas.getMaxD(0) + 1) < GameConstants.AREA_WIDTH))
        {
            if (!is_blocked_on_pos(new Vector2Int(coords.x + 1, coords.y), carcas, delta.y))
            {
                coords.x++;
                MoveBy(new Vector3(GameConstants.AREA_SQUARE_SIZE.x, 0f, 0f));
            }
        }
    }


    //=============================================
    //поворот фигуры по направлению
    void RotateFigura(int dir)
    {

        //if ( carcas_type == "square") return;
        //myUtils.console_log("rotate_figure");
        //создание клона фигуры и проверка
        int new_rotate = rotate_index;
        if (dir == GameConstants.DIR_LEFT)
            new_rotate--;
        else
            new_rotate++;
        if (new_rotate < 0)
            new_rotate += 4;
        new_rotate = new_rotate % 4;

        //попытка поворота и сдвига
        //получить каркас с поворотом
        FiguraCarcas tmpCarcas = FiguraCarcasesList.get_Carcas_byType( carcas_type, new_rotate);
        //myUtils.console_log("new_carcas = ", tmpCarcas);
        //при проверке нужно учесть, что возможно фигуру нужно подвинуть влево-вправо от бордюра на несколько квадратов
        //определение сдвига фигуры от края (если надо)
        Vector2Int new_min = tmpCarcas.getMinV2Int();
        Vector2Int new_max = tmpCarcas.getMaxV2Int();
        int shift_x = 0;
        int new_x = coords.x;
        if (coords.x + new_min.x < 0)
            shift_x = -coords.x + Mathf.Abs(new_min.x );  
        if (coords.x + new_max.x >= GameConstants.AREA_WIDTH)
            shift_x = -coords.x + GameConstants.AREA_WIDTH - (1 + new_max.x);
        new_x += shift_x;
        
        //если фигура изначально (до поворота) была прислонена к краю, то тоже ее придвинуть туда
        Vector2Int old_min = carcas.getMinV2Int();
        Vector2Int old_max = carcas.getMaxV2Int();
        if ( (coords.x + old_min.x) <= 0)
        {
            //фигура должна быть прижата к левому борту
            if ( new_x + new_min.x > 0)
            {
                shift_x =  -coords.x + Mathf.Abs(new_min.x);
                new_x = coords.x + shift_x;
            }
        }
        if ((coords.x + old_max.x+1) >= GameConstants.AREA_WIDTH)
        {
            //фигура должна быть прижата к правому борту
            if (new_x + new_max.x + 1 < GameConstants.AREA_WIDTH)
            {
                shift_x = -coords.x + GameConstants.AREA_WIDTH - (1+new_max.x);
                new_x = coords.x + shift_x;
            }
        }

        if (is_blocked_on_pos(new Vector2Int(new_x, coords.y), tmpCarcas, delta.y))
        {
            //myUtils.console_log("blocked", new_x, new_min.x, shift_x, coords.x);
            return;
        }


        //сохранение повернутого каркаса
        carcas.Reinit(tmpCarcas);
        //перемещение графических объектов
        for ( int c=0; c<carcas.numPoints; c++)
        {
            squaresList[c].brick.transform.localPosition = new Vector3(
                GameConstants.AREA_START.x + ((float)carcas.coords[c, 0] + (float)coords.x + 0.5f) * GameConstants.AREA_SQUARE_SIZE.x,
                GameConstants.AREA_START.y + ((float)carcas.coords[c, 1] + (float)coords.y + 0.5f) * GameConstants.AREA_SQUARE_SIZE.y - delta.y,
                0
            );
            //myUtils.console_log(old_crd, "->", new_crd);
        }
        MoveBy(new Vector3((float)shift_x * GameConstants.AREA_SQUARE_SIZE.x, 0f, 0f));
        //учет сдвига
        coords.x = new_x;
        rotate_index = new_rotate;
        return; // true;
    }



}
