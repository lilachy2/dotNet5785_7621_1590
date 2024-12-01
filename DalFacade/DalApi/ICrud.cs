using DO;

namespace DalApi;

public interface ICrud<T> where T : class
{
    void Create(T item);
    T? Read(int id);  // stage1
    T? Read(Func<T, bool> filter); // stage 2

    //List<T> ReadAll(); // stage1
    IEnumerable<T> ReadAll(Func<T, bool>? filter = null); // stage 2

    void Update(T item);
    void Delete(int id);
    void DeleteAll();


}
