using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitleScreenNew : MonoBehaviour 
{	
	// References to necessary objects
	public GameObject keyboardTest;
	public GameObject titleScreen;
	
	public GameObject highlight;
	
	public GameObject continueObj;
	public GameObject loadGame;
	public GameObject newGame;
	public GameObject settings;
	public GameObject quit;
	
	// References to necessary scripts
	public TextEvents textEvents;
	
	// Reference to Audio Player
	public AudioPlayer audioPlayer;
	
	// Properties for navigating the menu
	private int index;
	private int top;
	private int bottom;
	private int previous;
	
	// Default text for each menu option
	private string continueText;
	private string loadGameText;
	private string newGameText;
	private string settingsText;
	private string quitText;
	
	// Components of highlight cursor
	private Transform highlightTransform;
	private Color highlightColor;
	
	// Arrays of menu options and their default text
	private GameObject[] menuOptions = new GameObject[5];
	private string[] defaultText = new string[5];
	
	// Screen fade options
	private string[] options = new string[5];
	
	void Start() 
	{
		// Play BGM
		audioPlayer.playMusic("bgm_title_loop");
		
		// Set SFX channels
		audioPlayer.setSFX(0, "sfx_enemy_select");
		audioPlayer.setSFX(1, "sfx_enter");
		
		// Populate menu options array
		menuOptions[0] = continueObj;
		menuOptions[1] = loadGame;
		menuOptions[2] = newGame;
		menuOptions[3] = settings;
		menuOptions[4] = quit;
		
		// Get default text values and populate array
		continueText = menuOptions[0].GetComponent<Text>().text;
		loadGameText = menuOptions[1].GetComponent<Text>().text;
		newGameText = menuOptions[2].GetComponent<Text>().text;
		settingsText = menuOptions[3].GetComponent<Text>().text;
		quitText = menuOptions[4].GetComponent<Text>().text;
		
		defaultText[0] = continueText;
		defaultText[1] = loadGameText;
		defaultText[2] = newGameText;
		defaultText[3] = settingsText;
		defaultText[4] = quitText;
		
		// Set default index and top/bottom of list
		index = 2;
		top = 2;
		bottom = 4;
		
		// Get highlight components
		highlightTransform = highlight.GetComponent<Transform>();
		highlightColor = highlight.GetComponent<SpriteRenderer>().color;
		
		// Select menu option at default index
		menuOptions[index].GetComponent<Text>().text = "> " + defaultText[index] + " <";
		menuOptions[index].GetComponent<Text>().color = highlightColor;
		
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
		// Player presses up
		if(Input.GetKeyDown("up"))
		{
            audioPlayer.playSFX(0);

			// Move the index up
			previous = index;
			index -= 1;
			
			// If the index is above the top
			if(index < top)
			{
				// Send it down to the bottom
				index = bottom;
			}
			
			UpdateCursor();			
		}
		
		// Player pressed down
		if(Input.GetKeyDown("down"))
		{
			audioPlayer.playSFX(0);
		
			// Move the index down
			previous = index;
			index += 1;
			
			// If the index is below the bottom
			if(index > bottom)
			{
				// Send it back to the top
				index = top;
			}
			
			UpdateCursor();
		}
		
		// Player presses enter
		if(Input.GetKeyDown("return") || Input.GetKeyDown("enter"))
		{
			audioPlayer.playSFX(1);
			
			// Call appropriate function based on which menu option is currently selected
			switch(index)
			{
				case 0:
					Continue();
					break;
				case 1:
					LoadGame();
					break;	
				case 2:
					NewGame();
					break;	
				case 3:
					Settings();
					break;	
				case 4: 
					Quit();
					break;	
				default:
					Debug.Log("i have no idea how this happened??? good job breaking the game, player");
					break;	
			}
		}
	}
	
	// Continue transitioning from the keyboard test screen
	private IEnumerator TitleScreenTransition()
	{
		// Hide the keyboard test and display the title screen
		keyboardTest.SetActive(false);
		titleScreen.SetActive(true);
		
		// Fade in
		yield return new WaitForSeconds(0.3f);
		StartCoroutine(textEvents.fade(options));
		yield return new WaitForSeconds(1.0f);
	}
	
	//*** MENU OPTION FUNCTIONS ***//
	// Updates the cursor's position and color
	private void UpdateCursor()
	{	
		// Reset previous option's color and text
		menuOptions[previous].GetComponent<Text>().color = Color.white;
		menuOptions[previous].GetComponent<Text>().text = defaultText[previous];
		
		// Set highlight position to the position of the selected menu option
		highlightTransform.position = menuOptions[index].GetComponent<RectTransform>().position;
		
		// Change the selected menu option's color and text
		menuOptions[index].GetComponent<Text>().color = highlightColor;
		menuOptions[index].GetComponent<Text>().text = "> " + defaultText[index] + " <";
	}
	
	private void Continue()
	{
		Debug.Log("Continue Pressed");
	}
	
	private void LoadGame()
	{
		Debug.Log("Load Game Pressed");
	}
	
	private void NewGame()
	{
		Debug.Log("New Game Pressed");
	}
	
	private void Settings()
	{
		Debug.Log("Settings Pressed");
	}
	
	private void Quit()
	{
		Debug.Log("Quit Pressed");
	}
}