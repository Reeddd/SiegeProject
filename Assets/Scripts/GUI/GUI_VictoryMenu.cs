using UnityEngine;
using System.Collections;

public class GUI_VictoryMenu : MonoBehaviour {

	public Rect windowRect0 = new Rect(30, 30, 600, 200);
	private string message;

	void Start()
	{
		this.enabled = false;
	}

	void OnGUI()
	{
		windowRect0.Set(0,0, 600, 200);
		windowRect0.x = (Screen.width - windowRect0.width) / 2;
		windowRect0.y = ((Screen.height - windowRect0.height )*10)/ 28;
		windowRect0 = GUI.Window (0, windowRect0, DoMyWindow, message);
		//Time.timeScale = 0;
	}

	void DoMyWindow(int windowID)
	{
		if(GUI.Button (new Rect(10, 20, 580, 160), "Play Again"))
		{
			//Time.timeScale=1;
			Application.LoadLevel ("Map2");
		}
	}

	public void setMessage(string mess)
	{
		message = mess;
	}
}
