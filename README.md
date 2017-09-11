# AirController

A plug 'n play unity plugin for [AirConsole](https://www.airconsole.com/) for easier development.

An earlier version of this library was used to handle all AirConsole related things when developing our game [Basher Beatdown for AirConsole](https://www.airconsole.com/play/multiplayer-games/basher-beatdown). This version has been cleaned up and extended upon, to make it useful to a much wider range of games.

## Philosophy

With AirConsole you have to work with two separate systems, with Unity (or another engine) for the game itself and HTML/Javascript for the controllers. When working with two separate systems that have to work together where both have the power to make decisions it can quickly become a mess. The state of one might not be what the other expects anymore at any point in time.

To prevent this from happening and to keep all logic in the same place the controller doesn't decide anything, but simply acts as a normal input device, nothing else. Of course using a mobile phone as a controller gives the great opportunity of being able to display different screens and buttons, but what needs to be shown when is completely controlled by the Unity plugin.

## Features

Controller:
* Easy HTML setup, no javascript required. 
* Out of the box support for buttons, buttons with int values and joysticks
* Handles different screens, controlled by the Unity part.
* Handles hero users, can disable hero only buttons automatically. It will show the become hero screen if a non-hero pushes it.
* Smart sending of input, to minimize player input delay.
* HTML classes can be assigned by the Unity script, which then gets put in the body tag for easy CSS adjustments. (Think colors, enabling/disabling of things).
* Supports vibration on button press.

Unity:
* Auto handles the joining of devices.
* Auto handles the creation of player objects, which can be assigned to a device.
* Auto reconnects a device to a player after connection loss.
* The state of the controller is controlled by the device class in unity and thus allows for easy adjustment from in game. Show a different screen when a user is gameover? Easy, just change one value!
* AirController handles all the input sent by the controller. All you have to do is Player.Input.GetKeyDown("jump");
* Custom editor to keep track of what's going on.

---

# Usage

AirController consists out of four parts, the Aircontroller, Player, Device and Input.

One could argue that players and devices could be the same, but the fact that they are separated made certain situations easier. When a game starts you might request all Player objects and assign them to your own player object to get the input from. If player and device are the same what would happen when a device disconnects? Now a device can disconnect and be auto assigned back to its player. Your code now doesn't have to worry about anything.

Another example would be our game Basher Beatdown where there are only four players allowed. When the game starts, four players are created, and can then be claimed by a device. In this case, you can still receive input from all players (Devices) that joined.

## AirController (Unity)

The AirController is the base class that handles everything, even though AirController works out of the box it is advised that all 3 classes get extended for custom behavior.

![AirController editor window](http://i.imgur.com/u6ziTRl.png)

### Minimal implementation

```C#
using SwordGC.AirController;

public class ExampleAirController : AirController {

    public override Player GetNewPlayer(int playerId)
    {
        return new ExamplePlayer(playerId);
    }

    protected override Device GetNewDevice (int deviceId)
    {
        return new ExampleDevice(deviceId);
    }
}
```

### Variables

```C#
// TOGETHER: All devices get marked hero.
// SEPERATE: Only the hero device will be marked hero.
public enum HEROMODE { TOGETHER, SEPERATE }
public HEROMODE heroMode = HEROMODE.TOGETHER;

// AUTO: A device will automatically try to claim a player
// CUSTOM: A device won't claim a player (for a join now screen?). 
// 
// Note: When set to custom you can easily claim a player by sending a button called "claim" from the device
public enum JOINMODE { AUTO, CUSTOM }
public JOINMODE joinMode = JOINMODE.AUTO;

// AUTO: The system will create a player for each device.
// LIMITED: The system will create the specified amount of players and will only allow that many
public enum MAXPLAYERSMODE { AUTO, LIMITED }
public MAXPLAYERSMODE maxPlayersMode = MAXPLAYERSMODE.AUTO;

// Becomes true when the AirConsole plugin is ready
public bool IsReady { get; private set; }

// Becomes true when OnPremium is called
public bool HasHero { get; private set; }

// The maximum amount of players when maxPlayersMode is set to LIMITED
public int maxPlayers = 4;

// Turns all debugs on and off
public bool debug = true;

// Contains all players
public Dictionary<int, Player> Players { get; protected set; }

// The amount of players that are created but not claimed
public int PlayersAvailable;

// Contains all devices
public Dictionary<int, Device> Devices { get; protected set; }
```

### Functions

Note: The AirController already includes all AirConsole functions as virtual voids, which can all be overriden.

```C#
// Called when a new player is needed, override this function to insert your own Player extended class
public virtual Player GetNewPlayer (int playerId) {}

// Returns the matching player, returns null when none is found
public Player GetPlayer(int playerId) {}

// Call this to reset all players to it's default state. (Which is none, or maxPlayers when maxPlayerMode == LIMITED
public void ResetPlayers() {}

// Claims a player for a device
public void ClaimPlayer(int playerId) {}

// Tries to reconnect a device to a previously claimed player
public bool ReconnectWithPlayer (int deviceId) {}

// Returns the matching Device, returns null when none is found
public Device GetDevice (int deviceId) {}

// Called when a new device is needed, override this function to insert your own Device extended class
protected virtual Device GetNewDevice (int deviceId) {}

// Checks if a device has a player object
public bool DeviceHasPlayer(int deviceId) {}

// Return the player based on a deviceId
public Player GetPlayerFromDevice(int deviceId) {}

// Sends the current device states to the devices
public void UpdateDeviceStates() {}

// Resets the input, is called on LateUpdate
public void ResetInput() {}

```

## Player

### Variables

```C#
// Cached reference to the controller
AirController airController;

// The current state of this player
public enum STATE { UNCLAIMED, CLAIMED, DISCONNECTED }
public STATE state = STATE.UNCLAIMED;

// Wrapper for the device nickname
public string Nickname;

// Wrapper for the device profile picture
public Texture2D ProfilePicture;

// The id of this player
public int PlayerId { get; private set; }

// The id of the connected device
public int DeviceId { get; private set; }

// Returns the device of this player
public Device Device;

// Returns the input of this player
public Input Input;
```

### Functions

```C#
// Claims this player for a device
public Player Claim(int deviceId) {}

// Unclaims this player from it's device
public void UnClaim(){}

// Sets it's state to disconnected
public void Disconnect() {}
```

## Device

### Minimal/Example implementation
```C#
using SwordGC.AirController;

public class ExampleDevice : Device {

	public ExampleDevice (int deviceId) : base(deviceId)
    {

    }
	
	// required
    public override string View
    {
        get
        {
            if (Player == null) return "NotJoined";
            else return "Joined";
        }
    }
	
	// purely example
	public override string Classes
	{
		get
		{
			return "Color" + player.ColorId;
		}
	}
}
```

### Variables
```C#
// Reference to the input of this device
public Input Input { get; protected set; }

// Set to true when this specific device is here
private bool isHero;

// Returns true when this device is hero or heromode === TOGETHER and there's at least one hero in the party
public bool IsHero;

// The id of this device
public int DeviceId { get; private set; }

// Returns the playerId of the claimed player.
// When no player is claimed it will return -1
public int PlayerId;

// Returns the connected player
public Player Player;

// Returns the Nickname of this device
public string NickName;

// Should return the current view of the controller
public virtual string View;

// Should return the classes that should be inserted on the controller
public virtual string Classes;

// Holds the profile picture of the device in 512x512 px
public Texture2D ProfilePicture { get; private set; }
```

## Input

### Functions
```C#
// Returns true if the button was pressed this frame
public bool GetKey (string key);

// Retuyrns true if the button was pressed down this frame
public bool GetKeyDown (string key);

// Returns the axis of an object
// Example: The controller holds a joystick called Movement
// GetAxis("MovementHorizontal") will return the x value of movement
public float GetAxis (string key);

// Returns the vector of a given key
public Vector2 GetVector (string key);
```

# AirController (HTML/JS)

## HTML

The HTML follows a very simple boilerplate to make the controller. The AirController.css will remove all default CSS off a browser so you start off with a blank slate that will completely fill the screen.

### Page
First off everything has to be put in pages, these pages can be switched by the unity plugin. A page is created like this:

```HTML
<div id="Name" air-page="Name" class="page">
	<!-- page content -->
</div>
```

### Button (tap)
By adding the following to an HTML object it will become a button that sends onTouchStart events:
```HTML
air-tap-btn="name"
```

### Button (hold)
By adding the following to an HTML object it will become a button that sends both onTouchStart and onTouchEnd events:
```HTML
air-hold-btn="name"
```

### Button with int value
The below example shows how to add a value to a button
```HTML
air-tap-btn="name:0"
air-hold-btn="name:1";
```

### Hero only button
By adding the following to an HTML object it will become a hero only. The button won't send any information as long as the device is not marked as a hero, but will instead open up the become hero screen.
```HTML
air-hero="true"
```
When the following class is added to an object it's alpha will become 0.5f when the device is not marked as hero.
```HTML
class="herodisabled"
```

### Joystick
By adding the following to an HTML object it will become a joystick:
```HTML
air-joystick="name"
```

### Profile picture
By adding "air-profile-picture" to an HTML element it's CSS background-image value will be set to the profile picture 

## Javascript

This is all javascript that's needed at the end of your HTML

```Javascript
controller.init(page, orientation, vibrate);
```
Example:
```Javascript
controller.init("Loading", AirConsole.ORIENTATION_LANDSCAPE, true);
```


## Example
```HTML
<html>
<head>
    <meta name="viewport" content="user-scalable=no, width=device-width, initial-scale=1.0, maximum-scale=1.0"/>
    <link rel="stylesheet" type="text/css" href="AirController/css/AirController.css">
    <style>
        .button {
            float:left;
            height:50%;
            width:50%;
            text-align: center;
            vertical-align: middle;
            line-height: 50vh;
        }
        #not_joined_joystick, #joined_joystick {
            background-color: #8e79ff
        }
        #not_joined_button {
            background-color: #cbff50
        }
        #joined_button {
            background-color: #00ff00
        }
        #not_joined_hero, #joined_hero {
            background-color: #ff6775
        }
        #not_joined_number, #joined_number {
            background-color: #56ffe4
        }
        #joined_profile {
            background: url("http://via.placeholder.com/512x512") no-repeat center center;
        }
    </style>
</head>
<body class="">
    <div id="Loading" air-page="Loading" class="page">
        <h1>Loading</h1>
    </div>
    <div id="NotJoined" air-page="NotJoined" class="page">
        <div id="not_joined_joystick" air-joystick="joystick" class="button">Joystick</div>
        <div id="not_joined_button" air-hold-btn="claim" class="button">Claim</div>
        <div id="not_joined_hero" air-tap-btn="hero" air-hero="true" class="button herodisabled">Hero only</div>
        <div id="not_joined_number" air-tap-btn="number:6" class="button">Number Button</div>
    </div>
    <div id="Joined" air-page="Joined" class="page">
        <div id="joined_profile" air-profile-picture class="button"></div>
        <div id="joined_button" air-tap-btn="button" class="button">Button</div>
        <div id="joined_hero" air-tap-btn="hero" air-hero="true" class="button herodisabled">Hero only</div>
        <div id="joined_number" air-tap-btn="number:6" class="button">Number Button</div>
    </div>

    <script type="text/javascript" src="https://www.airconsole.com/api/airconsole-latest.js"></script>
    <script src="AirController/js/virtualjoystick.js"></script>
    <script src="AirController/js/victor.min.js"></script>
    <script src="AirController/js/jquery-1.11.3.min.js" type="text/javascript"></script>
    <script src="AirController/js/hammer.min.js" type="text/javascript"></script>
    <script type="text/javascript" src="AirController/js/AirController.js"></script>

    <script>
        controller.init("Joined", AirConsole.ORIENTATION_LANDSCAPE, true);

        controller.onData = function (customData) { console.log("onData: "); console.log(customData); };
        controller.onShowPage = function (newPage) { console.log("onShowPage "); console.log(newPage); };
        controller.onBecameHero = function () {};
    </script>
</body>
</html>
```
This example will output this:

![Rendered version of above example](http://i.imgur.com/snqW4YY.png)







