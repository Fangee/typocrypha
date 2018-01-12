using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ally : MonoBehaviour, ICaster {
    public CasterStats Stats
    {
        get
        {
            throw new System.NotImplementedException();
        }
    }

    private BuffDebuff buffDebuff;
    public BuffDebuff BuffDebuff { get { return buffDebuff; } }

    private int gauge_value = 50;
    public int Curr_hp { get { return gauge_value; } set { gauge_value = value; } }
    public int Curr_shield { get { return 0; } set { } }
    private int curr_stagger;
    public int Curr_stagger { get { return curr_stagger; } set { curr_stagger = value; } }

    bool is_stunned = false;
    public bool Is_stunned { get { return is_stunned; } }
    private bool is_dead = false;
    public bool Is_dead { get { return is_dead; } }

    public ICasterType CasterType { get { return ICasterType.NPC_ALLY; } }

    public void damage(CastData data, int d, int element, ICaster caster, bool crit, bool reflect = false)
    {
        throw new System.NotImplementedException();
    }

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
