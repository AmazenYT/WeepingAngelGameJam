using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class WeepingAngel : MonoBehaviour
{
    public NavMeshAgent ai;
    public Transform player;
    //animation stuff
    public Animator aiAnim;

    public Camera playerCam, jumpscareCam;
    public float aiSpeed, catchDistance, jumpscareTime;
    public string sceneAfterDeath;

    public float detectionRadius = 15f;
    public float escapeDistance = 25f;
    public float roamRadius = 10f;
    public float roamWaitTime = 3f;

    private bool isChasing = false;
    private bool isRoaming = true;
    private Vector3 lastRoamDestination;

    private NavMeshPath path; // Used for path validation

    //For Audio//
    public AudioSource MetalArmor;
    public AudioSource HuntMusic;
    public AudioSource BackGroundTrack;
    public AudioSource JumapScare;

    private void Start()
    {
        ai.speed = aiSpeed / 2;
        ai.avoidancePriority = 20; // Higher priority for avoiding obstacles
        ai.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance; // Avoid walls properly
        path = new NavMeshPath(); // Initialize path validation
        StartCoroutine(Roam());
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCam);
        bool isVisible = GeometryUtility.TestPlanesAABB(planes, this.gameObject.GetComponent<Renderer>().bounds);

        if (distance <= detectionRadius)
        {
            isChasing = true;
            isRoaming = false;
            StopCoroutine(Roam());
            MetalArmor.enabled = true;
            HuntMusic.enabled = true;
            BackGroundTrack.enabled = false;
        }
        else if (distance >= escapeDistance && !isRoaming)
        {
            isChasing = false;
            isRoaming = true;
            StartCoroutine(Roam());
            MetalArmor.enabled = false;
            HuntMusic.enabled = false;
            BackGroundTrack.enabled = true;
        }

        if (isChasing)
        {
            ai.speed = aiSpeed;

            if (isVisible)
            {
                aiAnim.speed = 0;
                ai.SetDestination(transform.position);
                MetalArmor.enabled = false;
                HuntMusic.enabled = false;
                BackGroundTrack.enabled = true;
            }
            else
            {
                if (NavMesh.CalculatePath(transform.position, player.position, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    ai.SetDestination(player.position); // Move only if path is valid
                    aiAnim.speed = 1;
                }
                else
                {
                    ai.SetDestination(transform.position); // Stay still if no valid path
                }
            }

            if (!isVisible && distance <= catchDistance)
            {
                player.gameObject.SetActive(false);
                jumpscareCam.gameObject.SetActive(true);
                StartCoroutine(KillPlayer());
            }
        }
    }

    IEnumerator Roam()
    {
        while (isRoaming)
        {
            Vector3 newRoamDestination = GetRandomNavMeshPoint(transform.position, roamRadius);

            if (newRoamDestination != lastRoamDestination)
            {
                ai.SetDestination(newRoamDestination);
                lastRoamDestination = newRoamDestination;
            }

            yield return new WaitForSeconds(roamWaitTime);
        }
    }

    Vector3 GetRandomNavMeshPoint(Vector3 origin, float range)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 randomDirection = Random.insideUnitSphere * range;
            randomDirection += origin;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomDirection, out hit, range, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }
        return transform.position;
    }

    IEnumerator KillPlayer()
    {
        JumapScare.enabled = true;
        yield return new WaitForSeconds(jumpscareTime);
        SceneManager.LoadScene(sceneAfterDeath);
    }
}
