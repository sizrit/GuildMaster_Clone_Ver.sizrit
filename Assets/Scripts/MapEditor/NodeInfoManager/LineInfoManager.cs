using TMPro;
using UnityEngine;

namespace Assets.Scripts.MapEditor.NodeInfoManager
{
    public class LineInfoManager : MonoBehaviour, IObjectInfo
    {
        [SerializeField] private GameObject idText;
        [SerializeField] private GameObject weightInputField;
        [SerializeField] private GameObject etcInputField;
        [SerializeField] private GameObject apply;
        
        private Line currentLine;
        
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

        private void Apply()
        {
            currentLine.data.weight = int.Parse(weightInputField.GetComponent<TMP_InputField>().text);
            currentLine.data.etc = etcInputField.GetComponent<TMP_InputField>().text;
        }

        public void SetInfo()
        {
            currentLine = LineManipulator.GetInstance().GetSelectedLine();

            idText.GetComponent<TextMeshProUGUI>().text = currentLine.lineId;
            weightInputField.GetComponent<TMP_InputField>().text = currentLine.data.weight.ToString();
            etcInputField.GetComponent<TMP_InputField>().text = currentLine.data.etc;
        }

        public void CloseInfo()
        {
            Destroy(this.gameObject);
        }
    }
}
