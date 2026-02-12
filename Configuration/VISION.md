# Configuration System Vision

- Keep collapsible values as the only source of truth; presets are batch writers only.
- Expose the full tuning surface for limb damage, knockout, mock death, Last Stand, animation pacing, and recovery behavior with stable option names.
- Ensure default values produce readable combat while allowing aggressive/high-fantasy presets without code edits.
- Keep logging controls strictly boolean: `Basic Logs`, `Diagnostics Logs`, and `Verbose Logs`.
- Preserve low-noise debugging workflows with recommended profiles for repro-first diagnosis.
- Keep option sync predictable across runtime updates, avoiding hidden one-off overrides outside option state.
