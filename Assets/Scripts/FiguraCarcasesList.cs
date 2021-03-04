using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//=============================================
//перечень обозначений каркасов фигур, empty=пусто, random=для генерации рандомного
public enum CarcasType
{
    //random,
    empty,
    square, skoba_l, skoba_r, line_h, zigzag_l, zigzag_r, halfcros, sector, duga_l, duga_r, circle, check_l, check_r, lomanaya
}


//=============================================
//структура шанса появления того или иного каркаса
public class CarcaseChance
{
    //величина шанса появления
    public int chance;  
    //тип каркаса
    public CarcasType type;
    public CarcaseChance( int _c, CarcasType _t)
    {
        chance = _c;
        type = _t;
    }
}



//=============================================
//набор каркасов фигур
public static class FiguraCarcasesList
{
    public static CarcaseChance[] chances = {
        new CarcaseChance(12, CarcasType.square),
        new CarcaseChance(12, CarcasType.line_h),
        new CarcaseChance(12, CarcasType.halfcros),
        new CarcaseChance(12, CarcasType.square),
        new CarcaseChance(12, CarcasType.zigzag_l),
        new CarcaseChance(12, CarcasType.zigzag_r),
        new CarcaseChance(12, CarcasType.skoba_l),
        new CarcaseChance(12, CarcasType.skoba_r),
        //new CarcaseChance(2, CarcasType.sector),
        //new CarcaseChance(5, CarcasType.duga_l),
        //new CarcaseChance(5, CarcasType.duga_r),
        //new CarcaseChance(1, CarcasType.circle),
        //new CarcaseChance(1, CarcasType.check_l),
        //new CarcaseChance(1, CarcasType.check_r),
        //new CarcaseChance(1, CarcasType.lomanaya),
    };


    //=========================================
    //рандомный тип фигуры из словаря с учетом рандом-веса, каждой
    public static CarcasType get_RandomCarcasType()
    {
        int total_val = 0;
        for (int i=0; i<chances.Length; i++)
        {
            total_val += chances[i].chance;
        }
        int rand_val = UnityEngine.Random.Range(1, total_val);
        int val = 0;
        for (int i = 0; i < chances.Length; i++)
        {
            val += chances[i].chance;
            if (rand_val <= val)
                return chances[i].type;
        }
        //return myUtils.GetRandomEnum<CarcasType>(1); //emty=0, 1 ... =  carcas types
        return CarcasType.square;
    }

    //получить каркас фигуры по ключу с данной ротацией (0..3 = 0,90,180,270)
    public static FiguraCarcas get_Carcas_byType(CarcasType key, int rotate = 0)
    {
        rotate = rotate % 4;
        FiguraCarcas baseCarcas = null;
        List<FiguraCarcas> carcas = new List<FiguraCarcas>();
        //myUtils.console_log("figura key =", key);
        switch (key)
        {
            case CarcasType.skoba_l:
                baseCarcas = new FiguraCarcas(4, new int[4, 2] { { -1, 1 }, { -1, 0 }, { 0, 0 }, { 1, 0 } });
                break;
            case CarcasType.skoba_r:
                baseCarcas = new FiguraCarcas(4, new int[4, 2] { { -1, 0 }, { 0, 0 }, { 1, 0 }, { 1, 1 }, });
                break;
            case CarcasType.zigzag_l:
                baseCarcas = new FiguraCarcas(4, new int[4, 2] { { -1, -1 }, { 0, -1 }, { 0, 0 }, { 1, 0 } });
                break;
            case CarcasType.zigzag_r:
                baseCarcas = new FiguraCarcas(4, new int[4, 2] { { -1, 1 }, { 0, 1 }, { 0, 0 }, { 1, 0 } });
                break;
            case CarcasType.halfcros:
                baseCarcas = new FiguraCarcas(4, new int[4, 2] { { -1, 0 }, { 0, 0 }, { 1, 0 }, { 0, 1 } });
                break;
            case CarcasType.line_h:
                baseCarcas = new FiguraCarcas(4, new int[4, 2] { { -1, 0 }, { 0, 0 }, { 1, 0 }, { 2, 0 } });
                break;
            case CarcasType.sector:
                baseCarcas = new FiguraCarcas(4, new int[4, 2] { { -1, -1 }, { 0, 0 }, { 1, 0 }, { 2, -1 } });
                break;
            case CarcasType.duga_l:
                baseCarcas = new FiguraCarcas(4, new int[4, 2] { { -1, -1 }, { 0, 0 }, { 1, 0 }, { 2, 0 } });
                break;
            case CarcasType.duga_r:
                baseCarcas = new FiguraCarcas(4, new int[4, 2] { { -1, 1 }, { 0, 0 }, { 1, 0 }, { 2, 0 } });
                break;
            case CarcasType.circle:
                baseCarcas = new FiguraCarcas(4, new int[4, 2] { { -1, 0 }, { 0, -1 }, { 1, 0 }, { 0, 1 } });
                break;
            case CarcasType.check_l:
                baseCarcas = new FiguraCarcas(4, new int[4, 2] { { -1, 0 }, { 0, -1 }, { 1, 0 }, { 2, 1 } });
                break;
            case CarcasType.check_r:
                baseCarcas = new FiguraCarcas(4, new int[4, 2] { { 1, 0 }, { 0, -1 }, { -1, 0 }, { -2, 1 } });
                break;
            case CarcasType.lomanaya:
                baseCarcas = new FiguraCarcas(4, new int[4, 2] { { 1, 0 }, { 0, -1 }, { -1, 0 }, { -2, -1 } });
                break;
            case CarcasType.square:
            default:
                carcas.Add(new FiguraCarcas(4, new int[4, 2] { { 0, 1 }, { 0, 0 }, { -1, 0 }, { -1, 1 } }));
                carcas.Add(new FiguraCarcas(4, new int[4, 2] { { 0, 0 }, { -1, 0 }, { -1, 1 }, { 0, 1 } }));
                carcas.Add(new FiguraCarcas(4, new int[4, 2] { { -1, 0 }, { -1, 1 }, { 0, 1 }, { 0, 0 } }));
                carcas.Add(new FiguraCarcas(4, new int[4, 2] { { -1, 1 }, { 0, 1 }, { 0, 0 }, { -1, 0 } }));
                break;
        }

        if (key != CarcasType.square)
        {
            carcas.Add(baseCarcas);
            baseCarcas = RotateCarcas(baseCarcas, GameConstants.DIR_RIGHT);
            carcas.Add(baseCarcas);
            baseCarcas = RotateCarcas(baseCarcas, GameConstants.DIR_RIGHT);
            carcas.Add(baseCarcas);
            baseCarcas = RotateCarcas(baseCarcas, GameConstants.DIR_RIGHT);
            carcas.Add(baseCarcas);
        }

        return carcas[rotate];
    }

