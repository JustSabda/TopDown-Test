using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;
using MoreMountains.TopDownEngine;
using static MoreMountains.TopDownEngine.Character;

[AddComponentMenu("TopDown Engine/Character/Abilities/CharacterMindControl")]
public class CharacterMindControl : CharacterAbility
{

    //[Header("TODO_HEADER")]
    ///// declare your parameters here
    //public float randomParameter = 4f;
    //public bool randomBool;

    protected const string _yourAbilityAnimationParameterName = "YourAnimationParameterName";
    protected int _yourAbilityAnimationParameter;

    /// <summary>
    /// Here you should initialize our parameters
    /// </summary>
    /// 

    public float Radius = 3f;

    public Vector3 DetectionOriginOffset = new Vector3(0, 0, 0);

    protected Vector2 _facingDirection;
    protected Vector2 _raycastOrigin;
    protected Color _gizmoColor = Color.yellow;
    protected bool _init = false;
    protected Collider2D[] _results;

    protected override void Initialization()
    {
        base.Initialization();
        _init = true;
        //randomBool = false;
    }

    /// <summary>
    /// Every frame, we check if we're crouched and if we still should be
    /// </summary>
    public override void ProcessAbility()
    {
        base.ProcessAbility();
    }

    /// <summary>
    /// Called at the start of the ability's cycle, this is where you'll check for input
    /// </summary>
    protected override void HandleInput()
    {
        // here as an example we check if we're pressing down
        // on our main stick/direction pad/keyboard
        if (_inputManager.TimeControlButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
        {
            DoSomething();
        }
        //if (_inputManager.TimeControlButton.State.CurrentState == MMInput.ButtonStates.ButtonUp)
        //{
        //    TimeControlStop();
        //}
    }

    /// <summary>
    /// If we're pressing down, we check for a few conditions to see if we can perform our action
    /// </summary>
    protected virtual void DoSomething()
    {
        ComputeRaycastOrigin();



        //// if the ability is not permitted
        //if (!AbilityPermitted
        //    // or if we're not in our normal stance
        //    || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal)
        //    // or if we're grounded
        //    || (!_controller.Grounded))
        //{
        //    // we do nothing and exit
        //    return;
        //}

        // if we're still here, we display a text log in the console
        MMDebug.DebugLogTime("We're doing something yay!");
    }

    protected virtual void ComputeRaycastOrigin()
    {
        _raycastOrigin = transform.position + DetectionOriginOffset;

    }

    /// <summary>
    /// Adds required animator parameters to the animator parameters list if they exist
    /// </summary>
    protected override void InitializeAnimatorParameters()
    {
        RegisterAnimatorParameter(_yourAbilityAnimationParameterName, AnimatorControllerParameterType.Bool, out _yourAbilityAnimationParameter);
    }

    /// <summary>
    /// At the end of the ability's cycle,
    /// we send our current crouching and crawling states to the animator
    /// </summary>
    public override void UpdateAnimator()
    {

        bool myCondition = true;
        MMAnimatorExtensions.UpdateAnimatorBool(_animator, _yourAbilityAnimationParameter, myCondition, _character._animatorParameters);
    }

    protected virtual void OnDrawGizmosSelected()
    {
        _raycastOrigin.x = transform.position.x + _facingDirection.x * DetectionOriginOffset.x / 2;
        _raycastOrigin.y = transform.position.y + DetectionOriginOffset.y;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_raycastOrigin, Radius);
        if (_init)
        {
            Gizmos.color = _gizmoColor;
            Gizmos.DrawSphere(_raycastOrigin, Radius);
        }
    }

}
