using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AutomaticStateChange : MonoBehaviour
{
    public TMP_Text m_DebugText;

    private StateManager SM;

    void Awake()
    {
        SM = StateManager.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        // m_DebugText.text = "transform.localEulerAngles.x: " + transform.localEulerAngles.x + "\n";

        if (SM.state == State.Map || SM.state == State.Camera)
            if (transform.localEulerAngles.x <= 50f || transform.localEulerAngles.x >= 270f)
            {
                if (SM.state != State.Camera)
                    SM.SetState(State.Camera);
            }
            else
            {
                if (SM.state != State.Map)
                    SM.SetState(State.Map);
            }

        // m_DebugText.text += "state: " + SM.state + "\n";
    }
}
