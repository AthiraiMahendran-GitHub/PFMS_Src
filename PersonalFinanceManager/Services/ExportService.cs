using ClosedXML.Excel;
using PersonalFinanceManager.Data;

namespace PersonalFinanceManager.Services;

public class ExportService
{
    private readonly ExpenseService _expenseService;
    private readonly InvestmentService _investmentService;
    private readonly TaskService _taskService;
    private readonly NoteService _noteService;

    public ExportService(
        ExpenseService expenseService,
        InvestmentService investmentService,
        TaskService taskService,
        NoteService noteService)
    {
        _expenseService = expenseService;
        _investmentService = investmentService;
        _taskService = taskService;
        _noteService = noteService;
    }

    public async Task<byte[]> ExportAllDataToExcelAsync()
    {
        using var workbook = new XLWorkbook();

        // Export Expenses
        var expenses = await _expenseService.GetAllAsync();
        var expenseSheet = workbook.Worksheets.Add("Expenses");
        expenseSheet.Cell(1, 1).Value = "Date";
        expenseSheet.Cell(1, 2).Value = "Category";
        expenseSheet.Cell(1, 3).Value = "Amount";
        expenseSheet.Cell(1, 4).Value = "Description";
        
        for (int i = 0; i < expenses.Count; i++)
        {
            var row = i + 2;
            expenseSheet.Cell(row, 1).Value = expenses[i].Date.ToString("yyyy-MM-dd");
            expenseSheet.Cell(row, 2).Value = expenses[i].Category;
            expenseSheet.Cell(row, 3).Value = expenses[i].Amount;
            expenseSheet.Cell(row, 4).Value = expenses[i].Description;
        }
        expenseSheet.Columns().AdjustToContents();

        // Export Investments
        var investments = await _investmentService.GetAllAsync();
        var investmentSheet = workbook.Worksheets.Add("Investments");
        investmentSheet.Cell(1, 1).Value = "Name";
        investmentSheet.Cell(1, 2).Value = "Type";
        investmentSheet.Cell(1, 3).Value = "Initial Amount";
        investmentSheet.Cell(1, 4).Value = "Current Value";
        investmentSheet.Cell(1, 5).Value = "Profit/Loss";
        investmentSheet.Cell(1, 6).Value = "ROI %";
        investmentSheet.Cell(1, 7).Value = "Purchase Date";
        
        for (int i = 0; i < investments.Count; i++)
        {
            var row = i + 2;
            investmentSheet.Cell(row, 1).Value = investments[i].Name;
            investmentSheet.Cell(row, 2).Value = investments[i].Type;
            investmentSheet.Cell(row, 3).Value = investments[i].InitialAmount;
            investmentSheet.Cell(row, 4).Value = investments[i].CurrentValue ?? investments[i].InitialAmount;
            investmentSheet.Cell(row, 5).Value = investments[i].ProfitLoss;
            investmentSheet.Cell(row, 6).Value = investments[i].ROIPercentage;
            investmentSheet.Cell(row, 7).Value = investments[i].PurchaseDate.ToString("yyyy-MM-dd");
        }
        investmentSheet.Columns().AdjustToContents();

        // Export Tasks
        var tasks = await _taskService.GetAllAsync();
        var taskSheet = workbook.Worksheets.Add("Tasks");
        taskSheet.Cell(1, 1).Value = "Company";
        taskSheet.Cell(1, 2).Value = "Title";
        taskSheet.Cell(1, 3).Value = "Status";
        taskSheet.Cell(1, 4).Value = "Priority";
        taskSheet.Cell(1, 5).Value = "Due Date";
        taskSheet.Cell(1, 6).Value = "Description";
        
        for (int i = 0; i < tasks.Count; i++)
        {
            var row = i + 2;
            taskSheet.Cell(row, 1).Value = tasks[i].Company;
            taskSheet.Cell(row, 2).Value = tasks[i].Title;
            taskSheet.Cell(row, 3).Value = tasks[i].Status.ToString();
            taskSheet.Cell(row, 4).Value = tasks[i].Priority.ToString();
            taskSheet.Cell(row, 5).Value = tasks[i].DueDate?.ToString("yyyy-MM-dd") ?? "";
            taskSheet.Cell(row, 6).Value = tasks[i].Description;
        }
        taskSheet.Columns().AdjustToContents();

        // Export Notes
        var notes = await _noteService.GetAllAsync();
        var noteSheet = workbook.Worksheets.Add("Notes");
        noteSheet.Cell(1, 1).Value = "Category";
        noteSheet.Cell(1, 2).Value = "Content";
        noteSheet.Cell(1, 3).Value = "Created At";
        
        for (int i = 0; i < notes.Count; i++)
        {
            var row = i + 2;
            noteSheet.Cell(row, 1).Value = notes[i].Category;
            noteSheet.Cell(row, 2).Value = notes[i].Content;
            noteSheet.Cell(row, 3).Value = notes[i].CreatedAt.ToString("yyyy-MM-dd HH:mm");
        }
        noteSheet.Columns().AdjustToContents();

        // Style all headers
        foreach (var sheet in workbook.Worksheets)
        {
            var headerRange = sheet.Range(1, 1, 1, sheet.LastColumnUsed().ColumnNumber());
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public async Task<byte[]> ExportExpensesToExcelAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var expenses = startDate.HasValue || endDate.HasValue
            ? await _expenseService.GetByDateRangeAsync(startDate ?? DateTime.MinValue, endDate ?? DateTime.MaxValue)
            : await _expenseService.GetAllAsync();

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("Expenses");
        
        sheet.Cell(1, 1).Value = "Date";
        sheet.Cell(1, 2).Value = "Category";
        sheet.Cell(1, 3).Value = "Amount";
        sheet.Cell(1, 4).Value = "Description";
        
        for (int i = 0; i < expenses.Count; i++)
        {
            var row = i + 2;
            sheet.Cell(row, 1).Value = expenses[i].Date.ToString("yyyy-MM-dd");
            sheet.Cell(row, 2).Value = expenses[i].Category;
            sheet.Cell(row, 3).Value = expenses[i].Amount;
            sheet.Cell(row, 4).Value = expenses[i].Description;
        }
        
        sheet.Columns().AdjustToContents();
        var headerRange = sheet.Range(1, 1, 1, 4);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
