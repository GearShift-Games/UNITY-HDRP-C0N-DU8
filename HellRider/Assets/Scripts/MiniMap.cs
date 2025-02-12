using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public Transform target; // Le pilote que cet icône représente
    public RectTransform minimapRect; // La zone de la minimap (UI)
    public Transform minimapCam; // La caméra qui voit la minimap
    public float mapSize = 100f; // Taille réelle de la carte en unités Unity

    private RectTransform iconTransform;

    void Start()
    {
        iconTransform = GetComponent<RectTransform>();
    }

    void Update()
    {
        if (target == null || minimapCam == null) return;

        // Récupérer la position relative du pilote par rapport à la caméra minimap
        Vector3 relativePos = minimapCam.InverseTransformPoint(target.position);

        // Normaliser les coordonnées (de -0.5 à 0.5)
        float normalizedX = relativePos.x / mapSize;
        float normalizedY = relativePos.z / mapSize; // On utilise Z car la piste est en 3D

        // Convertir en position UI (de -minimapRect à +minimapRect)
        float mappedX = normalizedX * (minimapRect.sizeDelta.x * 0.5f);
        float mappedY = normalizedY * (minimapRect.sizeDelta.y * 0.5f);

        // Appliquer la position
        iconTransform.anchoredPosition = new Vector2(mappedX, mappedY);
    }
}
