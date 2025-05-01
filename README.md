# Power Macros

Visual Studio extension that provides functionality to store and playback macros and code snippets.

## What you can do

- Store snippets of code that you can paste into the active window by keyboard shortcut or by a search pop-up.
- Record and playback key strokes.
- Manage and Edit your macros.
- Up to 10 keyboard shortcuts assigned automatically.
- Add a marker \$end\$ to move the caret to that position (similar to the snippet tool)

## How to use

### Macro Manager
Open the Macro Manager window with the shortcut [Shift+Ctrl+M, M] or in the Tools menu.

![image](https://github.com/user-attachments/assets/9f71a3b1-52ff-4a30-aa76-eba92db799ea)

Here you can create, edit and apply macros.\
The top 10 macros will have assigned to them the shortcut [Shift+Ctrl+M, $number$]\
Move the entries up and down to control what macro gets what shortcut

### Macro Editor
When you add a new macro or edit an existing one you will see the edit macro window.

![image](https://github.com/user-attachments/assets/bd639714-a9aa-4cef-8240-7fb838c41337)

if you select the type $Action$, press $Record$ to start recording key presses and then $Stop$ to end the recording

![image](https://github.com/user-attachments/assets/32ee59e4-4358-4091-bf89-1236eab6d08a)

Press $Save$ to save the current macro.

### Quick Macro

![quick_macro](https://github.com/user-attachments/assets/50ba6840-e691-4af1-9d69-1e45c318cebd)

Press the shortcut [Shift+Ctrl+M, N] to open the Quick Macro window.\
Search the macro by name, with autocomplete.\
Press enter to apply the macro and close the window.

### Action Macro

![quick_macro_2](https://github.com/user-attachments/assets/c9f7a470-d9af-49c6-8292-7857fc2d2947)

Action macros record and simulate key presses using the windows API.\
All keys, including modifiers, are available.\
Key down and Key up events are recorded separately to allow more complex combinations.

## License

GNU GPL
