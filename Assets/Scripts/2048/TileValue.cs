using System;
using UnityEngine;

namespace Puzzle2048
{
    [Serializable]
    public class TileValue
    {
        public Transform background;
        public Tile tile;
        public int value
        {
            get
            {
                if (tile == null) { return 0; }
                return (int)tile?.Type;
            }
        }

        public TileValue()
        {
        }

        public TileValue(Transform background, Tile tile)
        {
            this.background = background;
            if (tile != null)
            {
                SetTile(tile);
            }
        }

        public void SetTile(Tile tile)
        {
            this.tile = tile;
            if (tile == null) { return; }
            UpdateTile();
        }
        public void UpdateTile()
        {
            this.tile.transform.SetParent(background.transform);
            this.tile.transform.localPosition = Vector3.zero;
        }

        public void UpgradeTile(TileFactory factory)
        {
            if (tile == null) { return; }
            if (tile.Type == TileType.OneThousandTwoHundredTwentyFour) { return; }
            TileType type = tile.Type.GetUpgrade();
            tile.Recycle();
            tile = null;
            var t = factory.Get(type);
            Debug.Assert(t != null, "tile is null");
            SetTile(t);
        }
    }
}

