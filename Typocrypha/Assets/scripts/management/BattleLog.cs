using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Manages battle log text
public class BattleLog : MonoBehaviour {
	public GameObject battleLogCast; //Casting log object and Associated reference to store
	private Image castBox;
	public Text logCastText;
	public Text logCastInfo;
	public GameObject battleLogTalk; //talk log object and Associated reference to store
	private Image talkBox;
	public Text logTalkText;
	public Text logTalkInfo;

	public Color playerColor;
	public Color enemyColor;
	public Color allyColor;
	public Color clarkeColor;

	void Start () {
		castBox = battleLogCast.GetComponent<Image>();
		talkBox = battleLogTalk.GetComponent<Image>();
	}
	
	//Enable battle log UI state (call anywhere that the battlemanager pauses to cast)
	public void log(string cast, ICasterType caster, string talk, string speaker)
	{
		battleLogCast.SetActive(true);
		battleLogTalk.SetActive(true);
		logCastText.text = "> " + cast;
		logTalkText.text = talk;
		logTalkInfo.text = speaker;
		if (caster == ICasterType.ENEMY)
		{
			castBox.color = enemyColor;
			talkBox.color = enemyColor;
			logCastInfo.text = "ENEMY  CAST";
		}
		else if (caster == ICasterType.PLAYER)
		{
			castBox.color = playerColor;
			talkBox.color = playerColor;
			logCastInfo.text = "PLAYER CAST";
		}
		else if (caster == ICasterType.NPC_ALLY)
		{
			castBox.color = allyColor;
			talkBox.color = allyColor;
			logCastInfo.text = "ALLY   CAST";
		}
		else //caster == IcasterType.INVALID (clarke is speaking)
		{
			castBox.color = clarkeColor;
			talkBox.color = clarkeColor;
			logCastInfo.text = "ERROR  CAST";
		}
	}

	//Stop battle log UI (call after every pause to cast)
	public void stop()
	{
		battleLogCast.SetActive(false);
		battleLogTalk.SetActive(false);
	}
}
