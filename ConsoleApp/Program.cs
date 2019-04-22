using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            TestMethod();
        }

        private static void TestMethod()
        {
            FileStream fs = new FileStream("First PDF document.pdf", FileMode.Create);
            Document document = new Document(PageSize.A4, 5, 5, 5, 5);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);

            document.AddAuthor("Vito Donghvani");
            document.AddCreator("Barcode generator");
            document.AddKeywords("PDF Barcode 128");
            document.AddSubject("Barcode generation");
            document.AddTitle("Barcodes");

            document.Open();
            foreach (var image in GetBarcodeImages(writer.DirectContent, 24))
            {
                document.Add(image);
            }

            document.Close();
            writer.Close();
            fs.Close();
        }

        private static Image GetBarcodeImage(PdfContentByte pdfContentByte)
        {
            var now = DateTime.Now;
            var code = $"{now.Year}{now.Month.ToString("00")}{now.Day.ToString("00")}{now.Hour.ToString("00")}{now.Minute.ToString("00")}{now.Second.ToString("00")}{now.Millisecond.ToString("0000")}";

            var code128 = new Barcode128
            {
                CodeType = Barcode.CODE128,
                ChecksumText = true,
                GenerateChecksum = true,
                StartStopText = true,
                Code = code
            };

            return code128.CreateImageWithBarcode(pdfContentByte, null, null);
        }

        private static List<Image> GetBarcodeImages(PdfContentByte pdfContentByte, int count)
        {
            var images = new List<Image>();
            for (var index = 0; index < count; index++)
            {
                images.Add(GetBarcodeImage(pdfContentByte));
                Thread.Sleep(1);
            }
            return images;
        }
    }
}