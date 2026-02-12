# Agent Instructions

When making changes in this repo:
- Always build `Release` and `Nomad` before finalizing.
- Shared game DLL path for this workspace is `D:\Documents\Projects\repos\BS\libs`.
- Build artifacts are expected in `bin/PCVR/EnemyLimbDamageManager/` and `bin/Nomad/EnemyLimbDamageManager/`.
- Use a feature branch for every task (for example `agent/<topic>`), and avoid direct work on `main`/`master`.
- Presets are batch editors only; collapsible values remain source-of-truth.
- Keep logging controls boolean only (`Basic Logs`, `Diagnostics Logs`, `Verbose Logs`).
- When a user reports an issue, always recommend a specific boolean logging profile first.
- For low-noise repro in this repo, start with: `Basic Logs=On`, `Diagnostics Logs=Off`, `Verbose Logs=Off`.
- If runtime decision details are needed, set `Diagnostics Logs=On` (keep `Verbose Logs=Off`).
- Only set `Verbose Logs=On` for short targeted repro sessions.
- Sound implementation must follow `EnemyLimbDamageManager/_docs/SOUND_IMPLEMENTATION_GUIDE.md`.

## System Vision Files
- `Core/*`: read and follow `Core/VISION.md`.
- `Configuration/*`: read and follow `Configuration/VISION.md`.
- `Hooks/*`: read and follow `Hooks/VISION.md`.
- `Sounds/*`: read and follow `Sounds/VISION.md`.

When a change touches one of these systems, treat that system's `VISION.md` as required context before editing.  
When a change spans multiple systems, satisfy all relevant vision files.
