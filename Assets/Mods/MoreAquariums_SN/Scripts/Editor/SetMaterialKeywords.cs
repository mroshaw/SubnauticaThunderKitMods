using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DaftAppleGames.MoreAquariums.Editor
{
    [ExecuteInEditMode]
    public class SetMaterialKeywords : MonoBehaviour
    {
        [SerializeField] private bool isCutOut = true;
        
        private void OnEnable()
        {
            Material[] mats = gameObject.GetComponent<MeshRenderer>().sharedMaterials;

            foreach (Material mat in mats)
            {
                if (isCutOut)
                {
                    mat.EnableKeyword("MARMO_ALPHA_CLIP");
                    Debug.Log("MARMO_ALPHA_CLIP enabled");
                }
            }
        }
    }
}