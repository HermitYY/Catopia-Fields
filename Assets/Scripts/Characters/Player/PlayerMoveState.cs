using UnityEngine;

public class PlayerMoveState : PlayerState
{
    private Vector2 movementInput;
    private float currentSpeed;
    public PlayerMoveState(Player _player, PlayerStateMachine _stateMachine, PlayerAction _action) : base(_player, _stateMachine, _action)
    {
    }
    public override void Enter()
    {
        base.Enter();
        currentSpeed = 0f; // 从0开始加速
    }

    public override void Update()
    {
        base.Update();

        movementInput = new Vector2(xInput, yInput);

        if (xInput == 0 && yInput == 0)
        {
            stateMachine.ChangeState(player.IdleState);
        }
    }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        HandleMovement();
    }


    public override void Exit()
    {
        base.Exit();
        rb.velocity = Vector2.zero;
    }

    private void HandleMovement()
    {
        // 归一化输入方向（防止斜向移动更快）
        Vector2 inputDirection = movementInput.normalized;

        // 从player获取移动速度
        float targetSpeed = player.MoveSpeed;

        // 平滑加速/减速
        float acceleration = player.MoveAcceleration;
        float deceleration = player.MoveDeceleration;

        if (movementInput.magnitude > 0.1f)
        {
            // 加速到目标速度
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            // 减速到停止
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0f, deceleration * Time.deltaTime);
        }

        // 设置刚体速度
        rb.velocity = inputDirection * currentSpeed;
    }
}
