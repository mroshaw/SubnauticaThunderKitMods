# Optional VehicleFramework integration

Design proposal, created 26 October 2026; assumptions clarified 27 October 2026. No runtime implementation is included in this change.

**Baseline revision, 27 October 2026:** Nexus release 2.0.8 reports `Loading [VehicleFramework 2.0.7.301]` in `Player.log`, matching the DLL's embedded `BepInPlugin` attribute. The user confirmed that upstream `PluginInfo` retains 2.0.7 and has raised a bug ticket. At the user's direction, the minimum supported VF version is now **2.0.7**, checked with `new Version(2, 0, 7)` against registered BepInEx plugin metadata. No exact-build exception, hashing, alternate API paths or additional compatibility checks are introduced.

This document separates the agreed MVP from later enhancements. The MVP retains one shared Custom Vehicle settings group; enhancements build on it without adding work to the initial iteration.

**Binding implementation requirement:** the MVP and every enhancement must preserve the current mod's architectural consistency and strictly follow its established design, coding standards and conventions, together with the repository's [AGENTS.md](../../../AGENTS.md). The implementation standards below apply to the entire document, including VR Support and Dynamic Installed VF Vehicle Config Options.

## MVP Initial Iteration Design

### Architectural consistency and implementation standards

Extend the existing mod as one coherent codebase. The integration and later enhancements must use the same architectural approach as the current implementation; they must not form a parallel subsystem with different conventions or competing responsibilities.

- **Inspect and extend the existing execution paths.** Preserve the roles of the plugin, Harmony patches, `VehicleMotion`, display/audio controllers, layout components, `ModConfigFile` and `VehicleEnhancementsOptions`. Reuse their configuration and lifecycle paths rather than duplicating them for VF, VR or per-vehicle settings.
- **Keep responsibilities small and consistent.** Retain component-based design and the existing separation of motion/game logic, presentation, configuration and patch setup. Introduce a class or helper only for a concrete responsibility the existing structure cannot reasonably accommodate. Avoid speculative abstractions, generic frameworks and unrelated refactoring.
- **Inherit existing behavior.** Let the game/VF own their native HUD roots and normal visibility. Use Unity's parent/child activation and the existing controller cleanup methods. Preserve vanilla behavior, saved settings, enum values and serialized asset references unless a change is explicitly required by this design.
- **Follow the repository's C# and Unity conventions.** Target C# 7.3; use explicit variable types, established naming, braces and one class per file. Respect Unity object null semantics and serialization. Keep lifecycle methods lean, use Odin attributes for inspector-facing fields, and document public methods with the required XML summaries. Follow existing runtime/editor assembly boundaries and folder organization.
- **Retain the performance approach.** Cache frequently used references and optional API bindings. Avoid per-frame allocations, LINQ, repeated reflection and hierarchy searches in update paths. Use the existing display-update and layout patterns rather than implementing a second update system.
- **Keep configuration, assets and packaging consistent.** Extend the existing Nautilus options/JSON configuration, prefab authoring and ThunderKit build process. Later vehicle profiles must feed the same controllers and settings boundary; VR must reuse the same motion and audio logic. Neither enhancement may introduce a separate configuration or asset pipeline.
- **Review for consistency as well as correctness.** Check the resulting diff against the current mod and AGENTS.md, and verify behavior through the agreed manual gameplay testing. Flag existing inconsistencies rather than silently expanding the scope to fix them. Existing code that conflicts with AGENTS.md is not a precedent for new violations.

These are implementation constraints, not optional recommendations. Any proposed change to the existing architecture must solve a demonstrated requirement in this design and remain the smallest coherent change.

### Requirements, proposals and assumptions

The user-confirmed requirements are:

- Add support for custom vehicles created with VehicleFramework.
- Use the common VF vehicle contract without filtering by vehicle category. Add exceptions only where gameplay testing demonstrates a need.
- Keep VehicleFramework and ConfigurationManager optional: VE must continue functioning without either installed.
- Require VehicleFramework **2.0.7 or later** for custom-vehicle support, using a simple plugin-presence and minimum-version check. Target the standard supported installation rather than accommodating older APIs or unusual dependency failures.
- Keep the solution simple and architecturally sound: reuse existing VE components and the existing Harmony instance, adding only the optional integration and HUD lifecycle handling needed for normal use.
- Strictly retain architectural consistency with the current mod and follow the implementation standards above for the MVP and all enhancements.
- Design for VF 2.0.7's normal behavior. Do not add speculative fallbacks, repair paths or abstractions for hypothetical failures. Retain the required optional-dependency check and ordinary Unity lifecycle checks.
- Add indicators beneath existing game/VF HUD roots and inherit their visibility, scaling and lifecycle wherever possible. Show custom enhancements only while piloting, adding a small local check only where native HUD behavior does not already enforce this.
- Use the downloaded VF and Odyssey sources for investigation, and Odyssey as a basis for testing.
- Validate through manual gameplay testing. Unit tests and automated test suites are outside the current design scope.
- Add one **Custom Vehicle** subheading containing the same options as the other vehicle subheadings. Its values apply to all supported custom vehicles.
- Show the Custom Vehicle subheading and its options only when VF integration is enabled; otherwise omit them.

