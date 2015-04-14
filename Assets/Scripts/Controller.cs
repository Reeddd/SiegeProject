using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour 
{
	
	/**Overview of MoveObject
	 * Basically, it controls the game on a large scale
	 * it keeps track of all the waypoints and
	 * the list of unused game objects.
	 * When the waypoints are created they are added to the array of waypoints, and as troops enter into nodes
	 * they are added to unused, and when nodes are clicked and troops are sent out, they are taken out of waypoints.
	 * This is probably the extent of what Move Objects will do, but there will probably be another script
	 * for Money that we will also put on the invisible Control game object.
	 * 
	 * This should be placed on an invisible game object as it is not part of the game. Call the invisible object Control.
	 * There is no prefab for control so don't mess it up.
	 * 
	 * Also it's important to remember that this script controls the prefabs so don't forget to 
	 * put the red and blue prefabs on the control object
	 */
	private Waypoint[] waypoints;
	private int countW;
	private bool inArray;
	private Player red;
	private Player blue;
	private Camera cam;
	private int[] Astats;
	private int[] Dstats;
	private int[] Sstats;
	public char recent;
	GameObject tester;

	void Start () 
	{
		inArray = false;
		waypoints = new Waypoint[15];
		countW = 0;
		tester = GameObject.Find ("Tester");
		if (GameObject.Find ("Tester") != null) 
		{
			//red = (FirstAI)(tester.GetComponent ("FirstAI"));
		}
		else
		{
			//red = (Human)(this.GetComponent ("Human"));
		}
		red = (Human)(this.GetComponent ("Human"));
		blue = (FirstAI)(this.GetComponent("FirstAI"));
		//ai2 = this.addComponent(FirstAI);
		if(red!=null)
			red.setMover ("TeamRed");
		if(blue!=null)
			blue.setMover ("TeamBlue");

		/********** IMPORTANT ************
		 * 
		  The first 3 values in this six long (0,1,2) array are values for RED
		  It goes in order of 0 = health, 1 = attack, 2 = speed
		  The second 3 values, (3, 4, 5) are values for BLUE
		  Same order, 3 = health, 4 = attack, 5 = speed
		 *
		 */
		int [] a = {300, 100, 22, 300, 100, 22};
		int [] d = {660, 30, 16, 660, 30, 16};
		int [] s = {300, 60, 50, 300, 60, 50};
		Astats = a;
		Dstats = d;
		Sstats = s;
		//Waypoint current = gameObject.Find("TeamRed");
		//current.wayNumber = 1;
		//organizePoints (current);
	}
	
	void startup()
	{
			
	}
	
	void Update () 
	//Gets the first and second nodes for movement
	{
		
  	}
	public void checkAdd(Waypoint waypoint)
	//Checks to see if a waypoint has already been added when it's gathering the list of waypoints
	{
		Vector3 wayp = waypoint.transform.position;
		Vector3 wp;
		Waypoint w;
		for(int i=0; i < waypoints.Length; i++)
		{
			if(waypoints[i]!=null)
			{
				w = waypoints[i];
				wp = w.transform.position;
				if(wp == wayp)
				{
					inArray = true;
				}
			}
		}
		addWaypoint (waypoint);
		inArray = false;
	}
	//Adds a waypoint 
	public void addWaypoint(Waypoint waypoint)		
	{
		if(!inArray)
		{
			waypoints[countW] = waypoint;
			countW++;
		}
	}
	
	//public void organizePoints(Waypoint current)
	//{
		//if(current.Equals(gameObject.Find("TeamBlue")))
		/*{
			return;
		}
		foreach(Waypoint w in current.waypoints)
		{
			if(w.wayNumber == 0)
			{
				w.wayNumber = current + 1;
			}
		}
		return;
	}*/
	
	public Waypoint[] getPoints()
	{
		return waypoints;
	}
	public void setCamera(Camera cam)
	{
		this.cam = cam;
		if(red is Human)
		{
			((Human)red).setCamerap (this.cam);
		}
		else if(blue is Human)
		{
			((Human)blue).setCamerap (this.cam);
		}
	}
	//Player methods
	public void resetPlayer()    {red.resetN();}
	public void RAddUnused(GameObject troop)    {red.addUnused (troop);}
	public bool RHasFirst()    {return red.hasFirst();}
	public bool RHasSecond()     {return red.hasSecond();}
	public Waypoint RGetFirst()    {return red.getFirst();}
	public Waypoint RGetSecond()   {return red.getSecond();}
	public void setRecent(char s)	{recent = s;}
	public char getRecent()	{return recent;}
	public string getHumanColor() 
	{
		if(red is Human)
		{
			return ((Human)red).getMover ();
		}
		else if(blue is Human)
		{
			return ((Human)blue).getMover ();
		}
		return "";
	}
	//AI methods
	public void BAddUnused(GameObject troop)    {blue.addUnused (troop);}
	public bool BHasFirst()    {return blue.hasFirst();}
	public bool BHasSecond()     {return blue.hasSecond();}
	public Waypoint BGetFirst()    {return blue.getFirst();}
	public Waypoint BGetSecond()   {return blue.getSecond();}
	
	
	/********** IMPORTANT ************
		 * 
		  The first 3 values in this six long (0,1,2) array are values for RED
		  It goes in order of 0 = health, 1 = attack, 2 = speed
		  The second 3 values, (3, 4, 5) are values for BLUE
		  Same order, 3 = health, 4 = attack, 5 = speed
		 *
	 */
	
	//SETTERS - RED
	public void setRedStatsA(int health, int attack, int speed)
	{
		Astats[0]=health;
		Astats[1]=attack;
		Astats[2]=speed;
	}
	public void setRedStatsD(int health, int attack, int speed)
	{
		Dstats[0]=health;
		Dstats[1]=attack;
		Dstats[2]=speed;
	}
	public void setRedStatsS(int health, int attack, int speed)
	{
		Sstats[0]=health;
		Sstats[1]=attack;
		Sstats[2]=speed;
	}
	//SETTERS - BLUE
	public void setBlueStatsA(int health, int attack, int speed)
	{
		Astats[3]=health;
		Astats[4]=attack;
		Astats[5]=speed;
	}
	public void setBlueStatsD(int health, int attack, int speed)
	{
		Dstats[3]=health;
		Dstats[4]=attack;
		Dstats[5]=speed;
	}
	public void setBlueStatsS(int health, int attack, int speed)
	{
		Sstats[3]=health;
		Sstats[4]=attack;
		Sstats[5]=speed;
	}
	
	//GETTERS - RED
	
	public int[] getRedStatsA()
	{
		int[] temp = {Astats[0], Astats[1], Astats[2]};
		return temp;
	}
	public int[] getRedStatsD()
	{
		int[] temp = {Dstats[0], Dstats[1], Dstats[2]};
		return temp;
	}
	public int[] getRedStatsS()
	{
		int[] temp = {Sstats[0], Sstats[1], Sstats[2]};
		return temp;
	}
	//GETTERS - BLUE
	public int[] getBlueStatsA()
	{
		int[] temp = {Astats[3], Astats[4], Astats[5]};
		return temp;
	}
	public int[] getBlueStatsD()
	{
		int[] temp = {Dstats[3], Dstats[4], Dstats[5]};
		return temp;
	}
	public int[] getBlueStatsS()
	{
		int[] temp = {Sstats[3], Sstats[4], Sstats[5]};
		return temp;
	}

	public void gameOver(string color)
	{
		string message = color + " Team Wins!";
		cam.GetComponent<GUI_VictoryMenu> ().setMessage(message);
		cam.GetComponent<GUI_VictoryMenu> ().enabled = true;
	}
	
}
