using UnityEngine;
using TMPro;

public class MessageManager : MonoBehaviour
{
    public static MessageManager Instance;

    public GameObject messagePanel;
    public TextMeshProUGUI messageText;
    private float displayDuration = 0.5f;

    // Se asegura que sea solo una instancia (Singleton) para ser llamada globalmente 
    void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    // Muestra el mensaje 
    public void ShowMessage(string text)
    {
        messageText.text = text;
        messagePanel.SetActive(true);
        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), displayDuration);
    }

    public void ShowMessage(string text, int durationMultiplier)
    {
        messageText.text = text;
        messagePanel.SetActive(true);
        CancelInvoke(nameof(HideMessage));
        Invoke(nameof(HideMessage), displayDuration*durationMultiplier);
    }

    // Ocultar el panel de mensaje 
    void HideMessage()
    {
        messagePanel.SetActive(false);
    }
}
