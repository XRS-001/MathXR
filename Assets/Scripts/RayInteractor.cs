using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.InputSystem;
using System.Runtime.CompilerServices;

public class RayInteractor : MonoBehaviour
{
    [Header("Bezier Curve")]
    public Transform reticle;
    public Transform point1;
    public Transform point2;
    public Transform point3;
    public LineRenderer lineRenderer;
    public int vertexCount = 6;
    [Header("Interaction")]
    public InputActionReference UIPressButton;
    private Button button;
    private bool isHovering;
    public RayInteractor otherRayInteractor;
    private bool canPress = true;
    private void Start()
    {
        point1.transform.parent = null;
        lineRenderer.enabled = true;
    }
    // Update is called once per frame
    void Update()
    {
        if (lineRenderer.enabled)
        {
            var pointList = new List<Vector3>();
            for (float ratio = 0; ratio <= 1; ratio += 1.0f / vertexCount)
            {
                var tangentLineVertex1 = Vector3.Lerp(point1.position, point2.position, ratio);
                var tangentLineVertex2 = Vector3.Lerp(point2.position, point3.position, ratio);
                var bezierpoint = Vector3.Lerp(tangentLineVertex1, tangentLineVertex2, ratio);
                pointList.Add(bezierpoint);
            }
            lineRenderer.positionCount = pointList.Count;
            lineRenderer.SetPositions(pointList.ToArray());
        }
        Physics.Raycast(point3.position, point3.forward, out RaycastHit hitInfo, float.PositiveInfinity);
        if (hitInfo.collider)
        {
            if (hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("UI"))
            {
                reticle.gameObject.SetActive(true);
                reticle.rotation = Quaternion.LookRotation(hitInfo.normal, Vector3.up);
                lineRenderer.enabled = true;
                point1.position = Vector3.Lerp(point1.position, hitInfo.point, 0.1f);
                Vector3 centerpos = (hitInfo.point + point3.position) / 2;
                point2.position = centerpos;
                if(!button && !otherRayInteractor.button)
                    button = hitInfo.collider.GetComponent<Button>();
                else if (!button && otherRayInteractor.button != hitInfo.collider.GetComponent<Button>())
                    button = hitInfo.collider.GetComponent<Button>();

                if (button)
                {
                    if (!isHovering)
                    {
                        button.HoverButton();
                        isHovering = true;
                    }
                    else if (hitInfo.collider.GetComponent<Button>())
                    {
                        if (hitInfo.collider.GetComponent<Button>() != button)
                        {
                            button.UnHoverButton();
                            isHovering = false;
                            button = null;
                        }
                    }
                    else
                    {
                        button.UnHoverButton();
                        isHovering = false;
                        button = null;
                    }

                    if (UIPressButton.action.ReadValue<float>() > 0.5f && button && canPress)
                    {
                        button.PressButton();
                        Invoke(nameof(DelayCanPress), 0.5f);
                        canPress = false;
                    }
                }
                else if (hitInfo.collider.GetComponent<Button>())
                    if (UIPressButton.action.ReadValue<float>() > 0.5f)
                    {
                        hitInfo.collider.GetComponent<Button>().PressButton();
                    }
            }
            else
            {
                Disable();
            }
        }
        else if (lineRenderer.enabled)
        {
            Disable();
        }
    }
    void DelayCanPress()
    {
        canPress = true;
    }
    private void Disable()
    {
        if (button)
        {
            if (isHovering)
            {
                button.UnHoverButton();
                isHovering = false;
                button = null;
            }
        }
        lineRenderer.enabled = false;
        reticle.gameObject.SetActive(false);
    }
}
