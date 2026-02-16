using UnityEngine;

public class AdvancedBeatDetector : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private int _sampleSize = 1024;

    [Header("Beat Detection")]
    [Range(0.1f, 2f)]
    [SerializeField] private float _beatThreshold = 1.3f; // Multiplicador de energía promedio
    [Range(0.1f, 1f)]
    [SerializeField] private float _minTimeBetweenBeats = 0.2f;
    [Range(1, 100)]
    [SerializeField] private int _historySize = 43; // Ventana de histórico

    [Header("Debug")]
    [SerializeField] private bool _showDebug = false;

    private float[] _samples;
    private float[] _energyHistory;
    private int _historyIndex;
    private float _lastBeatTime;

    private void Start()
    {
        _samples = new float[_sampleSize];
        _energyHistory = new float[_historySize];

        if (_audioSource == null)
            _audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (_audioSource == null || !_audioSource.isPlaying)
            return;

        DetectBeat();
    }

    private void DetectBeat()
    {
        // Obtener datos del espectro
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.BlackmanHarris);

        // Calcular energía instantánea (frecuencias bajas 0-200Hz aprox)
        float instantEnergy = 0;
        for (int i = 0; i < 20; i++) // Primeros 20 bins son frecuencias bajas
        {
            instantEnergy += _samples[i] * _samples[i];
        }

        // Guardar en histórico
        _energyHistory[_historyIndex] = instantEnergy;
        _historyIndex = (_historyIndex + 1) % _historySize;

        // Calcular energía promedio del histórico
        float averageEnergy = 0;
        for (int i = 0; i < _historySize; i++)
        {
            averageEnergy += _energyHistory[i];
        }
        averageEnergy /= _historySize;

        // Detectar beat si energía instantánea supera el threshold
        if (instantEnergy > averageEnergy * _beatThreshold &&
            Time.time - _lastBeatTime > _minTimeBetweenBeats)
        {
            OnBeatDetected(instantEnergy, averageEnergy);
            _lastBeatTime = Time.time;
        }

        if (_showDebug && Time.frameCount % 10 == 0)
        {
            Debug.Log($"Energy: {instantEnergy:F4} | Avg: {averageEnergy:F4} | Ratio: {(instantEnergy / averageEnergy):F2}");
        }
    }

    private void OnBeatDetected(float instantEnergy, float averageEnergy)
    {
        if (_showDebug)
        {
            Debug.Log($"🎵 BEAT! Energy: {instantEnergy:F4} ({(instantEnergy / averageEnergy):F2}x avg)");
        }

        if (BeatEventSystem.Instance != null)
        {
            BeatEventSystem.Instance.TriggerBeat();
        }
    }
}