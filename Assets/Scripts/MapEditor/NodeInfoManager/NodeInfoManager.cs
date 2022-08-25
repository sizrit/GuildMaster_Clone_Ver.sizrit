using TMPro;
using UnityEngine;
using LogManager = Assets.Scripts.MapEditor.Log.LogManager;

namespace Assets.Scripts.MapEditor.NodeInfoManager
{
    public class NodeInfoManager : MonoBehaviour,IObjectInfo
    {
        [SerializeField] private GameObject idText;
        [SerializeField] private GameObject nameInputField;
        [SerializeField] private GameObject xInputField;
        [SerializeField] private GameObject yInputField;
        [SerializeField] private GameObject etcInputField;
        [SerializeField] private GameObject apply;
        
        private Node currentNode;

        public void SetInfo()
        {
            currentNode = NodeManipulator.GetInstance().GetSelectedNode();
            
            idText.GetComponent<TextMeshProUGUI>().SetText(currentNode.id);
            
            nameInputField.GetComponent<TMP_InputField>().text =currentNode.data.name;
            
            var position = currentNode.gameObject.transform.position;
            xInputField.GetComponent<TMP_InputField>().text = position.x.ToString("0.00");
            yInputField.GetComponent<TMP_InputField>().text = position.y.ToString("0.00");
            
            etcInputField.GetComponent<TMP_InputField>().text =currentNode.data.etc;
        }
        
        public void Click(RaycastHit2D[] hits)
        {
            foreach (var hit in hits)
            {
                if (hit.transform == apply.transform)
                {
                    Apply();
                }
            }
        }

        public void CloseInfo()
        {
            Destroy(this.gameObject);
        }

        public void UpdatePositionData()
        {
            var position = currentNode.gameObject.transform.position;
            xInputField.GetComponent<TMP_InputField>().text = position.x.ToString("0.00");
            yInputField.GetComponent<TMP_InputField>().text = position.y.ToString("0.00");
        }
        
        private void Apply()
        {
            currentNode.data.name = nameInputField.GetComponent<TMP_InputField>().text;
            
            Vector3 newPosition = new Vector3(
                float.Parse(xInputField.GetComponent<TMP_InputField>().text),
                float.Parse(yInputField.GetComponent<TMP_InputField>().text), 0);
            
            NodeManipulator.GetInstance().UpdateNodePosition(currentNode, newPosition);
            NodeCollisionManager.GetInstance().NodeCollisionCheck(currentNode);
            
            var position = currentNode.gameObject.transform.position;
            
            xInputField.GetComponent<TMP_InputField>().text = position.x.ToString("0.00");
            yInputField.GetComponent<TMP_InputField>().text = position.y.ToString("0.00");
            
            currentNode.data.etc = etcInputField.GetComponent<TMP_InputField>().text;
            
            LogManager.GetInstance().Log("Apply Node Data [Node Id : " + currentNode.id + "]");
        }
    }
}
