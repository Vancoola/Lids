using Microsoft.OpenApi;

namespace Application.Extensions;

public static class DocsExtensions
{
    public static IServiceCollection AddDocs(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddOpenApi(options =>
        {
            options.AddOperationTransformer((operation, context, _) =>
            {
                var method = context.Description.HttpMethod;

                if (method is not ("POST" or "PUT" or "PATCH" or "DELETE"))
                    return Task.CompletedTask;

                operation.Parameters ??= [];

                operation.Parameters.Add(new OpenApiParameter
                {
                    Name = "X-CSRF-TOKEN",
                    In = ParameterLocation.Header,
                    Required = true,
                    Description = "CSRF-токен, полученный через GET /api/auth/csrf"
                });

                return Task.CompletedTask;
            });
        });
        return serviceCollection;
    }
}