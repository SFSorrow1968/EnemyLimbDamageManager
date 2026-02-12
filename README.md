# Ultimate Injury Overhaul (UIO)

Ultimate Injury Overhaul disables individual enemy limbs when enough damage is dealt to that limb.  
Each limb can recover through configurable Last Stand logic, with knockout and death-mimic behavior.

## Menu model

- Presets are batch editors only.
- Collapsible values are the source of truth.
- Preset changes are synced into collapsibles at runtime.

## Core behavior

- Left Leg and Right Leg can be disabled independently.
- Left Arm and Right Arm can be disabled independently.
- Leg disable can force destabilize and clamp movement.
- Arm disable can disable ragdoll arm/hand interaction handles.
- Last Stand recovery restores each limb after the configured duration.

## Build outputs

- `bin/PCVR/EnemyLimbDamageManager/EnemyLimbDamageManager.dll`
- `bin/Nomad/EnemyLimbDamageManager/EnemyLimbDamageManager.dll`

