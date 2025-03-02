using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CubePicker : MonoBehaviour
{
    [SerializeField] private ColorPicker ColorPickerRef;
    [SerializeField] private TextMeshProUGUI SelectedCubeText;

    private Dictionary<Transform, Material> _cachedMaterials; // 缓存材质副本
    private Material _materialOfSelectedCube; // 当前选中物体的材质

    private void Start()
    {
        _cachedMaterials = new Dictionary<Transform, Material>();
        ColorPickerRef.OnColorValueChanged += ChangeColorOfSelectedCube;
    }

    private void LateUpdate()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit rayHit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out rayHit, 100))
            {
                SelectCube(rayHit.transform);
            }
        }
    }

    private void SelectCube(Transform cube)
    {
        if (_cachedMaterials.ContainsKey(cube))
        {
            _materialOfSelectedCube = _cachedMaterials[cube];
        }
        else
        {
            Material newMaterial = new Material(cube.GetComponent<MeshRenderer>().material);
            _cachedMaterials.Add(cube, newMaterial); 
            _materialOfSelectedCube = newMaterial;

            cube.GetComponent<MeshRenderer>().material = newMaterial;
        }

        
        ColorPickerRef.Show(true, true);
        ColorPickerRef.UpdateSelectedColor(_materialOfSelectedCube.color, false);
       
        SelectedCubeText.text = cube.name;
    }

    private void ChangeColorOfSelectedCube(Color newColor)
    {
        if (_materialOfSelectedCube != null)
        {
            _materialOfSelectedCube.color = newColor;
        }
    }
}
