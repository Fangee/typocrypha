using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Gameflow
{

    public class WaveManager : MonoBehaviour
    {
        public Battlefield field;
        public GameObject enemyPrefab;
        public Canvas enemyCanvas;
        public GameObject allyPrefab;
        public GameObject waveTransitionPrefab;
        public Canvas waveTransitionCanvas;
        [HideInInspector] public int totalWaves;

        private int waveNum = 0;


        public void startWave(BattleNodeWave waveData)
        {
            //Clear old battlefield data
            field.clear();
            // Timed effects and creation
            StartCoroutine(startWaveCR(waveData));
        }
        private IEnumerator startWaveCR(BattleNodeWave waveData)
        {
            //Create/manage allies
            yield return StartCoroutine(createEnemies(waveData.enemyData));
            waveTransition(waveData);
        }
        private IEnumerator createEnemies(EnemyData[] enemyData)
        {
            for(int i = 0; i < enemyData.Length; ++i)
            {
                if (enemyData[i] == null)
                    continue;
                EnemyData data = enemyData[i];
                ATB2.Enemy e = Instantiate(enemyPrefab, enemyCanvas.transform).GetComponent<ATB2.Enemy>();
                e.enemyData = data;
                field.Add(e, new Battlefield.Position(0, i));
                e.Setup();
                // Enemy Spawn Graphics               
                yield return new WaitForSeconds(0.4f);
            }
        }
        private void waveTransition(BattleNodeWave waveData)
        {
            GameObject wT = Instantiate(waveTransitionPrefab, waveTransitionCanvas.transform);
            wT.transform.Find("WaveBanner").GetComponentInChildren<Text>().text = DialogParser.main.substituteMacros(waveData.waveTitle);
            wT.transform.Find("WaveTitle").GetComponentInChildren<Text>().text = "Wave " + ++waveNum + "/ " + totalWaves;
        }
        public void startCombat()
        {
            Debug.Log("Battle Continues");
        }
    }

}