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
            //尝试复用tile,造成了一些麻烦
            DestroyImmediate(this.gameObject);
            //OriginFactory.Reclaim(this);
        }
    }

}