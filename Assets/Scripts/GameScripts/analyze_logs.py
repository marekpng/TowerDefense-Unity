import json
import pandas as pd
import matplotlib.pyplot as plt
import os


# PATH TO LOG FILE
LOG_DIR = "/Users/simonjerabek/Library/Application Support/DefaultCompany/TowerDefense/Logs"
LOG_FILES = [f for f in os.listdir(LOG_DIR) if f.endswith(".jsonl")]

if not LOG_FILES:
    raise FileNotFoundError("No .jsonl log files found in LOG_DIR.")

print("\nAvailable log files:")
full_paths = []
for index, filename in enumerate(LOG_FILES):
    full_path = os.path.join(LOG_DIR, filename)
    full_paths.append(full_path)
    size_kb = os.path.getsize(full_path) / 1024
    print(f"[{index}] {filename} ({size_kb:.1f} KB)")

choice = input("\nSelect log index: ").strip()

# Validate choice
if not choice.isdigit() or int(choice) < 0 or int(choice) >= len(full_paths):
    raise ValueError("Invalid selection. Restart and choose a valid index.")

LOG_PATH = full_paths[int(choice)]
print("\nUsing log file:", LOG_PATH)

# Detect level
import re
level_match = re.search(r"level(\d+)", LOG_PATH)
if level_match:
    print("Detected Level:", level_match.group(1))
else:
    print("Level not found in filename.")

# Load JSONL file
def load_logs(path):
    data = []
    with open(path, "r") as f:
        for line in f:
            try:
                data.append(json.loads(line))
            except json.JSONDecodeError:
                pass
    return pd.DataFrame(data)

df = load_logs(LOG_PATH)

print("Loaded events:", len(df))
print("Event types:", df["eventName"].unique())

# ===============================
# BASIC GRAPHS
# ===============================

# 1. Kills per Wave
kills = df[df["eventName"].str.contains("zombieKilled", na=False)]
kills_per_wave = kills.groupby(kills["eventName"].str.extract(r"wave_(\d+)")[0]).size()

plt.figure(figsize=(8,5))
kills_per_wave.plot(kind="bar", color="red")
plt.title("Zombie Kills per Wave")
plt.xlabel("Wave Number")
plt.ylabel("Kills")
plt.tight_layout()
plt.savefig("kills_per_wave.png")
print("Saved: kills_per_wave.png")

# 2. Tower DPS (Damage per Tower)
hits = df[df["eventName"].str.contains("projectileHit", na=False)]

# extract towerId & damage
hits["damage"] = hits["eventName"].str.extract(r"damage_(\d+)")
hits["damage"] = hits["damage"].astype(float)

dps = hits.groupby("towerId")["damage"].sum().sort_values(ascending=False)

plt.figure(figsize=(10,6))
dps.plot(kind="bar")
plt.title("Total Damage per Tower (DPS Tracking)")
plt.xlabel("Tower ID")
plt.ylabel("Total Damage")
plt.tight_layout()
plt.savefig("dps_per_tower.png")
print("Saved: dps_per_tower.png")

# 3. Money Over Time
money_events = df[df["eventName"].str.contains("money", na=False)]
money_events["time"] = money_events["sessionTime"]

money_events["money"] = money_events["eventName"].str.extract(r"new_(\d+)")
money_events["money"] = money_events["money"].astype(float)

plt.figure(figsize=(10,6))
plt.plot(money_events["time"], money_events["money"])
plt.title("Money Over Time")
plt.xlabel("Session Time (s)")
plt.ylabel("Money")
plt.tight_layout()
plt.savefig("money_over_time.png")
print("Saved: money_over_time.png")

# 4. Zombie HP Before/After First Hit
first_hits = df[df["eventName"].str.contains("zombieFirstHit", na=False)]

first_hits["hpBefore"] = first_hits["eventName"].str.extract(r"hpBefore_(\d+)")
first_hits["hpAfter"] = first_hits["eventName"].str.extract(r"hpAfter_(\d+)")
first_hits["hpBefore"] = first_hits["hpBefore"].astype(float)
first_hits["hpAfter"] = first_hits["hpAfter"].astype(float)

plt.figure(figsize=(10,6))
plt.scatter(first_hits["hpBefore"], first_hits["hpAfter"])
plt.title("Zombie HP: Before vs After First Hit")
plt.xlabel("HP Before")
plt.ylabel("HP After")
plt.grid(True)
plt.tight_layout()
plt.savefig("hp_first_hit.png")
print("Saved: hp_first_hit.png")

