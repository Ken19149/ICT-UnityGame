using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Collider))]
public class Enemy : MonoBehaviour
{
    [Header("Collision Settings")]
    [SerializeField] private string playerTag = "Player";

    [Header("Game Logic")]
    [SerializeField] private float destroyDelayAfterDissolve = 0.2f;

    [Header("Animation Settings")]
    [SerializeField] private Animator animator;
    [SerializeField] private string dieTriggerName = "Die";
    [SerializeField] private string dieStateName = "Die";
    [SerializeField] private int animatorLayerIndex = 0;

    [Header("Fading & Scaling Settings")]
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float scaleMultiplier = 1.5f; 
    
    // Remember to use "_BaseColor" for URP or "_Color" for Standard
    [SerializeField] private string colorPropertyName = "_BaseColor"; 

    private bool isDead = false;
    private bool isFading = false;
    private float fadeT = 0f;
    private bool destroyScheduled = false;

    private Vector3 initialScale;

    // A small helper class to remember the material and its original color
    private class FadePart
    {
        public Material material;
        public Color initialColor;
    }
    private List<FadePart> fadeParts = new List<FadePart>();

    private void Reset()
    {
        var col = GetComponent<Collider>();
        col.isTrigger = true;
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        initialScale = transform.localScale;

        // 1. Find ALL renderers (MeshRenderer and SkinnedMeshRenderer) in this object and its children
        Renderer[] allRenderers = GetComponentsInChildren<Renderer>();

        foreach (Renderer r in allRenderers)
        {
            // A single renderer might have multiple materials (e.g. skin + cloth)
            foreach (Material mat in r.materials) 
            {
                if (mat.HasProperty(colorPropertyName))
                {
                    FadePart part = new FadePart();
                    part.material = mat;
                    part.initialColor = mat.GetColor(colorPropertyName);
                    fadeParts.Add(part);
                }
                else
                {
                    Debug.LogWarning($"[EnemyFader] Material '{mat.name}' is missing property '{colorPropertyName}'");
                }
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (isDead) return;
        if (!other.CompareTag(playerTag)) return;
        HandleDeath();
    }

    private void HandleDeath()
    {
        isDead = true;

        if (GameManager.Instance != null) GameManager.Instance.RegisterKill();

        if (animator != null && !string.IsNullOrEmpty(dieTriggerName))
        {
            animator.SetTrigger(dieTriggerName);
        }
        else
        {
            StartFading();
        }
    }

    private void Update()
    {
        // 1) Wait for animation
        if (isDead && !isFading && animator != null)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(animatorLayerIndex);
            if (state.IsName(dieStateName) && state.normalizedTime >= 1f)
            {
                StartFading();
            }
        }

        // 2) Handle Fading
        if (isFading)
        {
            fadeT += Time.deltaTime / fadeDuration;
            float normalizedTime = Mathf.Clamp01(fadeT); 

            // A: Scale Up
            transform.localScale = Vector3.Lerp(initialScale, initialScale * scaleMultiplier, normalizedTime);

            // B: Update ALL materials we found
            foreach (FadePart part in fadeParts)
            {
                float newAlpha = Mathf.Lerp(part.initialColor.a, 0f, normalizedTime);
                Color fadedColor = new Color(part.initialColor.r, part.initialColor.g, part.initialColor.b, newAlpha);
                part.material.SetColor(colorPropertyName, fadedColor);
            }

            // C: Destroy
            if (normalizedTime >= 1f && !destroyScheduled)
            {
                destroyScheduled = true;
                Destroy(gameObject, destroyDelayAfterDissolve);
            }
        }
    }

    private void StartFading()
    {
        if (isFading) return;
        isFading = true;
        fadeT = 0f;
    }
}
