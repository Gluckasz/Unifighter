using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DragAndDrop : MonoBehaviour
{
    private GameObject discardPile;
    private GameObject player;
    public bool isDragging = false;
    private Vector3 offset;
    private RectTransform rectTransform;
    private RectTransform containerRectTransform;
    private Canvas canvas;
    private BringToFrontOnHover bringToFrontOnHover;
    private MoveOnHover moveOnHover;
    private HorizontalLayoutGroup horizontalLayoutGroup;
    private OnPlay onPlayScript;
    private EnergyDrain energyDrainScript;
    private EnergyManager energyManagerScript;

    private void OnTransformParentChanged()
    {
        if (transform.parent.gameObject.tag == "DiscardPile")
        {
            horizontalLayoutGroup.enabled = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate(containerRectTransform);
        }
        horizontalLayoutGroup = GetComponentInParent<HorizontalLayoutGroup>();
        if (horizontalLayoutGroup != null)
        {

            containerRectTransform = horizontalLayoutGroup.GetComponent<RectTransform>();
            horizontalLayoutGroup.enabled = true;
            LayoutRebuilder.ForceRebuildLayoutImmediate(containerRectTransform);
            discardPile = GameObject.FindGameObjectWithTag("DiscardPile");
            bringToFrontOnHover = GetComponent<BringToFrontOnHover>();
            moveOnHover = GetComponent<MoveOnHover>();
            rectTransform = GetComponent<RectTransform>();
            canvas = GetComponentInParent<Canvas>();
            bringToFrontOnHover.enabled = true;
            moveOnHover.enabled = true;
            player = GameObject.FindGameObjectWithTag("Player");
            energyManagerScript = player.GetComponent<EnergyManager>();
            energyDrainScript = GetComponent<EnergyDrain>();
        }
    }

    void OnMouseDown()
    {
        Debug.Log("Mouse down on: " + gameObject.name);
        // Make other scripts that interfere with this script inactive
        bringToFrontOnHover.enabled = false;
        moveOnHover.enabled = false;
        isDragging = true;
        offset = rectTransform.position - GetMousePosition();
        // Set this card to be on the first plane - also it will make sure that card is inserted as first after dropping it to hand
        int siblingCount = gameObject.transform.parent.childCount;
        transform.SetSiblingIndex(siblingCount);
        if (horizontalLayoutGroup != null)
        {
            horizontalLayoutGroup.enabled = false;
        }
    }

    void OnMouseDrag()
    {
        if (isDragging)
        {
            Vector3 mousePosition = GetMousePosition() + offset;
            rectTransform.position = new Vector3(mousePosition.x, mousePosition.y, rectTransform.position.z);
        }
    }

    private Vector3 GetMousePosition()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return mouseWorldPosition;
    }
    void OnMouseUp()
    {
        Debug.Log("Mouse up on: " + gameObject.name);
        isDragging = false;
        if (horizontalLayoutGroup != null)
        {
            // If card dropped on hand object, just insert it back to hand
            if (IsMouseWithinContainerBounds())
            {
                horizontalLayoutGroup.enabled = true;
                LayoutRebuilder.ForceRebuildLayoutImmediate(containerRectTransform);
                bringToFrontOnHover.enabled = true;
                moveOnHover.enabled = true;
            }
            else if (energyManagerScript.energy < energyDrainScript.energyCost * -1)
            {
                Debug.Log("Energy to low");
                horizontalLayoutGroup.enabled = true;
                LayoutRebuilder.ForceRebuildLayoutImmediate(containerRectTransform);
                bringToFrontOnHover.enabled = true;
                moveOnHover.enabled = true;
            }
            // If card dropped anywhere else, play it and insert it to discard pile
            else
            {
                gameObject.transform.SetParent(discardPile.transform);
                // Set the position to be outside of a screen to make player not able to click at this card
                rectTransform.anchoredPosition = new Vector2(1300, 400);
                onPlayScript = GetComponent<OnPlay>();
                onPlayScript.enabled = true;
            }
        }
    }

    private bool IsMouseWithinContainerBounds()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Camera.main.nearClipPlane;
        Vector3 mouseWorldPosition = Camera.main.ScreenToWorldPoint(mousePosition);
        return RectTransformUtility.RectangleContainsScreenPoint(containerRectTransform, mouseWorldPosition);
    }
}
