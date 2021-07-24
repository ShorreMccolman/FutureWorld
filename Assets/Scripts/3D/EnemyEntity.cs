using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

public enum Range
{
    Close = 0,
    Mid = 1,
    Long = 2,
    Out = 3,
    Inactive
}

public class EnemyEntity : Entity3D, IPopable, IMoveable, IAttacker
{
    [SerializeField] MapIcon MapIcon;

    public Enemy Enemy => State as Enemy;

    protected bool _isAlive;
    protected Range _range;

    protected Transform _target;

    protected bool _isAttackReady;
    protected float _rotateSpeed;

    protected CharacterController _controller;
    protected Collider _dropCollider;
    protected Animator _animator;

    protected Vector3 _origin;
    protected float _stuckDuration;

    bool _turnBased;
    bool _canMove;

    public static event System.Action<Enemy> OnEnemyDeath;
    public static event System.Action<Enemy> OnEnemyPickup;

    EntityStateMachine AI;

    // IMoveable
    public Transform Transform => transform;

    float _curSpeed;
    public float MovementSpeed 
    { 
        get { return _curSpeed; }
        set { _curSpeed = Mathf.Clamp(value, 0, Enemy.Data.CombatData.MoveSpeed); }
    }

    Quaternion _targetDirection;
    public void SetTargetDirection(Quaternion direction) => _targetDirection = direction;

    public virtual void Setup(Enemy enemy)
    {
        State = enemy;
        GameObject obj = Instantiate(enemy.Data.Model, transform);
        CullingObjects.Add(obj);

        _controller = GetComponent<CharacterController>();
        _dropCollider = GetComponent<SphereCollider>();
        _animator = GetComponentInChildren<Animator>();

        EnemyAnimReceiver receiver = GetComponentInChildren<EnemyAnimReceiver>();
        receiver.Setup(enemy);

        _range = Range.Out;
        _isAlive = true;
        obj.SetActive(false);

        _rotateSpeed = 6f;
        _isAttackReady = enemy.Cooldown <= 0;
        _origin = transform.position;
        MapIcon.gameObject.SetActive(false);

        MouseoverName = enemy.Data.DisplayName;
        IsTargetable = true;

        SetupBehaviour();

        Enemy.OnEnemyReady += EnemyReady;
        TurnController.OnTurnBasedToggled += ToggleTB;
        TurnController.OnEnemyMoveToggled += ToggleCanMove;
        Status.OnWizardEyeChanged += ToggleWizardEye;
    }

    void SetupBehaviour()
    {
        AI = new EntityStateMachine();

        IdleBehaviour idle = new IdleBehaviour(this);
        ZigZagBehaviour zigZag = new ZigZagBehaviour(this, Party.Instance.Entity.transform);
        PursueBehaviour pursue = new PursueBehaviour(this, Party.Instance.Entity.transform);
        AttackBehaviour attack = new AttackBehaviour(this, this, Party.Instance.Entity.transform);
        FleeBehaviour flee = new FleeBehaviour(this, Party.Instance.Entity.transform);

        TBIdleBehaviour tbIdle = new TBIdleBehaviour(this, Party.Instance.Entity.transform);
        TBPassBehaviour tbPass = new TBPassBehaviour(this, this, Party.Instance.Entity.transform);

        AI.AddTransition(idle, zigZag, () => _range < Range.Out && !_turnBased);
        AI.AddTransition(zigZag, idle, () => _range > Range.Long && !_turnBased);
        AI.AddTransition(zigZag, pursue, () => _range < Range.Long && !_turnBased);
        AI.AddTransition(pursue, zigZag, () => _range > Range.Mid && !_turnBased);
        AI.AddTransition(pursue, attack, () => _range < Range.Mid && !_turnBased);
        AI.AddTransition(attack, pursue, () => _range > Range.Close && !Enemy.MovementLocked && !_turnBased);


        AI.AddTransition(tbIdle, () => _range < Range.Out && Enemy.IsHostile && !_canMove && !Enemy.AwaitingTurn && _turnBased);
        AI.AddTransition(tbIdle, pursue, () => _canMove && _range > Range.Close && _turnBased);
        AI.AddTransition(pursue, tbIdle, () => _canMove && _range == Range.Close && _turnBased);
        AI.AddTransition(tbIdle, attack, () => Enemy.AwaitingTurn && _range == Range.Close && _turnBased);
        AI.AddTransition(tbIdle, tbPass, () => Enemy.AwaitingTurn && _range > Range.Close && _turnBased);

        AI.AddTransition(tbIdle, idle, () => !_turnBased);


        switch (Enemy.Data.CombatData.AIType)
        {
            case EnemyAIType.Wimp:
                AI.AddTransition(flee, () => Enemy.CurrentHP < Enemy.Data.HitPoints * 0.3f && _canMove && !Enemy.MovementLocked, 2); 
                break;
            case EnemyAIType.Normal:
                AI.AddTransition(flee, () => Enemy.CurrentHP < Enemy.Data.HitPoints * 0.175f && _canMove && !Enemy.MovementLocked, 2);
                break;
            case EnemyAIType.Aggressive:
                AI.AddTransition(flee, () => Enemy.CurrentHP < Enemy.Data.HitPoints * 0.05f && _canMove && !Enemy.MovementLocked, 2);
                break;
            case EnemyAIType.Suicidal:
                break;
        }

        if (Enemy.IsHostile)
        {
            AI.AddTransition(idle, () => !_isVisible, 10);

            AI.SetState(idle);
        } 
        else
        {
            RoamBehaviour roam = new RoamBehaviour(this, _origin);
            WatchBehaviour watch = new WatchBehaviour(this, Party.Instance.Entity.transform);

            AI.AddTransition(idle, () => Enemy.IsHostile);
            AI.AddTransition(watch, () => _turnBased);

            AI.AddTransition(roam, roam, () => _stuckDuration > 2.0f);
            AI.AddTransition(roam, watch, () => _range < Range.Mid);
            AI.AddTransition(watch, roam, () => _range > Range.Close && !_turnBased);

            AI.SetState(roam);
        }
    }

