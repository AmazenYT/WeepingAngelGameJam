using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class KeyManager : MonoBehaviour
{
    public static KeyManager instance;
    private int keysCollected = 0;
    private int totalKeys = 3;

    public TextMeshProUGUI keyText;

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
            return;
        }
    }

    private void Start()
    {
        AssignKeyText();
        ResetKeys(); // Ensure keys are reset on start
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        AssignKeyText();
        ResetKeys(); // Reset keys when a new scene loads
    }

    private void AssignKeyText()
    {
        if (keyText == null)
        {
            keyText = FindObjectOfType<TextMeshProUGUI>();
        }
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

    public void ResetKeys()
    {
        keysCollected = 0;
        PlayerPrefs.SetInt("KeysCollected", 0);
        PlayerPrefs.Save();
        UpdateKeyUI();
    }

    public void DestroySelf()
    {
        Debug.Log("Destroying KeyManager");
        Destroy(gameObject);
    }
}
