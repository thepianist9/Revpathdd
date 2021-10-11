using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace HistocachingII
{
    public class About : MonoBehaviour, IPointerClickHandler
    {
        private static readonly string[] titles = { "Geheim! Stasi an der TU Dresden.\nEine virtuelle Spurensuche.",
                                                    "Geheim! Stasi at the TU Dresden.\nTracing Clues Virtually." };

        private static readonly string[] captions = { @"Die App „Geheim!“ ist eine virtuelle Spurensuche auf dem Campus der TU Dresden. Sie führt zu verschiedenen Orten, an denen die Stasi bis 1989 beobachtete, überwachte und ermittelte. Über Histocaches können diese Orte und Geschichten entdeckt werden. 

Es sind Geschichten aus den Stasi-Akten, die mit Gebäuden auf dem Campus verknüpft sind. Die App gewährt einen Einblick in diese Geheimdienstakten – außerhalb des Archivs, am Ort des Geschehens. Augmented Reality (AR) lässt die historischen Gebäude wieder auferstehen. 

Die Geschichten erzählen von der Überwachung und Absicherung sensibler Forschungsbereiche, z.B. der Kernforschung, ebenso wie vom studentischen Alltagsleben, welches sich in Wohnheimen und Studentenklubs abspielte. Es sind Schlaglichter, die nur kleine Ausschnitte des Stasi-Überwachungssytems zeigen können, welches sich über die 40 Jahre, in der die DDR bestand, auch selbst veränderte. 

Die App „Geheim!“ ist ein Kooperationsprojekt des Bundesarchivs/Stasi-Unterlagen-Archivs, der Professur für Computergraphik und Visualisierung der TU Dresden und dem Universitätsarchiv der TU Dresden.

Programmiert und erstellt wurde die App von Studierenden im Rahmen eines Komplexpraktikums, welches die Professur für Computergraphik und Visualisierung zum Thema „HistoCaching II“ im Sommersemester 2021 anbot. Die Recherche und Texte der App sowie die Auswahl der Dokumente übernahm das Stasi-Unterlagen-Archiv Dresden, das Universitätsarchiv Dresden steuerte historische Bildaufnahmen für die AR-Ansichten und Hintergrundinformationen bei. 

Impressum:
Technische Umsetzung (TU Dresden): 
Prof. Dr. Stefan Gumhold, Dipl. Inf. Benjamin Russig, David Groß, MSc. Marzan Tasnim Oyshi, MSc.

Kontakt: 
Technische Universität Dresden, Professur für Computergraphik und Visualisierung, 01062 Dresden
<nobr>Telefon: +49 (0)351 463 38384</nobr>, <nobr>E-Mail: cgv-web@mailbox.tu-dresden.de</nobr>, <nobr>Web: <link=""https://tu-dresden.de/ing/informatik/smt/cgv""><u>https://tu-dresden.de/ing/informatik/smt/cgv</u></link></nobr>

Recherche, Texte, Redaktion (Stasi-Unterlagen-Archiv):
Cornelia Herold, M.A., Dr. Maria Fiebrandt, Luisa Fennert, B.A.

Kontakt: 
Bundesarchiv/Stasi-Unterlagen-Archiv Dresden, Riesaer Straße 7, 01129 Dresden
<nobr>Telefon: +49 (0)351 2508 0</nobr>, <nobr>E-Mail: dresden.stasi-unterlagen-archiv@bundesarchiv.de</nobr>, <nobr>Web: <link=""https://www.stasi-unterlagen-archiv.de""><u>www.stasi-unterlagen-archiv.de</u></link></nobr>

Recherche (Universitätsarchiv TU Dresden):
Dr. Matthias Lienert, Jutta Wiese

Kontakt:
Technische Universität Dresden, Universitätsarchiv, 01062 Dresden
<nobr>Telefon: +49 (0)351 463 34452</nobr>, <nobr>E-Mail: uniarchiv@tu-dresden.de</nobr>, <nobr>Web: <link=""https://tu-dresden.de/ua""><u>https://tu-dresden.de/ua</u></link></nobr>

Dresden, 2021",

                                                      @"The app “Geheim!“ is a virtual tour on the campus of TU Dresden. It enables users to visit various places that were monitored, investigated and surveilled by Stasi as late as 1989. These places and their stories can be explored by finding histocaches.
                                                      
The histocaches contain stories from the files of the Stasi, associated with buildings and places on the campus of TU Dresden. The app provides insights into these files ‒ outside the Archives, at the scene where they happened. Augmented reality revives the historic views of these places.
                                                      
The stories tell of the monitoring and protection of sensitive research areas like nuclear physics, but also the surveillance of everyday student life in dormitories and clubs. They can highlight only small parts of the Stasi surveillance system, which kept evolving over the 40 years of existence of the GDR.
                                                      
“Geheim!“ is a joint project of the Federal Archives and the Stasi Records Archive, the Chair of Computer Graphics and Visualization at TU Dresden and the University Archive of the TU Dresden.

The app was developed by students, doing a complex lab project at the Chair of Computer Graphics and Visualization. Research and curating was done by the Stasi Records Archive Dresden. The University Archive Dresden contributed the historic photographs used in AR mode as well as additional background information.
                                                      
Imprint:
Technical realization (TU Dresden):
Prof. Dr. Stefan Gumhold; Dipl. Inf. Benjamin Russig; David Groß, MSc. and Marzan Tasnim Oyshi, MSc.
                                                      
Contact:
Technische Universität Dresden, Professur für Computergraphik und Visualisierung, 01062 Dresden
<nobr>Phone: +49 (0)351 463 38384</nobr>, <nobr>Email: cgv-web@mailbox.tu-dresden.de</nobr>, <nobr>Web: <link=""https://tu-dresden.de/ing/informatik/smt/cgv""><u>https://tu-dresden.de/ing/informatik/smt/cgv</u></link></nobr>

Research, texts, editing (Stasi-Unterlagen-Archiv):
Cornelia Herold, M.A., Dr. Maria Fiebrandt, Luisa Fennert, B.A.

Contact: 
Bundesarchiv/Stasi-Unterlagen-Archiv Dresden, Riesaer Straße 7</nobr>, 01129 Dresden
<nobr>Phone: +49 (0)351 2508 0</nobr>, <nobr>Email: dresden.stasi-unterlagen-archiv@bundesarchiv.de</nobr>, <nobr>Web: <link=""https://www.stasi-unterlagen-archiv.de""><u>www.stasi-unterlagen-archiv.de</u></link></nobr>

Research (University Archive TU Dresden):
Dr. Matthias Lienert, Jutta Wiese

Contact:
Technische Universität Dresden, Universitätsarchiv, 01062 Dresden
<nobr>Phone: +49 (0)351 463 34452</nobr>, <nobr>Email: uniarchiv@tu-dresden.de</nobr>, <nobr>Web: <link=""https://tu-dresden.de/ua""><u>https://tu-dresden.de/ua</u></link></nobr>

Dresden, 2021" };

        private static readonly string[] clearDataTexts = { "Daten löschen", "Clear data" };

        // UI
        public Canvas canvas;
    
        public Text titleText;

        public ScrollRect scrollRect;

        public TMP_Text captionText;

        public Text clearDataText;

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
            Debug.Log("About::Show " + language);

            titleText.text = titles[language];
            captionText.text = captions[language];

            clearDataText.text = clearDataTexts[language];

            canvas.enabled = true;
        }

        public void Hide()
        {
            Debug.Log("About::Hide");

            canvas.enabled = false;
            gameObject.SetActive(false);

            scrollRect.normalizedPosition = new Vector2(0, 1);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            // If you are not in a Canvas using Screen Overlay, put your camera instead of null
            int linkIndex = TMP_TextUtilities.FindIntersectingLink(captionText, eventData.position, null);
            if (linkIndex != -1)
            {
                TMP_LinkInfo linkInfo = captionText.textInfo.linkInfo[linkIndex];

                Application.OpenURL(linkInfo.GetLinkID());
            }
        }

        public void ClearCache()
        {
            Davinci.ClearAllCachedFiles();
        }
    }
}
