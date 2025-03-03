using UnityEngine;
using UnityEngine.AI;

public class WeepingAngel : MonoBehaviour
{
    public NavMeshAgent ai;
    public Transform player;
    Vector3 dest;
    public Camera playerCam;
    public float aiSpeed;

    private void Update()
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(playerCam);

        if(GeometryUtility.TestPlanesAABB(planes, this.gameObject.GetComponent<Renderer>().bounds))
        {
            ai.speed = aiSpeed;
            dest = player.position;
            ai.destination = dest;
        }
    }
}
