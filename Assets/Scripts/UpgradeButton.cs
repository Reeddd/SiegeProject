using UnityEngine;
using System.Collections;

public class UpgradeButton : MonoBehaviour 
{
    public GUIStyle upgrade;

	// Use this for initialization
	void Start () 
    {
	
	}
	
	// Update is called once per frame
	void Update () 
    {
	
	}

    void OnGUI()
    {
        //UPGRADE BUTTON
        GUI.Box(new Rect(0, Screen.height - 100, 100, 50), "");
        if (GUI.Button(new Rect(10, Screen.height - 95, 80, 40), "Upgrades", upgrade))
        {

        }
    }
}
