using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class WeepingAngel : MonoBehaviour
{
    public NavMeshAgent ai;
    public Transform player;
    Vector3 dest;
    public Camera playerCam, jumpscareCam;
    public float aiSpeed, catchDistance, jumpscareTime;
    public string sceneAfterDeath;

    private void Update()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCam);
        bool isVisible = GeometryUtility.TestPlanesAABB(planes, this.gameObject.GetComponent<Renderer>().bounds);
        float distance = Vector3.Distance(transform.position, player.position);

        if (isVisible)
        {
            ai.speed = 0;
            ai.SetDestination(transform.position); 
        }
        else
        {
            ai.speed = aiSpeed;
            ai.SetDestination(player.position); 
        }

        
        if (!isVisible && distance <= catchDistance)
        {
            player.gameObject.SetActive(false);
            jumpscareCam.gameObject.SetActive(true);
            StartCoroutine(killPlayer());
        }
    }
    IEnumerator killPlayer()
    {
        yield return new WaitForSeconds(jumpscareTime);
        SceneManager.LoadScene(sceneAfterDeath);
    }
}
