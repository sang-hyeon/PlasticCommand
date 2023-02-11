﻿#pragma warning disable
#nullable enable
namespace Templates // replace: to {{ Namespace }}
{
    using Microsoft.Extensions.DependencyInjection;
    using PlasticCommand;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Generator = PlasticCommand.Generator;
    using TargetCommandSpec = PlasticCommand.Generator.TTFFCommandSpec;

    // replace: internal to public
    internal interface TTFFGeneratedCommandInterface : ICommandSpecification<Generator.TTFFParameter, Generator.TTFFResult>
    {
        // add: {{ Comment }}
        Task<Generator.TTFFResult> ExecuteAsync(
            Generator.TTFFParameter param, CancellationToken token = default);
    }
    
    // replace: internal to public
    internal class TTFFCommand : TTFFGeneratedCommandInterface
    {
        private readonly IServiceProvider _provider;

        public TTFFCommand(IServiceProvider provider)
        {
            this._provider = provider;
        }

        public async Task<Generator.TTFFResult> ExecuteAsync(
            Generator.TTFFParameter param, CancellationToken token = default)
        {
            Generator.TTFFResult response;
            using (IServiceScope scope = this._provider.CreateScope())
            {
                BuildPipeline? pipelineBuilder = scope.ServiceProvider.GetService<BuildPipeline>();
                IEnumerable<IPipe> pipeline = pipelineBuilder?.Invoke(scope.ServiceProvider) ?? Array.Empty<IPipe>();
                pipeline = pipeline.Reverse();
                PipelineContext context = CreatePipelineContext(param, scope.ServiceProvider);

                Behavior command = CreateCommandAsBehavior(scope.ServiceProvider, param, token);
                Behavior process =
                    pipeline.Aggregate(command, (next, pipe) => () => pipe.Handle(context, next, token));

                response = (Generator.TTFFResult)await process.Invoke().ConfigureAwait(false);
            }

            return response;
        }

        private static PipelineContext CreatePipelineContext(
            Generator.TTFFParameter param, IServiceProvider provider)
        {
            object?[] services = new object?[]
            {
                provider.GetService<IServiceProvider>() // replace: on template
            };

            object[] servicesFound = services.OfType<object>().ToArray();

            return new PipelineContext(param, typeof(TargetCommandSpec), servicesFound);
        }

        private static Behavior CreateCommandAsBehavior(
            IServiceProvider provider, Generator.TTFFParameter param, CancellationToken token = default)
        {
            return new Behavior(() =>
            {
                TargetCommandSpec spec = provider.GetRequiredService<TargetCommandSpec>();
                return spec.ExecuteAsync(param, token)
                            .ContinueWith(task => (object?)task.Result, TaskContinuationOptions.ExecuteSynchronously);
            });
        }
    }
}
#nullable disable
#pragma warning restore
