using UnityEngine;
using System.Collections;

public class GUI_VictoryMenu : MonoBehaviour {

	public Rect windowRect0 = new Rect(20, 20, 120, 50);
	public Rect windowRect1 = new Rect(20, 100, 120, 50);
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
//		windowRect0 = GUI.Window(0, windowRect0, DoMyWindow, "My Window");
//		windowRect1 = GUI.Window(1, windowRect1, DoMyWindow, "My Window");
	}
}
