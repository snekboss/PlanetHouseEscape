using UnityEngine;

/// <summary>
/// Interface of the game objects which are interactable by the player.
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
