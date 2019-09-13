﻿using System.Collections.Generic;
using UnityEngine;
using Shinn.CameraTools;
using Shinn.Commom;

public class test : MonoBehaviour
{
    private MouseController mouseController;
    private LoadXml loadXml_Email;
    private UDPServer server;
    private UDPClient client;
    private CsvTools csvTools;

    void Start()
    {
        mouseController = new MouseController();
        mouseController.Init(this);

        loadXml_Email = new LoadXml(new List<string> { "SMTP_Client", "SMTP_Port", "USER", "USER_Pass", "To", "Subject", "Body", "AttachFile" });
        loadXml_Email.Load(ShUnityPath.ApplicationStreamingAssetsPath, "EmailSetting.xml");

        server = new UDPServer();
        server.Init();

        client = new UDPClient();

        LoadFile.LoadAllFiles(ShUnityPath.ApplicationStreamingAssetsPath, ShExtension.XML);


        string[] title = { "A", "B", "C", "D" };
        csvTools = new CsvTools(title);
    }

    // Update is called once per frame
    void Update()
    {
        mouseController.Loop();
    }


    private void OnApplicationQuit()
    {
        mouseController.Dispose();
        loadXml_Email.Dispose();
        server.Dispose();
        client.Dispose();
        csvTools.Dispose();
    }

    [ContextMenu("Test_ClientSocket")]
    private void Test_ClientSocket()
    {
        client.SendData("Hello");
    }

    [ContextMenu("Test_WriteToCsv")]
    private void Test_WriteToCsv()
    {
        csvTools.WriteToCsv();
    }

}
