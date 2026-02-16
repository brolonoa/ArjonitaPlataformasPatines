using UnityEngine;
using UnityEngine.Events;

public class BeatManager : MonoBehaviour
{
    [SerializeField] private float _bpm = 120;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private Intervals[] _intervals;

    [Header("BPM Helper")]
    [SerializeField] private bool _showBPMHelper = false;
    [SerializeField] private KeyCode _tapKey = KeyCode.Space;

    private float _lastTapTime;
    private float _averageBPM;
    private int _tapCount;

    private void Start()
    {
        Debug.Log($"BPM actual: {_bpm}");
        if (_showBPMHelper)
        {
            Debug.Log($"Presiona {_tapKey} al ritmo de la música para calcular el BPM");
        }
    }

    private void Update()
    {
        // BPM Helper - presiona espacio al ritmo
        if (_showBPMHelper && Input.GetKeyDown(_tapKey))
        {
            float currentTime = Time.time;

            if (_lastTapTime > 0)
            {
                float timeDiff = currentTime - _lastTapTime;
                float instantBPM = 60f / timeDiff;

                _tapCount++;
                _averageBPM = ((_averageBPM * (_tapCount - 1)) + instantBPM) / _tapCount;

                Debug.Log($"Tap {_tapCount} - BPM instantáneo: {instantBPM:F1}, BPM promedio: {_averageBPM:F1}");

                if (_tapCount >= 8)
                {
                    Debug.Log($"===== BPM CALCULADO: {Mathf.Round(_averageBPM)} =====");
                }
            }

            _lastTapTime = currentTime;
        }

        // Resetear si pasa mucho tiempo
        if (_showBPMHelper && Time.time - _lastTapTime > 3f)
        {
            _tapCount = 0;
            _averageBPM = 0;
        }

        // Sistema de beats normal
        if (_audioSource == null || _audioSource.clip == null || _intervals == null)
            return;

        foreach (Intervals interval in _intervals)
        {
            float intervalLength = interval.GetIntervalLength(_bpm);
            float sampledTime = (_audioSource.timeSamples / (_audioSource.clip.frequency * intervalLength));
            interval.CheckForNewInterval(sampledTime);
        }
    }
}

[System.Serializable]
public class Intervals
{
    [SerializeField] private float _steps = 1;
    [SerializeField] private UnityEvent _trigger;
    private int _lastInterval = -1;

    public float GetSteps() => _steps;

    public float GetIntervalLength(float bpm)
    {
        return 60f / (bpm * _steps);
    }

    public void CheckForNewInterval(float interval)
    {
        int currentInterval = Mathf.FloorToInt(interval);

        if (currentInterval != _lastInterval)
        {
            _lastInterval = currentInterval;
            _trigger.Invoke();
        }
    }
}