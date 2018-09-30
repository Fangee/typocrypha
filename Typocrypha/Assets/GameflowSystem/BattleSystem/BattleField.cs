using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Encapsulates battle info
public class Battlefield {

    //Properties

    private int player_ind = 1;
    public ICaster Player { get { return allies[player_ind]; } }
    public bool Pause { get { return callback.battlePause; } set { callback.battlePause = value; } }

    public enum Position
    {
        NONE = -2,
        ANY = -1,
        LEFT,
        MIDDLE,
        RIGHT,
        ALLYLEFT,
        PLAYER,
        ALLYRIGHT,
    }
    public ICaster getCaster(Position pos)
    {
        return casters[pos];
    }
    public static bool isEnemy(Position pos)
    {
        return (pos == Position.LEFT || pos == Position.MIDDLE || pos == Position.RIGHT);
    }

    //BattleField data
    private Dictionary<Position, ICaster> casters = null
	public Enemy[] enemies = new Enemy[3]; // array of Enemy components (size 3)
	public ICaster[] allies = { null, null, null }; // array of Player and allies (size 3)
	public int enemy_count = 0; // number of enemies in battle
	public int curr_dead = 0;

    //Interrupt stuff
    [HideInInspector] public ICaster lastCaster = null;
    [HideInInspector] public List<CastData> lastCast; // last performed cast action
	[HideInInspector] public SpellData lastSpell; // last performed spell
    [HideInInspector] public bool[] lastRegister; // last spell register status

    //CALLBACK FUNCTIONS

    private BattleManagerS callback;

    public Battlefield(BattleManagerS callback)
    {
        this.callback = callback;
    }
    public void addSceneToQueue(GameObject interruptScene)
    {
        callback.addSceneToQueue(interruptScene);
    }
    public void update()
    {
        callback.updateEnemies();
    }
    public void updateScouterInfo()
    {
        callback.uiManager.updateScourterInfo();
    }
    public void breakThirdEye()
    {
        if(callback.thirdEyeActive)
            callback.stopThirdEye(true);
    }
}
