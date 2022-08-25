using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor
{
    public class ViewInfoPrefabManager : MonoBehaviour
    {
        [SerializeField] private GameObject textField;

        public void SetInfo(Node node)
        {
            string stringData = "";
            stringData += "Node Id : " + node.id +"\n";
            
            var position = node.gameObject.transform.position;
            stringData += "X : " + position.x.ToString("0.00") +  " /  Y : " + position.y.ToString("0.00")+  "\n";
           
            var fields = typeof(NodeData).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                stringData += field.Name;
                stringData += " : ";
                if (field.GetValue(node.data) != null)
                {
                    stringData += field.GetValue(node.data).ToString();
                }
                if (field != fields.Last())
                {
                    stringData += "\n";
                }
            }

            textField.GetComponent<Text>().text = stringData;
            SetPosition(node);
        }
        
        public void SetInfo(Line line)
        {
            string stringData = "";
            stringData += "Line Id : " + line.lineId +"\n";
            
            var fields = typeof(LineData).GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var field in fields)
            {
                stringData += field.Name;
                stringData += " : ";
                if (field.GetValue(line.data) != null)
                {
                    stringData += field.GetValue(line.data).ToString();
                }
                if (field != fields.Last())
                {
                    stringData += "\n";
                }
            }
            
            textField.GetComponent<Text>().text = stringData;

            SetPosition(line);
        }

        private void SetPosition(Node node)
        {
            this.transform.position = node.gameObject.transform.position;
            this.transform.GetComponent<RectTransform>().anchoredPosition += new Vector2(0,130);
        }

        private void SetPosition(Line line)
        {
            var position01 = line.nodeList[0].gameObject.transform.position;
            var position02 = line.nodeList[1].gameObject.transform.position;

            var position = new Vector3((position01.x + position02.x) / 2, (position01.y + position02.y) / 2, 0);
            this.transform.position = position;
        }
    }
}
