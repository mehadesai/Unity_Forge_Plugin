# Unity_Forge_Plugin

This package contains the required files for converting the SVF file into an Unity readable gameobject.
To upload and covert a model into and SVF file refer the heroku app. https://imaginateforge.herokuapp.com/

First the user will have to upload a model in the autodesk cloud bucket (It is currently under my student trial). 
Then the user has to click on translate by right clicking the uploaded model. Wait till the SVF appears in the browser.

  Now in Unity, I have currently implemented a system which accepts Bucketname and file name from the user to generate the URN of the model. This URN is then used to convert the SVF file in autodesk cloud into a gameobject during runtime using the combined.cs script when clicked on the upload button. The URN is then sent to the forge loader object to process and load it into Unity.

  The Uploading process takes 5-15 seconds, while the forgeloader takes time according to the size and no. of meshes the model contains. The slider is included to denote this process.
  
  That's it from User Point of View.
  
How to use the forge plugin,

1. The prefabs in the package has the necessary canvas and gameobjects
2. The Forge folder has the necessary scripts for the running of the forgeloader object.
3. The combined.cs script does 4 API calls to retrieve the SVF file from the bucket and translte it to a Unity scene. 
4. Also please make sure that the forge client id and secret in both script and heroku app is same as mentioned in the forge application created under your account.
