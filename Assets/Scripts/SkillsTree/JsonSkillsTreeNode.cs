using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class JsonSkillsTreeNode
{
    public int id;  //ид-ноды
    public int parent_id;   //ид-родительской ноды
    public int is_category; //1=если категория
    //public int level; // уровень вложенности (вычисляемое поле, возможно пригодится)
    public int sort; //индекс сортировки среди "собратьев"
    public string image; //путь к спрайту в ресурсах
    public string name;  //название ноды
    public string short_desc; //короткое описание
    public string full_desc;  //подробное описание ноды (текст)
    public string requirements;  //json-строк требований
}

