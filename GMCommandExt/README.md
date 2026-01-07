![GMCommand-access](https://github.com/denariigames/BuildingEntityManager/assets/755461/2dbc71dc-9eab-41cf-ac37-9acf43e77cd9)

Addon for [MMORPG Kit](https://assetstore.unity.com/packages/templates/systems/mmorpg-kit-2d-3d-survival-110188) replaces DefaultGMCommands with an enhanced implementation. 

- Supports six levels of chat command access control from userLevel (0 = everyone, 1 = premium account, 2 = chat moderator, 3 = game moderator, 4 = admin, 5 = superadmin)
- ~~Restricted commands (/ban, /kick, /kill, /mute, /warp_character) can optionally not apply to players of a higher level (ChatModerator cannot mute Admin)~~
- Developers can easily extend and implement their own commands

Last update: Kit 1.88

![GMCommandExt-config](https://github.com/denariigames/BuildingEntityManager/assets/755461/043fca0c-9ab8-4880-b57e-ba4d02aa82f2)

### Setup

1. Reference DefaultGMCommandsExt in GameInstance GM Commands.

2. Modify command access on DefaultGMCommandsExt as required.

3. Modify userLevel field in userlogin table as required 

### Developers

GMCommandExt implements a DevExtMethods plugin pattern. In your own addon, create a new file for a partial to class GMCommandExt for each command you wish to implement. There are four required sections to the partial.

1. Define the default access and the command string that will be recognized in chat.
```
[SerializeField] private PlayerLevel fooAccess = PlayerLevel.Admin;
public const string fooCommand = "/foo";
```

2. Implement the /help yourCommand response with a desciption and any command parameters required. Note that the detail logic is wrapped with a conditional against the helparg.
```
[DevExtMethods("HelpGMCommand")]
protected void HelpGMCommand_foo(int userLevel)
{
	if (userLevel >= (int)fooAccess)
		s_responseGmCommand += fooCommand + "  ";
}

[DevExtMethods("HelpGMCommandDetail")]
protected void HelpGMCommandDetail_foo(string helpArg, int userLevel)
{
	if (helpArg.Equals(fooCommand.ToLower()) || fooCommand.ToLower().Equals("/" + helpArg))
	{
		if (userLevel >= (int)fooAccess)
			s_responseGmCommand = "<color=white>Describe your command</color>\n/foo";
		else
			s_responseGmCommand = "<color=red>You do not have access to this command</color>";
	}
}
```

3. Return true for IsGMCommand and CanUseGMCommand only in the scope of your command. Note that the logic is wrapped with a conditional against the command.
```
[DevExtMethods("IsGMCommand")]
protected void IsGMCommand_foo(string command)
{
	if (command.Equals(fooCommand.ToLower()))
	{
		b_isGmCommand = true;
	}
}

[DevExtMethods("CanUseGMCommand")]
protected void CanUseGMCommand_foo(string command, int userLevel)
{
	if (command.Equals(fooCommand.ToLower()))
	{
		if (userLevel >= (int)fooAccess)
			b_canUseGMCommand = true;
	}
}
```

4. Execute your command logic only in the scope of your command. Note that the logic is wrapped with a conditional against the command. See the DefaultCommands for more examples.
```
[DevExtMethods("HandleGMCommand")]
protected void HandleGMCommand_foo(string sender, BasePlayerCharacterEntity characterEntity, string command, string[] data)
{
	if (command.Equals(fooCommand.ToLower()))
	{
		s_responseGmCommand = $"<color=green>Foo bar</color>";
	}
}
```