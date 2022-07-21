using UnityEngine;

/// <summary>
/// A class which designates the attached object as a FlipFlopButton.
/// </summary>
public class FlipFlopButton : SceneButton
{
    public Transform btnSwitchVisual1;
    public Transform btnSwitchVisual2;
    bool flipState;

    /// <summary>
    /// Implementation of the <see cref="SceneButton.BeInteracted(GameObject, object)"/> method.
    /// It the flip flop animation will also play out here.
    /// </summary>
    /// <param name="callerGO">The caller GameObject of this method.</param>
    /// <param name="args">Any arguments which the caller might want to pass to the callee via a general object reference.</param>
    public override void BeInteracted(GameObject callerGO, object args)
    {
        buttonEvent?.Invoke();

        flipState = !flipState;
        UpdateVisuals();
    }

    /// <summary>
    /// Sets the visibility of each visual based on the flipState.
    /// </summary>
    void UpdateVisuals()
    {
        btnSwitchVisual1.gameObject.SetActive(flipState);
        btnSwitchVisual2.gameObject.SetActive(!flipState);
    }

    /// <summary>
    /// Unity's Awake method. Awake is called when the script instance is being loaded.
    /// In this case, it is used to initialize the components of the FlipFlopButton.
    /// </summary>
    void Start()
    {
        UpdateVisuals();
    }
}

