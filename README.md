# PixelMap
High performance bitmap with a focus on pixel-level editing. No more lock bits.

## Why use PixelMap?

When manipulating a Bitmap at the pixel level, the GetPixel and SetPixel approach is simply lackluster.
Apart from the ugly Java-like syntax, these methods are magnitudes slower than they should be due to the layers of extra complexity contained within GDI.

Many solutions to this issue go through the route of low level memory management ('lockbits'), an ugly route that requires unsafe code to be written.
Whilst this offers the speed desired, it also adds unnecessary complexity to your code.

PixelMap solves this issue by replacing the Bitmap class entirely, offering a sleeker and faster approach to image manipulation.
When an image needs to be converted to or from GDI's Bitmap, the low level memory management is handled out of sight safely.

Color is handled by the Pixel class, solving GDI's HSL issues such as an inability to create colors from the HSL
colorspace despite the inverse operation being easily possible.

## By Example

### Changing Hue

This simple example loads and saves an image, modifying the hue of the pixels.

    //Quickly load a PixelMap through a Bitmap
    PixelMap map = new PixelMap(new Bitmap("Lenna.png"));
    
    for (int x = 0; x < map.Width; x++)
    {
      for (int y = 0; y < map.Height; y++)
      {
        //Sample a pixel
        Pixel pixel = map[x, y];
        
        //Create a hue value
        float value = ((float)x/map.Width)*360f;
      
        //Set the hue value to our sample
        pixel.Hue = value;
      
        //Return our sample to the PixelMap
        map[x, y] = pixel;
      }
    }
    //Save the PixelMap through a Bitmap
    map.GetBitmap().Save("output.png");
