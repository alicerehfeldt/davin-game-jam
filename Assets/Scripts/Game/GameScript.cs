﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class represents a "game", belonging to one of the two players
// It is a child of the MasterGameScript which controls everything
public class GameScript : MonoBehaviour {

	public bool isPlayer2 = false;
	public GUISkin guiSkin;
	public Transform foreground;
	public Texture[] threatTextures = new Texture[11];
	public Texture packetTexture;
	public Texture vectorTexture;
	
	private Camera playerCamera;
	private MasterGameScript masterGame;
	private LevelScript level;
	private RevengeSpawner revengeSpawner;

	private GameScript otherPlayer;
	private List<Wave> waves;


	public int incomingCount = 0;
	public int threatCount = 0;

	private float threatCooldown = 0;

	
	void Awake() {
		masterGame = GetComponentInParent<MasterGameScript>();
		foreground = transform.Find ("Camera/Foreground");
		playerCamera = GetComponentInChildren<Camera>();
		revengeSpawner = GetComponentInChildren<RevengeSpawner>();
		level = GetComponentInChildren<LevelScript>();
		waves = new List<Wave>();
	}

	// Use this for initialization
	void Start () {
		otherPlayer = masterGame.GiveMeTheOtherPlayer(this);
		foreach (Wave wave in masterGame.waves) {
			Wave myWave = (Wave) wave.Clone ();
			waves.Add (myWave);
		}
	}
	
	
	void FixedUpdate() {
		// Spawn the PVE waves
		foreach(Wave wave in waves) {
			if (wave.deployed) {
				continue;
			}
			
			if (level.percent >= wave.percent) {
				wave.deployed = true;
				GameObject waveObject = (GameObject) Instantiate(Resources.Load("Prefabs/Waves/"+wave.type));
				if (object.ReferenceEquals(null, waveObject)) {
					Debug.LogError ("Could not load wave type: "+wave.type);
				}
				waveObject.transform.parent = foreground;

				if (wave.type == "RandomSimpleWave") {
					RandomSimpleWave rando = waveObject.GetComponent<RandomSimpleWave>();
					rando.seed = wave.seed;
				}
				// Put stuff just above the camera
				float positionY = (playerCamera.orthographicSize) + 1;
				waveObject.transform.localPosition = new Vector2(0, positionY);
				Debug.Log ("Sending wave "+wave.type);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnGUI() {
		GUI.skin = guiSkin;
		
		int positionY = Mathf.CeilToInt (5);
		
		// Incoming Count
		string incomingText = incomingCount.ToString();
		if(incomingCount > 99) {
			incomingText = "99";
		}
	
		int positionX = Mathf.CeilToInt((Screen.width / 4) - 120);
		if (isPlayer2) {
			positionX = Mathf.CeilToInt((Screen.width / 4)*3 - 85);
			GUI.Label (new Rect(positionX, positionY,200,30), incomingText, "Incoming2");
		}	
		else {
			GUI.Label (new Rect(positionX, positionY,200,30), incomingText, "Incoming");
		}
		
		
		// Threat Meter
		positionX = Mathf.CeilToInt((Screen.width / 4) + 50);
		if (isPlayer2) {
			positionX = Mathf.CeilToInt((Screen.width / 4)*3 - 250);
		}
		
		int threatTextureIndex = 0;


		if (Time.time < threatCooldown) {
			GUI.Label (new Rect(positionX, positionY, 200, 50), "BUFFER OVERFLOW", "Buffer Overflow");
			
		} else {
			float threat = threatCount;
			// Always round up to nearest ten
			if (threat < 90) {
				threatTextureIndex = Mathf.CeilToInt(threat/10);
			} else {
				// But round down to 90 to only show full if we hit 100
				threatTextureIndex = Mathf.FloorToInt (threat/10);
			}
			GUI.Label (new Rect(positionX, positionY,200,50), threatTextures[threatTextureIndex]);
		}



		// Vector Label
		positionX = Mathf.CeilToInt((Screen.width / 4) - 60);
		if (isPlayer2) {
			positionX = Mathf.CeilToInt((Screen.width / 4)*3 - 50);
		}		
		GUI.Label (new Rect(positionX, positionY,200,50), vectorTexture);	

		// Incoming Label
		positionX = Mathf.CeilToInt((Screen.width / 4) - 225);
		if (isPlayer2) {
			positionX = Mathf.CeilToInt((Screen.width / 4)*3 + 120);
		}		
		GUI.Label (new Rect(positionX, positionY,200,50), packetTexture);			
		
		
		if (!masterGame.gameOn) {
			bool isWinner = (masterGame.winner == (isPlayer2 ? 2 : 1));
			string gameOverText = isWinner ? "VICTORIOUS" : "DELETED";
			string gameOverStyle = isWinner ? "End Winner" : "End Loser";
			
			positionY = Mathf.CeilToInt((Screen.height / 2) - 20);
			positionX = Mathf.CeilToInt((Screen.width / 4) - 120);
			if (isPlayer2) {
				positionX = Mathf.CeilToInt((Screen.width / 4)*3 - 80);
			}
			GUI.Label (new Rect(positionX, positionY,200,30), gameOverText, gameOverStyle);		
		}
	}

	public void addThreat(int howMuch) {
		threatCount += howMuch;
		if ( threatCount > 100) {
			threatCount = 100;
		}
	}
	
	// Send the other player some threat
	public void dumpThreat() {
		if(threatCount <=0 || Time.time < threatCooldown) {
			return;
		}
		otherPlayer.getDunked (threatCount);
		threatCount = 0;

		// When the buffer overflow goes away and we can dump again
		threatCooldown = Time.time + 3f;
	}
	
	// Other player just sent us some threat
	public void getDunked(int enemyThreat) {
		revengeSpawner.GenerateEnemiesBasedOnThreat(enemyThreat);
	}


}

public class Wave
{
	public string type;
	public double percent;
	public bool deployed = false;
	public int seed = 0;
	
	public Wave(){
	}
	
	public Wave(double p, string t) {
		percent = p;
		type = t;
	}
	public Wave(double p, string t, int s) {
		percent = p;
		type = t;
		seed = s;
	}
	protected Wave(Wave other){
		type = other.type;
		percent = other.percent;
		deployed = other.deployed;
		seed = other.seed;
	}
	public virtual object Clone()
	{
		return new Wave(this);
	}
}
