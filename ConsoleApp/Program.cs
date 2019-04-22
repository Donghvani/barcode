using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace ConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var numberOfColumns = 8;
            var numberOfBarcodes = 256;
            
            GenerateBarcodes(numberOfColumns, numberOfBarcodes);
        }

        private static void GenerateBarcodes(int numberOfColumns, int numberOfBarcodes)
        {
            var fs = new FileStream("First PDF document.pdf", FileMode.Create);
            var document = new Document(PageSize.A4, 0, 0, 0, 0);
            var writer = PdfWriter.GetInstance(document, fs);

            document.AddAuthor("Vito Donghvani");
            document.AddCreator("Barcode generator");
            document.AddKeywords("PDF Barcode 128");
            document.AddSubject("Barcode generation");
            document.AddTitle("Barcodes");

            document.Open();
                        
            var pdfTable = new PdfPTable(numberOfColumns)
            {
                HorizontalAlignment = Element.ALIGN_LEFT                
            };
            var columnWidth = PageSize.A4.Width / numberOfColumns;           
            var asd = Enumerable.Repeat(columnWidth, numberOfColumns).ToArray();
            
            pdfTable.SetTotalWidth(asd);
            pdfTable.LockedWidth = true;

            foreach (var image in GetBarcodeImages(writer.DirectContent, numberOfBarcodes))
            {             
                pdfTable.AddCell(image);
            }

            document.Add(pdfTable);
            document.Close();
            writer.Close();
            fs.Close();
        }

        private static Image GetBarcodeImage(PdfContentByte pdfContentByte)
        {
            var now = DateTime.Now;
            var code = 
                $"{now.Year}{now.Month:00}{now.Day:00}{now.Hour:00}{now.Minute:00}{now.Second:00}{now.Millisecond:0000}";

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