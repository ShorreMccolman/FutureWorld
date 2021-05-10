using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

public enum Range
{
    Out,
    Long,
    Mid,
    Close
}

public enum AIState
{
    Passive,
    Alert,
    Pursuit,
    Combat,
    Flee
}

public class EnemyEntity : Entity3D
{
    public Enemy Enemy { get { return State as Enemy; } }
    public bool IsAlive { get { return _isActive; } }

    bool _isActive;
    AIState _aiState;
    Range _range;

    Transform _target;

    [SerializeField] float moveSpeed = 3f;
    float _curMoveSpeed;

    bool attackReady;

    float rotateSpeed = 3f;

    CharacterController _controller;
    Collider _dropCollider;
    Animator _animator;

    public void Setup(Enemy enemy)
    {
        State = enemy;
        GameObject obj = Instantiate(enemy.Data.model, transform);

        _controller = GetComponent<CharacterController>();
        _dropCollider = GetComponent<SphereCollider>();
        _animator = GetComponentInChildren<Animator>();

        EnemyAnimReceiver receiver = GetComponentInChildren<EnemyAnimReceiver>();
        receiver.Setup(enemy);

        TimeManagement.Instance.OnTick += Tick;

        _aiState = AIState.Passive;
        _range = Range.Out;
        _isActive = true;

        attackReady = enemy.Cooldown <= 0;

        MouseoverName = enemy.Data.DisplayName;
        IsTargetable = true;
    }

    public override void OnEnterSphere(Party party, SphereLevel level)
    {
        base.OnEnterSphere(party, level);

        if(level == SphereLevel.Two)
        {
            _target = party.Entity.transform;
            _range = Range.Long;
        }
        if (level == SphereLevel.One)
        {
            _target = party.Entity.transform;
            _range = Range.Mid;
        }
        else if (level == SphereLevel.Zero)
        {
            _target = party.Entity.transform;
            _range = Range.Close;
        }
    }

    public override void OnExitSphere(Party party, SphereLevel level)
    {
        base.OnExitSphere(party, level);

        if (level == SphereLevel.Two)
        {
            _target = null;
            _range = Range.Out;
        }
        if (level == SphereLevel.One)
        {
            _target = party.Entity.transform;
            _range = Range.Long;
        }
        else if (level == SphereLevel.Zero)
        {
            _target = party.Entity.transform;
            _range = Range.Mid;
        }
    }

    public void OnDeath()
    {
        _animator.SetTrigger("Death");

        _animator.SetFloat("MoveSpeed", 0);
        _animator.SetFloat("TurnSpeed", 0);
        _curMoveSpeed = 0;

        _controller.enabled = false;
        _dropCollider.enabled = true;

        _isActive = false;
        IsTargetable = false;

        TimeManagement.Instance.OnTick -= Tick;
    }

    public override IEnumerator Interact(PartyEntity party)
    {
        if (!IsAlive)
        {
            bool success = HUD.Instance.PickupCorpse(Enemy);
            yield return new WaitForEndOfFrame();
            if (success)
            {
                Kill();
            }
        }
    }

    void LateUpdate()
    {
        if (_isActive)
        {
            Vector3 levelPosition;
            Vector3 targetVec;

            if(Enemy.MoveCooldown > 0)
            {
                Enemy.MoveCooldown -= Time.fixedDeltaTime;
            }

            switch (_range)
            {
                case Range.Out:
                    _animator.SetFloat("MoveSpeed", 0);
                    _animator.SetFloat("TurnSpeed", 0);
                    _curMoveSpeed = 0;
                    break;
                case Range.Long:
                case Range.Mid:
                    levelPosition = new Vector3(_target.position.x, transform.position.y, _target.position.z);

                    Quaternion targetRotation = Quaternion.LookRotation(levelPosition - transform.position);
                    float str = Mathf.Min(rotateSpeed * Time.fixedDeltaTime, 1);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);

                    if (!TimeManagement.IsCombatMode || Enemy.MoveCooldown > 0)
                    {
                        targetVec = (levelPosition - transform.position);
                        float ang = Vector3.Angle(transform.forward, targetVec);
                        if (ang < 25f && !Enemy.MovementLocked)
                        {
                            _curMoveSpeed += 5.0f * Time.fixedDeltaTime;
                            if (_curMoveSpeed > moveSpeed)
                                _curMoveSpeed = moveSpeed;

                            Move(_curMoveSpeed);
                        }
                    } 
                    else
                    {
                        _curMoveSpeed = 0;
                        _animator.SetFloat("MoveSpeed", 0);
                    }
                    break;
                case Range.Close:

                    _curMoveSpeed = 0;
                    _animator.SetFloat("MoveSpeed", 0);

                    levelPosition = new Vector3(_target.position.x, transform.position.y, _target.position.z);
                    targetVec = (levelPosition - transform.position);
                    if (attackReady && Enemy.MoveCooldown <= 0)
                    {
                        Enemy.LockMovement(true);
                        _animator.SetTrigger("DoAttack");
                        attackReady = false;
                    }
                    break;
            }
        }
    }

    public void Tick(float tick)
    {
        bool tickEvent = Enemy.TickCooldown(tick);
        if(tickEvent)
        {
            attackReady = true;
        }
    }

    void Move(float currentMoveSpeed)
    {
        Vector3 oldPos = transform.position;

        Vector3 desiredMove = transform.forward;

        // get a normal for the surface that is being touched to move along it
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, _controller.radius, Vector3.down, out hitInfo,
                           _controller.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

        Vector3 moveDir = Vector3.zero;
        moveDir.x = desiredMove.x * moveSpeed;
        moveDir.z = desiredMove.z * moveSpeed;

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
        bool isStill = distance == 0;
        _animator.SetFloat("MoveSpeed", isStill ? 0 : 1);
    }
}