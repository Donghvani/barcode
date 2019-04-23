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
            //args = new[] {"barcodes.pdf", "6"};
            
            if (args.Length == 0) return;
            
            var fileName = args[0];
            int.TryParse(args[1], out var numberOfColumns);
            
            var barcodes = GenerateBarcodes(fileName, numberOfColumns);
            Console.WriteLine($"Generated: {barcodes}");
        }

        private static int GenerateBarcodes(string fileName, int numberOfColumns)
        {
            var result = 0;
            using (var fs = new FileStream(fileName, FileMode.Create))
            {                
                var document = new Document(PageSize.A4, 0, 0, 0, 0);                            
                var writer = PdfWriter.GetInstance(document, fs);

                //TODO: Move out
                document.AddAuthor("Vito Donghvani");
                document.AddCreator("Barcode generator");
                document.AddKeywords("PDF Barcode 128");
                document.AddSubject("Barcode generation");
                document.AddTitle("Barcodes");

                document.Open();
                
                var pdfTable = new PdfPTable(numberOfColumns)
                {
                    HorizontalAlignment = Element.ALIGN_LEFT,
                    TotalWidth = PageSize.A4.Width, 
                    LockedWidth = true
                };

                var tableTotalHeight = 0f;
                var difference = 0f;
                
                do
                {
                    result++;
                    var image = GetBarcodeImage(writer.DirectContent);
                    //Console.WriteLine($"{pdfTable.TotalHeight} {PageSize.A4.Height}");
                    pdfTable.AddCell(image);

                    difference = pdfTable.TotalHeight - tableTotalHeight;
                    tableTotalHeight = pdfTable.TotalHeight;
                    Thread.Sleep(1);
                } while (pdfTable.TotalHeight + difference <= PageSize.A4.Height);                          

                Console.WriteLine($"Required row height = {difference}; Last row height = {PageSize.A4.Height - pdfTable.TotalHeight}");
                
                document.Add(pdfTable);
                document.Close();
                writer.Close();
            }

            return result;
        }

        private static Image GetBarcodeImage(PdfContentByte pdfContentByte)
        {
            var now = DateTime.Now;
            var code = 
                $"{now:yy}{now.Month:00}{now.Day:00}{now.Hour:00}{now.Minute:00}{now.Second:00}{now.Millisecond:0000}";

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