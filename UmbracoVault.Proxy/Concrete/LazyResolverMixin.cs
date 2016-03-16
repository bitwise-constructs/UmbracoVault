using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Umbraco.Core.Models;
using Umbraco.Web;

namespace UmbracoVault.Proxy.Concrete
{
    /// <summary>
    ///     Mixed into proxy object to provide lazy loading of values based on data provided by IPublishedContent node
    /// </summary>
    public class LazyResolverMixin : ILazyResolverMixin
    {
        private readonly IPublishedContent _node;
        private readonly Dictionary<string, object> _valueCache = new Dictionary<string, object>();
        private readonly IUmbracoContext _umbracoContext;

        public LazyResolverMixin(IPublishedContent node)
        {
            _node = node;
            _umbracoContext = Vault.Context;
        }

        public object GetOrResolve(string alias, PropertyInfo propertyInfo)
        {
            object value;
            if (!_valueCache.TryGetValue(alias, out value))
            {
                _umbracoContext.TryGetValueForProperty(
                    (propAlias, recursive) => ResolveValue(propAlias, recursive, _node),
                    propertyInfo,
                    out value);

                _valueCache.Add(alias, value);
            }
            else
            {
                Console.WriteLine("From cache");
            }

            return value;
        }

        private object ResolveValue(string alias, bool recursive, IPublishedContent node)
        {
            return node.GetPropertyValue(alias, recursive);
        }
    }
}