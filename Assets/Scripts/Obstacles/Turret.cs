using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private GameObject _proyectil;
    [SerializeField] private Transform _puntoDisparo;
    [SerializeField] private float _intervalo = 1f;
    [SerializeField] private bool _dispararAlInicio = true;

    [Header("Options")]
    [SerializeField] private bool _activarAlInicio = true;

    private bool _estaDisparando = false;

    void Start()
    {
        if (_activarAlInicio)
        {
            IniciarDisparo();
        }
    }

    public void IniciarDisparo()
    {
        if (_estaDisparando) return;

        float delay = _dispararAlInicio ? 0f : _intervalo;
        InvokeRepeating(nameof(Disparar), delay, _intervalo);
        _estaDisparando = true;
    }

    public void DetenerDisparo()
    {
        CancelInvoke(nameof(Disparar));
        _estaDisparando = false;
    }

    void Disparar()
    {
        if (_proyectil == null || _puntoDisparo == null)
        {
            Debug.LogWarning($"[{gameObject.name}] Falta asignar proyectil o punto de disparo!");
            return;
        }

        Instantiate(_proyectil, _puntoDisparo.position, _puntoDisparo.rotation);
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
}