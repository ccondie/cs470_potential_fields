using UnityEngine;
using System.Collections;

public class Field : MonoBehaviour {

	public int fieldType;
	public float radius;
	public bool showField;
	private Vector3 location;

	// Use this for initialization
	void Start () {
		location = this.gameObject.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public int getFieldType()
	{return fieldType;
	}

	public float getRadius()
	{return radius;
	}



	public Vector3 getLocation()
	{return location;
	}



	void OnDrawGizmos()
	{
		if (showField) {
			Gizmos.DrawWireSphere (this.gameObject.transform.position, radius);
		}
	}


	void OnCollisionEnter(Collision col)
	{
		if (col.gameObject.GetComponent<RobotController>()) {
			foreach (Wallcounter wc in GameObject.FindObjectsOfType<Wallcounter> ()) {
				wc.incrementCounter ();
			}
		}
	}

}

