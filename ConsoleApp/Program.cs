using System;
using System.IO;
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
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);

            document.AddAuthor("Vito Donghvani");
            document.AddCreator("Barcode generator");
            document.AddKeywords("PDF Barcode 128");
            document.AddSubject("Barcode generation");
            document.AddTitle("Barcodes");
 
            document.Open();
            document.Add(GetBarcodeImage(writer.DirectContent));

            document.Close();
            writer.Close();
            fs.Close();
        }

        private static Image GetBarcodeImage(PdfContentByte pdfContentByte)
        {
            var now = DateTime.Now;
            var code = $"{now.Year}{now.Month.ToString("00")}{now.Day.ToString("00")}{now.Hour.ToString("00")}{now.Minute.ToString("00")}{now.Second.ToString("00")}{now.Millisecond}";

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
    }
}