    void Update()
    {
        if (_isAlive)
        {
            AI.Update();
        }
    }
    void FixedUpdate()
    {
        if (_isAlive && _isVisible)
        {
            Rotate();
            Move();
        }
    }

    void ToggleTB(bool enable)
    {
        _turnBased = enable;
        _canMove = !enable;
    }

    void ToggleWizardEye(bool enabled, SkillProficiency proficiency)
    {
        MapIcon.gameObject.SetActive(enabled);
    }

    void ToggleCanMove(bool enable)
    {
        _canMove = enable;
    }

    public void EnemyReady()
    {
        _isAttackReady = true;
    }

    public override void EnterSphere(SphereLevel level)
    {
        base.EnterSphere(level);

        if (level == SphereLevel.Two)
        {
            _target = Party.Instance.Entity.transform;
            _range = Range.Long;
        }
        if (level == SphereLevel.One)
        {
            _target = Party.Instance.Entity.transform;
            _range = Range.Mid;
        }
        else if (level == SphereLevel.Zero)
        {
            _target = Party.Instance.Entity.transform;
            _range = Range.Close;
        }
    }

    public override void ExitSphere(SphereLevel level)
    {
        base.ExitSphere(level);

        if (level == SphereLevel.Two)
        {
            _target = null;
            _range = Range.Out;
        }
        if (level == SphereLevel.One)
        {
            _target = Party.Instance.Entity.transform;
            _range = Range.Long;
        }
        else if (level == SphereLevel.Zero)
        {
            _target = Party.Instance.Entity.transform;
            _range = Range.Mid;
        }
    }

    public void TryDodge()
    {
        if (_isAttackReady)
        {
            Enemy.DoDodge();
        }
    }

    public void TryAttack()
    {
        if (_isAttackReady)
        {
            _animator.SetTrigger("DoAttack");
            _isAttackReady = false;
            Enemy.LockMovement(true);
        }
    }

    public void OnDeath()
    {
        _animator.SetTrigger("Death");
        _animator.SetFloat("MoveSpeed", 0);
        _animator.SetFloat("TurnSpeed", 0);
        MovementSpeed = 0;

        _controller.enabled = false;
        _dropCollider.enabled = true;

        _isAlive = false;
        IsTargetable = false;

        OnEnemyDeath?.Invoke(Enemy);
    }

    public override IEnumerator Interact(PartyEntity party)
    {
        if (!_isAlive)
        {
            yield return new WaitForEndOfFrame();

            OnEnemyPickup?.Invoke(Enemy);
            Kill();
        }
    }

    public void ShowPopup()
    {
        Popups.ShowEnemy(Enemy);
    }

    protected void Rotate()
    {
        float str = _rotateSpeed * Time.fixedDeltaTime;
        transform.rotation = Quaternion.Lerp(transform.rotation, _targetDirection, str);

        //_animator.SetFloat("TurnSpeed", 0);
    }

    protected void Move()
    {
        Vector3 oldPos = transform.position;
        Vector3 desiredMove = transform.forward;

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, _controller.radius, Vector3.down, out hitInfo,
                           _controller.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        Vector3 moveDir = Vector3.zero;
        moveDir.x = desiredMove.x * MovementSpeed;
        moveDir.z = desiredMove.z * MovementSpeed;

        if (_controller.isGrounded)
        {
            moveDir.y = -10;
        }
        else
        {
            moveDir += Physics.gravity * 2 * Time.fixedDeltaTime;
        }

        CollisionFlags flags = _controller.Move(moveDir * Time.fixedDeltaTime);

        double distance = Vector3.Distance(oldPos, transform.position);
        bool isStill = distance <= 0.01;

        if (_curSpeed > 0 && isStill)
            _stuckDuration += Time.fixedDeltaTime;
        else
            _stuckDuration = 0;

        _animator.SetFloat("MoveSpeed", isStill ? 0 : 1);
    }
}