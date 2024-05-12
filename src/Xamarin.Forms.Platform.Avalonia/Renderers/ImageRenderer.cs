﻿using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Xamarin.Forms.Internals;
using Xamarin.Forms.Platform.Avalonia;

namespace Xamarin.Forms.Platform.Avalonia
{
    public class ImageRenderer : ViewRenderer<Image, global::Avalonia.Controls.Image>
    {
        protected override async void OnElementChanged(ElementChangedEventArgs<Image> e)
        {
            if (e.NewElement != null)
            {
                if (Control == null) // construct and SetNativeControl and suscribe control event
                {
                    SetNativeControl(new global::Avalonia.Controls.Image());
                }

                // Update control property 
                await TryUpdateSource();
                UpdateAspect();
            }

            base.OnElementChanged(e);
        }

        protected override async void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == Image.SourceProperty.PropertyName)
                await TryUpdateSource();
            else if (e.PropertyName == Image.AspectProperty.PropertyName)
                UpdateAspect();
        }

        void UpdateAspect()
        {
            Control.Stretch = Element.Aspect.ToStretch();
            if (Element.Aspect == Aspect.AspectFill) // Then Center Crop
            {
                Control.HorizontalAlignment = HorizontalAlignment.Center;
                Control.VerticalAlignment = VerticalAlignment.Center;
            }
            else // Default
            {
                Control.HorizontalAlignment = HorizontalAlignment.Left;
                Control.VerticalAlignment = VerticalAlignment.Top;
            }
        }

        protected virtual async Task TryUpdateSource()
        {
            try
            {
                await UpdateSource().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Log.Warning(nameof(ImageRenderer), "Error loading image: {0}", ex);
            }
            finally
            {
                Element.SetIsLoading(false);
            }
        }

        protected async Task UpdateSource()
        {
            if (Element == null || Control == null)
            {
                return;
            }

            var source = Element.Source;

            Element.SetIsLoading(true);
            try
            {
                var imagesource = await source.ToNativeImageSourceAsync();

                // only set if we are still on the same image
                if (Control != null && Element.Source == source)
                {
                    Control.Source = imagesource;
                }
            }
            finally
            {
                // only mark as finished if we are still on the same image
                if (Element.Source == source)
                {
                    Element.SetIsLoading(false);
                }
            }

            ((IVisualElementController)Element)?.InvalidateMeasure(InvalidationTrigger.RendererReady);
        }
    }

    public interface IImageSourceHandler : IRegisterable
    {
        Task<global::Avalonia.Media.Imaging.Bitmap> LoadImageAsync(ImageSource imagesoure, CancellationToken cancelationToken = default(CancellationToken));
    }

    public sealed class FileImageSourceHandler : IImageSourceHandler
    {
        public Task<global::Avalonia.Media.Imaging.Bitmap> LoadImageAsync(ImageSource imagesoure, CancellationToken cancelationToken = new CancellationToken())
        {
            global::Avalonia.Media.Imaging.Bitmap image = null;
            FileImageSource filesource = imagesoure as FileImageSource;
            if (filesource != null)
            {
                string file = filesource.File;
                image = new Bitmap(file);
            }
            return Task.FromResult(image);
        }
    }

    public sealed class StreamImageSourceHandler : IImageSourceHandler
    {
        public async Task<global::Avalonia.Media.Imaging.Bitmap> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = new CancellationToken())
        {
            Bitmap bitmapImage = null;
            StreamImageSource streamImageSource = imagesource as StreamImageSource;

            if (streamImageSource != null && streamImageSource.Stream != null)
            {
                using (Stream stream = await ((IStreamImageSource)streamImageSource).GetStreamAsync(cancelationToken))
                {
                    bitmapImage = new Bitmap(stream);
                }
            }

            return bitmapImage;
        }
    }

    public sealed class UriImageSourceHandler : IImageSourceHandler
    {
        public Task<global::Avalonia.Media.Imaging.Bitmap> LoadImageAsync(ImageSource imagesoure, CancellationToken cancelationToken = new CancellationToken())
        {
            Bitmap bitmapimage = null;
            var imageLoader = imagesoure as UriImageSource;
            if (imageLoader?.Uri != null)
            {
                var webRequest = (HttpWebRequest)WebRequest.Create(imageLoader?.Uri);
                var responseStream = webRequest.GetResponse().GetResponseStream();
                byte[] buffer = new byte[1024];
                var memoryStream = new MemoryStream();
                while (true)
                {
                    int read = responseStream.Read(buffer, 0, buffer.Length);
                    if (read <= 0) break;
                    memoryStream.Write(buffer, 0, read);
                }
                memoryStream.Position = 0;
                bitmapimage = new Bitmap(memoryStream);
            }
            return Task.FromResult<Bitmap>(bitmapimage);
        }
    }

    public sealed class FontImageSourceHandler : IImageSourceHandler
    {
        public Task<global::Avalonia.Media.Imaging.Bitmap> LoadImageAsync(ImageSource imagesource, CancellationToken cancelationToken = new CancellationToken())
        {
            var fontsource = imagesource as FontImageSource;
            var image = CreateGlyph(
                    fontsource.Glyph,
                    new FontFamily(new Uri("pack://application:,,,"), fontsource.FontFamily),
                    FontStyle.Normal,
                    FontWeight.Normal,
                    fontsource.Size,
                    (fontsource.Color != Color.Default ? fontsource.Color : Color.White).ToBrush());
            return Task.FromResult(image);
        }

        static global::Avalonia.Media.Imaging.Bitmap CreateGlyph(
            string text,
            FontFamily fontFamily,
            FontStyle fontStyle,
            FontWeight fontWeight,
            //FontStretch fontStretch,
            double fontSize,
            Brush foreBrush)
        {
            if (fontFamily == null || string.IsNullOrEmpty(text))
            {
                return null;
            }
            //var typeface = new Typeface(fontFamily, fontSize, fontStyle, fontWeight);
            //if (!typeface.TryGetGlyphTypeface(out GlyphTypeface glyphTypeface))
            //{
            //    //if it does not work 
            //    return null;
            //}

            //var glyphIndexes = new ushort[text.Length];
            //var advanceWidths = new double[text.Length];
            //for (int n = 0; n < text.Length; n++)
            //{
            //    var glyphIndex = glyphTypeface.CharacterToGlyphMap[text[n]];
            //    glyphIndexes[n] = glyphIndex;
            //    var width = glyphTypeface.AdvanceWidths[glyphIndex] * 1.0;
            //    advanceWidths[n] = width;
            //}

            //var gr = new GlyphRun(glyphTypeface,
            //    0, false,
            //    fontSize,
            //    glyphIndexes,
            //    new global::Avalonia.Point(0, 0),
            //    advanceWidths,
            //    null, null, null, null, null, null);
            //var glyphRunDrawing = new GlyphRunDrawing(foreBrush, gr);
            //return new DrawingImage(glyphRunDrawing);
            // TODO:
            return null;
        }
    }
}