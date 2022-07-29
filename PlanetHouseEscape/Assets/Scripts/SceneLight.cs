using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// A class which designates the attached object as a SceneLight.
/// A SceneLight is used in the house to light a particular area.
/// - How to use this script:?
/// - Initially, all the lights in the house are turned on. This is done automatically in the <see cref="Awake"/> method.
/// - The light can be toggled on and off via its public method <see cref="SwitchLight"/>.
/// - Finally, when the light is on, <see cref="lightVisualOn"/> is shown; and when the light is off, <see cref="lightVisualOff"/> is used.
/// And by "used", I mean "it will be made visible in the scene".
/// </summary>
public class SceneLight : MonoBehaviour
{
    public Light lightComponent;
    public GameObject lightVisualOn;
    public GameObject lightVisualOff;

    /// <summary>
    /// Switches the state of the light (on/off).
    /// </summary>
    public void SwitchLight()
    {
        lightComponent.enabled = !lightComponent.enabled;
        lightVisualOn.SetActive(lightComponent.enabled);
        lightVisualOff.SetActive(!lightComponent.enabled);
    }

    /// <summary>
    /// Unity's Awake method. Awake is called when the script instance is being loaded.
    /// In this case, it is used to initialize the light.
    /// </summary>
    void Awake()
    {
        lightComponent.enabled = true;
        lightVisualOn.SetActive(true);
        lightVisualOff.SetActive(false);
    }
}
