using UnityEngine;
using System.Collections;
using System.IO;
using System;

public class TestAIs : MonoBehaviour {

	private struct Results
	{
		public int testsRun;
		public string nextTest;

		public Results(string[] data)
		{
			testsRun = Int32.Parse (data[1]);
			nextTest = data[0];
		}
	}

	private GameObject control;
	private Controller cont;
	Results results;
	float timer;
	string path1 = @".\FirstVBaseResults.txt";
	string path2 = @".\PotentialVBaseResults.txt";
<<<<<<< HEAD
	string path3 = @".\DecisionVBaseResults.txt";
=======
	string path3 = @".\DTvBaseResults.txt"; //hey!
>>>>>>> 6c11d265eb46d44a7c8bb5929fb96e107cf44b60
	string masterPath = @".\trackTesting.txt";

	public float blueWins;
	public float redWins;
	public int blueDeaths;
	public int redDeaths;

	//master path, set to one of the numbered paths above
	string path;
	bool first;
	// Use this for initialization
	void Start () 
	{
		results = readMaster ();
		//Debug.Log (results.numberBaseWins +" "+results.numberFirstAIWins);
		timer = Time.time;
		path = path1;
		first = true;
		if(GameObject.Find ("Control")!=null)	//Don't ask me why this is here as opposed to in the start method, it just works this way and doesn't work that way
		{
			control = GameObject.Find("Control");	
			cont = (Controller)(control.GetComponent("Controller"));
		}
		blueWins = 0;
		redWins = 0;
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(first)
		{
			if(results.nextTest.Equals("Potential"))
			{
				path=path2;
				cont.setTeams ("Potential", "Base");
			}
			else if(results.nextTest.Equals ("First"))
			{
				path=path1;
				cont.setTeams ("First", "Base");
			}
			first = false;
		}
	}

	private void writeResults(string toWrite)
	{
		//Potential Fields
		//Base AI
		if(!System.IO.File.Exists (path))
		{
			using(System.IO.FileStream fs = System.IO.File.Create (path))
			{}
		}
		using(StreamWriter sw = File.AppendText(path))
		{
			sw.WriteLine(toWrite);
			sw.WriteLine ("Red won: " + ((redWins / (redWins+blueWins))*100.0f)+"% Blue won: "+((blueWins / (redWins+blueWins))*100.0f)+"%");
		}
		using(StreamWriter newTask = new StreamWriter(masterPath, false))
		{
			if(results.nextTest.Equals("Potential"))
				newTask.WriteLine("First");
			else if(results.nextTest.Equals ("First"))
				newTask.WriteLine ("Potential");
			newTask.WriteLine ("Run Count: " + (results.testsRun+1));
		}
	}

	public void troopWin(string color)
	{
		if(color.Equals("blue"))
		   blueWins+=1;
		else if(color.Equals("red"))
			redWins+=1;
	}

	private Results readMaster()
	{
		if(!System.IO.File.Exists (masterPath))
		{
			using(System.IO.FileStream fs = System.IO.File.Create (masterPath))
			{
				return new Results();
			}
		}
		else
		{
			string[] lines = System.IO.File.ReadAllLines(masterPath);
			string[] temp = lines[1].Split (new Char[] {':'});
			lines[1] = temp[1];
			return new Results(lines);
		}
	}

	public void addWin(string winAI, string lossAI, int winBought, int lossBought, string winColor)
	{
		Debug.Log (winAI);
		string[] temp = winAI.Split (new Char[] {'('});
		temp = temp[1].Split(new Char[] {')'});
		string winFinal = temp [0];
		temp = lossAI.Split (new Char[] {'('});
		temp = temp[1].Split(new Char[] {')'});
		string lossFinal = temp [0];
		string toPrint = "W: " + winFinal + " L: " + lossFinal + " Time: " + (Time.time - timer) + " Winner Bought: " + winBought + " Winner Deaths: " + (winColor.Equals("Red") ? redDeaths : blueDeaths) + " Loser Bought: " + lossBought + " Loser Deaths: " + (winColor.Equals("Red") ? blueDeaths : redDeaths);
		writeResults (toPrint);
		Application.LoadLevel ("Map2");
	}

	public void addDeath(string color)
	{
		if(color.Equals("blue"))
			blueDeaths+=1;
		else if(color.Equals("red"))
			redDeaths+=1;
	}

}
