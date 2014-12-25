using UnityEngine;
using System.Collections;
using System.Windows.Forms;
using System.IO;
using System;

[ExecuteInEditMode]
public class SceneGUI : MonoBehaviour {

	public Texture2D texturePlay;
	public Texture2D textureStop;
	public Texture2D textureImportGPS;
	public Texture2D textureImportRFID;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnGUI(){
		GUILayout.BeginVertical("box");
		{
			//控制路径回放的功能
			GUILayout.BeginHorizontal();
			if(GUILayout.Button(texturePlay, GUILayout.Width(48), GUILayout.Height(48))){
				this.gameObject.SendMessage("StartMovingAlongPath");
			}
			if(GUILayout.Button(textureStop, GUILayout.Width(48), GUILayout.Height(48))){
				this.gameObject.SendMessage("StopMoving");
			}
			GUILayout.EndHorizontal();

			//导入数据文件功能
			GUILayout.BeginHorizontal();			
			if(GUILayout.Button(textureImportGPS, GUILayout.Width(48), GUILayout.Height(48))){
				//Camera.main.nearClipPlane = 1.0f;
				OpenFileDialog dlgOpenGPS = new OpenFileDialog();
				dlgOpenGPS.InitialDirectory = "URI=file:" + UnityEngine.Application.dataPath;
				dlgOpenGPS.Filter = "csv files (*.csv)|*.csv";
				//dlgOpenGPS.RestoreDirectory = true ;				
				if (dlgOpenGPS.ShowDialog() == DialogResult.OK)
				{
					//打开文件，写到数据库
					try 
					{
						using (StreamReader sr = new StreamReader(dlgOpenGPS.FileName)) 
						{
							SqliteDB db = new SqliteDB();
							db.OpenDB("data.db");
							string tableName = Utilities.GetValidTableName(dlgOpenGPS.FileName);
							ArrayList cols = new ArrayList();
							cols.Add("id");
							cols.Add("lon");
							cols.Add("lat");
							cols.Add("alt");
							cols.Add("time");
							ArrayList types = new ArrayList();
							types.Add("INTEGER");
							types.Add("FLOAT");
							types.Add("FLOAT");
							types.Add("FLOAT");
							types.Add("TEXT");
							db.CreateTable(tableName, cols, types);
							string line = sr.ReadLine(); // head line
							ArrayList values;
							int index = 1;
							while ((line = sr.ReadLine()) != null) 
							{
								string[] items = line.Split(new char[]{','});
								values = new ArrayList();
								values.Add(index);
								values.Add(float.Parse(items[0]));
								values.Add(float.Parse(items[1]));
								values.Add(float.Parse(items[2]));
								values.Add(DateTime.Parse(items[3]).ToString());
								db.InsertGPSPosition(tableName, values);
								++index;
							}
							db.CloseDB();
						}
					}
					catch (Exception e) 
					{
						Debug.Log(e.Message);
					}
					this.gameObject.SendMessage("ReInitialize");
				}
			}
			//if(GUILayout.Button(textureImportRFID, GUILayout.Width(48), GUILayout.Height(48))){
				
			//}
			if(GUILayout.Button("退出", GUILayout.Width(48), GUILayout.Height(48))){
				UnityEngine.Application.Quit();
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
	}
}
