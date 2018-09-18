using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class KeyboardTest : MonoBehaviour 
{
	// References to text box objects
	public Text version;
	public Text team;
	public Text instructions;
	public Text testPhrase;
	public InputField inputField;
	public Text inputText;
	
	// References to necessary game objects
	public GameObject titleScreenObj;
	public GameObject dimmer;
	
	// References to necessary scripts
	public TitleScreenNew titleScreen;
	public TextScroll textScroll;
	public TextEvents textEvents;
	
	// Reference to Audio Player
	public AudioPlayer audioPlayer;
	
	// Text to be placed in scrolling text boxes
	public string instructionText;
	public string testPhraseText;
	
	// Variable for the case insensitive string comparison type
	private StringComparison caseInsensitive;
	
	// Screen fade options
	private string[] options = new string[5];
	
	void Awake()
	{
		Time.timeScale = 1; // for some reason it only wants to print 1 character unless i copy this from
							// the previous title screen script
							
		// Get the enum of types of string comparisons and set the caseInsensitive variable		
		StringComparison[] comparisons = (StringComparison[]) Enum.GetValues(typeof(StringComparison));
		caseInsensitive = comparisons[5];
		
		// Enable dimmer and disable title screen
		dimmer.SetActive(true);
		titleScreenObj.SetActive(false);
	}
	
	void Start() 
	{
		// Set SFX channels
		audioPlayer.setSFX(0, "sfx_type_key");
		audioPlayer.setSFX(1, "sfx_enter");
		audioPlayer.setSFX(2, "sfx_enter_bad");
		
		// Set options for screen fades
		options[0] = "in"; // in vs out
		options[1] = "0.5"; // length of fade in seconds
		options[2] = "0"; // rgb values, (0, 0, 0) = black
		options[3] = "0";
		options[4] = "0"; 
		
		StartCoroutine(KeyboardTestSequence());
	}
	
	// Fades in screen, scrolls text, and activates input field
	private IEnumerator KeyboardTestSequence()
	{
		// Fade in
		StartCoroutine(textEvents.fade(options));
		yield return new WaitForSeconds(1.0f);
		
		// Scroll Text
		textScroll.startPrint(instructionText, instructions);
		yield return new WaitUntil(() => !textScroll.is_print); // wait for text to finish scrolling
		yield return new WaitForSeconds(0.5f);
		
		textScroll.startPrint(testPhraseText, testPhrase);
		textScroll.dump(); // display test phrase all at once
		
		// Activate input field and put it into focus
		inputField.enabled = true;
		inputField.Select();
	}
	
	// Fade out screen and go to title screen
	private IEnumerator TitleScreenTransition()
	{
		// Flash input text 4 times
		for(int i = 0; i < 4; i++)
		{
			yield return new WaitForSeconds(0.15f);
			
			inputText.color = Color.black;
			yield return new WaitForSeconds(0.3f);
			
			inputText.color = Color.white;
			yield return new WaitForSeconds(0.25f);
		}
		yield return new WaitForSeconds(0.4f);
		
		// Fade out
		options[0] = "out";
		options[1] = "1";
		StartCoroutine(textEvents.fade(options));
		yield return new WaitForSeconds(1.3f);	
				
		// Start Title Screen script
		titleScreen.enabled = true;
	}
	
	//*** EVENT FUNCTIONS ***//
	// Plays typing sfx when a key is pressed
	public void PlayTypingSFX()
	{
		audioPlayer.playSFX(0);
	}
	
	// Checks to see if the input is correct
	public void CheckInput(string input)
	{
		// if the player typed the phrase correctly (ignoring case)
		if(String.Equals(testPhraseText, input, caseInsensitive))
		{
			audioPlayer.playSFX(1);
			
			// Transition to title screen
			StartCoroutine(TitleScreenTransition());
		}
		else // Player did not type phrase correctly
		{ 
			audioPlayer.playSFX(2);
			
			// Delete the text and reselect the input field
			inputField.text = "";
			inputField.Select();
			inputField.ActivateInputField();
		}
	}
}