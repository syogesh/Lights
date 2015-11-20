﻿using UnityEngine;

public class Dash : MonoBehaviour, Rechargeable {

	private float maxDashDistance = 3f;
	private bool teleportKeyDown = false;

    private int maxCharges = 2;
	private int currentCharges;
	private float cooldown = 2f;
    private float rechargeTime = 0f;
    private float lastCharge;

	private Rigidbody2D r;

    public int MaxCharges
    {
        get { return maxCharges; }
    }

    public int Charges
    {
        get { return currentCharges; }
    }

    public float ChargePercentage
    {
        get { return currentCharges < maxCharges ? rechargeTime / cooldown : 1f; }
    }

	void Awake() {
		currentCharges = maxCharges;
        lastCharge = -cooldown;
		r = GetComponent<Rigidbody2D>();
	}

	void Update() {
		teleportKeyDown = Input.GetKeyDown(KeyCode.J);
	}

	// Update is called once per frame
	void FixedUpdate() {
		UpdateCharges();

		if (CanDash() && teleportKeyDown) {
            // lastCharge needs to be updated if not currently charging
            if (currentCharges == maxCharges)
            {
                lastCharge = Time.time;
            }
			currentCharges -= 1;
			teleportKeyDown = false;

			var dashVector = GetDashVector();
			var velocity = r.velocity;

			velocity.x = dashVector.x;
			velocity.y = dashVector.y;

			r.velocity = velocity;

			gameObject.transform.position += dashVector;
		}
	}

	private void UpdateCharges() {
		if (currentCharges < maxCharges) {
            rechargeTime = Mathf.Min(Time.time - lastCharge, cooldown);
			if (rechargeTime >= cooldown) {
				++currentCharges;
				lastCharge = Time.time;
			}
		}
	}

	private bool CanDash() {
		return currentCharges > 0;
	}

	private Vector3 GetDashVector() {
		var dashDirection = GetDashDirection();
		var dashDistance = GetDashDistance(dashDirection);

		return dashDirection * dashDistance;
	}

	private Vector3 GetDashDirection() {
		var direction = Vector3.zero;

		if (Input.GetKey(KeyCode.W)) direction.y += 1;
		if (Input.GetKey(KeyCode.S)) direction.y -= 1;
		if (Input.GetKey(KeyCode.A)) direction.x -= 1;
		if (Input.GetKey(KeyCode.D)) direction.x += 1;

		return direction;
	}

	private float GetDashDistance(Vector3 direction) {
		// Find all the walls in the dash's path.
		var start = gameObject.transform.position;
		var mask = (1 << LayerMask.NameToLayer("Terrain"));

		var hits = Physics2D.RaycastAll(start, direction, maxDashDistance, mask);

		// Return the max dash distance if there are no walls.
		if (hits.Length == 0) return maxDashDistance;

		// Raycast backwards to find the end point of the wall.
		var lastHit = hits[hits.Length - 1];
		var end = start + direction * maxDashDistance;

		var reverseHit = Physics2D.Raycast(end, -1 * direction, maxDashDistance, mask);
		var reverseHitPoint = reverseHit.point;

		if (Vector3.Distance(start, reverseHitPoint) < maxDashDistance) {
			// The reverse hitpoint is within the dash's range. Use the max dash distance for the dash.
			return maxDashDistance;
		}
		else {
			// The reverse hitpoint is within the dash's range. Teleport to the last wall's hit point.
			return lastHit.distance;
		}
	}
}
