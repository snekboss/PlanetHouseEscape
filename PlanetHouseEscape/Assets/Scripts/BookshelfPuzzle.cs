using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A class which designates the attached object as a BookshelfPuzzle.
/// It is the main script that controls the overall state of the puzzle.
/// </summary>
public class BookshelfPuzzle : MonoBehaviour
{
    public Door bookLever;
    public GameObject bookLeverVisualRegular;
    public GameObject bookLeverVisualPhosphorous;

    public Door secretDoor; // will be opened when the puzzle is solved.
    public SceneButton roomLightSwitch;

    bool isPuzzleComplete = false;


    /// <summary>
    /// This method is supposed to be subscribed to a FlipFlopButton's buttonEvent.
    /// The particular FlipFlopButton will act as the light switch of the room in which this puzzle is contained.
    /// The function handles the bookLever visual, which is supposed to be the hint of this puzzle.
    /// </summary>
    public void OnLightSwitchFlip()
    {
        if (isPuzzleComplete)
        {
            return;
        }

        FlipBookLeverVisual();
    }


    /// <summary>
    /// This method is supposed to be subscribed to a SceneTrigger's triggerEvent.
    /// That particular SceneTrigger acts as the trigger of this BookshelfPuzzle's bookLever.
    /// When the bookLever enters the trigger area of that particular SceneTrigger, the puzzle will be solved.
    /// The SceneTrigger object is not known by the BookshelfPuzzle, but the trigger knows which method to invoke, which happens to be this one.
    /// Due to Unity's usual uncooperativeness, the actual SceneTrigger script itself has to be passed in the Editor in order to let things work.
    /// Therefore, the argument has to exactly *that* SceneTrigger instance.
    /// </summary>
    /// <param name="col">Reference of the SceneTrigger instance which was triggered.</param>
    public void OnBookLeverTriggerEnter(SceneTrigger sceneTriggerItself)
    {
        if (isPuzzleComplete)
        {
            return;
        }

        Collider otherCol = sceneTriggerItself.otherCol;
        Door D = otherCol.GetComponent<Door>();
        
        if (D == null)
        {
            return;
        }

        if (D == bookLever)
        {
            isPuzzleComplete = true;
            secretDoor.unlocked = true;
            secretDoor.BeInteracted(this.gameObject, null);

            if (bookLeverVisualPhosphorous.activeSelf)
            {
                // if the lights are off, then turn them on.
                roomLightSwitch.BeInteracted(this.gameObject, null);
            }

            Destroy(sceneTriggerItself.gameObject);

            InitBookLevelVisuals();
        }
    }

    /// <summary>
    /// Sets the initial active states of the visuals of the book lever.
    /// Initially, the room's light will be lit, so the regular version will be active in the scene.
    /// </summary>
    void InitBookLevelVisuals()
    {
        bookLeverVisualRegular.SetActive(true);
        bookLeverVisualPhosphorous.SetActive(false);
    }

    /// <summary>
    /// Flips the active states of the bookLever visuals.
    /// Only one of them can be active in the scene at a time.
    /// </summary>
    void FlipBookLeverVisual()
    {
        bool regularState = bookLeverVisualRegular.activeSelf;
        bool phosphorousState = bookLeverVisualPhosphorous.activeSelf;
        bookLeverVisualRegular.SetActive(!regularState);
        bookLeverVisualPhosphorous.SetActive(!phosphorousState);
    }

    /// <summary>
    /// Unity's Awake Method.
    /// In this  case, it is used to set up the initial active states of the bookLever visuals.
    /// </summary>
    void Awake()
    {
        InitBookLevelVisuals();
    }
}
