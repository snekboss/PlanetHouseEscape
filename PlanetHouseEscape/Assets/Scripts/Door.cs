using System.Collections;
using UnityEngine;

/// <summary>
/// A class which allows the attached the game object to behave like a door.
/// </summary>
public class Door : MonoBehaviour, IInteractable
{
    public enum DoorRotationAxis
    {
        X = 0,
        Y,
        Z
    }
    public DoorRotationAxis chosenRotationAxis;
    public Transform containerTransform;

    [Range(-180f, 180f)]
    public float doorOpenAngle;
    [Range(0f, 10f)]
    public float openSpeed;
    public bool unlocked = true;
    Rigidbody rbody;

    float defaultAngle;
    float currentAngle;
    bool isOpening = false;

    bool animCoroutineIsRunning;


    /// <summary>
    /// Call open or close the door.
    /// </summary>
    /// <param name="callerGO">The caller GameObject of this method.</param>
    /// <param name="args">Any arguments which the caller might want to pass to the callee via a general object reference.</param>
    public void BeInteracted(GameObject callerGO, object args)
    {
        if (unlocked == false)
        {
            return;
        }

        if (!animCoroutineIsRunning)
        {
            isOpening = !isOpening;
            SetCurrentAngle();

            StartCoroutine("AnimCoroutine");
        }
    }

    /// <summary>
    /// Plays out the door's open/close animation. Coroutines are able to work over several frames.
    /// </summary>
    /// <returns>Returns some kind of yield return IEnumerator magic thing made by Unity.</returns>
    IEnumerator AnimCoroutine()
    {
        animCoroutineIsRunning = true;

        float openTime = 0;
        while (true)
        {
            openTime += Time.deltaTime * openSpeed;
            openTime = Mathf.Clamp01(openTime);

            containerTransform.localEulerAngles = GetLerpedAngle(openTime);

            if (openTime >= 1f)
            {
                break;
            }

            yield return null;
        }
        animCoroutineIsRunning = false;
    }

    /// <summary>
    /// Returns the lerped local euler angles result based on an openTime percentage.
    /// The vector contains the angles between defaultAngle and doorOpenAngle based on openTime normalized time.
    /// </summary>
    /// <param name="openTime">A float value between 0 and 1 which defines the normalized time of the door's opening animation progress.</param>
    /// <returns>Returns a Vector3 of (x,y,z) local euler angles of the lerped angles.</returns>
    Vector3 GetLerpedAngle(float openTime)
    {
        float x, y, z = 0;
        if (chosenRotationAxis == DoorRotationAxis.X)
        {
            float offsetX = isOpening ? doorOpenAngle : 0;
            x = Mathf.LerpAngle(currentAngle, defaultAngle + offsetX, openTime);
            y = containerTransform.localEulerAngles.y;
            z = containerTransform.localEulerAngles.z;
        }
        else if (chosenRotationAxis == DoorRotationAxis.Y)
        {
            x = containerTransform.localEulerAngles.x;
            float offsetY = isOpening ? doorOpenAngle : 0;
            y = Mathf.LerpAngle(currentAngle, defaultAngle + offsetY, openTime);
            z = containerTransform.localEulerAngles.z;
        }
        else
        {
            x = containerTransform.localEulerAngles.x;
            y = containerTransform.localEulerAngles.y;
            float offsetZ = isOpening ? doorOpenAngle : 0;
            z = Mathf.LerpAngle(currentAngle, defaultAngle + offsetZ, openTime);
        }

        return new Vector3(x, y, z);
    }

    /// <summary>
    /// Sets the default angle of the door based on the chosen DoorRotationAxis.
    /// </summary>
    void SetDefaultAngle()
    {
        if (chosenRotationAxis == DoorRotationAxis.X)
        {
            defaultAngle = containerTransform.localEulerAngles.x;
        }
        else if (chosenRotationAxis == DoorRotationAxis.Y)
        {
            defaultAngle = containerTransform.localEulerAngles.y;
        }
        else
        {
            defaultAngle = containerTransform.localEulerAngles.z;
        }
    }

    /// <summary>
    /// Sets the currentAngle of the door based on the DoorRotationAxis.
    /// The currentAngle is used while animating the door.
    /// </summary>
    void SetCurrentAngle()
    {
        if (chosenRotationAxis == DoorRotationAxis.X)
        {
            currentAngle = containerTransform.localEulerAngles.x;
        }
        else if (chosenRotationAxis == DoorRotationAxis.Y)
        {
            currentAngle = containerTransform.localEulerAngles.y;
        }
        else
        {
            currentAngle = containerTransform.localEulerAngles.z;
        }
    }

    /// <summary>
    /// Unity's Start method. Start is called before the first frame update
    /// </summary>
    void Start()
    {
        SetDefaultAngle();

        rbody = gameObject.GetComponent<Rigidbody>();
        if (rbody == null)
        {
            rbody = gameObject.AddComponent<Rigidbody>();
        }
        rbody.useGravity = false;
        rbody.isKinematic = true;
    }
}