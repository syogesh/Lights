using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player S;

    public GameObject water;

    public Rigidbody2D r;

    Walk walk;
    Swim swim;

    public int maxHealth;
    public int regenRate;
    public float regenDelay;

    public bool __________________;

	public int direction = 1;

    public int health;
    private int regenStartHealth;
    private float lastDamage;
    private float regenStart;

    public float HealthPercentage
    {
        get
        {
            return health / (float) maxHealth;
        }
    }

    void Awake()
    {
        S = this;
        r = gameObject.GetComponent<Rigidbody2D>();

        OnReset();
    }

    // Use this for initialization
    void Start()
    {
        walk = GetComponent<Walk>();
        swim = GetComponent<Swim>();

        walk.enabled = true;
        swim.Enable(false);

        Events.Register<OnDeathEvent>(OnReset);
        Events.Register<OnPauseEvent>(OnPause);
    }

    void FixedUpdate()
    {
        float time = Time.time;
        if (time >= lastDamage + regenDelay)
        {
            if (regenStart == 0)
            {
                regenStart = time;
                regenStartHealth = health;
            }
            health = Mathf.Min(maxHealth, regenStartHealth + (int)((time - regenStart) * regenRate));
        }
    }

    public void takeDamage(int damage)
    {
        lastDamage = Time.time;
        regenStart = 0;
        if (MainCam.S.invincible) return;
		MainCam.ShakeForSeconds(0.5f);
        health = Mathf.Max(0, health - damage);
        if (health <= 0)
        {
            Events.Broadcast(new OnDeathEvent());
        }
		print ("player hp: " + health);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.tag == "water")
        {
            walk.enabled = false;
            swim.Enable(true);

            water = collider.gameObject;
        }
    }

    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.tag == "water")
        {
            walk.enabled = true;
            swim.Enable(false);

            if (water == collider.gameObject) water = null;
        }
    }

    void OnPause(OnPauseEvent e) {
        if (e.paused) {
            walk.enabled = false;
            swim.Enable(false);

            Pauser.Pause(this);
        }
        else {
            walk.enabled = (water == null);
            swim.Enable(water != null);

            Pauser.Resume(this);
        }
    }

    void OnReset()
    {
        health = maxHealth;
        lastDamage = 0;
        regenStart = 0;
        regenStartHealth = health;
    }
}











