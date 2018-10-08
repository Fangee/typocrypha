using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class CheckBox : MenuOption 
{
	// Event callbacks when the box is checked/unchecked
	public UnityEvent OnChecked;
	public UnityEvent OnUnchecked;
	
	// Sprites for checked and unchecked values
	public Sprite spriteUnchecked;
	public Sprite spriteChecked;
	
	// Reference to this check box's sprite renderer component and text box
	public SpriteRenderer spriteRenderer;
	public Text textBox;
	
	// Whether the box is checked, (0/false == unchecked, 1/true == checked)
	private bool checkedState;
	
	void Awake() 
	{
		// Set flags
		confirmable = true;
		slidable = false;
		
		// Save default text box values
		defaultText = textBox.text;
		defaultColor = textBox.color;
	}
		
	// Always called OnConfirm, changes check box state
	public void ChangeCheckedState()
	{
		// Invert the check box bool
		checkedState = !checkedState;
		
		// Change sprite and call appropriate event based on state of check box
		switch(checkedState)
		{
			case true:
				spriteRenderer.sprite = spriteChecked;
				OnChecked.Invoke();
				break;
			case false:
				spriteRenderer.sprite = spriteUnchecked;
				OnUnchecked.Invoke();
				break;
		}
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
