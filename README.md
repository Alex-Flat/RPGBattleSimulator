# RPG Battle Simulator

This Unity project is an RPG Battle Simulator featuring two teams of agents, various actions for each agent, and minimal UI to follow the battle's progress. My goal was to focus on game balance by adjusting key features, such as agent properties (attack, health, etc.), or selection of action (e.g., choosing to heal a low health teammate). In addition, I want to add variance to the prior mentioned features so that battles are unique and engaging, and to create a flexible code base for future game development.

## Table of Contents
- [How to Run the Battle](#how-to-run-the-battle)
- [Development Process](#development-process)
  - [Q1: Battle Agents](#q1-battle-agents)
  - [Q2: Turn-Based Action System](#q2-turn-based-action-system)
  - [Q3: Action Types and Effects](#q3-action-types-and-effects)
  - [Q4: Putting It Together](#q4-putting-it-together)
- [Reflections](#reflections)

## How to Run the Battle
1. Clone this repository to your preferred local path.
2. In Unity Hub, click "Add -> "Add project from disk."  Navigate to the cloned repository and select the RPGBattle folder inside of the RPGBattleSimulator folder.
3. Open the project in Unity (tested with 2021.3.13f1).
4. In the Editor, navigate to Assets/Scenes and double click "RPGBattle"
5. Press the "Play" button to start the battle simulation!

If you would like to mess with configurations, in the editor, click on BattleManager, where you can change the player/enemy count, as well as their sprites and UI prefabs before playing.
In the code, agent stats and action calculations can be adjusted (Constants.cs). In addition, agent actions can be specifically configured in BattleManager.cs in the SetupBattle function.

*Note*: Ensure TextMeshPro is installed in your Unity environment.

## Development Process

### Q1: Battle Agents
**Time Spent**: 40 minutes

#### Development Process
- Created an abstract `Agent` class with core stats (HP, attack, defense, speed) and a `Team` enum (Player or Enemy).
- Implemented `Initialize()` for dynamic stat assignment with variance.
- Integrated into `BattleManager.SetupBattle()` for configurable team sizes.
- Added `SetupVisuals()` for sprite and UI instantiation.

#### Design Choices
- Made `Agent` abstract to share stats and behavior, supporting future subclasses (e.g., specialized roles).
- Applied ±20% variance to base stats (via `Constants`) for diversity, keeping configuration in code for runtime flexibility and developer tuning.
- Assigned sprites via the Unity Inspector in `BattleManager` to separate art from code.
- Added a low-health crit multiplier to enhance battle excitement.

---

### Q2: Turn-Based Action System
**Time Spent**: 65 minutes

#### Development Process
- Built `BattleManager` to manage the battle loop, action timers, and agent updates.
- Implemented a speed-based action queue in `UpdateBattle()` using `Dictionary<Agent, float>` for cooldowns.

#### Design Choices
- Used separate `playerTeam` and `enemyTeam` lists, plus `actionTimers` and `activeEffects` dictionaries, for efficient battle state management.
- Calculated action intervals with `Time.deltaTime` for real-time precision.
- Applied random stat variance in `SetupBattle()` to ensure unique battles.
- Configured actions per agent in `SetupBattle()` code, with plans to move this to `Agent.Initialize()` for specialized agents (e.g., tank, healer) in future iterations.
- Positioned agents dynamically to support up to 15 per team within screen bounds.

---

### Q3: Action Types and Effects
**Time Spent**: 55 minutes

#### Development Process
- Extended `Action` into subclasses: `ActionDamage`, `ActionHeal`, `ActionDoT`, `ActionHoT`, `ActionBuff`, `ActionDebuff`.
- Tied action effects to `Constants` (e.g., damage scales with attack, healing is fixed, buffs/debuffs use multipliers).
- Implemented effect logic in `Execute()` and periodic updates in `Update()`.
- Added `PerformAction()` for action selection and targeting.

#### Design Choices
- Used inheritance from `Action` for reusable timing logic, grouping all actions in one file for easy modification.
- Centralized tuning in `Constants.cs` for quick balance adjustments.
- Added basic AI in `PerformAction()` (e.g., heal low-health allies) with flexibility for future complexity.

---

### Q4: Putting It Together
**Time Spent**: 85 minutes

#### Development Process
- Implemented win conditions in `UpdateBattle()` (battle ends when one team’s HP reaches 0).
- Created `AgentUI` for health bars, floating damage/heal numbers, effect displays, and attack timers.
- Used `BattleUI` to announce the winner.
- Consolidated gameplay values in `Constants` class.

#### Design Choices
- Organized agent-specific UI in `AgentUI` for clarity.
- Used a single Canvas, ensuring `BattleUI` text renders on top with `SetAsLastSibling()`.
- Added floating numbers and health bars for visual feedback.

---

## Total Time Estimate
- **Q1**: 40 minutes
- **Q2**: 65 minutes
- **Q3**: 55 minutes
- **Q4**: 85 minutes
- **Total**: ~245 minutes (~4.1 hours)

Time was reduced significantly by using Grok AI for coding assistance and understanding Unity features.

## Reflections
For my first Unity project, this was a great accomplishment in my eyes. After learning the basics, I am fairly confident with Unity, but still currently struggle with project structure. With more experience on Unity, I will be able to learn better project management with regards to what items should be configurable in Inspector rather than in code, script organization, prefab creation and usage, and much more. In addition, this project was the first time I used Cursor, and it is now by far my favorite IDE. 

For my next project, I want to focus on the aforementioned project organization as well as taking better notes on my development process and design choices. I feel I made meaningful git commits messages, but I should constantly explain my thinking and implementations in another document while coding, rather than trying to do it retroactively after significant progress.

If you have any feedback, I would love to hear it so I can learn and improve as a developer! Thank you for viewing my project, and have a great day! :)
