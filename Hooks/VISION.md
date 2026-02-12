# Hooks System Vision

- Keep event hooks thin and deterministic: subscribe once, forward events, and avoid embedding gameplay policy in hook glue.
- Guarantee safe lifecycle behavior (idempotent subscribe/unsubscribe and clean handler teardown on unload).
- Preserve event ordering and timing fidelity so core state transitions (hit, kill, despawn, unload) run exactly once.
- Log hook lifecycle failures and subscription boundaries clearly, without flooding normal gameplay logs.
- Treat hooks as an integration boundary: no hidden side effects beyond dispatch to core systems.
