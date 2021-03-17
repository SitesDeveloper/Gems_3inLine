using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SkillsTreeEditorController : MonoBehaviour
{
    //public GameObject pnlSkillsTree;


    public GameObject pnlTest;
    private int n = 0;
    // Start is called before the first frame update
    void Start()
    {
        TestAddElement();
        TestAddElement();
        TestAddElement();
        TestAddElement();

    }

    // Update is called once per frame
    void Update()
    {
        
    }

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



}




