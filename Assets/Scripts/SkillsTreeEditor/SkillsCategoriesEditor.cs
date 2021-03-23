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

    SkillsElementsEditor scElements;

    List<JsonSkillsTreeNode> catList = new List<JsonSkillsTreeNode>();


    //ид текущей категории
    private int curCatID = 0; 


    //***********************************************************************
    // Start is called before the first frame update
    void Start()
    {
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

        if (scElements==null)
            scElements = GetComponent<SkillsElementsEditor>();
        scElements.UpdateListByCatID(curCatID);
    }




    //***********************************************************************
    //  TESTS
    //***********************************************************************


    public void Test1()
    {
        Debug.Log("test categories ok");
    }

}




