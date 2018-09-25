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
	
	// Variable for default text
	private string defaultText;
	
	void Start()
	{
		// Save default text
		defaultText = text.text;
	}
	
	// Called when the option is selected
	public override void OnSelect(BaseEventData eventData) 
	{
		Debug.Log("select: " + defaultText);
	}
	
	// Called when the option is deselected
	public override void OnDeselect(BaseEventData eventData) 
	{
		Debug.Log("deselect: " + defaultText);
	}
}
