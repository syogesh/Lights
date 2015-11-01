﻿using UnityEngine;
using System.Collections;

public class Swim : MonoBehaviour {

	public float swimSpeed;
	public float swimGravity;

	private Rigidbody2D rigidBody;
	
	public void Awake() {
		this.rigidBody = this.GetComponent<Rigidbody2D>();
	}
	
	public void Start() {
		this.rigidBody.gravityScale = this.swimGravity;
	}

	public void OnDisable() {
		this.rigidBody.gravityScale = 1f;
	}
	
	public void Update() {
		var velocity = this.rigidBody.velocity;

		// Lateral swimming.
		if (Input.GetKey(KeyCode.A) && Input.GetKey(KeyCode.D) == false) {
			velocity.x = -1f * this.swimSpeed;
		}
		else if (Input.GetKey(KeyCode.D) && Input.GetKey (KeyCode.A) == false) {
			velocity.x = this.swimSpeed;
		}

		// Up/down swimming.
		if (Input.GetKey(KeyCode.W) && Input.GetKey (KeyCode.S) == false) {
			velocity.y = this.swimSpeed;
		}
		else if (Input.GetKey(KeyCode.S) && Input.GetKey (KeyCode.W) == false) {
			velocity.y = -1f * this.swimSpeed - this.swimGravity;
		}

		// TODO: handle collisions.

		this.rigidBody.velocity = velocity;
	}
}
