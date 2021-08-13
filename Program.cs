

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

var services = new ServiceCollection();

services.TryAddEnumerable(ServiceDescriptor.Scoped(typeof(IProcessor<int>), typeof(IntProcessor)));
// Fix: ordering open generic above concretes
services.TryAddEnumerable(ServiceDescriptor.Scoped(typeof(IProcessor<>), typeof(AnyProcessor<>)));

var serviceProvider = services.BuildServiceProvider();

using var scope = serviceProvider.CreateAsyncScope();

// bug is reproducible only when below line enabled
var processor = scope.ServiceProvider.GetService<IProcessor<int>>();

var processors = scope.ServiceProvider.GetService<IEnumerable<IProcessor<int>>>() ?? Enumerable.Empty<IProcessor<int>>();

// bug?: prints "IntProcessor -- IntProcessor" instead of IntProcessor -- AnyProcessor`1 if line 17 commented.
Console.WriteLine(string.Join(" -- ", processors.Select(p => p.GetType().Name)));

interface IProcessor<T> { }

record AnyProcessor<T> : IProcessor<T>;

record IntProcessor : IProcessor<int>;

