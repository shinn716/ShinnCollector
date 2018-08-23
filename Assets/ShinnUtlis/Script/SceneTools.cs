using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace ShinnUtil
{
    public class SceneTools : MonoBehaviour
    {

        static SceneTools s_Instance;

        [Header("Reload")]
        public KeyCode ReloadKey = KeyCode.F5;
        public int level = 0;

        [Header("Screen Resolutuin setting")]
        public Vector2 ScreenResolution = new Vector2(1920, 281);
        public bool FullScreen = true;
        public bool FitToScreen = false;

        [Header("Show sys info")]
        public bool showFPS = false;
        public bool showTime = false;

        private float updateInterval = 0.5f;
        private float accum = 0.0f;
        private int frames = 0;
        private float timeleft;
        private string fps;

        [Header("Text style")]
        public GUIStyle myStyle;


        public bool ShowFPS
        {
            get { return showFPS; }
            set { showFPS = value; }
        }

        public bool ShowTime
        {
            get { return showTime; }
            set { showTime = value; }
        }

        private void Awake()
        {
            if (s_Instance == null)
            {
                s_Instance = this;
                DontDestroyOnLoad(gameObject);
            }

            else if (this != s_Instance)
            {
                Destroy(gameObject);
            }


            /// Screen Setting
            /// 

            if (FitToScreen)
            {
                Debug.Log("Resolution: " + ScreenResolution.x + " x " + ScreenResolution.y + " FullScreen: " + FullScreen);
                Screen.SetResolution((int)ScreenResolution.x, (int)ScreenResolution.y, FullScreen);
            }
        }

        void Start()
        {
            timeleft = updateInterval;
        }

        void Update()
        {
            if (Input.GetKeyDown(ReloadKey))
                Application.LoadLevel(level);


            ///Show FPS
            ///

            if (showFPS)
            {
                timeleft -= Time.deltaTime;
                accum += Time.timeScale / Time.deltaTime;
                ++frames;

                if (timeleft <= 0.0)
                {
                    fps = "" + (accum / frames).ToString("f0");
                    timeleft = updateInterval;
                    accum = 0.0f;
                    frames = 0;
                }
            }

        }


        private void OnGUI()
        {
            if (showFPS)
                GUI.Label(new Rect(Screen.width - 100, 0, 70, 20), "FPS  " + fps, myStyle);

            if (showTime)
                GUI.Label(new Rect(Screen.width - 100, 20, 70, 60), "Time " + Time.time.ToString("f0"), myStyle);
        }
    }



}
