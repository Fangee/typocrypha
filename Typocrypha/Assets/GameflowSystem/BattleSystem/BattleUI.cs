using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class BattleUI : MonoBehaviour
{
    public EnemyChargeBars charge_bars; // creates and mananges charge bars
    public EnemyStaggerBars stagger_bars; // creates and manages stagger bars
    public EnemyHealthBars health_bars; // creates and manages enemy health bars
    public BattleLog battle_log;
    public GameObject target_ret; // contains targetting sprites
    public GameObject target_floor; // holds the enemy floor panels
    public Image wave_transition_title;
    public Image wave_transition_banner;
    public Text wave_title_text;
    public Text wave_banner_text;
	public Text last_cast_text; // Text for logging previous spell attempt
	public TrackTyping trackTyping;
    //public GameObject dialogue_box; // text box for dialogue
    public GameObject battle_bg_prefab; // prefab of battle background

    TargetReticule target_ret_scr; // TargetReticule script ref
    TargetFloor target_floor_scr;  // TargetFloor script ref

    public const float enemy_spacing = 6f; // horizontal space between enemies
    public const float enemy_y_offset = 0.5f; // offset of enemy from y axis
    public const float reticule_y_offset = 2.5f; // offset of target reticule

    private const int initial_target_ind = 1;

    // configure script refs
    void Start()
    {
        target_ret_scr = target_ret.GetComponent<TargetReticule>();
        target_floor_scr = target_floor.GetComponent<TargetFloor>();
    }
    //Initialize the battle bg
    public void initBg()
    {
        //Set background
        BackgroundEffects.main.setPrefabBG(battle_bg_prefab);
    }

	//Initialize a specific battle bg
	public void initBg(string prefab_name)
	{
		//Set background
		BackgroundEffects.main.setPrefabBG(prefab_name);
	}

    //Initialize the targeting and target floor UI
    public void initTarget()
    {
		//Show targeting UI
		target_ret.SetActive(true);
		target_floor.SetActive (true);
		//Update UI
		target_ret_scr.updateTarget(new Vector2((initial_target_ind - 1) * enemy_spacing, reticule_y_offset));
		target_floor_scr.updateFloor();
    }
    //Start the UI for a new wave
    public void startWave()
    {
        charge_bars.removeAll();
        stagger_bars.removeAll();
        health_bars.removeAll();
        charge_bars.initChargeBars();
        stagger_bars.initStaggerBars();
        health_bars.initHealthBars();
    }
    //Update targeting UI floor UI, and charge bars
    public void updateUI()
    {
        //Update target and floor effects
        target_ret_scr.updateTarget();
        target_floor_scr.updateFloor();
		charge_bars.updateChargeBars ();
		last_cast_text.text = "[TAB] -> USE LAST CAST\n> " + trackTyping.getBuffer().Replace(" ", "-").ToUpper();
    }
    //Sets the target from specified index
    public void setTarget(int target_ind)
    {
		target_ret_scr.updateTarget(new Vector2((target_ind - 1) * enemy_spacing, reticule_y_offset));
        target_floor_scr.updateFloor();
        // play sfx
        AudioPlayer.main.playSFX("sfx_enemy_select");
    }
    //Toggles if the scouter is on or not
    public void toggleScouter()
    {
        target_ret_scr.toggleScouter();
		if (target_ret_scr.isScouterVisible()) {
			AudioPlayer.main.playSFX("sfx_scanner_open");
		} else {
			AudioPlayer.main.playSFX("sfx_scanner_close");
		}
    }
    public void showScouter()
    {
        target_ret_scr.showScouter();
    }
    public void hideScouter()
    {
        target_ret_scr.hideScouter();
    }
    //update scouterInfo (refreshes the info for form changes)
    public void updateScourterInfo()
    {
        target_ret_scr.updateScouterInfo();
    }
    //Clears all enemy UI and battle bg
    public void clear()
    {
        charge_bars.removeAll();
        stagger_bars.removeAll();
        health_bars.removeAll();
        target_ret.SetActive(false);
        target_floor.SetActive(false);
        BackgroundEffects.main.removePrefabBG(2.0f);
    }
    //Play the wave transition animations
    public IEnumerator waveTransition(string title, int curr_wave, int max_wave)
    {
        AudioPlayer.main.playSFX("sfx_spell_miss");
        wave_banner_text.text = title;
        wave_title_text.text = "Wave " + curr_wave + "/ " + max_wave;
        Animator banner_text_animator = wave_banner_text.GetComponent<Animator>();
        Animator banner_img_animator = wave_transition_banner.GetComponent<Animator>();
        Animator title_text_animator = wave_title_text.GetComponent<Animator>();
        Animator title_img_animator = wave_transition_title.GetComponent<Animator>();
        banner_text_animator.Play("anim_wave_banner_text");
        banner_img_animator.Play("anim_wave_banner_image");
        title_text_animator.Play("anim_wave_title_text");
        title_img_animator.Play("anim_wave_title_image");
        yield return new WaitForSeconds(1.2f);
        wave_banner_text.GetComponent<Animator>().enabled = false;
        wave_transition_banner.GetComponent<Animator>().enabled = false;
        wave_title_text.GetComponent<Animator>().enabled = false;
        wave_transition_title.GetComponent<Animator>().enabled = false;
        yield return new WaitUntil(() => Input.GetKeyDown(KeyCode.Space));
        wave_banner_text.GetComponent<Animator>().enabled = true;
        wave_transition_banner.GetComponent<Animator>().enabled = true;
        wave_title_text.GetComponent<Animator>().enabled = true;
        wave_transition_title.GetComponent<Animator>().enabled = true;
        AudioPlayer.main.playSFX("sfx_enter");
        AudioPlayer.main.playSFX("sfx_enemy_death");
        yield return new WaitForSeconds(0.7f);
    }

	// Set the active state of the enemy status bars
	public void setEnabledGauges(bool isActive){
		health_bars.transform.gameObject.SetActive(isActive);
		charge_bars.transform.gameObject.SetActive(isActive);
		stagger_bars.transform.gameObject.SetActive(isActive);
	}
}