using iText.Commons.Actions;
using iText.Kernel.Font;
using iText.Kernel.Pdf.Canvas;
using iText.Kernel.Pdf.Event;
using iText.Kernel.Pdf;
using iText.IO.Font.Constants;

namespace GCTL.Core.Helpers
{
    public class PageNumberEventHandler : AbstractPdfDocumentEventHandler
    {
        private int _totalPages;

        public void SetTotalPages(int totalPages)
        {
            _totalPages = totalPages;
        }

        protected override void OnAcceptedEvent(AbstractPdfDocumentEvent @event)
        {
            PdfDocumentEvent docEvent = (PdfDocumentEvent)@event;
            PdfDocument pdf = docEvent.GetDocument();
            PdfPage page = docEvent.GetPage();
            int pageNumber = pdf.GetPageNumber(page);

            // Define margins and positions
            float marginLeft = 35;
            float marginRight = 100;
            float y = 20; // Vertical distance from the bottom

            // Calculate the x positions for both "Print Date" and "Page Number"
            float printDateX = marginLeft;
            float pageNumberX = pdf.GetDefaultPageSize().GetWidth() - marginRight;

            // Format the current date for "Print Date"
            string printDateText = $"Print Date: {DateTime.Now:yyyy-MM-dd HH:mm}";

            // Create canvas to draw text
            PdfCanvas canvas = new PdfCanvas(page.NewContentStreamAfter(), page.GetResources(), pdf);
            PdfFont font = PdfFontFactory.CreateFont(StandardFonts.HELVETICA);

            // Draw "Print Date" on the left
            canvas.BeginText()
                .SetFontAndSize(font, 10)
                .MoveText(printDateX, y)
                .ShowText(printDateText)
                .EndText();

            // Draw page number on the right
            canvas.BeginText()
                .SetFontAndSize(font, 10)
                .MoveText(pageNumberX, y)
                .ShowText($"Page {pageNumber} of {_totalPages}")
                .EndText()
                .Release();
        }
    }
}
