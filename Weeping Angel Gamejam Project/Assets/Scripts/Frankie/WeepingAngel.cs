using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class WeepingAngel : MonoBehaviour
{
    public NavMeshAgent ai;
    public Transform player;
    public Camera playerCam, jumpscareCam;
    public float aiSpeed = 3.5f, catchDistance = 2f, jumpscareTime = 2f;
    public string sceneAfterDeath;

    private void Update()
    {
        if (ai == null || player == null || playerCam == null || jumpscareCam == null)
        {
            Debug.LogError("Missing references in WeepingAngel script!");
            return;
        }

        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCam);
        float distance = Vector3.Distance(transform.position, player.position);

        // Check if the AI is in the player's view
        if (GeometryUtility.TestPlanesAABB(planes, GetComponent<Renderer>().bounds))
        {
            ai.speed = 0;
            ai.SetDestination(transform.position);  // Stop moving
        }
        else
        {
            ai.speed = aiSpeed;
            ai.SetDestination(player.position);  // Chase the player
        }

        // Catch player if close enough
        if (distance <= catchDistance)
        {
            CatchPlayer();
        }
    }

    private void CatchPlayer()
    {
        player.gameObject.SetActive(false);
        jumpscareCam.gameObject.SetActive(true);
        StartCoroutine(KillPlayer());
    }

    IEnumerator KillPlayer()
    {
        yield return new WaitForSeconds(jumpscareTime);

        if (!string.IsNullOrEmpty(sceneAfterDeath))
        {
            SceneManager.LoadScene(sceneAfterDeath);
        }
        else
        {
            Debug.LogError("Scene name for death is not set in Inspector!");
        }
    }
}
