# Simple-Pipeline-image-edit-software

***Description:*** This application is an assignment from QUT about developing an image edit software. The software's input includes 2 main parts which are **an image.png** and **a simple text file** with instruction lines to edit the image. 

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

![](https://github.com/jtsdaniel/Pipeline-image-edit-software/blob/main/FilterPipeApp/src/example%20image/demo.png?raw=true)

The application also takes some approaches of using parallel computing to improve image processing time - [click here for more details](https://github.com/jtsdaniel/Pipeline-image-edit-software/blob/main/Report%20about%20app's%20optimisation.pdf)


## Installation guide

Clone the repo in your local machine

```bash
git clone https://github.com/jtsdaniel/Pipeline-image-edit-software.git
```
## User guide

1. To run the application, simply follow this path: *"...\Pipeline-image-edit-software\FilterPipeApp\src"* then enter *"dotnet run"* command

```bash
dotnet run
```
2. To change the image input or instruction lines for image edit - please follow this [guide](https://github.com/jtsdaniel/Pipeline-image-edit-software/blob/main/How%20to%20Run_FilterPipeApp.pdf)
