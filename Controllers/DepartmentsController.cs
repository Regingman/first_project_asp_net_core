using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication3.Data;
using Rotativa;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using TemplateEngine.Docx;

namespace WebApplication3.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private static readonly string DOCX_FILE_MIME_TYPE = "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
        // private readonly IWebHostEnvironment environment;

        public DepartmentsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Departments.Include(d => d.Faculty);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Faculty)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        public IActionResult Create()
        {
            ViewData["Id_faculty"] = new SelectList(_context.Faculties, "Id", "Id");
            return View();
        }

        // POST: Departments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Id_faculty")] Department department)
        {
            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id_faculty"] = new SelectList(_context.Faculties, "Id", "Id", department.Id_faculty);
            return View(department);
        }

        // GET: Departments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            ViewData["Id_faculty"] = new SelectList(_context.Faculties, "Id", "Id", department.Id_faculty);
            return View(department);
        }

        // POST: Departments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Id_faculty")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["Id_faculty"] = new SelectList(_context.Faculties, "Id", "Id", department.Id_faculty);
            return View(department);
        }

        // GET: Departments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Faculty)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            _context.Departments.Remove(department);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }


        public IActionResult CSV()
        {
            var builder = new StringBuilder();
            var departments = _context.Departments.ToList();
            builder.AppendLine("Id,Name,FacultyName");
            foreach (var dep in departments)
            {
                builder.AppendLine($"{dep.Id},{dep.Name}");
            }
            return File(Encoding.UTF8.GetBytes(builder.ToString()), "text/csv", "Departments.csv");
        }

        public IActionResult Excel()
        {
            var departments = _context.Departments.ToList();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("Departments");
                var currentRow = 1;
                worksheet.Cell(currentRow, 1).Value = "Id";
                worksheet.Cell(currentRow, 2).Value = "Name";

                foreach (var dep in departments)
                {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = dep.Id;
                    worksheet.Cell(currentRow, 2).Value = dep.Name;
                }

                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();
                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        "DepartmentInfo.xlsx");

                }
            }
        }

        public IActionResult Pdf()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "template.docx");
            string templatePath = AppDomain.CurrentDomain.BaseDirectory + "temp.docx";
            System.IO.File.Copy(path, templatePath, true);

            var applicationDbContext = _context.Departments.Include(d => d.Faculty).ToList();
            var valuesToFill = GetContent(applicationDbContext);
            using (var outputDocument = new TemplateProcessor(templatePath)
                 .SetRemoveContentControls(true))
            {
                outputDocument.FillContent(valuesToFill);
                outputDocument.SaveChanges();

            }

            SautinSoft.Document.DocumentCore dc = SautinSoft.Document.DocumentCore.Load(templatePath);
            dc.Save(AppDomain.CurrentDomain.BaseDirectory + "DocToPDF.pdf");
            var bytes = System.IO.File.ReadAllBytes(AppDomain.CurrentDomain.BaseDirectory + "DocToPDF.pdf");
            
            string file_name = "DepartmentInfo.pdf";
            System.IO.File.Delete(templatePath);
            return File(bytes, "application/pdf", file_name);
        }

        public IActionResult Word()
        {
            using (MemoryStream mem = new MemoryStream())
            {
                using (WordprocessingDocument wordDoc = WordprocessingDocument.Create(mem, DocumentFormat.OpenXml.WordprocessingDocumentType.Document, true))
                {
                    wordDoc.AddMainDocumentPart();
                    // siga a ordem
                    Document doc = new Document();
                    Body body = new Body();

                    // 1 paragrafo
                    Paragraph para = new Paragraph();

                    ParagraphProperties paragraphProperties1 = new ParagraphProperties();
                    ParagraphStyleId paragraphStyleId1 = new ParagraphStyleId() { Val = "Normal" };
                    Justification justification1 = new Justification() { Val = JustificationValues.Center };
                    ParagraphMarkRunProperties paragraphMarkRunProperties1 = new ParagraphMarkRunProperties();

                    paragraphProperties1.Append(paragraphStyleId1);
                    paragraphProperties1.Append(justification1);
                    paragraphProperties1.Append(paragraphMarkRunProperties1);

                    Run run = new Run();
                    RunProperties runProperties1 = new RunProperties();

                    Text text = new Text() { Text = "The OpenXML SDK rocks!" };

                    // siga a ordem 
                    run.Append(runProperties1);
                    run.Append(text);
                    para.Append(paragraphProperties1);
                    para.Append(run);

                    // 2 paragrafo
                    Paragraph para2 = new Paragraph();

                    ParagraphProperties paragraphProperties2 = new ParagraphProperties();
                    ParagraphStyleId paragraphStyleId2 = new ParagraphStyleId() { Val = "Normal" };
                    Justification justification2 = new Justification() { Val = JustificationValues.Start };
                    ParagraphMarkRunProperties paragraphMarkRunProperties2 = new ParagraphMarkRunProperties();

                    paragraphProperties2.Append(paragraphStyleId2);
                    paragraphProperties2.Append(justification2);
                    paragraphProperties2.Append(paragraphMarkRunProperties2);

                    Run run2 = new Run();
                    RunProperties runProperties3 = new RunProperties();
                    Text text2 = new Text();
                    text2.Text = "Teste aqui";

                    run2.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Break());
                    run2.AppendChild(new Text("Hello"));
                    run2.AppendChild(new DocumentFormat.OpenXml.Wordprocessing.Break());
                    run2.AppendChild(new Text("world"));

                    para2.Append(paragraphProperties2);
                    para2.Append(run2);

                    // todos os 2 paragrafos no main body
                    body.Append(para);
                    body.Append(para2);

                    doc.Append(body);

                    wordDoc.MainDocumentPart.Document = doc;

                    wordDoc.Close();
                }
                return File(mem.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document", "ABC.docx");
            }
        }

        public IActionResult WordTwo()
        {
            string path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "template.docx");
            string templatePath = AppDomain.CurrentDomain.BaseDirectory + "temp.docx";
            System.IO.File.Copy(path, templatePath, true);

            var applicationDbContext = _context.Departments.Include(d => d.Faculty).ToList();
            var valuesToFill = GetContent(applicationDbContext);
            using (var outputDocument = new TemplateProcessor(templatePath)
                 .SetRemoveContentControls(true))
            {
                outputDocument.FillContent(valuesToFill);
                outputDocument.SaveChanges();

            }

            var bytes = System.IO.File.ReadAllBytes(templatePath);
            string file_name = "department.docx";
            System.IO.File.Delete(templatePath);
            return File(bytes, DOCX_FILE_MIME_TYPE, file_name);

        }


        private Content GetContent(List<Department> departments)
        {
            var studentsTable = new TableContent("departmentTable");
            for (int i = 0; i < departments.Count; i++)
            {
                studentsTable.AddRow(
                        new FieldContent("departmentId", departments[i].Id.ToString()),
                        new FieldContent("departmentName", departments[i].Name),
                        new FieldContent("departmentFacultyName", departments[i].Faculty.Name));
            }
            var valuesToFill = new Content(
                new FieldContent("name", "Department"),
                studentsTable,
                new FieldContent("count", departments.Count.ToString()),
                new FieldContent("date", DateTime.Now.Date.ToString()));
            return valuesToFill;
        }


        public IActionResult Bar()
        {
            var applicationDbContext = _context.Departments.Include(d => d.Faculty);

            var lstModel1 = _context.Faculties.Select(c =>
            new SimpleReportViewModel { DimensionOne = c.Name, Quantity = c.Departments.Count })
                .ToList();

            var lstModel2 = _context.Departments.Select(c =>
            new SimpleReportViewModel { DimensionOne = c.Faculty.Name, Quantity = c.Faculty.Departments.Count })
                .GroupBy(b => b.DimensionOne)
                .Select(b =>
                new SimpleReportViewModel { DimensionOne = b.Key, Quantity = b.Count() })
                .ToList();

            return View(lstModel2);
        }
    }
}


public class SimpleReportViewModel
{
    public string DimensionOne { get; set; }
    public int Quantity { get; set; }
}
