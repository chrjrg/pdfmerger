using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

class Program
{
    static void Main()
    {
        // Define input and output folder paths
        string inputFolder = Path.Combine(Directory.GetCurrentDirectory(), "input"); // Change to your input folder path
        string outputFolder = Path.Combine(Directory.GetCurrentDirectory(), "output"); // Change to your output folder path
        // Ensure output directory exists
        Directory.CreateDirectory(outputFolder);

        // Prompt the user for the output file name
        Console.WriteLine("Enter the desired output file name (without extension):");
        string? outputFileName = Console.ReadLine()?.Trim();
        if (string.IsNullOrWhiteSpace(outputFileName))
        {
            Console.WriteLine("Invalid file name. Using default name 'MergedDocument.pdf'.");
            outputFileName = "MergedDocument.pdf";
        }
        else
        {
            outputFileName += ".pdf";
        }


        // Ensure output directory exists
        Directory.CreateDirectory(outputFolder);

        // Get all PDF files from the input folder
        var pdfFiles = Directory.GetFiles(inputFolder, "*.pdf")
            .OrderBy(file => ExtractNumberFromFileName(Path.GetFileName(file)))
            .ToList();

        // Debug: Print the list of PDF files found
        Console.WriteLine("PDF files found:");
        pdfFiles.ForEach(file => Console.WriteLine(file));

        // Check if there are any PDF files to merge
        if (pdfFiles.Count == 0)
        {
            Console.WriteLine("No PDF files found in the input folder.");
            return;
        }

        // Merge the PDFs
        string outputFilePath = Path.Combine(outputFolder, outputFileName);
        MergePDFs(pdfFiles, outputFilePath);

        Console.WriteLine($"Merged PDF created at: {outputFilePath}");
    }

    // Extract number from file name
    static int ExtractNumberFromFileName(string fileName)
    {
        var number = new string(fileName.Where(char.IsDigit).ToArray());
        return int.TryParse(number, out int result) ? result : 0;
    }

    // Merge PDFs into a single PDF
    static void MergePDFs(List<string> pdfFiles, string outputFilePath)
    {
        using (PdfDocument outputDocument = new PdfDocument())
        {
            foreach (var pdfFile in pdfFiles)
            {
                using (PdfDocument inputDocument = PdfReader.Open(pdfFile, PdfDocumentOpenMode.Import))
                {
                    // Add pages from the input document to the output document
                    foreach (PdfPage page in inputDocument.Pages)
                    {
                        outputDocument.AddPage(page);
                    }
                }
            }

            // Save the merged document
            outputDocument.Save(outputFilePath);
        }
    }
}
