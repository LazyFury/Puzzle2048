using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Puzzle2048
{
    public class Actor : MonoBehaviour
    {
        public void RemoveAllAttachActors()
        {
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                GameObject child = transform.GetChild(0).gameObject;
                child.GetComponent<Actor>()?.RemoveAllAttachActors();
                child.SetActive(false);
                if (Application.isEditor)
                {
                    DestroyImmediate(child);
                }
                else
                {
                    DestroyImmediate(child);
                }
            }
        }

    }
}
