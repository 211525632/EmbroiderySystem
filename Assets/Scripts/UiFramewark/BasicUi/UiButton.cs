using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UiFramewark
{
    /// <summary>
    /// 需要手动配置图片
    /// </summary>
    public class UiButton : BasicUi,IButton
    {
        private Button _button;

        private void Awake()
        {
            CheckButtonComponent();


            ///用于不规则检测
            //Image image = this.GetComponent<Image>();
            
            //image.alphaHitTestMinimumThreshold = 0.5f;
        }


        public bool CheckButtonComponent()
        {
            _button = GetComponent<Button>();

            if (object.ReferenceEquals(null, _button))
            {
                Debug.LogError("UiButton不能找到合适Button组件：name：" + this.name);
                _button = this.AddComponent<Button>();
            }

            return _button != null;
        }


        public void AddListener(UnityAction onClickAction)
        {
            _button?.onClick?.AddListener(onClickAction);
        }

        public void RemoveListener(UnityAction releaseAction)
        {
            _button?.onClick?.RemoveListener(releaseAction);
        }

    }
}