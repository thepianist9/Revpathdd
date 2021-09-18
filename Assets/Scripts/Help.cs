using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

namespace HistocachingII
{
    public class Help : MonoBehaviour
    {
        private static readonly string[] titles = { "Anleitung",
                                                    "How to use?" };

        private static readonly string[] captions = { "Mit dieser App können sie die die Relikte der Stasizeit erkunden und mit eigenen Augen sehen. Das Prinzip ist simpel: In der unteren linken Ecke sehen sie eine kleine Karte. Wenn sie ihr Gerät parallel zum Boden halten, nimmt die Karte den gesamten Bildschirm ein und sie können sich so einen Überblick über Interessante Orte in ihrer Umgebung schaffen. Indem sie zwei finger aufeinander zu oder voneinander weg bewegen, können sie den Sichtbereich der Karte verkleinern oder Vergrößern. Wenn sie ihr Gerät nun Vertikal halten, gehen sie in den AR Modus über. Falls sie sich in der Nähe eines Interessanten Punktes befinden, können sie ihr Gerät auf das Gebäude ausrichten und ein historisches Bild betrachten, so wie es früher ausgesehen hat. In der oberen rechten Ecke befindet sich ein ausklappbares Menü. In diesem Menü können sie mit hilfe des zweiten Knopfes auch in die Fotogallerie übergehen. Der Dritte Knopf Erklärt was es mit dieser App auf sich hat. Der Vierte Knopf brachte sie hierher. Der Fünfte Knopf ändert die Sprache zu Englisch und der erste Knopf schließt dieses Menü.",

        "With this app you can explore the relics of the Stasi era and see them with your own eyes. The principle is simple: In the lower left corner you see a minimap. If you hold your device parallel to the ground, the map takes up the entire screen and you can get an overview of interesting places in your surroundings. By moving two fingers towards or away from each other, you can zoom in or out on the map. If you now hold your device vertically, you enter the AR mode. If you are near a point of interest, you can point your device at the building and view a historical image of how it used to look. In the upper right corner there is a Drop-down menu. In this menu you can also go to the photo gallery with the help of the second button. The third button explains what this app is all about. The fourth button brings you here. The fifth button changes the language to German and the first button closes this menu." };

        // private static readonly float[] times = { 0f, 4f, 8f };
        private static readonly string[] subtitles = { "Walk to the viewpoint", "Point your phone to building", "OK" };

        // UI
        public Canvas canvas;
        
        public Text titleText;
        public Text captionText;

        public VideoPlayer videoPlayer;

        // Start is called before the first frame update
        void Start()
        {
            videoPlayer.Prepare();
        }

        void Update()
        {
            // Make sure user is on Android platform
            if (Application.platform == RuntimePlatform.Android)
            { 
                // Check if Back was pressed this frame
                if (Input.GetKeyDown(KeyCode.Escape))
                    Hide();
            }
        }

        public void Show(int language)
        {
            Debug.Log("Help::Show " + language);

            // titleText.text = titles[language];
            captionText.text = captions[language];

            canvas.enabled = true;

            videoPlayer.Play();

            // StartCoroutine(ShowSubtitles());
        }

        public void Hide()
        {
            Debug.Log("Help::Hide");

            // StopCoroutine(ShowSubtitles());

            videoPlayer.Stop();

            canvas.enabled = false;
        }

        private IEnumerator ShowSubtitles()
        {
            while (!videoPlayer.isPrepared)
            {
                yield return null;
            }
            
            while (videoPlayer.isPlaying)
            {
                if (videoPlayer.time < 4f)
                {
                    titleText.text = subtitles[0];

                    yield return new WaitForSeconds(4f - (float) videoPlayer.time);
                }
                else if (videoPlayer.time < 8f)
                {
                    titleText.text = subtitles[0] + "\n" + subtitles[1];

                    yield return new WaitForSeconds(8f - (float) videoPlayer.time);
                }
                else
                {
                    titleText.text = subtitles[1] + "\n" + subtitles[2];

                    yield return new WaitForSeconds((float) (videoPlayer.length - videoPlayer.time));
                }

                // int currentIndex = index % subtitles.Length;

                // if (times.Length > index)
                // {
                //     yield retun new WaitForSeconds((float) (videoPlayer.length - videoPlayer.time));
                // }
                // else
                // {

                // }

                // if (videoPlayer.time < times[currentIndex])
                // {
                //     yield return new WaitForSeconds(times[currentIndex] - (float) videoPlayer.time);
                // }
                // else
                // {
                //     Debug.Log("TANIA " + videoPlayer.time);
                    
                //     titleText.text = subtitles[currentIndex];

                //     index += 1;

                //     yield return new WaitForSeconds();
                // }
            }
        }
    }
}
