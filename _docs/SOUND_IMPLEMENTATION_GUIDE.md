# Sound Implementation Guide

## Scope
Use this guide whenever adding or changing runtime sounds for Ultimate Injury Overhaul (UIO).

## Core Rules
- NPC-only: never play injury/death voice sounds for player creatures.
- Gender-aware: resolve male/female/neutral pools and apply deterministic fallback order.
- Position-aware: voice sounds must follow a live anchor (`jaw` preferred, then head/neck, then creature root).
- Managed source tagging: all mod-spawned audio sources must use the managed prefix so they can be excluded from vanilla scans.

## Kill Voice Routing
- Neck-slice kill (`DamageType.Slash` on neck): play neck-slice gurgle only.
- Kill while in death-mimic state: play short gurgle only.
- Any other kill: play aspiration.

## Vanilla Voice Interop
- Before kill sync starts, measure active vanilla vocal duration on likely voice sources.
- Suppress competing vanilla vocal sources once a modded kill voice is chosen.
- Extend kill-sync window if vanilla vocal duration outlasts the initial estimate.

## Mouth Sync
- Keep death mouth presentation slightly open by default.
- Apply low-frequency oscillation tied to kill-audio elapsed time.
- Smooth factor updates frame-to-frame to avoid jaw jitter/spasms.
- Restore original jaw rotation when presentation ends.

## Animation Sync
- Compute kill animation speed from effective vocal duration (modded vs vanilla max).
- Clamp to safe bounds so animation can only slow down, not accelerate above normal in kill sync mode.
- Extend despawn/death animation window to cover remaining vocal duration.

## Mix Guidance
- Keep death vocals below disable/break impacts in perceived loudness.
- Keep bone crack layers clearly audible on disable events, especially tier-2 and slash/pierce disables.

## Logging
- Log all sound routing decisions at `Info` level with mode, gender pool, and clip selected.
- Log fallback reasons and suppression counts.
- Use diagnostics-only logs for high-frequency mouth-sync factor updates.
