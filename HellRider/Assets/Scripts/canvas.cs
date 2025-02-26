using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class canvas : MonoBehaviour
{
    // Pour le UI du joueur rose
    public RectTransform UiPink;
    public Timer timerPink;

    // Pour le UI du joueur rouge
    public RectTransform UiRed;
    public Timer timerRed;

    // Pour le UI du joueur bleu
    public RectTransform UiBlue;
    public Timer timerBlue;

    // Pour le UI du joueur jaune
    public RectTransform UiYellow;
    public Timer timerYellow;

    // Pour le UI du joueur vert
    public RectTransform UiGreen;
    public Timer timerGreen;

    // positions dans le ui
    public Vector2 firstPosition = new Vector2(-715, 325);
    public Vector2 secondPosition = new Vector2(-715, 137);
    public Vector2 thirdPosition = new Vector2(-715, 0);
    public Vector2 fourthPosition = new Vector2(-715, -137);
    public Vector2 fivePosition = new Vector2(-715, -325);


    void Update()
    {
        int placementPink = timerPink.position;
        int placementRed = timerRed.position;
        int placementBlue = timerBlue.position;
        int placementYellow = timerYellow.position;
        int placementGreen = timerGreen.position;
        placeUiIcons(placementPink, UiPink);
        placeUiIcons(placementRed, UiRed);
        placeUiIcons(placementBlue, UiBlue);
        placeUiIcons(placementYellow, UiYellow);
        placeUiIcons(placementGreen, UiGreen);
    }

    void placeUiIcons(int placement, RectTransform ui)
    {
        if (placement == 1)
        {
            ui.anchoredPosition = firstPosition;
        } else if (placement == 2)
        {
            ui.anchoredPosition = secondPosition;
        }
        else if (placement == 3)
        {
            ui.anchoredPosition = thirdPosition;
        }
        else if (placement == 4)
        {
            ui.anchoredPosition = fourthPosition;
        }
        else if (placement == 5)
        {
            ui.anchoredPosition = fivePosition;
        }
    }
}
