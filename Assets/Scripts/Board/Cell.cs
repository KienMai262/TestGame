using System;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int BoardX { get; private set; }
    public int BoardY { get; private set; }
    public int Layer { get; private set; }
    public Item Item { get; private set; }
    public bool IsEmpty => Item == null;

    public void Setup(int cellX, int cellY, int layer)
    {
        this.BoardX = cellX;
        this.BoardY = cellY;
        this.Layer = layer;
    }

    public void Free()
    {
        Item = null;
    }

    public void Assign(Item item)
    {
        Item = item;
        Item.SetCell(this);
    }

    internal void Clear()
    {
        if (Item != null)
        {
            Item.Clear();
            Item = null;
        }
    }

    public void ApplyItemPosition(bool withAppearAnimation, int layer)
    {
        Item.SetViewPosition(this.transform.position);
        // Item.View.SetParent(this.transform);
        Item.View.gameObject.GetComponent<SpriteRenderer>().sortingOrder = layer;

        if (withAppearAnimation)
        {
            Item.ShowAppearAnimation();
        }
    }


}
