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

    //фон попапов
    public GameObject pnl_popups_backfon;



    SkillsCategoriesEditor scCategories;
    SkillsElementsEditor scElements;

    JsonSkillsTreeNode[] skillsTreeNodes;

    private int n = 0;

    public bool is_inited = false;


    // Start is called before the first frame update
    void Start() 
    {
        scCategories =  GetComponent<SkillsCategoriesEditor>();
        scElements = GetComponent<SkillsElementsEditor>();

    }

    // Update is called once per frame
    void Update()
    {
        if (!is_inited)
        {
            //test_save_json_skill_tre();
            ReloadFromFile();
            is_inited = true;
        }
    }


    public void ReloadFromFile()
    {
        //загрузка дерева из файла в статичный класс дерева (через него будет доступ к файлу)
        SkillsTreeFile.Load();
        //заполнение списков в классах-графах категорий и элементов
        scElements.reloadFromFile();
        scCategories.reloadFromFile();

        //обновление графа категорий + элементов текущей категории
        scCategories.RefreshGrap();
    }


    // SAVE TO FILE
    // RELOAD FROM FILE



    //***********************************************************************
    //закрытие попапа
    public void btn_popup_close()
    {
        pnl_popups_backfon.SetActive(false);
    }


    //***********************************************************************
    //запись в файл
    public void btn_SaveToFile()
    {

        MessageBox.ShowMessage(
            () => {
                int num = SkillsTreeFile.Save();
                MessageBox.ShowMessage(
                    () => {
                    },
                    "Запись в файл произведена. Сохранено "+num+" нодов."
                );
            },
            "Хотите произвести запись всех изменений в файл?",
            true
        );

    }



    //***********************************************************************
    //загрузка из файла
    public void btn_LoadFromFile()
    {
        
    }






    //***********************************************************************
    //  TESTS
    //***********************************************************************

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

        SkillsTreeFile.SaveOtherNodes(skills);

    }

}




