using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace UiFramewark
{
    public class UiText : BasicUi,IText
    {
        private TextMeshProUGUI _textMeshProUGUI;

        private void Awake()
        {
            CheckTextComponent();
        }

        public void SetText(string text)
        {
            _textMeshProUGUI.SetText(text);
        }

        public void UpdateText(string text)
        {
            SetText(text);
        }

        public bool CheckTextComponent()
        {
            _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
            if(object.ReferenceEquals(_textMeshProUGUI,null))
            {
                _textMeshProUGUI = this.AddComponent<TextMeshProUGUI>();
            }
            return true;
        }
    }
}