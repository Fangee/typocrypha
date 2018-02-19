using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// edits by James Iwamasa

// represents an interrupt check
public delegate bool BattleInterruptDel(string[] opt);

// represents a cutscene that happens in the middle of a battle
public class BattleInterrupt {
	public CutScene scene; // scene to play
	BattleInterruptDel interrupt_cond; // condition for interrupting
	string[] opt; // options to interrupt condition
	// array of size 4 describing who is in scene (true if in scene, and must be alive for scene to play)
	// 0-3, left, middle, right, player
	bool[] who_speak;

	public BattleInterrupt(CutScene i_scene, string[] opts, bool[] i_who_speak) {
		scene = i_scene;
		who_speak = i_who_speak;
		// get function handle and opts for interrupt condition
		Debug.Log(opts[0]);
		interrupt_cond = BattleInterrupts.main.battle_interrupt_map[opts[0]];
		opt = opts.Skip (1).Take (opts.Length - 1).ToArray();
	}

	// returns whether interrupt condition has been fulfilled
	public bool checkCondition() {
		// make sure all speaking members are still alive
		bool dead_speaker = false;
		for (int j = 0; j < 3; j++) {
			if (who_speak [j] && BattleManager.main.enemy_arr [j].Is_dead) {
				dead_speaker = true;
				break;
			}
		}
		if (dead_speaker) return false;
		else              return interrupt_cond (opt);
	}
}

// event class for function handle checks to start battle interrupts
public class BattleInterrupts : MonoBehaviour {
	public static BattleInterrupts main = null; // static globabl ref
	public Dictionary<string, BattleInterruptDel> battle_interrupt_map;

	void Awake() {
		if (main == null) main = this;
		battle_interrupt_map = new Dictionary<string, BattleInterruptDel> {
			{"check-health",checkHealth},
			{"timed",timed},
			{"check-stun",checkStun},
			{"check-register-elem",checkRegisterElem},
			{"check-register-root",checkRegisterRoot},
			{"check-register-style",checkRegisterStyle},
			{"count-player-attacks",countPlayerAttacks}
		};
	}

	// checks if character's health is under a certain percentage
	// input: [0]: int   range(0-3) : whose health are we tracking (0=left, 1=middle, 2=right, 3=player)
	//        [1]: float range(0-1) : health ratio cutoff
	bool checkHealth(string[] opt) {
		int who_cond = int.Parse (opt [0]);
		float health_cond = float.Parse (opt [1]);
		// check if condition is fulfilled
		if (who_cond < 3) { // for enemy health
			Enemy curr_enemy = BattleManager.main.enemy_arr [who_cond];
			if ((float)curr_enemy.Curr_hp / (float)curr_enemy.Stats.max_hp <= health_cond)
				return true;
		} else { // for player health
			if ((float)Player.main.Curr_hp / (float)Player.main.Stats.max_hp <= health_cond)
				return true;
		}
		return false;
	}

	// checks if past a certain time
	// NOTE: since interrupts aren't checked at every frame, interrupt will occur on next checkInterrupt call
	// input: [0]: float : time in seconds from start of battle
	bool timed(string[] opt) {
		float time = float.Parse (opt [0]);
		if (DateTime.Compare (DateTime.Now, BattleManager.main.time_started.AddSeconds(time)) >= 0)
			 return true;
		else return false;
	}

	// checks if last spell cast caused a stun
	// input: NONE
	bool checkStun(string[] opt) {
		foreach (CastData d in BattleManager.main.last_cast)
			if (d.isStun) return true;
		return false;
	}

	// checks if new element was registered
	// to get what was actually registered, need to check BattleManager's 'last_register' and 'last_spell' fields
	// if you want to show what was registered in dialogue, use macro "{last-cast,elem}"
	// input: NONE
	bool checkRegisterElem(string[] opt) {
		return BattleManager.main.last_register [0];
	}

	// checks if new root was registered
	// to get what was actually registered, need to check BattleManager's 'last_register' and 'last_spell' fields
	// if you want to show what was registered in dialogue, use macro "{last-cast,root}"
	// input: NONE
	bool checkRegisterRoot(string[] opt) {
		return BattleManager.main.last_register [1];
	}

	// checks if new style was registered
	// to get what was actually registered, need to check BattleManager's 'last_register' and 'last_spell' fields
	// if you want to show what was registered in dialogue, use macro "{last-cast,style}"
	// input: NONE
	bool checkRegisterStyle(string[] opt) {
		return BattleManager.main.last_register [2];
	}

	// checks number of player attacks
	// input: [0]: int : number of attacks when interrupt should occur
	bool countPlayerAttacks(string[] opt) {
		int count = int.Parse (opt [0]);
		if (BattleManager.main.num_player_attacks >= count)
			 return true;
		else return false;
	}
}
