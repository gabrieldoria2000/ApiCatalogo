﻿using System.Linq.Expressions;

namespace APICatalogo.Repository;

public interface IRepository<T>
{
    //Adiciona os MÉTODOS GENÉRICOS

    IQueryable<T> Get();
    T GetById(Expression<Func<T, bool>> predicate);
    void Add(T entity);
    void Update(T entity);
    void Delete(T entity);

}
