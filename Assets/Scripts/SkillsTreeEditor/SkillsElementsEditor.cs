using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillsElementsEditor : MonoBehaviour
{
    //граф элементов = контент от scroll_view
    public GameObject goElements;
    public Color normalColor;
    public Color selectedColor;

    public TMP_Text tmptxt_elm_preview;

    SkillsCategoriesEditor skillsCategories;


    //общий список элементов со всех категорий
    List<JsonSkillsTreeNode> elmsList = new List<JsonSkillsTreeNode>();
    //элементы текущей категории из общего списка элементов 
    List<JsonSkillsTreeNode> elmsCurCat = new List<JsonSkillsTreeNode>();


    //текущий выбранный элемент
    private int curElmID = 0;


    // Start is called before the first frame update
    void Start()
    {
        skillsCategories = GetComponent<SkillsCategoriesEditor>();
        ClearGraph();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //***********************************************************************
    //установить данные нодов элементов
    public void SetData(List<JsonSkillsTreeNode> nodes)
    {
        elmsList.Clear();
        elmsList.AddRange(nodes);
    }


    //***********************************************************************
    //очистка списка элементов
    void ClearGraph()
    {
        if (goElements.transform.childCount > 0)
        {
            foreach (Transform elm in goElements.transform) Destroy(elm.gameObject);
        }
        curElmID = 0;
    }

    //***********************************************************************
    public string CreateNameByID(int id)
    {
        return "element_" + id;
    }

    //***********************************************************************
    //заполнение списка элементов
    public void UpdateListByCatID( int catID )
    {
        ClearGraph();
        if (elmsList.Count > 0)
        {
            elmsCurCat.Clear();
            foreach (JsonSkillsTreeNode node in elmsList)
            {
                if (node.parent_id == catID) {
                    elmsCurCat.Add(node);
                }
            }


            if (elmsCurCat.Count > 0)
            {

                elmsCurCat.Sort(delegate (JsonSkillsTreeNode a, JsonSkillsTreeNode b)
                {
                    //сортировка по индексу
                    if (a.sort > b.sort)
                        return 1;
                    if (a.sort < b.sort)
                        return -1;
                    return 0;
                });

                foreach (JsonSkillsTreeNode node in elmsCurCat)
                {
                    string prefab_name = GameConstants.PREFAB_EDITOR_ELEMENT;
                    string spr_name = "item";

                    GameObject obElm = (GameObject)Instantiate(Resources.Load(prefab_name));
                    obElm.transform.SetParent(goElements.transform);
                    string name = CreateNameByID(node.id);
                    obElm.name = name;
                    obElm.transform.Find("tmp_name").GetComponent<TMP_Text>().text = node.name;
                    obElm.transform.Find("tmp_id").GetComponent<TMP_Text>().text = node.id.ToString();
                    GameObject img = obElm.transform.Find("pnl_img").Find("Image").gameObject;
                    img.GetComponent<Image>().sprite = GameConstants.GetSprite(spr_name);
                    Button btn = obElm.GetComponentInChildren<Button>();
                    btn.onClick.AddListener(delegate { Set_Element_Active(node.id); });
                }

                curElmID = elmsCurCat[0].id;
            }
        }

        Set_Element_Active(curElmID);

    }

    //***********************************************************************
    //получить ID из строки имени вида element_ID
    public int ExtractIDfromName(string name)
    {
        string[] words = name.Split(new char[] { '_' }, StringSplitOptions.RemoveEmptyEntries);
        int id = 0; ;
        if ((words.Length == 2) && Int32.TryParse(words[1], out id))
        {
            return id;
        }
        return 0;
    }


    //***********************************************************************
    public JsonSkillsTreeNode getElmNodeByID(int id)
    {
        return elmsCurCat.Find(x => x.id == id);
        //if (elmsCurCat.Count > 0)        {        }
        //return null;
    }

    //***********************************************************************
    public void Set_Element_Active(int id)
    {
        curElmID = id;
        string name = CreateNameByID(id);
        tmptxt_elm_preview.text = "";


        if (goElements.transform.childCount > 0)
        {
            foreach (Transform gElm in goElements.transform)
            {
                Button btn = gElm.GetComponent<Button>();
                ColorBlock cb = btn.colors;
                if (gElm.name == name)
                {
                    cb.normalColor = selectedColor;
                    btn.colors = cb;
                    btn.tag = "btn_selected";
                    //JsonSkillsTreeNode nodeElm = getElmNodeByID(ExtractIDfromName(name));
                    JsonSkillsTreeNode nodeElm = getElmNodeByID( id );
                    tmptxt_elm_preview.text = nodeElm.full_desc;

                }
                else
                {
                    cb.normalColor = normalColor;
                    btn.colors = cb;
                    btn.tag = "btn_not_selected";
                };
            }
        }

    }



    //***********************************************************************
    //  TESTS
    //***********************************************************************
    public void Test1()
    {
        Debug.Log("test elements ok");
    }
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



        void test_save_json_skill_tre()
        {
            JsonSkillsTreeNode[] skills = new JsonSkillsTreeNode[3];
            skills[0] = SkillsTreeFile.create_one_node(1, 0, 1, "Категори 1", "кор. описание к1", "полное описание к1");
            skills[1] = SkillsTreeFile.create_one_node(2, 0, 1, "Категори 2", "кор. описание к2", "полное описание к2");
            skills[2] = SkillsTreeFile.create_one_node(3, 0, 1, "Категори 3", "кор. описание к3", "полное описание к3");

            SkillsTreeFile.Save(skills);

        }
    */
}




