using UnityEditor.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Scripts.MapEditor.Log
{
    public class LogUnFoldManager : MonoBehaviour
    {
        [SerializeField] private GameObject box;
        [SerializeField] private GameObject content;
        [SerializeField] private GameObject scrollbar;
        private const float Delta = 0.4f;
        
        public void SetLog(string log)
        {
            content.GetComponent<Text>().text = log;
            content.GetComponent<ContentSizeFitter>().SetLayoutVertical();
            float contentH = content.GetComponent<RectTransform>().rect.height;
            float boxH = box.GetComponent<RectTransform>().rect.height;

            if (contentH > boxH)
            {
                content.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(0,
                        box.GetComponent<RectTransform>().anchoredPosition.y + (contentH / 2f - boxH / 2f));
            }
            else
            {
                content.GetComponent<RectTransform>().anchoredPosition =
                    new Vector2(0,
                        box.GetComponent<RectTransform>().anchoredPosition.y - (contentH / 2f - boxH / 2f));
            }
        }
        
        private bool OnCursorCheck()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit2D[] hits = Physics2D.GetRayIntersectionAll(ray);
            
            foreach (var hit in hits)
            {
                if (hit.transform.CompareTag("MapEditorLog"))
                {
                    return true;
                }
            }

            return false;
        }

        private void ScrollAction()
        {
            float contentH = content.GetComponent<RectTransform>().rect.height;
            float boxH = box.GetComponent<RectTransform>().rect.height;

            if (contentH > boxH)
            {
                if (Input.mouseScrollDelta.y != 0 && OnCursorCheck())
                {
                    content.GetComponent<RectTransform>().position -=
                        Vector3.up * (int)Input.mouseScrollDelta.y * Delta;

                    float cY = content.GetComponent<RectTransform>().anchoredPosition.y;
                    float bY = box.GetComponent<RectTransform>().anchoredPosition.y;
                    float d = cY + bY;

                    if (contentH / 2f - boxH / 2f < Mathf.Abs(d))
                    {
                        if (d < 0)
                        {
                            content.GetComponent<RectTransform>().anchoredPosition =
                                new Vector2(0,
                                    box.GetComponent<RectTransform>().anchoredPosition.y - (contentH / 2f - boxH / 2f));
                        }
                        else
                        {
                            content.GetComponent<RectTransform>().anchoredPosition =
                                new Vector2(0,
                                    box.GetComponent<RectTransform>().anchoredPosition.y + (contentH / 2f - boxH / 2f));
                        }
                    }
                }
            }
        }

        private void ControlScrollBar()
        {
            float contentH = content.GetComponent<RectTransform>().rect.height;
            float boxH = box.GetComponent<RectTransform>().rect.height;
            float contentY = content.GetComponent<RectTransform>().anchoredPosition.y;
            float boxY = box.GetComponent<RectTransform>().anchoredPosition.y;
            
            if (contentH < boxH)
            {
                return;
            }

            Scrollbar scrollbarEditor = scrollbar.GetComponent<Scrollbar>();
            // float max = boxY + boxH/2 - contentH/2; 1
            // float min = boxY - boxH/2 + contentH/2; 0
            float max = contentH - boxH;
            float range = Mathf.Abs(contentY - boxY + boxH / 2f - +contentH / 2f);
            scrollbarEditor.value = range / max;
            //Debug.Log(scrollbarEditor.value);
        }

        private void Update()
        {
            ScrollAction();
            ControlScrollBar();
        }
    }
}
