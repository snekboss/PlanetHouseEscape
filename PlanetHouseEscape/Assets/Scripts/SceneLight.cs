using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A class which designates the attached object as a SceneLight.
/// A SceneLight is used in the house to light a particular area.
/// It can be toggled on and off via its public method <see cref="SwitchLight"/>.
/// </summary>
public class SceneLight : MonoBehaviour
{
    Light lightComponent;

    /// <summary>
    /// Switches the state of the light (on/off).
    /// </summary>
    public void SwitchLight()
    {
        lightComponent.enabled = !lightComponent.enabled;
    }

    /// <summary>
    /// Unity's Awake method. Awake is called when the script instance is being loaded.
    /// In this case, it is used to get the reference of the light component which is attached to this object.
    /// </summary>
    void Awake()
    {
        lightComponent = GetComponent<Light>();
        lightComponent.enabled = true;
    }
}
