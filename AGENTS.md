# Agent Instructions

When making changes in this repo:
- Always build `Release` and `Nomad` before finalizing.
- Shared game DLL path for this workspace is `D:\Documents\Projects\repos\BS\libs`.
- Build artifacts are expected in `bin/Release/PCVR/EnemyLimbDamageManager/` and `bin/Release/Nomad/EnemyLimbDamageManager/`.
- Use a feature branch for every task (for example `agent/<topic>`), and avoid direct work on `main`/`master`.
- Presets are batch editors only; collapsible values remain source-of-truth.
- Keep logging controls boolean only (`Basic Logs`, `Diagnostics Logs`, `Verbose Logs`, `Session Diagnostics`).
- When a user reports an issue, always recommend a specific boolean logging profile first.
- For low-noise repro in this repo, start with: `Session Diagnostics=On`, `Basic Logs=Off`, `Diagnostics Logs=Off`, `Verbose Logs=Off`.
- If runtime decision details are needed, set `Diagnostics Logs=On`.
- Only set `Verbose Logs=On` for short targeted repro sessions.
