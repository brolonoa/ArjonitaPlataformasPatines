using UnityEngine;

public class BeatColorChangerSimple : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] private AudioSource _audioSource;

    [Header("Colors")]
    [SerializeField] private Color _lowEnergyColor = Color.blue;
    [SerializeField] private Color _highEnergyColor = Color.red;
    [SerializeField] private float _maxEnergy = 0.005f;

    [Header("Settings")]
    [SerializeField] private float _smoothSpeed = 10f;

    [Header("References")]
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private float[] _samples = new float[512];

    private void Awake()
    {
        if (_spriteRenderer == null)
            _spriteRenderer = GetComponent<SpriteRenderer>();

        if (_audioSource == null)
            _audioSource = FindObjectOfType<AudioSource>();
    }

    private void Update()
    {
        if (_audioSource == null || !_audioSource.isPlaying) return;

        // Obtener espectro
        _audioSource.GetSpectrumData(_samples, 0, FFTWindow.BlackmanHarris);

        // Calcular energía de bajos (beat detection)
        float energy = 0;
        for (int i = 0; i < 20; i++)
        {
            energy += _samples[i] * _samples[i];
        }

        // Normalizar entre 0 y 1
        float normalizedEnergy = Mathf.Clamp01(energy / _maxEnergy);

        // Interpolar color
        Color targetColor = Color.Lerp(_lowEnergyColor, _highEnergyColor, normalizedEnergy);
        _spriteRenderer.color = Color.Lerp(_spriteRenderer.color, targetColor, Time.deltaTime * _smoothSpeed);
    }
}