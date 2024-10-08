# Carl's Fancy Spaceship Simulator

![Screenshot 2024-04-23 090649](https://github.com/user-attachments/assets/d0bb90a7-6698-4941-93ac-3854c49f2f2d)

## Getting started

Download the zip file from "releases".

Alternatively, download the zip file from here: https://drive.google.com/drive/folders/1rch6zxa7OjYHpX0dJPhJTZobDaCrrPNV?usp=drive_link (OUTDATED, use [releases](https://github.com/YeeticusFinch/SpaceshipSimulator/releases) tab)

Unzip the file, and run `SpaceshipSimulator.exe`

## Playing the game

![Screenshot 2024-09-29 211607](https://github.com/user-attachments/assets/269b2f05-95c7-4c51-a36a-1b6676245f2c)

You can select which map you want to play, some of them have gravity and an atmosphere, and some of them don't.

The `race` maps will have goals for you to complete in a certain amount of time. The goals are either flying through a ring or shooting a ball.

You can also select ships for teammates and enemies, if you want to get a spaceship battle going.

![Screenshot 2024-09-24 170340](https://github.com/user-attachments/assets/85862698-7546-4e09-8005-23cae80a689c)

If you can't tell from the images, this game was largely inspired from the Expanse series. I was reading Nemesis Games (book 5) and I had gotten to the scene where Naomi was stuck on the Chetzemoka, but she managed to turn on a maneuvering thruster, causing the ship to tumble into a spiral. I had difficulty imagining it, so I made it in Unity. And then scope creep kinda turned that projet into a game.

## Controls

### Keyboard Controls

WASD - These will rotate your ship, if you hold L+CTRL then these buttons will strafe your ship instead.

Q and E - Also rotates your ship. These will still rotate your ship even if you hold L+CTRL

Shift - Backwards movement

Space - Main thrusters, forward movement. If you hold L+CTRL, then it will use maneuvering thrusters to move forward, rather than the main thruster

Right-Click - Rotate the camera

Middle-Click - Pan the camera

Scroll Wheel - Zooms the camera

## Panels

### Movement

![Screenshot 2024-09-24 165810](https://github.com/user-attachments/assets/062c1d46-3d1c-4f58-85c1-0f6a42b9c48e)

Controls most things related to movement and thrusters.

**Stabilizers**

Stabilizers will counter your forward and backward velocity, using your maneuvering thrusters to slow your speed down to zero. They will also counter any rotational velocity, to stop any rotations.

When you hold L+CTRL, stabilizers will switch to countering sideways velocity.

`Stabilizer Power` represents the maximum amount of thrust power the stabilizers are allowed to use.

`Speed Tolerance` is the minimum amount of speed required to trigger the stabilizers. If `Speed Tolerance` is set too low, then the stabilizers might continuously overshoot, causing the spaceship to shake.

`Stabilizer Kp` is the proportional constant for the PID controller that controls the stabilizers. The stabilizers will fire the thrusters at a power proportional to `stabalizer_Kp * speed`. Higher Kp's will result in faster and more aggressive stabilization, but may also incur some overshoot. Lower Kp's will result in slower and more gentle stabilization.

If `Stabilizer Thrust` is `ON`, then the maneuvering thrusters will fire along with the main drive when the Spacebar is pressed, giving you more thrust at the cost of maneuverability.

If `Main Drive Stabilizer` is `ON`, then the stabilizers will use the main drive to stabilize. This is particularly useful for hovering in gravity.

**Autopilot**

Currently, the only autopilot features enabled for player ships is `Flip and Burn` and `Point Towards` (point towards can be found under "Telemetry" panel). 

`Flip and Burn` will flip the ship to face the opposite direction of the velocity vector, and then fire the main drive to zero out the velocity as quickly as possible. This is the most reliable way to make your ship stop moving.

`Navigation Kp` is the proportional constant used by the autopilot's PID controller, which is position-based rather than velocity-based like the stabilizers.

`Navigation Kd` is the derivative constant used by the autopilot's PID controller.

 The maneuvering thrusters will fire at a power proportional to `nav_Kp * distance + nav_Kd * velocity`, where distance could be a translational distance or an angle distance, and velocity could be translational velocity or rotational velocity. 
 
 Higher Kp's will result in faster and more aggressive maneuvering, but may also incur some overshoot. Lower Kp's will result in slower and more gentle maneuvering.

 The higher the Kd, the more the controller will slow down the ship's movement prior to reaching the destination (again, the "destination" may refer to a position or a rotation). If the Kd is too high, then it may never even reach the destination (or it might reach it very slowly).

Below are some position vs time graphs to show you the effects that Kp and Kd have on the ship's movement.

 ![image](https://github.com/user-attachments/assets/5410fcf7-0f37-40b0-b1e4-d4f41cd0764a)
 
 Pictured above is what overshoot looks like. As you can see, this system is underdamped. You can fix that by increasing Kd or decreasing Kp.

 ![image](https://github.com/user-attachments/assets/377b584a-359f-49f1-bbd8-dcdbb521e36d)
 
 If you have way too much overshoot, it could even look like this (pictured above).

 ![image](https://github.com/user-attachments/assets/099d278e-59ff-4e24-b265-447d92e4e4b4)
 
 For reference, this is how you want it to look (pictured above).

![image](https://github.com/user-attachments/assets/4e7e03b7-7f82-4d5f-95cd-f22d02880cb0)

Above is an image that illustrates the difference between underdamped and overdamped systems.

`Main Drive Power` is a scalar for the main drive. So if you set `Main Drive Power` to 0.5, then the main drive will only thrust at half power (the thrust signal going into the main drive gets multiplied by the `Main Drive Power`).

`Maneuvering Power` is the same as above but for the maneuvering thrusters.

`Reboot Propulsion` will reset all your thruster configurations, reboot the autopilot and the stabilizers, it will clear all movement signals, and it will fix any jammed thrusters.

### Comms

Communication systems is yet to be implemented. This will include:
- Ability to communicate with ally and enemy ships
- Ability to command ally ships, get them into formation, have them target specific targets, make them cease fire...
- Sharing target locks
- Sharing radar pings
- Jamming target locks, radars, and missiles

### Weapons

![Screenshot 2024-09-30 182435](https://github.com/user-attachments/assets/236cae91-1a5a-4174-877a-73e98f9017f3)

Weapon systems, including turrets, missiles, and cannons.

**Turrets**

![Screenshot 2024-09-24 165715](https://github.com/user-attachments/assets/76412442-3b65-453f-b2ad-fe9cb8ef3ae3)

Turrets can rotate, and some of them can even fold. 

`Turret Fire Mode` will define when the turret will shoot a bullet. `Auto` means that it will fire when it has a firing solution on a target, and it is aimed at the right spot, and it has a clear path to the target. `Manual` means that it will fire when the left mouse button is clicked, but only if the turret is currently pointing in the direction of the camera. `Off` means that the turret will never fire.

`Turret Aim Mode` will define how the turret spins. `Auto` means that it will automatically calculate firing solutions and rotate towards targets. `Manual` means that it will rotate towards the direction the camera is facing, specifically to a point at a specific distance in front of the camera (defined by `Manual Aim Distance`). 

When `Turret Aim Mode` is set to `Auto`, and the turret is aiming at a moving target, it will take the target's position, velocity, and acceleration relative to itself into account when aiming. So it knows exactly where to shoot, because it knows how fast the target is moving and how fast its bullets can fly. However, it is very bad at hitting objects with a changing acceleration, such as a ship that is turning. To evade turret fire, frequent turns are recommended to dodge the lines of fire. When battling a ship that is making a lot of turns to dodge your lines of fire, it could be beneficial to switch to manual mode and manually aim at the ship.

`Target Lock` is for assigning a target to the turret. Click on the `Target Lock` button, then click on a radar ping on your screen, and it will assign that radar ping to the selected turret, and if `Turret Aim Mode` is set to `Auto` it will spin towards the target, and it will fire if `Turret Fire Mode` is set to `Auto`.

`Shoot Incomming` is for shooting any objects that are on a collision course with your ship. This only works if `Turret Aim Mode` and `Turret Fire Mode` are set to `Auto`. Any incomming missiles or asteroids or spaceships will be targeted and shot.

`Flashlights` most weapons are equipped with flashlights, this button can turn them on or off.

`Turret Camera` will show you the perspective from the selected turret, giving you a crosshair to aim with, and giving you direct manual control over the turret. You might want to switch `Turret Aim Mode` and `Turret Fire Mode` to `Off`, as they can interfere with your direct control of the turret.

`Next` / `Previous` will cycle between the available turrets.

**Missiles**

![Screenshot 2024-09-24 175915](https://github.com/user-attachments/assets/00551f4f-7522-420b-9bee-6bd77a3ee632)

`Launch at Target` will give you a target lock (much like the turret target lock feature), and as soon as you click on the target, it will fire a missile.

When a missile launches, it starts by moving a safe distance away from the spaceship, then aiming itself towards the target, then it begins accelerating.

If the missile is launched from a missile gun, it will skip those aforementioned steps, and it will be launched with an initial velocity, and then it will steer itself towards the target.

`Intercept Incoming` will determine whether this missile launcher should fire missiles to intercept incoming missiles.

`Camera Follows Missile` is just a fun gimmick, it gives you a 3rd person camera view of the missile as it flies towards the target. This does occasionally glitch the game.

`Manual Missile Control` gives you direct control of the missile, allowing you to fly it however you want. This feature is still under development (it works when I manually change a variable in the code lol).

`Next` / `Previous` will cycle between missile launchers. Each missile launcher can launch one type of missile.

**Cannons**

![Screenshot 2024-10-02 155319](https://github.com/user-attachments/assets/f8437b44-a4f3-4d0f-943d-a48a6337d03f)

Cannons are similar to turrets, except a lot simpler, because they don't move. They are completely stationary, which means you need to aim them by turning your ship. Most cannons will be forward-facing.

`Cannon Fire Mode` defines when the cannon will shoot. When set to `Auto`, it will shoot only when the cannon is pointed at the target. When set to `Manual`, it will shoot when you left click.

`Target Lock` defines the cannon's target. Cannons cannot spin, so this feature isn't very useful. Unless you want the cannon to automatically fire when your ship is pointed in the right direction. But you are better off shooting your cannon manually.

### Diagnostics

Shows you the current health of all the different components on your ship. Destroyed components won't show up in this list. Components with the lowest health percent will show at the top of the list.

### Telemetry

Controls for the radar and the targeters.

`Trace Radar` will trace a green line between your radar antenna and radared objects. And a red line between your targeter antenna and targetted objects. Radars and targeters need to have line of sight in order to function.

`Trace Missiles` will trace lines between missiles and their targets.

`Gun Lasers` will trace lasers down the barrel of all guns and outwards, allowing you to visualize where each gun is aiming at. This feature is very useful for aiming cannons.

`Radar Objects` will allow the radar to also ping inanimate objects, such as asteroids.

There will then be a list of all objects and spaceships and missiles that have been radared. You can use this list to assign targets to your turrets and your missiles. You can also use the `Clear Target Lock` button to clear all target locks on that particular radar ping. There should be one item in this list for each radar ping. If they don't all fit in the window, you can use the arrow buttons to cycle through the list.

## Power

The reactor powers your ship, but you can also turn off your reactor and run your ship on battery.

If you overclock your reactor, you get a bunch of upgrades:
- Turrets spin faster and fire rate increases
- All thrusters get a boost
- Radars and targeters can reach up to triple their original range

However, overclocking will destabilize your reactor.

You can regain reactor stability by dropping core (it will take 10 seconds for a core to regrow).

The core size is also important, you want it to stay around 10. If the core size deviates too far from 10 (ie it gets too big or too small), then you will experience power issues, random stuff will shut down. Lower reactor stabilities will mess with your core size.

If your reactor stability drops below 10, it has a chance of exploding.

You will also lose reactor stability if your reactor gets damaged.

All electrical systems generate electric noise. Electric noise makes it easier for radars and targeters to spot your ship.

You can lower your electric noise by turning off your reactor and running on batteries, but also by turning off power to different systems.

Running your reactor in low power will make you a bit less detectable, but it has drawbacks:
- Turrets spin slower and fire rate decreases
- Thrusters get nerfed
- Targeters get nerfed

Your reactor will charge your battery.

## Parts of a Ship

Every component on the ship has it's own health, it's own resistances and vulnerabilities to damage types, and the ability to absorb damage from each damage type. When a projectile hits a component on a ship, be it a piece of armor, a thruster, an antenna, or whatever, it will deal the damage to that component (depending on the damage types and damage resistances), then the projectile will lose damage according to the damage absorption properties of the ship component, and depending on how much damage was absorbed, the projectile might penetrate through (weaker), might get deflected, or might just stop.

![image](https://github.com/user-attachments/assets/07c1405a-0ca4-44ce-be15-29f69f59ef5b)

Pictured above is the damage properties of a minigun turret. Damage factor is the factor of each damage type that it actually takes. Damage absorbed is the percent damage that the projectile will lose upon impact. So if the projectile does 10 piercing damage, the turret will suffer 7 damage, and the projectile will penetrate through the turret, but it will be weakened to only do 2 damage to the next thing it hits.

Ships have many parts:
- Reactor (powers the ship)
- Hull (armor plating, hull pieces, it's just a box with some health)
- Thrusters
- Guns
- Missile launcheres
- Radar antennas (antennas with the green tips)
- Targeting antennas (antennas with the red tips)
- Cockpits (with people inside that can be killed, if you lose all your people then your ship can no longer fly)
