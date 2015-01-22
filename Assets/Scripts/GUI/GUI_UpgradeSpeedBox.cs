using UnityEngine;
using System.Collections;

public class GUI_UpgradeSpeedBox : MonoBehaviour 
{
	public GUIStyle upgrade;
	//public bool GUIon = GameObject.Find ("Main Camera").GetComponent<GUI_UpgradeSpeedkButton>().enabled;
	
	
    void OnGUI()
    {
        //UPGRADE BUTTON
        GUI.Box(new Rect(450, Screen.height - 150, 70, 100), "");
        if (GUI.Button(new Rect(450, Screen.height - 150, 70, 100), "+", upgrade))
        {
			GameObject.Find ("Main Camera").GetComponent<GUI_UpgradeSpeedButton>().enabled = true;
			this.enabled = false;
        }
    }
}
