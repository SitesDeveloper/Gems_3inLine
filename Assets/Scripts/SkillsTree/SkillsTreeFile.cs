﻿using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class SkillsTreeFile
{
    public static List<JsonSkillsTreeNode> catList = new List<JsonSkillsTreeNode>();
    public static List<JsonSkillsTreeNode> elmsList = new List<JsonSkillsTreeNode>();

    public static bool is_changed = false;
    public static int sort_step = 10;

    //****************************************************************
    //загрузка из файла и заполнение списков
    public static JsonSkillsTreeNode[] Load(string fname="") {
        if (fname.Length<=0)
            fname = PathUtil.GetDocumentFilePath(GameConstants.PATH_TO_SKILLS_TREE);
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
    public static int getNextID()
    {
        int max_id = 1;
        foreach( JsonSkillsTreeNode node in catList)
        {
            if (node.id > max_id) max_id = node.id;
        }
        foreach (JsonSkillsTreeNode node in elmsList)
        {
            if (node.id > max_id) max_id = node.id;
        }
        max_id++;
        return max_id;
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

        //сортировка корневых разделов
        rootCats.Sort(delegate(JsonSkillsTreeNode a, JsonSkillsTreeNode b) {
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
    //запись в файл текущих данных, возвращает кол-во записанных nodes
    public static int Save(string fname="")
    {
        if (fname.Length<=0)
            fname = PathUtil.GetDocumentFilePath(GameConstants.PATH_TO_SKILLS_TREE);
        JsonSkillsTreeNode[] jsonSkills = ListsToArray();
        string jsonString = JsonHelper.ToJson(jsonSkills, true);
        File.WriteAllText(fname, jsonString);
        return jsonSkills.Length;
    }



    //****************************************************************
    //запись в файл произвольных нод
    public static void SaveOtherNodes( JsonSkillsTreeNode[] jsonSkills ) {
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
    //добавление/правка одной категории
    public static int addEditCategory(JsonSkillsTreeNode newNode)
    {
        int new_id = 0;
        if (newNode.id > 0)
        {
            new_id = newNode.id;
            int i = catList.IndexOf(catList.Find(n => n.id == newNode.id));
            if (i >= 0)
            {
                catList[i] = newNode;
            }
            else
            {
                throw new Exception("Категория с данным id=" + newNode.id + " не найдена");
            }
        }
        else
        {
            new_id = newNode.id = getNextID();
            catList.Add(newNode);
        }
        SortCategories();
        //Save(ListsToArray());
        return new_id;
    }


    //****************************************************************
    // удаление одной категории, возвращает кол-во удаленных категорий
    public static int deleteCategoryAndSubnodes(int id)
    {
        int deleted_nodes = 0;

        //получить категорию и все е дочерние категории
        List<JsonSkillsTreeNode> catNodes = catList.FindAll(x => (x.parent_id == id) || (x.id == id));
        if (catNodes.Count > 0)
        {
            foreach (JsonSkillsTreeNode catNode in catNodes)
            {
                //удалить элементы каждой категории
                //List<JsonSkillsTreeNode> elmsNodes = elmsList.FindAll(x => x.parent_id == catNode.id);
                deleted_nodes += elmsList.RemoveAll(x => x.parent_id == catNode.id);
            }
            deleted_nodes += catList.RemoveAll(x => (x.parent_id == id) || (x.id == id));
        }
        else
        {
            throw new Exception("Категория с данным id=" + id + " не найдена");
        }
        return deleted_nodes;
    }

    //****************************************************************
    //ренумерация всех категорий с шагом 10
    public static void renumAllCats()
    {
        if (catList.Count>0)
        {
            int sort = sort_step;
            for (int i=0; i<catList.Count; i++)
            {
                catList[i].sort = sort;
                sort += sort_step;
            }
        }
    }





    //****************************************************************
    //добавление/правка одного элемента
    public static int addEditElement(JsonSkillsTreeNode newNode)
    {
        int new_id = 0;
        if (newNode.id > 0)
        {
            new_id = newNode.id;
            int i = elmsList.IndexOf(elmsList.Find(n => n.id == newNode.id));
            if (i > 0)
            {
                elmsList[i] = newNode;
            }
            else
            {
                throw new Exception("Елемент с данным id=" + newNode.id + " не найден");
            }
        }
        else
        {
            new_id = newNode.id = getNextID();
            elmsList.Add(newNode);
        }
        SortElements();
        //Save(ListsToArray());
        return new_id;
    }



    //****************************************************************
    // удаление одной категории, возвращает кол-во удаленных категорий
    public static bool deleteElement(int id)
    {
        int num = elmsList.RemoveAll(x => x.id == id);
        return (num > 0);
    }



    //****************************************************************
    //ренумерация элементов данной категории с шагом sort_step
    public static void renumAllElements(int cat_id)
    {
        if (elmsList.Count > 0)
        {
            int sort = sort_step;
            for (int i = 0; i < elmsList.Count; i++)
            {
                if (elmsList[i].parent_id != cat_id)
                    continue;
                elmsList[i].sort = sort;
                sort += sort_step;
            }
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

