using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Linq;

public enum ControlState
{
    MouseControl,
    LookControl,
    MenuLock,
    Previous
}

public class MemberPriority
{
    List<CombatEntity> _queue;

    public MemberPriority()
    {
        _queue = new List<CombatEntity>();
    }

    public bool IsReady()
    { return _queue.Count > 0; }

    public void Add(CombatEntity member)
    {
        if (_queue.Contains(member))
            _queue.Remove(member);

        int index = 0;
        for(int i=0;i<index;i++)
        {
            if (member.GetCooldown() > _queue[i].GetCooldown())
                index = i;
        }
        _queue.Insert(index, member);
    }

    public void Add(MemberPriority priority)
    {
        foreach(var member in priority._queue)
        {
            Add(member);
        }
    }

    public bool NextCooldown(out float cooldown)
    {
        if (_queue.Count == 0)
        {
            cooldown = 0;
            return false;
        }

        cooldown = _queue[0].GetCooldown();
        return cooldown <= 0;
    }

    public CombatEntity Get()
    {
        if (!IsReady())
            return null;

        CombatEntity member = _queue[0];
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

    [SerializeField] Transform NewGameSpawn;
    [SerializeField] GameObject PartyEntityObject;

    public PartyEntity Entity { get; private set; }

    public ControlState ControlState { get { return _controlState; } }
    Party _party;
    ControlState _controlState;
    ControlState _previousControlState;
    MemberPriority _priorityQueue;
    List<Entity3D> _shortRange = new List<Entity3D>();
    List<Entity3D> _midRange = new List<Entity3D>();
    List<Entity3D> _longRange = new List<Entity3D>();
    bool _isInteracting;
    Entity3D _intendedTarget;

    public void NewParty(Party party)
    {
        Instance = this;

        GameObject obj = Instantiate(PartyEntityObject);
        party.CreateEntity(obj, NewGameSpawn);
        _party = party;
        Entity = party.Entity as PartyEntity;
        Entity.Init(party);
        HUD.Instance.InitParty(party);

        _priorityQueue = new MemberPriority();

        _controlState = ControlState.LookControl;
        _previousControlState = ControlState.LookControl;

        MenuManager.Instance.OnMenuLock += MenuEvent;
        TimeManagement.Instance.OnTick += Tick;
    }

    public void LoadParty(Party party)
    {
        Instance = this;

        GameObject obj = Instantiate(PartyEntityObject);
        party.CreateEntity(obj);
        _party = party;
        Entity = party.Entity as PartyEntity;
        Entity.Init(party);
        HUD.Instance.InitParty(party);

        // Maybe save and load this later???
        _controlState = ControlState.LookControl;
        _previousControlState = ControlState.LookControl;

        MenuManager.Instance.OnMenuLock += MenuEvent;
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
        foreach (var member in Party.Instance.Members)
        {
            if (member.IsConcious())
            {
                bool readyEvent = member.Vitals.TickCooldown(tick);
                if (readyEvent)
                {
                    HUD.Instance.ReadyEvent(member);
                    _priorityQueue.Add(member);
                }
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

        _intendedTarget = null;

        foreach (var member in Party.Instance.Members)
        {
            bool expressionEvent = member.Vitals.TickExpression(Time.deltaTime);
            if (expressionEvent)
            {
                HUD.Instance.UpdateDisplay();
            }
        }

        bool hoveringUI = false;

        string message = "";
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        results.ForEach((result) => {
            IInfoMessenger msg = result.gameObject.GetComponent<IInfoMessenger>();
            if (msg != null)
            {
                hoveringUI = true;
                message = msg.GetInfoMessage();
            }

        });

        if (_controlState != ControlState.MenuLock)
        {
            Vector3 point = Entity.Camera.ScreenToViewportPoint(Input.mousePosition);
            if (point.x <= 1 && point.y >= 0)
            {
                Ray ray = Entity.Camera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider.tag == "Interactable")
                    {
                        _intendedTarget = hit.collider.GetComponent<Entity3D>();
                        if (_intendedTarget == null)
                            _intendedTarget = hit.collider.GetComponentInParent<Entity3D>();

                        if (_intendedTarget != null && message == "")
                        {
                            message = _intendedTarget.MouseoverName;
                        }
                    }
                }
            }

            if (_isInteracting)
                return;

            if (_controlState != ControlState.MenuLock)
            {
                if (Input.GetMouseButtonDown(0) && _intendedTarget != null && !hoveringUI)
                {
                    if (_intendedTarget is NPCEntity)
                    {
                        TryInteraction();
                    }
                    else if (_intendedTarget is EnemyEntity)
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
                    MenuManager.Instance.OpenMenu("Rest", true);
                }
                else if (Input.GetKeyDown(KeyCode.Q))
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
                else if (Input.GetKeyDown(KeyCode.Return))
                {
                    TimeManagement.Instance.ToggleCombatMode();
                }
            }
        }

        HUD.Instance.SendInfoMessage(message);

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            HUD.Instance.ToggleSelectedCharacter();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            MenuManager.Instance.CloseAllMenus();
        }
    }

