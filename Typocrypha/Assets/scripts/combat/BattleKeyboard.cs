using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Manages special conditions
public class BattleKeyboard : MonoBehaviour {
    public Dictionary<char, Image> image_map; // map from characters to key images (set from trackTyping)

    Dictionary<char, StatusEffect> status_map = new Dictionary<char, StatusEffect>();
    char[] keys = { 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm', ' ' };
    int numKeysAffected = 0;

    public void inflictCondition(Player player, int element, int intensity)
    {
        int numKeys = getNumKeysFromIntensity(intensity);
        char c = ' ';
        for(int i = 0; i <= numKeys; ++i)
        {
            c = getRandomValidChar();
            if (c == '?')
                break;
            Debug.Log("key " + c + "was inflicted with the " + Elements.toString(element) + " condition!");
            switch (element)
            {
                case Elements.fire:
                    status_map[c] = new StatusBurn(player);
                    StartCoroutine(keyTimer(c, 10f));
                    ++numKeysAffected;
                    break;
                case Elements.ice:
                    status_map[c] = new StatusFreeze();
                    ++numKeysAffected;
                    break;
                case Elements.volt:
                    char swap = getRandomValidChar();
                    if (swap == '?')
                        break;
                    status_map[c] = new StatusShock(swap);
                    status_map[swap] = new StatusShock(c);
                    StartCoroutine(keyTimer(c, 10f));
                    StartCoroutine(keyTimer(swap, 10f));
                    StartCoroutine(status_map[swap].keyGraphics(swap, image_map[swap]));
                    numKeysAffected += 2;
                    break;
                default:
                    throw new System.NotImplementedException("element " + Elements.toString(element) + " does not have a special condition yet!");
            }
            StartCoroutine(status_map[c].keyGraphics(c, image_map[c]));
        }
    }
    public string processKey(char c)
    {
        string ret = status_map[c].processKey(c);
        if (status_map[c].update())
        {
            --numKeysAffected;
            status_map[c] = new StatusNormal();
            image_map[c].color = Color.gray;
        }
        return ret;
    }
    public IEnumerator keyGraphics(char key, Image image)
    {
        return status_map[key].keyGraphics(key, image);
    }
    private IEnumerator keyTimer(char c, float seconds)
    {
        float time = 0;
        while(time < seconds)
        {
            yield return new WaitWhile(() => BattleManager.main.pause);
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
        status_map[c] = new StatusNormal();
        --numKeysAffected;
        image_map[c].color = Color.gray;
        yield break;
    }
    private void Start()
    {
        foreach (char c in keys)
            status_map.Add(c, new StatusNormal());
    }

    //Calculates number of keys to inflict condition on from intensity of attack
    private int getNumKeysFromIntensity(int intensity)
    {
        return 1 + intensity/25;
    }
    //Gets a char with no status effect (returns '?' if all keys are affected)
    private char getRandomValidChar()
    {
        if (numKeysAffected >= keys.Length)
            return '?';
        int randomInd = Random.Range(0, keys.Length - 1);
        while (!status_map[keys[randomInd]].isNormal)
        {
            randomInd = Random.Range(0, keys.Length - 1);
        }
        return keys[randomInd];
    }
}
