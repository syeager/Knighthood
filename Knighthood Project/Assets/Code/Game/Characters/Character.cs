﻿// Steve Yeager
// 8.18.2013

using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Base class for any myCharacter.
/// </summary>
[RequireComponent(typeof(Health))]
public class Character : BaseMono
{
    #region Reference Fields

    protected CharacterMotor myMotor;
    protected Transform myTransform;
    protected Rigidbody myRigidbody;

    #endregion

    #region State Fields

    public enum States { Spawning, Idling, Moving, Jumping, Falling, Attacking, Flinching, Dying }
    protected States currentState;
    protected States initialState;
    private readonly Dictionary<States, Action<Dictionary<string, object>>> enterMethods = new Dictionary<States, Action<Dictionary<string, object>>>();
    private readonly Dictionary<States, Action<Dictionary<string, object>>> exitMethods = new Dictionary<States, Action<Dictionary<string, object>>>();
    protected Job currentStateJob;

    #endregion

    #region Movement Fields

    public float moveSpeed;
    protected Vector3 velocity;
    public float gravity;
    public float terminalVelocity;
    /// <summary>How fast the myCharacter moves upward.</summary>
    public float jumpSpeed = 21;
    /// <summary>How long the myCharacter can jump.</summary>
    public float climbTime = 0.3f;

    #endregion

    #region Stat Fields

    public StatManager StatManager;// { get; protected set; }

    #endregion


    #region MonoBehaviour Overrides

    protected virtual void Awake()
    {
        // get references
        myMotor = GetComponent<CharacterMotor>();
        myTransform = transform;
        myRigidbody = rigidbody;
    }

    #endregion

    #region State Methods

    /// <summary>
    /// Create a new currentState.
    /// </summary>
    /// <param name="stateName">State.</param>
    /// <param name="enterMethod">Enter method for currentState.</param>
    /// <param name="exitMethod">Exit method for currentState.</param>
    protected void CreateState(States stateName, Action<Dictionary<string, object>> enterMethod, Action<Dictionary<string, object>> exitMethod)
    {
        enterMethods.Add(stateName, enterMethod);
        exitMethods.Add(stateName, exitMethod);
    }


    /// <summary>
    /// Exit the current state and enter the new one.
    /// </summary>
    /// <param name="stateName">State to transition to.</param>
    /// <param name="info">Info to pass to the exit and enter states.</param>
    public void SetState(States stateName, Dictionary<string, object> info)
    {
        // save previous state
        if (info == null)
        {
            info = new Dictionary<string, object>();
        }
        info.Add("previous state", currentState);

        // exit state
        exitMethods[currentState](info);
        currentStateJob.Kill();

        // enter state
        Log(name + " Entering: " + stateName + " - " + Time.timeSinceLevelLoad);
        currentState = stateName;
        enterMethods[currentState](info);
    }


    /// <summary>
    /// Start the initial state. Doesn't call any exit methods.
    /// </summary>
    /// <param name="info">Info to pass to state enter method.</param>
    protected void StartInitialState(Dictionary<string, object> info)
    {
        currentState = initialState;
        enterMethods[initialState](info);
    }

    #endregion

    #region Public Methods

    /// <summary>
    /// Change the character to Attacking state.
    /// </summary>
    /// <param name="attackTexture">Texture corresponding to the attack being performed.</param>
    public void Attack(Texture attackTexture)
    {
        SetState(States.Attacking, new Dictionary<string, object> { { "attackTexture", attackTexture } });
    }


    /// <summary>
    /// End the current attack state. Set to Idle.
    /// </summary>
    public void EndAttack()
    {
        SetState(States.Idling, null);
    }

    #endregion
}