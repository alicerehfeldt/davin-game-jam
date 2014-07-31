﻿using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enemy generic behavior
/// </summary>
public class EnemyScript : GameChild
{
	private WeaponScript[] weapons;
	private bool hasSpawn = false;
	private MoveScript moveScript;
	
	void Awake()
	{
		// Retrieve the weapon only once
		weapons = GetComponentsInChildren<WeaponScript>();
		moveScript = (MoveScript) GetComponent(typeof(MoveScript));
	}

	public override void Start()
	{
		base.Start ();
		// Disable everything
		// -- collider
		collider2D.enabled = false;
		// -- Shooting
		foreach (WeaponScript weapon in weapons)
		{
			weapon.enabled = false;
		}
	}
	
	void Update()
	{
		if (hasSpawn == false)
		{
			if (renderer.IsVisibleFrom(this.playerCamera))
			{
				Spawn();
			}
		}
		else
		{
		
			if (this.renderer.IsVisibleFrom (this.playerCamera) == false) {
				Destroy (this.gameObject);
			}
			
			foreach (WeaponScript weapon in weapons)
			{
				// Auto-fire
				if (weapon != null && weapon.CanAttack)
				{
					weapon.Attack(true);
				}
			}


		}
	}

	void Spawn()
	{
		this.hasSpawn = true;
		collider2D.enabled = true;
		foreach (WeaponScript weapon in weapons)
		{
			weapon.enabled = true;
		}
	}
	
	public void Moving(bool moving)
	{
		this.moveScript.enabled = moving;
	}
}