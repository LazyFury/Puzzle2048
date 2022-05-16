using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle2048
{
    public enum TileType
    {
        Two = 2,
        Four = 4,
        Eight = 8,
        Sixteen = 16,
        ThirtyTwo = 32,
        SixtyFour = 64,
        OneHundredTwentyEight = 128,
        TwoHundredFiftySix = 256,
        FiveHundredTwelve = 512,
        OneThousandTwoHundredTwentyFour = 1024,
    }

    public static class TileTypeExtensions
    {
        public static int GetValue(this TileType tileType)
        {
            return (int)tileType;
        }

        public static TileType GetUpgrade(this TileType type)
        {
            return (TileType)((int)type + (int)type);
        }
    }

    [CreateAssetMenu(menuName = "Puzzle2048/TileFactory")]
    public class TileFactory : ScriptableObject
    {
       
        #region Prefabs;
        public Tile TwoPrefab;
        public Tile FourPrefab;
        public Tile EightPrefab;
        public Tile SixteenPrefab;
        public Tile ThirtyTwoPrefab;
        public Tile SixtyFourPrefab;
        public Tile OneHundredTwentyEightPrefab;
        public Tile TwoHundredFiftySixPrefab;
        public Tile FiveHundredTwelvePrefab;
        public Tile OneThousandTwoHundredFiftySixPrefab;
        #endregion

        Dictionary<TileType, List<Tile>> collections = new Dictionary<TileType, List<Tile>>();

        public Tile Get(TileType type)
        {
            /*¸´ÓÃtile*/
            if (collections.ContainsKey(type))
            {
                var list = collections[type];
                if (list.Count > 0)
                {
                    int lastIndex = list.Count - 1;
                    var tile = list[lastIndex];
                    list.RemoveAt(lastIndex);
                    if (tile != null)
                        return tile;
                }
            }
            //spawn tile
            Tile toSpawn = null;
            switch (type)
            {
                case TileType.Two:
                    toSpawn = TwoPrefab;
                    break;
                case TileType.Four:
                    toSpawn = FourPrefab;
                    break;
                case TileType.Eight:
                    toSpawn = EightPrefab;
                    break;
                case TileType.Sixteen:
                    toSpawn = SixteenPrefab;
                    break;
                case TileType.ThirtyTwo:
                    toSpawn = ThirtyTwoPrefab;
                    break;
                case TileType.SixtyFour:
                    toSpawn = SixtyFourPrefab;
                    break;
                case TileType.OneHundredTwentyEight:
                    toSpawn = OneHundredTwentyEightPrefab;
                    break;
                case TileType.TwoHundredFiftySix:
                    toSpawn = TwoHundredFiftySixPrefab;
                    break;
                case TileType.FiveHundredTwelve:
                    toSpawn = FiveHundredTwelvePrefab;
                    break;
                case TileType.OneThousandTwoHundredTwentyFour:
                    toSpawn = OneThousandTwoHundredFiftySixPrefab;
                    break;
            }
            if (toSpawn != null)
            {
                Tile tile = Instantiate(toSpawn);
                tile.OriginFactory = this;
                return tile;
            }
            return null;
        }

        public void Reclaim(Tile tile)
        {
            //check factory
            if (tile.OriginFactory != this)
            {
                return;
            }

            // check collection list
            if (!collections.ContainsKey(tile.Type))
            {
                collections.Add(tile.Type, new List<Tile>());
            }
            collections[tile.Type].Add(tile);
            tile.transform.SetParent(null);
            tile.gameObject.SetActive(false);
        }
    }
}
