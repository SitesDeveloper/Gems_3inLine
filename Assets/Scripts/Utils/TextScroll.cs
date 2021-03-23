using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextScroll : MonoBehaviour
{
    [SerializeField] RectTransform txtRT;
    [SerializeField] RectTransform contentRT;

    void Update()
    {
        var size = contentRT.sizeDelta;
        size.y = txtRT.sizeDelta.y;
        contentRT.sizeDelta = size;

        
    }
}
