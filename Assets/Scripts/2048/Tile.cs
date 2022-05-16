using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle2048
{

    public class Tile : Actor
    {
        [SerializeField] public TileType Type = TileType.Two;
        [SerializeField] TileFactory originFactory;

        public TileFactory OriginFactory
        {
            get => originFactory;
            set
            {
                Debug.Assert(originFactory == null, "Redefined Factory!");
                originFactory = value;
            }
        }

        public void Recycle()
        {
            DestroyImmediate(this.gameObject);
            //OriginFactory.Reclaim(this);
        }
    }

}