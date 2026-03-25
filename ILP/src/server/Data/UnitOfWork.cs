using Microsoft.EntityFrameworkCore;
using server.Data.DbContexts;
using server.Data.Repositories;

namespace server.Data;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ICourseRepository Courses { get; }
    IModuleRepository Modules { get; }
    ILessonRepository Lessons { get; }
    IEnrollmentRepository Enrollments { get; }
    ILessonProgressRepository LessonProgresses { get; }
    IReviewRepository Reviews { get; }

    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}

public class UnitOfWork : IUnitOfWork
{
    private readonly BaseDbContext _context;
    private IUserRepository? _users;
    private ICourseRepository? _courses;
    private IModuleRepository? _modules;
    private ILessonRepository? _lessons;
    private IEnrollmentRepository? _enrollments;
    private ILessonProgressRepository? _lessonProgresses;
    private IReviewRepository? _reviews;

    private Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction? _transaction;

    public UnitOfWork(BaseDbContext context)
    {
        _context = context;
    }

    public IUserRepository Users => _users ??= new UserRepository(_context);
    public ICourseRepository Courses => _courses ??= new CourseRepository(_context);
    public IModuleRepository Modules => _modules ??= new ModuleRepository(_context);
    public ILessonRepository Lessons => _lessons ??= new LessonRepository(_context);
    public IEnrollmentRepository Enrollments => _enrollments ??= new EnrollmentRepository(_context);
    public ILessonProgressRepository LessonProgresses => _lessonProgresses ??= new LessonProgressRepository(_context);
    public IReviewRepository Reviews => _reviews ??= new ReviewRepository(_context);

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
            await _transaction.CommitAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
            await _transaction.RollbackAsync(cancellationToken);
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
