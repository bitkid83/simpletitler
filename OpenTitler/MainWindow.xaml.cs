using System;
using System.IO;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Forms;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Xceed.Wpf.Toolkit;



namespace OpenTitler
{

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private int textLayerCounter = 0;
        private static string outputFilename = "title.png";
        private ObservableCollection<string> layerList = new ObservableCollection<string> { };
        private ObservableCollection<System.Windows.Controls.TextBox> textboxCollection = new ObservableCollection<System.Windows.Controls.TextBox> { };

        public MainWindow()
        {
            InitializeComponent();            
        }

        private void UpdateZOrder()
        {
            /*
            for (int i = textboxCollection.Count - 1; i >= 0; i--)
            {
                Canvas.SetZIndex(textboxCollection[i], i);
            }
            */
            int ZOrder = textboxCollection.Count;

            for (int i = 0; i < textboxCollection.Count; i++)
            {
                Canvas.SetZIndex(textboxCollection[i], ZOrder);
                ZOrder--;
            }
        }


        //Manually save the contents of the primaryCanvas to a PNG image
        private void saveImageButton_Click(object sender, RoutedEventArgs e)
        {
            primaryCanvas.Background.Opacity = 0x00;
            //render canvas contents to bitmap data
            Rect rect = new Rect( primaryCanvas.Margin.Left, 
                                  primaryCanvas.Margin.Top, 
                                  primaryCanvas.ActualWidth, 
                                  primaryCanvas.ActualHeight ); //Get actual dimensions of Canvas, irrespective of placement in WPF grid or window

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)rect.Right, (int)rect.Bottom, 96d, 96d, PixelFormats.Default);
            rtb.Render(primaryCanvas);

            //encode bitmap data as PNG
            BitmapEncoder pngEncoder = new PngBitmapEncoder();
            pngEncoder.Frames.Add(BitmapFrame.Create(rtb));

            //save to memory stream
            MemoryStream ms = new MemoryStream();
            pngEncoder.Save(ms);
            ms.Close();
            
            Byte[] pngdata = ms.GetBuffer();
            using (FileStream fs = new FileStream(outputFilename, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.ReadWrite))
            {
                fs.Write(pngdata, 0, pngdata.Length);
                fs.Close();
            }
            //File.WriteAllBytes("titleimage.png", ms.ToArray());     
            primaryCanvas.Background.Opacity = 0xFF;
        }

        private void addTextButton_Click(object sender, RoutedEventArgs e)
        {
            //Add a new TextBox object to the collection
            textboxCollection.Insert(0, new System.Windows.Controls.TextBox());
            
            //Set the name of the new TextBox object
            textboxCollection[0].Name = "textLayer" + textLayerCounter;            



            //Set the size of the TextBox
            textboxCollection[0].Width = primaryCanvas.ActualWidth;
            textboxCollection[0].Height = primaryCanvas.ActualHeight;

            //Set the default style for this TextBox
            SolidColorBrush transparentBrush = new SolidColorBrush(System.Windows.Media.Color.FromArgb(0x00, 0x00, 0x00, 0x00));
            
            textboxCollection[0].FontSize = 64;
            textboxCollection[0].Background = transparentBrush;
            textboxCollection[0].BorderBrush = transparentBrush;

            //Add new TextBox as a child of the Canvas
            //primaryCanvas.Children.Insert(0, textboxCollection[0]);
            primaryCanvas.Children.Add(textboxCollection[0]);

            //Set initial position in the Canvas (optional)
            //InkCanvas.SetLeft(tb, 25);
            //InkCanvas.SetRight(tb, 25);
            
            //Set the name of this TextBox in the Layer list          
            //layerList.Add("New Text Layer " + textLayerCounter.ToString()); //add new text layer's name to the ObservableCollection<string> object
            layerList.Insert(0, "New Text Layer " + textLayerCounter.ToString());
            //Update the List Box with the names of objects in the Layer List
            layerListBox.ItemsSource = layerList; 

            //Place new TextBox on top in the Z-Order
            Canvas.SetZIndex(textboxCollection[0], textboxCollection.Count);

            textLayerCounter++;
        }

        //Move Layers up in the list/z-order
        private void moveupButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = layerListBox.SelectedIndex;

            if (selectedIndex > 0)
            {

                var moveItem = layerList[selectedIndex];
                var moveTb = textboxCollection[selectedIndex];

                //Re-order the list box contents
                layerList.RemoveAt(selectedIndex);
                layerList.Insert(selectedIndex - 1, moveItem);
                layerListBox.SelectedIndex = selectedIndex - 1;

                //Re-order the TextBox Layers
                textboxCollection.RemoveAt(selectedIndex);
                textboxCollection.Insert(selectedIndex - 1, moveTb);

                UpdateZOrder();
            }
        }

        //Move Layers down in the list/z-order
        private void movedownButton_Click(object sender, RoutedEventArgs e)
        {
            int selectedIndex = layerListBox.SelectedIndex;

            if (selectedIndex + 1 < layerList.Count)
            {
                var moveItem = layerList[selectedIndex];
                var moveTb = textboxCollection[selectedIndex];

                layerList.RemoveAt(selectedIndex);
                layerList.Insert(selectedIndex + 1, moveItem);
                layerListBox.SelectedIndex = selectedIndex + 1;

                textboxCollection.RemoveAt(selectedIndex);
                textboxCollection.Insert(selectedIndex + 1, moveTb);

                UpdateZOrder();           
            }
        }

        //I've solved this in XAML so this is not needed at this time
        private void SetTransparentBG()
        {
            ImageBrush brush = new ImageBrush();
            brush.ImageSource = new BitmapImage(new Uri(@"c:\users\paulh\documents\visual studio 2012\Projects\OpenTitler\OpenTitler\Images\transparentbg.png"));
            brush.TileMode = TileMode.Tile;
            brush.ViewportUnits = BrushMappingMode.Absolute;
            brush.Viewport = new Rect(0, 0, brush.ImageSource.Width, brush.ImageSource.Height);

            primaryCanvas.Background = brush;
        }

        private void fontsButton_Click(object sender, RoutedEventArgs e)
        {
            if (layerListBox.SelectedIndex > -1)
            {
                FontDialog fd = new FontDialog();
                System.Windows.Forms.DialogResult dr = fd.ShowDialog();
                if (dr != System.Windows.Forms.DialogResult.Cancel)
                {
                    textboxCollection[layerListBox.SelectedIndex].FontFamily = new System.Windows.Media.FontFamily(fd.Font.Name);
                    textboxCollection[layerListBox.SelectedIndex].FontSize = fd.Font.Size * 96.0 / 72.0;
                    textboxCollection[layerListBox.SelectedIndex].FontWeight = fd.Font.Bold ? FontWeights.Bold : FontWeights.Regular;
                    textboxCollection[layerListBox.SelectedIndex].FontStyle = fd.Font.Italic ? FontStyles.Italic : FontStyles.Normal;
                }
            }
            else
            {
            }
        }

        private void fontColorPicker_SelectedColorChanged(object sender, RoutedEventArgs e)
        {
            if (layerListBox.SelectedIndex > -1)
            {
                SolidColorBrush foregroundColor = new SolidColorBrush(fontColorPicker.SelectedColor);
                textboxCollection[layerListBox.SelectedIndex].Foreground = foregroundColor;
            }
            else
            {
            }
        }

        private void xSlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            Canvas.SetLeft(textboxCollection[layerListBox.SelectedIndex], xPosSlider.Value);
        }

        private void ySlider_ValueChanged(object sender, RoutedEventArgs e)
        {
            Canvas.SetTop(textboxCollection[layerListBox.SelectedIndex], yPosSlider.Value);
        }
    }
}
