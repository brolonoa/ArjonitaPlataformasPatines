using UnityEngine;

public class BeatSpriteChanger : MonoBehaviour
{
    private bool isActive;

    private void Start()
    {
        isActive = true;
        gameObject.SetActive(true);
    }
    private void OnEnable()
    {
        BeatManager.OnBeat += HandleBeat;
    }

    private void OnDisable()
    {
        BeatManager.OnBeat -= HandleBeat;
    }

    private void HandleBeat()
    {
        if (!isActive)
        {
            isActive = true;
            gameObject.SetActive(true);
        }
        else
        {
            isActive = false;
            gameObject.SetActive(false);
        }
        Debug.Log("bich");
    }
        //[Header("Sprites")]
        //[SerializeField] private Sprite[] _sprites; 

        //[Header("References")]
        //[SerializeField] private SpriteRenderer _spriteRenderer;

        //private int _currentSpriteIndex = 0;

        //private void Awake()
        //{
        //    if (_spriteRenderer == null)
        //        _spriteRenderer = GetComponent<SpriteRenderer>();

        //    if (_sprites.Length > 0 && _spriteRenderer != null)
        //    {
        //        _spriteRenderer.sprite = _sprites[0];
        //    }
        //}

        //private void OnEnable()
        //{
        //    if (BeatEventSystem.Instance != null)
        //    {
        //        BeatEventSystem.Instance.OnBeat += OnBeatReceived;
        //    }
        //}

        //private void OnDisable()
        //{
        //    if (BeatEventSystem.Instance != null)
        //    {
        //        BeatEventSystem.Instance.OnBeat -= OnBeatReceived;
        //    }
        //}

        //private void OnBeatReceived()
        //{
        //    if (_sprites.Length == 0 || _spriteRenderer == null) return;

        //    _currentSpriteIndex = (_currentSpriteIndex + 1) % _sprites.Length;
        //    _spriteRenderer.sprite = _sprites[_currentSpriteIndex];
        //}
    }
