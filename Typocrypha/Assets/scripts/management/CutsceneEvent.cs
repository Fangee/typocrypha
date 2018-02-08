using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//Edits by Valentino Abate

//Event class (use baked in arguments (in constructor) to start function call, finish with call)

public abstract class CutsceneEvent
{
    public abstract void call();
}

//Registration Event. Attempts to register keywords in array to spellnook through given SpellDictionary
public class RegisterSpellEvent : CutsceneEvent
{
    private string[] keywords;
    private SpellDictionary dict;

    public RegisterSpellEvent(List<string> keywords, SpellDictionary dict)
    {
        this.keywords = keywords.ToArray();
        this.dict = dict;
    }

    public override void call()
    {
        if(dict == null)
            throw new System.ArgumentNullException();
        foreach(string s in keywords)
        {
            dict.safeRegister(s.ToLower());
        }
        AudioPlayer.main.playSFX(0, SFXType.SPELL, "sfx_learn_spell_cutscene");
    }
}

