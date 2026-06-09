using System;
using System.IO;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

class Program
{
    static void Main(string[] args)
    {
        // Default paths relative to the executable or output folder
        string srcPath = args.Length > 0 ? args[0] : "../../../Apex_Digital_QTG-2606-001_Quotation.pdf";
        string destPath = args.Length > 1 ? args[1] : "../../../Apex_Digital_QTG-2606-001_Quotation_temp.pdf";

        if (!File.Exists(srcPath))
        {
            Console.WriteLine($"Error: Source PDF file not found at {Path.GetFullPath(srcPath)}");
            Console.WriteLine("Usage: pdf-updater-tool [source_pdf_path] [destination_pdf_path]");
            return;
        }

        Console.WriteLine($"Opening source PDF: {Path.GetFullPath(srcPath)}");
        Console.WriteLine($"Writing to destination PDF: {Path.GetFullPath(destPath)}");

        try
        {
            using (var pdfReader = new PdfReader(srcPath))
            using (var pdfWriter = new PdfWriter(destPath))
            using (var pdfDoc = new PdfDocument(pdfReader, pdfWriter))
            {
                // Load Helvetica for the quotation metadata table and headers
                var fontLabel = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                
                // Load Arial and Microsoft YaHei for Page 4
                var fontRegular = PdfFontFactory.CreateFont("C:\\Windows\\Fonts\\arial.ttf", iText.IO.Font.PdfEncodings.IDENTITY_H);
                var fontBold = PdfFontFactory.CreateFont("C:\\Windows\\Fonts\\arialbd.ttf", iText.IO.Font.PdfEncodings.IDENTITY_H);
                var fontChinese = PdfFontFactory.CreateFont("C:\\Windows\\Fonts\\msyh.ttc,0", iText.IO.Font.PdfEncodings.IDENTITY_H);

                string newQuoteNo = "QTG-2606-001";

                // --- UPDATE HEADERS ON ALL PAGES ---
                Console.WriteLine("Updating page headers...");
                for (int pageNum = 1; pageNum <= pdfDoc.GetNumberOfPages(); pageNum++)
                {
                    var page = pdfDoc.GetPage(pageNum);
                    var canvas = new PdfCanvas(page);

                    // 1. Cover the original header quote number at the top
                    // X_PDF: 91.574 to 176.015 -> Width: 86
                    // Y_PDF: 806.989 to 812.547 -> Y_PDF: 804, Height: 10
                    canvas.SaveState()
                          .SetFillColor(ColorConstants.WHITE)
                          .Rectangle(91f, 804f, 86f, 10f)
                          .Fill()
                          .RestoreState();

                    // 2. Cover the mispositioned header quote number at the bottom from the previous run
                    // Y_PDF: 30 to 42
                    canvas.SaveState()
                          .SetFillColor(ColorConstants.WHITE)
                          .Rectangle(90f, 30f, 90f, 12f)
                          .Fill()
                          .RestoreState();

                    // Draw new quote number in the header (at the top)
                    canvas.BeginText()
                          .SetFontAndSize(fontLabel, 7.5f)
                          .SetFillColor(new DeviceRgb(115, 115, 115))
                          .MoveText(91.574, 806.989)
                          .ShowText(newQuoteNo)
                          .EndText();
                }

                // --- PAGE 1: Update Quote No in the Details Table ---
                Console.WriteLine("Updating Page 1 Quote No table value...");
                var page1 = pdfDoc.GetPage(1);
                var canvas1 = new PdfCanvas(page1);

                // Calculate right-aligned X coordinate for newQuoteNo (ends at X = 524.424f)
                float valueWidth = fontLabel.GetWidth(newQuoteNo, 8.5f);
                float valueLeftX = 524.424f - valueWidth;
                Console.WriteLine($"Quote No Left X: {valueLeftX:F3} (Width: {valueWidth:F3}pt)");

                // Cover the old Quote No value in the table
                // X_PDF: 430 to 526, Y_PDF: 702 to 713 (Height: 11)
                canvas1.SaveState()
                       .SetFillColor(ColorConstants.WHITE)
                       .Rectangle(430f, 702f, 96f, 11f)
                       .Fill()
                       .RestoreState();

                // Draw new Quote No value (right-aligned) using dark near-black color (#171717)
                canvas1.BeginText()
                       .SetFontAndSize(fontLabel, 8.5f)
                       .SetFillColor(new DeviceRgb(23, 23, 23))
                       .MoveText(valueLeftX, 704.431)
                       .ShowText(newQuoteNo)
                       .EndText();

                // --- PAGE 1: Update "Date:" to "Date of Issue:" (Right-aligned) ---
                Console.WriteLine("Updating Page 1 Date label...");
                string labelText = "Date of Issue:";
                float labelWidth = fontLabel.GetWidth(labelText, 8.5f);
                float labelLeftX = 397.288f - labelWidth;

                // Cover original Date label and any previous misaligned text
                canvas1.SaveState()
                       .SetFillColor(ColorConstants.WHITE)
                       .Rectangle(340f, 684f, 130f, 18f)
                       .Fill()
                       .RestoreState();

                canvas1.BeginText()
                       .SetFontAndSize(fontLabel, 8.5f)
                       .SetFillColor(new DeviceRgb(115, 115, 115))
                       .MoveText(labelLeftX, 686.33075)
                       .ShowText(labelText)
                       .EndText();

                // --- PAGE 4: Update Payment Terms ---
                Console.WriteLine("Updating Page 4 Payment Terms...");
                var page4 = pdfDoc.GetPage(4);
                var canvas4 = new PdfCanvas(page4);

                // Cover bottom mispositioned text
                canvas4.SaveState()
                       .SetFillColor(ColorConstants.WHITE)
                       .Rectangle(85f, 146f, 445f, 49f)
                       .Fill()
                       .RestoreState();

                // Cover original text at the top
                canvas4.SaveState()
                       .SetFillColor(ColorConstants.WHITE)
                       .Rectangle(85f, 648f, 445f, 48f)
                       .Fill()
                       .RestoreState();

                float boldWidth = fontBold.GetWidth("Payment Terms:", 9f);
                
                // Line 1:
                canvas4.BeginText()
                       .SetFontAndSize(fontBold, 9f)
                       .SetFillColor(new DeviceRgb(82, 82, 82))
                       .MoveText(88.866, 680.8463)
                       .ShowText("Payment Terms:")
                       .SetFontAndSize(fontRegular, 9f)
                       .MoveText(boldWidth, 0)
                       .ShowText(" A 50% non-refundable deposit is required to commence work. The remaining 50% balance")
                       .EndText();

                // Line 2:
                canvas4.BeginText()
                       .SetFontAndSize(fontRegular, 9f)
                       .SetFillColor(new DeviceRgb(82, 82, 82))
                       .MoveText(88.866, 666.2363)
                       .ShowText("is due upon satisfied with demo shared.")
                       .EndText();

                // Line 3 (Chinese):
                canvas4.BeginText()
                       .SetFontAndSize(fontChinese, 8.5f)
                       .SetFillColor(new DeviceRgb(163, 163, 163))
                       .MoveText(93.366, 651.6263)
                       .ShowText("(付款方式：需支付 50% 不可退还的首期订金以启动项目，剩余 50% 尾款于客户对演示版满意后支付。)")
                       .EndText();
            }

            Console.WriteLine("PDF updated successfully!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating PDF: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
        }
    }
}
