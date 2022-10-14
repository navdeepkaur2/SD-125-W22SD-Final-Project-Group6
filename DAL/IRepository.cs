namespace SD_340_W22SD_Final_Project_Group6.DAL;

public interface IRepository<T> where T : class
{
    void Create(T entity);

    T? FindById(int id);

    T? Find(Func<T, bool> predicate);

    ICollection<T> FindList(Func<T, bool> predicate);

    ICollection<T> GetAll();

    T Update(T entity);

    void Delete(T entity);

    void Save();
}
