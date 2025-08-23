using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[ExecuteInEditMode]
public class IntLabel : MonoBehaviour
{
    public Color normalColor = Color.green;
    public Color oneColor = Color.yellow;
    public Color zeroColor = Color.gray;
    public float outlineColorFactor = 0.5f; // Factor para oscurecer el color del outline
    public TMP_Text label;

    // Se recomienda usar un MaterialPropertyBlock para evitar crear nuevas instancias de material
    private MaterialPropertyBlock _mpb;

    // Método para setear el texto del label
    public void SetLabel(int value)
    {
        // Actualiza el texto con el valor entero
        label.text = value.ToString();
        // Llama a la función para actualizar el color
        UpdateColor(value);
    }

    // Método para actualizar el color del label y del outline
    void UpdateColor(int value)
    {
        Color finalColor;
        Color finalOutlineColor;

        // Determina el color base según el valor
        switch (value)
        {
            case 0:
                finalColor = zeroColor;
                break;
            case 1:
                finalColor = oneColor;
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
}