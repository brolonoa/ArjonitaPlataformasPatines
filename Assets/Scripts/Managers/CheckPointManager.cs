using UnityEngine;

public class CheckPointManager : MonoBehaviour, IEInteractable
{
    [SerializeField] BoxCollider2D checkPointTrigger;
    [SerializeField] Transform checkPointTransform;

    private void Start()
    {
        checkPointTrigger.enabled = true;
    }
    public void OnInteract()
    {
        LevelManager.Instance.ChangeCheckPointPos(checkPointTransform);
        checkPointTrigger.enabled = false;
    }
    
}
