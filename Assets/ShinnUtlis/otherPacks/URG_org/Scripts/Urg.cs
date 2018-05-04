﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using URG;

// Official Documents of URG Sensor
// http://sourceforge.net/p/urgnetwork/wiki/top_jp/
// https://www.hokuyo-aut.co.jp/02sensor/07scanner/download/pdf/URG_SCIP20.pdf

class UrgMesh
{
    public List<Vector3> VertexList { get; private set; }
    public List<Vector2> UVList { get; private set; }
    public List<int> IndexList { get; private set; }

    public UrgMesh()
    {
        VertexList = new List<Vector3>();
        UVList = new List<Vector2>();
        IndexList = new List<int>();
    }

    public void Clear()
    {
        VertexList.Clear();
        UVList.Clear();
        IndexList.Clear();
    }

    public void AddVertex(Vector3 pos)
    {
        VertexList.Add(pos);
    }

    public void AddUv(Vector2 uv)
    {
        UVList.Add(uv);
    }

    public void AddIndices(int[] indices)
    {
        IndexList.AddRange(indices);
    }
}

public class Urg : MonoBehaviour
{
    #region Device Config
    [SerializeField]
    URGDevice urg;

    [SerializeField]
    string ipAddress = "192.168.0.35";

    [SerializeField]
    int portNumber = 10940;

    [SerializeField]
    string portName = "COM3";

    [SerializeField]
    int baudRate = 115200;

    [SerializeField]
    bool useEthernetTypeURG = true;

    int urgStartStep;
    int urgEndStep;

	public static int _startstep = 460;
	public static int _endstep   = 620;


	float detectRange=100;
	public float DetectRange
	{
		get { return detectRange; }
		set {detectRange = value; }
	}

    #endregion

    #region Debug

	Vector3 _scale = new Vector3 (0.05f, 0.05f, 1);
	public Vector3 _Scale
    {
        get { return _scale; }
        set
        {
                _scale = value;
        }
    }

	float rotate = 0;
	public float Rotate
	{
		get { return rotate; }
		set
		{
			//if (value > 0)
				rotate = value;
		}
	}

    Vector3 posOffset = Vector3.zero;
    public Vector3 PosOffset
    {
        get { return posOffset; }
        set { posOffset = value;  }
    }

    bool drawMesh = true;
    public bool DrawMesh {
        get { return drawMesh; }
        set { drawMesh = value; }
    }

    #endregion

    #region Mesh
    UrgMesh urgMesh;
    MeshFilter meshFilter;
    MeshRenderer meshRenderer;
    Mesh mesh;
    #endregion

    [SerializeField]

    long[] distances;

    public Vector4[] DetectedObstacles { get; private set; }
	List<Vector3> DetectSolution = new List<Vector3>();

    public bool IsConnected { get { return urg.IsConnected; } }

	bool ScriptStart=false;
	public GameObject UST_POS;

	public SceneController main;

	bool workOnce = false;

	//public bool enableTracking = true;

	public void init()

    {

		if (!main.GetDebugMode ()) {
			
			if (useEthernetTypeURG) {
				urg = new EthernetURG (ipAddress, portNumber);
			} else {
				urg = new SerialURG (portName, baudRate);
			}

			urg.Open ();

			urgStartStep = _startstep;
			urgEndStep = _endstep;

			distances = new long[urgEndStep - urgStartStep + 1];

			meshFilter = GetComponent<MeshFilter> ();
			meshRenderer = GetComponent<MeshRenderer> ();
			mesh = new Mesh ();
			urgMesh = new UrgMesh ();
			DetectedObstacles = new Vector4[urgEndStep - urgStartStep + 1];

			ScriptStart = true;


			//----UST ERROR
			/*if(distances.Length==0){
				init ();
			}*/
		}

    }
		
    void Update()
    {

		if (!main.GetDebugMode() ) {

			if(distances.Length==0 && !workOnce){
				print ("Need Reset");
				workOnce = true;
				Reset ();
				StartCoroutine (ResetFuct (5));
			}


			if (ScriptStart) {
				if (urg.Distances.Count () == distances.Length) {
					
					try {
						distances = urg.Distances.ToArray ();
					} catch (Exception e) {
						Debug.LogException (e, this);
					}

				}

				//UpdateObstacleData ();
				meshRenderer.enabled = drawMesh;

				if (drawMesh) {
					CreateMesh ();
					ApplyMesh ();
					UST_POS.transform.position = new Vector3 (PosOffset.x, PosOffset.y, -20);
				}
			}

		}
    }

