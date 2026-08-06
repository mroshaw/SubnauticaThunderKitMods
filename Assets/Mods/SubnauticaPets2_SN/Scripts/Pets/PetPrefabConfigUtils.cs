using Nautilus.Handlers;
using Nautilus.Utility;
using System.Collections.Generic;
using DaftAppleGames.ModTools;
using DaftAppleGames.ModTools.Extensions;
using DaftAppleGames.SubnauticaPets.Utils;
using Nautilus.Assets;
using Nautilus.Assets.Gadgets;
using Nautilus.Crafting;
using UnityEngine;
using static DaftAppleGames.SubnauticaPets.SubnauticaPetsPlugin;

namespace DaftAppleGames.SubnauticaPets.Pets
{
    internal static class PetPrefabConfigUtils
    {
        /// <summary>
        /// Adds and configures components for Custom Pets
        /// </summary>
        internal static void AddCustomPetComponents(GameObject targetGameObject, string audioClipName, string busPath,
            float audioVolume)
        {
            targetGameObject.EnsureComponent<PetStateController>();
            targetGameObject.EnsureComponent<SimpleMovement>();
            targetGameObject.EnsureComponent<IdleAction>();
            targetGameObject.EnsureComponent<WanderAction>();
            targetGameObject.EnsureComponent<MoveToAction>();
            targetGameObject.EnsureComponent<KilledAction>();
            targetGameObject.EnsureComponent<SleepAction>();
            
            LiveMixin liveMixin = targetGameObject.EnsureComponent<LiveMixin>();
            liveMixin.data = ScriptableObject.CreateInstance<LiveMixinData>();
            liveMixin.data.maxHealth = 50;
            liveMixin.health = 50;

            CustomPet customPet = targetGameObject.EnsureComponent<CustomPet>();
            customPet.babyScaleSize = 1.0f;
            PetAnimator petAnimator = targetGameObject.EnsureComponent<PetAnimator>();
            CreatureDeath creatureDeath = targetGameObject.EnsureComponent<CreatureDeath>();
            creatureDeath.liveMixin = liveMixin;
            creatureDeath.respawn = false;
            creatureDeath.useRigidbody = targetGameObject.GetComponent<Rigidbody>();

            ModDebugLog.LogDebug( "Setting up FMOD Emitter");
            FMOD_CustomEmitter customEmitter = targetGameObject.EnsureComponent<FMOD_CustomEmitter>();
            ModAudioUtils.RegisterSound(audioClipName, busPath, ModAssetUtils, ModDebugLog, 0.1f, 8.0f, 0, false);
            FMODAsset petFmodAsset = AudioUtils.GetFmodAsset(audioClipName);
            ModAudioUtils.ConfigureEmitter(customEmitter,  petFmodAsset, ModDebugLog);

            // Configure the CharacterController collider to interact with the MoonPool blocker
            CharacterController characterController = targetGameObject.GetComponent<CharacterController>();
        }

        /// <summary>
        /// Adds the specified child pet class component to the given creature GameObject
        /// based on the given PetCreatureType
        /// </summary>
        internal static void AddPetComponent(GameObject targetGameObject)
        {
            targetGameObject.EnsureComponent<Pet>();
        }

        internal static void ConfigureLargeWorldEntity(GameObject targetGameObject, bool state)
        {
            LargeWorldEntity largeWorldEntity = targetGameObject.GetComponent<LargeWorldEntity>();
            if (largeWorldEntity)
            {
                largeWorldEntity.cellLevel = LargeWorldEntity.CellLevel.Global;
                // largeWorldEntity.enabled = state;
            }
        }

        internal static void AddPrefabIdentifier(GameObject targetGameObject, string classId, TechType techType)
        {
            targetGameObject.EnsureComponent<PrefabIdentifier>().ClassId = classId;

            if (techType != TechType.None)
            {
                targetGameObject.EnsureComponent<TechTag>().type = techType;
            }
        }

        /// <summary>
        /// Adds a Capsule Collider to DNA prefab
        /// </summary>
        public static void AddDnaCapsuleCollider(GameObject targetGameObject)
        {
            ModDebugLog.LogDebug("AddDnaCapsuleCollider started...");

            Collider collider = targetGameObject.GetComponentInChildren<Collider>(true);
            if (collider)
            {
                Object.Destroy(collider);
                CapsuleCollider newCollider = collider.gameObject.AddComponent<CapsuleCollider>();
                newCollider.center = new Vector3(0, 0, 0);
                newCollider.radius = 0.18f;
                newCollider.height = 0.73f;
                newCollider.direction = 1;
            }

            ModDebugLog.LogDebug( "AddDnaCapsuleCollider done.");
        }

