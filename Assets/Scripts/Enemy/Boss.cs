﻿using UnityEngine;
using LOS;

public enum BossState
{ 
	Waiting,
	Stealing,
	Attacking,
	Dying
}

public class Boss : MonoBehaviour
{
	public static Boss S;

	public BossState state;
	
	public float sightRange;
	public float naviStolenRange;
    public float attackSpeed;
	public GameObject projectile;

	public Vector3 naviStolenPos;

    bool __________________;

    public LOSRadialLight enemyLight;

    float nextAttackTime;

	bool ___________________;

	public bool stageTwo;
	public int health = 2;
	Enemy enemyComponent;

    public void Awake()
    {
		S = this;
		Events.Register<OnTorchGroupLitEvent>((e) => {
			if(e.group == TorchGroup.BossFight) // Trigger Stage Two
			{
				GetComponent<Enemy>().enabled = true;
				Navi.S.stolen = false;
				MainCam.ShakeForSeconds(2f);
				//state = BossState.Dying;
			}
		});

		Events.Register<OnDeathEvent>(() => {
			state = BossState.Waiting;

			Navi.S.stolen = false;
		});
    }

	public void Start()
	{
		state = BossState.Waiting;
		enemyComponent = GetComponent<Enemy>();
	}

    // Update is called once per frame
    void Update()
	{
		if (state == BossState.Waiting) {
			//check if player is in sightrange and move to stealing if true
			if(!stageTwo && Vector2.Distance(Navi.S.transform.position, transform.position) < sightRange)
			{
				print ("Stole navi!");
				state = BossState.Stealing;
				Navi.S.stolen = true;
			}
		}
		else if (state == BossState.Stealing) {
			if(Vector2.Distance(Navi.S.transform.position, transform.position) < naviStolenRange)
			{
				state = BossState.Attacking;
				nextAttackTime = Time.time + attackSpeed;
			}
		}
		else if (state == BossState.Attacking) {
			//check if dead

			if(nextAttackTime < Time.time)
			{
				//spawn enemy
				GameObject proj = Instantiate(projectile);
				proj.transform.position = this.transform.position;
				BossProjectile script = proj.GetComponent<BossProjectile>();
				script.targetPos = Player.S.transform.position + 4*(Player.S.transform.position - transform.position);
				nextAttackTime = Time.time + attackSpeed;
			}
		}
		if (state == BossState.Dying) {
			Navi.S.stolen = false;
			MainCam.ShakeForSeconds(2f);
			Destroy(gameObject);
			Destroy(GameObject.Find("BossDoorExit"));
		}
    }

	public void takeDamage()
	{
		health -=1;
		transform.localScale *= 0.5f;
		enemyLight.radius *= 0.5f;
		enemyComponent.followSpeed *= 2;

		if (health == 0) {
			state = BossState.Dying;
		}

	}
}