using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutoCanvas : MonoBehaviour
{
    // Pour le UI du joueur rose
    public RectTransform UiPink;
    public Timer timerPink;

    // Pour le UI du joueur rouge
    public RectTransform UiRed;
    public Timer timerRed;

    // positions dans le ui
    public Vector2 firstPosition = new Vector2(-300, 300);
    public Vector2 secondPosition = new Vector2(300, 300);


    void Update()
    {
        int placementPink = timerPink.position;
        int placementRed = timerRed.position;
        placeUiIcons(placementPink, UiPink);
        placeUiIcons(placementRed, UiRed);
    }

    void placeUiIcons(int placement, RectTransform ui)
    {
        if (placement == 1)
        {
            ui.anchoredPosition = firstPosition;
        }
        else if (placement == 2)
        {
            ui.anchoredPosition = secondPosition;
        }
    }
}
