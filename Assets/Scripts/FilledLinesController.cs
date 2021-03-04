using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FilledLinesController : MonoBehaviour
{
    public List<OneFilledLine> filledLines = new List<OneFilledLine>();
    

    // Update is called once per frame
    void Update()
    {
        for (int i = filledLines.Count-1; i>=0 ; i--)
        {
            if ( filledLines[i].Update() )
            {
                filledLines[i].Clear();
                filledLines.RemoveAt(i);
            }
        }
    }

    //добавление еще одной линии в контроллер анимации линий
    public void AddLine(int y, List<int> lineSquares, Transform parent)
    {
        OneFilledLine line = new OneFilledLine(y, lineSquares, parent);
        filledLines.Add(line);
        SoundManager.PlaySound("line_filled");
    }

}
