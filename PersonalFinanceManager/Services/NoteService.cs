using Microsoft.EntityFrameworkCore;
using PersonalFinanceManager.Data;

namespace PersonalFinanceManager.Services;

public class NoteService
{
    private readonly AppDbContext _context;

    public NoteService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<Note>> GetAllAsync()
    {
        return await _context.Notes
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<Note>> GetByCategoryAsync(string category)
    {
        return await _context.Notes
            .Where(n => n.Category == category)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }

    public async Task<Note?> GetByIdAsync(int id)
    {
        return await _context.Notes.FindAsync(id);
    }

    public async Task<Note> CreateAsync(Note note)
    {
        _context.Notes.Add(note);
        await _context.SaveChangesAsync();
        return note;
    }

    public async Task UpdateAsync(Note note)
    {
        note.UpdatedAt = DateTime.Now;
        _context.Notes.Update(note);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var note = await _context.Notes.FindAsync(id);
        if (note != null)
        {
            _context.Notes.Remove(note);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<string>> GetCategoriesAsync()
    {
        return await _context.Notes
            .Select(n => n.Category)
            .Distinct()
            .OrderBy(c => c)
            .ToListAsync();
    }

    public async Task<List<Note>> SearchAsync(string searchTerm)
    {
        return await _context.Notes
            .Where(n => n.Content.Contains(searchTerm) || n.Category.Contains(searchTerm))
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();
    }
}
