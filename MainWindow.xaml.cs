using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using FellowOakDicom;
using FellowOakDicom.Imaging;
using System.Windows.Media.Imaging;
using EvilDICOM.Core.IO.Reading;
using FellowOakDicom.Imaging.Render;
using EvilDICOM.Core;
using EvilDICOM.Core.Element;
using FellowOakDicom.IO.Buffer;
using EvilDICOM.Core.IO.Data;
using EvilDICOM.Core.Enums;
using EvilDICOM.Core.Interfaces;



namespace DicomWpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void LoadDICOMButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "DICOM Files (*.dcm)|*.dcm|All Files (*.*)|*.*";

            if (openFileDialog.ShowDialog() == true)
            {
                string filePath = openFileDialog.FileName;

                var dicomFile = DicomFile.Open(filePath);
                var dicomImage = new DicomImage(dicomFile.Dataset);

                // Пример конвертации изображения в массив байтов
                byte[] imageBytes = ConvertDicomImageToBytes(dicomImage);

                // Пример обработки изображения с использованием вашей нейросети
                var processedResult = NeuralNetwork.ProcessImage(imageBytes);

                // Отображение обработанного изображения
                DisplayImage(imageBytes);

                // Вывод результата
                resultText.Text = "Result: " + processedResult.ToString();
            }
        }




        private void DisplayImage(byte[] imageBytes)
        {
            BitmapImage bitmap = new BitmapImage();
            bitmap.BeginInit();
            bitmap.CacheOption = BitmapCacheOption.OnLoad;
            bitmap.StreamSource = new MemoryStream(imageBytes);
            bitmap.EndInit();

            dicomImage.Source = bitmap;
        }
        private byte[] ConvertDicomImageToBytes(DicomImage dicomImage)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Получаем объект DicomPixelData изображения
                var dicomPixelData = dicomImage.RenderImage().As<DicomPixelData>();

                // Получаем значения пикселей в виде массива байтов
                var byteArray = dicomPixelData.GetFrame(0).Data;

                // Записываем массив байтов в поток
                memoryStream.Write(byteArray, 0, byteArray.Length);

                return memoryStream.ToArray();
            }
        }

      
    }
}
