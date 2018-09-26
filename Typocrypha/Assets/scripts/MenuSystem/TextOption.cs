using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Simple menu option that displays text and calls a function when pressed
public class TextOption : MenuOption 
{
	// Reference to text object
	public Text text;
	
	// Color to be changed to when selected
	public Color highlightColor;
	
	// Default value variables
	private string defaultText;
	private Color defaultColor;
	
	void Awake()
	{
		// Set flags
		confirmable = true;
		slidable = false;
		
		// Save default values
		defaultText = text.text;
		defaultColor = text.color;
	}
	
	// Called when the option is selected
	public override void OnSelect(BaseEventData eventData) 
	{
		// Change color to highlight color
		text.color = highlightColor;
		
		// Add cursor arrows around text
		text.text = "> " + defaultText + " <";
	}
	
	// Called when the option is deselected
	public override void OnDeselect(BaseEventData eventData) 
	{
		// Revert color back to original
		text.color = defaultColor;
		
		// Remove arrows around text
		text.text = defaultText;
	}
}
