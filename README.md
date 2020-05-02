# README
This is the project for the Augmented Reality Course at FHV in 2020 from team VR with Glasses (Pascal Grones, Janik Mayr, Patrick Poiger, Matthias Rupp).

## Goal of the project
The goal of the project is to use the Vuforia Engine in Unity to create an AR-application that recognizes pre-scanned objects and shows information about them as well as usage instructions next to the recognized object.
Said information will be displayed in a scrollable UI. If possible, gesture recognition will be added to allow the user to scroll with hand gestures.
A paper will be written about our findings.

## Requirements
In order for the application to work properly, an Android Device is required. Testing the project in Unity is possible, but Manomotion's gesture recognition does not work properly when using Unity's play mode.
It is recommened to switch to Android in the project's build settings in Unity (File -> Build Settings -> click on Android -> click "Switch Platform"), as Manomotion cannot find a needed dll-file otherwise (it still does not work even then, for some reason).
All other functionality (object recognition, scrolling with Keyboard/Buttons/By grabbing pane) works even when playing in Unity.

## Object Recognition
The Vuforia Engine is fully integrated and works.
Currently, the application can recognize the following objects: 
  - a battery 
  - small model car
  - printer (HP Deskjet)
  - a QR-cube.
  
The cube can be made by oneself by going into Assets\Resources\Printables and printig out the QR-cube pdf.
The cube can then be folded and scanned with the application. It was chosen because everyone can make them at home, and are not dependent on having the same kind of battery or printer (an attempt was made to use a 50-cent coin instead, but object recognition does not work reliably on such a small and flat object).
The information displayed currently is the name, a paper format, an image and instructions on how to use the recognized object. Please note that writing printer instructions, for example, is not the main goal of this project, so a long placeholder text was used to simulate more complex instructions instead.

## Information UI
The UI that is displayed when an object is recognized consists of a redish pane that is overlayed over the object and has a scrollbar. Inside the pane, information is displayed based on which object was recognized.
Currently, this is the name of the object, a paper format, an image, and instructions in text. Furthermore, two buttons for scrolling by mouseclick/touchscreen are displayed only when there is a currently recognized object.

## Information Storage
Currently, information regarding the objects is stored in a script (DataStoreDummyImpl.cs). However, with the way the information retrieval is set up, this implementation can be switched for one using a different approach (e.g. a database) easily.
Since database development was outside the scope of the project, the DummyImpl is still in use. 

## Control
The application uses the device camera for object recognition. To recognize an object, it simply needs to be captured with the device camera.
The information-screen that is then displayed can be controlled in various ways:
  - By grabbing the redish pane directly and dragging it up and down
  - By pressing the UP/DOWN buttons displayed when an object is recognized
  - (Only when running on an Android device) By also capturing the users hand and performing a pick gesture (scrolling up) or a click gesture (scrolling down).

### Gestures
  - Pick: make sure the camera can cleary see both your thumb and index finger. Press your thumb and index finger together to perform the pick gesture. Release to stop.
  - Click: make sure the camera can cleary see both your thumb and index finger. Shortly touch your thumb and index finger together.

## Troubleshooting
Like stated under the Requirements section, an Android device is needed for the gesture recognition to work. 
Should the gesture recognition still not work at all even then, try the following:
  - Restart the application
  - Change the angle of your hand
  - Change the distance between your hand and the camera
  - Change the location to one with better lighting
  - If possible, use a camera of higher quality
  
Also, keep in mind that covering the recognized object with your hand when trying to perform gesture recognition will lead to the object being "lost".

Should the object recognition not be working, try the following:
  - Make sure the object you wish to be recognized is the same as the one in the application's database. (The QR-cube is recommended for testing for this reason)
  - Change the object's placement/angle (e.g. trying to scan a different side of the cube)
  - Change the distance between the object and the camera
  - Place the object to be scanned in a place with better lighting
  - If possible, use a camera of higher quality