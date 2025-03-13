using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[Serializable]
public class Item
{
    public Cell Cell { get; private set; }

    public Transform View { get; private set; }

    public bool IsSelected = true;

    public virtual void SetView()
    {
        string prefabname = GetPrefabName();

        if (!string.IsNullOrEmpty(prefabname))
        {
            GameObject prefab = Resources.Load<GameObject>(prefabname);
            if (prefab)
            {
                View = GameObject.Instantiate(prefab).transform;
            }
        }
    }

    protected virtual string GetPrefabName() { return string.Empty; }

    public virtual void SetCell(Cell Cell)
    {
        Cell = Cell;
    }

    internal void AnimationMoveToPosition()
    {
        if (View == null) return;

        View.DOMove(Cell.transform.position, 0.2f);
    }

    public void SetViewPosition(Vector3 pos)
    {
        if (View)
        {
            View.position = pos;
        }
    }

    public void SetViewRoot(Transform root)
    {
        if (View)
        {
            View.SetParent(root);
        }
    }

    public void SetSortingLayerHigher()
    {
        if (View)
        {
            View.SetAsLastSibling();
        }
    }

    public void ShowAppearAnimation()
    {
        if (View == null) return;

        View.localScale = Vector3.zero;
        View.DOScale(Vector3.one, 0.2f);
    }
    internal virtual bool IsSameType(Item other)
    {
        return false;
    }

    internal virtual void ExplodeView()
    {
        if (View)
        {
            View.DOScale(0.1f, 0.1f).OnComplete(
                () =>
                {
                    GameObject.Destroy(View.gameObject);
                    View = null;
                }
                );
        }
    }

    public void Clear()
    {
        if (View)
        {
            GameObject.Destroy(View.gameObject);
        }
    }

    public void AnimateForHint()
    {
        if (View == null) return;

        View.DOPunchScale(Vector3.one * 0.1f, 0.5f, 10, 1);
    }

    public bool IsOverlapping(GameObject obj, List<Transform> allObjects)
    {
        Bounds myBounds = GetObjectBounds(obj);

        if (obj.tag == "Boerd1")
        {
            foreach (Transform other in allObjects)
            {
                if (other.gameObject.tag == "Boerd0" && other.gameObject.activeSelf == true)
                {
                    Bounds otherBounds = GetObjectBounds(other.gameObject);

                    if (myBounds.Intersects(otherBounds))
                    {
                        return true;
                    }
                }
            }
        }
        else if (obj.tag == "Boerd2")
        {
            foreach (Transform other in allObjects)
            {
                if (other.gameObject.tag == "Boerd1" && other.gameObject.activeSelf == true)
                {
                    Bounds otherBounds = GetObjectBounds(other.gameObject);

                    if (myBounds.Intersects(otherBounds))
                    {
                        return true;
                    }
                }
            }
        }
        else if (obj.tag == "Boerd3")
        {
            foreach (Transform other in allObjects)
            {
                if (other.gameObject.tag == "Boerd2" && other.gameObject.activeSelf == true)
                {
                    Bounds otherBounds = GetObjectBounds(other.gameObject);

                    if (myBounds.Intersects(otherBounds))
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    private Bounds GetObjectBounds(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();
        if (renderer != null)
        {
            Bounds bounds = renderer.bounds;

            return new Bounds(obj.transform.position, new Vector3(renderer.bounds.size.x, renderer.bounds.size.y, 0f));
        }

        return new Bounds(obj.transform.position, Vector3.zero);
    }



}
