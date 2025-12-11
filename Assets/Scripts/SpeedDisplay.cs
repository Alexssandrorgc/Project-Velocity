using UnityEngine;
using TMPro;

public class SpeedDisplay : MonoBehaviour
{
    public TextMeshProUGUI speedText;
    public RunnerMovement player;

    void Update()
    {
        if (speedText != null && player != null)
        {
            float speed = player.GetCurrentSpeed();
            speedText.text = $"Speed: {speed:F1} m/s";
        }
    }
}