The design below includes the confirmed decisions recorded in the table, alongside implementation proposals. Source observations are recorded separately under Evidence and compatibility baseline. Confirmation of a design assumption does not establish installed-binary compatibility or successful in-game behavior.

The user has confirmed **A1–A9 and A11–A15**. **A10 has been moved to HUD setup as an implementation detail**, retaining its ID below for reference. Technical behavior still needs to be checked during implementation and gameplay testing, including where the design assumption is confirmed. These checks do not require a separate approval step; follow the agreed task scope and report any material change needed.

| ID | Assumption or proposed default | Reason and validation/consequence |
| --- | --- | --- |
| A1 | **Confirmed:** implement generic VF support through its common vehicle contract; do not require an Odyssey-specific adapter or a registration entry for each vehicle mod. | Fits the requested custom-vehicle support. Odyssey is the first test case, not the integration boundary. Add vehicle-specific exceptions only if a demonstrated incompatibility requires them. |
| A2 | **Confirmed:** use the common VF contract without category-based exclusions for drones, walkers, skimmers or other vehicles. | Require a valid VF vehicle, body transform/Rigidbody and `IsPlayerControlling()`. Reuse VF's own current-vehicle lookup, including remote control. Add an exception only if gameplay demonstrates an actual incompatibility; generic eligibility does not mean every vehicle has been tested. |
| A3 | **Confirmed:** initial delivery targets ordinary monitor play. VR means playing Subnautica with a virtual reality headset; enabling VR support is a separate phase described below. | VF has a separate VR HUD builder. Until that phase is implemented and gameplay-tested, keep custom enhancements inactive in VR while retaining clone configuration. There is no formal VR certification. |
| A4 | **Confirmed:** add the shared indicator prefab as a child of each applicable existing VF HUD root, matching VE's current approach. | Inherit native visibility and scaling. VF selects which HUD variant is active; only that variant's indicator child can run. Bespoke cockpit panels remain outside the proposal. |
| A5 | **Confirmed:** reuse existing VE display calculations, graphics and reversing sounds, preserving vanilla features and saved settings. | Provide the existing enhancement set without new instruments, voices or changes to vehicle physics. Inspect serialized changes and regression-test vanilla vehicles. |
| A6 | **Confirmed:** show custom enhancements only while actively piloting; being inside a walkable vehicle is insufficient. | Let the existing HUD enforce this wherever it already does. If VF also shows its HUD while walking inside, add only a small check controlling our child, never the native root. |
| A7 | **Confirmed:** inherit the native HUD's PDA, pause and other visibility behavior, including existing child/audio lifecycle effects. | No additional implementation is required for this policy. Parent the indicators under the native HUD and reuse existing controller lifecycle methods. Do not add PDA/pause checks, visibility patches, separate audio hosting or special handling to keep audio running when the parent is hidden. Gameplay testing observes the inherited behavior rather than requiring a new policy. |
| A8 | **Confirmed:** a supported vehicle exposes a usable body transform, Rigidbody and accurate control predicate. Body forward/up represent its physical orientation. | Source suggests these common hooks; verify installed VF and Odyssey in game. Camera orientation and animation style are not substitutes. Reject unusable motion data; add an exception only where a differing axis contract is demonstrated. |
| A9 | **Confirmed:** keep integration reflection-only in VE's existing assembly, using cached delegates and VE's existing Harmony instance. | Resolve the known VF 2.0.7 API once; reflection keeps VF optional. No separate patch identifier, patch registry or compatibility layer is needed. Verify normal operation on the target Mono/BepInEx runtime. |
| A10 | **Implementation detail, moved to HUD setup:** reconfigure the indicators VF clones from the Seamoth HUD, rather than destroying and recreating them. | Monitor support uses the existing clones. VR construction is handled separately below. Verify through gameplay that each copy uses custom settings and motion. |
| A11 | **Confirmed:** include the Custom Vehicle subheading and options only when VF integration is enabled. | Use the result of the existing startup check/initialization when constructing options. Otherwise omit the group entirely. No disabled controls, availability messages, separate integration toggle or dynamic menu rebuilding is needed. |
| A12 | **Confirmed:** default custom instruments and beeps to enabled, generic reversing voice to selected, and adjustment multipliers to `0, 0, 1`. Reuse existing slider ranges and global audio volume/logging. | Defaults and the shared option set are confirmed; exact serialized member names remain implementation proposals. Tune the common base layout against Odyssey without making its settings exclusive to Odyssey. |
| A13 | **Confirmed:** existing audio emitter placement, volume and low-pass processing can be reused with a common custom-vehicle offset. | Verify audibility inside Odyssey and across vehicle sizes. Do not assume one offset gives identical acoustic results in every vehicle. |
| A14 | **Confirmed:** validate a second independently implemented VF vehicle before claiming broad tested compatibility. | Select the second vehicle during implementation; no particular additional mod has been chosen. Odyssey remains the specified initial test basis. |
| A15 | **Confirmed:** use disposable saves for manual gameplay testing and inspect the packaged files to confirm optional dependencies are not bundled. | No in-game tests or dependency removals have been completed for this design. VF 2.0.7 is the agreed baseline; successful runtime behavior remains to be tested. |

Unless a later section explicitly reports an observation, its concrete filenames, initialization sequence, layout choices, defaults, failure policies and acceptance criteria describe the proposed implementation under these assumptions. They must not be presented as already implemented or tested.

