# AirController

A plug 'n play unity plugin for [AirConsole](https://www.airconsole.com/) for easier development.

An earlier version of this library was used to handle all AirConsole related things when developing our game [Basher Beatdown for AirConsole](https://www.airconsole.com/play/multiplayer-games/basher-beatdown). This version has been cleaned up and extended upon, to make it useful to a much wider range of games.

## Philosophy

With AirConsole you have to work with two separate systems, with Unity (or another engine) for the game itself and HTML/Javascript for the controllers. When working with two separate systems that have to work together where both have the power to make decisions it can quickly become a mess. The state of one might not be what the other expects anymore at any point in time.

To prevent this from happening and to keep all logic in the same place the controller doesn't decide anything, but simply acts as a normal input device, nothing else. Of course using a mobile phone as a controller gives the great opportunity of being able to display different screens and buttons, but what needs to be shown when is completely controlled by the Unity plugin.

## Features

Controller:
* Easy HTML setup, no javascript required. 
* Out of the box support for buttons, buttons with int values, pan, swipe and tilt/orientation.
* Handles different screens, controlled by the Unity part.
* Handles hero users, can disable hero only buttons automatically. It will show the become hero screen if a non-hero pushes it.
* Smart sending of input, to minimize player input delay.
* HTML classes can be assigned by the Unity script, which then gets put in the body tag for easy CSS adjustments. (Think colors, enabling/disabling of things).
* Easy profile picture loading
* Drag & swipe events
* Device orientation and motion

Unity:
* Auto handles the joining of devices.
* Auto handles the creation of player objects, which can be assigned to a device.
* Auto reconnects a device to a player after connection loss.
* The state of the controller is controlled by the device class in unity and thus allows for easy adjustment from in game. Show a different screen when a user is gameover? Easy, just change one value!
* AirController handles all the input sent by the controller. All you have to do is Player.Input.GetKeyDown("jump");
* Custom editor to keep track of what's going on.
* Auto profile picture loading
* Auto orientation detection
* Orientation based roll/tilt
* Auto handling of savedata
* Send custom data to the controller

# Hall of Fame

These games are powered by AirController:

[The Underground King](https://www.airconsole.com/play/the-underground-king)

# Usage

AirController consists out of four parts, the Aircontroller, Player, Device and Input.

One could argue that players and devices could be the same, but the fact that they are separated made certain situations easier. When a game starts you might request all Player objects and assign them to your own player object to get the input from. If player and device are the same what would happen when a device disconnects? Now a device can disconnect and be auto assigned back to its player. Your code now doesn't have to worry about anything.

Another example would be our game Basher Beatdown where there are only four players allowed. When the game starts, four players are created, and can then be claimed by a device. In this case, you can still receive input from all players (Devices) that joined.

## Warning

The script execution order must be set in the right order for the system to work. They should be in this order:
```
NDream.AirConsole.AirConsole
SwordGC.AirController.AirController
[Extended AirController Class]
Default Time
```

## Getting started

The most minimal example of how the input in AirController works is something like this:

HTML:
```HTML
<div id="jump" air-hold-btn="jump">Jump</div>
```
Unity:
```C#
foreach (Player p in AirController.Instance.Players.Values) {
    if(p.Input.GetKeyDown("jump")){
        Debug.Log("Pressed jump");
    }
}
```

To create pages/views the most basic example is this:

HTML:
```HTML
<div id="menu" air-page="Menu" class="page">
	<!-- page content -->
</div>
<div id="gameplay" air-page="Gameplay" class="page">
	<!-- page content -->
</div>
```
Unity:
```C#
foreach (Player p in AirController.Instance.Players.Values) {
    p.Device.View = "Menu";
}
```

## Player and Device

To account for different use cases and to make it easy for each of these cases the player and devices are seperated. In general game logic only needs to worry about know about the players. AirController will make sure that devices are connected to a player (Although this can also be done custom)

### Example 1: Fixed number of players, X number of devices (JoinMode custom)
This was the use case for Basher Beatdown, there can only be 4 players and there always are 4 players. In this case AirController will create 4 players when it starts. For each connected device a Device object will be created, but it won't be assigned to a player. Only once a device specificly claims a player they get connect and that device will play in the game.

```
JoinMode: Custom
MaxPlayersMode: Limited, 4
```

![Fixed number players, X devices](https://i.imgur.com/b3ykYXx.png)

### Example 2: Fixed number of players, X number of devices (JoinMode auto)
This is a very similar to example 1, but might be easier if you don't want to include the claim/unclaim of players but rather just have the first X players playing. It will create a player for each device when it connects untill it reaches the limit.
```
JoinMode: Auto
MaxPlayersMode: Limited, 4
```

![Same number of players and devices](https://i.imgur.com/BciFVXQ.png)

### Example 3: Same number of players as devices
This case is for games like Cards Against Humanity on AirConsole where there's no limit to the amount of players in the game. In this case each device will just get it's own player.

```
JoinMode: Auto
MaxPlayersMode: Auto
```

![Same number of players and devices](https://i.imgur.com/LvM12Lm.png)

## Docs
A more in depth explanation of everything can be found in the [wiki](https://github.com/crashkonijn/AirController/wiki/AirController-(Unity))

