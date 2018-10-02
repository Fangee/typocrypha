using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

// Container class for MenuOption callback functions
public class MenuEvents : MonoBehaviour 
{
	// MenuEvents class will likely need to have references to variables it needs to change unless they're 
	// global or something, put them here later
	
	public void TestCallback()
	{
		Debug.Log("abababababa");
	}
	
	public void TestCallback2()
	{
		Debug.Log("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
	}
}
