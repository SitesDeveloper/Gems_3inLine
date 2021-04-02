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
    //превью текста элемента
    public TMP_Text tmptxt_elm_preview;

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



    SkillsCategoriesEditor skillsCategories;


    //общий список элементов со всех категорий
    List<JsonSkillsTreeNode> elmsList = new List<JsonSkillsTreeNode>();
    //элементы текущей категории из общего списка элементов 
    List<JsonSkillsTreeNode> elmsCurCat = new List<JsonSkillsTreeNode>();


    //категория элементы которой юзаются
    private int curCatID = 0;

    //текущий выбранный элемент
    private int curElmID = 0;


    // Start is called before the first frame update
    void Start()
    {
        if (skillsCategories == null)
            skillsCategories = GetComponent<SkillsCategoriesEditor>();
        //ClearGraph();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //***********************************************************************
    //обновление списка и графа
    public void refresh_all()
    {
        reloadFromFile();
        setCategoryAndRefreshGraph(curCatID);
    }

    //***********************************************************************
    //установить данные нодов элементов
    public void reloadFromFile( )
    {
        if (skillsCategories == null)
            skillsCategories = GetComponent<SkillsCategoriesEditor>();

        elmsList.Clear();
        elmsList.AddRange( SkillsTreeFile.elmsList );
    }

    //***********************************************************************
    //заполнить список элементов категории - данными выбранной категории и обновить граф
    public void setCategoryAndRefreshGraph(int cat_id)
    {
        curCatID = cat_id;
        if (elmsList.Count > 0)
        {

            elmsCurCat.Clear();
            foreach (JsonSkillsTreeNode node in elmsList)
            {
                if (node.parent_id == curCatID)
                {
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
            }
        }
        RefreshGrap();
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
    //обновление графа и установка текущим самого первого элемента
    public void RefreshGrap()
    {
        ClearGraph();
        if (elmsCurCat.Count > 0)
        {

            //Debug.Log("elms_curcat_count=" + elmsCurCat.Count);
            foreach (JsonSkillsTreeNode node in elmsCurCat)
            {
                string prefab_name = GameConstants.PREFAB_EDITOR_ELEMENT;
                string spr_name = "item";

                GameObject obElm = (GameObject)Instantiate(Resources.Load(prefab_name));
                obElm.transform.SetParent(goElements.transform);
                string name = CreateNameByID(node.id);
                obElm.name = name;
                obElm.transform.Find("tmp_name").GetComponent<TMP_Text>().text = "["+node.id+"] " +  node.name;
                obElm.transform.Find("tmp_id").GetComponent<TMP_Text>().text = node.sort.ToString();
                GameObject img = obElm.transform.Find("pnl_img").Find("Image").gameObject;
                img.GetComponent<Image>().sprite = GameConstants.GetSprite(spr_name);
                Button btn = obElm.GetComponentInChildren<Button>();
                btn.onClick.AddListener(delegate { Set_Element_Active(node.id); });
            }
            curElmID = elmsCurCat[0].id;
        }
        Set_Element_Active(curElmID);
    }

    //***********************************************************************
    //получить ID из строки имени вида element_ID
    public int ExtractIDfromName(string name)
    {
        return myUtils.SplitStringAndGetFromIndx(name, new char[] { '_' }, 1);
    }


    //***********************************************************************
    //возвращает ноду-элемента из текущей категории по id-элемента или null
    public JsonSkillsTreeNode getElmNodeByID(int id)
    {
        return elmsCurCat.Find(x => x.id == id);
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
    //кнопка добавить/редактировать, is_new=1 если добавление, иначе редактирование текущей
    public void btn_AddEdit_Element(int is_new = 0)
    {
        //если попытка редактировать, а элемент не выбран
        if (is_new != 1 && curElmID <= 0)
            //то добавление нового
            is_new = 1; 
        popup_is_new = is_new;

        //открытие попапа
        pnl_popups_backfon.SetActive(true);
        JsonSkillsTreeNode curNode = elmsCurCat.Find(x => x.id==curElmID);
        popup_caption.text = "Добавление нового элемента";
        if (is_new != 1)
        {
            popup_caption.text = "Редактирование элемента [" + curElmID + "]";
        }

        //поле: категория  элемента 
        //заполнение дроп-списка в попапе всеми категориями 
        List<JsonSkillsTreeNode> catList = skillsCategories.getCatList();
        TMP_Dropdown popup_parent_dropdown = pnl_form.transform.Find("pnl_parent").GetComponentInChildren<TMP_Dropdown>();
        List<string> opts = new List<string>();
        //opts.Add("0: корень"); нельзя прописать элемент к корню
        int opt_indx = 0,  //для выбора опции в drop-списке
            n = 0; //для исчисления родителей
        foreach (JsonSkillsTreeNode node in catList)
        {
            if (node.parent_id == 0)
                //корневая категория
                opts.Add(node.id.ToString() + ": " + node.name);
            else
                //подкатегория
                opts.Add("    " + node.id.ToString() + ": " + node.name);
            if (node.id == curCatID)
                opt_indx = n;
            n++;
        }
        popup_parent_dropdown.ClearOptions();
        popup_parent_dropdown.AddOptions(opts);
        //выбор в списке текущей выбранной категории
        popup_parent_dropdown.value = opt_indx;


        //поле: тип ноды (категория / элемент)
        Toggle[] popup_type_toggle = pnl_form.transform.Find("pnl_type").GetComponentsInChildren<Toggle>();
        popup_type_toggle[0].isOn = false;
        popup_type_toggle[0].interactable = false;
        popup_type_toggle[1].isOn = true;
        popup_type_toggle[1].interactable = false;

        //поле: сортировка
        TMP_InputField tmptxt = pnl_form.transform.Find("pnl_sort").GetComponentInChildren<TMP_InputField>();
        tmptxt.text = (is_new != 1) ? curNode.sort.ToString() : "10";

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


        //save
        var btn = pnl_popup_btm.transform.Find("btn_save").GetComponent<Button>();
        btn.onClick.RemoveAllListeners();
        btn.onClick.AddListener(delegate { btn_Save_FormElement(); });
    }



    //***********************************************************************
    //кнопка сохранить
    public void btn_Save_FormElement()
    {
        // 1 получение данных с формы в node
        JsonSkillsTreeNode formNode = new JsonSkillsTreeNode();
        //поле родитель
        TMP_Dropdown popup_parent_dropdown = pnl_form.transform.Find("pnl_parent").GetComponentInChildren<TMP_Dropdown>();
        string tmp = popup_parent_dropdown.captionText.text.Trim();
        formNode.parent_id = myUtils.SplitStringAndGetFromIndx(tmp, new char[] { ':' }, 0);

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
        if (formNode.is_category != 0)
        {
            Errors.Add("Тип ноды должен быть элемент.");
        }
        if (formNode.name.Length < 5)
        {
            Errors.Add("Название должно быть не меньше 5 символов.");
        }
        if (formNode.short_desc.Length < 10 || formNode.full_desc.Length < 10)
        {
            Errors.Add("Описание (короткое или полное) должно быть не меньше 10 символов.");
        }

        //Errors.Add("Тестовая ошибка какая-то еще какой-то текст плюс еще какой-то текст и еще текст.");

        if (Errors.Count > 0)
        {
            //найдены ошибки - показ сообщения об этом
            MessageBox.ShowMessage(
                () => { Debug.Log("message_box closed"); },
                string.Join("\n", Errors.ToArray())
            );

        }
        else
        {
            //все ок сохранение данных и закрытие формы
            pnl_popups_backfon.SetActive(false);


            if (popup_is_new == 1)
                formNode.id = 0;
            else
                formNode.id = curElmID;
            int new_id = curElmID = SkillsTreeFile.addEditElement(formNode);
            //обновление списка и графа
            refresh_all();
            //активация только созданного/измененного элемента
            Set_Element_Active(new_id);
        }
    }





    //***********************************************************************
    public void btn_Delete_Element()
    {
        if (elmsCurCat.Count > 0)
        {
            MessageBox.ShowMessage(
                () =>
                {
                    int elm_id = curElmID;
                    SkillsTreeFile.deleteElement(curElmID);
                //обновление списка и графа
                refresh_all();
                    MessageBox.ShowMessage(
                        () => { },
                        "Удален элемент #" + elm_id
                    );
                },
                "Действительно хотите удалить выбранный элемент?",
                true
            );
        }
    }



    //***********************************************************************
    public void btn_RenumAllElements()
    {
        SkillsTreeFile.renumAllElements( curCatID );
        int elm_id = curElmID;
        refresh_all();
        Set_Element_Active(elm_id);
    }


  
}




