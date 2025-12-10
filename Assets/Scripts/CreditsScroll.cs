using UnityEngine;

public class CreditsScroll : MonoBehaviour
{
    public RectTransform content; // El Content del ScrollView
    public float speed = 40f;      // Velocidad a la que suben los créditos

    void Update()
    {
        content.anchoredPosition += Vector2.up * speed * Time.deltaTime;
    }
}
