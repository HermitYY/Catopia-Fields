using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditorInternal;
using UnityEngine;

public class Player : MonoBehaviour
{
    [HideInInspector] public Animator Anim;
    public Rigidbody2D Rb {get; private set;}

    [Header("MoveInfo")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float moveAcceleration = 10f; // 加速度
    [SerializeField] private float moveDeceleration = 15f; // 减速度
    public float MoveSpeed => moveSpeed;
    public float MoveAcceleration => moveAcceleration;
    public float MoveDeceleration => moveDeceleration;
    #region 状态机
    public PlayerStateMachine StateMachine { get; private set; }
    public PlayerIdleState IdleState { get; private set; }
    public PlayerMoveState MoveState { get; private set; }
    #endregion

    private void Awake()
    {
        StateMachine = new PlayerStateMachine();
        IdleState = new PlayerIdleState(this, StateMachine, PlayerAction.Idle);
        MoveState = new PlayerMoveState(this, StateMachine, PlayerAction.Move);
    }
    void Start()
    {
        Rb = GetComponent<Rigidbody2D>();
        Anim = GetComponentInChildren<Animator>();

        StateMachine.Initialize(IdleState);
    }

    void Update()
    {
        StateMachine.currentState.Update();
    }

    private void FixedUpdate()
    {
        StateMachine.currentState.FixedUpdate();
    }
}
