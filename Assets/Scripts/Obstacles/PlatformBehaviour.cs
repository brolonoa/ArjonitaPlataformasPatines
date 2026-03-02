using System.Collections.Generic;
using UnityEngine;
using System.Collections;

public class PlatformBehaviour : MonoBehaviour
{
    [Header("Platform Configuration")]
    [SerializeField] private bool isFragile = false;
    [SerializeField] private float disappearanceTime = 1f;
    [SerializeField] private float respawnTime = 3f;


    [Header("Components")]
    private SpriteRenderer spriteRenderer;
    [SerializeField] Collider2D platformCollider;
    private Rigidbody2D rb;


    private bool platformActive = true;
    private bool playerOnPlatform = false;
    private Transform playerTransform;

    [SerializeField] bool canPlayerMoveAlong;


    void Start()
    {
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        platformCollider = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
    }


    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canPlayerMoveAlong)
        {

            playerTransform = collision.transform;
            playerOnPlatform = true;


            collision.transform.SetParent(transform);

            if (isFragile)
            {
                StartDisappearance();
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && canPlayerMoveAlong)
        {
            playerOnPlatform = false;
            playerTransform = null;


            if (collision.transform.parent == transform)
            {
                collision.transform.SetParent(null);
            }


        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player") && isFragile)
        {
            StartDisappearance();
        }
    }



    public void StartDisappearance()
    {
        if (platformActive)
        {
            StopAllCoroutines();
            StartCoroutine(DisappearanceCycle());
        }
    }

    private IEnumerator DisappearanceCycle()
    {
        platformActive = false;

        yield return new WaitForSeconds(0.5f);

        yield return StartCoroutine(BlinkBeforeDisappear());

        DeactivatePlatform();
        yield return new WaitForSeconds(respawnTime);
        ActivatePlatform();
    }

    private IEnumerator BlinkBeforeDisappear()
    {
        if (spriteRenderer != null)
        {
            float blinkTime = 0.3f;
            float timeBetweenBlinks = 0.1f;
            float endTime = Time.time + blinkTime;

            while (Time.time < endTime)
            {
                spriteRenderer.enabled = false;
                yield return new WaitForSeconds(timeBetweenBlinks);
                spriteRenderer.enabled = true;
                yield return new WaitForSeconds(timeBetweenBlinks);
            }
        }
    }

    private void DeactivatePlatform()
    {

        if (spriteRenderer != null)
            spriteRenderer.enabled = false;

        if (platformCollider != null)
            platformCollider.enabled = false;

        if (playerOnPlatform && playerTransform != null)
        {
            if (playerTransform.parent == transform)
            {
                playerTransform.SetParent(null);
            }
        }

    }

    private void ActivatePlatform()
    {

        if (spriteRenderer != null)
            spriteRenderer.enabled = true;

        if (platformCollider != null)
            platformCollider.enabled = true;


        platformActive = true;
    }

}   

