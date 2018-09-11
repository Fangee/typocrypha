using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
	
	// Text to be placed in scrolling text boxes
	public string instructionText;
	public string testPhraseText;
	
	void Awake()
	{
		Time.timeScale = 1; // for some reason it only wants to print 1 character unless i copy this from
							// the previous title screen script
	}
	
	void Start() 
	{
		// Fade in
		
		StartCoroutine(scrollText());
		
		// when done, activate text field (put into focus)
	}
	
	void Update() 
	{
		// if key pressed, play sound
		// onSubmit():
		// if correct text, go to title screen, play sound
		// fade out of keyboard test, fade into title screen
		// if incorrect text, clear text, play sound
	}
	
	// Scrolls the two lines of text
	IEnumerator scrollText()
	{
		textScroll.startPrint(instructionText, instructions);
		yield return new WaitUntil(() => !textScroll.is_print); // wait for text to finish scrolling
		
		textScroll.startPrint(testPhraseText, testPhrase);
		yield return new WaitUntil(() => !textScroll.is_print);
	}
}