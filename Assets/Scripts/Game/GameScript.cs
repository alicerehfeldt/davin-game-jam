﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

// This class represents a "game", belonging to one of the two players
// It is a child of the MasterGameScript which controls everything
public class GameScript : MonoBehaviour {

	public bool isPlayer2 = false;
	public GUISkin guiSkin;
	public Transform foreground;
	
	private Camera camera;
	private MasterGameScript masterGame;
	private LevelScript level;

	private GameScript otherPlayer;
	
	private List<Wave> waves;


	public int incomingCount = 0;
	
	void Awake() {
		masterGame = GetComponentInParent<MasterGameScript>();
		foreground = transform.Find ("Camera/Foreground");
		level = GetComponentInChildren<LevelScript>();
		camera = GetComponentInChildren<Camera>();
		waves = new List<Wave>();
	}

	// Use this for initialization
	void Start () {
		otherPlayer = masterGame.GiveMeTheOtherPlayer(this);
		Debug.Log("Me: "+isPlayer2+ " / Them: " + otherPlayer.isPlayer2);
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
				// Put stuff just above the camera
				float positionY = (this.camera.orthographicSize) + 1;
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
		// Update incoming
		string incomingText = "Incoming: "+incomingCount;
		int positionY = Mathf.CeilToInt (5);
		int positionX = Mathf.CeilToInt((Screen.width / 4) - 120);
		if (isPlayer2) {
			positionX = Mathf.CeilToInt((Screen.width / 4)*3 - 80);
		}
		GUI.Label (new Rect(positionX, positionY,200,30), incomingText, "Incoming");		
		







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
}

public class Wave
{
	public string type;
	public double percent;
	public bool deployed = false;
	
	public Wave(){
	}
	
	public Wave(double p, string t) {
		percent = p;
		type = t;
	}
	protected Wave(Wave other){
		type = other.type;
		percent = other.percent;
		deployed = other.deployed;
	}
	public virtual object Clone()
	{
		return new Wave(this);
	}
}
