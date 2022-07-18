using UnityEngine;

public interface IInteractable
{
    void BeInteracted(GameObject callerGO, object args);
}
