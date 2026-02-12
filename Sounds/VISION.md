# Sounds System Vision

- Keep injury and kill audio readable, punchy, and synchronized with animation/state transitions.
- Follow NPC-only routing, gender-aware pool selection, and live positional anchoring for all voice-style sounds.
- Keep kill/mimic sound behavior consistent with visual presentation so recoverable and non-recoverable outcomes do not look or sound contradictory.
- Ensure mod-managed audio sources are tagged and cleaned up reliably to prevent leaks, overlap bugs, or persistent ambience.
- Favor deterministic fallback behavior when clips, pools, or anchors are missing, with clear logs for each fallback path.
- Implement all sound behavior changes in compliance with `_docs/SOUND_IMPLEMENTATION_GUIDE.md`.
