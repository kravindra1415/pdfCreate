using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using SelectPdf;

namespace WebApplication1.Controllers
{
    [Route("pdf")]
    public class PdfController : Controller
    {
        private readonly ICompositeViewEngine _compositeViewEngine;

        public PdfController(ICompositeViewEngine compositeViewEngine)
        {
            _compositeViewEngine = compositeViewEngine;
        }

        [Route("invoice")]
        public async Task<IActionResult> InvoiceAsync()
        {
            using (var stringWriter = new StringWriter())
            {
                var viewResult = _compositeViewEngine.FindView(ControllerContext, "_Invoice", false);

                if (viewResult.View == null)
                {
                    throw new ArgumentNullException("view can not be found");
                }

                var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary());

                var viewContext = new ViewContext(
                    ControllerContext, viewResult.View, viewDictionary, TempData, stringWriter, new HtmlHelperOptions()
                    );

                await viewResult.View.RenderAsync(viewContext);

                var htmlToPdf = new HtmlToPdf(1000, 1414);
                htmlToPdf.Options.DrawBackground = true;

                var pdf = htmlToPdf.ConvertHtmlString(stringWriter.ToString());
                var pdfBytes = pdf.Save();

                using (var streamWriter = new StreamWriter(@"F:\PdfGenerate\pdfStore\invoice.pdf"))
                {
                    await streamWriter.BaseStream.WriteAsync(pdfBytes, 0, pdfBytes.Length);
                }

                return File(pdfBytes, "application/pdf");
            }
        }

        //[Route("website")]
        //public async Task<IActionResult> WebsiteAsync()
        //{
        //    var mobileView = new HtmlToPdf();
        //    mobileView.Options.WebPageWidth = 480;

        //    var tabletView = new HtmlToPdf();
        //    mobileView.Options.WebPageWidth = 1024;

        //    var desktopView = new HtmlToPdf();
        //    mobileView.Options.WebPageWidth = 1920;

        //    var pdf = mobileView.ConvertUrl("https://www.roundthecode.com");
        //    pdf.Append(tabletView.ConvertUrl("https://www.roundthecode.com"));
        //    pdf.Append(desktopView.ConvertUrl("https://www.roundthecode.com"));

        //    var pdfBytes = pdf.Save();

        //    using (var streamWriter = new StreamWriter(@"F:\PdfGenerate\pdfStore\pdfData.pdf"))
        //    {
        //        await streamWriter.BaseStream.WriteAsync(pdfBytes, 0, pdfBytes.Length);
        //    }

        //    return File(pdfBytes, "application/pdf");

        //}
    }
}
