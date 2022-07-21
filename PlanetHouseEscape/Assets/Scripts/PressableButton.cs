using System.Collections;
using UnityEngine;

/// <summary>
/// A class which designates the attached object as a PressableButton.
/// </summary>
public class PressableButton : SceneButton
{
    public Transform transBtnPress; // button will end up at this position when it is pressed.
    public GameObject btnVisual;

    [Range(0.0f, 10.0f)]
    public float recoverySpeed; // speed of getting back to the initial position

    bool isAnimPlaying = false;
    Vector3 posBtnVisualInit; // initial position of the visual of the pressable part of the button
    Vector3 dirBtnVisualInitFwd; // initial forward direction of the visual of the pressable part of the button

    /// <summary>
    /// Implementation of the <see cref="SceneButton.BeInteracted(GameObject, object)"/> method.
    /// The visual animation will be invoked by this function via a coroutine.
    /// </summary>
    /// <param name="callerGO">The caller GameObject of this method.</param>
    /// <param name="args">Any arguments which the caller might want to pass to the callee via a general object reference.</param>
    public override void BeInteracted(GameObject callerGO, object args)
    {
        if (isAnimPlaying)
        {
            return;
        }

        buttonEvent?.Invoke();
        StartCoroutine("PressAnimation");
    }

    /// <summary>
    /// A method which is meant to be invoked as a Unity coroutine, called by <see cref="MonoBehaviour.StartCoroutine(string, object)"/>.
    /// Handles the visual animation of a button press.
    /// For smoothness, the animation is played over several frames.
    /// It is strongly recommended to start this coroutine via <see cref="Pressable.BeInteracted"/>.
    /// </summary>
    /// <returns>Returns some kind of yield return IEnumerator magic thing made by Unity.</returns>
    IEnumerator PressAnimation()
    {
        isAnimPlaying = true;

        // Press the button
        btnVisual.transform.position = transBtnPress.position;
        yield return null; // wait one frame

        // Release it over time.
        float recoveryTime = 0;
        while (true)
        {
            recoveryTime += Time.deltaTime * recoverySpeed;
            recoveryTime = Mathf.Clamp01(recoveryTime);

            btnVisual.transform.position = Vector3.Lerp(btnVisual.transform.position, posBtnVisualInit, recoveryTime);

            if (recoveryTime >= 1f)
            {
                break;
            }

            yield return null;
        }
        isAnimPlaying = false;
    }


    /// <summary>
    /// Unity's Awake method. Awake is called when the script instance is being loaded.
    /// In this case, it is used to initialize the components of the PressableButton.
    /// </summary>
    void Awake()
    {
        posBtnVisualInit = btnVisual.transform.position;
        dirBtnVisualInitFwd = btnVisual.transform.forward;
    }
}
