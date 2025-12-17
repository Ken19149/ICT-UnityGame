using UnityEngine;
using TMPro;

public class KillCounterUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI killText;
    [SerializeField] private string format = "Ingredient Collected: {0}/{1}";
    void Start()
    {
        killText = GetComponent<TextMeshProUGUI>();
    }

    private void Update()
    {
        if (GameManager.Instance == null || killText == null) return;

        killText.text = string.Format(
            format,
            GameManager.Instance.CurrentKillCount,
            GameManager.Instance.KillThreshold
        );
    }
}
