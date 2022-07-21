using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// A class which designates the attached object as a SceneTrigger.
/// SceneTriggers are invisible colliders in the scene which can be used to trigger certain events via <see cref="triggerEvent"/>.
/// </summary>
public class SceneTrigger : MonoBehaviour
{
    public UnityEvent<UnityEngine.Object> triggerEvent;
    Collider myCol;
    /// <summary>
    /// The other collider which entered the confines of this SceneTrigger instance.
    /// </summary>
    public Collider otherCol; 

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
