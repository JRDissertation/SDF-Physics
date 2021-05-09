# SDF-Physics
This is an SDF-based physics system written for my dissertation. It's a Unity
project, creating a new Unity project and putting the Assets and ProjectSettings
into the project will allow you to use the physics system and the primitives.

To create a scene, add the RayMarcherComponent to the camera and set the Raymarcher and Physics compute kernels
in the RayMarcherComponent to the correct kernels.

To add SDF objects to the scene, add a GameObject and add a "Shape" script to it. From here, you can set the parameters
of the shape as you like. Physics are simulated when the play button is pressed (note that when the play button is pressed,
there is no more communication with the graphics card other than shader dispatches. You can't modify values of physics objects
after the simulation has started).
