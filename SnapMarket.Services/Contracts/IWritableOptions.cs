using Microsoft.Extensions.Options;
using System;

namespace SnapMarket.Services.Contracts
{
    public interface IWritableOptions<out T> : IOptions<T> where T : class, new()
    {
        T Value { get; }
        void Update(Action<T> applyChanges);
    }
}
