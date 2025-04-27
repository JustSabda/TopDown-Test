using MoreMountains.Feedbacks;
using MoreMountains.Tools;
using MoreMountains.TopDownEngine;
using UnityEngine;

[AddComponentMenu("TopDown Engine/Character/Abilities/CharacterMindControl")]
public class MindControlAbility : CharacterAbility
{
    private class DamageTakenEventListener : MMEventListener<MMDamageTakenEvent>
    {
        public void OnMMEvent(MMDamageTakenEvent eventType)
        {
            var damageEmitter = eventType.Instigator.GetComponentInParent<Health>();
            if (damageEmitter == null)
            {
                Debug.LogWarning($"Damage Emitter Null..." +
                                 $"Effect may not be triggered...");
                return;
            }

            var damageTaker = eventType.AffectedHealth.gameObject;
            Debug.Log($"Current Object: {damageTaker.name}" +
                      $"\nDamaged by: {damageEmitter.name}");

            if (eventType.AffectedHealth.gameObject.layer == LayerMask.NameToLayer("Enemies"))
            {
                var mindControlWeaponEffect = damageEmitter.GetComponentInParent<MindControlWeaponEffect>();
                if (mindControlWeaponEffect != null)
                {
                    Debug.Log($"Mind Control Effect Triggered for: {damageTaker.name}");
                    if (damageTaker.TryGetComponent(out MindControlAbility characterMindControl))
                    {
                        characterMindControl.TriggerEffect();
                    }
                }
            }
        }
    }

    private readonly DamageTakenEventListener _damageTakenEventListener = new();
    private bool _isEffectActive;
    private float _currentEffectTimeElapsed;

    [SerializeField]
    private float radius = 3f;
    [SerializeField]
    private Vector3 detectionOriginOffset;
    [SerializeField]
    private float effectDuration = 5f;

    [Header("Override Parameter")]
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    [SerializeField] private LayerMask alliedLayer;
    [SerializeField] private LayerMask enemyLayer;

    [SerializeField] private AIDecisionDetectTargetRadius2D decisionHandler;
    [SerializeField] private AIBrain brain;


    public MMFeedbacks WinFeedbacks;

    protected override void OnEnable()
    {
        base.OnEnable();
        // Register the event listener for damage taken events
        MMEventManager.AddListener(_damageTakenEventListener);
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        // Unregister the event listener for damage taken events
        MMEventManager.RemoveListener(_damageTakenEventListener);
    }

    protected override void Initialization()
    {
        base.Initialization();
        _currentEffectTimeElapsed = 0f;
    }

    private void FixedUpdate()
    {
        // Decrease the effect time elapsed if the effect is active
        if (_currentEffectTimeElapsed > 0)
        {
            _currentEffectTimeElapsed -= Time.deltaTime;
            if (_currentEffectTimeElapsed <= 0)
            {
                // Reset the effect
                _currentEffectTimeElapsed = 0;
            }
        }
        else
        {
            // Make sure to trigger the reset effect once, if the effect is active
            if (_isEffectActive)
            {
                ResetEffect();
            }
        }
    }

    /// <summary>
    /// Triggers the mind control effect on the target.
    /// </summary>
    private void TriggerEffect()
    {
        // Change the sprite color to red to visually indicate the effect is active
        _spriteRenderer.color = Color.red;

        // Set the duration for which the effect will remain active
        _currentEffectTimeElapsed = effectDuration;

        // Change the layer of the decision handler to "EnemiesControlled" to reflect the new state
        decisionHandler.gameObject.layer = LayerMask.NameToLayer("EnemiesControlled");

        // Update the target layer of the decision handler to the enemy layer
        decisionHandler.TargetLayer = enemyLayer;

        // Mark the effect as active
        _isEffectActive = true;

        // Reset the AI brain to ensure it adapts to the new state
        brain.ResetBrain();
    }

    /// <summary>
    /// Resets the effect to its original state.
    /// </summary>
    private void ResetEffect()
    {
        //Reset the sprite color to white to indicate the effect is no longer active
        _spriteRenderer.color = Color.white;

        //Reset the effect duration timer to 0
        _currentEffectTimeElapsed = 0;

        //Change the decision handler's layer back to "Enemies" to reflect its original state
        decisionHandler.gameObject.layer = LayerMask.NameToLayer("Enemies");

        //Update the target layer of the decision handler to the allied layer
        decisionHandler.TargetLayer = alliedLayer;  

        //Mark the effect as inactive
        _isEffectActive = false;

        //Reset the AI brain to ensure it adapts to the reverted state
        brain.ResetBrain();

        WinFeedbacks?.PlayFeedbacks();

        LevelManager.Instance.PlayerWin();
    }
}