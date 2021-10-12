using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HistocachingII
{
    public class Toast : MonoBehaviour
    {
        public Text text;
        public Color color;

        private IEnumerator currentToast;

        public void Show(string text)
        {
            Debug.Log("Toast::Show " + text);

            if (currentToast != null)
            {
                StopCoroutine(currentToast);
                currentToast = null;
            }

            currentToast = ShowToast(text, 2f);
            StartCoroutine(currentToast);
        }

        private IEnumerator ShowToast(string text, float duration)
        {
            this.text.text = text;
            this.text.enabled = true;

            yield return FadeInAndOut(this.text, true, color, 0.25f);

            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                yield return null;
            }

            yield return FadeInAndOut(this.text, false, Color.clear, 0.25f);

            this.text.enabled = false;
            this.text.color = color;

            currentToast = null;
            gameObject.SetActive(false);
        }

        private IEnumerator FadeInAndOut(Text text, bool fadeIn, Color color, float duration)
        {
            float a, b;
            if (fadeIn)
            {
                a = 0f;
                b = 1f;
            }
            else
            {
                a = 1f;
                b = 0f;
            }

            float time = 0;
            while (time < duration)
            {
                time += Time.deltaTime;
                float alpha = Mathf.Lerp(a, b, time / duration);

                text.color = new Color(color.r, color.g, color.b, alpha);
                yield return null;
            }
        }
    }
}
