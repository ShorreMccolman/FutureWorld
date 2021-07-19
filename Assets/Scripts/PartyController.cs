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

public class PartyController : MonoBehaviour {
    public static PartyController Instance { get; private set; }
    private void Awake() { Instance = this; }

    [SerializeField] Transform NewGameSpawn;
    [SerializeField] GameObject PartyEntityObject;
    [SerializeField] GameObject BloodParticle;

    public PartyEntity Entity { get; private set; }

    public ControlState ControlState => _controlState;
    ControlState _controlState;
    ControlState _previousControlState;

    List<Entity3D> _shortRange = new List<Entity3D>();
    List<Entity3D> _longRange = new List<Entity3D>();

    bool _isInteracting;
    bool _showingPopup;
    Entity3D _intendedTarget;
    Party _party;
    IPopable _currentPop;

    public delegate void InputEvent();
    public InputEvent OnPressClick;
    public InputEvent OnReleaseClick;

    public static event System.Action OnNearbyEnemiesChanged;

    public void NewParty(Party party)
    {
        GameObject obj = Instantiate(PartyEntityObject);
        party.CreateEntity(obj, NewGameSpawn);
        _party = party;
        Entity = party.Entity as PartyEntity;
        Entity.Init(party);
        HUD.Instance.InitParty(party);

        _controlState = ControlState.LookControl;
        _previousControlState = ControlState.LookControl;

        MenuManager.OnMenuOpened += MenuOpened;
        MenuManager.OnMenusClosed += MenusClosed;
        Entity3D.OnEntityDestroyed += RemoveFromRange;
        PlayerSphere.OnEntityEnteredSphere += EntityEnteredSphere;
        PlayerSphere.OnEntityExitedSphere += EntityExitedSphere;
    }

    public void LoadParty(Party party)
    {
        GameObject obj = Instantiate(PartyEntityObject);
        party.CreateEntity(obj);
        _party = party;
        Entity = party.Entity as PartyEntity;
        Entity.Init(party);
        HUD.Instance.InitParty(party);

        _controlState = ControlState.LookControl;
        _previousControlState = ControlState.LookControl;

        MenuManager.OnMenuOpened += MenuOpened;
        MenuManager.OnMenusClosed += MenusClosed;
        Entity3D.OnEntityDestroyed += RemoveFromRange;
        PlayerSphere.OnEntityEnteredSphere += EntityEnteredSphere;
        PlayerSphere.OnEntityExitedSphere += EntityExitedSphere;
    }

    public void ReviveParty(Party party)
    {
        GameObject obj = Instantiate(PartyEntityObject);
        party.CreateEntity(obj, NewGameSpawn);
        Entity = party.Entity as PartyEntity;
        Entity.Init(party);
        HUD.Instance.InitParty(party);

        _controlState = ControlState.LookControl;
        _previousControlState = ControlState.LookControl;
    }

    void Update()
    {
        if (Entity == null)
            return;

        foreach (var member in Party.Instance.Members)
        {
            member.Vitals.TickExpression(Time.deltaTime);
        }

        if(Input.GetMouseButtonDown(0) || Input.GetMouseButtonDown(1))
        {
            OnPressClick?.Invoke();
        }
        else if(Input.GetMouseButtonUp(0) || Input.GetMouseButtonUp(1))
        {
            OnReleaseClick?.Invoke();
        }

        bool hoveringUI = false;

        string message = "";
        PointerEventData pointerData = new PointerEventData(EventSystem.current)
        {
            position = Input.mousePosition
        };

        IPopable popable = null;
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, results);

        results.ForEach((result) => {
            IInfoMessenger messenger = result.gameObject.GetComponent<IInfoMessenger>();
            if (messenger != null)
            {
                hoveringUI = true;
                message = messenger.GetInfoMessage();
            }

            TryFindPopable(ref popable, result.gameObject);
        });

        _intendedTarget = null;
        if (_controlState != ControlState.MenuLock)
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

                    TryFindPopable(ref popable, _intendedTarget.gameObject);

                    if (_intendedTarget != null && message == "")
                    {
                        message = _intendedTarget.MouseoverName;
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
                else if (Input.GetKeyDown(KeyCode.C))
                {
                    TryCast();
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
                    TurnController.Instance.ToggleCombatMode();
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _party.ToggleSelectedCharacter();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            HUD.Instance.OpenRest();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            HUD.Instance.OpenSpells();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            HUD.Instance.OpenQuests();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (MenuManager.Instance.IsMenuOpen())
                MenuManager.Instance.CloseAllMenus();
            else
                HUD.Instance.OpenSystem();
        }

        InfoMessageReceiver.Send(message);

        if (popable != null)
        {
            if (popable != _currentPop)
            {
                popable.ShowPopup();
                _showingPopup = true;
                _currentPop = popable;
            }
        }
        else
        {
            if (_showingPopup)
            {
                Popups.Close();
                _showingPopup = false;
                _currentPop = null;
            }
        }
    }

