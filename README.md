# ISReverb

This Unity project aims at graphically simulating reverb in a room by using the Image Sources method.

## Description

Given the surfaces of a room and the positons of source and listener, sound reflectons are represented by Image Sources (ISs).
ISs are clones of the original source that simulate the original sound being heard by the listener from different directions because of reverberation. Specifically, they used to compute the paths followed by sound rays inside the room. Sound rays, in turn, serve to approximate with acceptable accuracy the behaviour of soundwaves.

The focus of this project is the generation of ISs and the validation of reflection paths for their sound rays, with special attention to implementing different opmizations to drastically cut time costs and achieve real-time computations. The simulation aslo allows to dinamically enable or disable different optimizations independently for benchmarking.

For the full process and algorithms implemented in this simulation, refer to the full documentation inside this folder.

## Getting Started

### Dependencies

Running the project requires installing Unity Hub and Unity Editor 6000.0.26f1.

### Executing program

Download this folder and, on the Unity Hub, add it as a project from disk.
If not already installed, Unity Hub will automatically download the correct editor.

After opening the Unity project, navigate to the *Asstes/Scenes* folder to select one of the provided scenes, the *ShoeBox* scene is a good start.

After running the chosen scene, in the *Scene Hierarchy* window, under *Sources*, click on the *Source1* object.
The *Inspector* window will show all components attached to the object: under the *Source* component, different fields allow to personalize and launch Image Sources generation, as well as debug and view all sound rays and Image Sources. Just hover on a field to see its tooltip.

In the *Scene Viewer* window, while holding down the right mouse button, freely move and look around using the WASD buttons and moving your mouse.

## Authors

[Gianmaria Forte](www.linkedin.com/in/gianmaria-forte-278306261)