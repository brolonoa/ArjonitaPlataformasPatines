using System.Collections;
using UnityEngine;

public class Turret : MonoBehaviour, IParryable
{
    [Header("Shooting")]
    [SerializeField] private GameObject _proyectil;
    [SerializeField] private GameObject parryProjectil;
    [SerializeField] private Transform _puntoDisparo;
    [SerializeField] private float _intervalo = 1f;
    [SerializeField] private bool _dispararAlInicio = true;

    [Header("Direction")]
    [SerializeField] private bool _invertirDireccion;
    [SerializeField] private Transform _visual;

    [Header("Options")]
    [SerializeField] private bool _activarAlInicio = true;

    private bool _estaDisparando = false;

    private int projectileIndex;
    [SerializeField] private int parryProjectileIndex;
    [SerializeField] float stunedTime;
    private bool isStuned;

    [SerializeField] private bool canBeParried;
    public bool CanBeParried => canBeParried;

    [SerializeField] Animator anim;

    private int direccion = 1;

    void Start()
    {
        ConfigurarDireccion();

        if (_activarAlInicio)
        {
            IniciarDisparo();
        }

        canBeParried = true;
    }

    void ConfigurarDireccion()
    {
        direccion = _invertirDireccion ? -1 : 1;

        if (_visual != null)
        {
            _visual.localScale = new Vector3(direccion, 1, 1);
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
        anim.SetTrigger("shoot");
        projectileIndex++;

        GameObject prefabToSpawn;

        if (projectileIndex == parryProjectileIndex)
        {
            projectileIndex = 0;
            prefabToSpawn = parryProjectil;
        }
        else
        {
            prefabToSpawn = _proyectil;
        }

        GameObject bullet = Instantiate(prefabToSpawn, _puntoDisparo.position, Quaternion.identity);

        Bullet_0 bulletScript = bullet.GetComponent<Bullet_0>();
        if (bulletScript != null)
        {
            bulletScript.SetDirection(Vector2.right * direccion);
        }
    }

    private void OnDestroy()
    {
        CancelInvoke();
    }

    public void OnParry()
    {
        if (!isStuned)
        {
            StartCoroutine(StunedTime());
        }
    }

    IEnumerator StunedTime()
    {
        anim.SetTrigger("stuned");
        DetenerDisparo();
        isStuned = true;
        canBeParried = false;

        yield return new WaitForSeconds(stunedTime);

        anim.SetTrigger("recoberStun");
        yield return new WaitForSeconds(1.1f);

        isStuned = false;
        canBeParried = true;
        IniciarDisparo();
    }
}