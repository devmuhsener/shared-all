using Shared.Core.Domain.Repositories;

namespace Shared.Activities.Domain;

public interface IActivityUnitOfWork : IUnitOfWork
{
    IUserActivityRepository Activities { get; }
}