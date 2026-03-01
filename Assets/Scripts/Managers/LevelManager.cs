using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;
    public Transform currentCheckPointPos;
    [SerializeField] GameObject playerPrefab;
    [SerializeField] Transform levelStartPos;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else Destroy(gameObject);
    }
    public void ChangeCheckPointPos(Transform newCheckPointPos)
    {
        currentCheckPointPos = newCheckPointPos;
    }

    public void OnPlayerDeath()
    {
        playerPrefab.transform.position = currentCheckPointPos.position;
    }
    //public void OnDamageRecibed()
    //{
    //    playerPrefab.transform.position = currentCheckPointPos.position;
    //}

    

}
