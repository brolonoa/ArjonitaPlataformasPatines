using UnityEngine;

public class Turret : MonoBehaviour
{
    [Header("Shooting")]
    [SerializeField] private GameObject _proyectil;
    [SerializeField] private GameObject parryProjectil;
    [SerializeField] private Transform _puntoDisparo;
    [SerializeField] private float _intervalo = 1f;
    [SerializeField] private bool _dispararAlInicio = true;

    [Header("Options")]
    [SerializeField] private bool _activarAlInicio = true;

    private bool _estaDisparando = false;

    private int projectileIndex;
    [SerializeField] private int parryProjectileIndex;

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
        projectileIndex++;

        if(projectileIndex == parryProjectileIndex)
        {
            projectileIndex = 0;
            Instantiate(parryProjectil, _puntoDisparo.position, _puntoDisparo.rotation);
           
        }
        else
        {
            Instantiate(_proyectil, _puntoDisparo.position, _puntoDisparo.rotation);
        }
            
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }
}