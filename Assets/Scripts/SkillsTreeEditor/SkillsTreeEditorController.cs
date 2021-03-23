using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

//главный управляющий скрипт редактора
public class SkillsTreeEditorController : MonoBehaviour
{
    //контенты от scroll_view
    //public GameObject goCategories; 
    //public GameObject goElements;


    SkillsCategoriesEditor scCategories;
    SkillsElementsEditor scElements;

    JsonSkillsTreeNode[] skillsTreeNodes;

    private int n = 0;

    


    // Start is called before the first frame update
    void Start()
    {
        scCategories =  GetComponent<SkillsCategoriesEditor>();
        scElements = GetComponent<SkillsElementsEditor>();

        //test_save_json_skill_tre();
        ReloadFromFile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void ReloadFromFile()
    {
        //загрузка из файла
        SkillsTreeFile.Load();
        //заполнение списков в классах-графах категорий и элементов
        scCategories.SetData(SkillsTreeFile.catList);
        scElements.SetData(SkillsTreeFile.elmsList);

        //обновление графа категорий + элементов текущей категории
        scCategories.FillGrap();
    }





    //***********************************************************************
    //  TESTS
    //***********************************************************************

    /*
        void TestAddElement()
        {
            //создание объекта из префаба
            GameObject obj = (GameObject)Instantiate(Resources.Load("Prefab/SkillsTreeEditor/element"));
            obj.transform.SetParent(pnlTest.transform);
            string name = "element_" + n.ToString();
            n++;
            obj.name = name;

            //добавление функций на кнопку
            Component [] btns;
            btns = obj.GetComponentsInChildren(typeof(Button));
            Debug.Log(btns);

            if (btns != null)
            {
                foreach (Button b in btns)
                {
                    b.onClick.AddListener(delegate { test_btn_click(name + " : " + b.name);  });
                }
            }
            else
            {
                Debug.Log("btns not found");
            }
        }


        void test_btn_click( string val )
        {
            Debug.Log("clicked " + val);
        }

    */


    void test_save_json_skill_tre()
    {
        JsonSkillsTreeNode[] skills = new JsonSkillsTreeNode[10];
        skills[0] = SkillsTreeFile.create_one_node(1, 0, 1, "Категория 1", "кор. описание к1", "полное описание к1");
        skills[1] = SkillsTreeFile.create_one_node(2, 0, 1, "Категория 2", "кор. описание к2", "полное описание к2");
        skills[2] = SkillsTreeFile.create_one_node(3, 1, 1, "Категория 1.1", "кор. описание к1.1", "полное описание к1.1");
        skills[3] = SkillsTreeFile.create_one_node(4, 1, 1, "Категория 1.2", "кор. описание к1.2", "полное описание к1.2");

        skills[4] = SkillsTreeFile.create_one_node(5, 3, 0, "Элемент 1.1.1", "кор. описание 1.1.1", "полное описание 1.1.1");
        skills[5] = SkillsTreeFile.create_one_node(6, 3, 0, "Элемент 1.1.2", "кор. описание 1.1.2", "полное описание 1.1.2");

        skills[6] = SkillsTreeFile.create_one_node(7, 2, 1, "Категория 2.1", "кор. описание к2.1", "полное описание к2.1");
        skills[7] = SkillsTreeFile.create_one_node(8, 2, 1, "Категория 2.2", "кор. описание к2.2", "полное описание к2.2");


        skills[8] = SkillsTreeFile.create_one_node(9, 7, 0, "Элемент 2.1.1", "кор. описание 2.1.1", "полное описание 2.1.1");
        skills[9] = SkillsTreeFile.create_one_node(10, 7, 0, "Элемент 2.1.2", "кор. описание 2.1.2", "полное описание 2.1.2");

        SkillsTreeFile.Save(skills);

    }

}




