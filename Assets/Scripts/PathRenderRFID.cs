using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class PathRenderRFID : MonoBehaviour {

	// data to be rendered
	public Dictionary<string, List<RFIDPoint>> tracks = new Dictionary<string, List<RFIDPoint>>();

	// 模型有一个基础高度
	public float baseHeight = 0.0f;

	// RFID实验所选原点和CAD图中的原点有偏移,单位是像素
	public Vector2 originOffset = new Vector2(548, 21);

	/// <summary>
	/// The students 对象
	/// </summary>
	public GameObject[] students;

	public GameObject line;
	
	GameObject playerParent = null;
	GameObject studentsParent = null;
	GameObject linesParent = null;
	GameObject npcsParent = null;
	SmoothFollow smoothFollow = null;
	
	// Use this for initialization
	void Start () {
		iTweenPath.GetPath("path1");
		playerParent = GameObject.Find("/Player");
		studentsParent = GameObject.Find("/Instances");
		linesParent = GameObject.Find("/Lines");
		npcsParent = GameObject.Find("/NPCs");
		smoothFollow = Camera.main.GetComponent<SmoothFollow>();
		Initialize();
		Render();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	// 各张表的数据读进内存
	void Initialize(){
		SqliteDB db = new SqliteDB();
		db.OpenDB("rfid.db");
		ArrayList tableNames = db.GetTableNames();
		for(int i = 0; i < tableNames.Count; i++){
			//Debug.Log(tableNames[i]);
			string tn = tableNames[i].ToString();
			ArrayList contents = db.ReadFullTable(tn);
			List<RFIDPoint> track = new List<RFIDPoint>();
			for(int row = 0; row < contents.Count; row++){
				ArrayList record = (ArrayList)contents[row];
				RFIDPoint pnt = new RFIDPoint();
				pnt.id = int.Parse(record[0].ToString());
				pnt.coordx = float.Parse(record[1].ToString()) - originOffset.x;
				pnt.coordy = float.Parse(record[2].ToString()) - originOffset.y;
				pnt.alititude = float.Parse(record[3].ToString());
				pnt.time = DateTime.Parse(record[4].ToString());
				track.Add(pnt);
			}
			tracks.Add(tn, track);
		}
		db.CloseDB();
	}

	// 渲染人和路线
	void Render(){
		RenderPlayer();
		RenderPathLines();
		RenderStudents();
	}

	void ReInitialize(){
		StopMoving();
		tracks.Clear();
		Initialize();
		RenderPathLines();
		RenderStudents();
	}
	
	void RenderPlayer(){
		// 0-7 角色对象中前8个是学生
		int idx = (int)Math.Round(UnityEngine.Random.value * 7.0);
		GameObject player = (GameObject)Instantiate(students[idx], 
		                                            playerParent.transform.position, 
		                                            Quaternion.identity);
		player.transform.parent = playerParent.transform;
		player.gameObject.name = "player1";
		smoothFollow.target = player.transform;
	}

	void RenderPathLines(){
		foreach(KeyValuePair<string, List<RFIDPoint>> trk in tracks){
			GameObject lineInstance = (GameObject)Instantiate(line, linesParent.transform.position, 
			                                            Quaternion.identity);
			lineInstance.name = trk.Key;
			lineInstance.transform.parent = linesParent.transform;
			List<RFIDPoint> points = trk.Value;
			LineRenderer lr = lineInstance.GetComponent<LineRenderer>();
			lr.SetVertexCount(points.Count);
			for(int i = 0; i < points.Count; i++){
				Vector3 temPoint = new Vector3(points[i].coordx, baseHeight, points[i].coordy);
				lr.SetPosition(i, temPoint);
			}
		}
	}

	void RenderStudents(){
		int idx = (int)Math.Round(UnityEngine.Random.value * students.Length);
		foreach(KeyValuePair<string, List<RFIDPoint>> trk in tracks){
			List<RFIDPoint> points = trk.Value;
			GameObject student = (GameObject)Instantiate(students[idx],
				new Vector3(points[0].coordx, baseHeight, points[0].coordy), 
			    Quaternion.identity);
			student.name = trk.Key;
			student.transform.parent = studentsParent.transform;
			student.transform.tag = "People";
			student.GetComponent<SuperMarioAnimation>().enabled = false;
			student.GetComponent<SuperMarioController>().enabled = false;
			student.GetComponent<CharacterController>().enabled = false;
		}
	}

	void StartMovingAlongPath(){
		foreach(Transform st in studentsParent.transform){
			List<RFIDPoint> points = tracks[st.gameObject.name];
			Vector3[] path = new Vector3[points.Count];
			double totalTime = 0.0;
			for(int i = 0; i < points.Count; i++){
				if(i != 0){
					TimeSpan ts = points[i].time.Subtract(points[i-1].time);
					totalTime += ts.TotalSeconds;
				}
				path[i] = new Vector3(points[i].coordx, baseHeight, points[i].coordy);
			}
			st.gameObject.animation.wrapMode = WrapMode.Loop;
			st.gameObject.animation.CrossFade("walk");
			iTween.MoveTo(st.gameObject, 
			            iTween.Hash("path",path,
			            "time",totalTime,
			            "orienttopath",true,
			            "looktime",0.2,
			            "lookahead",0.005,
			            "looptype",iTween.LoopType.none,
			            "easetype",iTween.EaseType.linear));
		}

		foreach(Transform st in npcsParent.transform){
			string pathName = st.gameObject.GetComponent<iTweenPath>().pathName;
			Vector3[] path = iTweenPath.GetPath(pathName);
			st.gameObject.animation.wrapMode = WrapMode.Loop;
			st.gameObject.animation.CrossFade("walk");
			iTween.MoveTo(st.gameObject, 
			              iTween.Hash("path",path,
			            "time",15,
			            "orienttopath",true,
			            "looktime",0.2,
			            "lookahead",0.005,
			            "looptype",iTween.LoopType.loop,
			            "easetype",iTween.EaseType.linear));
		}
	}

	void StopMoving(){
		iTween.Stop(studentsParent, true);
		foreach(Transform st in studentsParent.transform){
			st.gameObject.animation.CrossFade("idle");
		}
		iTween.Stop(npcsParent, true);
		foreach(Transform st in npcsParent.transform){
			st.gameObject.animation.CrossFade("idle");
		}
	}
}
