
using UnityEngine.UI;
using UnityEngine;


public class PanZoom : MonoBehaviour 
{
    Vector3 touchStart;
    public float min_text_size = 25;
    public float max_text_size = 100;
    private Text descriptionText;
        
    void Start()
    {
        descriptionText = gameObject.GetComponent<Text>();
    }
    // Update is called once per frame
    void Update () 
    {

        if(Input.GetKeyDown("i"))
        {
        	descriptionText.fontSize+= 3;
            Debug.Log("i is being pressed");
        }
	    if(Input.GetKeyDown("o"))
        {
        	descriptionText.fontSize-= 3;
        }

       
        // if(Input.touchCount == 2)
        // {
        //     Touch touchZero = Input.GetTouch(0);
        //     Touch touchOne = Input.GetTouch(1);
 
        //     Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
        //     Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;
 
        //     float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
        //     float currentMagnitude = (touchZero.position - touchOne.position).magnitude;
 
        //     float difference = currentMagnitude - prevMagnitude;
 
        //     zoom(difference);

        // }     
         
    }
 
    void zoom(float increment)
    {
        descriptionText.fontSize+= (int)increment;
    }


    public void ResetZoom()
    {
        descriptionText.fontSize = (int)min_text_size;
    }
 }
