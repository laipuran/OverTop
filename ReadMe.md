# Over Top
## This is a tool for deep computer users
### The Way to Run the App
```
git clone https://github.com/laipuran/OverTop
cd ...(To the folder of the app)
dotnet restore
dotnet run
```
Congratulations! You have already started the app.
### This App has the features below:
### 0. Main Window
- The Main color of the window will change when the System Glass Brush changes
- Tab to hide MainWindow
- Close Main Window to close all the floatings closed
### 1. Create Floating Windows
### Control Panel
- Can create the two kinds of floating windows below
- Can load a Hanger Window from a .json file
- by click the button or drag the file to the button
### Hangers
- Left Ctrl to save the window to a GUID.json file at the user documents folder
- Push Key R and left click to remove the item
- Push Key M and left click to modify the item
### Recent
- It automatically gets the files from the %AppData%/.../recent folder, and displays them in the list
- Push Key Left Ctrl and left click to open that file/folder
- Push Key R and left click to reload the files
### The Same Features
- Escape to close the window
- Right Click on the window to configure it
- Left (Double) Click and drag to move the window
- Tab to set the window to the top or the bottom of other windows
### Property Window
- Can set window's Size, Opacity, Background Color, Contents
- And can set the values to default
- Escape to close the window without saving the settings
- Close the window to save the settings to window parameters
### App Window
- Right Click on the window to add app links
- Double Click to activate MainWindow
- Push Key R and left click an element to remove it
- Push Key C and left click to remove all the elements in the window
- AppWindow is like a magnet, it can stick to any side of the screen.
- It can not be closed, but it shares the same feature(3, 4) as other floatings
### Chooser Window
- Left click the icons to add them to the App Window
- Push Key R and left click the window to refresh all the items
- It shares the same feature(1, 3, 4)
- Click the button to add any other link to the App Window
### 2. Get some values from SystemParameters
- Get Window Glass Color
- Get Desktop Brush
- Click Fast to find the painted eggshell