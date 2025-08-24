using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// [ExecuteInEditMode]
public class NavigationCursor : MonoBehaviour
{
    public float sensibility = 10;
    NavigationSelectable _selectableSelected;
    public NavigationSelectable selectableSelected
    {
        get { return _selectableSelected; }
        set
        {
            _selectableSelected = value;
            UpdateColor(selectableSelected != null);
        }
    }
    public Color normalColor = Color.gray;
    public Color selectingColor = Color.green;
    public float normalOffsetDepth = 0;
    public float selectedOffsetDepth = 0;
    public float outlineColorFactor = 0.5f; // Factor para oscurecer el color del outline
    public TMP_Text label;
    private MaterialPropertyBlock _mpb;

    void LateUpdate()
    {
        // Vector2 translation = sensibility * new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        if (selectableSelected != null)
        {
            Vector3 selectedPosition = selectableSelected.transform.position;
            selectedPosition.z = selectableSelected.gadgebot.transform.position.z - Camera.main.transform.position.z + selectedOffsetDepth;
            transform.position = Camera.main.ScreenToWorldPoint(selectedPosition);
            // transform.position = selectableSelected.transform.position;
        }
        else
        {
            // transform.position += (Vector3)translation;
            // transform.position = Input.mousePosition;
            Vector3 mouseScreenPosition = Input.mousePosition;
            mouseScreenPosition.z = -Camera.main.transform.position.z + normalOffsetDepth;
            transform.position = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        }
    }

    void UpdateColor(bool isSelected)
    {
        Color finalColor;
        Color finalOutlineColor;

        // Determina el color base según el valor
        switch (isSelected)
        {
            case true:
                finalColor = selectingColor;
                break;
            default:
                finalColor = normalColor;
                break;
        }

        // Calcula el color del outline (el color final oscurecido)
        finalOutlineColor = new Color(
            finalColor.r * outlineColorFactor,
            finalColor.g * outlineColorFactor,
            finalColor.b * outlineColorFactor,
            finalColor.a
        );

        // Obtén el renderizador para aplicar el MaterialPropertyBlock
        Renderer renderer = label.GetComponent<Renderer>();

        // Si el renderizador existe, aplica las propiedades
        if (renderer != null)
        {
            if (_mpb == null) _mpb = new MaterialPropertyBlock();
            // Carga las propiedades del material actual
            renderer.GetPropertyBlock(_mpb);

            // Asigna los nuevos colores
            _mpb.SetColor("_FaceColor", finalColor);
            _mpb.SetColor("_OutlineColor", finalOutlineColor);

            // Aplica el bloque de propiedades al renderizador
            renderer.SetPropertyBlock(_mpb);
        }
    }

    void OnValidate()
    {
        UpdateColor(selectableSelected);
    }
}
