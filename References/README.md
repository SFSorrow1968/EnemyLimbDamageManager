# B&S Animation Reference Pack

Source install:
- `C:\Program Files (x86)\Steam\steamapps\common\Blade & Sorcery`

## `Managed/`
Core decompile targets for animation/state logic and API names.
- `ThunderRoad*.dll/.pdb/.xml`: game-specific creature, ragdoll, brain, face/eye/mouth, and animation systems.
- `Assembly-CSharp*.dll/.pdb`: additional game-side glue types.
- `UnityEngine.AnimationModule*`, `UnityEngine.DirectorModule*`, `Unity.Animation.Rigging*`, `Unity.Timeline*`, `UnityEngine*`: Unity animation/playables/rigging runtime APIs.

## `Metadata/`
Load-order and assembly context for accurate decompilation.
- `ScriptingAssemblies.json`
- `RuntimeInitializeOnLoads.json`

## `AssetCatalog/`
Addressables lookup data for finding where animation assets are packed.
- `catalog_bas.json`, `catalog_bas.hash`, `bas.jsondb`
- `catalog.json`, `settings.json`

## `Bundles/`
Core asset bundles most relevant to animation names and runtime controllers.
- `animations_assets_all.bundle`
- `creatures_assets_all.bundle`

Recommended first pass for animation investigation:
1. Decompile `Managed/ThunderRoad.dll` with symbols/xml.
2. Use `AssetCatalog/catalog_bas.json` + `AssetCatalog/bas.jsondb` to map asset keys.
3. Inspect `Bundles/animations_assets_all.bundle` and `Bundles/creatures_assets_all.bundle` for clip/controller names.
