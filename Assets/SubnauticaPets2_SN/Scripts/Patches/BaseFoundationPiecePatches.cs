using DaftAppleGames.SubnauticaPets.Pets;
using DaftAppleGames.SubnauticaPets.Utils;
using HarmonyLib;
using UnityEngine;

namespace DaftAppleGames.SubnauticaPets.Patches
{
    [HarmonyPatch(typeof(BaseFoundationPiece))]
    internal class BaseFoundationPiecePatches
    {

        private const int PetObstacleLayer = 25;
        private static bool _layerConfigured;
        
        /// <summary>
        /// Patches the Start method, adding a special collider to the Moon Pool to stop pets falling in
        /// </summary>
        [HarmonyPatch(nameof(BaseFoundationPiece.Start))]
        [HarmonyPostfix]
        public static void Start_Postfix(BaseFoundationPiece __instance)
        {
            if (__instance.gameObject.name != "BaseMoonpool(Clone)")
            {
                return;
            }
            
            Transform poolColliderTransform  = __instance.transform.Find("entrance");

            if (!poolColliderTransform)
            {
                LogUtils.LogError(LogArea.Patches,
                    $"Could not patch MoonPool on {__instance.gameObject.name}! Couldn't find pool collider transform!");
            }

            BoxCollider entranceCollider = poolColliderTransform.GetComponent<BoxCollider>();

            GameObject petColliderGameObject = GameObject.CreatePrimitive(PrimitiveType.Cube);
            petColliderGameObject.name = "petcollider";
            petColliderGameObject.layer = PetObstacleLayer;
            petColliderGameObject.tag = poolColliderTransform.gameObject.tag;

            petColliderGameObject.transform.SetParent(__instance.transform);
            petColliderGameObject.transform.position = entranceCollider.transform.position + new Vector3(0, -1f, 0);
            petColliderGameObject.transform.rotation = entranceCollider.transform.rotation;
            petColliderGameObject.transform.localScale = entranceCollider.size + (new Vector3(0, 2f, 0));

            if (!_layerConfigured)
            {
                ConfigureLayerMatrix();
            }
            
            Object.Destroy(petColliderGameObject.GetComponent<MeshRenderer>());
            Object.Destroy(petColliderGameObject.GetComponent<MeshFilter>());
        }
        
        private static void ConfigureLayerMatrix()
        {
            /*
            for (int currLayer = 0; currLayer <= 31; currLayer++)
            {
                if (currLayer == PetObstacleLayer)
                {
                    continue;
                }
                
                Physics.IgnoreLayerCollision(currLayer, PetObstacleLayer);
            }
            */
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Vehicle"), PetObstacleLayer);
            Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), PetObstacleLayer);
            
            Physics.IgnoreLayerCollision(PetObstacleLayer, LayerMask.NameToLayer("Vehicle"));
            Physics.IgnoreLayerCollision(PetObstacleLayer, LayerMask.NameToLayer("Player"));
            
            _layerConfigured = true;
        }
    }
}