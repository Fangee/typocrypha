using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//Manages special conditions
public class BattleKeyboard : MonoBehaviour {
    public Dictionary<char, Image> image_map; // map from characters to key images (set from trackTyping)
    public Dictionary<char, Text> text_map;
	public Sprite key_default; // default key image
	public Sprite[] frozen_keys = new Sprite[4]; // frozen key images
	public GameObject popper_object; // popper object for player burn damage


    Dictionary<char, StatusEffect> status_map = new Dictionary<char, StatusEffect>();
    char[] keys = { 'q', 'w', 'e', 'r', 't', 'y', 'u', 'i', 'o', 'p', 'a', 's', 'd', 'f', 'g', 'h', 'j', 'k', 'l', 'z', 'x', 'c', 'v', 'b', 'n', 'm'};
    int numKeysAffected = 0;

    private void Start()
    {
        foreach (char c in keys)
            status_map.Add(c, new StatusNormal(key_default));
    }

    public void clearStatus()
    {
        status_map.Clear();
        numKeysAffected = 0;
        foreach (char c in keys)
        {
            status_map.Add(c, new StatusNormal(key_default));
            if(image_map != null)
            {
                image_map[c].sprite = key_default;
                image_map[c].color = Color.gray;
            }
        }
    }
    public void inflictCondition(Player player, int element, int elementIntensity, int damage)
    {
        //Get the number of keys to inflict conditions on from the formula (maybe ,ake per-element)
        int numKeys = getNumKeysFromIntensity(element, elementIntensity, damage);
        char c = ' ';
        for(int i = 0; i < numKeys; ++i)
        {
            c = getRandomValidChar();
            if (c == '?')
                break;
            Debug.Log("key " + c + "was inflicted with the " + Elements.toString(element) + " condition!");
			Popper[] player_popper = popper_object.GetComponents<Popper>(); // player popper for burn damage
            switch (element)
            {
                case Elements.fire:
                    status_map[c] = new StatusBurn(player, player_popper[0], c);
                    StartCoroutine(keyTimer(c, 10f));
                    ++numKeysAffected;
                    break;
                case Elements.ice:
                    status_map[c] = new StatusFreeze(frozen_keys, player_popper[0], c);
                    ++numKeysAffected;
                    break;
                case Elements.volt:
                    char swap = getRandomValidChar();
                    if (swap == '?')
                        break;
                    int j = 0;
                    while (swap == c && j < 5)
                    {
                        swap = getRandomValidChar();
                    }
                    status_map[c] = new StatusShock(swap, c, player_popper[0]);
                    status_map[swap] = new StatusShock(c, swap, player_popper[0]);
                    StartCoroutine(keyTimer(c, 10f));
                    StartCoroutine(keyTimer(swap, 10f));
                    StartCoroutine(status_map[swap].keyGraphics(swap, image_map[swap], text_map[swap]));
                    numKeysAffected += 2;
                    break;
                default:
                    throw new System.NotImplementedException("element " + Elements.toString(element) + " does not have a special condition yet!");
            }
            StartCoroutine(status_map[c].keyGraphics(c, image_map[c], text_map[c]));
        }
    }
    public string processKey(char c)
    {
        string ret = status_map[c].processKey(c);
        if (status_map[c].update())
        {
            --numKeysAffected;
			status_map[c] = new StatusNormal(key_default);
            image_map[c].color = Color.gray;
        }
        return ret;
    }
    public IEnumerator keyGraphics(char key, Image image, Text text)
    {
        return status_map[key].keyGraphics(key, image, text);
    }
    private IEnumerator keyTimer(char c, float seconds)
    {
        float time = 0;
        while(time < seconds)
        {
            yield return new WaitWhile(() => BattleManagerS.main.pause);
            yield return new WaitForEndOfFrame();
            time += Time.deltaTime;
        }
		status_map[c] = new StatusNormal(key_default);
        --numKeysAffected;
        image_map[c].color = Color.gray;
        yield break;
    }

    //Calculates number of keys to inflict condition on from intensity of attack
    private int getNumKeysFromIntensity(int element, int intensity, int damage)
    {
        if (element == Elements.volt)
            return 1 + intensity + Random.Range(0, damage/30);
        return 2 + Random.Range(0,3 + damage/30) + intensity;
    }
    //Gets a char with no status effect (returns '?' if all keys are affected)
    private char getRandomValidChar()
    {
        if (numKeysAffected >= keys.Length)
            return '?';
        int randomInd = Random.Range(0, keys.Length);
        while (!status_map[keys[randomInd]].isNormal)
        {
            randomInd = Random.Range(0, keys.Length);
        }
        return keys[randomInd];
    }
}
