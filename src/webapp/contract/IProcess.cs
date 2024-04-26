﻿namespace webapp.Contract
{
    public interface IProcessor
    {
        Task<TResult> ExecuteAsync<TResult>(object query);
    }
}
