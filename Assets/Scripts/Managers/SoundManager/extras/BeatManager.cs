using UnityEngine;
using System;

public class BeatManager : MonoBehaviour
{
    [SerializeField] private float _bpm = 120f;
    [SerializeField] private AudioSource _audioSource;

    [Header("Subdivisiones")]
    [SerializeField] private int _subdivisions = 1; // 1 = beat normal, 2 = corcheas, 4 = semicorcheas

    public static event Action OnBeat;
    public static event Action OnSubBeat;

    private double _secondsPerBeat;
    private double _nextBeatTime;
    private double _nextSubBeatTime;
    private double _secondsPerSubBeat;

    private void Start()
    {
        _secondsPerBeat = 60.0 / _bpm;
        _secondsPerSubBeat = _secondsPerBeat / _subdivisions;

        if (_audioSource != null)
        {
            _audioSource.Play();
            _nextBeatTime = AudioSettings.dspTime + _secondsPerBeat;
            _nextSubBeatTime = AudioSettings.dspTime + _secondsPerSubBeat;
        }
    }

    private void Update()
    {
        double dspTime = AudioSettings.dspTime;

        // Beat principal
        if (dspTime >= _nextBeatTime)
        {
            OnBeat?.Invoke();
            _nextBeatTime += _secondsPerBeat;
        }

        // Subdivisiones
        if (_subdivisions > 1 && dspTime >= _nextSubBeatTime)
        {
            OnSubBeat?.Invoke();
            _nextSubBeatTime += _secondsPerSubBeat;
        }
    }
}