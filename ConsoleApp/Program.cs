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
            var now = DateTime.Now;
            Barcode128 code128 = new Barcode128
            {
                CodeType = Barcode.CODE128,
                ChecksumText = true,
                GenerateChecksum = true,
                StartStopText = true,
                //Code = "045746201627080857"
                Code = $"{now.Year}{now.Month.ToString("00")}{now.Day.ToString("00")}{now.Hour.ToString("00")}{now.Minute.ToString("00")}{now.Second.ToString("00")}{now.Millisecond}"
            };

            var asd = "0457 4620 1627 0808 57";
            var ass = "2019 0422 1549 22 1000";

            FileStream fs = new FileStream("First PDF document.pdf", FileMode.Create);
            Document document = new Document(PageSize.A4, 25, 25, 30, 30);
            PdfWriter writer = PdfWriter.GetInstance(document, fs);

            document.AddAuthor("Vito Donghvani");
            document.AddCreator("Barcode generator");
            document.AddKeywords("PDF Barcode 128");
            document.AddSubject("Barcode generation");
            document.AddTitle("Barcodes");
 
            document.Open();
            document.Add(new Paragraph("Hello World!"));

            PdfContentByte cb = writer.DirectContent;
            var image = code128.CreateImageWithBarcode(cb, null, null);
            document.Add(image);

            document.Close();
            writer.Close();
            fs.Close();
        }
    }
}