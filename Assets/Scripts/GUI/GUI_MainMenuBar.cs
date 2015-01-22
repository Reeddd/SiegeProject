using UnityEngine;
using System.Collections;

public class GUI_MainMenuBar : MonoBehaviour
{	
	
	public GUIStyle unclickable;
    public GUIStyle custom;
	public AudioClip spawnSound;
    
	public Camera activeCamera;
	public Camera cameraMain;
	public Camera cameraAlt;
	private Controller cont;
	public GUIStyle attack;
    public int attackCost;
	
	public GUIStyle defense;
    public int defenseCost;
	
	public GUIStyle speed;
	public int speedCost;
	
	public int gold;

    // Use this for initialization
    void Start()
    {
		cameraMain.enabled = true;
		cameraAlt.enabled = false;
		activeCamera = cameraMain;
		cont = (Controller)GameObject.Find ("Control").GetComponent ("Controller");
		cont.setCamera (activeCamera);
		gold = 25;
		InvokeRepeating("GimmeMoney", 1.5f, 0.3f);
		speedCost = 25;
		defenseCost = 35;
		attackCost = 50;
		GameObject.Find("Main Camera").GetComponent<GUI_UpgradeAttackBox>().enabled = false;
		GameObject.Find("Main Camera").GetComponent<GUI_UpgradeDefenseBox>().enabled = false;
		GameObject.Find("Main Camera").GetComponent<GUI_UpgradeSpeedBox>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
		if(Input.GetKeyDown ("c"))
		{
			if(cameraMain.enabled)
			{
				cameraMain.enabled = false;
				cameraAlt.enabled = true;
				activeCamera = cameraAlt;
			}
			else
			{
				cameraAlt.enabled = false;
				cameraMain.enabled = true;
				activeCamera = cameraMain;
			}
			
			cont.setCamera(activeCamera);
		}
    }
	
	public void GimmeMoney()
	{
		gold++;
	}	

	public void increaseCost()
	{
		attackCost += 1;
		defenseCost += 1;
		speedCost += 1;
	}

    void OnGUI()
    {
        //MENU BAR
        GUI.Box(new Rect(50, Screen.height - 50, Screen.width-100, 50), "");
        //GOLD
		GUI.Label(new Rect(75, Screen.height - 40, 80, 30), "" + gold , custom);
        //ATTACK BOX
		if(gold >= attackCost){
			if(GUI.Button(new Rect(250, Screen.height - 40, 70, 30), "Attack", attack))
        	{
				GameObject.Find("TeamRed").GetComponent<Waypoint>().addTroopRedA();
				audio.PlayOneShot(spawnSound);
				gold -= attackCost;
				increaseCost ();
        	}
		}
		else GUI.Box(new Rect(250, Screen.height - 40, 70, 30), "Attack", unclickable);
		//DEFENSE BOX
		if(gold >= defenseCost){
        	if (GUI.Button(new Rect(350, Screen.height - 40, 70, 30), "Defense", defense))
        	{
				GameObject.Find("TeamRed").GetComponent<Waypoint>().addTroopRedD();
				audio.PlayOneShot(spawnSound);
				gold -= defenseCost;
				increaseCost ();
        	}
		}
		else GUI.Box(new Rect(350, Screen.height - 40, 70, 30), "Defense", unclickable); 
		//SPEED BOX
		if(gold >= speedCost){
	        if (GUI.Button(new Rect(450, Screen.height - 40, 70, 30), "Speed", speed))
	        {
				GameObject.Find("TeamRed").GetComponent<Waypoint>().addTroopRedS();
				audio.PlayOneShot(spawnSound);
				gold -= speedCost;
				increaseCost ();
	        }
		}
		else GUI.Box(new Rect(450, Screen.height - 40, 70, 30), "Speed", unclickable);
    }
	
}
