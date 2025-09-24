public interface IObjectPool<T>
{
    // Obtiene un objeto del pool
    T Get();

    // Devuelve un objeto al pool
    void Release(T obj);
}