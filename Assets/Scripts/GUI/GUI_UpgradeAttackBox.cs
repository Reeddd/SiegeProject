using UnityEngine;
using System.Collections;

public class GUI_UpgradeAttackBox : MonoBehaviour 
{
	public GUIStyle upgrade;
	
	void OnGUI()
    {
        //UPGRADE BUTTON
        GUI.Box(new Rect(250, Screen.height - 150, 70, 100), "");
        if (GUI.Button(new Rect(250, Screen.height - 150, 70, 100), "+", upgrade))
        {
			GameObject.Find ("Main Camera").GetComponent<GUI_UpgradeAttackButton>().enabled = true;
			this.enabled = false;
        }
    }
}
