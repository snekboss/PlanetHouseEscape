using System.Collections;
using UnityEngine;

/// <summary>
/// A class which allows the attached the game object to behave like a door.
/// </summary>
public class Door : MonoBehaviour, IInteractable
{
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
            currentAngle = containerTransform.localEulerAngles.y;

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

            float x = containerTransform.localEulerAngles.x;
            float offsetY = isOpening ? doorOpenAngle : 0;
            float y = Mathf.LerpAngle(currentAngle, defaultAngle + offsetY, openTime);
            float z = containerTransform.localEulerAngles.z;
            containerTransform.localEulerAngles = new Vector3(x, y, z);

            if (openTime >= 1f)
            {
                break;
            }

            yield return null;
        }
        animCoroutineIsRunning = false;
    }

    /// <summary>
    /// Unity's Start method. Start is called before the first frame update
    /// </summary>
    void Start()
    {
        defaultAngle = containerTransform.localEulerAngles.y;

        rbody = gameObject.GetComponent<Rigidbody>();
        if (rbody == null)
        {
            rbody = gameObject.AddComponent<Rigidbody>();
        }
        rbody.useGravity = false;
        rbody.isKinematic = true;
    }
}