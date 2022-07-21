using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An abstract class which should be inherited by all buttons which are interactable by the Player in the scene.
/// </summary>
public abstract class SceneButton : MonoBehaviour, IInteractable
{
    public UnityEvent buttonEvent;
    /// <summary>
    /// Abstract implementation of the IInteractable.BeInteracted method.
    /// See <see cref="IInteractable.BeInteracted(GameObject, object)"/>.
    /// </summary>
    /// <param name="callerGO">The caller GameObject of this method.</param>
    /// <param name="args">Any arguments which the caller might want to pass to the callee via a general object reference.</param>
    public abstract void BeInteracted(GameObject callerGO, object args);
}
