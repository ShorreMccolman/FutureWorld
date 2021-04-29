using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlState
{
    MouseControl,
    LookControl,
    MenuLock,
    Previous
}

public class MemberPriority
{
    List<PartyMember> _queue;

    public MemberPriority()
    {
        _queue = new List<PartyMember>();
    }

    public bool IsReady()
    { return _queue.Count > 0; }

    public void Add(PartyMember member)
    {
        if (_queue.Contains(member))
            _queue.Remove(member);

        _queue.Add(member);
    }

    public PartyMember Get()
    {
        if (!IsReady())
            return null;

        PartyMember member = _queue[0];
        _queue.Remove(member);
        return member;
    }

    public void Flush(PartyMember member)
    {
        if (_queue.Contains(member))
            _queue.Remove(member);
    }
}

public class PartyController : MonoBehaviour {
    public static PartyController Instance { get; private set; }
    void Awake() { Instance = this; }

    [SerializeField] Transform NewGameSpawn;
    [SerializeField] GameObject PartyEntityObject;

    public PartyEntity Entity { get; private set; }
    public Party Party { get { return Entity.Party; } }
    public List<PartyMember> Members { get { return Entity.Party.Members; } }
    public PartyMember ActiveMember { get { return Entity.Party.ActiveMember; } }

    MemberPriority _priorityQueue;

    ControlState _controlState;
    ControlState _previousControlState;

    List<Entity3D> _shortRange = new List<Entity3D>();
    List<Entity3D> _longRange = new List<Entity3D>();
    bool _isInteracting;

    Entity3D _intendedTarget;

    public Vector3 SelectionPosition
    {
        get
        {
            Vector3 pos = Input.mousePosition;
            if (_controlState == ControlState.LookControl)
            {
                // TODO: Make less jank
                pos = new Vector3(368f, 293f + 214f, 0);
            }
            return pos;
        }
    }

    public void NewParty(Party party)
    {
        GameObject obj = Instantiate(PartyEntityObject);
        party.CreateEntity(obj, NewGameSpawn);
        Entity = party.Entity as PartyEntity;
        Entity.Init(party);
        HUD.Instance.InitParty(party);

        _priorityQueue = new MemberPriority();

        _controlState = ControlState.LookControl;
        _previousControlState = ControlState.LookControl;

        TimeManagement.Instance.OnTick += Tick;
    }

    public void LoadParty(Party party)
    {
        GameObject obj = Instantiate(PartyEntityObject);
        party.CreateEntity(obj);
        Entity = party.Entity as PartyEntity;
        Entity.Init(party);
        HUD.Instance.InitParty(party);

        // Maybe save and load this later???
        _controlState = ControlState.LookControl;
        _previousControlState = ControlState.LookControl;

        TimeManagement.Instance.OnTick += Tick;
    }

    public void ReviveParty(Party party)
    {
        GameObject obj = Instantiate(PartyEntityObject);
        party.CreateEntity(obj, NewGameSpawn);
        Entity = party.Entity as PartyEntity;
        Entity.Init(party);
        HUD.Instance.InitParty(party);

        // Maybe save and load this later???
        _controlState = ControlState.LookControl;
        _previousControlState = ControlState.LookControl;
    }

    void Tick(float tick)
    {
        foreach (var member in Members)
        {
            bool readyEvent = member.Vitals.TickCooldown(tick);
            if (readyEvent)
            {
                HUD.Instance.ReadyEvent(member);
                _priorityQueue.Add(member);
            }

            bool statusEvent = member.Status.TickConditions(tick);
            if (statusEvent)
            {
                HUD.Instance.UpdateDisplay();
            }
        }
    }

