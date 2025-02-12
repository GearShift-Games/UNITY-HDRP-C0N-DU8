using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public Transform target; // Le pilote que cet ic�ne repr�sente
    public RectTransform minimapRect; // La zone de la minimap (UI)
    public Transform minimapCam; // La cam�ra qui voit la minimap
    public float mapSize = 100f; // Taille r�elle de la carte en unit�s Unity

    private RectTransform iconTransform;

    void Start()
    {
        iconTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (target == null || minimapCam == null) return;

        // R�cup�rer la position relative du pilote par rapport � la cam�ra minimap
        Vector3 relativePos = minimapCam.InverseTransformPoint(target.position);

        // Normaliser les coordonn�es (de -0.5 � 0.5)
        float normalizedX = relativePos.x / mapSize;
        float normalizedY = relativePos.z / mapSize; // On utilise Z car la piste est en 3D

        // Convertir en position UI (de -minimapRect � +minimapRect)
        float mappedX = normalizedX * (minimapRect.sizeDelta.x * 0.5f);
        float mappedY = normalizedY * (minimapRect.sizeDelta.y * 0.5f);

        // Appliquer la position
        iconTransform.anchoredPosition = new Vector2(mappedX, mappedY);
    }
}
