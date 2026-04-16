using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;

namespace PersonalFinanceManager.Services;

public class TaskService
{
    private readonly AppDbContext _context;

    public TaskService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<DailyTask>> GetAllAsync()
    {
        return await _context.Tasks
            .OrderBy(t => t.DueDate)
            .ThenByDescending(t => t.Priority)
            .ToListAsync();
    }

    public async Task<List<DailyTask>> GetByCompanyAsync(string company)
    {
        return await _context.Tasks
            .Where(t => t.Company == company)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<List<DailyTask>> GetByStatusAsync(Data.TaskStatus status)
    {
        return await _context.Tasks
            .Where(t => t.Status == status)
            .OrderBy(t => t.DueDate)
            .ToListAsync();
    }

    public async Task<DailyTask?> GetByIdAsync(int id)
    {
        return await _context.Tasks.FindAsync(id);
    }

    public async Task<DailyTask> CreateAsync(DailyTask task)
    {
        _context.Tasks.Add(task);
        await _context.SaveChangesAsync();
        return task;
    }

    public async Task UpdateAsync(DailyTask task)
    {
        if (task.Status == Data.TaskStatus.Completed && task.CompletedAt == null)
        {
            task.CompletedAt = DateTime.Now;
        }
        
        _context.Tasks.Update(task);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var task = await _context.Tasks.FindAsync(id);
        if (task != null)
        {
            _context.Tasks.Remove(task);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<Dictionary<Data.TaskStatus, int>> GetTaskCountByStatusAsync()
    {
        return await _context.Tasks
            .GroupBy(t => t.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);
    }

    public async Task<List<string>> GetCompaniesAsync()
    {
        return await _context.Tasks
            .Select(t => t.Company)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    // SubTask methods
    public async Task<List<SubTask>> GetSubTasksAsync(int parentTaskId)
    {
        return await _context.SubTasks
            .Where(st => st.ParentTaskId == parentTaskId)
            .OrderBy(st => st.CreatedAt)
            .ToListAsync();
    }

    public async Task<SubTask> CreateSubTaskAsync(SubTask subTask)
    {
        _context.SubTasks.Add(subTask);
        await _context.SaveChangesAsync();
        return subTask;
    }

    public async Task UpdateSubTaskAsync(SubTask subTask)
    {
        if (subTask.IsCompleted && subTask.CompletedAt == null)
        {
            subTask.CompletedAt = DateTime.Now;
        }
        
        _context.SubTasks.Update(subTask);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteSubTaskAsync(int id)
    {
        var subTask = await _context.SubTasks.FindAsync(id);
        if (subTask != null)
        {
            _context.SubTasks.Remove(subTask);
            await _context.SaveChangesAsync();
        }
    }
}
