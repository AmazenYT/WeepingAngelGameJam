using UnityEngine;
using UnityEngine.UI;
using TMPro; // Add this if using TextMeshPro

public class KeyManager : MonoBehaviour
{
    public static KeyManager instance;
    private int keysCollected = 0;
    private int totalKeys = 3;

    public TextMeshProUGUI keyText; // Use TextMeshProUGUI for TMP
    // public Text keyText; // Uncomment this if using Legacy Text UI

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        UpdateKeyUI();
    }

    public void CollectKey()
    {
        keysCollected++;
        UpdateKeyUI();
        Debug.Log("Keys Collected: " + keysCollected);
    }

    public bool HasAllKeys()
    {
        return keysCollected >= totalKeys;
    }

    private void UpdateKeyUI()
    {
        if (keyText != null)
        {
            keyText.text = keysCollected + "/" + totalKeys + " Keys Collected";
        }
    }
}
