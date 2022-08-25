using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using UnityEngine;
using LogManger = Assets.Scripts.MapEditor.Log.LogManager;

namespace Assets.Scripts.MapEditor
{
    public class LoadManager
    {
        #region Singleton

        private static LoadManager _instance;

        public static LoadManager GetInstance()
        {
            if (_instance == null)
            {
                _instance = new LoadManager();
            }

            return _instance;
        }
        #endregion

        public void Load(string loadFileName ,Action callBackFunc)
        {
            string path = Application.streamingAssetsPath + "/Save/" + loadFileName;
            string jsonData = File.ReadAllText(path);
            SaveData saveData = JsonConvert.DeserializeObject<SaveData>(jsonData);
            
            LoadNode(saveData);
            LoadLine(saveData);
            SetNodeLine();
            LoadEtc(saveData);
            callBackFunc();
            SaveManager.GetInstance().SetRecentSaveData(saveData);
            ViewInfoModeManager.GetInstance().UpdateInfo();
            LogManger.GetInstance().Log("Load   " + loadFileName);
        }

        private void LoadNode(SaveData saveData)
        {
            List<NodeJsonData> nodeJsonDataList = saveData.nodeJsonData;

            foreach (var nodeJsonData in nodeJsonDataList)
            {
                NodeManager.GetInstance().MakeNewNode(nodeJsonData.id,
                    new Vector3(nodeJsonData.position[0], nodeJsonData.position[1], 0),nodeJsonData.nodeData);
            }
        }

        private void LoadLine(SaveData saveData)
        {
            List<LineJsonData> lineJsonDataList = saveData.lineJsonData;

            foreach (var line in lineJsonDataList)
            {
                LineManager.GetInstance().MakeLine(line.id,line.connectedNode);
            }
        }

        private void SetNodeLine()
        {
            List<Node> nodeList = NodeManager.GetInstance().GetAllNode();
            foreach (var node in nodeList)
            {
                List<Line> connectedLineList = LineManager.GetInstance().GetAllLineConnectWithNode(node);
                foreach (var line in connectedLineList)
                {
                    node.lineList.Add(line);
                }
            }
        }

        private void LoadEtc(SaveData saveData)
        {
            EtcJsonData etcJsonData = saveData.etcJsonData;
            NodeManager.GetInstance().nodeCount = etcJsonData.nodeCount;
            LineManager.GetInstance().lineCount = etcJsonData.lineCount;
        }
    }
}
