using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
// edits by James Iwamasa

// represents status of a battle interrupt check
public enum BattleInterruptStatus { FALSE, TRUE, REPEAT };

// represents an interrupt check
public delegate BattleInterruptStatus BattleInterruptDel(string[] opt);

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
		interrupt_cond = BattleInterrupts.main.battle_interrupt_map[opts[0]];
		opt = opts.Skip (1).Take (opts.Length - 1).ToArray();
	}

	// returns whether interrupt condition has been fulfilled
	public BattleInterruptStatus checkCondition() {
		// make sure all speaking members are still alive
		bool dead_speaker = false;
		for (int j = 0; j < 3; j++) {
			if (who_speak [j] && BattleManagerS.main.field.enemy_arr [j].Is_dead) {
				dead_speaker = true;
				break;
			}
		}
		if (dead_speaker) return BattleInterruptStatus.FALSE;
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
	BattleInterruptStatus checkHealth(string[] opt) {
		int who_cond = int.Parse (opt [0]);
		float health_cond = float.Parse (opt [1]);
		// check if condition is fulfilled
		if (who_cond < 3) { // for enemy health
			Enemy curr_enemy = BattleManagerS.main.field.enemy_arr [who_cond];
			if ((float)curr_enemy.Curr_hp / (float)curr_enemy.Stats.max_hp <= health_cond)
				return BattleInterruptStatus.TRUE;
		} else { // for player health
			if ((float)Player.main.Curr_hp / (float)Player.main.Stats.max_hp <= health_cond)
				return BattleInterruptStatus.TRUE;
		}
		return BattleInterruptStatus.FALSE;
	}

	// checks if past a certain time
	// NOTE: since interrupts aren't checked at every frame, interrupt will occur on next checkInterrupt call
	// input: [0]: float : time in seconds from start of battle
	BattleInterruptStatus timed(string[] opt) {
		float time = float.Parse (opt [0]);
		if (DateTime.Compare (DateTime.Now, BattleManagerS.main.field.time_started.AddSeconds(time)) >= 0)
			 return BattleInterruptStatus.TRUE;
		else return BattleInterruptStatus.FALSE;
	}

	// checks if last spell cast caused a stun
	// input: NONE
	BattleInterruptStatus checkStun(string[] opt) {
		foreach (CastData d in BattleManagerS.main.field.last_cast)
			if (d.isStun) return BattleInterruptStatus.TRUE;
		return BattleInterruptStatus.FALSE;
	}

	// checks if new element was registered
	// to get what was actually registered, need to check BattleManagerS's 'last_register' and 'last_spell' fields
	// if you want to show what was registered in dialogue, use macro "{last-cast,elem}"
	// input: NONE
	BattleInterruptStatus checkRegisterElem(string[] opt) {
		if (BattleManagerS.main.field.last_register [0]) {
			BattleManagerS.main.field.last_register [0] = false;
			return BattleInterruptStatus.REPEAT;
		}
		else return BattleInterruptStatus.FALSE;
	}

	// checks if new root was registered
	// to get what was actually registered, need to check BattleManagerS's 'last_register' and 'last_spell' fields
	// if you want to show what was registered in dialogue, use macro "{last-cast,root}"
	// input: NONE
	BattleInterruptStatus checkRegisterRoot(string[] opt) {
		if (BattleManagerS.main.field.last_register [1]) {
			BattleManagerS.main.field.last_register [1] = false;
			return BattleInterruptStatus.REPEAT;
		}
		else return BattleInterruptStatus.FALSE;
	}

	// checks if new style was registered
	// to get what was actually registered, need to check BattleManagerS's 'last_register' and 'last_spell' fields
	// if you want to show what was registered in dialogue, use macro "{last-cast,style}"
	// input: NONE
	BattleInterruptStatus checkRegisterStyle(string[] opt) {
		if (BattleManagerS.main.field.last_register [2]) {
			BattleManagerS.main.field.last_register [2] = false;
			return BattleInterruptStatus.REPEAT;
		}
		else return BattleInterruptStatus.FALSE;
	}

	// checks number of player attacks
	// input: [0]: int : number of attacks when interrupt should occur
	BattleInterruptStatus countPlayerAttacks(string[] opt) {
		int count = int.Parse (opt [0]);
		if (BattleManagerS.main.field.num_player_attacks >= count)
			 return BattleInterruptStatus.TRUE;
		else return BattleInterruptStatus.FALSE;
	}
}
