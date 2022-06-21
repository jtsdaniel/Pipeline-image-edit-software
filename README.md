# Simple-Pipeline-image-edit-software

Description: This application is an assignment from QUT about developling an image edit software. The software's input includes 2 main parts which are **an image.png** and **a simple text file** with instruction lines to edit the image. The application also takes some approaches of using parallel computing to improve image processing time - [click here for more details](https://github.com/jtsdaniel/Pipeline-image-edit-software/blob/main/Report%20about%20app's%20optimisation.pdf)

Here's the example contents of the edit instruction text file:

```
node=resize newSize=4000x2200
node=grayscale
node=add_border borderSize=100 borderColor=100,40,30
node=sepia
node=vignette
node=normalise
node=flip direction=[vertical]
```

## Installation guide

## User guide
