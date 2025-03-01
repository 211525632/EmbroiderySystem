

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UiFramewark
{
    public class UiTest : MonoBehaviour
    {

        public void OnButtonClick()
        {
            Debug.Log("Click！");
        }

        public void AddNewUiCollection()
        {
            UiManager.Instance.AddUiColliction("test");
        }

        public void CloseUiCollection()
        {
            UiManager.Instance.CloseUiCollection("test");
        }

        public void ChangeText()
        {
            UiManager.Instance.GetUiCollection("test").GetUi<UiText>("mainInf").SetText("this is aother test Text！");
        }

        /// <summary>
        /// 应该在新的Collection被启用的时候
        /// 自动通知其他的组件
        /// 被通知的组件，根据需要，实时绑定当前的事件
        /// </summary>
        public void ActiveButtonEvent()
        {
            UiManager.Instance.GetUiCollection("test")
                .GetUi<UiButton>("Button")
                .AddListener(
                () => { 
                    Debug.Log("Trigger the ButtonEvent！"); 
                } );
        }
    }
}