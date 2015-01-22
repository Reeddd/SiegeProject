using UnityEngine;
using System.Collections;

public class GUI_UpgradeSpeedButton : MonoBehaviour 
{
    public GUIStyle upgrade;

    void OnGUI()
    {
        //UPGRADE BUTTON
        GUI.Box(new Rect(470, Screen.height - 80, 30, 30), "");
        if (GUI.Button(new Rect(470, Screen.height - 80, 30, 30), "+", upgrade))
        {
			GameObject.Find ("Main Camera").GetComponent<GUI_UpgradeSpeedBox>().enabled = true;
			this.enabled = false;
        }
    }
}

