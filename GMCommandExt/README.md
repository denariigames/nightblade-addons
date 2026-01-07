# GMCommandExt

<img src="https://github.com/denariigames/BuildingEntityManager/assets/755461/2dbc71dc-9eab-41cf-ac37-9acf43e77cd9" alt="GMCommand-access" height="350">

An addon for NightBlade MMO replaces DefaultGMCommands with an enhanced implementation. 

- supports six levels of chat command access control from userLevel column in the userlogin table (0 = everyone, 1 = premium account, 2 = chat moderator, 3 = game moderator, 4 = admin, 5 = superadmin)
- developers can easily extend and implement their own commands
- **(New)** command logging by User level

### Usage

1. reference DefaultGMCommandsExt in GameInstance GM Commands.
2. modify command access on DefaultGMCommandsExt as required.
3. modify userLevel field in userlogin table as required 

![GMCommandExt-config](https://github.com/denariigames/BuildingEntityManager/assets/755461/043fca0c-9ab8-4880-b57e-ba4d02aa82f2)

### Logging

Commands can be logged by User Level. The log format is:

`[GMCommandExt] {System.DateTime.Now:yyyy-MM-dd HH:mm:ss}|{command.category}|{sender}|{chatMessage}`


### Developers

GMCommandExt implements a DevExtMethods plugin pattern. **Version 2.0.0 introduces an entirely new, streamlined pattern for creating commands. Any custom commands from prior versions will need to be modified.**

1. In your own addon, create a new file for a *partial to class GMCommandExt* for each command you wish to implement.
```
using UnityEngine;
using NightBlade.DevExtension;

namespace NightBlade
{
	public partial class GMCommandExt
	{
	}
}
```

2. Define the default *PlayerLevel* access for the command.
```
[SerializeField] private PlayerLevel myCommandAccess = PlayerLevel.Admin;
```

3. Implement the *RegisterCommand* DevExtMethod.
```
[DevExtMethods("RegisterCommand")]
private void GMCommand__myCommand()
{
	AddGMCommand(
		"my_command", //string that will be recognized as your command, note the prefix slash is not included
		myCommandAccess, //reference to the PlayerLevel field above
		"This description is shown in /help my_command", //description of your command
		"{characterName} {item_id} {optional: amount}", //arguments shown below description of your command
		"Economy" //category (see below)
	);
}
```

4. Implement your command handler method. Note that it is magic-named and the method must follow the naming convention.
```
protected string HandleGMCommand__myCommand(string sender, BasePlayerCharacterEntity characterEntity, string[] data)
{
	string response = "<color=red>You do not have access to this command</color>";

	//perform any data length checks depending on your arguments
	if (data.Length == 2)
	{
		//perform your command logic and update response
		response = $"<color=green>Command did the thing</color>";
	}

	return response;
}
```

### Command Categories

Commands are grouped in categories and the default categories are:

- General
- Moderation
- Player
- Economy
- Server

You are free to add your own category name if you want to group your custom commands together.
