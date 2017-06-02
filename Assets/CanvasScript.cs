using UnityEngine;
using System.Collections;

public class CanvasScript : MonoBehaviour {

	public GameObject menu; //Assign canvas to this in inspector, make sure script is on EventHandler
	public string hideShow; //Set to the key you want to press to open and close the GUI
	public string FileName; //Title the file you want to export data to!  Will be saved in resources.
	private bool isShowing;

	// Use this for initialization
	void Start () {
		isShowing = true;
		if(hideShow.Equals(""))
			hideShow = "escape";
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
}
