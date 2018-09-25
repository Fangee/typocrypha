using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TypocryphaGameflow
{

    public class WaveManager : MonoBehaviour
    {

        public BattleField field;
        public GameObject enemyPrefab;
        public GameObject allyPrefab;
        public GameObject waveTransitionPrefab;
        public Canvas waveTransitionCanvas;
        [HideInInspector] public int totalWaves;

        private int waveNum = 0;


        public IEnumerator startWave(BattleNodeWave waveData)
        {
            //Clear old battlefield data
            //Create/manage allies
            yield return StartCoroutine(createEnemies(waveData.enemyData));
            waveTransition(waveData);
        }
        private IEnumerator createEnemies(EnemyData[] enemyData)
        {
            yield break;
        }
        private void waveTransition(BattleNodeWave waveData)
        {
            GameObject waveTransition = Instantiate(waveTransitionPrefab, waveTransitionCanvas.transform);
            waveTransition.transform.Find("WaveBannerText").GetComponent<Text>().text = DialogParser.main.substituteMacros(waveData.waveTitle);
            waveTransition.transform.Find("WaveTitleText").GetComponent<Text>().text = "Wave " + ++waveNum + "/ " + totalWaves; ;
        }
    }

}