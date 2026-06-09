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
        string srcPath = args.Length > 0 ? args[0] : "../../../Apex_Digital_Quotation_Lead_Gen_v8.pdf";
        string destPath = args.Length > 1 ? args[1] : "../../../Apex_Digital_Quotation_Lead_Gen_v8_updated.pdf";

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
                // Load fonts from Windows system Fonts folder
                var fontLabel = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);
                var fontRegular = PdfFontFactory.CreateFont("C:\\Windows\\Fonts\\arial.ttf", iText.IO.Font.PdfEncodings.IDENTITY_H);
                var fontBold = PdfFontFactory.CreateFont("C:\\Windows\\Fonts\\arialbd.ttf", iText.IO.Font.PdfEncodings.IDENTITY_H);
                var fontChinese = PdfFontFactory.CreateFont("C:\\Windows\\Fonts\\msyh.ttc,0", iText.IO.Font.PdfEncodings.IDENTITY_H);

                // --- PAGE 1: Update "Date:" to "Date of Issue:" ---
                Console.WriteLine("Updating Page 1...");
                var page1 = pdfDoc.GetPage(1);
                var canvas1 = new PdfCanvas(page1);

                // Calculate the right-aligned X coordinate for "Date of Issue:" (ends at X = 397.288f)
                string labelText = "Date of Issue:";
                float labelWidth = fontLabel.GetWidth(labelText, 8.5f);
                float labelLeftX = 397.288f - labelWidth;

                // Cover the old text "Date:" AND any previous misaligned drawings
                canvas1.SaveState()
                       .SetFillColor(ColorConstants.WHITE)
                       .Rectangle(340f, 684f, 130f, 18f)
                       .Fill()
                       .RestoreState();

                // Draw new text "Date of Issue:" (right-aligned) using standard Helvetica
                canvas1.BeginText()
                       .SetFontAndSize(fontLabel, 8.5f)
                       .SetFillColor(new DeviceRgb(115, 115, 115))
                       .MoveText(labelLeftX, 686.33075)
                       .ShowText(labelText)
                       .EndText();

                // --- PAGE 4: Update Payment Terms ---
                Console.WriteLine("Updating Page 4...");
                var page4 = pdfDoc.GetPage(4);
                var canvas4 = new PdfCanvas(page4);

                // 1. Cover any mispositioned new payment terms at the bottom
                canvas4.SaveState()
                       .SetFillColor(ColorConstants.WHITE)
                       .Rectangle(85f, 146f, 445f, 49f)
                       .Fill()
                       .RestoreState();

                // 2. Cover the original payment terms at the top
                canvas4.SaveState()
                       .SetFillColor(ColorConstants.WHITE)
                       .Rectangle(85f, 648f, 445f, 48f)
                       .Fill()
                       .RestoreState();

                float boldWidth = fontBold.GetWidth("Payment Terms:", 9f);
                
                // Draw new payment terms at the top
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
