using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//A class that processes an input key
public abstract class StatusEffect {
    public bool isNormal = false;
    public abstract string processKey(char key);
    public abstract IEnumerator keyGraphics(char key, Image image);
    //return true if status should be reset to normal
    public virtual bool update() { return false; }
}

public class StatusNormal : StatusEffect
{
    public StatusNormal()
    {
        isNormal = true;
    }
    public override IEnumerator keyGraphics(char key, Image image)
    {
        image.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        image.color = Color.gray;
    }

    public override string processKey(char key)
    {
        return key.ToString();
    }
}

public class StatusFreeze : StatusEffect
{
    int freezeNum = 5;
    public override IEnumerator keyGraphics(char key, Image image)
    {
        image.color = Color.blue;
        yield break;
    }

    public override string processKey(char key)
    {
        return "";
    }
    public override bool update()
    {
        --freezeNum;
        return freezeNum <= 0;
    }
}

public class StatusBurn : StatusEffect
{
    int dmg = 5;
    Player p;
    public StatusBurn(Player p)
    {
        this.p = p;
    }
    public override IEnumerator keyGraphics(char key, Image image)
    {
        image.color = Color.red;
        yield break;
    }

    public override string processKey(char key)
    {
        p.Curr_hp -= dmg;
        return key.ToString();
    }
}

public class StatusShock : StatusEffect
{
    char swapped;
    public StatusShock(char swap)
    {
        swapped = swap;
    }
    public override IEnumerator keyGraphics(char key, Image image)
    {
        image.color = Color.yellow;
        yield break;
    }

    public override string processKey(char key)
    {
        return swapped.ToString();
    }
}