        /// <summary>
        /// Configure the animator component
        /// </summary>
        internal static void ConfigureAnimator(GameObject targetGameObject, bool isEnabled)
        {
            ModDebugLog.LogDebug( "ConfigureAnimator started...");
            Creature creature = targetGameObject.GetComponent<Creature>();
            Animator animator = creature.GetAnimator();
            animator.enabled = isEnabled;
            ModDebugLog.LogDebug( "ConfigureAnimator done.");
        }

        /// <summary>
        /// Adds the ScaleOnStart component
        /// </summary>
        internal static void AddScaleOnStart(GameObject targetGameObject, float scaleFactor)
        {
            ModDebugLog.LogDebug( "AddScaleOnStart started...");
            ScaleOnStart scaleOnStart = targetGameObject.AddComponent<ScaleOnStart>();
            scaleOnStart.Scale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
            ModDebugLog.LogDebug( "AddScaleOnStart done.");
        }

        /// <summary>
        /// Add VFX Fabricator component
        /// </summary>
        internal static void AddVFXFabricating(GameObject targetGameObject, string pathToModel, float minY, float maxY,
            Vector3 posOffset, float scaleFactor, Vector3 eulerOffset)
        {
            ModDebugLog.LogDebug( "AddVFXFabricating started...");
            GameObject modelGameObject = targetGameObject.GetComponentInChildren<Animator>().gameObject;
            if (modelGameObject != null)
            {
                PrefabUtils.AddVFXFabricating(modelGameObject, pathToModel, minY, maxY, posOffset, scaleFactor,
                    eulerOffset);
            }

            ModDebugLog.LogDebug( "AddVFXFabricating done.");
        }

        /// <summary>
        /// Adds a Prefab Identifier component
        /// </summary>
        internal static void AddPrefabIdentifier(GameObject targetGameObject)
        {
            ModDebugLog.LogDebug("AddPrefabIdentifier started...");
            PrefabIdentifier prefabIdentifier = targetGameObject.GetComponent<PrefabIdentifier>();
            if (!prefabIdentifier)
            {
                targetGameObject.AddComponent<PrefabIdentifier>();
            }

            ModDebugLog.LogDebug("AddPrefabIdentifier done.");
        }

        /// <summary>
        /// Sets all Mesh Renderers to the given colour
        /// </summary>
        internal static void SetMeshRenderersColor(GameObject targetGameObject, string modelGameObjectName, Color color)
        {
            ModDebugLog.LogDebug("SetMeshRenderersColor started...");
            targetGameObject.FindChild(modelGameObjectName).GetComponent<MeshRenderer>().material.color = color;
            ModDebugLog.LogDebug("SetMeshRenderersColor done.");
        }

        /// <summary>
        /// Adds a RotateModel component
        /// </summary>
        /// <param name="targetGameObject"></param>
        /// <param name="modelGameObjectName"></param>
        internal static void AddRotateModel(GameObject targetGameObject, string modelGameObjectName)
        {
            ModDebugLog.LogDebug("AddRotateModel started...");
            GameObject dnaGameObject = targetGameObject.transform.Find(modelGameObjectName).gameObject;
            RotateModel rotateModel = dnaGameObject.AddComponent<RotateModel>();
            rotateModel.RotationSpeed = 0.1f;
            ModDebugLog.LogDebug("AddRotateModel done.");
        }

        /// <summary>
        /// Adds a TechTag component
        /// </summary>
        internal static void AddTechTag(GameObject targetGameObject, TechType techType)
        {
            ModDebugLog.LogDebug( "AddTechTag started...");
            TechTag techTag = targetGameObject.GetComponent<TechTag>();
            if (techTag == null)
            {
                techTag = targetGameObject.AddComponent<TechTag>();
            }

            techTag.type = techType;
            ModDebugLog.LogDebug( "AddTechTag done");
        }

        /// <summary>
        /// Updates the Pickupable component
        /// </summary>
        internal static void UpdatePickupable(GameObject targetGameObject, bool isPickupable)
        {
            ModDebugLog.LogDebug( "UpdatePickupable started...");
            // Prevent fragments from being phsyically picked up
            Pickupable pickupable = targetGameObject.GetComponent<Pickupable>();
            if (pickupable)
            {
                pickupable.isPickupable = isPickupable;
            }

            ModDebugLog.LogDebug("UpdatePickupable done.");
        }

