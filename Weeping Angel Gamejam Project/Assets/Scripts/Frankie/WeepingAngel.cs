using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class WeepingAngel : MonoBehaviour
{
    public NavMeshAgent ai;
    public Transform player;
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

    private NavMeshPath path;

    public AudioSource MetalArmor;
    public AudioSource HuntMusic;
    public AudioSource BackGroundTrack;
    public AudioSource JumapScare;

    private void Start()
    {
        ai.speed = aiSpeed / 2;
        ai.avoidancePriority = 20;
        ai.obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
        path = new NavMeshPath();
        Debug.Log("AI Started Roaming");
        StartCoroutine(Roam());
    }

    private void Update()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCam);
        bool isVisible = GeometryUtility.TestPlanesAABB(planes, this.gameObject.GetComponent<Renderer>().bounds);

        Debug.Log($"Distance to player: {distance}");
        Debug.Log($"Is Visible: {isVisible}");

        if (distance <= detectionRadius)
        {
            isChasing = true;
            isRoaming = false;
            StopCoroutine(Roam());
            MetalArmor.enabled = true;
            HuntMusic.enabled = true;
            BackGroundTrack.enabled = false;
            Debug.Log("AI Entered Chase Mode");
        }
        else if (distance >= escapeDistance && !isRoaming)
        {
            isChasing = false;
            isRoaming = true;
            StartCoroutine(Roam());
            MetalArmor.enabled = false;
            HuntMusic.enabled = false;
            BackGroundTrack.enabled = true;
            Debug.Log("AI Returned to Roaming");
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
                Debug.Log("AI Frozen Due to Visibility");
            }
            else
            {
                if (NavMesh.CalculatePath(transform.position, player.position, NavMesh.AllAreas, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    ai.SetDestination(player.position);
                    aiAnim.speed = 1;
                    Debug.Log("AI Chasing Player");
                }
                else
                {
                    ai.SetDestination(transform.position);
                    Debug.Log("No Valid Path to Player");
                }
            }

            if (!isVisible && distance <= catchDistance)
            {
                Debug.Log("AI Caught the Player");
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
            Debug.Log($"New Roam Destination: {newRoamDestination}");

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
                Debug.Log($"Found valid NavMesh point: {hit.position}");
                return hit.position;
            }
        }
        Debug.Log("Failed to find valid NavMesh point");
        return transform.position;
    }

    IEnumerator KillPlayer()
    {
        JumapScare.enabled = true;
        yield return new WaitForSeconds(jumpscareTime);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}
