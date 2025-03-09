using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement; // Required for scene switching

public class Door : MonoBehaviour
{
    public bool IsOpen = false;
    [SerializeField] private bool IsRotatingDoor = true;
    [SerializeField] private float Speed = 1f;
    [SerializeField] private float RotationAmount = 90f;
    [SerializeField] private float ForwardDirection = 0;

    private Vector3 StartRotation;
    private Vector3 Forward;
    private Coroutine AnimationCoroutine;

    private void Awake()
    {
        StartRotation = transform.rotation.eulerAngles;
        Forward = transform.right;
    }

    public void Open(Vector3 UserPosition)
    {
        if (KeyManager.instance != null && KeyManager.instance.HasAllKeys())
        {
            if (!IsOpen)
            {
                if (AnimationCoroutine != null)
                {
                    StopCoroutine(AnimationCoroutine);
                }

                if (IsRotatingDoor)
                {
                    float dot = Vector3.Dot(Forward, (UserPosition - transform.position).normalized);
                    AnimationCoroutine = StartCoroutine(DoRotationOpen(dot));
                }
            }
        }
        else
        {
            Debug.Log("You need all keys to open this door!");
        }
    }

    private IEnumerator DoRotationOpen(float ForwardAmount)
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation;

        if (ForwardAmount >= ForwardDirection)
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y - RotationAmount, 0));
        }
        else
        {
            endRotation = Quaternion.Euler(new Vector3(0, StartRotation.y + RotationAmount, 0));
        }

        IsOpen = true;

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * Speed;
        }

        // Load the victory scene after animation completes
        SceneManager.LoadScene("VictoryScene");
    }

    public void Close()
    {
        if (IsOpen)
        {
            if (AnimationCoroutine != null)
            {
                StopCoroutine(AnimationCoroutine);
            }

            if (IsRotatingDoor)
            {
                AnimationCoroutine = StartCoroutine(DoRotationClose());
            }
        }
    }

    private IEnumerator DoRotationClose()
    {
        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = Quaternion.Euler(StartRotation);

        IsOpen = false;

        float time = 0;
        while (time < 1)
        {
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, time);
            yield return null;
            time += Time.deltaTime * Speed;
        }
    }
}