    public void MenuEvent(bool locked)
    {
        if (locked)
            SetControlState(ControlState.MenuLock);
        else
            SetControlState(ControlState.Previous);
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

    public void MidRange(Entity3D entity, bool add)
    {
        if (entity is PartyEntity)
            return;

        if (add && !_midRange.Contains(entity))
            _midRange.Add(entity);
        else if (!add && _midRange.Contains(entity))
            _midRange.Remove(entity);
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

    public void Knockout(PartyMember member)
    {
        _priorityQueue.Flush(member);
        if (member == _party.ActiveMember)
            HUD.Instance.ToggleSelectedCharacter();
    }

    public void TryAttack()
    {
        if(TimeManagement.IsCombatMode)
        {
            if (_party.ActiveMember == null)
                return;

            if (_isInteracting)
                return;

            StartCoroutine(AttackRoutine());
        } 
        else
        {
            if (_party.ActiveMember == null)
                return;

            if (_isInteracting)
                return;

            StartCoroutine(AttackRoutine());
        }
    }

    public List<Enemy> GetActiveEnemies()
    {
        List<Enemy> enemies = new List<Enemy>();
        foreach (Entity3D ent in _longRange)
        {
            EnemyEntity enemy = ent as EnemyEntity;
            if (enemy != null)
                enemies.Add(enemy.Enemy);
        }
        return enemies;
    }

    IEnumerator AttackRoutine()
    {
        PartyMember attacker = null;
        if (_party.ActiveMember.Vitals.IsReady())
            attacker = _party.ActiveMember;
        else if (_priorityQueue.IsReady())
            attacker = _priorityQueue.Get() as PartyMember;

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
                        projectile.Setup(((target.transform.position + Vector3.up ) - Entity.transform.position).normalized, true);
                        DropController.Instance.SpawnProjectile(Entity.transform, projectile, Entity.GetSpeed());
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
                    projectile.Setup(Entity.transform.forward.normalized, true);
                    DropController.Instance.SpawnProjectile(Entity.transform, projectile, Entity.GetSpeed());
                }
            }

            yield return null;
            _isInteracting = false;

            _priorityQueue.Flush(attacker);
            TimeManagement.Instance.EntityAttack(attacker);
        }

        if(_party.ActiveMember == attacker)
            Party.Instance.SetActiveMember(_priorityQueue.Get() as PartyMember);
        HUD.Instance.UpdateDisplay();
    }

    Entity3D GetNearestTargetable(out bool shortRange)
    {
        Entity3D target;

        if (_longRange.Contains(_intendedTarget) && _intendedTarget.IsTargetable)
        {
            target = _intendedTarget;
        } 
        else
        {
            target = GetTarget(_longRange);
        }

        if (target != null)
            shortRange = Vector3.Distance(target.transform.position, Entity.transform.position) <= 5f;
        else
            shortRange = false;
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
        Entity3D target;

        if (_shortRange.Contains(_intendedTarget) && !_intendedTarget.IsTargetable)
        {
            target = _intendedTarget;
        }
        else
        {
            target = GetNearestInteractable();
        }

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
