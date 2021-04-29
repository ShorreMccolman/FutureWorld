using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Xml;

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

    Transform _target;

    [SerializeField] float moveSpeed = 3f;
    float _curMoveSpeed;

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

        _aiState = AIState.Passive;
        _isActive = true;

        MouseoverName = enemy.Data.DisplayName;
        IsTargetable = true;
    }

    public override void OnEnterSphere(Party party, SphereLevel level)
    {
        base.OnEnterSphere(party, level);

        if(level == SphereLevel.Two)
        {
            _target = party.Entity.transform;
            _aiState = AIState.Pursuit;
        }
    }

    public override void OnExitSphere(Party party, SphereLevel level)
    {
        base.OnExitSphere(party, level);

        if (level == SphereLevel.Two)
        {
            _target = null;
            _aiState = AIState.Passive;
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

            switch (_aiState)
            {
                case AIState.Passive:
                    _animator.SetFloat("MoveSpeed", 0);
                    _animator.SetFloat("TurnSpeed", 0);
                    _curMoveSpeed = 0;
                    break;
                case AIState.Pursuit:
                    levelPosition = new Vector3(_target.position.x, transform.position.y, _target.position.z);

                    Quaternion targetRotation = Quaternion.LookRotation(levelPosition - transform.position);
                    float str = Mathf.Min(rotateSpeed * Time.fixedDeltaTime, 1);
                    transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, str);

                    targetVec = (levelPosition - transform.position);
                    float ang = Vector3.Angle(transform.forward, targetVec);
                    if(ang < 25f)
                    {
                        if (targetVec.sqrMagnitude > 2f * 2f)
                        {
                            _curMoveSpeed += 5.0f * Time.fixedDeltaTime;
                            if (_curMoveSpeed > moveSpeed)
                                _curMoveSpeed = moveSpeed;

                            Vector3 oldPos = transform.position;

                            Move(_curMoveSpeed);
                      
                            double distance = Vector3.Distance(oldPos, transform.position);

                            _animator.SetFloat("MoveSpeed", distance == 0 ? 0 : 1);
                        }
                        else
                        {
                            _aiState = AIState.Combat;
                        }
                    }
                    break;
                case AIState.Combat:

                    _curMoveSpeed = 0;
                    _animator.SetFloat("MoveSpeed", 0);

                    levelPosition = new Vector3(_target.position.x, transform.position.y, _target.position.z);
                    targetVec = (levelPosition - transform.position);
                    if (targetVec.sqrMagnitude > 3f * 3f)
                    {
                        _aiState = AIState.Pursuit;
                    }
                    else
                    {
                        if (Enemy.Cooldown <= 0)
                        {
                            _animator.SetTrigger("DoAttack");
                            Enemy.ReadyAttack();
                        }
                    }
                    break;
                case AIState.Flee:
                    break;
            }

            Enemy.TickCooldown(Time.fixedDeltaTime);
        }
    }

    void Move(float currentMoveSpeed)
    {
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
    }
}