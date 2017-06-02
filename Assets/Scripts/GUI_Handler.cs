using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;
using System.IO;

public class GUI_Handler : MonoBehaviour {

	public GameObject menu; //Assign canvas to this in inspector, make sure script is on EventHandler
	public KeyCode hideShow; //Set to the key you want to press to open and close the GUI
	public string FileName; //Title the file you want to export data to!  Will be saved in resources.

	private bool isShowing;
	public InputField raName, partic,exp,age,height,other; 
	//TODO: Ask around to see if there's a more efficient and elegant method to get references to all of the input fields!
	public Toggle left, right;
	public Dropdown sex;
	public Text sexLabel;
	public bool isVisible {
		get {
			return menu.activeSelf;
		}
	}

	// Use this for initialization
	void Start () {
		if (FileName.Equals (""))
			FileName = "default";
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (hideShow)) {
			isShowing = !isShowing;
			menu.SetActive (isShowing);
		}
	}

	public void exportData(List<string> data) {
		string path = @"Assets\Data\" + FileName + ".txt";
		DateTime now = DateTime.Now;
		string theTime = DateTime.Now.ToString ("hh:mm:ss");
		string theDate = DateTime.Now.ToString ("d");
		if (!File.Exists(path)) {
			string header = "Detected\tGain\tCorrect Count\tStep Size\tSubject Number\tHeight\tAge\tSex\tRA Name\tOther\r\n";
			File.WriteAllText (path, header);
		}

		string hand = "";
		if (left.isOn)
			hand = hand + "L";
		if (right.isOn)
			hand = hand + "R";

		string appendText = string.Format("{0}\t{1}\r\n", string.Join("\t", data.ToArray()), string.Join("\t", new string[] {partic.text, height.text, age.text, sexLabel.text, raName.text, other.text}));
		
		File.AppendAllText (path, appendText);

	}

/*
	public double getRotationTime() {
		return rotationTime;
	}
*/
}
