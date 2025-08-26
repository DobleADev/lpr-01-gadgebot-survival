using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// [ExecuteInEditMode]
public class NavigationCursor : MonoBehaviour
{
    public float pixelSizeRef = 20;
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
    public float normalVisualScale = 1.5f;
    public float selectedVisualScale = 0.9f;
    public float outlineColorFactor = 0.5f; // Factor para oscurecer el color del outline
    public TMP_Text label;
    private MaterialPropertyBlock _mpb;
    Vector3 cursorPosition;
    public float timeScale = 1;
    public float gadgebotDetectionSize = 1;
    public float gameplayDepth = 0;
    public Camera gameCamera;
    public LayerMask gadgebotLayer = 0;
    public Vector2 cursorVectorOnSelected;
    public Vector3 position
    {
        get
        {
            return transform.position;
        }
        set
        {
            cursorPosition = gameCamera.WorldToScreenPoint(value);
            transform.position = value;
        }
    }

    Vector3 debugOrigin;

    void Awake()
    {
        gameCamera = Camera.main;
        cursorPosition.x = Screen.width * 0.5f;
        cursorPosition.y = Screen.height * 0.5f;
    }

    // void LateUpdate()
    public void UpdatePosition()
    {
        Vector2 translation = sensibility * new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));

        if (selectableSelected != null)
        {
            Vector3 selectedPosition = selectableSelected.transform.position;
            cursorPosition = selectedPosition;
            selectedPosition.z = selectableSelected.gadgebot.transform.position.z - gameCamera.transform.position.z + selectedOffsetDepth;
            transform.position = gameCamera.ScreenToWorldPoint(selectedPosition);
            cursorVectorOnSelected += timeScale * translation;
            label.transform.localScale = selectedVisualScale * Vector3.one;
            // transform.position = selectableSelected.transform.position;
        }
        else
        {
            // transform.position += (Vector3)translation;
            // transform.position = Input.mousePosition;
            // cursorPosition = Input.mousePosition;
            cursorPosition += timeScale * (Vector3)translation;
            cursorPosition.x = Mathf.Clamp(cursorPosition.x, pixelSizeRef, Screen.width - pixelSizeRef);
            cursorPosition.y = Mathf.Clamp(cursorPosition.y, pixelSizeRef, Screen.height - pixelSizeRef);
            cursorPosition.z = -gameCamera.transform.position.z + normalOffsetDepth;

            transform.position = gameCamera.ScreenToWorldPoint(cursorPosition);
            label.transform.localScale = normalVisualScale * Vector3.one;
            cursorVectorOnSelected = Vector2.zero;
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

    void OnDrawGizmos()
    {
        // Gizmos.DrawWireCube(debugOrigin, 0.1f * Vector2.one);
        Gizmos.DrawWireSphere(debugOrigin, gadgebotDetectionSize);
    }

    public Vector3 GetScreenPosition()
    {
        return gameCamera.WorldToScreenPoint(transform.position);
    }

    public Gadgebot DoGadgebotRaycast()
    {
        Color rayColor = Color.gray;
        Vector3 screenPosition = GetScreenPosition();
        screenPosition.z = gameplayDepth - gameCamera.transform.position.z;
        Vector2 origin = gameCamera.ScreenToWorldPoint(screenPosition);
        debugOrigin = origin;
        // Collider2D collider = Physics2D.OverlapPoint(origin, gadgebotLayer);
        Collider2D collider = Physics2D.OverlapCircle(origin, gadgebotDetectionSize, gadgebotLayer);
        if (collider == null)
        {
            // Debug.DrawRay(origin, Vector3.forward * 100f, rayColor);
            return null;
        }
        rayColor = Color.red;
        Gadgebot gadgebot = collider.GetComponent<Gadgebot>();
        if (gadgebot != null) rayColor = Color.green;
        // Debug.DrawRay(origin, Vector3.forward * 100f, rayColor);
        return gadgebot;
    }
}
