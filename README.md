# AR_Draw #

## Demo ##
[Video](https://youtu.be/u5I_b45NQmg)

## Augmented reality drawing with Unity3D ##
The principle is to display a model colored exactly like the drawing scanned by a smartphone.
<pre>1st step
Print the image of the desired animal and color this.
Folder : Assets/Ressources</pre>
<pre>2nd step
Build and run the application on your phone</pre>
<pre>3rd step
Enjoy it !</pre>

The different models are divided into 5 parts that will be colored independently.  
Vuforia is used to display a model.  
Texture region-capture is a library allowing to straighten the image.

### WARNING ! ###
If you run the application on android phone you will have the wrong colors on the animal model. The library used to straighten the image doesn't work on the latest version (2018.2.18f1) of Unity3D.  
Download the old version of Unity3D (2018.3.0f2) and Vuforia (7.5.26) works perfectly.
