using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventTriggerTest : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("q"))
        {
            EventManager.TriggerEvent("quit");
        }

        if (Input.GetKeyDown("n"))
        {
            EventManager.TriggerEvent("n_new_level");
        }


        if (Input.GetKeyDown("f"))
        {
            EventManager.TriggerEvent("f_figura_new");
        }


    }
}
