# MazeRunner2D - Complete Setup Instructions

## 🎮 Quick Start Guide

### Step 1: Add the Game Enhancer
1. In Unity, create an empty GameObject in your scene (Right-click in Hierarchy → Create Empty)
2. Name it "GameEnhancer"
3. Add the `GameEnhancer` component to it
4. **The GameEnhancer will automatically create all missing prefabs!**

### Step 2: Add the Improved Maze Setup
1. Create another empty GameObject named "MazeSetup"
2. Add the `ImprovedMazeSetup` component to it
3. Check "Auto Setup" and "Force Regenerate" options
4. This will automatically set up the entire game!

### Step 3: Verify Your Scene Has These GameObjects:
- ✅ Player (tagged "Player")
- ✅ Main Camera (tagged "MainCamera")
- ✅ GameManager
- ✅ Glade (background)

### Step 4: Press Play!
The game will automatically:
- Create all missing prefabs (Trees, Spiders, Power-ups, Bushes, Walls, Clouds)
- Set up the maze
- Spawn all objects
- Create UI elements

---

## 🔧 Manual Setup (if needed)

### If Objects Don't Appear:

1. **Check the Console** - Look for debug messages about what was created
2. **Press 'I' Key** - Shows debug info about spawned objects
3. **Use Context Menu**:
   - Right-click `GameEnhancer` component → "Create All Prefabs"
   - Right-click `ImprovedMazeSetup` component → "Setup Complete Game"

### Common Issues & Fixes:

#### "Can't see spiders"
- Check Console for "Spiders: X" message
- Spiders spawn far from the player (check minimap)
- Press 'R' to regenerate maze

#### "Can't see power-ups"
- Power-ups spawn randomly in the maze
- Look for glowing star-shaped objects
- They have different colors based on type

#### "Trees not spawning"
- GameEnhancer creates tree prefab automatically
- Check MazeGenerator has "treePrefab" assigned
- Press 'R' to regenerate

---

## 🎨 Visual Improvements

### What's Enhanced:

1. **Better Sprites**:
   - Procedurally generated sprites with proper shapes
   - Anti-aliased edges for smooth visuals
   - Distinct colors for different objects

2. **Player Visuals**:
   - Blue player with glow effect
   - More visible on dark background
   - Proper size and scale

3. **Spiders**:
   - Dark red body with 8 legs
   - Visible and distinct from other objects
   - Proper size for gameplay

4. **Power-ups**:
   - Star-shaped sprites
   - Color-coded by type:
     - 🟢 Green = Health Pack
     - 🟡 Yellow = Speed Boost
     - 🔵 Cyan = Invisibility
     - 🔴 Red = Spider Repellent
     - 🟣 Magenta = Score Multiplier
   - Floating and glowing animations

5. **UI Elements**:
   - Health bar (top-left)
   - Score display (top-right)
   - Minimap frame (bottom-left)
   - Better visibility with outlines

6. **Environment**:
   - Dark forest background
   - Green trees (darker)
   - Light green bushes
   - Brown walls with texture

---

## 🎮 Gameplay Controls

### Movement:
- **WASD** or **Arrow Keys** - Move player
- **Mouse** - Look direction (if implemented)

### Game Controls:
- **R** - Regenerate maze
- **F** - Toggle fog of war
- **ESC** - Pause game
- **I** - Show debug info (developer mode)

### Power-up Effects:
- **Health Pack** (Green) - Restores 25 HP
- **Speed Boost** (Yellow) - 1.5x movement speed for 10 seconds
- **Invisibility** (Cyan) - Spiders can't see you for 5 seconds
- **Spider Repellent** (Red) - Spiders run away for 10 seconds
- **Score Multiplier** (Magenta) - Bonus 500 points

---

## 🐛 Debugging

### Check Object Counts:
Press **'I'** in-game to see:
- Number of walls spawned
- Number of spiders spawned (and their positions)
- Number of power-ups spawned (and their types)
- Number of trees and bushes
- Player position

### Console Messages:
Look for these messages to confirm everything is working:
- ✓ "GameEnhancer created"
- ✓ "Created Tree Prefab"
- ✓ "Created Spider Prefab"
- ✓ "Created Power-Up Prefab"
- ✓ "All prefabs created successfully!"
- ✓ "Maze ready with spider enemies!"
- ✓ "Successfully spawned X/X spiders"
- ✓ "Successfully spawned X/X power-ups"

### Visual Checks:
1. **Player**: Should be blue circle at center (0, 0)
2. **Spiders**: Dark red with legs, should be moving
3. **Power-ups**: Star-shaped, glowing, floating
4. **Trees**: Dark green circles in glade area
5. **Bushes**: Light green ovals scattered in maze
6. **Walls**: Brown squares forming maze

---

## 📊 Game Statistics

### Default Settings:
- Maze Size: 30x30 cells
- Cell Size: 1 unit
- Glade Size: 10 units
- Spider Count: 3
- Power-up Count: 5
- Tree Count: ~7 (in glade)
- Bush Count: ~25% of path cells

### Score System:
- Survival: +10 points per second
- Reach Exit: +1000 points
- Avoid Spider: +50 points
- Collect Power-up: +100 points
- Level Complete: Time bonus

---

## 🎯 Tips for Better Gameplay

1. **Start Small**: Begin with 3 spiders to learn the mechanics
2. **Use Bushes**: Hide in bushes to avoid spiders
3. **Collect Power-ups**: They're essential for survival
4. **Find the Exit**: Look for glowing green square
5. **Watch Health**: Spiders deal 25 damage per hit
6. **Use Fog of War**: Makes game more challenging (press F)

---

## 🔄 Updating the Game

If you make changes and things break:

1. Delete all objects in scene except:
   - Player
   - Main Camera
   - Glade

2. Run the GameEnhancer and ImprovedMazeSetup again

3. Everything will be recreated!

---

## 📝 Notes

- All sprites are procedurally generated (no image files needed!)
- Game auto-saves progress
- Performance optimized with LOD system
- Mobile-friendly controls (if implemented)
- Fully extensible - add your own content!

---

## 🎉 Enjoy Your Enhanced MazeRunner2D!

Your game now has:
- ✅ Visible and distinct sprites
- ✅ Proper spawning systems
- ✅ Beautiful UI
- ✅ Realistic visuals
- ✅ Smooth gameplay
- ✅ Complete feature set

Happy gaming! 🎮

