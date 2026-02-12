# Core System Vision

- Build a deterministic injury state machine where limb damage, knockout, mock death, revive, and final death all transition cleanly.
- Keep vanilla kills and recoverable kills visually indistinguishable at the moment of death (same death animation and immediate presentation).
- Treat mock death as a real "appears dead but alive" window: instant entry after death resolution, configurable duration, random eye states, and recoverable stand-up.
- Keep knockout, death KO, and permanent death visually consistent for eyes, mouth, limpness, and motion pacing; only death paths play death animations.
- Do not use a damage-threshold cancel system for queued Last Stand; recovery proceeds through the configured mock-death window unless the NPC is killed again.
- Allow a configurable percentage of revival stand-up attempts to visibly fail: the NPC should freeze/stall during get-up to simulate a failed recovery beat before retrying.
- Slow animation by explicit multipliers that reliably extend duration for death, mock death, and stand-up phases.
- Maintain structured logs that clearly explain state decisions, interruption causes, animation-speed values, and recovery outcomes.
