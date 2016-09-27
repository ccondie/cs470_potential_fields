using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotController : MonoBehaviour {

	public float speed;

	//Used to keep track of "forward direction" in manual control
	float rotationAngle = 0;
	//Indicates which way is "forward" in manual control
	GameObject followArrow;
	public RunType runtype; 
	public enum RunType{one, two, three};

	// Use this for initialization
	void Start () {
		followArrow = GameObject.FindObjectOfType<ObjectFollow> ().gameObject;
	}


	float vec_length(Vector3 v) {
		return Mathf.Sqrt (Mathf.Pow (v [0], 2) + Mathf.Pow (v [1], 2) + Mathf.Pow (v [1], 2));
	}
	// Update is called once per frame
	void Update () {

		Vector3 toMove = getManualInput ();
	
		//Alter the toMove variable here based on the varius fields in the scene.
		//You can set the runtype in Unity in the Inspector Window
		switch (runtype) {
		case RunType.one:
			//For Part 1 of the lab
			List<Field> att_fields = getFieldofType (1);
			List<Field> rep_fields = getFieldofType (2);

			Vector3 my_pos = myLocation ();

			Field goal = att_fields [0];
			Vector3 goal_pos = goal.getLocation ();
			float goal_dist = getDistance (goal);

			float exp_shift = (float)1;
			float goal_scalar = (float)1 + (1 / (goal_dist - exp_shift));

			Vector3 move_force = goal_scalar * ((goal_pos - my_pos) / vec_length (goal_pos - my_pos));

			float counter_speed_radius = goal.getRadius () / 2;
			if (goal_dist < goal.getRadius ()) {
				move_force = move_force - (counter_speed_radius / goal_dist) * myVelocity ();
			}


			// apply the influence of the repulsive fields, by unit vector and scalar
			for (int i = 0; i < rep_fields.Count; i++) {
				Field cur_rep = rep_fields [i];
				Vector3 cur_pos = cur_rep.getLocation ();
				float cur_dist = getDistance (cur_rep);
				float cur_scalar = 0;

				if (cur_dist < cur_rep.getRadius ()) {
					cur_scalar = (1 / (cur_dist - exp_shift));
				} else {
					cur_scalar = 0;
				}

				move_force += cur_scalar * ((my_pos - cur_pos) / vec_length (my_pos - cur_pos));
			}
			move (move_force);


			break;
		case RunType.two:
			//For Part 2 of the lab
			//	readSonars ();
			break;

		case RunType.three:
			//For whatever else
			//feel free to add more 

			break;

		}

	
		move (toMove);


	}


	// add a force to me in the given direction, this input force is normalized (made to have a magnitude of 1) for your convenience
	// you can control its total acceleration by altering the speed if you like.
	public void move(Vector3 direction)
	{
		this.GetComponent<Rigidbody>().AddForce( direction.normalized * Time.deltaTime * speed);

	}



	// Returns a List of 16 floats of the distance to a given object, the first float is point in the north direction,
	//and then they increment clockwise going around the circle. if nothing is detected, it will be a number of size 25
	// Example, an object to the north would be in the 0 index, a object to the east (90 degrees) would be in the 4th index and so on
	public List<float> readSonars() {

		List<float> myContacts = new List<float> ();
		//Vector3 loc = myLocation ();
		Vector3 fwd = new Vector3(1,0,0);

		for (int i = 0; i < 16; i++) {

			RaycastHit hitobj ;
			float distance = 20;
			if (Physics.Raycast ( transform.position, fwd, out hitobj, 25)) {
				Vector3 hitlocation = hitobj.point;
				distance = Vector3.Distance (hitlocation, transform.position);

				Debug.Log (" hit at "+ (360 / 16) *(i));

			}
			myContacts.Add (distance);
			//Rotate the raycast by 22ish degrees.
			fwd = Quaternion.AngleAxis ((360 / 16), Vector3.up) * fwd;
		

		}
		return myContacts;



	}

	//Distance between me and a given field
	public float getDistance(Field f)
	{
		return Vector3.Distance(f.getLocation(), myLocation());
	}


	public Vector3 myLocation()
	{
		return this.transform.position;
	}

	public Vector3 myVelocity(){
		return this.GetComponent<Rigidbody> ().velocity;
	}




	//Get all Fields.
	public List<Field> getPowerFields()
	{List<Field> myList = new List<Field> ();
		foreach (Field f in GameObject.FindObjectsOfType<Field> ()) {
			myList.Add (f);

		}
		return myList;
	}

	//Only get fields that match the n fieldType in range of the robot .
	public List<Field> getFieldsInRange(int n)
	{
		List<Field> myList = new List<Field> ();
		foreach (Field f in GameObject.FindObjectsOfType<Field> ()) {
			if (f.getFieldType() == n  && Vector3.Distance(myLocation(), f.getLocation()) <= f.getRadius()) {
				myList.Add (f);			
			}
		}

		return myList;

	}
	//get all fields in range of the robot.
	public List<Field> getFieldsInRange()
	{
		List<Field> myList = new List<Field> ();
		foreach (Field f in GameObject.FindObjectsOfType<Field> ()) {
			if (Vector3.Distance(myLocation(), f.getLocation()) <= f.getRadius()) {
				myList.Add (f);			
			}
		}

		return myList;

	}
	//get all fields that match this type.
	public List<Field> getFieldofType(int n)
	{
		List<Field> myList = new List<Field> ();
		foreach (Field f in GameObject.FindObjectsOfType<Field> ()) {
			if (f.getFieldType() == n) {
				myList.Add (f);			
			}
		}

		return myList;

	}


	// get input from the AWSD keys.
	public Vector3 getManualInput()
	{Vector3 MoveDirection = Vector3.zero;
		if (Input.GetKey (KeyCode.W)) {
			MoveDirection += (Quaternion.Euler(0,rotationAngle,0)* Vector3.forward * Time.deltaTime * speed/2);
		}

		else if (Input.GetKey (KeyCode.S)) {
			MoveDirection += (Quaternion.Euler(0,rotationAngle,0)* Vector3.forward * Time.deltaTime * -speed/2);
		}

		if (Input.GetKey (KeyCode.D)) {
			rotationAngle += 180 * Time.deltaTime;
		}
		else if (Input.GetKey (KeyCode.A)) {
			rotationAngle -= 180 * Time.deltaTime;
		}
		followArrow.transform.rotation = Quaternion.Euler (0, rotationAngle -90, 0);

		return MoveDirection;
	}


}
