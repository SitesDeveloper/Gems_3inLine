using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillsCategoriesEditor : MonoBehaviour
{
    //граф категоирй = контент от scroll_view
    public GameObject goCategories;
    public Color normalColor;
    public Color selectedColor;

    //фон попапов
    public GameObject pnl_popups_backfon;
    //попап-форма
    public GameObject pnl_form;
    //попап-заголовок
    public TMP_Text popup_caption;
    //попап - панель снизу с кнопками
    public GameObject pnl_popup_btm;
    //переменная для хранения того, что нужно сделать по клику "сохранить" в попапе
    // 1=добавить категорию, 0=изменить текущую
    private int popup_is_new = 0;



    SkillsElementsEditor scElements;

    List<JsonSkillsTreeNode> catList = new List<JsonSkillsTreeNode>();


    //ид текущей категории
    private int curCatID = 0; 


    //***********************************************************************
    // Start is called before the first frame update
    void Start()
    {
        if (scElements == null)
            scElements = GetComponent<SkillsElementsEditor>();
    }

    //***********************************************************************
    // Update is called once per frame
    void Update()
    {
        
    }

    //***********************************************************************
    //установить данные нодов категорий
    public void SetData( List<JsonSkillsTreeNode> nodes)
    {
        if (scElements == null)
            scElements = GetComponent<SkillsElementsEditor>();
        catList.Clear();
        catList.AddRange(nodes);
    }


    //***********************************************************************
    //очистка графа от всех категорий и подкатегорий
    public void ClearGraph()
    {
        //Debug.Log("cat_counts_before_del=" + goCategories.transform.childCount);
        if (goCategories.transform.childCount > 0)
        {
            foreach (Transform ct in goCategories.transform) Destroy(ct.gameObject);
        }
        curCatID = 0;
    }



    //***********************************************************************
    //заполнение списка категорий
    public void FillGrap()
    {
        ClearGraph();
        if (catList.Count>0)
        {
            foreach ( JsonSkillsTreeNode node in catList)
            {
                string prefab_name = GameConstants.PREFAB_EDITOR_CATEGORY;
                string spr_name = "cat_k";
                if (node.parent_id > 0)
                {
                    prefab_name = GameConstants.PREFAB_EDITOR_SUBCATEGORY;
                    spr_name = "cat";
                }

                GameObject obCat = (GameObject)Instantiate(Resources.Load(prefab_name));
                obCat.transform.SetParent(goCategories.transform);
                string name = CreateNameByID(node.id);
                obCat.name = name;
                obCat.transform.Find("tmp_name").GetComponent<TMP_Text>().text = node.name;
                obCat.transform.Find("tmp_id").GetComponent<TMP_Text>().text = node.id.ToString();
                GameObject img = obCat.transform.Find("pnl_img").Find("Image").gameObject;
                    img.GetComponent<Image>().sprite = GameConstants.GetSprite(spr_name);
                Button btn = obCat.GetComponentInChildren<Button>();
                btn.onClick.AddListener(delegate { Set_Category_Active(node.id); });
            }
            Set_Category_Active(catList[0].id);
        } else
        {
            Set_Category_Active(0);
        }


    }

    //***********************************************************************
    public string CreateNameByID( int id )
    {
        return "category_" + id;
    }



    //***********************************************************************
    public void Set_Category_Active(int id)
    {
        curCatID = id;
        string name = CreateNameByID(id);

        if (goCategories.transform.childCount > 0)
        {
            foreach (Transform gCat in goCategories.transform)
            {
                Button btn = gCat.GetComponent<Button>();
                ColorBlock cb = btn.colors;
                if (gCat.name == name)
                {
                    cb.normalColor = selectedColor;
                    btn.colors = cb;
                    btn.tag = "btn_selected";
                }
                else
                {
                    cb.normalColor = normalColor;
                    btn.colors = cb;
                    btn.tag = "btn_not_selected";
                };
            }
        }

        //if (scElements==null) scElements = GetComponent<SkillsElementsEditor>();
        scElements.UpdateListByCatID(curCatID);

        //btn_AddEdit_Category();
    }


    //***********************************************************************
    public JsonSkillsTreeNode get_CatNodeByID(int id)
    {
        return catList.Find(x => x.id == id);
    }


    //***********************************************************************
    //кнопка добавить/редактировать, is_new=1 если добавление, иначе редактирование текущей
    public void btn_AddEdit_Category( int is_new=0 )
    {
        popup_is_new = is_new;
        //открытие попапа
        pnl_popups_backfon.SetActive(true);
        JsonSkillsTreeNode curNode = get_CatNodeByID(curCatID);

        popup_caption.text = "Добавление новой категории";
        if (is_new!=1)
        {
            popup_caption.text = "Редактирование категории [" + curCatID.ToString()+"]";
        }

        //поле: родительская категория  //.Find("Dropdown_parent").gameObject
        //заполнение дроп-списка в попапе, только корневыми категориями
        TMP_Dropdown popup_parent_dropdown = pnl_form.transform.Find("pnl_parent").GetComponentInChildren<TMP_Dropdown>();
        List<string> opts = new List<string>();
        opts.Add("0: корень");
        int opt_indx=0,  //для выбора опции в drop-списке
            n = 1; //для исчисления родителей
        foreach (JsonSkillsTreeNode node in catList)
        {
            if (node.parent_id == 0)
            {
                opts.Add(node.id.ToString() + ": " + node.name);
                if (node.id == curNode.parent_id)
                    opt_indx = n;
                n++;
            }
        }
        popup_parent_dropdown.ClearOptions();
        popup_parent_dropdown.AddOptions(opts);
        //выбор в списке текущей выбранной категории
        popup_parent_dropdown.value = opt_indx;


        //поле: тип ноды (категория / элемент)
        Toggle [] popup_type_toggle = pnl_form.transform.Find("pnl_type").GetComponentsInChildren<Toggle>();
        //if (curNode.is_category == 1) {}
        popup_type_toggle[0].isOn = true;
        popup_type_toggle[0].interactable = false;
        popup_type_toggle[1].isOn = false;
        popup_type_toggle[1].interactable = false;

        //поле: сортировка
        TMP_InputField tmptxt = pnl_form.transform.Find("pnl_sort").GetComponentInChildren<TMP_InputField>();
        tmptxt.text = (is_new != 1) ? curNode.sort.ToString() : "";

        //поле: название
        tmptxt = pnl_form.transform.Find("pnl_name").GetComponentInChildren<TMP_InputField>();
        tmptxt.text = (is_new != 1) ? curNode.name : "";

        //поле: изображение
        tmptxt = pnl_form.transform.Find("pnl_image").GetComponentInChildren<TMP_InputField>();
        tmptxt.text = (is_new != 1) ? curNode.image : "";

        //поле: короткое описание
        tmptxt = pnl_form.transform.Find("pnl_small_desc").GetComponentInChildren<TMP_InputField>();
        tmptxt.text = (is_new != 1) ? curNode.short_desc : "";
        //поле: полное описание
        tmptxt = pnl_form.transform.Find("pnl_full_desc").GetComponentInChildren<TMP_InputField>();
        tmptxt.text = (is_new != 1) ? curNode.full_desc : "";


        //gDp.onValueChanged.AddListener(delegate { ... });

    }





    //***********************************************************************
    public void btn_Delete_Category()
    {

    }

    //***********************************************************************
    public void btn_Move_Category()
    {

    }


    //***********************************************************************
    //кнопка сохранить
    public void btn_Save_FormData()
    {
        // 1 получение данных с формы в node
        JsonSkillsTreeNode formNode = new JsonSkillsTreeNode();
        //поле родитель
        TMP_Dropdown popup_parent_dropdown = pnl_form.transform.Find("pnl_parent").GetComponentInChildren<TMP_Dropdown>();
        string tmp = popup_parent_dropdown.captionText.text;
        formNode.parent_id = myUtils.SplitStringAndGetFromIndx(tmp, new char[] { ':' },0);

        //поле тип
        Toggle[] togls = pnl_form.transform.Find("pnl_type").GetComponentsInChildren<Toggle>();
        formNode.is_category = togls[0].isOn ? 1 : 0;
        //поле: сортировка
        TMP_InputField tmptxt = pnl_form.transform.Find("pnl_sort").GetComponentInChildren<TMP_InputField>();
        Int32.TryParse(tmptxt.text, out formNode.sort);

        //поле: название
        tmptxt = pnl_form.transform.Find("pnl_name").GetComponentInChildren<TMP_InputField>();
        formNode.name = tmptxt.text;

        //поле: изображение
        tmptxt = pnl_form.transform.Find("pnl_image").GetComponentInChildren<TMP_InputField>();
        formNode.image = tmptxt.text;

        //поле: короткое описание
        tmptxt = pnl_form.transform.Find("pnl_small_desc").GetComponentInChildren<TMP_InputField>();
        formNode.short_desc = tmptxt.text;

        //поле: полное описание
        tmptxt = pnl_form.transform.Find("pnl_full_desc").GetComponentInChildren<TMP_InputField>();
        formNode.full_desc = tmptxt.text;

        Debug.Log(formNode);

        List<String> Errors = new List<string>();
        //2 проверка не пустые ли данные
        if (formNode.is_category!=1) {
            Errors.Add("Тип ноды должен быть категория.");
        }
        if (formNode.name.Length<5)
        {
            Errors.Add("Название должно быть не меньше 5 символов.");
        }
        if (formNode.short_desc.Length < 10  || formNode.full_desc.Length < 10)
        {
            Errors.Add("Описание (короткое или полное) должно быть не меньше 10 символов.");
        }

        Errors.Add("Тестовая ошибка какая-то еще какой-то текст плюс еще какой-то текст и еще текст.");

        if (Errors.Count > 0)
        {
            //найдены ошибки - показ сообщения об этом
            MessageBox.ShowMessage(
                () => { Debug.Log("message_box closed");}, 
                string.Join("\n", Errors.ToArray() ) 
            );

        }
        else
        {
            //все ок сохранение данных и закрытие формы
            pnl_popups_backfon.SetActive(false);
        }
    }





    //***********************************************************************
    //закрытие попапа
    public void btn_popup_close() {
        pnl_popups_backfon.SetActive(false);
    }



    //***********************************************************************
    //  TESTS
    //***********************************************************************


    public void Test1()
    {
        Debug.Log("test categories ok");
    }

}