    void Update()
    {
        if (Entity == null)
            return;

        if (Camera.main == null)
            return;

        _intendedTarget = null;

        foreach (var member in Members)
        {
            bool expressionEvent = member.Vitals.TickExpression(Time.deltaTime);
            if (expressionEvent)
            {
                HUD.Instance.UpdateDisplay();
            }
        }

        if (Input.GetKeyDown(KeyCode.Q) && _controlState != ControlState.MenuLock)
        {
            switch (_controlState)
            {
                case ControlState.LookControl:
                    SetControlState(ControlState.MouseControl);
                    break;
                case ControlState.MouseControl:
                    SetControlState(ControlState.LookControl);
                    break;
            }
        }

        if (!MenuManager.Instance.IsMenuOpen())
        {
            Vector3 point = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (!Entity.GameCamera.rect.Contains(point))
                return;

            Ray ray = Camera.main.ScreenPointToRay(SelectionPosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.tag == "Interactable")
                {
                    _intendedTarget = hit.collider.GetComponent<Entity3D>();
                    if (_intendedTarget != null)
                    {
                        HUD.Instance.SendInfoMessage(_intendedTarget.MouseoverName);
                    }
                }
                else
                {
                    HUD.Instance.SendInfoMessage("");
                }
            }

            if (Input.GetMouseButtonDown(0) && _intendedTarget != null)
            {
                if (_intendedTarget is EnemyEntity)
                {
                    TryAttack();
                }
                else
                {
                    TryInteraction();
                }
            }
            else if (Input.GetKeyDown(KeyCode.E))
            {
                TryInteraction();
            }
            else if (Input.GetKeyDown(KeyCode.F))
            {
                TryAttack();
            }
            else if (Input.GetKeyDown(KeyCode.R))
            {
                MenuManager.Instance.OpenMenu("Rest");
            }
        }
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            HUD.Instance.ToggleSelectedCharacter();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuManager.Instance.CloseAllMenus();
        }
    }

    public void SetControlState(ControlState state, bool isForced = false)
    {
        if (state == ControlState.Previous)
            state = _previousControlState;

        _controlState = state;
        Entity.SetControls(state);

        if (!isForced && (state == ControlState.LookControl || state == ControlState.MouseControl))
            _previousControlState = state;
    }

    public void LongRange(Entity3D entity, bool add)
    {
        if (entity is PartyEntity)
            return;

        if (add && !_longRange.Contains(entity))
            _longRange.Add(entity);
        else if (!add && _longRange.Contains(entity))
            _longRange.Remove(entity);
    }

    public void ShortRange(Entity3D entity, bool add)
    {
        if (entity is PartyEntity)
            return;

        if (add && !_shortRange.Contains(entity))
            _shortRange.Add(entity);
        else if (!add && _shortRange.Contains(entity))
            _shortRange.Remove(entity);
    }

    public void TryAttack()
    {
        if (ActiveMember == null)
            return;

        if (_isInteracting)
            return;

        StartCoroutine(AttackRoutine());
    }

    IEnumerator AttackRoutine()
    {
        PartyMember attacker = null;
        if (ActiveMember.Vitals.IsReady())
            attacker = ActiveMember;
        else if (_priorityQueue.IsReady())
            attacker = _priorityQueue.Get();

        if (attacker != null)
        {
            _isInteracting = true;

            bool shortRange;
            Entity3D target = GetNearestTargetable(out shortRange);
            if (target != null)
            {
                Enemy enemy = target.State as Enemy;

                if(shortRange)
                {
                    int damage;
                    bool hit = attacker.TryHit(enemy, out damage);
                    if (hit)
                    {
                        enemy.OnHit(damage);
                    }
                }
                else
                {
                    Projectile projectile;
                    _isInteracting = true;
                    attacker.TryShoot(target as EnemyEntity, out projectile);

                    if(projectile != null)
                    {
                        projectile.Setup(((target.transform.position + Vector3.up ) - Party.Entity.transform.position).normalized, true);
                        DropController.Instance.SpawnProjectile(Party.Entity.transform, projectile);
                    }
                }

            }
            else
            {
                Projectile projectile;
                _isInteracting = true;
                attacker.TryShoot(null, out projectile);

                if (projectile != null)
                {
                    projectile.Setup(Party.Entity.transform.forward.normalized, true);
                    DropController.Instance.SpawnProjectile(Party.Entity.transform, projectile);
                }
            }

            yield return null;
            _isInteracting = false;

            _priorityQueue.Flush(attacker);
        }

        if(ActiveMember == attacker)
            Party.SetActiveMember(_priorityQueue.Get());
        HUD.Instance.UpdateDisplay();
    }

    Entity3D GetNearestTargetable(out bool shortRange)
    {
        if (_intendedTarget != null && _intendedTarget.IsTargetable)
        {
            if (_shortRange.Contains(_intendedTarget))
            {
                shortRange = true;
                return _intendedTarget;
            }
            else if (_longRange.Contains(_intendedTarget))
            {
                shortRange = false;
                return _intendedTarget;
            }
        }
        shortRange = true;
        Entity3D target = GetTarget(_shortRange);
        if (target == null)
        {
            shortRange = false;
            target = GetTarget(_longRange);
        }
        return target;
    }

    Entity3D GetTarget(List<Entity3D> targets)
    {
        float distance = 100000;
        Entity3D target = null;
        foreach (var entity in targets)
        {
            if (!entity.IsTargetable)
                continue;

            EnemyEntity enemy = entity as EnemyEntity;
            if (enemy != null)
            {
                Plane[] planes = GeometryUtility.CalculateFrustumPlanes(Entity.Camera);

                bool isVisible = false;

                Renderer renderer = entity.GetComponentInChildren<Renderer>();
                if (renderer != null)
                    isVisible = GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
                else
                {
                    Collider collider = entity.GetComponent<Collider>();
                    if (collider != null)
                        isVisible = GeometryUtility.TestPlanesAABB(planes, collider.bounds);
                }

                if (!isVisible)
                    continue;

                float dist = Vector3.Distance(transform.position, entity.transform.position);
                if (dist < distance)
                {
                    distance = dist;
                    target = entity;
                }
            }
        }
        return target as EnemyEntity;
    }

    public void TryInteraction()
    {
        if (_isInteracting)
            return;

        if (_shortRange.Count == 0)
            return;

        StartCoroutine(InteractionRoutine());
    }

    IEnumerator InteractionRoutine()
    {
        Entity3D target = GetNearestInteractable();
        if (target != null)
        {
            _isInteracting = true;

            yield return target.Interact(Entity);
            _isInteracting = false;
        }
    }

    Entity3D GetNearestInteractable()
    {
        if (_shortRange.Count == 0)
            return null;

        float distance = 100000;
        Entity3D target = null;
        foreach (var entity in _shortRange)
        {
            if (entity.IsTargetable)
                continue;

            Plane[] planes= GeometryUtility.CalculateFrustumPlanes(Entity.Camera);

            bool isVisible = false;

            Renderer renderer = entity.GetComponentInChildren<Renderer>();
            if (renderer != null)
                isVisible = GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
            else
            {
                Collider collider = entity.GetComponent<Collider>();
                if (collider != null)
                    isVisible = GeometryUtility.TestPlanesAABB(planes, collider.bounds);
            }

            if (!isVisible)
                continue;

            float dist = Vector3.Distance(transform.position, entity.transform.position);
            if (dist < distance)
            {
                distance = dist;
                target = entity;
            }
        }
        return target;
    }
}
