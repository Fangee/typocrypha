using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenNew : MonoBehaviour 
{	
	// References to necessary objects
	public GameObject keyboardTest;
	public GameObject titleScreen;
	
	// References to necessary scripts
	public TextEvents textEvents;
	
	// Screen fade options
	private string[] options = new string[5];
	
	void Start() 
	{
		// Set options for screen fades
		options[0] = "in"; // in vs out
		options[1] = "1"; // length of fade in seconds
		options[2] = "0"; // rgb values, (0, 0, 0) = black
		options[3] = "0";
		options[4] = "0"; 
		
		StartCoroutine(TitleScreenTransition());
	}
	
	void Update() 
	{
		
	}
	
	// Continue transitioning from the keyboard test screen
	private IEnumerator TitleScreenTransition()
	{
		// Hide the keyboard test and display the title screen
		keyboardTest.SetActive(false);
		titleScreen.SetActive(true);
		
		// Fade in
		yield return new WaitForSeconds(0.5f);
		StartCoroutine(textEvents.fade(options));
		yield return new WaitForSeconds(1.0f);
	}
}

// public List<MenuItem> menuItems = new List<MenuItem>();
// MenuItem Class:
// text box property
// or maybe string property and just create the text box in script might be less painful
// OnClick()
// or just make unity events somehow
// or even easier just string[] menuOptions why did i want to make a class
// selectedOption string + functions
// if enter pressed && selectedOption == blahblah do that menu item function
// just do a switch case you idiot