        /// <summary>
        /// Adds the SimpleMovement component
        /// </summary>
        internal static void AddSimpleMovement(GameObject targetGameObject)
        {
            ModDebugLog.LogDebug( "AddSimpleMovement started...");
            // Add simple movement component
            SimpleMovement movement = targetGameObject.GetComponent<SimpleMovement>();
            if (movement == null)
            {
                movement = targetGameObject.AddComponent<SimpleMovement>();
                movement.SetMoveSpeed(1.0f);
            }

            ModDebugLog.LogDebug( "AddSimpleMovement done.");
        }

        /// <summary>
        /// Add the WorldForces component
        /// </summary>
        internal static void AddWorldForces(GameObject targetGameObject)
        {
            ModDebugLog.LogDebug( "AddWorldForces started...");
            WorldForces worldForces = targetGameObject.GetComponent<WorldForces>();
            if (worldForces == null)
            {
                worldForces = targetGameObject.AddComponent<WorldForces>();
            }

            ModDebugLog.LogDebug( "AddWorldForces done.");
        }

        /// <summary>
        /// Add the PetHandTarget component
        /// </summary>
        internal static void AddPetHandTarget(GameObject targetGameObject)
        {
            ModDebugLog.LogDebug( "AddPetHandTarget started...");
            targetGameObject.AddComponent<PetHandTarget>();
            ModDebugLog.LogDebug( "AddPetHandTarget done.");
        }

        /// <summary>
        /// Sets the pet scale
        /// </summary>
        internal static void SetScale(GameObject targetGameObject, Vector3 scaleFactor)
        {
            ModDebugLog.LogDebug( "SetScale started...");
            targetGameObject.transform.localScale = scaleFactor;
            ModDebugLog.LogDebug( "SetScale done.");
        }

        /// <summary>
        /// Configure Pet Traits for "friendly" creatures
        /// </summary>
        internal static void ConfigurePetTraits(GameObject targetGameObject)
        {
            ModDebugLog.LogDebug( "ConfigurePetTraits started...");
            Creature creature = targetGameObject.GetComponent<Creature>();
            if (creature)
            {
                creature.Friendliness.Value = 1.0f;
                creature.Happy.Value = 1.0f;
                creature.Aggression.Value = 0.0f;
                creature.Scared.Value = 0.0f;
                creature.Curiosity.Value = 1.0f;
                creature.Hunger.Value = 1.0f;
                creature.Tired.Value = 0.0f;
            }

            ModDebugLog.LogDebug( "ConfigurePetTraits done.");
        }

        /// <summary>
        /// Adds a RigidBody, if not one already
        /// </summary>
        internal static void AddRigidBody(GameObject targetGameObject)
        {
            ModDebugLog.LogDebug( "AddRigidBody started...");
            Rigidbody rigidbody = targetGameObject.GetComponent<Rigidbody>();
            if (rigidbody == null)
            {
                rigidbody = targetGameObject.AddComponent<Rigidbody>();
                rigidbody.mass = 0.5f;
                rigidbody.useGravity = true;
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
                rigidbody.isKinematic = false;
            }

            ModDebugLog.LogDebug( "AddRigidBody done.");
        }


        /// <summary>
        /// Update the state of the RigidBody
        /// </summary>
        internal static void SetRigidBodyKinematic(GameObject targetGameObject, bool isKinematic)
        {
            ModDebugLog.LogDebug( "SetRigidBodyKinematic started...");
            Rigidbody rigidbody = targetGameObject.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative;
                rigidbody.isKinematic = isKinematic;
            }

            ModDebugLog.LogDebug( "SetRigidBodyKinematic done.");
        }

        /// <summary>
        /// Adds the Freeze On Settle component
        /// </summary>
        internal static void AddFreezeOnSettle(GameObject targetGameObject)
        {
            ModDebugLog.LogDebug( "AddFreezeOnSettle started...");
            FreezeOnSettle freeze = targetGameObject.GetComponent<FreezeOnSettle>();
            if (freeze == null)
            {
                freeze = targetGameObject.AddComponent<FreezeOnSettle>();
                freeze.CheckType = FreezeCheckType.Velocity;
                freeze.VelocityThreshold = 0.025f;
                freeze.RayCastDistance = 5f;
                freeze.StartDelay = 2.0f;
            }

            ModDebugLog.LogDebug( "AddFreezeOnSettle done.");
        }

