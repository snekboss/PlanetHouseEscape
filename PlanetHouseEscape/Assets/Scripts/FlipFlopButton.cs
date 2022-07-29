using UnityEngine;

/// <summary>
/// A class which designates the attached object as a FlipFlopButton.
/// It inherits from <see cref="SceneButton"/>. Check out its documentation page for more details as to what it is.
/// - How this class works:
/// Create a game object in the scene which is supposed to be your FlipFlopButton.
/// We're going to assume that this game object has two main parts: Its visuals, and its collider.
/// The player can only interact with the collider part, so make sure that the collider AND this FlipFlopButton script is attached to the same game object.
/// Then, since this is a flip flop button, add references to the necessary fields in the Inspector menu about the two visuals that will be swapped
/// when the button is interacted.
/// The button will keep track of its own flipState as it is interacted via <see cref="BeInteracted(UnityEngine.GameObject, object)"/>.
/// </summary>
public class FlipFlopButton : SceneButton
{
    public Transform btnSwitchVisual1;
    public Transform btnSwitchVisual2;
    bool flipState = true;

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