    void UpdateObstacleData()
    {
		
        for ( int i = 0; i < distances.Length; i++)
        {
			
			Vector3 position = new Vector3(_scale.x*Index2Position(i).x + PosOffset.x,  _scale.y * Index2Position(i).y + PosOffset.y, 1);

            DetectedObstacles[i] = new Vector4(position.x, position.y, position.z, distances[i]);

			if (position.x > SceneController.touchArea_value.x && position.x < SceneController.touchArea_value.y && position.y < SceneController.touchArea_value.z && position.y > SceneController.touchArea_value.w) {
				DetectSolution.Add (new Vector3 (position.x, position.y, position.z));
			}
				
		
        }





		for(int i=0; i<DetectSolution.Count; i++){

			RaycastHit hit;
			Vector3 _screenPos = Camera.main.WorldToScreenPoint (DetectSolution [i]);
			Ray ray = Camera.main.ScreenPointToRay (_screenPos);

			if(Physics.Raycast(ray, out hit, 100))
			{
				Debug.DrawLine (Camera.main.transform.position, hit.transform.position, Color.red, .1f, true);
				if(hit.collider.gameObject.transform.tag=="trigger")
					hit.transform.gameObject.SendMessage ("CubeStart");
			
			}
		}
			

		DetectSolution.Clear ();
			

    }

	void FixedUpdate(){
		//if (!main.GetDebugMode () && enableTracking) {
		if (!main.GetDebugMode ()) {
			UpdateObstacleData ();
		}
	}

    static bool IsValidDistance(long distance)
    {
        return distance >= 21 && distance <= 30000;
    }

    bool IsOffScreen(Vector3 worldPosition)
    {
        Vector3 viewPos = Camera.main.WorldToViewportPoint(worldPosition);
        return (viewPos.x < 0 || viewPos.x > 1 || viewPos.y < 0 || viewPos.y > 1);
    }

    float Index2Rad(int index)
    {
        float step = 2 * Mathf.PI / urg.StepCount360;
        float offset = step * (urg.EndStep + urg.StartStep) / 2;
        return step * index + offset;
    }

    Vector3 Index2Position(int index)
    {
		float radius = rotate * Mathf.Deg2Rad;
		return new Vector3( distances[index]*Mathf.Cos(Index2Rad(index + urgStartStep)+radius )-distances[index]*Mathf.Sin(Index2Rad(index + urgStartStep)+radius ), 
							distances[index]*Mathf.Sin(Index2Rad(index + urgStartStep)+radius )+distances[index]*Mathf.Cos(Index2Rad(index + urgStartStep)+radius ), 0);
    }

    void CreateMesh()
    {
        urgMesh.Clear();
        urgMesh.AddVertex(PosOffset);
        urgMesh.AddUv(Camera.main.WorldToViewportPoint(PosOffset));

        for (int i = distances.Length - 1; i >= 0; i--)
        {

            //urgMesh.AddVertex(scale * Index2Position(i) + PosOffset);
            //urgMesh.AddUv(Camera.main.WorldToViewportPoint(scale * Index2Position(i) + PosOffset));


			urgMesh.AddVertex( new Vector3(_scale.x*Index2Position(i).x + PosOffset.x,  _scale.y * Index2Position(i).y + PosOffset.y, 1) );
			urgMesh.AddUv(Camera.main.WorldToViewportPoint( new Vector3(_scale.x*Index2Position(i).x + PosOffset.x,  _scale.y * Index2Position(i).y + PosOffset.y, 1)));
        }
        for (int i = 0; i < distances.Length - 1; i++)
        {
            urgMesh.AddIndices(new int[] { 0, i + 1, i + 2 });
        }
    }

    void ApplyMesh()
    {
        mesh.Clear();
        mesh.name = "URG Data";
        mesh.vertices = urgMesh.VertexList.ToArray();
        mesh.uv = urgMesh.UVList.ToArray();
        mesh.triangles = urgMesh.IndexList.ToArray();
        meshFilter.sharedMesh = mesh;
    }


	void Reset(){
		Disconnect ();
		urg.Close ();


		init ();
		Connect ();
	}


	IEnumerator ResetFuct(float delay){
		yield return new WaitForSeconds (delay);
		workOnce = false;
	}

    public void Connect()
    {
        urg.Write(SCIP_library.SCIP_Writer.MD(urgStartStep, urgEndStep, 1, 0, 0));
    }

    public void Disconnect()
    {
        urg.Write(SCIP_library.SCIP_Writer.QT());
    }

	void OnApplicationQuit(){
	
		if (!main.debugMode) {
			urg.Write (SCIP_library.SCIP_Writer.QT ());
			urg.Close ();
		}

	}

	void OnDestroy(){
		
		if (!main.debugMode) {
			urg.Write (SCIP_library.SCIP_Writer.QT ());
			urg.Close ();
		}

	}


}