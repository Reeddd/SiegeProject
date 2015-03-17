using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public GameObject[] unused;
	private int countU;
	private GameObject reuse;
	public GameObject prefabA;
	public GameObject prefabD;
	public GameObject prefabS;
	
	public Movement()
	{
		Start ();	
	}
	
	// Use this for initialization
	void Start () 
	{
		countU = 0;
		unused = new GameObject[10];
		reuse = null;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	//This method checks to see if a troop of the correct color in the unused, and if so uses it
	public void moveTroop(bool red, Waypoint first, Waypoint second)
	{
		bool done=false;
		//Check to see if there are any troop game objects not being used
			if(countU > 0)
			{
				int i;
				for(i=0; i<unused.Length-1;i++)
				{
					if(unused[i]==null)
					{
						
					}
					else if(red)
					{
						reuse = unused[i];
						if((reuse.GetComponent<Speed>() as Speed !=null && first.getCountS()>0)
						|| (reuse.GetComponent<Attack>() as Attack !=null && first.getCountA()>0)
						|| (reuse.GetComponent<Defense>() as Defense !=null && first.getCountD()>0))
						{	
							reuse.transform.position = getRadPosition(first, second);
							//reuse.transform.position = first.collider.bounds.center;
							reuse.GetComponent<Troop>().startM(red);
							countU--;
							unused[i] = null;
							if(reuse.GetComponent<Speed>() as Speed !=null)
								first.subtractS();
							else if(reuse.GetComponent<Attack>() as Attack !=null)
								first.subtractA();
							else if(reuse.GetComponent<Defense>() as Defense !=null)
								first.subtractD ();
							done = true;
							break;
						}
					}
					else if(!red)
					{
						reuse = unused[i];
						unused[i] = null;
						reuse.transform.position = getRadPosition (first, second);
						reuse.GetComponent<Troop>().startM(red);
						countU--;
						if(reuse.GetComponent<Speed>() as Speed !=null)
							first.subtractS();
						else if(reuse.GetComponent<Attack>() as Attack !=null)
							first.subtractA();
						else if(reuse.GetComponent<Defense>() as Defense !=null)
							first.subtractD ();
						done=true;
						break;
					}
				}

		}
		//Instantiate a new troop if there isn't one in unused
		if(!done)
		{
			Vector3 instPos = getRadPosition (first,second);
			if(red)
			{	
				if(first.getCountA()>0)
				{
					GameObject newTroop = (GameObject)Instantiate(prefabA, instPos, first.transform.rotation);
					newTroop.GetComponent<Troop>().setColor ("red");
					newTroop.GetComponent<Troop>().startM (red);
					first.subtractA();
				}
				else if(first.getCountS()>0)
				{
					GameObject newTroop = (GameObject)Instantiate(prefabS, instPos, first.transform.rotation);
					newTroop.GetComponent<Troop>().setColor ("red");
					newTroop.GetComponent<Troop>().startM (red);
					first.subtractS();
				}
				else if(first.getCountD()>0)
				{
					GameObject newTroop = (GameObject)Instantiate(prefabD, instPos, first.transform.rotation);
					newTroop.GetComponent<Troop>().setColor ("red");
					newTroop.GetComponent<Troop>().startM (red);
					first.subtractD();
				}
			}
			else
			{
				if(first.getCountA()>0)
				{
					GameObject newTroop = (GameObject)Instantiate(prefabA, instPos, first.transform.rotation);
					newTroop.GetComponent<Troop>().setColor ("blue");
					newTroop.GetComponent<Troop>().startM (red);
					first.subtractA();
				}
				else if(first.getCountS()>0)
				{
					GameObject newTroop = (GameObject)Instantiate(prefabS, instPos, first.transform.rotation);
					newTroop.GetComponent<Troop>().setColor ("blue");
					newTroop.GetComponent<Troop>().startM (red);
					first.subtractS();
				}
				else if(first.getCountD()>0)
				{
					GameObject newTroop = (GameObject)Instantiate(prefabD, instPos, first.transform.rotation);
					newTroop.GetComponent<Troop>().setColor ("blue");
					newTroop.GetComponent<Troop>().startM (red);
					first.subtractD();
				}
			}
		}
		done=false;
		
	}

	public Vector3 getRadPosition(Waypoint first, Waypoint second)
	{
		Vector3 between = second.collider.bounds.center - first.collider.bounds.center;
		float angle = Mathf.Atan(between.x/between.z);
		Vector3 dirRadius = new Vector3();
		dirRadius.z = Mathf.Cos(angle) * 4.2f;
		dirRadius.x = Mathf.Sin(angle) * 4.2f;
		if(between.z < 0)
		{	
			dirRadius = dirRadius * -1;
		}
		return first.collider.bounds.center + dirRadius;
	}
	
	public void addUnused(GameObject troop)
	{
		bool done = false;
		int i;
		for(i=0; i<unused.Length-1; i++)
		{
			if(unused[i]==null)
			{
				unused[i] = troop;
				countU++;
				done = true;
				break;
			}
		}
		if(!done)
		{
			unused[i]=troop;
		}
	}
}