        /// <summary>
        /// Adds the Align to Floor component
        /// </summary>
        internal static void AddAlignToFloor(GameObject targetGameObject)
        {
            ModDebugLog.LogDebug( "AddAlignToFloor started...");
            AlignToFloorOnStart alignToFloor = targetGameObject.GetComponent<AlignToFloorOnStart>();
            if (alignToFloor == null)
            {
                alignToFloor = targetGameObject.AddComponent<AlignToFloorOnStart>();
            }

            ModDebugLog.LogDebug( "AddAlignToFloor done.");
        }

        /// <summary>
        /// Resize the box collider
        /// </summary>
        internal static void ResizeCollider(GameObject targetGameObject, Vector3 colliderCenter, Vector3 colliderSize)
        {
            ModDebugLog.LogDebug( "ResizeCollider started...");
            BoxCollider collider = targetGameObject.GetComponentInChildren<BoxCollider>(true);
            if (collider)
            {
                collider.center = colliderCenter;
                collider.size = colliderSize;
            }

            ModDebugLog.LogDebug( "ResizeCollider done.");
        }

        /// <summary>
        /// Deletes the old model
        /// </summary>
        internal static void RemoveOldModel(GameObject targetGameObject, string modelNameHint)
        {
            ModDebugLog.LogDebug( "RemoveOldModel started...");
            GameObject oldModelGameObject = targetGameObject.FindChild(modelNameHint);
            if (oldModelGameObject != null)
            {
                Object.Destroy(oldModelGameObject);
            }

            ModDebugLog.LogDebug( "RemoveOldModel done.");
        }

        /// <summary>
        /// Configures the Sky and SkyApplier, to ensure
        /// creature mesh shaders don't look "dull".
        /// </summary>
        internal static void ConfigureSkyApplier(GameObject targetGameObject)
        {
            ModDebugLog.LogDebug( "ConfigureSkyApplier started...");
            Pet pet = targetGameObject.GetComponent<Pet>();

            SkyApplier skyApplier = targetGameObject.EnsureComponent<SkyApplier>();
            ModDebugLog.LogDebug( "Pet: ConfigureSkyApplier added SkyApplier component.");

            ModDebugLog.LogDebug( "Pet: ConfigureSkyApplier setting SkyApplier Sky.");
            // skyApplier.SetSky(Skies.BaseInterior);

            ModDebugLog.LogDebug( "Pet: ConfigureSkyApplier updating renderers...");
            Renderer[] creatureRenderers = targetGameObject.GetComponentsInChildren<Renderer>(true);
            ModDebugLog.LogDebug(
                $"Pet: ConfigureSkyApplier found {creatureRenderers.Length} renderers...");
            // skyApplier.anchorSky = Skies.Auto;
            // skyApplier.emissiveFromPower = false;
            skyApplier.dynamic = false;
            if (creatureRenderers.Length > 0)
            {
                skyApplier.renderers = creatureRenderers;
            }

            ModDebugLog.LogDebug( "ConfigureSkyApplier done.");
        }

        /// <summary>
        /// Prevents a Pet from floating on death
        /// </summary>
        internal static void PreventFloatingOnDeath(GameObject targetGameObject)
        {
            ModDebugLog.LogDebug( "PreventFloatingOnDeath started...");
            // Remove the CreatureDeath component, to prevent floating on death
            targetGameObject.DestroyComponentsInChildren<CreatureDeath>();
            ModDebugLog.LogDebug( "PreventFloatingOnDeath done.");
        }

        /// <summary>
        /// Remove the given behaviour from the behavior array
        /// </summary>
        private static Behaviour[] RemoveBehaviourItem(Behaviour[] array, System.Type typeToRemove)
        {
            ModDebugLog.LogDebug( "RemoveBehaviourItem started...");
            List<Behaviour> behaviourList = new List<Behaviour>(array);
            Behaviour behaviorToRemove = behaviourList.Find(x => x.GetType() == typeToRemove);
            behaviourList.Remove(behaviorToRemove);
            ModDebugLog.LogDebug( "RemoveBehaviourItem done.");
            return behaviourList.ToArray();
        }

