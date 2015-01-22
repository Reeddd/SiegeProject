using UnityEngine;
using System.Collections;

public class GUI_UpgradeDefenseButton : MonoBehaviour 
{
    public GUIStyle upgrade;

    void OnGUI()
    {
        //UPGRADE BUTTON
        GUI.Box(new Rect(370, Screen.height - 80, 30, 30), "");
        if (GUI.Button(new Rect(370, Screen.height - 80, 30, 30), "+", upgrade))
        {
			GameObject.Find ("Main Camera").GetComponent<GUI_UpgradeDefenseBox>().enabled = true;
			this.enabled = false;
        }
    }
}