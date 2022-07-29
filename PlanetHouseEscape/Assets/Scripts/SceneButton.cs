using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// An abstract class which should be inherited by all buttons which are interactable by the Player in the scene.
/// Since this is an abstract class, you cannot attach it to a game object.
/// However, it contains a field called buttonEvent of type <see cref="UnityEvent"/>
/// which is able to hold the reference to the method which you wish to call.
/// The player is already able to interact with objects in the scene which has a collider
/// AND a component which implements the <see cref="IInteractable"/> interface.
/// Therefore, the <see cref="BeInteracted(UnityEngine.GameObject, object)"/> method will be called automatically by the Player.
/// When that method is called, it is up to the derived classes of <see cref="SceneButton"/> to deal with it.
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
