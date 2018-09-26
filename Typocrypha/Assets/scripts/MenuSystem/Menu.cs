using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Container object for menu options
public class Menu : MonoBehaviour 
{
	// Reference to cursor object
	public Transform cursorTransform;
	
	// Variable for keeping track of navigation through the menu
	private int index;
	
	// List containing menu entries
	public List<MenuOption> menuList;
	
	void Awake()
	{
		// Default values
		cursorTransform.position = menuList[index].GetComponent<RectTransform>().position;
		index = 0;
	}
	
	void Start() 
	{		
		// Select the default option
		menuList[index].Select();
	}
	
	void Update() 
	{
		HandleMenuNavigation();
	}
	
	// Checks for input and proccesses it
	private void HandleMenuNavigation()
	{
		// Player presses up
		if(Input.GetKeyDown("up"))
		{
			// Move the index up
			index -= 1;
			
			// If the index is above the top
			if(index < 0)
			{
				// Send it down to the bottom
				index = menuList.Count - 1;
			}
			
			// Select the option at current index
			menuList[index].Select();
			
			// Update cursor position
			cursorTransform.position = menuList[index].GetComponent<RectTransform>().position;
		}
		// Player presses down
		else if(Input.GetKeyDown("down"))
		{
			// Move the index down
			index += 1;
			
			// If the index is below the bottom
			if(index > menuList.Count - 1)
			{
				// Send it back to the top
				index = 0;
			}
			
			// Select the option at current index
			menuList[index].Select();
			
			// Update cursor position
			cursorTransform.position = menuList[index].GetComponent<RectTransform>().position;
		}
		
		// Check if the current menu option is confirmable
		if(menuList[index].confirmable)
		{
			// Player presses enter
			if(Input.GetKeyDown("return") || Input.GetKeyDown("enter"))
			{
				// Call the OnConfirm function of that menu option
				menuList[index].OnConfirm.Invoke();
			}
		}
		
		// Check if the current menu option is a slider
		if(menuList[index].slidable)
		{
			// Player presses or holds left
			if(Input.GetKey("left"))
			{
				// Reduce the slider variable
				// menuList[index].Reduce();	
			}
			// Player presses or holds right
			if(Input.GetKey("right"))
			{
				// Increase the slider variable
				// menuList[index].Increase();
			}
		}
	}
}

