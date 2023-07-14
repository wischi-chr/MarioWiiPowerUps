# Mario Wii Red Toad House Power Up Panel Solver
A little web tool to identify power-up panels in *New Super Mario Bros. Wii.*

![UI Screenshot](./res/MarioWiiUI.png)

## How to use it

1. Start https://wischi-chr.github.io/MarioWiiPowerUps/
2. Open the highlighted section in-game
3. Click shown item and repeat.
4. Profit - *Get all power-ups*

### Details

| Icon | Text |
|:------|:------|
| ![regular field](./res/explain_regular.png) | This kind of field is shown if no useful information could be deduced so far. In the beginning all fields are in that state. |
| ![highlighted field](./res/explain_highlight.png) | The highlighted dark border around the field signals that this is the currently selected field. The field is automatically selected by an algorithm that helps you to find a solution for your in-game board without revealing two matching bowsers. You can select a different field by clicking at the board. |
| ![safe spot](./res/explain_safe_spot.png) | The dark shaded spots are so called *"safe spots"*. The tool can't tell you at that point what exact item is behind that field, but it knows for a fact that it's not a bowser. |
| ![translucent icon](./res/explain_translucent.png) | When the exact item can be determined it will be shown in a semi-transparent way. |
| ![finish flag field](./res/explain_finish.png) | This kind of field is a *safe spot* that is guaranteed to solve the entire panel. |
| ![unknown bowser field](./res/explain_danger.png) | If the tool knowns that a given situation will reveal a bowser, but doesn't know yet which one (regular bowser, or bowser junior) this field will be shown. |
| ![POW button](./res/explain_pow.png) | This button will reset the entire state of the board. |
| ![bullet button](./res/explain_bullet.png) | This button will undo your last board change.<br><sup>*Note: selecting a different field is not considered a board change*</sup> |
| ![help button](./res/explain_help.png) | This button opens a short help note. |

## Background and technical stuff
In 2016 I wrote a little (quick & dirty) prototye in C# (WPF) to solve power up panels, but it wasn't very practical because it was windows-only and you'd need to start a PC or notebook just to help with a few power-up panels. So I decided to make a web app and ported it with bridge.net. Because I hate fiddling with HTML and CSS I used a canvas and drew everything by code ;-) Not the best option but it does the job for this tool.
At the moment the code is bit of a mess (and I probably won't fix that, because it's not worth it) so be kind to it :smile:

### "The algorithm"
There is a fixed set of power-up panels in *New Super Mario Bros. Wii* (see [MarioPowerUpBoard.txt](https://github.com/wischi-chr/MarioWiiPowerUps/blob/master/res/MarioPowerUpBoard.txt)) and the software calculates (based on the input) what fields you should open next.

The algorithm (currently - it went through a few iterations) uses a few heuristics to determine what the next best candidate is. It takes into account which fields are bowsers (it will never suggest a matching bowser), the distance to the previous opened field (so you can reach it fast in-game) and the entropy of the field. The algorithm needs 3 ± 1 steps to solve the entire panel. We had a "faster" algorithm in the past, but it wasn't as practical, because it suggested more bowsers (also never a matching pair) and didn't take the moving distance in-game into account.

## Install as app
This tool is a PWA and you can add it to your homescreen on many platforms (only tested with chrome and android) and it will launch in landscape mode and fullscreen.
