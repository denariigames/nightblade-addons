# GMCommandExt

<img src="https://github.com/denariigames/nightblade-addons/blob/master/GMCommandExt/dist/screenshot.png?raw=true" alt="GMCommand-access" height="350">

An addon for NightBlade MMO replaces default DefaultGMCommands with a more flexible and extensible version, featuring granular access control, easy custom command development, and command logging.

- **Granular Access Control:** Supports six levels of GMcommand permissions based on the userLevel column in the userlogin table:
  - 0: Everyone
  - 1: Premium account
  - 2: Chat moderator
  - 3: Game moderator
  - 4: Admin
  - 5: Superadmin

- **Easy Extensibility:** Developers can quickly add custom commands using a streamlined plugin pattern.

- **Command Logging:** Logs commands filtered by user level for better moderation and auditing.

- **Data Sanitzation:** Set maximum amount for exp_rate, gold_rate, give_gold, gold, level, skillpoint and statpoint commands.

![GMCommandExt-config](https://github.com/denariigames/nightblade-addons/blob/master/GMCommandExt/gmcommandext-perms.png?raw=true)

### Usage

To integrate GMCommandExt into your NightBlade MMO:

1. create your own GMCommandsExt scriptable object with `Create > Create GameData > GMCommandExt` (default may be overridden on addon updates)
2. adjust command access levels directly on your `GMCommandsExt` as needed
3. in your InitScene, edit the `GameInstance` GameObject and reference your `GMCommandsExt` for GM Commands
4. update the userLevel field in the userlogin table for individual accounts

### Logging

Commands can be logged by User Level to help track usage for security and debugging purposes. The log format is:

`{System.DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ")}|GMCommand|{sender}|{command.category}|{commandKey}|{chatMessage}`

### Developers

GMCommandExt implements a DevExtMethods plugin pattern. **Version 2.0.0 introduces an entirely new, streamlined pattern for creating commands. Any custom commands from prior versions will need to be modified.**

1.  In your addon project, create a new C# file with a **partial class** extension for `GMCommandExt`:
```csharp
using UnityEngine;
using NightBlade.DevExtension;

namespace NightBlade
{
	public partial class GMCommandExt
	{
	}
}
```

2. Define the default access level:
```csharp
[SerializeField] private PlayerLevel myCommandAccess = PlayerLevel.Admin;
```

3. Register the command using the DevExtMethod:
```csharp
[DevExtMethods("RegisterCommand")]
private void GMCommand__myCommand()
{
	AddGMCommand(
        "my_command",                                      // Command name (without leading slash)
        myCommandAccess,                                   // Access level reference
        "This description is shown in /help my_command",   // Command description
        "{characterName} {item_id} {optional: amount}",    // Argument hints
        "Economy"                                          // Category
	);
}
```

4. Implement the command handler method (note the required naming convention with HandleGMCommand followed by two underscores and your command name):
```csharp
protected string HandleGMCommand__my_command(string sender, BasePlayerCharacterEntity characterEntity, string[] data)
{
	string response = "<color=red>You do not have access to this command</color>";

    if (data.Length == 2) // Adjust based on your expected arguments
    {
        // Your command logic here
        response = "<color=green>Command executed successfully!</color>";
    }

	return response;
}
```

### Command Categories

Commands are organized into categories for better /help output. Default categories include:

- General
- Moderation
- Player
- Economy
- Server

Feel free to create custom categories to group your own commands.
