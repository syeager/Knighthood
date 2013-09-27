﻿// Steve Yeager
// 8.22.2013

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Base enemy class.
/// </summary>
public class Enemy : Character
{
    #region Reference Fields

    private NavAgent myNavAgent;

    #endregion

    #region State Fields

    public const string JumpingState = "Jumping";
    public float navBuffer = 0.5f;
    public float navStopRange = 3f;
    private bool fallingThrough;

    #endregion


    #region MonoBehaviour Overrides

    protected override void Awake()
    {
        base.Awake();

        // get references
        myNavAgent = GetComponent<NavAgent>();

        // create states
        CreateState(SpawningState, info => SetState(IdlingState, new Dictionary<string, object>()), info => {});
        CreateState(IdlingState, IdlingEnter, info => {});
        CreateState(MovingState, MovingEnter, MovingExit);
        CreateState(JumpingState, JumpingEnter, info => {});
        CreateState(FallingState, FallingEnter, info => {});

        initialState = IdlingState;
    }


    protected void Start()
    {
        StartInitialState(new Dictionary<string, object>{{"Target", LevelManager.Instance.PlayerTransforms[0]}});
    }

    #endregion

    #region State Methods

    private void IdlingEnter(Dictionary<string, object> info)
    {
        ClearVelocity();
        currentStateJob = new Job(IdlingUpdate());
    }


    private IEnumerator IdlingUpdate()
    {
        Transform player = LevelManager.Instance.PlayerTransforms[0];
        while (true)
        {
            if (Vector3.Distance(myTransform.position, player.position) > 5f)
            {
                SetState(MovingState, new Dictionary<string, object>{{"Target", player}});
                yield break;
            }
            yield return null;
        }
    }


    private void MovingEnter(Dictionary<string, object> info)
    {
        StartCoroutine("CalculatePath", info["Target"]);

        currentStateJob = new Job(MovingUpdate());
    }


    private IEnumerator MovingUpdate()
    {
        Vector3 nodePosition;
        while (true)
        {
            // idle
            if (!myNavAgent.GetNextNodePosition(out nodePosition))
            {
                SetState(IdlingState, new Dictionary<string, object>());
                yield break;
            }

            // rotate
            myMotor.SetRotation((nodePosition - myTransform.position).x);

            // jump
            if (nodePosition.y - myTransform.position.y >= myNavAgent.stepHeight)
            {
                SetState(JumpingState, new Dictionary<string, object>{{"target", nodePosition}});
                yield break;
            }

            // fall
            if (!myMotor.IsGrounded() || (nodePosition.y - myTransform.position.y < -myNavAgent.stepHeight && myMotor.OverTranslucent()))
            {
                SetState(FallingState, new Dictionary<string, object>{{"target", nodePosition}, {"translucent",  true}});
                yield break;
            }

            // move
            SetVelocityX(Mathf.Sign((nodePosition - myTransform.position).x) * moveSpeed);
            
            // next node
            if (Vector3.Distance(myTransform.position, nodePosition) <= myNavAgent.allowedRadius)
            {
                Vector3 nextNode;
                myNavAgent.Continue(out nextNode);
            }

            yield return null;
        }
    }


    private void MovingExit(Dictionary<string, object> info)
    {
        StopCoroutine("CalculatePath");
    }


    private void JumpingEnter(Dictionary<string, object> info)
    {
        ClearVelocity();
        currentStateJob = new Job(JumpingUpdate((Vector3)info["target"]));
    }


    private IEnumerator JumpingUpdate(Vector3 target)
    {
        float timeX = Mathf.Abs((target.x - myTransform.position.x)/moveSpeed);
        float velY = (Mathf.Abs(target.y - myTransform.position.y) + 0.5f*gravity*timeX*timeX)/timeX;
        Vector3 initialVel = new Vector3(Mathf.Sign((target-myTransform.position).x)*moveSpeed, velY, 0f);
        Log(initialVel, true, Debugger.LogTypes.Navigation);

        while (timeX > 0f)
        {
            Debug.DrawRay(myTransform.position, initialVel);
            timeX -= GameTime.deltaTime;
            SetVelocity(initialVel);
            initialVel += Vector3.down*gravity*GameTime.deltaTime;

            yield return null;
        }

        myNavAgent.Continue();
        SetState(MovingState, new Dictionary<string, object> { { "Target", LevelManager.Instance.PlayerTransforms[0] } });

        yield return null;
    }


    private void FallingEnter(Dictionary<string, object> info)
    {
        if (info.ContainsKey("translucent"))
        {
            fallingThrough = true;
            InvokeAction(() => fallingThrough = false, 0.2f);    
        }

        currentStateJob = new Job(FallingUpdate((Vector3)info["target"]));
    }


    private IEnumerator FallingUpdate(Vector3 target)
    {
        while (true)
        {
            // land
            if (!fallingThrough && myMotor.IsGrounded())
            {
                SetState(MovingState, new Dictionary<string, object>{{"Target", LevelManager.Instance.PlayerTransforms[0]}});
                yield break;
            }

            // move
            if (myMotor.velocity.y > -terminalVelocity)
            {
                AddVelocityY(-gravity * GameTime.deltaTime);
            }

            yield return null;
        }
    }

    #endregion

    #region Movement Methods

    /// <summary>
    /// Recalculate the NavAgent path at intervals.
    /// </summary>
    /// <param name="Target">Transform of destination.</param>
    private IEnumerator CalculatePath(Transform Target)
    {
        while (true)
        {
            myNavAgent.FindPath(Target.position);
            yield return WaitForTime(navBuffer);
        }
    }

    #endregion
}