using UnityEngine;

/// <summary>
/// Interface of the game objects which are interactable by the player.
/// - How to use this script?:
/// Since this is an interface, it cannot be added to any game object as a component.
/// However, you can implement this interface on classes to which you deem interactable.
/// Just make sure that the game object contains a collider as well as your class, otherwise the player won't be able to interact with it.
/// After you implement the interface, your class will be considered an IInteractable, and the player will automatically detect and interact with it
/// when the player presses the interact button.
/// </summary>
public interface IInteractable
{
    /// <summary>
    /// Call to interact with an IInteractable game object.
    /// The interactable game object will play out how the interaction should take place.
    /// </summary>
    /// <param name="callerGO">The caller GameObject of this method.</param>
    /// <param name="args">Any arguments which the caller might want to pass to the callee via a general object reference.</param>
    void BeInteracted(GameObject callerGO, object args);
}
