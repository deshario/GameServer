using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using SocketIO;

public class OtherPlayer : MonoBehaviour 
{
	public string Name ;
	public string ID;
	public string MyColor;
	public Vector3 startPosition;
	public Vector3 currentPosition = Vector3.zero;
	private SocketIOComponent socket;
	public Renderer rend;

	void Start () 
	{
		GameObject go = GameObject.Find("SocketIO");
		socket = go.GetComponent<SocketIOComponent>();
		rend = GetComponent<Renderer>();
	}

	void Update () 
	{
		switch (MyColor) 
		{
		case "red": { rend.material.color = Color.red;
		}break;
		case "blue": { rend.material.color = Color.blue;
		}break;
		case "green": { rend.material.color = Color.green;
		}break;
		}

		gameObject.name = Name;
		if (startPosition != Vector3.zero) 
		{
			currentPosition = startPosition;
			startPosition = Vector3.zero;
			transform.position = Vector3.Lerp (transform.position, currentPosition, 50 * Time.deltaTime);
		}
		else
		{
			transform.position = Vector3.Lerp (transform.position, currentPosition, 50 * Time.deltaTime);
		}
	}
}
