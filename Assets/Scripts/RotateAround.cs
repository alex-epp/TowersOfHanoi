using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAround : MonoBehaviour
{
	public float slowRadialSpeed = .001f;
	public float fastRadialSpeed = .01f;
	public float radius = 10;
	public Transform target;
	public float angle = -Mathf.PI / 2;

	private Vector3 vt;

	// Use this for initialization
	void Start ()
	{
		vt = target.transform.position;
		transform.position = new Vector3 (Mathf.Cos(angle)*radius+vt.x, vt.y+2, Mathf.Sin(angle)*radius+vt.z);
	}

	float Interpolate(float a, float b, float t)
	{
		return Mathf.Lerp (a, b, t*t*t);
	}

	// Update is called once per frame
	void Update ()
	{
		angle += Interpolate(slowRadialSpeed, fastRadialSpeed, Mathf.Abs(Mathf.Cos(angle)));
		transform.position = new Vector3 (Mathf.Cos(angle)*radius+vt.x, vt.y+2, Mathf.Sin(angle)*radius+vt.z);
		transform.rotation = Quaternion.LookRotation (vt - transform.position);
	}
}
