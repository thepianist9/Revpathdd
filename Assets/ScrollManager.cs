using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScrollManager : MonoBehaviour {

    public TextMeshProUGUI text;
    public float scrollSpeed = 10.0f;

    private TextMeshProUGUI cloneText;
    private RectTransform textRectTransform;
    private string sourceText;
    private string tempText;

    // Use this for initialization
    void Awake () {
        textRectTransform = text.GetComponent<RectTransform>();
        
        cloneText = Instantiate(text) as TextMeshProUGUI;
        RectTransform cloneRectTransform = cloneText.GetComponent<RectTransform>();
        cloneRectTransform.SetParent(textRectTransform);
        cloneRectTransform.anchorMin = new Vector2(1, 0.5f);
        cloneRectTransform.localPosition = new Vector3(0, 0, cloneRectTransform.position.z);
        // cloneRectTransform.localScale = new Vector3(1, 1, 1);
        // cloneText.text = text.text;  

    }
    private IEnumerator Start()
    {
        float width = text.preferredWidth;
        Vector3 startPosition = textRectTransform.position;

        float scrollPosition = 0;
        while (true)
        {
            textRectTransform.position = new Vector3(-scrollPosition % width, startPosition.y, startPosition.z);
            scrollPosition += scrollSpeed * 20 * Time.deltaTime;
            yield return null;
        }
    }

}