# ======================================
# 5. Damage Timeline (DPS per second)
# ======================================
print("Generating: damage_timeline.png")

if not hits.empty:
    hits["second"] = hits["sessionTime"].astype(int)
    dmg_timeline = hits.groupby("second")["damage"].sum()

    plt.figure(figsize=(12,6))
    plt.plot(dmg_timeline.index, dmg_timeline.values)
    plt.title("Damage Per Second (Timeline)")
    plt.xlabel("Time (s)")
    plt.ylabel("Damage")
    plt.tight_layout()
    plt.savefig("damage_timeline.png")
    print("Saved: damage_timeline.png")
else:
    print("No projectileHit logs → skipping damage timeline")

# ======================================
# 6. Kills Timeline
# ======================================
print("Generating: kills_timeline.png")

if not kills.empty:
    kills["time"] = kills["sessionTime"].astype(int)
    kills_timeline = kills.groupby("time").size()

    plt.figure(figsize=(12,6))
    plt.plot(kills_timeline.index, kills_timeline.values)
    plt.title("Kills Per Second")
    plt.xlabel("Time (s)")
    plt.ylabel("Kills")
    plt.tight_layout()
    plt.savefig("kills_timeline.png")
    print("Saved: kills_timeline.png")
else:
    print("No zombieKilled logs → skipping kills timeline")

# ======================================
# 7. Heatmap Death Positions
# ======================================
print("Generating: heatmap_deaths.png")

if not kills.empty:
    plt.figure(figsize=(8,6))
    plt.hist2d(kills["posX"], kills["posZ"], bins=30, cmap="hot")
    plt.colorbar(label="Death Count")
    plt.title("Zombie Death Heatmap (X/Z)")
    plt.xlabel("X Position")
    plt.ylabel("Z Position")
    plt.tight_layout()
    plt.savefig("heatmap_deaths.png")
    print("Saved: heatmap_deaths.png")
else:
    print("No kill positions available → skipping death heatmap")

# ======================================
# 8. Heatmap First-Hit Positions
# ======================================
print("Generating: heatmap_first_hits.png")

if not first_hits.empty:
    plt.figure(figsize=(8,6))
    plt.hist2d(first_hits["posX"], first_hits["posZ"], bins=30, cmap="Blues")
    plt.colorbar(label="Hit Count")
    plt.title("First Hit Heatmap (X/Z)")
    plt.xlabel("X Position")
    plt.ylabel("Z Position")
    plt.tight_layout()
    plt.savefig("heatmap_first_hits.png")
    print("Saved: heatmap_first_hits.png")
else:
    print("No first-hit logs → skipping hit heatmap")

# ======================================
# 9. Cumulative Money Curve
# ======================================
print("Generating: cumulative_money.png")

if not money_events.empty:
    money_events_sorted = money_events.sort_values("sessionTime")
    money_events_sorted["cumulative"] = money_events_sorted["money"]

    plt.figure(figsize=(12,6))
    plt.plot(money_events_sorted["sessionTime"], money_events_sorted["cumulative"])
    plt.title("Cumulative Money Over Time")
    plt.xlabel("Time (s)")
    plt.ylabel("Money")
    plt.tight_layout()
    plt.savefig("cumulative_money.png")
    print("Saved: cumulative_money.png")
else:
    print("No money logs → skipping cumulative money graph")

# ======================================
# 10. Enemy Survival Time (time alive)
# ======================================
print("Generating: survival_time.png")

spawn_events = df[df["eventName"].str.contains("zombieSpawn", na=False)]
death_events = df[df["eventName"].str.contains("zombieKilled", na=False)]

if not spawn_events.empty and not death_events.empty:
    survival = []

    for zid in spawn_events["zombieId"].unique():
        start = spawn_events[spawn_events["zombieId"] == zid]["sessionTime"]
        end = death_events[death_events["zombieId"] == zid]["sessionTime"]

        if not start.empty and not end.empty:
            survival.append(float(end.values[0]) - float(start.values[0]))

    if len(survival) > 0:
        plt.figure(figsize=(10,6))
        plt.hist(survival, bins=20, color="green")
        plt.title("Zombie Survival Time Distribution")
        plt.xlabel("Seconds Alive")
        plt.ylabel("Count")
        plt.tight_layout()
        plt.savefig("survival_time.png")
        print("Saved: survival_time.png")
    else:
        print("Could not match spawn/death pairs → skipping survival graph")
else:
    print("Missing spawn or kill logs → skipping survival graph")

print("Analysis complete. PNG graphs generated.")
