using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(Player _player, PlayerStateMachine _stateMachine, PlayerAction _action):base(_player, _stateMachine, _action)
    {
    }

    public override void Update()
    {
        base.Update();
        if (xInput != 0 || yInput != 0)
        {
            stateMachine.ChangeState(player.MoveState);
        }
    }
}
