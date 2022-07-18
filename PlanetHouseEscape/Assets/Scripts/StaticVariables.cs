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
    public static float mouseSensitivity = 45f;
    public static float time = 0f;
    public static bool loadedForTheFirstTime = true;
}
