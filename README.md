# Carl's Fancy Spaceship Simulator

## Getting started

Download the zip file from "releases".

Alternatively, download the zip file from here: https://drive.google.com/drive/folders/1rch6zxa7OjYHpX0dJPhJTZobDaCrrPNV?usp=drive_link

Unzip the file, and run `SpaceshipSimulator.exe`

## Playing the game

You can select which map you want to play, some of them have gravity and an atmosphere, and some of them don't.

The `race` maps will have goals for you to complete in a certain amount of time. The goals are either flying through a ring or shooting a ball.

You can also select ships for teammates and enemies, if you want to get a spaceship battle going.

## Controls

### Keyboard Controls

WASD - These will rotate your ship, if you hold L+CTRL then these buttons will strafe your ship instead.

Q and E - Also rotates your ship. These will still rotate your ship even if you hold L+CTRL

Shift - Backwards movement

Space - Main thrusters, forward movement. If you hold L+CTRL, then it will use maneuvering thrusters to move forward, rather than the main thruster

Right-Click - Rotate the camera

Middle-Click - Pan the camera

Scroll Wheel - Zooms the camera

### Panels

#### Movement

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
 This is what overshoot looks like. As you can see, this system is underdamped. You can fix that by increasing Kd or decreasing Kp.

 ![image](https://github.com/user-attachments/assets/377b584a-359f-49f1-bbd8-dcdbb521e36d)
 If you have way too much overshoot, it could even look like this.

 ![image](https://github.com/user-attachments/assets/099d278e-59ff-4e24-b265-447d92e4e4b4)
 For reference, this is how you want it to look.

![image](https://github.com/user-attachments/assets/4e7e03b7-7f82-4d5f-95cd-f22d02880cb0)
Here's an image that illustrates the difference between underdamped and overdamped systems.

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

Weapon systems, including turrets, missiles, and cannons.

**Turrets**

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

`Launch at Target` will give you a target lock (much like the turret target lock feature), and as soon as you click on the target, it will fire a missile.

When a missile launches, it starts by moving a safe distance away from the spaceship, then aiming itself towards the target, then it begins accelerating.

If the missile is launched from a missile gun, it will skip those aforementioned steps, and it will be launched with an initial velocity, and then it will steer itself towards the target.

`Intercept Incoming` will determine whether this missile launcher should fire missiles to intercept incoming missiles.

`Camera Follows Missile` is just a fun gimmick, it gives you a 3rd person camera view of the missile as it flies towards the target. This does occasionally glitch the game.

`Manual Missile Control` gives you direct control of the missile, allowing you to fly it however you want. This feature is still under development (it works when I manually change a variable in the code lol).

`Next` / `Previous` will cycle between missile launchers. Each missile launcher can launch one type of missile.

**Cannons**

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
