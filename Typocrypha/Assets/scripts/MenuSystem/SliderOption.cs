using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class SliderOption : MenuOption 
{
	// Unity events for increasing and reducing slider value
	public UnityEvent Increase;
	public UnityEvent Reduce;
	
	// Reference to value to be changed by slider
	// public int sliderVariable;
	
	// Default value of the slider variable
	public int defaultValue;
	
	// Min/Max value of slider variable
	public int sliderMin;
	public int sliderMax;
	
	// Interval to change the slider value by
	public int interval;
	
	// References to cursor location and text box object
	public RectTransform cursorSprite;
	public Text textBox;

	// Current value of the slider
	private int currentSliderValue;
	
	void Awake() 
	{
		// Set flags
		confirmable = false;
		slidable = true;
		
		// Save default text box values
		defaultText = textBox.text;
		defaultColor = textBox.color;
		
		// Set default slider value
		currentSliderValue = defaultValue;
		// sliderVariable = currentSliderValue;
	}
	
	// Called when the option is selected
	public override void OnSelect(BaseEventData eventData) 
	{
		// Change color to highlight color
		textBox.color = highlightColor;
		
		// Add cursor arrows around text
		textBox.text = "> " + defaultText;
	}
	
	// Called when the option is deselected
	public override void OnDeselect(BaseEventData eventData) 
	{
		// Revert color back to original
		textBox.color = defaultColor;
		
		// Remove arrows around text
		textBox.text = defaultText;
	}
}

// menu events class contains references object/variable to be changed
// invoke unity event on left/right, each event is different for each slider
// pass reference to sliderOption.Increase/Reduce(variable, interval)
// interval/min/max still in the slider class?
// increase/reduce is generic for every slider

// place example slider onscreen, placeholder assets