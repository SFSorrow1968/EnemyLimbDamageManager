# -*- coding: utf-8 -*-
"""Build and release Enemy Limb Damage Manager mod."""
import json
import subprocess
from pathlib import Path

BASE = Path(__file__).parent.parent
MANIFEST = BASE / "manifest.json"


def run(cmd, cwd=None):
    print(f"$ {cmd}")
    subprocess.run(cmd, shell=True, cwd=cwd or BASE, check=True)


def get_version():
    with open(MANIFEST, "r", encoding="utf-8") as f:
        return json.load(f)["ModVersion"]


def require_clean_non_main_git_state():
    try:
        branch = subprocess.check_output("git branch --show-current", shell=True, cwd=BASE, text=True).strip()
    except subprocess.CalledProcessError as exc:
        raise RuntimeError("Not a git repository. Initialize git before running release automation.") from exc

    if branch in {"main", "master"}:
        raise RuntimeError(f"Refusing release on protected branch '{branch}'. Use a feature/release branch.")

    status = subprocess.check_output("git status --porcelain", shell=True, cwd=BASE, text=True).strip()
    if status:
        raise RuntimeError("Working tree is dirty. Commit or stash changes before releasing.")


def main():
    version = get_version()
    tag = f"v{version}"

    print(f"\n=== Releasing Enemy Limb Damage Manager {version} ===\n")
    require_clean_non_main_git_state()

    run("powershell -ExecutionPolicy Bypass -File _agent/ci-smoke.ps1 -Strict")

    print("\n=== Creating release zips ===\n")
    run('powershell -Command "Compress-Archive -Path bin/Release/PCVR/EnemyLimbDamageManager -DestinationPath ELDM-PCVR.zip -Force"')
    run('powershell -Command "Compress-Archive -Path bin/Release/Nomad/EnemyLimbDamageManager -DestinationPath ELDM-Nomad.zip -Force"')

    result = subprocess.run(f"git tag -l {tag}", shell=True, capture_output=True, text=True, cwd=BASE)
    if tag not in result.stdout:
        run(f"git tag {tag}")

    run(f"git push origin {tag}")

    print("\n=== Creating GitHub release ===\n")
    run(f'gh release create {tag} ELDM-PCVR.zip ELDM-Nomad.zip --title "Enemy Limb Damage Manager {version}" --notes "Release {version}"')

    print(f"\n=== Enemy Limb Damage Manager {version} released ===\n")


if __name__ == "__main__":
    main()