    bool TryFindPopable(ref IPopable popable, GameObject obj)
    {
        if (popable == null && Input.GetMouseButton(1))
        {
            IPopable possiblePopup = obj.GetComponent<IPopable>();
            if (possiblePopup != null)
            {
                popable = possiblePopup;
                return true;
            }
        }
        return false;
    }

    public void MenuOpened(bool useVignette, bool hideSide)
    {
        SetControlState(ControlState.MenuLock);
    }

    public void MenusClosed()
    {
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

    public void EntityEnteredSphere(SphereLevel level, Entity3D entity)
    {
        if(level == SphereLevel.Zero && !_shortRange.Contains(entity))
        {
            _shortRange.Add(entity);
            OnNearbyEnemiesChanged?.Invoke();
        }
        else if (level == SphereLevel.Two && !_longRange.Contains(entity))
        {
            _longRange.Add(entity);
            OnNearbyEnemiesChanged?.Invoke();
        }
    }

    public void EntityExitedSphere(SphereLevel level, Entity3D entity)
    {
        if (level == SphereLevel.Zero && _shortRange.Contains(entity))
        {
            _shortRange.Remove(entity);
            OnNearbyEnemiesChanged?.Invoke();
        }
        else if (level == SphereLevel.Two && _longRange.Contains(entity))
        {
            _longRange.Remove(entity);
            OnNearbyEnemiesChanged?.Invoke();
        }
    }

    public void RemoveFromRange(Entity3D entity)
    {
        if (_shortRange.Contains(entity))
            _shortRange.Remove(entity);
        if (_longRange.Contains(entity))
            _longRange.Remove(entity);

        OnNearbyEnemiesChanged?.Invoke();
    }

    public void TryCast()
    {
        if (_isInteracting)
            return;

        PartyMember attacker = _party.GetAttacker();

        if (attacker != null)
        {
            if(string.IsNullOrEmpty(attacker.Profile.QuickSpell))
            {
                TryAttack();
                return;
            }

            bool success = attacker.TryCastQuickSpell();
            if(!success)
            {
                TryAttack();
                return;
            }
        }
    }

    public void TryAttack()
    {
        if (_isInteracting)
            return;

        PartyMember attacker = _party.GetAttacker();

        if (attacker != null)
        {
            bool shortRange;
            Entity3D target = GetNearestTargetable(out shortRange);
            if (target != null)
            {
                Enemy enemy = target.State as Enemy;

                if (shortRange)
                {
                    int damage;
                    bool hit = attacker.TryHit(enemy, out damage);
                    if (hit)
                    {
                        Vector3 pos = Entity.transform.position;
                        Vector3 dir = enemy.Entity.transform.position + Vector3.up * Random.Range(1.0f, 1.5f) - pos;
                        Ray ray = new Ray(pos, dir);
                        RaycastHit rayhit;
                        if (Physics.Raycast(ray, out rayhit))
                        {
                            // TODO create some kind of pooling system
                            GameObject part = Instantiate(BloodParticle, rayhit.point, Quaternion.LookRotation(-dir));
                        }

                        enemy.OnHit(damage);
                    }
                }
                else
                {
                    Vector3 lookdir = (target.transform.position + Vector3.up - Entity.transform.position).normalized;
                    attacker.TryShoot(target, Entity.transform.position, lookdir, Entity.MoveSpeed);
                }

            }
            else
            {
                attacker.TryShoot(target, Entity.transform.position, Entity.transform.forward.normalized, Entity.MoveSpeed);
            }
        }
    }

    public bool AreEnemiesInArea()
    {
        foreach (var ent in _longRange)
        {
            EnemyEntity enemy = ent as EnemyEntity;
            if (enemy != null)
            {
                if (enemy.Enemy.IsHostile && enemy.Enemy.IsConcious())
                {
                    return true;
                }
            }
        }
        return false;
    }

    public bool AreEnemiesInRange()
    {
        foreach (var ent in _shortRange)
        {
            EnemyEntity enemy = ent as EnemyEntity;
            if (enemy != null)
            {
                if (enemy.Enemy.IsHostile && enemy.Enemy.IsConcious())
                {
                    return true;
                }
            }
        }
        return false;
    }

    public List<Enemy> GetActiveEnemies()
    {
        List<Enemy> enemies = new List<Enemy>();
        foreach (Entity3D ent in _longRange)
        {
            EnemyEntity entity = ent as EnemyEntity;
            if (entity != null)
            {
                if(entity.Enemy.IsConcious() && entity.Enemy.IsHostile)
                    enemies.Add(entity.Enemy);
            }
        }
        return enemies;
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
