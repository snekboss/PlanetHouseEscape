using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// The component of the 3D Axes game object which the Player controls.
/// It is used to help indicate which rotation axis is being used to rotate the pickup object.
/// It is supposed to be an alternative to Unity's LineRenderer, because that one has some unnecessary complexity to it.
/// - This class works in the following way:
/// The script is attached to a game object which is supposed to act like a 3D coordinate axes.
/// The player gets a reference to that game object.
/// The player makes the axes game object visible whenever it decides to do so (which happens while rotating a pickup object).
/// </summary>
public class Axes3D : MonoBehaviour
{
    public enum Axis
    {
        X = 0,
        Y,
        Z
    }

    public GameObject xAxisGO;
    Vector3 xAxisInitScale;

    public GameObject yAxisGO;
    Vector3 yAxisInitScale;

    public GameObject zAxisGO;
    Vector3 zAxisInitScale;
    public bool isActive = false;
    float chosenAxisScale = 3.0f;

    /// <summary>
    /// Set whether the X,Y,Z axes should be visible in the game.
    /// </summary>
    /// <param name="isActive">True if you want them to be visible; false otherwise.</param>
    public void SetActiveAxes(bool isActive)
    {
        xAxisGO.SetActive(isActive);
        yAxisGO.SetActive(isActive);
        zAxisGO.SetActive(isActive);
    }
    /// <summary>
    /// The chosen axis will appear thicker in the game, to indicate that it is currently being used to rotate the pickup object.
    /// </summary>
    /// <param name="axis">If X axis is being used, then choose Axis.X. Similarly for the others.</param>
    public void SetChosenAxis(Axis axis)
    {
        if (axis == Axis.X)
        {
            xAxisGO.transform.localScale = xAxisInitScale * chosenAxisScale;
            yAxisGO.transform.localScale = yAxisInitScale;
            zAxisGO.transform.localScale = zAxisInitScale;
        }
        else if (axis == Axis.Y)
        {
            xAxisGO.transform.localScale = xAxisInitScale;
            yAxisGO.transform.localScale = yAxisInitScale * chosenAxisScale;
            zAxisGO.transform.localScale = zAxisInitScale;
        }
        else // Z
        {
            xAxisGO.transform.localScale = xAxisInitScale;
            yAxisGO.transform.localScale = yAxisInitScale;
            zAxisGO.transform.localScale = zAxisInitScale * chosenAxisScale;
        }
    }

    /// <summary>
    /// Unity's Start method. Start is called before the first frame update
    /// </summary>
    void Start()
    {
        xAxisInitScale = xAxisGO.transform.localScale;
        yAxisInitScale = yAxisGO.transform.localScale;
        zAxisInitScale = zAxisGO.transform.localScale;

        SetActiveAxes(false);
    }
}
