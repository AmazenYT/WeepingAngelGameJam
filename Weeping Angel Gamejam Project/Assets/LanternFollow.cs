using UnityEngine;

public class LanternFollow : MonoBehaviour
{
    public Transform cameraTransform; 
    public GameObject lanternModel; 
    public float followSpeed = 10f; 
    public Vector3 modelOffset = new Vector3(0.4f, -0.2f, 0.5f); 
    public Vector3 modelRotationOffset = new Vector3(0, 180f, 0); 

    public Light lanternLight; 
    public float maxLightIntensity = 5f; 
    public float lightDecayRate = 0.5f; 
    private bool isDepleting = true; 
    private GameObject nearbyCandle = null; 

    void Update()
    {
        if (cameraTransform == null)
        {
            Debug.LogWarning("Camera Transform is not assigned!");
            return;
        }

       
        transform.position = Vector3.Lerp(transform.position, cameraTransform.position, followSpeed * Time.deltaTime);
        transform.rotation = cameraTransform.rotation;

        
        if (lanternModel != null)
        {
            lanternModel.transform.position = cameraTransform.position + cameraTransform.TransformDirection(modelOffset);
            lanternModel.transform.rotation = cameraTransform.rotation * Quaternion.Euler(modelRotationOffset);
        }

        
        if (isDepleting && lanternLight != null)
        {
            lanternLight.intensity -= lightDecayRate * Time.deltaTime;
            if (lanternLight.intensity <= 0)
            {
                lanternLight.intensity = 0; 
            }
        }

        
        if (nearbyCandle != null && Input.GetKeyDown(KeyCode.E))
        {
            RechargeLantern();
            Destroy(nearbyCandle);
            nearbyCandle = null;
        }
    }

    public void RechargeLantern()
    {
        if (lanternLight != null)
        {
            lanternLight.intensity = maxLightIntensity; 
            isDepleting = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Candle")) 
        {
            nearbyCandle = other.gameObject; 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Candle"))
        {
            nearbyCandle = null; 
        }
    }
}