    //===================================================
    public static FiguraCarcas RotateCarcas(FiguraCarcas carcas, int dir)
    {
        FiguraCarcas tmpCarcas = new FiguraCarcas(carcas);
        int x, y;
        //поворот клона
        for (int i = 0; i < tmpCarcas.numPoints; i++)
        {
            if (dir == GameConstants.DIR_LEFT)
            {
                x = tmpCarcas.coords[i, 1];
                y = -tmpCarcas.coords[i, 0];
            }
            else
            {
                x = -tmpCarcas.coords[i, 1];
                y = tmpCarcas.coords[i, 0];
            }
            tmpCarcas.coords[i, 0] = x;
            tmpCarcas.coords[i, 1] = y;
        }

        return tmpCarcas;
    }
}




//=============================================
//каркас фигуры
public class FiguraCarcas : UnityEngine.Object
{
    public int numPoints { get; set; }
    public int[,] coords { get; set; }

    public override string ToString()
    {
        string str = "carcas, num=" + numPoints.ToString() + ", points=";
        for (int i = 0; i < numPoints; i++)
        {
            str = str + " [" + coords[i, 0].ToString() + "," + coords[i, 1].ToString() + "]";
        }
        return str;
    }

    public FiguraCarcas(int num, int[,] points)
    {
        numPoints = num;
        coords = new int[10, 2];
        Reinit(num, points);
    }
    public FiguraCarcas(FiguraCarcas fd)
    {
        numPoints = fd.numPoints;
        coords = new int[10, 2];
        Reinit(fd.numPoints, fd.coords);
    }

    public void Reinit(int num, int[,] points)
    {
        numPoints = num;
        for (int i = 0; i < numPoints; i++)
        {
            coords[i, 0] = points[i, 0];
            coords[i, 1] = points[i, 1];
        }
    }

    public void Reinit(FiguraCarcas fd)
    {
        Reinit(fd.numPoints, fd.coords);
    }


    public int getMinD(int j)
    {
        int min = 100;
        for (int i = 0; i < numPoints; i++)
            if (min > coords[i, j])
                min = coords[i, j];
        return min;
    }
    public int getMaxD(int j)
    {
        int max = -100;
        for (int i = 0; i < numPoints; i++)
            if (max < coords[i, j])
                max = coords[i, j];
        return max;
    }

    public Vector2Int getMinV2Int()
    {
        return new Vector2Int(getMinD(0), getMinD(1));
    }
    public Vector2Int getMaxV2Int()
    {
        return new Vector2Int(getMaxD(0), getMaxD(1));
    }

}