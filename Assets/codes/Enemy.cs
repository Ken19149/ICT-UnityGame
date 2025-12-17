using UnityEngine;

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

    [Header("Dissolve Shader Settings")]
    [SerializeField] private SkinnedMeshRenderer skinnedMesh;      // Assign in Inspector
    [SerializeField] private float dissolveDuration = 1.5f;
    [SerializeField] private string dissolvePropertyName = "_DissolveAmount";

    private bool isDead = false;
    private bool isDissolving = false;
    private float dissolveT = 0f;
    private bool destroyScheduled = false;

    private Material dissolveMaterial;

    private void Reset()
    {
        // Helper when you add the script
        var col = GetComponent<Collider>();
        col.isTrigger = true;

        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (skinnedMesh != null)
        {
            // Get an instance so we don't modify the shared material
            dissolveMaterial = skinnedMesh.material;
            // Start fully visible
            dissolveMaterial.SetFloat(dissolvePropertyName, 0f);
        }
        else
        {
            Debug.LogWarning($"[Enemy] SkinnedMeshRenderer not assigned on {name}. Dissolve won't work.");
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

        // Notify GameManager
        if (GameManager.Instance != null)
        {
            GameManager.Instance.RegisterKill();
        }
        else
        {
            Debug.LogWarning("[Enemy] GameManager.Instance is null. Make sure a GameManager exists in the scene.");
        }

        // Trigger Die animation
        if (animator != null && !string.IsNullOrEmpty(dieTriggerName))
        {
            animator.SetTrigger(dieTriggerName);
        }
        else
        {
            // If no animator, start dissolve immediately
            StartDissolve();
        }
    }

    private void Update()
    {
        // 1) Wait until Die animation is finished, then start dissolve
        if (isDead && !isDissolving && animator != null)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(animatorLayerIndex);

            // Check if we're in the Die state and the clip has finished (normalizedTime >= 1)
            if (state.IsName(dieStateName) && state.normalizedTime >= 1f)
            {
                StartDissolve();
            }
        }

        // 2) If dissolving, update shader property over time
        if (isDissolving && dissolveMaterial != null)
        {
            dissolveT += Time.deltaTime / dissolveDuration;
            float value = Mathf.Clamp01(dissolveT);

            // Same behavior as your original: _DissolveAmount goes from 0 to -1
            dissolveMaterial.SetFloat(dissolvePropertyName, -value);

            if (value >= 1f && !destroyScheduled)
            {
                destroyScheduled = true;
                Destroy(gameObject, destroyDelayAfterDissolve);
            }
        }
    }

    private void StartDissolve()
    {
        if (isDissolving) return;

        isDissolving = true;
        dissolveT = 0f;
    }
}
