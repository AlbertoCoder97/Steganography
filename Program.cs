using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Steganography
{
    class Program
    {
        static void Main(string[] args)
        {
            //First of all let's make a copy of the original image.
            //You must have a file called "original.bmp" in your directory
            if(!File.Exists("copy.bmp"))
                File.Copy("original.bmp", "copy.bmp");

            if(File.Exists("crypted.bmp"))
            {
                Decrypt(new Bitmap("crypted.bmp"));
                return;
            }
                
            //Let's load the file:
            Bitmap copy = new Bitmap("copy.bmp");
            //We need to calculate how many bits of infos we can store per image (based on size)
            var textsize = (8.0 * ((copy.Height * (copy.Width / 3) * 3) / 3 - 1)) / 1024;
            
            
            /*
            *Let's start all the Steganography processes.
            *First of all we need to hide bytes of messages into each pixel.
            *We can hide 3 or 6 bits of information inside each pixel.
            *1 or 2 bits per RGB channel.
            *We'll be using the LSB Algorithm. 
            */
            Console.WriteLine("Write text to hide and press enter:");
            String toHide = Console.ReadLine();
            //This will give us the text length used for later purposes
            double textLength = (System.Text.ASCIIEncoding.ASCII.GetByteCount(toHide)) / 1024;
            if (textsize<textLength)
            {
                Console.WriteLine("Text too long!");
            }else{
                //Let's encode the message.
                for(int i = 0; i < copy.Width; i++)
                {
                    for(int j = 0; j < copy.Height; j++)
                    {
                        //Let's get the pixel color
                        Color pixel = copy.GetPixel(i,j);

                        //Now we need to make some checks:
                        //We need to check if we are at the end of the file or we are hiding a letter.
                        if(i < 1 && j < toHide.Length)
                        {
                            
                            //Let's get the char we are hiding.
                            char letter = Convert.ToChar(toHide.Substring(j,1));
                            int value = Convert.ToInt32(letter);

                            //Let's set the new color
                            copy.SetPixel(i,j,Color.FromArgb(pixel.R, pixel.G, value));
                        }
                        if (i == copy.Width - 1 && j == copy.Height - 1)
                        {
                            //For decryption purposes we write how long is the text length
                            copy.SetPixel(i, j, Color.FromArgb(pixel.R, pixel.G, toHide.Length));
                        }
                    }
                }

                //Let's save the file.
                copy.Save("crypted.bmp");
                Console.WriteLine("Image saved!");
            }

            Decrypt(new Bitmap("crypted.bmp"));
            Console.WriteLine("Press enter to end the application!");
            Console.ReadLine();
        }

        public static void Decrypt(Bitmap image)
        {
            //Let's get the message length first.
            int msgLength = image.GetPixel(image.Width - 1, image.Height - 1).B;
            String message = "";
            //Let's decrypt the message!
            for(int i = 0; i < image.Width; i++)
            {
                for(int j = 0; j < image.Height; j++)
                {
                    Color pixel = image.GetPixel(i,j);
                    //Let's get the LSB from the pixels
                    if (i < 1 && j < msgLength)
                    {
                        char c = Convert.ToChar(pixel.B);

                        //let's convert everything to bytes array first and then to string and add it to the message;
                        String letter = System.Text.Encoding.ASCII.GetString(BitConverter.GetBytes(c));
                        message += letter;
                    }
                }
            }
            Console.WriteLine("Decrypted message: {0}", message);
        }
    }
}
