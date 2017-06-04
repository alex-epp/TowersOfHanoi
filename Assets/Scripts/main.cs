using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class main : MonoBehaviour
{
	public Transform[] towers;
	public int torusCount;

	private float percentsPerSecond = 1f;
	private float currentPathPercent = 0.0f;
	private Vector3[] path;
	private GameObject target;
	private List<Vector3> bottoms;
	private List<Vector3> tops;
	private List<Vector2> solution;
	private int solutionPosition;
	private Stack<GameObject>[] towerStacks;

	private static Color[] colArray = new Color[] {Color.red, Color.yellow, Color.green, Color.blue, Color.black};

	void GenerateSolution()
	{
		solution = new List<Vector2> ();
		RecursiveSolve (0, 2, 1, torusCount);
	}

	void RecursiveSolve(int f, int t, int s, int count)
	{
		if (count < 1)
			return; // Shouldn't happen unless the initial RecursiveSolve was called wrong
		if (count == 1)
		{
			solution.Add (new Vector2 (f, t));
			return;
		}
		// move from 'from' to 'spare'
		// since there are 'count' things to be moved in total, this step removes count-1
		RecursiveSolve(f, s, t, count-1);
		// move from 'from' to 'to'
		solution.Add(new Vector2(f, t));
		// move from 'spare' to 'to'
		RecursiveSolve(s, t, f, count-1);
	}

	List<GameObject> GenerateTori()
	{
		List<GameObject> tori = new List<GameObject> ();
		for (int i = 0; i < torusCount; i++)
		{
			float t = (float)i / (torusCount - 1f);
			float scale = Mathf.Lerp (0.8f, 0.6f, t);
			float scaledT = t * (float)(colArray.Length - 2f);
			Color pc = colArray [(int)scaledT];
			Color nc = colArray [(int)(scaledT + 1f)];
			float newt = scaledT - (float)((int)scaledT);
			Color col = Color.Lerp (pc, nc, newt);
			Vector3 pos = bottoms [0];
			pos.y += 0.3f * i;
			GameObject torus = (GameObject)Instantiate (Resources.Load ("torus"), pos, new Quaternion());
			torus.transform.Find ("default").GetComponent<MeshRenderer> ().material.color = col;
			torus.transform.localScale = new Vector3(scale, 1, scale);
			tori.Add (torus);
		}
		return tori;
	}

	// Use this for initialization
	void Start ()
	{
		if (PlayerPrefs.HasKey ("NumRings"))
			torusCount = Mathf.Max (1, PlayerPrefs.GetInt ("NumRings"));
		else
			PlayerPrefs.SetInt ("NumRings", torusCount);

		bottoms = new List<Vector3> ();
		tops = new List<Vector3> ();
		foreach (Transform t in towers)
		{
			bottoms.Add (new Vector3 (t.position.x, t.position.y - 1.7f, t.position.z));
			tops.Add (new Vector3 (t.position.x, t.position.y + 2.3f, t.position.z));
		}

		towerStacks = new Stack<GameObject>[] { new Stack<GameObject> (), new Stack<GameObject> (),  new Stack<GameObject> () };
		List<GameObject> tori = GenerateTori ();
		foreach (GameObject t in tori)
			towerStacks[0].Push (t);
		
		GenerateSolution ();
		solutionPosition = 0;
		setupSolutionChunk ();
	}

	void setupSolutionChunk()
	{
		int start = (int)solution [solutionPosition].x;
		int end = (int)solution [solutionPosition].y;
		target = towerStacks [start].Pop ();
		towerStacks [end].Push (target);
		path = new Vector3[] { target.transform.position, tops [start], tops [end],
			new Vector3(bottoms[end].x, bottoms[end].y + 0.3f * (towerStacks[end].Count-1), bottoms[end].z) };
		currentPathPercent = 0;
	}

	void Update ()
	{
		if (solutionPosition < solution.Count)
		{
			currentPathPercent += percentsPerSecond * Time.deltaTime;
			iTween.PutOnPath (target, path, Mathf.Clamp01 (currentPathPercent));
			if (currentPathPercent >= 1)
			{
				solutionPosition++;
				if (solutionPosition < solution.Count)
					setupSolutionChunk ();
			}
		}
	}

	void OnDrawGizmos()
	{
		if (path != null)
			//Visual. Not used in movement
			iTween.DrawPath(path);
	}
}
