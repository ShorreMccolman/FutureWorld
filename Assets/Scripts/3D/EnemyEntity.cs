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

    protected bool _isActive;
    protected AIState _aiState;
    protected Range _range;

    protected Transform _target;

    [SerializeField] protected float moveSpeed = 3f;
    protected float _curMoveSpeed;

    protected bool attackReady;

    protected float rotateSpeed = 3f;

    protected CharacterController _controller;
    protected Collider _dropCollider;
    protected Animator _animator;

    protected Vector3 _origin;
    protected Vector3 _roamTarget;
    protected float _idleDuration;

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
        _idleDuration = Random.Range(5f, 10f);

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
        PartyController.Instance.Party.OnEnemyDeath(Enemy);
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
            if (Enemy.IsHostile)
                HostilePursuit();
            else
                Roam();
        }
    }

    void HostilePursuit()
    {
        Vector3 levelPosition;
        Vector3 targetVec;

        if (Enemy.MoveCooldown > 0)
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

    void Roam()
    {
        Vector3 levelPosition;
        Vector3 targetVec;
        Quaternion targetRotation;

        switch (_range)
        {
            case Range.Out:
            case Range.Long:
            case Range.Mid:

                if (_idleDuration <= 0)
                    RefreshRoamTarget();

                levelPosition = new Vector3(_roamTarget.x, transform.position.y, _roamTarget.z);

                float dist = (transform.position - levelPosition).magnitude;
                if (dist > 1.0f)
                {
                    targetRotation = Quaternion.LookRotation(levelPosition - transform.position);
                    float str = Mathf.Min(rotateSpeed * Time.fixedDeltaTime, 1);
                    Quaternion rot = Quaternion.Lerp(transform.rotation, targetRotation, str);
                    transform.rotation = Quaternion.Euler(0, rot.eulerAngles.y, 0);

                    if (!TimeManagement.IsCombatMode)
                    {
                        targetVec = (levelPosition - transform.position);
                        float ang = Vector3.Angle(transform.forward, targetVec);
                        if (ang < 25f)
                        {
                            _curMoveSpeed += 5.0f * Time.fixedDeltaTime;
                            if (_curMoveSpeed > moveSpeed)
                                _curMoveSpeed = moveSpeed;

                            //Ray ray = new Ray(transform.position, transform.forward);
                            //RaycastHit hit;
                            //if (Physics.Raycast(ray, out hit))
                            //{
                            //    if (hit.distance < 1)
                            //    {
                            //        _roamTarget = transform.position;
                            //    }
                            //}

                            Move(_curMoveSpeed);
                        }
                    }
                }
                else
                {
                    _curMoveSpeed = 0;
                    _animator.SetFloat("MoveSpeed", 0);
                    _idleDuration -= Time.fixedDeltaTime;
                }

                break;
            case Range.Close:

                _curMoveSpeed = 0;
                _animator.SetFloat("MoveSpeed", 0);

                levelPosition = new Vector3(_target.position.x, transform.position.y, _target.position.z);

                targetRotation = Quaternion.LookRotation(levelPosition - transform.position);
                float strength = Mathf.Min(rotateSpeed * Time.fixedDeltaTime, 1);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, strength);

                break;
        }
    }

    public void RefreshRoamTarget(bool initial = false)
    {
        if (initial)
        {
            _idleDuration = Random.Range(0f, 10f);
            _roamTarget = transform.position;
            _origin = _roamTarget;
        }
        else
        {
            _idleDuration = Random.Range(7f, 25f);

            if (Vector3.Distance(_roamTarget, _origin) > 30f)
            {
                _roamTarget = _origin;
                return;
            }

            float angle = Random.Range(0, 2 * Mathf.PI);
            Vector3 dir = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

            float distance = Random.Range(7f, 15f);

            Ray ray = new Ray(transform.position, dir);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                float reduced = hit.distance - 3f;
                if (reduced < 0)
                {
                    _idleDuration = 0.5f;
                    distance = 0;
                }
                else if (reduced < distance)
                    distance = reduced;
            }

            _roamTarget = transform.position + dir * distance;
        }
    }

    public virtual void Tick(float tick)
    {
        bool tickEvent = Enemy.TickCooldown(tick);
        if(tickEvent)
        {
            attackReady = true;
        }
    }

    protected void Move(float currentMoveSpeed)
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