using System;
using System.Collections.Generic;
using UnityEngine;

namespace DaftAppleGames.SubnauticaPets.BaseParts
{
    [CreateAssetMenu(fileName = "PetListTemplates", menuName = "ScriptableObjects/SubnauticaPets/PetListTemplates", order = 1)]
    public class ConsoleListTemplates : ScriptableObject
    {
        [SerializeField] private List<PetTemplate> petTemplates;

        internal GameObject GetTemplate(string techType)
        {
            foreach (PetTemplate petTemplate in petTemplates)
            {
                if (petTemplate.petTechType == techType)
                {
                    return petTemplate.listTemplatePrefab;
                }
            }
            return null;
        }
    }

    [Serializable]
    internal class PetTemplate
    {
        [SerializeField] internal string petTechType;
        [SerializeField] internal GameObject listTemplatePrefab;
    }
}