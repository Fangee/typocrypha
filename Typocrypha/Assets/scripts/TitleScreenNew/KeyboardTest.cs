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
	
	// Reference to necessary scripts
	public TitleScreenNew titleScreen;
	public TextScroll textScroll;
	
	// Reference to Audio Player
	public AudioPlayer audioPlayer;
	
	// Text to be placed in scrolling text boxes
	public string instructionText;
	public string testPhraseText;
	
	// Variable for the case insensitive string comparison type
	private StringComparison caseInsensitive;
	
	void Awake()
	{
		Time.timeScale = 1; // for some reason it only wants to print 1 character unless i copy this from
							// the previous title screen script
							
		// Get the enum of types of string comparisons and set the caseInsensitive variable		
		StringComparison[] comparisons = (StringComparison[]) Enum.GetValues(typeof(StringComparison));
		caseInsensitive = comparisons[5];
	}
	
	void Start() 
	{
		// Set SFX channels
		audioPlayer.setSFX(0, "sfx_type_key");
		audioPlayer.setSFX(1, "sfx_enter");
		audioPlayer.setSFX(2, "sfx_enter_bad");
		
		StartCoroutine(KeyboardTestSequence());
	}
		
	// Fades in screen, scrolls text, and activates input field
	private IEnumerator KeyboardTestSequence()
	{
		// Fade in
		
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
	
	// EVENT FUNCTIONS
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
			
			// fade out
			// go to next screen
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