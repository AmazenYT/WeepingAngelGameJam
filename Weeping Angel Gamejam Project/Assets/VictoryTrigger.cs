using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene management

public class VictoryTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger is the player
        if (other.CompareTag("Player"))
        {
            // Load the victory scene (ensure it's added in build settings)
            SceneManager.LoadScene("VictoryScene");
        }
    }
}