using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EmbroideryFramewark
{
    public class RopeColorChange:MonoBehaviour,IRopeColorChange,IEnableAndDisable
    {

        [Header("设置绳子材质与颜色：(暂时从RopeManager中获取)")]
        private GameObject ColorChangeWithCanvus;

        private ColorPicker _colorPicker;

        private Material _selectedMaterial = null;

        int  cnt = 0; 

        private void Awake()
        {
            this.cnt++;

            ColorChangeWithCanvus = RopeManager.Instance.ColorChangeWithCanvus;

            if (!object.ReferenceEquals(ColorChangeWithCanvus, null))
            {
                _colorPicker = GameObject.Instantiate<GameObject>(ColorChangeWithCanvus).GetComponentInChildren<ColorPicker>();
                Debug.Log(_colorPicker+$"   cnt:{cnt}");
            }
            else
            {
                Debug.LogWarning("ColorChangeWithCanvus获取失败！");
                return;
            }


            Debug.Log("one");


            if (object.ReferenceEquals(_colorPicker, null))
            {
                Debug.LogError("初始化失败，找不到合适的ColorPicker");
                return;
            }

            Debug.Log("two");
            ///设置事件，自动改变绳子颜色
            _colorPicker.OnColorValueChanged += SetColor;
        }


        public void SetTargetMaterial(Material targetMaterial)
        {
            cnt++;
            Debug.Log("Set:"+_colorPicker + $"   cnt:{cnt}");

            if (object.ReferenceEquals(_colorPicker,null))
            {
                Debug.LogError("没有初始化colorPicker！");
                return ;
            }

            if (!object.ReferenceEquals(targetMaterial,null))
            {
                _selectedMaterial = targetMaterial;
                _colorPicker.Show(true, true);
                _colorPicker.UpdateSelectedColor(_selectedMaterial.color, false);
            }
            else
            {
                Debug.LogWarning("传入的材质是null，材质切换失败，仍然改变之前的材质");
            }
        }

        public void SetColor(Color color)
        {
            if (_selectedMaterial != null)
            {
                _selectedMaterial.color = color;
            }
        }

        #region 开闭
        
        private bool _isEnable = false;
        public bool IsEnable => _isEnable;
        
        public void Enable()
        {
            _isEnable = true;
            _colorPicker?.gameObject.SetActive(true);
        }

        public void Disable()
        {
            _isEnable = false;
            _colorPicker?.gameObject.SetActive(false);
        }

        #endregion
    }
}
