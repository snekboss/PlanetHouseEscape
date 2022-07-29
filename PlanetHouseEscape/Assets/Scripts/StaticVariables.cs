using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// C# doesn't have a global space, so this "hack" (but not really a hack) is used to cover up for it.
/// Any simple data that should remain across different scenes will reside here.
/// It is possible to create a game object and use Unity's <see cref="Object.DontDestroyOnLoad(UnityEngine.Object)"/>,
/// but the data is simple enough to consider the static variable approach instead.
/// The class itself is not static, because MonoBehaviour doesn't allow it.
/// </summary>
public class StaticVariables : MonoBehaviour
{
    /// <summary>
    /// Sensitivity of the player when it uses any kind of rotation.
    /// </summary>
    public static float PlayerRotationSensitivity = 45f;
    /// <summary>
    /// The global constant which specifies a pickup. If you want your object to be picked up, then its tag should be set to this.
    /// </summary>
    public const string TagPickup = "Pickup";
    /// <summary>
    /// The name of the escape key. It is here because Unity adds suffixes like "(Clone)" to the things it instantiates.
    /// So this used to force the name back to "EscapeKey".
    /// </summary>
    public const string EscapeKeyName = "EscapeKey";
}
