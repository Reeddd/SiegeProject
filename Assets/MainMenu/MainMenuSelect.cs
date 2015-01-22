using UnityEngine;
using System.Collections;

public class MainMenuSelect : MonoBehaviour 
{
	public AudioClip playSelect;

	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Input.GetMouseButtonDown(0)) 
		{
			selectMenuItem();
		}
	}

	public void selectMenuItem()	
	{	
		Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
		RaycastHit hit;

		if (Physics.Raycast (ray, out hit)) 
		{
			string objectHit = hit.collider.name;

			if(objectHit.Equals("Play_Castle"))
			{
				audio.PlayOneShot(playSelect);
				AutoFade.LoadLevel("Map1", 3, 1, Color.black);
			}

			if(objectHit.Equals("Exit_Castle"))
				Application.Quit();

		}
	}
}
