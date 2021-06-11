using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutomaticStateChange : MonoBehaviour
{
    public TMP_Text _tmpText;

    private StateManager SM;

    void Awake()
    {
        SM = StateManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        _tmpText.text = "transform.localEulerAngles.x: " + transform.localEulerAngles.x + "\n";

        if (SM.state == State.Map || SM.state == State.Camera)
            if (transform.localEulerAngles.x <= 40f || transform.localEulerAngles.x >= 270f)
            {
                if (SM.state != State.Camera)
                    SM.SetState(State.Camera);
            }
            else
            {
                if (SM.state != State.Map)
                    SM.SetState(State.Map);
            }

        _tmpText.text += "state: " + SM.state + "\n";
    }
}
