
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace UiFramewark
{
    public interface IButton
    {
        bool CheckButtonComponent();
        void AddListener(UnityAction onClickAction);

        void RemoveListener(UnityAction releaseAction);
    }

    public interface IText
    {
        bool CheckTextComponent();

        void SetText(string text);
        void UpdateText(string text);
    }

    public interface ISlider
    {
        float currentIndex { get; set; }

        float MaxIndex { get; set; }

        float MinIndex { get; set; }


        void SetSliderIndex(float currentIndex);

        void UpdateSlide();

    }



    public interface IUiAction
    {
        void OnEnable();

        void OnDisable();


        void OnCreate();

        void OnDestroy();
    }
}
