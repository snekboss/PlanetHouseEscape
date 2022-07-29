using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A class which designates the attached object as a SceneTrigger.
/// SceneTriggers are invisible colliders in the scene which can be used to trigger certain events via <see cref="triggerEvent"/>.
/// - How to use it?:
/// Create a game object in the scene, and make sure it has a Collider on it.
/// Then, attach this script to that game object.
/// Make sure that the script and the Collider components are attached to the same game object.
/// You will now be able to add methods to invoke when something enters the trigger area of this collider that you created.
/// When you want to add a method to invoke in a certain script, start by adding a reference of the game object which owns that script.
/// Then, select the method you wish to invoke.
/// Now, since this is a parameterized UnityEvent, you need to choose a parameter in Unity's Inspector menu.
/// - WARNING 1: Due to the unnecessary complexity of parameterized UnityEvent objects, the script itself must be chosen as the argument.
/// - WARNING 2: If you decide to pass a different argument via C# code, Unity will completely ignore that, and default back to whatever you chose
/// in the Inspector menu instead. This is how Unity works, so deal with it, don't shoot at the messenger.
/// </summary>
public class SceneTrigger : MonoBehaviour
{
    public UnityEvent<UnityEngine.Object> triggerEvent;
    Collider myCol;
    /// <summary>
    /// The other collider which entered the confines of this SceneTrigger instance.
    /// </summary>
    [System.NonSerialized] public Collider otherCol; 

    /// <summary>
    ///  Unity's OnTriggerEnter method. In this case, it is used to invoke the triggerEvent associated with this trigger collider.
    /// </summary>
    /// <param name="other">The collider which entered the confines of this trigger collider.</param>
    void OnTriggerEnter(Collider other)
    {
        otherCol = other; // Store the other collider's information. See the reason below.


        /* IMPORTANT: Unity ignores the argument passed to Invoke function via code.
         * It prioritizes whatever was selected in the Editor.
         * Therefore, make sure you specify *this* game object as the argument of Invoke in the Editor.
         * Meaning, as the argument of triggerEvent, designate the game object to which this SceneTrigger script is attached.
        */
        triggerEvent.Invoke(this); 
    }

    /// <summary>
    /// Unity's Awake method. Awake is called when the script instance is being loaded.
    /// In this case, it is used to ensure that the collider is a trigger collider.
    /// </summary>
    void Awake()
    {
        myCol = GetComponent<Collider>();
        myCol.isTrigger = true;
    }
}
