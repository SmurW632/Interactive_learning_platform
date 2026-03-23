namespace server.Data.Repositories;

public interface IRepository<T> : IDisposable where T : class
{
    Task<IEnumerable<T>> GetListAsync() { return GetListAsync(); }
    Task<T> GetAsync(Guid id) { return GetAsync(id); }
    void CreateAsync(T item) { }
    void UpdateAsync(T item) { }
    Task<bool> DeleteAsync(Guid id) { return DeleteAsync(id); }
    void SaveAsync();
}
