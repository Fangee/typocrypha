using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ATB
{
    // UI amount bar with shadowed transitions between amounts
    public class ShadowBar : MonoBehaviour
    {
        public RectTransform amount; // Transform for main bar
        public RectTransform shadow; // Transform for shadow bar
        public float curr // Normalized current amount
        {
            get
            {
                return amount.localScale.x;
            }
            set
            {
                StartCoroutine(transition(amount, delay, time, value));
                StartCoroutine(transition(shadow, shadowDelay, shadowTime, value));
            }
        }
        public float delay = 0f; // Delay before amount changes
        public float shadowDelay = 0.5f; // Delay before shadow changes
        public float time = 0f; // Time it takes for bar to reach target amount
        public float shadowTime = 0.5f; // Time it takes for shadow to reach target amount
        
        IEnumerator transition(RectTransform bar, float delay, float time, float target)
        {
            if (delay != 0f)
                yield return new WaitForSeconds(delay);
            float steps = Mathf.Floor(time / Time.fixedDeltaTime);
            float start = bar.localScale.x;
            if (time != 0f)
            {
                for (float step = 0; step < steps; step++)
                {
                    float scale = Mathf.Lerp(start, target, step / steps);
                    bar.localScale = new Vector3(scale, bar.localScale.y, bar.localScale.z);
                    yield return new WaitForFixedUpdate();
                }
            }
            bar.localScale = new Vector3(target, bar.localScale.y, bar.localScale.z);
        }
    }
}

