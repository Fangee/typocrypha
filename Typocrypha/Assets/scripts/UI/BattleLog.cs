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
	public GameObject battleLogSub; // Sub log that displays the name of the spell used
	public Sprite[] battleLogIcons; // Icons for spell elements (0 - Phys; 1 - Fire; 2 - Ice; 3 - Volt)
	Image[] battleLogSubSpellIcons;
	Animator battleLogSubAnimator;
	Text[] battleLogSubText;

	public Color playerColor;
	public Color enemyColor;
	public Color allyColor;
	public Color clarkeColor;

	void Start () {
		castBox = battleLogCast.GetComponent<Image>();
		talkBox = battleLogTalk.GetComponent<Image>();
		battleLogSubAnimator = battleLogSub.GetComponent<Animator> ();
		battleLogSubText = battleLogSub.GetComponentsInChildren<Text> ();
		battleLogSubSpellIcons = battleLogSub.GetComponentsInChildren<Image> ();
	}
	
	//Enable battle log UI state (call anywhere that the battlemanager pauses to cast)
	public void log(SpellData cast, ICasterType caster, string talk, string speaker, Vector3 casterPosition)
	{
		battleLogCast.SetActive(false);
		battleLogTalk.SetActive(false);
		battleLogSub.SetActive (true);
		string ele = "";
		switch (cast.element) {
		case "agni":
			battleLogSubSpellIcons [1].sprite = battleLogIcons [1];
			battleLogSubSpellIcons [2].sprite = battleLogIcons [1];
			ele = "FIRE";
			break;
		case "cryo":
			battleLogSubSpellIcons [1].sprite = battleLogIcons [2];
			battleLogSubSpellIcons [2].sprite = battleLogIcons [2];
			ele = "ICE";
			break;
		case "veld":
			battleLogSubSpellIcons [1].sprite = battleLogIcons [3];
			battleLogSubSpellIcons [2].sprite = battleLogIcons [3];
			ele = "VOLT";
			break;
		default:
			battleLogSubSpellIcons [1].sprite = battleLogIcons [0];
			battleLogSubSpellIcons [2].sprite = battleLogIcons [0];
			ele = "PHYSICAL";
			break;
		}
		battleLogSubAnimator.Play ("anim_sub_battlelog_enter");
		battleLogCast.transform.position = casterPosition;
		battleLogSubText[0].text = cast.ToString();
		logCastText.text = "> " + cast.ToString();
		logTalkText.text = talk;
		//logTalkInfo.text = speaker;
		logCastInfo.text = speaker;
		if (caster == ICasterType.ENEMY)
		{
			castBox.color = enemyColor;
			talkBox.color = enemyColor;
			battleLogSubText[1].text = "ENEMY CASTS " + ele;
		}
		else if (caster == ICasterType.PLAYER)
		{
			castBox.color = playerColor;
			talkBox.color = playerColor;
			battleLogSubText[1].text = "PLAYER CASTS " + ele;
		}
		else if (caster == ICasterType.ALLY)
		{
			castBox.color = allyColor;
			talkBox.color = allyColor;
			battleLogSubText[1].text = "ALLY CASTS " + ele;
		}
		else //caster == IcasterType.INVALID (clarke is speaking)
		{
			castBox.color = clarkeColor;
			talkBox.color = clarkeColor;
			battleLogSubText[1].text = "ERROR CAST";
		}
	}

	//Stop battle log UI (call after every pause to cast)
	public void stop()
	{
		battleLogCast.SetActive(false);
		battleLogTalk.SetActive(false);
		battleLogSubAnimator.Play ("anim_sub_battlelog_exit_swell");
	}
}
