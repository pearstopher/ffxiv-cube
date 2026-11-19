# Cube

[![Build and test](https://github.com/pearstopher/ffxiv-cube/actions/workflows/build.yml/badge.svg?branch=main)](https://github.com/pearstopher/ffxiv-cube/actions/workflows/build.yml)
[![Latest release](https://img.shields.io/github/v/release/pearstopher/ffxiv-cube)](https://github.com/pearstopher/ffxiv-cube/releases/latest)

A [Dalamud](https://github.com/goatcorp/Dalamud) plugin for FINAL FANTASY XIV that draws a window showing a
3D cube of space around your character, with the players near you inside it as small labelled cubes and the
ground, walls and objects around you as simple boxes.

![The cube window: a wireframe cube with you at the centre and nearby players as small coloured cubes, each with a name label](https://raw.githubusercontent.com/pearstopher/ffxiv-cube/main/images/cube-window.png)

*Drawn by the project's snapshot tool from a real game frame; in game the bars are the players' names.*

## What it does

- **Type `/cube`** to show or hide the cube window.
- **You are the cube in the middle.** The big cube is centred on you and moves with you, 40 yalms across by default.
- **Players inside the cube are shown as small cubes** with their names next to them. Players outside it aren't shown.
- **Named NPCs are shown too,** the same way: townspeople, quest NPCs and enemies inside the cube, in purple, with
  their names. Pets and chocobo companions belong to players, so they're left out. You can turn NPCs off.
- **The ground, walls and objects around you are shown as grey-blue boxes:** a simplified view of what you'd walk
  on, bump into or be stopped by, cut off at the cube's edges. It's built from the collision the game has already
  loaded, a little at a time so it never slows the game; right after you arrive somewhere the boxes fill in over a
  moment, and they're redrawn when you change zone or the area around you loads. You can make the boxes coarser, or
  turn them off.
- **Colours tell you who's who:** one colour for you, one for party and alliance members, one for everyone else,
  and one for NPCs.
- **Near things cover far things,** so you can see which players are in front.
- **Turn the view with the rotate gizmo** in the corner of the window: drag the red, green or blue ring to turn the
  cube around that axis, or the outer ring to spin it on the screen. Any angle works, and your view is remembered.
- **Resize the cube with the second gizmo** next to it: drag the red, green or blue handle to make the cube wider, taller or
  deeper (it becomes a box), or the centre handle to zoom in and out. The size is remembered too.
- **Nothing is drawn while you're logged out, loading, in a cutscene, in GPose, or in PvP.**

It only reads what the game already knows and draws it in its own window. It never changes anything in the game,
never sends anything to the server, and never automates anything. Names are only drawn on screen; they're never
saved or sent anywhere.

## Settings

Type `/cube config` (or use the settings button in `/xlplugins`). Changes show straight away.

| Setting | What it does | Default |
| --- | --- | --- |
| Width / Height / Depth (yalms) | How far the cube reaches around you on each axis, side to side, top to bottom and front to back (1–100 each). Different values make it a box. | 40 / 40 / 40 |
| Zoom (largest side, yalms) | Shrinks or grows all three together, keeping the shape. Shows the longest side; lower shows less space, bigger. | 40 |
| Reset size | Back to 40 × 40 × 40. | |
| Player height in cube | Where you sit from bottom to top of the cube. 0% puts you on the floor of the cube, so it shows only what's above you; 100% puts you on the ceiling, so it shows only what's below. | 50% (middle) |
| Orientation | The cube itself stays still on screen; this decides what happens to what's inside it. **Rotate contents with camera:** forward in the cube is where the game camera looks, so when you turn the camera everything inside turns around you. **Keep contents fixed (north stays put):** north is always the same side of the cube and the camera changes nothing. | Rotate contents with camera |
| View control corner | Which corner of the cube window the x/y/z view control sits in. | Bottom right |
| Reset view | Puts the view back to the default: from behind and above, at 30°. | |
| Show names | Name labels for other players, NPCs, enemies and interactables. | On |
| Show your name | Your own name next to your cube. | On |
| Name position | Names **to the side** of their cubes, or **centred above** them. | To the side |
| You / Party / alliance / Others | The colour for each kind of player. | Blue / green / orange |
| Show players | Whether other players (party and everyone else) are shown. You are always shown. | On |
| Show NPCs / NPCs | Whether friendly named NPCs are shown, and their colour. At most 50 NPCs and enemies are drawn at once. | On / purple |
| Show enemies / Enemies | Whether hostile enemies are shown, and their colour. | On / red |
| Show interactables / Interactables | Whether objects the game labels with words ("Entrance", "Placard", "Summoning Bell", ...) are shown, and their colour. At most 30 are drawn at once. | On / teal |
| Show collision / Detail | Whether the collision boxes are shown, and how finely. **Full** draws the most boxes and follows the shapes most closely; **Medium** and **Coarse** draw fewer, bigger boxes that still cover everything Full does. At most 2000 boxes are kept. | On / Full |
| Collision style | **Boxes** draws simplified boxes and is cheap. **Mesh (slower)** draws every collision triangle the game has as a thin wireframe, cut off exactly at the cube, with nothing simplified. It costs noticeably more than Boxes (a busy place can take a few milliseconds a frame), so it is off by default; Detail applies to Boxes only. | Boxes |
| Opacity | How see-through everything in the window is. | 1.00 |
| Reset to defaults | Puts every setting back. | |

## Installing

This is a personal plugin. It follows the official Dalamud plugin rules but isn't in the official plugin list, so
it's installed from its own custom repository:

1. In game, type `/xlsettings` and open the **Experimental** tab.
2. Under **Custom Plugin Repositories**, paste this URL, tick **Enabled**, and click **Save**:

   ```
   https://raw.githubusercontent.com/pearstopher/ffxiv-cube/main/repo.json
   ```

3. Type `/xlplugins`, search for **Cube** and install it.
4. Type `/cube`.

## Problems or ideas

Please [report a bug](https://github.com/pearstopher/ffxiv-cube/issues/new?template=bug.md). The form asks for your
versions, settings, what happened, and the relevant lines from `dalamud.log`.

## Contributing

- **Building and testing:** see the [development guide](https://github.com/pearstopher/ffxiv-cube/blob/main/DEVELOPMENT.md).
- **How it works:** see the [code reference](https://github.com/pearstopher/ffxiv-cube/blob/main/docs/reference/README.md), generated from the code's comments.

## License

[AGPL-3.0](https://github.com/pearstopher/ffxiv-cube/blob/main/LICENSE.md)
