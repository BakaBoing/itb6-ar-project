# README
This is the project for the Augmented Reality Course at FHV in 2020 from team VR with Glasses (Pascal Grones, Janik Mayr, Patrick Poiger, Matthias Rupp).

## Goal of the project
The goal of the project is to use the Vuforia Engine in Unity to create an AR-application that recognizes pre-scanned objects and shows information about them as well as usage instructions next to the recognized object.
A paper will be written about our findings.

## Current status

### Object Recognition
The Vuforia Engine is fully integrated and works.
Currently, the application can recognize the following objects: 
  - a battery 
  - small model car
  - printer (HP Deskjet)
  - a QR-cube.
  
The cube can be made by oneself by going into Assets\Resources\Printables and printig out the QR-cube pdf.
The cube can then be folded and scanned with the application. It was chosen because everyone can make them at home, and are not dependent on having the same kind of battery or printer (an attempt was made to use a 50-cent coin instead, but object recognition does not work reliably on such a small and flat object).
The information displayed currently is the name, a paper format and an (currently empty) image.

### Information Storage
Currently, information regarding the objects is stored in a script (DataStoreDummyImpl.cs). However, with the way the information retrieval is set up, this implementation can be switched for one using a different approach (e.g. a database) easily.

### Control
To control the application (e.g. scrolling through instructions), a lot of approaches are being tested. Currently, gesture recognition with Manomotion SDK Lite is being investigated (on the dev-branch).
Virtual Buttons were considered, but unfortunately they only work for planar image targets. Alternatively, if gesture controls does not work, a first attempt at a scrollbar-implementation was also made.
