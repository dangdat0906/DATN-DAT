using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using HtmlToOpenXml;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace APP.UTILS
{
    public static class HtmlToWord
    {
        public static byte[] HtmlToWordMethod(String html)
        {
            const string filename = "test.docx";
            if (File.Exists(filename)) File.Delete(filename);

            using (MemoryStream generatedDocument = new MemoryStream())
            {
                using (WordprocessingDocument package = WordprocessingDocument.Create(
                       generatedDocument, WordprocessingDocumentType.Document))
                {
                    MainDocumentPart mainPart = package.MainDocumentPart;
                    if (mainPart == null)
                    {
                        mainPart = package.AddMainDocumentPart();
                        new Document(new Body()).Save(mainPart);
                    }
                    HtmlConverter converter = new HtmlConverter(mainPart);
                    var sections = mainPart.Document.Descendants<SectionProperties>();
                    Body body = mainPart.Document.Body;
                    SectionProperties sectProp = new SectionProperties();
                    PageSize pageSize = new PageSize()
                    { Width = 15840U, Height = 12240U, Orient = PageOrientationValues.Landscape };
                    PageMargin pageMargin = new PageMargin() { Top = 1440, Right = 1440U, Bottom = 1440, Left = 1440U };
                    Columns columns = new Columns() { Space = "720" };

                    DocGrid docGrid = new DocGrid() { LinePitch = 360 };

                    sectProp.Append(pageSize, pageMargin, columns, docGrid);
                    body.Append(sectProp);
                    var paragraphs = converter.Parse(html);
                    for (int i = 0; i < paragraphs.Count; i++)
                    {
                        body.Append(paragraphs[i]);
                    }
                    foreach (TableRow t in body.Descendants<TableRow>())
                    {
                        t.Append(new CantSplit());
                    }
                    mainPart.Document.Save();
                }
                return generatedDocument.ToArray();
            }
        }
        public static byte[] HtmlToWordMethod11(String html)
        {
            const string filename = "test.docx";
            if (File.Exists(filename)) File.Delete(filename);

            using (MemoryStream generatedDocument = new MemoryStream())
            {
                using (WordprocessingDocument package = WordprocessingDocument.Create(
                       generatedDocument, WordprocessingDocumentType.Document))
                {
                    
                    MainDocumentPart mainPart = package.MainDocumentPart;
                    if (mainPart == null)
                    {
                        mainPart = package.AddMainDocumentPart();
                        new Document(new Body()).Save(mainPart);
                    }
                    HtmlConverter converter = new HtmlConverter(mainPart);
                    var sections = mainPart.Document.Descendants<SectionProperties>();

                    Body body = mainPart.Document.Body;
                    SectionProperties sectProp = new SectionProperties();
                    PageSize pageSize = new PageSize()
                    { Width = 15840U, Height = 12240U, Orient = PageOrientationValues.Landscape };
                    PageMargin pageMargin = new PageMargin() { Top = 1440, Right = 1440U, Bottom = 1440, Left = 1440U };
                    Columns columns = new Columns() { Space = "720" };
                    DocGrid docGrid = new DocGrid() { LinePitch = 360 };
                    sectProp.Append(pageSize, pageMargin, columns, docGrid);
                    body.Append(sectProp);
                    var paragraphs = converter.Parse(html);
                    for (int i = 0; i < paragraphs.Count; i++)
                    {
                        body.Append(paragraphs[i]);
                    }
                    foreach (TableRow t in body.Descendants<TableRow>())
                    {
                        t.Append(new CantSplit());
                    }
                    StyleDefinitionsPart stylesPart = null;
                    stylesPart = mainPart.StyleDefinitionsPart;
                    ChangeStyleDefinitionsPart1(stylesPart);
                    mainPart.Document.Save();
                }
                return generatedDocument.ToArray();
            }        
        }
        private static void ChangeStyleDefinitionsPart1(StyleDefinitionsPart styleDefinitionsPart1)
        {
            Styles styles1 = styleDefinitionsPart1.Styles;
            Style style1 = styles1.GetFirstChild<Style>(); //get the specifc style
            Rsid rsid1 = new Rsid() { Val = "00B10D4B" };
            style1.Append(rsid1);
            StyleRunProperties styleRunProperties1 = new StyleRunProperties();
            var runFont = new RunFonts { Ascii = "Time New Roman" };
            FontSize fontSize1 = new FontSize() { Val = "28" };
            styleRunProperties1.Append(fontSize1, runFont);
            style1.Append(styleRunProperties1);
        }
    }
}
