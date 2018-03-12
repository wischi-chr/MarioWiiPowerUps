# MarioWiiPowerUps
A little web tool that identify power-up panels in New Super Mario Bros Wii

![UI Screenshot](https://github.com/wischi-chr/MarioWiiPowerUps/blob/master/res/MarioWiiUI.png)

## How to use it

1. Start https://wischi-chr.github.io/MarioWiiPowerUps/
2. Open the marked section in *New Super Mario Bros. Wii* (PowerUp Panel)
3. Click shown item and repeat.

* Click on the board to select a different spot.
* Use *"POW"* to reset the board
* Click the bullet to undo the last step.

The algoritm needs 2-3 items (~2,43 on average - if you use the suggested fields) to find the correct panel. Sometimes the algorithm suggests a spot which may be a bowser (to find the correct panel faster) but would never suggest a matching bowser.
After it found the correct panel all items are shown.

You can add it to your homescreen on most mobile plattforms (only tested with chrome and android) and it will launch in landscape mode and fullscreen.

## Background
About two years ago I wrote a little (quick & dirty) prototye in C# (WPF) but it was not very practically to start the notebook just to help with a few power-up panels. So I decided to port it (with bridge.net) and make a web app. Sadly I'm not very good with html and css designs so I used a canvas and drew everything by code ;-) Not the best option but it does the job.
At the moment the code is bit of a mess so be kind to it :smile:

## How does it work
There is a fixed set of power-up panels in *New Super Mario Bros. Wii* (see [MarioPowerUpBoard.txt](https://github.com/wischi-chr/MarioWiiPowerUps/blob/master/res/MarioPowerUpBoard.txt)) and the software calculates (based on the input) which field has the highest entropy and suggests that as the next input to find panels as fast as possible.