### Objective and decisions

Extend Vehicle Enhancements SN (VE) to vehicles derived from VehicleFramework's `ModVehicle`, using Odyssey as the first acceptance vehicle. Provide the existing speedometer/VSI, HSI, time/weather and reversing audio while the player pilots a supported vehicle.

Use a small, reflection-only integration inside the existing VE assembly. Resolve the optional API once, retain ordinary game/Unity references and cached delegates, and reuse the existing controllers. Attach the indicator prefab beneath VF's existing HUD roots, following the existing vanilla integration. Let the parent HUD supply normal visibility, scaling and object lifetime; add only the piloting restriction that VF does not already provide.

The following are proposed engineering constraints for meeting those requirements:

- VE loads and its Seamoth, Prawn and Cyclops features work with neither VehicleFramework nor ConfigurationManager installed.
- No assembly reference, hard BepInEx dependency, asset-bundle script, package requirement or staged DLL introduces either optional dependency.
- Check BepInEx's registered VF plugin version once at startup: absent means no integration; metadata below 2.0.7 means skip integration with an update warning; metadata 2.0.7 or later means initialize the integration.
- Keep one initialization error boundary so an unexpected integration error is logged without aborting vanilla startup. Do not build additional dependency-health checks or recovery machinery.
- Configuration remains in VE's existing Nautilus options and JSON file. Do not access VehicleFramework's configuration system or ConfigurationManager.
- Preserve existing vanilla settings, serialized prefab fields and enum values.

Generic support without per-mod registration or category filtering, reuse of native HUD roots and a separate VR phase are confirmed under A1–A4. A VF vehicle supplying the common control and motion contract is eligible without a dedicated adapter. Return no motion when required references or control state are unavailable; do not maintain a vehicle-category allowlist or denylist.

### Evidence and compatibility baseline

All paths below are relative to the repository root. The actual downloaded-source directory is `GameFiles~/ExternalMods`, rather than `GameFiles/~/ExternalMods`. Treat it as read-only.

Abbreviations:

- **VE**: `Assets/Mods/VehicleEnhancements_SN`.
- **VF**: `GameFiles~/ExternalMods/VehicleFramework/VehicleFramework/VehicleFramework` (the active project; exclude `BACKUP` and `MigrationBackup`).
- **OD**: `GameFiles~/ExternalMods/OdysseySubmarine/OdysseyVehicle`.

| Source | Observed behavior and design consequence |
| --- | --- |
| VE `Scripts/VehicleEnhancementsPlugin_SN.cs` | `Awake` loads config, registers Nautilus options and audio assets, then calls `Harmony.PatchAll()`. Optional initialization must be isolated from these required operations. |
| VE `Scripts/Patches/UGuiPatches.cs` | `uGUI.Awake` installs `EnhancedIndicators` beneath vanilla Seamoth and Prawn HUD roots. Its helper configures four controllers and a HUD layout. |
| VE `Scripts/Patches/CyclopsHelmHudPatches.cs` | Cyclops uses authored world-space panels and separate piloting hooks. These locations cannot be reused for Odyssey merely because it is a submarine. |
| VE `Scripts/Enhancements/VehicleMotion.cs` | `EnhancedVehicle` has three entries. Motion lookup casts to vanilla vehicles or requires a Cyclops `SubRoot`. It cannot currently supply custom-vehicle motion. |
| VE controllers and `VehicleHudElementLayout.cs` | Vehicle selection determines both layout and settings. Most default branches mean Seamoth; adding a new enum value alone silently reuses Seamoth behavior. |
| VF `PluginInfo.cs`, `MainPatcher.cs` | Plugin GUID is `com.mikjaw.subnautica.vehicleframework.mod`; local source declares `2.0.7.<BuildInfo.BuildVersion>`. The inspected plugin attributes explicitly depend on Nautilus. |
| VF `ModVehicle/ModVehicle.cs` | `VehicleFramework.ModVehicle : Vehicle`. `IsPlayerControlling()` is virtual; `BeginPiloting`/`StopPiloting` broadcast status through VF listener interfaces. The inherited `useRigidbody` supplies physical velocity. |
| VF `VehicleTypes/Submarine.cs` | `IsPlayerInside()` and `IsPlayerPiloting()` are separate. Piloting is seat state. `BeginPiloting()` calls the base before setting `isPilotSeated`, so a base-method postfix can observe incomplete state. |
| VF `Extensions/ExtensionMethods.cs` | `GetModVehicle(Player)` selects a mounted drone, then `Player.GetVehicle()`, then a component on `Player.currentSub`. This handles walkable submarines where `GetVehicle()` alone is insufficient. |
| VF `VehicleBuilding/HUDBuilder.cs` | Normal and storage HUDs clone the Seamoth HUD. VR has a separate builder. Cloning can also duplicate VE components already attached to the Seamoth HUD. |
| VF `MiscComponents/uGUI_VehicleHUD.cs` | Actual type name is `VehicleFramework.MiscComponents.UGUI_VehicleHUD`. Its `root` is initialized by the builder, not necessarily during `Awake`. VF selects normal/storage roots and hides them for PDA use; being inside a submarine can be sufficient for VF's native HUD. |
| VF `Patches/PlayerPatcher.cs` | `Player.Awake` postfix calls `HUDBuilder.DecideBuildHUD()`. Installing VE only in `uGUI.Awake` is too early to assume the VF HUD exists. |
| OD `Odyssey.cs`, `MainPatcher.cs` | `Odyssey : Submarine` has its own cameras and engine; declares Seamoth piloting animation style. Source plugin is `com.mikjaw.subnautica.odyssey.mod`, version `2.0.2`, with a VF dependency. Animation style is not a vehicle classification or HUD contract. |

