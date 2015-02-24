using UnityEngine;
using System.Collections;

public class GUI_VictoryMenu : MonoBehaviour {

	public Rect windowRect0 = new Rect(30, 30, 1000, 1000);
	//public Rect windowRect1 = new Rect(20, 100, 120, 50);
	
	void Start()
	{
		this.enabled = false;
	}

	void OnGUI()
	{
		windowRect0.Set(0,0, 600, 200);
		windowRect0.x = (Screen.width - windowRect0.width) / 2;
		windowRect0.y = ((Screen.height - windowRect0.height )*10)/ 28;
		windowRect0 = GUI.Window (0, windowRect0, DoMyWindow, "My Window");
//		windowRect0 = GUI.Window(0, windowRect0, DoMyWindow, "My Window");
//		windowRect1 = GUI.Window(1, windowRect1, DoMyWindow, "My Window");
	}

	void DoMyWindow(int windowID)
	{
		if(GUI.Button (new Rect(10, 20, 580, 160), "Hello World"))
			print("Got a click");
	}
}
