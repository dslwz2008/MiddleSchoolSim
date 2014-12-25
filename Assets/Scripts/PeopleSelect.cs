using UnityEngine;
using System.Collections;

public class PeopleSelect : MonoBehaviour {

	public GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.Find("/Player/player1");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			RaycastHit hit;
			Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
			if (Physics.Raycast(ray, out hit)){
				//Debug.DrawLine(ray.origin, hit.point);
				//Debug.Log(hit.transform.gameObject.name);
				if(hit.transform.gameObject.tag == "People"){
					Camera.main.GetComponent<SmoothFollow>().target = hit.transform;
				}
			}		
		}

		if(Input.GetMouseButtonDown(1)){
			Camera.main.GetComponent<SmoothFollow>().target = player.transform;
		}

		if(Input.GetKeyDown(KeyCode.Escape)){
			Application.Quit();
		}
	}
}