When checked on 26 October 2026, the [Vehicle Framework Nexus page](https://www.nexusmods.com/subnautica/mods/859) listed version **2.0.8**, updated 23 June 2026, and listed Configuration Manager, Nautilus and BepInEx as requirements. The downloaded source does not establish that it matches that release. Installed files were found under `E:\Games\Steam\steamapps\common\Subnautica\BepInEx\plugins\VehicleFramework`, `OdysseyVehicle` and `ConfigurationManager`; DLL file versions are `1.0.0.0` and are not a reliable plugin-version identifier. These are investigation-time observations, not confirmation that the installed dependency set loads successfully.

**Agreed baseline:** implement against VehicleFramework **2.0.7**, with **2.0.7 as the minimum supported version**. Use the installed 2.0.7 API as the reference where the downloaded source differs. Confirm any needed hook details while implementing, then test with Odyssey. This is normal implementation work, not a runtime compatibility audit: no assembly hashing, API-shape validation, version-specific adapters or exhaustive source/binary comparison is required. Accept later versions through the same minimum-version check and API path; report versions actually tested in release notes.

`ProjectSettings/ProjectVersion.txt` identifies Unity **2019.4.36f1**. Build VE with C# 7.3; do not copy newer language features from the VF source. Verify the installed Nautilus version against VF's needs before testing.

### Architecture

```text
VE plugin: required startup and vanilla Harmony patches
    |
    +-- optional initialization (plugin GUID + version >= 2.0.7)
            |
            +-- HUD-builder postfix, using VE's existing Harmony instance
                   +-- reconfigure cloned indicators beneath each VF HUD root
                          +-- existing display, layout and audio controllers
                          +-- small custom HUD controller on native root, only if needed

VF controls its HUD roots; our indicator children inherit that behavior
VehicleMotion.TryGet(CustomVehicle) obtains the current custom vehicle's motion
Existing vanilla cases continue to resolve motion as before
```

Proposed files, all within VE's existing runtime assembly:

| File | Responsibility |
| --- | --- |
| `Scripts/Integration/VehicleFrameworkIntegration.cs` | One startup check, cached VF API references and application of the HUD-builder postfix through VE's existing Harmony instance. No external types in signatures. |
| `Scripts/Integration/VehicleFrameworkCustomHudController.cs` | If required, a small component on a native VF HUD root that toggles only our indicator child according to piloting state. |

Use the integration class for optional initialization and reconfiguring cloned indicators, plus the existing motion/configuration helpers. Add a small custom HUD controller only if the native root remains visible while not piloting. There is no independent HUD manager or scene-wide visibility controller. Reuse existing component lifecycle methods for cleanup.

#### Optional loading boundary

1. Add a literal-string `[BepInDependency("com.mikjaw.subnautica.vehicleframework.mod", BepInDependency.DependencyFlags.SoftDependency)]` to VE for optional load ordering. Keep this attribute unversioned and enforce the minimum in our check, so an older VF installation does not prevent VE's vanilla features from loading. Do not reference VF's `PluginInfo` constant from compiled code.
2. After required VE startup, check `Chainloader.PluginInfos` once in VE `Start` using the VF GUID. If absent, return quietly. If `Metadata.Version < new Version(2, 0, 7)`, log one warning that custom vehicles require VF 2.0.7 or later, then return. Otherwise initialize custom-vehicle support. Use BepInEx plugin metadata, not DLL file versions or archive names.
3. Resolve only the known VF 2.0.7 types/members needed by the integration from the registered plugin's assembly: `ModVehicle`, `ExtensionMethods.GetModVehicle(Player)`, `IsPlayerControlling()`, `HUDBuilder.DecideBuildHUD()` and `UGUI_VehicleHUD.root`. Cache these references and bind the current-vehicle lookup once to a game-facing delegate. Bind a cached `Func<bool>` for the control predicate when the vehicle changes. Do not resolve subtype identities, probe alternative signatures or validate the rest of VF.
4. Resolve the required references first, then apply the known builder postfix with VE's existing Harmony instance using `Harmony.Patch(MethodInfo, ...)`. Mark the integration initialized and set up any already-created HUD. Keep this optional callback free of auto-discovered `[HarmonyPatch]` attributes so required `PatchAll()` never searches for VF when it is absent. This manual call simply allows patching after the version check; it does not require separate patch ownership.
5. Use one initialization `try/catch`: log an unexpected error and leave custom support inactive, with vanilla VE running. The callback returns if integration is inactive. No separate Harmony instance, unpatch/rollback system, dependency retries or recovery framework is needed. Normal scene/HUD lifecycle handling remains separate from dependency checking.

Register VE's Nautilus options once after this optional initialization attempt, including its absent/older/error outcomes. Always register vanilla options; add the Custom Vehicle group only if integration is enabled. Move the existing options registration from `Awake` to this point rather than rebuilding the menu later.

No `using VehicleFramework`, `typeof(VehicleFramework...)`, optional base class/interface or serialized optional component is allowed in the VE assembly. Attribute arguments, static initializers and other metadata/code expressions must not reference symbols from an optional assembly; the literal GUID soft dependency is safe. Retain current `.asmdef` precompiled references and ThunderKit merge/staging boundaries. Do not include VF/Odyssey/ConfigurationManager binaries or source in VE's package.

BepInEx and VehicleFramework are responsible for VF's own dependencies, including ConfigurationManager where required. VE does not inspect or validate them. The standard use case assumes a working VF 2.0.7-or-later installation; without a registered VF plugin, VE simply retains its vanilla functionality.

#### Runtime vehicle context

Extend the existing `VehicleMotion` lookup for custom vehicles; do not introduce a separate runtime context manager. Cache the current vehicle/body and bound VF control predicate as needed, refreshing them when the player or candidate changes:

- Obtain the candidate through VF's existing `ExtensionMethods.GetModVehicle(Player)` helper using the cached delegate, and use its game `Vehicle` base for motion. This lets VF select the current vehicle, including a remotely controlled drone, without duplicating its selection logic. Check Unity validity, body/Rigidbody availability and `IsPlayerControlling()`; do not filter vehicle subtypes or add a separate remote-control path.
- On candidate identity change, clear the old binding and cache the new `Vehicle`, body transform, `useRigidbody` and bound control predicate. Reuse existing controller initialization/reset behavior; add a local reset only if gameplay reveals stale readings or audio when switching directly between vehicles.
- Require the candidate's `IsPlayerControlling()` result for both enhanced displays and reversing audio. `currentSub`, `isPlayerInside`, nonzero velocity and piloting animation style are not sufficient.
- For Odyssey, use the vehicle body transform's forward/up and `useRigidbody.velocity`. Validate its axes in game. Never use the player, camera or moving cockpit transform for attitude. If another vehicle uses different physical axes, defer it until an explicit supported mapping is known.
- Inherit PDA, pause, docking and general HUD visibility from the native hierarchy. When a native root deactivates, Unity disables its indicator child and the existing reversing controller's `OnDisable` stops audio. Do not keep an independent audio object alive to bypass that behavior.
- Return no motion when the player, vehicle or Rigidbody is unavailable, or the player is not controlling the vehicle. Do not infer control from thrust or power, since an unpowered craft may still be moving backward.

Use ordinary null checks for managed objects and Unity equality for `Vehicle`, `Transform`, `Rigidbody`, `GameObject` and other Unity objects. Do not retain delegates targeting destroyed vehicles. Avoid component searches, reflection invocation, LINQ, closures and collections allocated per frame; delegate creation and metadata lookup occur only during initialization or rebinding.

#### HUD setup and reuse of cloned indicators

Reuse the `EnhancedIndicators` child VF clones from the Seamoth HUD beneath each applicable native root, including its normal and storage variants. This is one instance per HUD root, not per vehicle. VF chooses which root to show, so only the selected variant's child is active in the hierarchy. Do not create a sibling overlay or reproduce VF's HUD selection, PDA handling or visibility rules.

After `HUDBuilder.DecideBuildHUD()` returns, find each native root's existing indicator child and configure its controllers for `CustomVehicle`. Call the same setup method once during optional initialization to handle roots that already exist. If the HUD has not been built yet, let the builder callback handle setup. Monitor support relies on VF cloning the already-enhanced Seamoth HUD; it does not instantiate replacement indicators or provide a create-if-missing fallback. Repeated setup reuses the same objects and components.

**Configure the copies even if all custom features are switched off.** Enumerate the current HUD's VF `UGUI_VehicleHUD` components through the cached optional type and read their initialized `root` references during setup. Temporarily deactivate each indicator child, configure its speedometer, HSI, time/weather and reversing audio for `CustomVehicle`, then restore its eligibility through the normal parent/piloting rules. Do not modify the original Seamoth indicators or destroy a valid clone.

Make the existing `Configure` methods safe to call again: get and reconfigure an existing `VehicleHudElementLayout`/`VehicleHudLayout` instead of unconditionally adding another. Apply the authored custom base positions/scales before recapturing layout baselines, so copied Seamoth adjustments do not carry over or accumulate. Keep panels and audio together and inherit native parent scaling. Runtime-created helper children, such as the time display's moon images, should be reused/rebound when present rather than duplicated by cloned-object initialization. Keep these changes local to the existing components; no clone-management system is needed.

**Piloting is the only additional visibility condition.** The inspected VF HUD can remain visible while the player walks inside a submarine. Where that behavior exists in VF 2.0.7, place a small custom HUD controller component on the native root and have it enable/disable only our indicator child using `IsPlayerControlling()`. The controller stays on the parent so it can re-enable the child when the player sits down. It never activates or deactivates VF's root. Use a simple early update before our display/audio controllers; no piloting state machine or separate seat-entry/exit patches are needed. If the native root already limits visibility to piloting, omit the controller.

VF and Unity handle variant changes and object destruction through the hierarchy. Existing `OnDisable`/`OnDestroy` methods stop audio and release DSPs; retain those paths. Clear cached vehicle references when they become invalid or change. Reuse setup when new HUD roots are built after loading a save. Check through gameplay that only one variant's indicators and audio run at a time; do not add another HUD coordinator to manage native variants.

Until the separate VR phase below is completed, keep custom enhancements inactive in VR, log the limitation once and retain clone configuration and vanilla functionality.

### Minimal changes to existing code and assets

1. Append `CustomVehicle` to `EnhancedVehicle`; do not reorder existing values. Add an explicit custom branch to every settings, motion and layout switch. Unknown enum values must not accidentally select custom support.
2. In `VehicleMotion.TryGet`, read the integration's cached validated context only for `CustomVehicle`. Preserve the existing signed forward-speed, world-space vertical-speed and pitch/roll calculations. No VF reflection belongs in individual display controllers.
3. Extend controller configuration and `VehicleHudElementLayout` with custom settings/default placement. Make configuration reuse existing layout components so cloned indicators can be reconfigured in place. Inherit native parent visibility and scaling; add no separate PDA visibility or audio policy. Keep vanilla `Configure(EnhancedVehicle)` callers working.
4. Keep `UGuiPatches` as the vanilla hook. Reuse its controller-configuration steps for existing VF clones, extracting a small helper only if needed. Leave VR instantiation to the separate VR phase; do not redesign all HUD construction.
5. Reuse `ReversingAudioController`: generic voice, body-relative emitter offset, existing volume and low-pass behavior. Verify Odyssey's size/interior attenuation before choosing its default offset; no new voice recording or Odyssey-specific asset is required. Add explicit stop/reset on context loss and vehicle switches where existing activation alone is insufficient.
6. Update the existing prefab/setup scene through serialized attributes for custom default positions/scales. Preserve script GUIDs and existing serialized data; rebuild `enhancedvehiclesassetbundle` with the implementation.

Existing public configuration fields and underscore-prefixed audio fields do not follow all current repository conventions. Flagged here for awareness; do not change their serialization or unrelated naming as part of this integration.

### Configuration and user behavior

Add one **Custom Vehicle** subheading within the existing Vehicle Enhancements SN options, using the option-order range `400–499` and keeping General at `500+`. It contains the same set of options as each vanilla vehicle subheading. One shared set of values applies to all supported custom vehicles; there are no per-model settings or dynamically generated vehicle sections.

Proposed persisted members:

- `EnableCustomVehicleSpeedometer`, `EnableCustomVehicleHsi`, `EnableCustomVehicleTime` (default true).
- `CustomVehicleReversingVoice` (default `ThisVehicleIsReversing`) and `EnableCustomVehicleReversingBeeps` (default true).
- `CustomVehicle{Speedometer,Hsi,Time}{HorizontalAdjust,VerticalAdjust,UiScale}` using existing ranges and neutral `0, 0, 1` adjustments.

Use the same labels, ordering, voice choices, slider ranges and save behavior as the vanilla vehicle options: Speedometer, HSI, Time and Weather, Reversing Voice, Reversing Beeps, then horizontal adjustment, vertical adjustment and UI scale for each display. Do not add a separate integration toggle. Detect optional support automatically, and let the shared feature options control enhancements on every supported custom vehicle.

Include the subheading and its controls only when VF 2.0.7-or-later integration is enabled. Use that existing startup result to conditionally add the group during options construction; no additional availability checks or menu updates are needed. Vehicle spawn state and HUD visibility do not affect option availability. Changes take effect on the current custom vehicle and remain in force when switching to another. Keep clone configuration installed when integration is initialized even if all custom features are off. Installing/updating VF requires a game restart; do not support hot loading.

Existing JSON settings and `ReversingVoice` numeric values remain unchanged. New fields use defaults when absent, with the same save behavior as existing options. Global reversing volume and detailed logging apply to custom vehicles too.

### Implementation sequence

1. **Use the agreed baseline:** reference VF 2.0.7 for the required integration hooks and confirm Odyssey's axes during gameplay testing. No compatibility audit or support for older VF APIs is needed.
2. **Optional initialization:** check VF presence/version once, cache the required API references and apply the HUD-builder postfix using VE's existing Harmony instance. Verify that absent or older VF leaves vanilla VE running.
3. **Motion and settings:** add the custom enum/config branches and minimal controller changes, preserving vanilla serialization and paths.
4. **HUD integration:** reconfigure VF's existing cloned indicators and add only the custom HUD controller needed by A6. Inherit native visibility and existing component cleanup. Author shared default layout and rebuild assets.
5. **Gameplay testing and review:** manually run the scenarios below with Odyssey and the vanilla vehicles, check the game logs for errors, and inspect the packaged files. Review the changes for correctness and lifecycle issues. Update README with supported/tested VF/Odyssey versions and limitations after gameplay testing passes.

### Gameplay testing and acceptance

Use a disposable test save. Do not overwrite a valued save after removing vehicle mods. Dependency removal tests require a full game restart, with the same built VE package; restore the installed plugins after testing. No plugins were removed or game sessions run while preparing this design.

| Installed configuration | Required result |
| --- | --- |
| VE + existing required dependencies; VF, Odyssey and ConfigurationManager absent | VE loads, vanilla options/enhancements work, and the Custom Vehicle group is absent; no missing-assembly/type-load errors. |
| ConfigurationManager present, VF/Odyssey absent | Same vanilla result; VE never queries ConfigurationManager. |
| Registered VF metadata below 2.0.7 | Custom integration is skipped with one update warning and the Custom Vehicle group is absent; vanilla VE continues working. |
| Working VF 2.0.7 and its dependencies present, Odyssey absent | Integration initializes and the Custom Vehicle group is available without requiring a spawned/registered custom vehicle; vanilla behavior is unchanged. |
| Working VF 2.0.7 + Odyssey + their required dependencies | Odyssey acceptance cases below pass; only one VE HUD/audio pair runs. |
| Shared Custom Vehicle feature options changed with VF present | Changes apply immediately and carry over to every supported custom vehicle; disabling all features leaves custom indicators disabled. |

Odyssey acceptance cases:

- Spawn/build, board and walk inside: no VE pilot displays or reversing audio. Sit at the helm: exactly one indicator set appears. Stand up while drifting backward: audio stops and displays disappear.
- Move forward/backward and ascend/descend: signed speed/VSI match the body's velocity. Pitch/roll match the hull while looking around and switching Odyssey cameras. Camera orientation must not alter HSI or reversing detection.
- Open/close PDA, pause, lose power, dock/undock, die and destroy/unload the vehicle. Observe the inherited native HUD behavior and existing audio cleanup; confirm no stuck indicators, continuing emitters after deactivation or errors in the game log. Do not require new PDA/pause behavior.
- Switch VF normal/storage HUD, adjust game UI scaling, resolution/aspect ratio and VE positions/scales. No duplicate panels, offscreen defaults, layout jitter or doubled scaling.
- Change voice/beeps/volume while reversing, switch directly between two custom vehicles, then Seamoth/Prawn/Cyclops. No stale audio position, previous-vehicle smoothing or settings leakage.
- Save/load while inside and while piloting, return to menu and load a second save repeatedly. Confirm one indicator instance per applicable native root, only one visible variant at a time, and no stale panels or continuing audio.
- Before the separate VR phase is complete, exercise VR for safe opt-out and clone configuration. When enabling VR, run the headset gameplay checks in VR Support. Under A14, add a second independently implemented VF vehicle before advertising broad tested compatibility; Odyssey alone verifies one submarine implementation. Where the tested vehicle offers remote control, confirm the instruments follow the controlled vehicle rather than the player's location or host vehicle.

Gameplay testing is the acceptance method: observe the indicators, audio, options and vehicle/session transitions directly, and check the game logs when something behaves incorrectly. Confirm that Seamoth, Prawn and Cyclops enhancements still work with and without VF installed. Build/compile checks remain part of producing the mod; a successful compile alone does not demonstrate correct gameplay behavior. No unit tests, automated test suites or mandatory profiler captures are planned.

Release acceptance requires the **same distributable** to pass both the absent-dependencies scenario and Odyssey tests. Inspect its AssemblyRef metadata, BepInEx attributes, bundle script dependencies and ThunderKit staging output: no VF, Odyssey or ConfigurationManager dependency or binary may be introduced.

## Enhancements

These are later implementation phases. Their proposed details do not change the MVP's confirmed shared-options behavior.

Both enhancements must strictly follow Architectural consistency and implementation standards above. They extend the same mod components and conventions; they do not authorize an architectural rewrite or a separate implementation framework.

### VR Support

This is a separate implementation phase using the same generic VF integration. It adds support for playing with a headset; it does not require another vehicle adapter, dependency-checking system or set of mod options. The initial monitor implementation can be completed first.

#### Implementation

1. **Use the existing VR mode and HUD path.** Follow VF 2.0.7's use of `UnityEngine.XR.XRSettings.enabled` and its `HUDBuilder.BuildVRHUD` path. Keep the same startup version check and existing Harmony instance. Use the normal builder callback to set up the HUD after VF finishes construction.
2. **Construct indicators for the separate VR HUD path.** The inspected VF code clones `VRVehicleCanvas/Seamoth`, where current VE code does not attach indicators. In this VR phase, use the existing prefab helper to attach/configure indicators beneath VF's resulting VR HUD root. This is the normal VR construction path, not a fallback for monitor support. Confirm the root and base placement in a headset; inherit native activation/scaling and adjust existing layout only where testing demonstrates a need.
3. **Keep one indicator instance per native root and one settings group.** Reuse the **Custom Vehicle** options, including position/scale adjustments, voice and beeps. VR can have separately authored base placement, but the user's saved adjustments remain shared; do not add a VR-specific menu or enable toggle.
4. **Keep vehicle data independent of head tracking.** Continue reading attitude and velocity from the vehicle body. Looking around, leaning, recentering the headset or using Odyssey's cameras must not change instrument readings or reversing detection. Keep reversing audio positioned relative to the vehicle and verify it through the game's existing audio listener; do not add another listener or a separate audio system.
5. **Enable the existing custom-vehicle path in VR.** After headset gameplay checks pass, remove the initial VR exclusion and its limitation log. Retain the minimal piloting check and inherit native HUD visibility and existing cleanup, just as in monitor mode. Do not introduce VR-specific PDA/pause or audio visibility handling.

#### Headset gameplay checks

Use Odyssey with VF 2.0.7 and record the headset/VR setup tested. Additional VR modifications, if used, must be named in the test notes; compatibility with every VR setup is not assumed.

- Board Odyssey, walk inside, sit at the helm and stand up: exactly one indicator set appears only while piloting, and reversing audio stops on exit.
- Confirm both eyes see readable, comfortably placed indicators without clipping, flicker or overlap with VF's HUD. Look around, lean and recenter; the panels should follow the intended native HUD behavior.
- Drive forward/backward, ascend/descend and pitch/roll the hull. Move the headset independently and switch Odyssey cameras: readings remain tied to the vehicle, with no head-driven reversing sounds.
- Open/close the PDA, change the shared position/scale and audio options, and exercise the VF HUD variants available in VR. Confirm the controls work and no duplicate indicators or emitters appear.
- Save/load, return to the menu and load again, and switch between custom and vanilla vehicles. Check for stale panels, continuing audio or errors in the game log.
- Restart in ordinary monitor mode and confirm the same build and saved settings still behave correctly.

VR support is enabled in the delivered mod after these manual checks pass. Until then, the monitor implementation and VR clone configuration remain the supported behavior.

### Dynamic Installed VF Vehicle Config Options

#### Goal and scope

Generate an options subheading for each installed, registered VF vehicle type, such as **Odyssey**, with the same instruments, layout and reversing-audio options as the MVP's **Custom Vehicle** group. Settings apply to all instances of that type. A vehicle does not need to be spawned or unlocked for its options to appear.

Use registered vehicle types rather than BepInEx plugin names: one vehicle mod may register several types, and a plugin dependency alone does not identify its vehicles. Discovery only supplies settings sections; generic vehicle support remains unchanged and requires no per-mod adapter.

This is a proposed enhancement, not part of the MVP. Retain **Custom Vehicle** as the shared defaults group. Each named vehicle section offers **Use Custom Vehicle defaults** (initially enabled). Turning it off enables that vehicle type's own copy of the same option set. This extra choice belongs only to this enhancement; it does not change the agreed MVP options.

#### Discovery and menu construction

The inspected source provides `VehicleFramework.Admin.VehicleManager.vehicleTypes` and `GetVehicleTypesWhere`, with entries defined by `VehicleBuilding.VehicleEntry` (`mv`, `name`, `techType`). The registry represents registered prefabs, unlike `VehiclesInPlay`, which represents spawned instances. `VehicleRegistrar.RegisterVehicle` runs as a coroutine, and `VehicleBuilder.Prefabricate` briefly adds an entry with `TechType.None` before craftable registration supplies the completed entry.

Read the completed registry entries when Nautilus builds VE's options panel, using the existing `BuildModOptions` override. Skip entries whose `techType` is `None`, sort by display name and add one named section per type. Cache the small amount of optional reflection required, using the same VF 2.0.7 integration. Do not scan installed DLLs, search the scene or poll for vehicles.

Registration may finish after VE's startup options registration. Building the list at panel construction avoids assuming it is complete in `Start`. If registration completes while a panel is already open, include the new entry the next time the panel is built; no live refresh system or registration patches are proposed. Keep one registered VE options object and update only its generated option items/heading data during that build, avoiding duplicate items and event handlers. Verify the installed Nautilus panel rebuild behavior while implementing this enhancement.

#### Settings identity and runtime use

Store profiles in VE's existing JSON configuration, keyed by the registered TechType's **string identifier**, not its numeric value, localized label, hull nickname or VF registration index. Use a display name only for the heading. Match the currently controlled vehicle through its registered TechType using the existing game/vehicle information. Confirm the normal VF 2.0.7 identifier mapping during implementation; no plugin-to-vehicle ownership map is required.

A small serializable `CustomVehicleSettings` class can hold the existing option set, with a dictionary of type identifiers to settings and a per-profile use-defaults flag. Introduce it in this enhancement when multiple profiles create a concrete need; do not refactor the MVP into a profile framework in advance. Existing shared custom settings remain the defaults, and old JSON files continue to work unchanged.

Resolve the effective settings when the controlled vehicle changes and when the user changes its options. Pass those settings through the existing config/controller boundary; retain the same cloned HUD objects, native visibility and motion calculations. Changing Odyssey's profile must not change another type's settings. Global volume and logging remain global.

Only registered vehicle types appear in the menu, and all custom sections remain absent when VF integration is disabled. Keep saved profiles for removed vehicle mods so reinstalling restores their choices; do not add removal prompts or profile management screens.

#### Manual gameplay checks

- Install Odyssey and a second VF vehicle: both named sections appear without spawning either vehicle. Multiple instances of Odyssey share one profile and one section.
- Keep default inheritance enabled: both types use the shared Custom Vehicle values. Override Odyssey, change its layout/audio settings and switch vehicles: only Odyssey uses the override.
- Save/restart and reopen options: choices persist. Return a vehicle to shared defaults and confirm subsequent shared changes apply to it.
- Remove/reinstall one vehicle mod between game launches: its section disappears/reappears and its saved profile is retained. Restart without VF: all custom sections are absent and vanilla options still work.
- Open options before and after normal vehicle registration completes: later entries appear on the next panel build, with no duplicated controls or repeated save handlers.
