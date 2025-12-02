using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
public enum PlayerAction
{
    Idle,
    Move,
    Chop,
    Water,
    Hoe
}

public class PlayerState
{
    protected PlayerStateMachine stateMachine;
    protected Player player;
    protected Rigidbody2D rb;

    protected float xInput;
    protected float yInput;

    protected PlayerAction action;
    protected float lastXInput = 0;
    protected float lastYInput = -1;


    protected float stateTimer;
    protected bool triggerCalled;

    public PlayerState(Player _player, PlayerStateMachine _stateMachine, PlayerAction _action)
    {
        this.player = _player;
        this.stateMachine = _stateMachine;
        this.action = _action;
    }

    public virtual void Enter()
    {
        rb = player.Rb;
        //triggerCalled = false;
        player.Anim.SetInteger("action", (int)action);
    }

    public virtual void Update()
    {
        float currentX = Input.GetAxisRaw("Horizontal");
        float currentY = Input.GetAxisRaw("Vertical");

        xInput = currentX;
        yInput = currentY;

        if (MathF.Abs(currentX) >= 0.1 || MathF.Abs(currentY) >= 0.1)
        {
            lastXInput = currentX;
            lastYInput = currentY;
            UpdateDirectionAnimation();
        }
    }

    public virtual void FixedUpdate()
    {

    }

    public virtual void Exit()
    {
        // 如果需要，可以在这里做一些退出动作处理
    }


    protected void UpdateDirectionAnimation()
    {
        player.Anim.SetFloat("lastXInput", lastXInput);
        player.Anim.SetFloat("lastYInput", lastYInput);
    }


    public virtual void AnimationFinishTrigger()
    {
        //triggerCalled = true;
    }
}
