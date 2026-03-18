using AutoMapper;
using DematecStock.Application.AutoMapper;
using Microsoft.Extensions.Logging.Abstractions;

namespace DematecStock.Tests;

/// <summary>
/// Cria um IMapper real usando MapperConfiguration — necessário no AutoMapper 16
/// que exige ILoggerFactory como segundo parâmetro.
/// </summary>
internal static class MapperFactory
{
    public static IMapper Create()
    {
        var config = new MapperConfiguration(
            cfg => cfg.AddProfile<AutoMapping>(),
            NullLoggerFactory.Instance);
        return config.CreateMapper();
    }
}
