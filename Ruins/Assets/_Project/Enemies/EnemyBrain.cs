using UnityEngine;
using UnityEngine.AI;

// «Мозг» врага из главы 24, доработанный для «Руин»: удар — анимацией,
// жетоны атаки, ожидание очереди на дистанции, боевая музыка
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBrain : MonoBehaviour
{
    private static readonly int AttackId = Animator.StringToHash("Attack");

    [SerializeField] private Transform[] waypoints;
    [SerializeField] private Vision vision;
    [SerializeField] private Health playerHealth;
    [SerializeField] private Animator animator;
    [SerializeField] private AttackTokens tokens;
    [SerializeField] private float attackRange = 1.8f;
    [SerializeField] private float holdDistance = 3.5f;
    [SerializeField] private float attackCooldown = 1.8f;
    [SerializeField] private float searchDuration = 5f;
    [SerializeField] private float patrolSpeed = 1.5f, chaseSpeed = 3.2f;

    private NavMeshAgent agent;
    private Transform player;
    private StateMachine fsm;
    private IState patrol;
    private Vector3 lastSeen, home;
    private float searchTimer, attackTimer;
    private int waypoint;
    private bool seesPlayer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        player = playerHealth.transform;
        home = transform.position;

        patrol = new FuncState(
            enter: () => { Disengage(); agent.speed = patrolSpeed;
                           GoToWaypoint(); },
            tick: _ => { if (Arrived()) NextWaypoint(); });

        var chase = new FuncState(
            enter: () => { Engage(); agent.speed = chaseSpeed; },
            tick: _ => ChaseTick());

        var attack = new FuncState(
            enter: () => { agent.ResetPath(); attackTimer = 0.3f; },
            tick: dt => AttackTick(dt),
            exit: () => tokens.Release(this));

        var search = new FuncState(
            enter: () => { Disengage(); searchTimer = searchDuration;
                           agent.SetDestination(lastSeen); },
            tick: dt => searchTimer -= dt);

        var returnHome = new FuncState(
            enter: () => { agent.speed = patrolSpeed; GoToWaypoint(); });

        fsm = new StateMachine();
        fsm.AddTransition(patrol, chase, () => seesPlayer);
        fsm.AddTransition(chase, attack, () => seesPlayer
            && InRange(attackRange) && tokens.TryAcquire(this));
        fsm.AddTransition(attack, chase,
            () => !InRange(attackRange + 0.6f) || !playerHealth.IsAlive);
        fsm.AddTransition(chase, search, () => !seesPlayer && Arrived());
        fsm.AddTransition(search, chase, () => seesPlayer);
        fsm.AddTransition(search, returnHome, () => searchTimer <= 0f);
        fsm.AddTransition(returnHome, chase, () => seesPlayer);
        fsm.AddTransition(returnHome, patrol, Arrived);
    }

    void Start()
    {
        // В сборке агент может появиться раньше, чем NavMeshSurface
        // загрузит сетку (она делает это в OnEnable), — ставим его заново
        if (!agent.isOnNavMesh) agent.Warp(transform.position);
        fsm.SetState(patrol);
    }

    void Update()
    {
        seesPlayer = playerHealth.IsAlive && vision.CanSee(player);
        if (seesPlayer) lastSeen = player.position;
        fsm.Tick(Time.deltaTime);
    }

    void OnDisable()
    {
        tokens.Release(this);
        Disengage();
    }

    private void ChaseTick()
    {
        // Пока жетон у других — держим дистанцию и ждём очереди
        bool waiting = InRange(holdDistance) && !tokens.TryAcquire(this);
        if (waiting) { agent.ResetPath(); FacePlayer(Time.deltaTime); }
        else agent.SetDestination(lastSeen);
    }

    private void AttackTick(float dt)
    {
        FacePlayer(dt);
        attackTimer -= dt;
        if (attackTimer > 0f) return;
        attackTimer = attackCooldown;
        animator.SetTrigger(AttackId);   // урон — событиями анимации
    }

    private void FacePlayer(float dt)
    {
        Vector3 look = player.position - transform.position;
        look.y = 0f;
        if (look.sqrMagnitude < 0.01f) return;
        transform.rotation = Quaternion.Slerp(transform.rotation,
            Quaternion.LookRotation(look), 10f * dt);
    }

    private void Engage() => CombatMusic.Engaged.Add(this);
    private void Disengage() => CombatMusic.Engaged.Remove(this);

    private bool InRange(float range) =>
        (player.position - transform.position).sqrMagnitude <= range * range;

    private bool Arrived() =>
        !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance;

    private void GoToWaypoint() => agent.SetDestination(
        waypoints.Length > 0 ? waypoints[waypoint].position : home);

    private void NextWaypoint()
    {
        if (waypoints.Length == 0) return;
        waypoint = (waypoint + 1) % waypoints.Length;
        GoToWaypoint();
    }
}
