using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

public class Draggable : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Vector2 prePos;
    private GameObject preParent;
    private bool isDropped = false;

    public Action beforeBeginDrag;
    public Action<RectTransform, Action> onDropSuccess;
    public Action<Action> onDropFail;
    public Dictionary<RectTransform, Vector2> snapPositions;

    private List<RectTransform> dropAreas = new List<RectTransform>();

    public void SetDropAreas(List<RectTransform> areas)
    {
        dropAreas = areas;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (isDropped) return;

        beforeBeginDrag?.Invoke();
        prePos = transform.position;
        preParent = transform.parent.gameObject;
        transform.SetParent(transform.root.gameObject.transform, true);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isDropped) return;

        Vector3 vec = Camera.main.WorldToScreenPoint(transform.position);
        vec.x += eventData.delta.x;
        vec.y += eventData.delta.y;
        transform.position = Camera.main.ScreenToWorldPoint(vec);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (isDropped) return;

        bool isSuccess = false;
        foreach (RectTransform area in dropAreas)
        {
            if (Contains(area, eventData))
            {
                isSuccess = true;
                if (snapPositions != null && snapPositions.ContainsKey(area))
                {
                    transform.position = snapPositions[area];
                }
                isDropped = true;
                onDropSuccess?.Invoke(area, resetPos());
                break;
            }
        }

        if (!isSuccess)
        {
            if (onDropFail == null)
            {
                resetPos().Invoke();
            }
            else
            {
                onDropFail.Invoke(resetPos());
            }
        }
    }

    private Action resetPos()
    {
        return () =>
        {
            transform.position = prePos;
            transform.SetParent(preParent.transform, true);
        };
    }

    private bool Contains(RectTransform area, PointerEventData target)
    {
        var selfBounds = GetBounds(area);
        Vector3 worldPos;
        RectTransformUtility.ScreenPointToWorldPointInRectangle(
            area,
            target.position,
            target.pressEventCamera,
            out worldPos);
        worldPos.z = 0f;
        return selfBounds.Contains(worldPos);
    }

    private Bounds GetBounds(RectTransform target)
    {
        Vector3[] s_Corners = new Vector3[4];
        var min = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        var max = new Vector3(float.MinValue, float.MinValue, float.MinValue);
        target.GetWorldCorners(s_Corners);
        for (var index2 = 0; index2 < 4; ++index2)
        {
            min = Vector3.Min(s_Corners[index2], min);
            max = Vector3.Max(s_Corners[index2], max);
        }

        max.z = 0f;
        min.z = 0f;

        Bounds bounds = new Bounds(min, Vector3.zero);
        bounds.Encapsulate(max);
        return bounds;
    }
}