        /// <summary>
        /// Sets up a PDA Databank entry
        /// </summary>
        internal static void ConfigureDatabankEntry(string encyKey, string encyPath, string mainImageTextureName,
            string popupImageTextureName)
        {
            Texture2D mainImage =
                ModAssetUtils.GetObjectFromAssetBundle<Texture2D>(mainImageTextureName) as Texture2D;
            Sprite popupImageSprite =
                ModAssetUtils.GetObjectFromAssetBundle<Sprite>(popupImageTextureName) as Sprite;
            if (!popupImageSprite)
            {
                Texture2D popupImageTexture =
                    ModAssetUtils.GetObjectFromAssetBundle<Texture2D>(popupImageTextureName) as Texture2D;
                popupImageSprite = ModAssetUtils.GetSpriteFromTexture(popupImageTexture);
            }

            PDAHandler.AddEncyclopediaEntry(encyKey, encyPath, null, null,
                mainImage, popupImageSprite);
        }

        public static void RegisterCustomPet(PrefabInfo prefabInfo, string classId, string bundlePrefabName,
            string audioClipName,
            TechType techType, TechType dnaTechType)
        {
            CustomPrefab prefab = new CustomPrefab(prefabInfo);

            GameObject prefabGameObject =
                ModAssetUtils.GetObjectFromAssetBundle<GameObject>(bundlePrefabName) as GameObject;

            GameObject model = prefabGameObject.transform.Find("model").gameObject;
            Transform petEyes = prefabGameObject.transform.Find("Eyes");
            SimpleMovement simpleMovement = prefabGameObject.AddComponent<SimpleMovement>();

            // Standard components
            PrefabUtils.AddBasicComponents(prefabGameObject, classId, prefabInfo.TechType,
                LargeWorldEntity.CellLevel.Medium);
            PrefabUtils.AddConstructable(prefabGameObject, prefabInfo.TechType, ConstructableFlags.Base, model);
            PrefabUtils.AddVFXFabricating(prefabGameObject, "model", -0.2f, 0.9f, new Vector3(0.0f, 0.0f, 0.0f), 0.7f,
                new Vector3(0.0f, 0.0f, 0.0f));
            prefabGameObject.GetComponent<LargeWorldEntity>().enabled = false;
            MaterialUtils.ApplySNShaders(prefabGameObject);

            // Custom Pet Components
            AddPetComponent(prefabGameObject);
            AddCustomPetComponents(prefabGameObject, audioClipName, AudioUtils.BusPaths.SurfaceCreatures, 10.0f);
            AddPetHandTarget(prefabGameObject);
            
            prefab.SetGameObject(prefabGameObject);

            // Set the recipe, depends on whether in "Adventure" or "Creative" mode.
            RecipeData recipe = null;
            if (SubnauticaPetsPlugin.ConfigFile.ModMode == ModMode.Adventure)
            {
                if (dnaTechType != TechType.None)
                {
                    recipe = new RecipeData(
                        new Ingredient(TechType.Gold, 1),
                        new Ingredient(TechType.Titanium, 1),
                        new Ingredient(TechType.Salt, 1),
                        new Ingredient(dnaTechType, 2));
                }
                else
                {
                    recipe = new RecipeData(
                        new Ingredient(TechType.Gold, 1),
                        new Ingredient(TechType.Titanium, 1),
                        new Ingredient(TechType.Salt, 1));
                }
            }
            else
            {
                recipe = new RecipeData(new Ingredient(TechType.Titanium, 1));
            }

            CraftingGadget crafting = prefab.SetRecipe(recipe);
            prefab.Register();
        }

        /// <summary>
        /// Destroy the EmpAttack component
        /// </summary>
        internal static void DestroyEmpAttack(GameObject targetGameObject)
        {
            ModDebugLog.LogDebug( "DestroyEmpAttack started...");
            targetGameObject.DestroyComponentsInChildren<EMPAttack>();
            ModDebugLog.LogDebug( "DestroyEmpAttack done.");
        }

        /// <summary>
        /// Destroy the AttackLastTarget component
        /// </summary>
        internal static void DestroyAttackLastTarget(GameObject targetGameObject)
        {
            ModDebugLog.LogDebug( "DestroyAttackLastTarget started...");
            targetGameObject.DestroyComponentsInChildren<AttackLastTarget>();
            ModDebugLog.LogDebug( "DestroyAttackLastTarget done.");
        }
    }
}
