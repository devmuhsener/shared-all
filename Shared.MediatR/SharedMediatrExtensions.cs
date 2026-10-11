using Microsoft.Extensions.DependencyInjection;
using Shared.MediatR.Behaviours;

namespace Shared.MediatR;

public static class SharedMediatrExtensions
{


    public static MediatRServiceConfiguration AddSharedValidationBehavior(this MediatRServiceConfiguration cfg)
    {
        cfg.AddOpenBehavior(typeof(ValidationBehaviour<,>));
        return cfg;
    }
}