using AI.Mistrel.model.Service.Interface;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;

namespace AI.Mistrel.model.Service.Implement
{
    public class ResumeParser : IResumeParser
    {
        public async Task<string> ExtractTextFromFileAsync(IFormFile file)
        {
            var extension = System.IO.Path.GetExtension(file.FileName).ToLowerInvariant();
            
            return extension switch
            {
                ".pdf" => await ExtractTextFromPdfAsync(file),

                ///////FILHAL YE NH CHAL RHA ONLY PDF WORKING /////

                ".doc" or ".docx" => await ExtractTextFromWordAsync(file),
                _ => throw new NotSupportedException($"File type {extension} is not supported")
            };
        }

        private async Task<string> ExtractTextFromPdfAsync(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var pdfReader = new PdfReader(memoryStream);
            var text = new System.Text.StringBuilder();

            for (int page = 1; page <= pdfReader.NumberOfPages; page++)
            {
                text.AppendLine(PdfTextExtractor.GetTextFromPage(pdfReader, page));
            }

            return text.ToString();
        }



        ///FILHAL YE NH CHAL RHA ONLY PDF WORKING ... 

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<string> ExtractTextFromWordAsync(IFormFile file)
        {
            using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream);
            memoryStream.Position = 0;

            using var document = WordprocessingDocument.Open(memoryStream, false);
            var body = document.MainDocumentPart?.Document?.Body;
            
            if (body == null) return string.Empty;

            var text = new System.Text.StringBuilder();
            foreach (var paragraph in body.Elements<Paragraph>())
            {
                text.AppendLine(paragraph.InnerText);
            }

            return text.ToString();
        }
    }
}
