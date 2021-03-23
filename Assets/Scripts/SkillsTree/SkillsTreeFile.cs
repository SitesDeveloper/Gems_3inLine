using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class SkillsTreeFile
{
    public static List<JsonSkillsTreeNode> catList = new List<JsonSkillsTreeNode>();
    public static List<JsonSkillsTreeNode> elmsList = new List<JsonSkillsTreeNode>();

    public static bool is_changed = false;

    //****************************************************************
    //загрузка из файла и заполнение списков
    public static JsonSkillsTreeNode[] Load() {

        string fname = PathUtil.GetDocumentFilePath(GameConstants.PATH_TO_SKILLS_TREE);
        if (!File.Exists(fname)) { 
            throw new Exception("Not found skills_tree file");
        }
        string jsonString = "";
        jsonString = File.ReadAllText(fname);
        JsonSkillsTreeNode[] skillsArray = JsonHelper.FromJson<JsonSkillsTreeNode>(jsonString);

        ArrayToLists(skillsArray);

        //Debug.Log(skillsArray);
        return skillsArray;
    }


    //****************************************************************
    //заполнение списков (категории и элементы) из массива  + их сортировка
    public static bool ArrayToLists( JsonSkillsTreeNode[] skillsArray )
    {
        catList.Clear();
        elmsList.Clear();

        foreach (JsonSkillsTreeNode skillsNode in skillsArray)
        {
            if (skillsNode.is_category == 1)
            {
                catList.Add(skillsNode);
            } else
            {
                elmsList.Add(skillsNode);
            }
        }

        SortCategories();
        SortElements();

        return true;
    }


    //****************************************************************
    //сортировка категорий по двум полям: parent_id и sort
    public static void SortCategories()
    {
        List<JsonSkillsTreeNode> newCatList = new List<JsonSkillsTreeNode>();
        List<JsonSkillsTreeNode> rootCats = new List<JsonSkillsTreeNode>();

        foreach (JsonSkillsTreeNode node in catList)
        {
            if (node.parent_id == 0)
            {
                rootCats.Add(node);
            }
        }

        catList.Sort(delegate (JsonSkillsTreeNode a, JsonSkillsTreeNode b)
        {
            if (a.parent_id > b.parent_id)
                return 1;
            if (a.parent_id < b.parent_id)
                return -1;
            //если родители равны, то по индексу сортировки
            if (a.sort > b.sort)
                return 1;
            if (a.sort < b.sort)
                return -1;
            return 0;
        });

        foreach (JsonSkillsTreeNode rootItem in rootCats)
        {
            newCatList.Add(rootItem);
            foreach (JsonSkillsTreeNode subItem in catList)
            {
                if (subItem.parent_id == rootItem.id)
                {
                    newCatList.Add(subItem);
                }
            }
        }
        catList.Clear();
        catList.AddRange(newCatList);


        /*
        catList.Sort(delegate (JsonSkillsTreeNode a, JsonSkillsTreeNode b)
        {
            //субкатегории идут вперед следующей корневой категории
            if ( ( b.parent_id < a.id)  && (b.parent_id!=0) )
                return -1;

            if (a.parent_id > b.parent_id)
                return 1;
            if (a.parent_id < b.parent_id)
                return -1;
            //если родители равны, то по индексу сортировки
            if (a.sort > b.sort)
                return 1;
            if (a.sort < b.sort)
                return -1;
            return 0;
        });
        */
    }


    //****************************************************************
    //сортировка элементов по двум полям: parent_id и sort
    public static void SortElements()
    {
        elmsList.Sort(delegate (JsonSkillsTreeNode a, JsonSkillsTreeNode b)
        {
            if (a.parent_id > b.parent_id)
                return 1;
            if (a.parent_id < b.parent_id)
                return -1;
            //если родители равны, то по индексу сортировки
            if (a.sort > b.sort)
                return 1;
            if (a.sort < b.sort)
                return -1;
            return 0;
        });
    }




    //****************************************************************
    //функция обратная ArrayToLists()  заполнение массива из списков (категории и элементы)
    public static JsonSkillsTreeNode[] ListsToArray()
    {
        JsonSkillsTreeNode[] skillsArray = new JsonSkillsTreeNode[catList.Count + elmsList.Count];
        int n = 0;

        foreach (JsonSkillsTreeNode cat in catList)
        {
            skillsArray[n] = cat;
            n++;
        }
        foreach (JsonSkillsTreeNode elm in elmsList)
        {
            skillsArray[n] = elm;
            n++;
        }
        return skillsArray;
    }



    //****************************************************************
    //запись в файл
    public static void Save( JsonSkillsTreeNode[] jsonSkills ) {
        string jsonString = JsonHelper.ToJson(jsonSkills, true);
        string fname = PathUtil.GetDocumentFilePath(GameConstants.PATH_TO_SKILLS_TREE);
        File.WriteAllText(fname, jsonString );
    }




    //****************************************************************
    //распечатка списка
    public static void Log(List<JsonSkillsTreeNode> skillsList)
    {
        foreach (JsonSkillsTreeNode item in skillsList)
        {
            Debug.Log(item.id + " [" + item.parent_id + "] " + item.name + " (is_cat=" + item.is_category + ") ");
        }
    }




    //****************************************************************
    //создание одной ноды типа JsonSkillsTreeNode
    public static JsonSkillsTreeNode create_one_node(
        int id=0,  //ид-ноды
        int parent_id=0,   //ид-родительской ноды
        int is_category=1, //1=если категория
        string name = "имя",  //название ноды
        string short_desc = "короткое описание", //короткое описание
        string full_desc = "полный текст",  //подробное описание ноды (текст)
        int sort = 0 , //индекс сортировки среди "собратьев"
        string image = "", //путь к спрайту в ресурсах
        string requirements=""  //json-строк требований
    )
    {
        JsonSkillsTreeNode node = new JsonSkillsTreeNode();
        node.id = id;
        node.parent_id = parent_id;
        node.is_category = is_category;
        node.name = name;
        node.short_desc = short_desc;
        node.full_desc = full_desc;
        node.sort = sort;
        node.image = image;
        node.requirements = requirements;
        return node;
    }

}

