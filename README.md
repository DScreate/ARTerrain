# Man-in-the-Mountain
EWU Winter 2018 Senior Project

Background: This project was constructed from scratch for the 2018 Microsoft TEALS Conference.

In brief, the goals of this project were to describe, design and construct a product to be presented to high achieving high school students with two specific intentions:

1. Eliciting a positive remotional response so as to encourage interest in Computer Science as a whole
2. Display and show off different interesting technologies interacting in unique ways

How to Run: A webcam IS required in order for this project to operate properly. To run this project, first download the currently available working release

It should be a zip file that contains the project exe file, a Unity generated NAME_data folder and a Unity generated .dll file.

Simply run the exe file and the software will run.

The program is designed so that it will search through available webcam devices tagged as such by the operating system, starting with the first device being managed. Under Camera Options in the Menu, the "Change Camera" button can be clicked in order to move forward in the list of available devices known to the operating system, with this button also causing the software to loop back at the start of known devices once that list is exhausted.

Known Issues: Dependent upon operating system, sometimes the software is unable to access the camera. This is usually the result of security based permissions. Make sure your webcam is both accessible and that no security software is blocking this software from accessing your webcam.

-------------------------------------------------------------
UI

Webcame Controls:
Play -> activates data feeding from currently selected webcam
Pause -> deactivates data feeding to program but does not deactivate webcam
Speed -> controls the speed at which virtual camera moves about scene
Change Camera -> cycles through detected available camera devices provided by OS

Texture Controls:
Each slider controls the height at which a texture is applied.
The blank text entry boxes can be used for manual entry of a given value. They do NOT update the sliders.

The controls are, in order:
Deep Water
Water
Sand
Grass
Mountain
Snow

Map Controls:
Texture Scaling -> Controls the overall texture height-based application algorithm. It is of most use in extreme lighting conditions (very bright or very dark)
Height Multiplier -> Controls the multiplier for Mesh Height without influencing texturing

Noise Controls:
Scaling -> Controls "zooming" of perlin noise data that is used to create aesthetic improvements to map appearance
Octaves, Persistance, Lacunarity -> Control associated noise values. For those unfamiliar with Perlin noise generation, the following analogy is provided:
Octaves -> Mountains
Persistance -> Boulders
Lacunarity -> Rocks and Debris
