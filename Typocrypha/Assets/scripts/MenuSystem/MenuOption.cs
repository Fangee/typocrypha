using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Base class for menu options, more specific types of options will inherit from this
abstract public class MenuOption : Selectable, ISelectHandler, IDeselectHandler
{
	// Function that is called when the option is pressed
	public UnityEvent OnConfirm;
	
	// Called when the option is selected
	abstract public void OnSelect(BaseEventData eventData);
	
	// Called when the option is deselected
	abstract public void OnDeselect(BaseEventData eventData);
}