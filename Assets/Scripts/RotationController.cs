using System;
using UnityEngine;
using System.Collections.Generic;
using Valve.VR;

[RequireComponent(typeof(AudioSource))]
public class RotationController : MonoBehaviour
{
	private enum Option {
		RoomA = 0,
		RoomB = 1
	};

	public GameObject roomObject;
	public GameObject headset;
	public GUI_Handler gui;
	public float maxTrialTime = 30;
	public KeyCode switchRoom = KeyCode.Z;
	public KeyCode selectA = KeyCode.X;
	public KeyCode selectB = KeyCode.C;
	public UnityEngine.UI.Text roomText;

	public float initialGain;

	/*public Vector3 uprightPosition;
	public float uprightX;
	public float uprightZ;*/
	public bool turnRight = true;

	public float gainProportionMultiplier = 0.6f; // decrement in maxGainProportion from block to block
	public float gainDecrementMultiplier = 0.6f; // change in trial decrement from block to block
	public float gainDecrement = 0.05f; // initial gain decrement

	private float maxGainProportion; // initialize to same value as gain proportion, used for starting new blocks
	private float gainProportion; 
	private int gainSign;

	private int failedBlocks = 0;
	private int maxFailedBlocks = 4;
	private bool started = false;

	private int trials = 0;
	private int correctTrials = 0;
	private int minTrialsToPass = 7;
	private int maxTrials = 10;

	private Option rotatingRoom;
	private Option selectedRoom;

	private AudioSource chimes;
	private bool chimesPlayed = false;
	private float timer = 0;

	public GameObject tracker;
	[HideInInspector]
	public GameObject trackerInitCoordinates;
	private float previousDistance;
	private float currentDistance;

	private System.Random random;

	void Start()
	{
		chimes = this.gameObject.GetComponent<AudioSource>();
		random = new System.Random();
		rotatingRoom = (Option)random.Next(2);
		selectedRoom = Option.RoomA;
		SetRoomText(selectedRoom);

		maxGainProportion = initialGain;
		gainProportion = initialGain;

		// set original position of tracker (to delete)
		/*uprightPosition = tracker.transform.position;
		uprightX = uprightPosition.x;
		uprightZ = uprightPosition.z;
		previousDistance = 0f;*/

		// instantiate tracker coordinate system
		trackerInitCoordinates = Instantiate (Resources.Load ("SubjectCoordinates", typeof(GameObject))) as GameObject;
		trackerInitCoordinates.transform.position = tracker.transform.position;
		trackerInitCoordinates.transform.rotation = tracker.transform.rotation;

		gainSign = 2 * random.Next (2) - 1; // random -1 or 1
	}

	void Update()
	{
		if (!started)
			return;

		if (selectedRoom == rotatingRoom) {

			Vector3 currentPosition = trackerInitCoordinates.transform.InverseTransformPoint(tracker.transform.position);
			currentDistance = currentPosition.z;

			// this code bases lean velocity on distance from initial point (probably will delete)
			/*float xCurrent = tracker.transform.position.x;
			float zCurrent = tracker.transform.position.z;

			// calculate distance from original tracker position (x and z only)
			currentDistance = Mathf.Pow((Mathf.Pow((xCurrent-uprightX), 2f) + Mathf.Pow((zCurrent - uprightZ), 2f)), ((1f/2f)));*/
			float deltaDistance = currentDistance - previousDistance;

			float leanVelocity = deltaDistance / Time.deltaTime; 

			Debug.Log(string.Format("Current Distance: {0} deltaDistance: {1} lean velocity: {2}", currentDistance, deltaDistance, leanVelocity));

			// TODO: find appropriate scalars (use variables) (100 is arbitrary)
			roomObject.transform.RotateAround(tracker.transform.position, Vector3.down, gainSign*gainProportion*leanVelocity*Time.deltaTime);
		}

		previousDistance = currentDistance;

		timer += Time.deltaTime;
		if (timer > maxTrialTime && !chimesPlayed) {
			chimes.Play();
			chimesPlayed = true;
		}

		if (Input.GetKeyDown(switchRoom) && !gui.isVisible) //Change whether its rotating
		{
			selectedRoom = selectedRoom == Option.RoomA ? Option.RoomB : Option.RoomA; // switches the room (A goes to B, B goes to A)
			timer = 0;
			chimesPlayed = false;
			SetRoomText(selectedRoom);
			roomObject.transform.Rotate(Vector3.down * random.Next(360)); // randomly set rotation of room
		}

		if (Input.GetKeyUp(selectA) || Input.GetKeyUp(selectB) && !gui.isVisible) //User answers
		{
			bool? correct = null;
			trials += 1;
			timer = 0f;
			chimesPlayed = false;

			if (Input.GetKeyUp(selectA)) //A is rotating
			{
				correctTrials += rotatingRoom == Option.RoomA ? 1 : 0;
				correct = rotatingRoom == Option.RoomA;

				Debug.Log ("Selected A");
			}
			else if (Input.GetKeyUp(selectB)) //B is rotating
			{
				correctTrials += rotatingRoom == Option.RoomB ? 1 : 0;
				correct = rotatingRoom == Option.RoomB;

				Debug.Log ("Selected B");
			}

			Debug.Log(string.Format("Failed Trials: {0}, Number Answered: {1}, Number Right: {2}, Gain: {3}", failedBlocks, trials, correctTrials, gainProportion * gainSign));
			gui.exportData(new List<string> {correct.ToString(), (gainSign * gainProportion).ToString("G"), correctTrials.ToString(), gainDecrement.ToString("G")});

			gainProportion -= gainDecrement;

			if (trials >= maxTrials)
			{
				if (correctTrials >= minTrialsToPass)
				{
					maxGainProportion *= gainProportionMultiplier;
					gainDecrement *= gainDecrementMultiplier;
				}
				else
				{
					failedBlocks += 1;
				}

				gainProportion = maxGainProportion;
				trials = 0;
				correctTrials = 0;
			}

			rotatingRoom = (Option)random.Next(2);
			selectedRoom = Option.RoomA;
			SetRoomText(selectedRoom);
			gainSign *= -1;
			roomObject.transform.Rotate(Vector3.down * random.Next(360));

			if (failedBlocks >= maxFailedBlocks)
			{
				Application.Quit();
				UnityEditor.EditorApplication.isPlaying = false;
			}
		}
	}

	public void StartExperiment() {
		if (!started) {
			Debug.Log("Started");
			roomObject.transform.Rotate(Vector3.down * random.Next(360));
			roomObject.transform.position = Vector3.zero;
		}
		started = true;
	}

	void SetRoomText(Option o) {
		if (o == Option.RoomA)
			roomText.text = "Room A";
		else if (o == Option.RoomB)
			roomText.text = "Room B";
	}
}
